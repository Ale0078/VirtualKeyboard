using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Globalization;
using System.Runtime.InteropServices;

using static VirtualKeyboard.Functions.User32;

using VirtualKeyboard.Enums;
using VirtualKeyboard.Structs;
using VirtualKeyboard.Data;
using VirtualKeyboard.ViewModels;
using VirtualKeyboard.Helpers;

namespace VirtualKeyboard
{
    public class Keyboard : Control
    {
        private const int ROW_COUNT = 4;

        private static KeyMetadata[][] _keyBoardMetadatas;
        private static KeyMetadata[][] _numpadMetadatas;

        public static readonly DependencyProperty KeyboardPartProperty;
        public static readonly DependencyProperty LanguagesIdProperty;
        public static readonly DependencyProperty CurrentLanguageProperty;
        public static readonly DependencyProperty KeysDataProperty;
        public static readonly DependencyProperty NumpadDataProperty;
        public static readonly DependencyProperty KeyClickProperty;
        public static readonly DependencyProperty LShiftKeyCheckedProperty;
        public static readonly DependencyProperty LShiftKeyUncheckedProperty;
        public static readonly DependencyProperty SelectLenguageProperty;
        public static readonly DependencyProperty KeyMarginProperty;
        public static readonly DependencyProperty KeyboardMarginProperty;
        public static readonly DependencyProperty NumpadMarginProperty;

        public static readonly DependencyProperty ButtonMetadataProperty;

        static Keyboard()
        {
            GenerateKeyboardMetadatas();
            GenerateNumpadeMetadatas();

            FocusableProperty.OverrideMetadata(
                forType: typeof(Keyboard),
                typeMetadata: new FrameworkPropertyMetadata(false));

            KeyboardPartProperty = DependencyProperty.Register(
                name: nameof(KeyboardPart),
                propertyType: typeof(KeyboardPart),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(KeyboardPart.All));

            LanguagesIdProperty = DependencyProperty.Register(
                name: nameof(LanguagesId),
                propertyType: typeof(IEnumerable<CultureInfo>),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(CreateCurrendLenguagesId()));

            CurrentLanguageProperty = DependencyProperty.Register(
                name: nameof(CurrentLanguage),
                propertyType: typeof(CultureInfo),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(GetCurrentLanguage()));

            KeysDataProperty = DependencyProperty.Register(
                name: nameof(KeysData),
                propertyType: typeof(IEnumerable<KeyViewModel>),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(CreateKeysData()));

            NumpadDataProperty = DependencyProperty.Register(
                name: nameof(NumpadData),
                propertyType: typeof(IEnumerable<KeyViewModel>),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(CreateNumpadData()));

            KeyClickProperty = DependencyProperty.Register(
                name: nameof(KeyClick),
                propertyType: typeof(ICommand),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(CreateDefaultKeyClickCommand()));

            LShiftKeyCheckedProperty = DependencyProperty.Register(
                name: nameof(LShiftKeyChecked),
                propertyType: typeof(ICommand),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(CreateDefaultLShiftKeyCheckedCommand()));
            
            LShiftKeyUncheckedProperty = DependencyProperty.Register(
                name: nameof(LShiftKeyUnchecked),
                propertyType: typeof(ICommand),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(CreateDefaultLShiftKeyUncheckedCommand()));

            SelectLenguageProperty = DependencyProperty.Register(
                name: nameof(SelectLenguage),
                propertyType: typeof(ICommand),
                ownerType: typeof(Keyboard));

            KeyMarginProperty = DependencyProperty.Register(
                name: nameof(KeyMargin),
                propertyType: typeof(Thickness),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(new Thickness(3)));

            KeyboardMarginProperty = DependencyProperty.Register(
                name: nameof(KeyboardMargin),
                propertyType: typeof(Thickness),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(new Thickness(5)));

            NumpadMarginProperty = DependencyProperty.Register(
                name: nameof(NumpadMargin),
                propertyType: typeof(Thickness),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(new Thickness(5)));

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
            SelectLenguage = CreateDefaultSelectLenguageCommand();
        }

        public KeyboardPart KeyboardPart 
        {
            get => (KeyboardPart)GetValue(KeyboardPartProperty);
            set => SetValue(KeyboardPartProperty, value);
        }

        public CultureInfo CurrentLanguage 
        {
            get => (CultureInfo)GetValue(CurrentLanguageProperty);
            set => SetValue(CurrentLanguageProperty, value);
        }

        public IEnumerable<CultureInfo> LanguagesId
        {
            get => (IEnumerable<CultureInfo>)GetValue(LanguagesIdProperty);
            set => SetValue(LanguagesIdProperty, value);
        }

        public IEnumerable<KeyViewModel> KeysData 
        {
            get => (IEnumerable<KeyViewModel>)GetValue(KeysDataProperty);
            set => SetValue(KeysDataProperty, value);
        }

        public IEnumerable<KeyViewModel> NumpadData
        {
            get => (IEnumerable<KeyViewModel>)GetValue(NumpadDataProperty);
            set => SetValue(NumpadDataProperty, value);
        }

