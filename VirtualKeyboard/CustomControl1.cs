using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

using VirtualKeyboard.Enums;
using VirtualKeyboard.Structs;

namespace VirtualKeyboard
{
    public class CustomControl1 : Button
    {
        static CustomControl1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl1), new FrameworkPropertyMetadata(typeof(CustomControl1)));
        }

        public CustomControl1()
        {
            Focusable = false;

            ModifierKeysConverter converter = new();

            char d = Convert.ToChar(MapVirtualKey((uint)VirtualKeyShort.KEY_A, 2));


            CultureInfo info = CultureInfo.GetCultureInfo(0x0419);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(0x0419);

            char c = Convert.ToChar(VirtualKeyShort.KEY_A, info);
            ContentStringFormat = c.ToString().ToLower();
        }

        private enum InputEventType : uint
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }
         
        protected override void OnClick()
        {
            base.OnClick();

            INPUT[] inputs = new INPUT[1];

            uint a = (uint)VirtualKeyShort.KEY_A;

            inputs[0] = new INPUT()
            {
                type = (uint)InputEventType.INPUT_KEYBOARD,
                U = new InputUnion()
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = (VirtualKeyShort)a
                    }
                }
            };

            SendInput(1, inputs, INPUT.Size);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

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

        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,
           [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
           int cbSize);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }
}
