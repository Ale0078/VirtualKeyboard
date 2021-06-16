using System;
using System.Runtime.InteropServices;

using VirtualKeyboard.Enums;

namespace VirtualKeyboard.Structs
{
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
}
