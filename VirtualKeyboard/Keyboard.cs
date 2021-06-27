using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VirtualKeyboard.ViewModels;

namespace VirtualKeyboard
{
    public class Keyboard : Control
    {
        private const int NUMPAD_ROWS = 4;
        //private const int NUMPAD_COLUMNS = 3;
        //private const double DEFAULT_BUTTON_WIDTH = 60;//ToDo: Set Width to buttons from user
        //private const double DEFAULT_BUTTON_HEIGHT = 60;//ToDo: Set Height to buttons from user

        //private Thickness _buttonMargen = new Thickness(5);//ToDo: Set Margen to buttons from user
        private KeyMetadata[][] _keyBoardMetadatas;

        //private readonly Grid _numpad;
        //private readonly Grid _keyBoard;

        public static readonly DependencyProperty HasNumpadProperty;
        public static readonly DependencyProperty HasKeyBoardProperty;
        public static readonly DependencyProperty KeysDataProperty;

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

            KeysDataProperty = DependencyProperty.Register(
                name: nameof(KeysData),
                propertyType: typeof(IEnumerable<KeyViewModel>),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(null));

            ButtonMetadataProperty = DependencyProperty.RegisterAttached(
                name: "KeyMetadata",
                propertyType: typeof(KeyMetadata),
                ownerType: typeof(Keyboard));
        }

        internal static void SetKeyMetadata(DependencyObject element, KeyMetadata metadata) =>
                element.SetValue(ButtonMetadataProperty, metadata);

        internal static KeyMetadata GetKeyMetadata(DependencyObject element) =>
            (KeyMetadata)element.GetValue(ButtonMetadataProperty);

