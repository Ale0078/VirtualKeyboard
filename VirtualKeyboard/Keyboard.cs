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
        private const int NUMPAD_ROWS = 4;
        //private const int NUMPAD_COLUMNS = 3;
        //private const double DEFAULT_BUTTON_WIDTH = 60;//ToDo: Set Width to buttons from user
        //private const double DEFAULT_BUTTON_HEIGHT = 60;//ToDo: Set Height to buttons from user

        //private Thickness _buttonMargen = new Thickness(5);//ToDo: Set Margen to buttons from user
        private KeyMetadata[][] _keyBoardMetadatas;
        
        public static readonly DependencyProperty HasNumpadProperty;
        public static readonly DependencyProperty HasKeyBoardProperty;
        public static readonly DependencyProperty LanguagesIdProperty;
        public static readonly DependencyProperty CurrentLanguageProperty;
        public static readonly DependencyProperty KeysDataProperty;
        public static readonly DependencyProperty KeyClickProperty;
        public static readonly DependencyProperty LShiftKeyCheckedProperty;
        public static readonly DependencyProperty LShiftKeyUncheckedProperty;
        public static readonly DependencyProperty SelectLenguageProperty;

        public static readonly DependencyProperty ButtonMetadataProperty;

        static Keyboard()
        {
            FocusableProperty.OverrideMetadata(
                forType: typeof(Keyboard),
                typeMetadata: new FrameworkPropertyMetadata(false));

            HasNumpadProperty = DependencyProperty.Register(
                name: nameof(HasNumpad),
                propertyType: typeof(bool),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(true));

            HasKeyBoardProperty = DependencyProperty.Register(
                name: nameof(HasKeyBoard),
                propertyType: typeof(bool),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(true));//ToDo: switch to true

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
                ownerType: typeof(Keyboard));

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
            GenerateMetadata();

            KeysData = CreateKeysData();
            SelectLenguage = CreateDefaultSelectLenguageCommand();
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

        private static string TranslateKeyCode(VirtualKeyShort keyCode, int? languageId = null) 
        {
            StringBuilder builder = new();

            byte[] buffer = new byte[255];
            
            GetKeyboardState(buffer);

            IntPtr hkl;

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
                INPUT[] inputs = new INPUT[1];

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

                SendInput(1, inputs, INPUT.Size);
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

                SendInput(1, messages, INPUT.Size);

                IEnumerable<KeyViewModel> keysReference = keys as IEnumerable<KeyViewModel>;

                foreach (KeyViewModel key in keysReference)
                {
                    key.Name = key.Name.ToUpper();
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

                SendInput(1, messages, INPUT.Size);

                IEnumerable<KeyViewModel> keysReference = keys as IEnumerable<KeyViewModel>;

                foreach (KeyViewModel key in keysReference)
                {
                    key.Name = key.Name.ToLower();
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
                        _ => TranslateKeyCode(key.KeyData.KeyCode.Value, ((CultureInfo)lenguage).KeyboardLayoutId).ToLower()
                    };
                }
            });
    }
}
