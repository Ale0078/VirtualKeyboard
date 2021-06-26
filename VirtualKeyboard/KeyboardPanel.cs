using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using VirtualKeyboard.Data;

namespace VirtualKeyboard
{
    public class KeyboardPanel : Panel
    {
        private SortedDictionary<int, List<FrameworkElement>> _keyboardElements;
        private List<Scale> _keysHeightScales;
        private bool _isChildChanged;
        private bool _isStart;

        public KeyboardPanel()
        {           
            _keyboardElements = new SortedDictionary<int, List<FrameworkElement>>();
            _keysHeightScales = new List<Scale>();
            _isStart = true;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            _isChildChanged = visualRemoved is not null || visualAdded is not null;

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return base.MeasureOverride(availableSize);
            }

            if (_isChildChanged || _isStart)
            {
                SetKeyboardElements();
                SetKeysHeightScales(availableSize.Height);

                _isStart = false;
                _isChildChanged = false;
            }

            int rowIndex = 0;

            foreach (KeyValuePair<int, List<FrameworkElement>> pair in _keyboardElements)
            {
                Scale widthScale = GetKeysWidthScale(availableSize.Width, rowIndex);

                for (int i = 0; i < pair.Value.Count; i++)
                {
                    KeyMetadata data = Keyboard.GetKeyMetadata(pair.Value[i]);

                    Size keySize = GetKeySize(data, widthScale, _keysHeightScales[i], pair.Value[i]);

                    pair.Value[i].Measure(keySize);
                }

                rowIndex++;
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
            {
                return base.ArrangeOverride(finalSize);
            }

            double leftOffSet = 0;
            //double topOffSet = 0;

            List<double> topOffSets = new(_keyboardElements.Max(pair => pair.Value.Count));

            foreach (KeyValuePair<int, List<FrameworkElement>> pair in _keyboardElements)
            {
                //foreach (FrameworkElement key in pair.Value)
                //{
                //    key.Arrange(new Rect(new Point(leftOffSet, topOffSet), key.DesiredSize));

                //    leftOffSet += key.Width;
                //}
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    pair.Value[i].Arrange(new Rect(
                        location: new Point(leftOffSet, topOffSets[i]),
                        size: pair.Value[i].DesiredSize));

                    topOffSets[i] += pair.Value[i].Height;
                    leftOffSet += pair.Value[i].Width;
                }

                leftOffSet = 0;
            }

            return finalSize;
        }

        private void SetKeyboardElements() 
        {
            foreach (FrameworkElement key in Children)
            {
                KeyMetadata data = Keyboard.GetKeyMetadata(key);

                if (!_keyboardElements.ContainsKey(data.Row))
                {
                    _keyboardElements.Add(data.Row, new List<FrameworkElement>());
                }

                FrameworkElement repeatedKey = _keyboardElements[data.Row].Find(compareKey =>
                {
                    KeyMetadata compareKeyData = Keyboard.GetKeyMetadata(compareKey);

                    if (compareKeyData.Row == data.Row && compareKeyData.Column == data.Column)
                    {
                        return true;
                    }

                    return false;
                });

                if (repeatedKey is not null)
                {
                    _keyboardElements[data.Row].Remove(repeatedKey);
                }

                _keyboardElements[data.Row].Add(key);
            }

            foreach (KeyValuePair<int, List<FrameworkElement>> pair in _keyboardElements)
            {
                pair.Value.Sort(new KeyColumnComparer());
            }
        }

        private void SetKeysHeightScales(double keyboardPanelHeight) 
        {
            for (int i = 0; i < _keyboardElements.First().Value.Count; i++)
            {
                _keysHeightScales.Add(GetKeysHeightScale(keyboardPanelHeight, i));
            }
        }

        private Scale GetKeysHeightScale(double keyboardPanelHeight, int columnIndex) 
        {
            double keysHeight = _keyboardElements.Sum(pair =>
            {
                KeyMetadata data = Keyboard.GetKeyMetadata(pair.Value[columnIndex]);

                return pair.Value[columnIndex].Height * data.HeightScale;
            });

            Scale scale = new();

            if (keyboardPanelHeight > keysHeight)
            {
                scale.IsPositive = true;
                scale.Number = (keyboardPanelHeight - keysHeight) / _keyboardElements.Count;
            }
            else
            {
                scale.IsPositive = false;
                scale.Number = (keysHeight - keyboardPanelHeight) / _keyboardElements.Count;
            }

            return scale;
        }

        private Scale GetKeysWidthScale(double keyboardPanelWidth, int rowIndex) 
        {
            double keysWidth = _keyboardElements[rowIndex].Sum(key =>
            {
                KeyMetadata data = Keyboard.GetKeyMetadata(key);

                return key.Width * data.WidthScale;
            });

            Scale scale = new();

            if (keyboardPanelWidth > keysWidth)
            {
                scale.IsPositive = true;
                scale.Number = (keyboardPanelWidth - keysWidth) / _keyboardElements[rowIndex].Count;
            }
            else
            {
                scale.IsPositive = false;
                scale.Number = (keysWidth - keyboardPanelWidth) / _keyboardElements[rowIndex].Count;
            }

            return scale;
        }

        private Size GetKeySize(KeyMetadata data, Scale widthScale, Scale heightScale, FrameworkElement key) 
        {
            Size keySize = new(
                width: key.Width * data.WidthScale,
                height: key.Height * data.HeightScale);

            if (widthScale.IsPositive)
            {
                keySize.Width += widthScale.Number;
            }
            else 
            {
                keySize.Width -= widthScale.Number;
            }

            if (heightScale.IsPositive)
            {
                keySize.Width += heightScale.Number;
            }
            else
            {
                keySize.Width -= heightScale.Number;
            }

            return keySize;
        }



        private class KeyColumnComparer : IComparer<FrameworkElement>
        {
            public int Compare(FrameworkElement x, FrameworkElement y)
            {
                KeyMetadata xData = Keyboard.GetKeyMetadata(x);
                KeyMetadata yData = Keyboard.GetKeyMetadata(y);

                return xData.Column.CompareTo(yData.Column);
            }
        }

        private struct Scale
        {
            public bool IsPositive { get; set; }
            public double Number { get; set; }
        }
    }
}