        public Keyboard()
        {
            Focusable = false;

            //_numpad = new Grid();
            //_keyBoard = new Grid();

            GenerateMetadata();
            //FillNumpad();
            //FillKeyBoard();



            KeysData = CreateKeysData();
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

        public IEnumerable<KeyViewModel> KeysData 
        {
            get => (IEnumerable<KeyViewModel>)GetValue(KeysDataProperty);
            set => SetValue(KeysDataProperty, value);
        }

        //private void FillNumpad() //ToDo: Add margin to buttons
        //{
        //    GenerateRowAndColumn(_numpad, NUMPAD_ROWS, NUMPAD_COLUMNS);

        //    SetZeroPointToNumpad();

        //    uint number = 0x31;

        //    for (int i = _numpad.RowDefinitions.Count - 2; i >= 0; i--)
        //    {
        //        for (int j = 0; j < _numpad.ColumnDefinitions.Count; j++)
        //        {
        //            RepeatButton numberButton = new()
        //            {
        //                Content = Convert.ToChar(number),
        //                Margin = _buttonMargen,
        //                Focusable = false
        //            };

        //            SetKeyMetadata(numberButton, new KeyMetadata { KeyCode = (VirtualKeyShort)number });

        //            numberButton.Click += ClickRepeatButton;

        //            _numpad.Children.Add(numberButton);

        //            SetRowColumnPosition(numberButton, i, j);

        //            number++;
        //        }
        //    }
        //}

        //private void SetZeroPointToNumpad() 
        //{
        //    RepeatButton point = new()
        //    {
        //        Content = '.',
        //        Margin = _buttonMargen,
        //        Focusable = false
        //    };

        //    SetKeyMetadata(point, new KeyMetadata { KeyCode = VirtualKeyShort.DECIMAL });//ToDo: point

        //    point.Click += ClickRepeatButton;

        //    SetRowColumnPosition(point, _numpad.RowDefinitions.Count - 1, _numpad.ColumnDefinitions.Count - 1);

        //    _numpad.Children.Add(point);

        //    RepeatButton zero = new()
        //    {
        //        Content = '0',
        //        Margin = _buttonMargen,
        //        Focusable = false
        //    };

        //    SetKeyMetadata(zero, new KeyMetadata { KeyCode = VirtualKeyShort.KEY_0 });

        //    zero.Click += ClickRepeatButton;

        //    Grid.SetColumnSpan(zero, 2);
        //    Grid.SetRow(zero, _numpad.RowDefinitions.Count - 1);

        //    _numpad.Children.Add(zero);
        //}

        //private void FillKeyBoard() 
        //{
        //    GenerateRowAndColumn(_keyBoard, NUMPAD_ROWS, 0);

        //    for (int i = 0; i < NUMPAD_ROWS; i++)
        //    {
        //        StackPanel panel = new StackPanel
        //        {
        //            Orientation = Orientation.Horizontal
        //        };

        //        int position = 0;
        //        ButtonBase button;

        //        foreach (KeyMetadata data in _keyBoardMetadatas[i])
        //        {
        //            if (data.KeyCode is null)
        //            {
        //                UIElement element = CreateUniqueButtons(data, position);//ToDo: set handler to combobox and button

        //                panel.Children.Add(element);

        //                position++;

        //                continue;
        //            }

        //            if (data.KeyCode is VirtualKeyShort.LSHIFT)
        //            {
        //                button = new Button()//ToDo: with shift
        //                {
        //                    Height = DEFAULT_BUTTON_HEIGHT * data.HeightScale,
        //                    Width = DEFAULT_BUTTON_WIDTH * data.WidthScale,
        //                    Focusable = false,
        //                    Content = TranslateKeyCode(data.KeyCode.Value)
        //                };

        //                button.Click += ClickShiftButton;
        //            }
        //            else
        //            {
        //                button = new RepeatButton()
        //                {
        //                    Height = DEFAULT_BUTTON_HEIGHT * data.HeightScale,
        //                    Width = DEFAULT_BUTTON_WIDTH * data.WidthScale,
        //                    Focusable = false,
        //                    Content = TranslateKeyCode(data.KeyCode.Value)
        //                    //Margin = _buttonMargen
        //                };

        //                button.Click += ClickRepeatButton;
        //            }

        //            SetConstName(data, button);

        //            SetKeyMetadata(button, data);

        //            panel.Children.Add(button);

        //            position++;
        //        }

        //        _keyBoard.Children.Add(panel);

        //        Grid.SetRow(panel, i);
        //    }
        //}

        private void GenerateMetadata() 
        {
            _keyBoardMetadatas = new KeyMetadata[NUMPAD_ROWS][];

            _keyBoardMetadatas[0] = new KeyMetadata[]
            {
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_Q, Row = 0, Column = 0 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_W, Row = 0, Column = 1 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_E, Row = 0, Column = 2 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_R, Row = 0, Column = 3 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_T, Row = 0, Column = 4 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_Y, Row = 0, Column = 5 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_U, Row = 0, Column = 6 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_I, Row = 0, Column = 7 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_O, Row = 0, Column = 8 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_P, Row = 0, Column = 9 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_4, Row = 0, Column = 10 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_6, Row = 0, Column = 11 },
                new KeyMetadata { KeyCode = VirtualKeyShort.BACK, Row = 0, Column = 12 }
            };

            _keyBoardMetadatas[1] = new KeyMetadata[]
            {
                new KeyMetadata { KeyCode = VirtualKeyShort.TAB, Row = 1, Column = 0 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_A, Row = 1, Column = 1 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_S, Row = 1, Column = 2 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_D, Row = 1, Column = 3 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_F, Row = 1, Column = 4 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_G, Row = 1, Column = 5 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_H, Row = 1, Column = 6 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_J, Row = 1, Column = 7 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_K, Row = 1, Column = 8 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_L, Row = 1, Column = 9 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_1, Row = 1, Column = 10 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_7, Row = 1, Column = 11 },
                new KeyMetadata { KeyCode = VirtualKeyShort.RETURN, Row = 1, Column = 12 }
            };

            _keyBoardMetadatas[2] = new KeyMetadata[]
            {
                new KeyMetadata { KeyCode = VirtualKeyShort.LSHIFT, WidthScale = 2.0d, Row = 2, Column = 0 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_Z, Row = 2, Column = 1 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_X, Row = 2, Column = 2 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_C, Row = 2, Column = 3 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_V, Row = 2, Column = 4 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_B, Row = 2, Column = 5 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_N, Row = 2, Column = 6 },
                new KeyMetadata { KeyCode = VirtualKeyShort.KEY_M, Row = 2, Column = 7 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_COMMA, Row = 2, Column = 8 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_PERIOD, Row = 2, Column = 9 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_3, Row = 2, Column = 10 },
                new KeyMetadata { KeyCode = VirtualKeyShort.OEM_2, Row = 2, Column = 11 }
            };

            _keyBoardMetadatas[3] = new KeyMetadata[]
            {
                new KeyMetadata { Is = true, Row = 3, Column = 0 },
                new KeyMetadata { KeyCode = VirtualKeyShort.SPACE, WidthScale = 9.0d, Row = 3, Column = 1 },
                new KeyMetadata { KeyCode = VirtualKeyShort.LEFT, Row = 3, Column = 2 },
                new KeyMetadata { KeyCode = VirtualKeyShort.RIGHT, Row = 3, Column = 3 },
                new KeyMetadata { IsLayoutSwitch = true, Row = 3, Column = 4 }
            };
        }

