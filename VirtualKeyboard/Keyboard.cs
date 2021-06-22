﻿using System;
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
using VirtualKeyboard.Data;

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

        private Thickness _buttonMargen = new Thickness(5);//ToDo: Set Margen to buttons from user
        private double DEFAULT_BUTTON_WIDTH = 60;
        private double DEFAULT_BUTTON_HEUGHT = 60;

        private Grid _boardContainer;

        private readonly Grid _numpad;
        private readonly Grid _keyBoard;

        public static readonly DependencyProperty HasNumpadProperty;
        public static readonly DependencyProperty HasKeyBoardProperty;

        public static readonly DependencyProperty ButtonMetadataProperty;

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

            ButtonMetadataProperty = DependencyProperty.RegisterAttached(
                name: "ButtonMetadata",
                propertyType: typeof(ButtonMetadata),
                ownerType: typeof(Keyboard));
        }

        internal static void SetButtonMetadata(DependencyObject element, ButtonMetadata metadata) =>
                element.SetValue(ButtonMetadataProperty, metadata);

        internal static ButtonMetadata GetButtonMetadata(DependencyObject element) =>
            (ButtonMetadata)element.GetValue(ButtonMetadataProperty);

        public Keyboard()
        {
            Focusable = false;

            _numpad = new Grid();
            _keyBoard = new Grid();

            FillNumpad();
            FillKeyBoard();
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
                        Margin = _buttonMargen,
                        Focusable = false
                    };

                    SetButtonMetadata(numberButton, new ButtonMetadata { KeyCode = (VirtualKeyShort)number });

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
                Margin = _buttonMargen,
                Focusable = false
            };

            SetButtonMetadata(point, new ButtonMetadata { KeyCode = VirtualKeyShort.OEM_PERIOD });

            point.Click += ClickRepeatButton;

            SetRowColumnPosition(point, _numpad.RowDefinitions.Count - 1, _numpad.ColumnDefinitions.Count - 1);

            _numpad.Children.Add(point);

            RepeatButton zero = new()
            {
                Content = '0',
                Margin = _buttonMargen,
                Focusable = false
            };

            SetButtonMetadata(zero, new ButtonMetadata { KeyCode = VirtualKeyShort.KEY_0 });

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
            INPUT[] inputs = new INPUT[1];

            ButtonMetadata key = GetButtonMetadata(sender as DependencyObject);

            inputs[0] = new INPUT()
            {
                type = (uint)InputEventType.INPUT_KEYBOARD,
                U = new InputUnion()
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key.KeyCode
                    }
                }
            };

            SendInput(1, inputs, INPUT.Size);
        }

        private void SetRowColumnPosition(UIElement element, int rowPosition, int columnPosition) 
        {
            Grid.SetColumn(element, columnPosition);
            Grid.SetRow(element, rowPosition);
        }
    }
}
