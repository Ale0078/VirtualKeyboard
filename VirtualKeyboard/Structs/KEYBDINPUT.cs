using System;
using System.Runtime.InteropServices;

using VirtualKeyboard.Enums;

namespace VirtualKeyboard.Structs
{
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