        //private UIElement CreateUniqueButtons(KeyMetadata data, int position) => position switch
        //{
        //    0 => new Button()
        //    {
        //        Height = DEFAULT_BUTTON_HEIGHT * data.HeightScale,
        //        Width = DEFAULT_BUTTON_WIDTH * data.WidthScale,
        //        //Margin = _buttonMargen,
        //        Focusable = false,
        //        Content = "&123"
        //    },
        //    4 => new ComboBox()
        //    {
        //        Height = DEFAULT_BUTTON_HEIGHT * data.HeightScale,
        //        Width = DEFAULT_BUTTON_WIDTH * data.WidthScale,
        //        //Margin = _buttonMargen,
        //        Focusable = false
        //    },
        //    _ => null
        //};

        //private void GenerateRowAndColumn(Grid field, int rowCount, int columnCount) 
        //{
        //    for (int i = 0; i < rowCount; i++)
        //    {
        //        field.RowDefinitions.Add(new RowDefinition());
        //    }

        //    for (int i = 0; i < columnCount; i++)
        //    {
        //        field.ColumnDefinitions.Add(new ColumnDefinition());
        //    }
        //}

        //private void SetConstName(KeyMetadata data, ButtonBase button) 
        //{
        //    switch (data.KeyCode.Value)
        //    {
        //        case VirtualKeyShort.LSHIFT:
        //            button.Content = "shift";
        //            return;
        //        case VirtualKeyShort.TAB:
        //            button.Content = "tab";
        //            return;
        //        case VirtualKeyShort.RETURN:
        //            button.Content = "enter";
        //            return;
        //        case VirtualKeyShort.BACK:
        //            button.Content = "back";
        //            return;
        //        case VirtualKeyShort.SPACE:
        //            button.Content = "space";
        //            return;
        //        case VirtualKeyShort.LEFT:
        //            button.Content = "<";
        //            return;
        //        case VirtualKeyShort.RIGHT:
        //            button.Content = ">";
        //            return;
        //        default:
        //            return;
        //    }
        //}

        private void ClickRepeatButton(object sender, RoutedEventArgs e) 
        {
            INPUT[] inputs = new INPUT[1];

            KeyMetadata key = GetKeyMetadata(sender as DependencyObject);

            inputs[0] = new INPUT()
            {
                type = (uint)InputEventType.INPUT_KEYBOARD,
                U = new InputUnion()
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key.KeyCode.Value
                    }
                }
            };

            SendInput(1, inputs, INPUT.Size);
        }

        private void ClickShiftButton(object sender, RoutedEventArgs e)
        {
            INPUT[] inputs = new INPUT[2];

            inputs[0] = new INPUT()
            {
                type = (uint)InputEventType.INPUT_KEYBOARD,
                U = new InputUnion()
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VirtualKeyShort.CAPITAL
                    }
                }
            };

            inputs[1] = new INPUT()
            {
                type = (uint)InputEventType.INPUT_KEYBOARD,
                U = new InputUnion()
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = VirtualKeyShort.CAPITAL,
                        dwFlags = KEYEVENTF.KEYUP
                    }
                }
            };

            SendInput(2, inputs, INPUT.Size);
        }

        //private void SetRowColumnPosition(UIElement element, int rowPosition, int columnPosition) 
        //{
        //    Grid.SetColumn(element, columnPosition);
        //    Grid.SetRow(element, rowPosition);            
        //}

        private string TranslateKeyCode(VirtualKeyShort keyCode) 
        {
            StringBuilder builder = new();

            byte[] buffer = new byte[255];

            GetKeyboardState(buffer);

            IntPtr hkl = GetKeyboardLayout(0);

            ToUnicodeEx((uint)keyCode, 0, buffer, builder, 2, 0, hkl);

            return builder.ToString();
        }





        private IEnumerable<KeyViewModel> CreateKeysData() 
        {
            ObservableCollection<KeyViewModel> keysData = new();

            foreach (KeyMetadata[] metadatas in _keyBoardMetadatas)
            {
                foreach (KeyMetadata data in metadatas)
                {
                    KeyViewModel keyData = new()
                    {
                        KeyData = data
                    };


                    keyData.Name = data switch
                    {
                        { KeyCode: VirtualKeyShort.LSHIFT } => "shift",
                        { KeyCode: VirtualKeyShort.TAB } => "tab",
                        { KeyCode: VirtualKeyShort.RETURN } => "enter",
                        { KeyCode: VirtualKeyShort.BACK } => "back",
                        { KeyCode: VirtualKeyShort.SPACE } => "space",
                        { KeyCode: VirtualKeyShort.LEFT } => "<",
                        { KeyCode: VirtualKeyShort.RIGHT } => ">", 
                        { IsLayoutSwitch: true } => "EU",
                        { Is: true } => "&123",
                        _ => TranslateKeyCode(data.KeyCode.Value).ToLower()
                    };

                    keysData.Add(keyData);
                }
            }

            return keysData;
        }
    }
}
