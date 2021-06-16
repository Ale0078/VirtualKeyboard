using System.Runtime.InteropServices;

using VirtualKeyboard.Structs;

namespace VirtualKeyboard.Functions
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);
    }
}