        public ICommand KeyClick
        {
            get => (ICommand)GetValue(KeyClickProperty);
            set => SetValue(KeyClickProperty, value);
        }

        public ICommand LShiftKeyChecked 
        {
            get => (ICommand)GetValue(LShiftKeyCheckedProperty);
            set => SetValue(LShiftKeyCheckedProperty, value);
        }
        
        public ICommand LShiftKeyUnchecked 
        {
            get => (ICommand)GetValue(LShiftKeyUncheckedProperty);
            set => SetValue(LShiftKeyUncheckedProperty, value);
        }

        public ICommand SelectLenguage 
        {
            get => (ICommand)GetValue(SelectLenguageProperty);
            set => SetValue(SelectLenguageProperty, value);
        }

        public Thickness KeyMargin 
        {
            get => (Thickness)GetValue(KeyMarginProperty);
            set => SetValue(KeyMarginProperty, value);
        }

        public Thickness KeyboardMargin 
        {
            get => (Thickness)GetValue(KeyboardMarginProperty);
            set => SetValue(KeyboardMarginProperty, value);
        }

        public Thickness NumpadMargin 
        {
            get => (Thickness)GetValue(NumpadMarginProperty);
            set => SetValue(NumpadMarginProperty, value);
        }
        
        private static string TranslateKeyCode(VirtualKeyShort keyCode, bool doesShiftPresses = false, int? languageId = null) 
        {
            StringBuilder builder = new();

            byte[] buffer = new byte[255];
            
            GetKeyboardState(buffer);

            IntPtr hkl;

            if (doesShiftPresses)
            {
                buffer[16] = 128;
                buffer[160] = 128;
            }
            else 
            {
                buffer[16] = 0;
                buffer[160] = 0;
            }

            if (languageId is null)
            {
                hkl = GetKeyboardLayout(0);
            }
            else
            {
                hkl = (IntPtr)languageId.Value;
            }

            ToUnicodeEx((uint)keyCode, 0, buffer, builder, 3, 0, hkl);

            return builder.ToString();
        }

        private static void GenerateKeyboardMetadatas()
        {
            _keyBoardMetadatas = new KeyMetadata[ROW_COUNT][];

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

        private static void GenerateNumpadeMetadatas() 
        {
            _numpadMetadatas = new KeyMetadata[ROW_COUNT][];

            _numpadMetadatas[0] = new KeyMetadata[]
            {
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD7, Column = 0, Row = 0 },
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD8, Column = 1, Row = 0 },
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD9, Column = 2, Row = 0 }
            };

