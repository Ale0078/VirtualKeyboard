using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using static VirtualKeyboard.Functions.User32;

using VirtualKeyboard.Enums;
using VirtualKeyboard.Structs;

namespace VirtualKeyboard
{
    [TemplatePart(Name = BOARD_CONTAINER, Type = typeof(Grid))]
    public class Keyboard : Control
    {
        private const string BOARD_CONTAINER = "PART_boardContainer";

        private const int NUMPAD_ROWS = 4;
        private const int NUMPAD_COLUMNS = 3;

        private const int KEY_BOARD_ROWS = 4;
        private const int KEY_BOARD_COLUMNS = 13;

        private Grid _boardContainer;

        private readonly Grid _numpad;
        private readonly Grid _keyBoard;

        public static readonly DependencyProperty HasNumpadProperty;
        public static readonly DependencyProperty HasKeyBoardProperty;

        static Keyboard()
        {
            MouseLeftButtonDownEvent.AddOwner(typeof(Keyboard));

            HasNumpadProperty = DependencyProperty.Register(
                name: nameof(HasNumpad),
                propertyType: typeof(bool),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(true));

            HasKeyBoardProperty = DependencyProperty.Register(
                name: nameof(HasKeyBoard),
                propertyType: typeof(bool),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(false));//ToDo: switch to true
        }

        public Keyboard()
        {
            Focusable = false;

            _numpad = new Grid();
            _keyBoard = new Grid();

            FillNumpad();
        }

        public bool HasNumpad 
        {
            get => (bool)GetValue(HasNumpadProperty);
            set => SetValue(HasNumpadProperty, value);
        }

        public bool HasKeyBoard 
        {
            get => (bool)GetValue(HasKeyBoardProperty);
            set => SetValue(HasKeyBoardProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _boardContainer = GetTemplateChild(BOARD_CONTAINER) as Grid;
            _boardContainer.Children.Add(_numpad);//ToDo: do it after changing hasnumap or haskeyboard property
        }


        private void FillNumpad() //ToDo: Add margin to buttons
        {
            GenerateRowAndColumn(_numpad, NUMPAD_ROWS, NUMPAD_COLUMNS);

            SetZeroPointToNumpad();

            uint number = 0x31;

            for (int i = _numpad.RowDefinitions.Count - 2; i >= 0; i--)
            {
                for (int j = 0; j < _numpad.ColumnDefinitions.Count; j++)
                {
                    RepeatButton numberButton = new()
                    {
                        Content = Convert.ToChar(number),
                        DataContext = (VirtualKeyShort)number,
                        Margin = new Thickness(5),
                        Focusable = false
                    };

                    numberButton.Click += ClickRepeatButton;

                    _numpad.Children.Add(numberButton);

                    SetRowColumnPosition(numberButton, i, j);

                    number++;
                }
            }
        }

        private void SetZeroPointToNumpad() 
        {
            RepeatButton point = new()
            {
                Content = '.',
                DataContext = VirtualKeyShort.OEM_PERIOD,
                Margin = new Thickness(5),
                Focusable = false
            };

            point.Click += ClickRepeatButton;

            SetRowColumnPosition(point, _numpad.RowDefinitions.Count - 1, _numpad.ColumnDefinitions.Count - 1);

            _numpad.Children.Add(point);

            RepeatButton zero = new()
            {
                Content = '0',
                DataContext = VirtualKeyShort.KEY_0,
                Margin = new Thickness(5),
                Focusable = false
            };

            zero.Click += ClickRepeatButton;

            Grid.SetColumnSpan(zero, 2);
            Grid.SetRow(zero, _numpad.RowDefinitions.Count - 1);

            _numpad.Children.Add(zero);
        }

        private void FillKeyBoard() 
        {
            GenerateRowAndColumn(_keyBoard, KEY_BOARD_ROWS, KEY_BOARD_COLUMNS);


        }

        private void GenerateRowAndColumn(Grid field, int rowCount, int columnCount) 
        {
            for (int i = 0; i < rowCount; i++)
            {
                field.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columnCount; i++)
            {
                field.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private void ClickRepeatButton(object sender, RoutedEventArgs e) 
        {
            INPUT[] inputs = new INPUT[2];

            VirtualKeyShort key = (VirtualKeyShort)((Control)sender).DataContext;

            inputs[0] = new INPUT()
            {
                type = (uint)InputEventType.INPUT_KEYBOARD,
                U = new InputUnion()
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key
                    }
                }
            };

            //if (key == VirtualKeyShort.CAPITAL)
            //{
            //    inputs[1] = new INPUT()
            //    {
            //        type = (uint)InputEventType.INPUT_KEYBOARD,
            //        U = new InputUnion()
            //        {
            //            ki = new KEYBDINPUT
            //            {
            //                wVk = key,
            //                dwFlags = KEYEVENTF.KEYUP
            //            }
            //        }
            //    };

            //    SendInput(2, inputs, INPUT.Size);

            //    return;
            //}

            SendInput(1, inputs, INPUT.Size);
        }

        private void SetRowColumnPosition(UIElement element, int rowPosition, int columnPosition) 
        {
            Grid.SetColumn(element, columnPosition);
            Grid.SetRow(element, rowPosition);
        }
    }
}
