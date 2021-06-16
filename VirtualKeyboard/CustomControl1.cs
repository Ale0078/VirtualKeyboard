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

        private enum EventType : uint
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
                type = (uint)EventType.INPUT_KEYBOARD,
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
                type = (uint)EventType.INPUT_KEYBOARD,
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
                type = (uint)EventType.INPUT_KEYBOARD,
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

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal uint type;
            internal InputUnion U;
            internal static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal VirtualKeyShort wVk;
            internal ScanCodeShort wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }
    }
}
