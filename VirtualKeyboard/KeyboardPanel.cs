using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VirtualKeyboard.Data;

namespace VirtualKeyboard
{
    public class KeyboardPanel : Panel
    {
        private SortedDictionary<int, List<FrameworkElement>> _keyboardElements;
        private Dictionary<int, Scale> _keysHeightScales;
        private bool _isChildChanged;
        private bool _isStart;

        static KeyboardPanel() 
        {
            FocusableProperty.OverrideMetadata(
                forType: typeof(KeyboardPanel),
                typeMetadata: new FrameworkPropertyMetadata(false));
        }

        public KeyboardPanel()
        {           
            _keyboardElements = new SortedDictionary<int, List<FrameworkElement>>();
            _keysHeightScales = new Dictionary<int, Scale>();
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

                _isStart = false;
                _isChildChanged = false;
            }

            SetKeysHeightScales(availableSize.Height);

            int rowIndex = 0;

            foreach (KeyValuePair<int, List<FrameworkElement>> pair in _keyboardElements)
            {
                Scale widthScale = GetKeysWidthScale(availableSize.Width, rowIndex);

                for (int i = 0; i < pair.Value.Count; i++)
                {
                    KeyMetadata data = Keyboard.GetKeyMetadata(pair.Value[i]);

                    Size keySize = GetKeySize(data, widthScale, _keysHeightScales[i], pair.Value[i]);

                    pair.Value[i].Height = keySize.Height;
                    pair.Value[i].Width = keySize.Width;

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

            List<double> topOffSets = new(_keyboardElements.Max(pair => pair.Value.Count));

            for (int i = 0; i < topOffSets.Capacity; i++)
            {
                topOffSets.Add(0.0d);
            }

            foreach (KeyValuePair<int, List<FrameworkElement>> pair in _keyboardElements)
            {
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
                FrameworkElement bufferKey = key;

                if (key is ContentPresenter)
                {
                    ((ContentPresenter)key).ApplyTemplate();

                    bufferKey = ((ContentPresenter)key).ContentTemplate.FindName("key", key) as FrameworkElement;
                }                

                KeyMetadata data = Keyboard.GetKeyMetadata(bufferKey);

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

                bufferKey.Height *= data.HeightScale;
                bufferKey.Width *= data.WidthScale;

                _keyboardElements[data.Row].Add(bufferKey);
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
                _keysHeightScales[i] = GetKeysHeightScale(keyboardPanelHeight, i);
            }
        }

        private Scale GetKeysHeightScale(double keyboardPanelHeight, int columnIndex)
        {
            double keysHeight = _keyboardElements.Sum(key =>
            {
                if (key.Value.Count <= columnIndex)
                {
                    return key.Value[key.Value.Count - 1].Height;
                }

                return key.Value[columnIndex].Height;
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
                return key.Width;
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
            Size keySize = new(key.Width, key.Height);

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
                keySize.Height += heightScale.Number;
            }
            else
            {
                keySize.Height -= heightScale.Number;
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