            _numpadMetadatas[1] = new KeyMetadata[]
            {
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD4, Column = 0, Row = 1 },
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD5, Column = 1, Row = 1 },
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD6, Column = 2, Row = 1 }
            };

            _numpadMetadatas[2] = new KeyMetadata[]
            {
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD1, Column = 0, Row = 2 },
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD2, Column = 1, Row = 2 },
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD3, Column = 2, Row = 2 }
            };

            _numpadMetadatas[3] = new KeyMetadata[]
            {
                new KeyMetadata() { KeyCode = VirtualKeyShort.NUMPAD0, Column = 0, Row = 3, WidthScale = 2.0d },
                new KeyMetadata() { KeyCode = VirtualKeyShort.DECIMAL, Column = 1, Row = 3 }
            };
        }

        private static IEnumerable<KeyViewModel> CreateKeysData() 
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

        private static IEnumerable<KeyViewModel> CreateNumpadData() 
        {
            ObservableCollection<KeyViewModel> numpadeDatas = new();

            foreach (KeyMetadata[] metadatas in _numpadMetadatas)
            {
                foreach (KeyMetadata data in metadatas)
                {
                    KeyViewModel keyData = new()
                    {
                        Name = TranslateKeyCode(data.KeyCode.Value),
                        KeyData = data
                    };

                    numpadeDatas.Add(keyData);
                }
            }

            return numpadeDatas;
        }

        private static CultureInfo GetCurrentLanguage() =>
            CultureInfo.GetCultureInfo((short)GetKeyboardLayout(0));

        private static IEnumerable<CultureInfo> CreateCurrendLenguagesId() 
        {
            uint lenguageCount = GetKeyboardLayoutList(0, null);

            IntPtr[] lenguages = new IntPtr[lenguageCount];

            GetKeyboardLayoutList(lenguages.Length, lenguages);

            ObservableCollection<CultureInfo> lenguagesId = new();

            foreach (IntPtr id in lenguages)
            {
                lenguagesId.Add(CultureInfo.GetCultureInfo((short)id));
            }

            return lenguagesId;
        }

        private static ICommand CreateDefaultKeyClickCommand() =>
            new RelayCommand(metadata =>
            {
                INPUT[] inputs = new INPUT[2];

                inputs[0] = new INPUT()
                {
                    type = (uint)InputEventType.INPUT_KEYBOARD,
                    U = new InputUnion()
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = ((KeyMetadata)metadata).KeyCode.Value
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
                            wVk = ((KeyMetadata)metadata).KeyCode.Value,
                            dwFlags = KEYEVENTF.KEYUP
                        }
                    }
                };

                SendInput((uint)inputs.Length, inputs, INPUT.Size);
            });

        private static ICommand CreateDefaultLShiftKeyCheckedCommand() =>
            new RelayCommand(keys =>
            {
                INPUT[] messages = new INPUT[1];

                messages[0] = new INPUT
                {
                    type = (ushort)InputEventType.INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = VirtualKeyShort.LSHIFT
                        }
                    }
                };

                SendInput((uint)messages.Length, messages, INPUT.Size);

                IEnumerable<KeyViewModel> keysReference = keys as IEnumerable<KeyViewModel>;

                foreach (KeyViewModel key in keysReference)
                {
                    key.Name = key.KeyData switch
                    {
                        { KeyCode: VirtualKeyShort.LSHIFT } => key.Name.ToUpper(),
                        { KeyCode: VirtualKeyShort.TAB } => key.Name.ToUpper(),
                        { KeyCode: VirtualKeyShort.RETURN } => key.Name.ToUpper(),
                        { KeyCode: VirtualKeyShort.BACK } => key.Name.ToUpper(),
                        { KeyCode: VirtualKeyShort.SPACE } => key.Name.ToUpper(),
                        { KeyCode: VirtualKeyShort.LEFT } => key.Name.ToUpper(),
                        { KeyCode: VirtualKeyShort.RIGHT } => key.Name.ToUpper(),
                        { IsLayoutSwitch: true } => key.Name.ToUpper(),
                        { Is: true } => key.Name.ToUpper(),
                        _ => TranslateKeyCode(key.KeyData.KeyCode.Value, true).ToUpper()
                    };
                }
            });

        private static ICommand CreateDefaultLShiftKeyUncheckedCommand() =>
            new RelayCommand(keys =>
            {
                INPUT[] messages = new INPUT[1];

                messages[0] = new INPUT
                {
                    type = (ushort)InputEventType.INPUT_KEYBOARD,
                    U = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = VirtualKeyShort.LSHIFT,
                            dwFlags = KEYEVENTF.KEYUP
                        }
                    }
                };

                SendInput((uint)messages.Length, messages, INPUT.Size);

                IEnumerable<KeyViewModel> keysReference = keys as IEnumerable<KeyViewModel>;

                foreach (KeyViewModel key in keysReference)
                {
                    key.Name = key.KeyData switch
                    {
                        { KeyCode: VirtualKeyShort.LSHIFT } => key.Name.ToLower(),
                        { KeyCode: VirtualKeyShort.TAB } => key.Name.ToLower(),
                        { KeyCode: VirtualKeyShort.RETURN } => key.Name.ToLower(),
                        { KeyCode: VirtualKeyShort.BACK } => key.Name.ToLower(),
                        { KeyCode: VirtualKeyShort.SPACE } => key.Name.ToLower(),
                        { KeyCode: VirtualKeyShort.LEFT } => key.Name.ToLower(),
                        { KeyCode: VirtualKeyShort.RIGHT } => key.Name.ToLower(),
                        { IsLayoutSwitch: true } => key.Name.ToLower(),
                        { Is: true } => key.Name.ToLower(),
                        _ => TranslateKeyCode(key.KeyData.KeyCode.Value).ToLower()
                    };
                }
            });

        private ICommand CreateDefaultSelectLenguageCommand() =>
            new RelayCommand(lenguage =>
            {
                const int WM_INPUT_LANG_CHANGE_REQUEST = 0x0050;

                PostMessage(
                    hWnd: new HandleRef(null, GetForegroundWindow()),
                    Msg: WM_INPUT_LANG_CHANGE_REQUEST,
                    wParam: IntPtr.Zero, 
                    lParam: (IntPtr)((CultureInfo)lenguage).KeyboardLayoutId);

                byte[] buffer = new byte[255];

                GetKeyboardState(buffer);

                bool doesShiftPressed = buffer[16] == 129 && buffer[160] == 129
                    || buffer[16] == 128 && buffer[160] == 128;

                foreach (KeyViewModel key in KeysData)
                {
                    key.Name = key.KeyData switch
                    {
                        { KeyCode: VirtualKeyShort.LSHIFT } => key.Name,
                        { KeyCode: VirtualKeyShort.TAB } => key.Name,
                        { KeyCode: VirtualKeyShort.RETURN } => key.Name,
                        { KeyCode: VirtualKeyShort.BACK } => key.Name,
                        { KeyCode: VirtualKeyShort.SPACE } => key.Name,
                        { KeyCode: VirtualKeyShort.LEFT } => key.Name,
                        { KeyCode: VirtualKeyShort.RIGHT } => key.Name,
                        { IsLayoutSwitch: true } => key.Name,
                        { Is: true } => key.Name,
                        _ => doesShiftPressed 
                                ? TranslateKeyCode(key.KeyData.KeyCode.Value, doesShiftPressed, ((CultureInfo)lenguage).KeyboardLayoutId).ToUpper()
                                : TranslateKeyCode(key.KeyData.KeyCode.Value, doesShiftPressed, ((CultureInfo)lenguage).KeyboardLayoutId).ToLower()
                    };
                }
            });
    }
}
