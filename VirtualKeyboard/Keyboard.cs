using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static VirtualKeyboard.Functions.User32;

using VirtualKeyboard.Enums;
using VirtualKeyboard.Structs;

namespace VirtualKeyboard
{
    public class Keyboard : Control
    {
        private readonly Grid _numpad;
        private readonly Grid _keyBoard;

        public static readonly DependencyProperty HasNumpadProperty;
        public static readonly DependencyProperty HasKeyBoardProperty;

        static Keyboard()
        {
            HasNumpadProperty = DependencyProperty.Register(
                name: nameof(HasNumpad),
                propertyType: typeof(bool),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(true));

            HasKeyBoardProperty = DependencyProperty.Register(
                name: nameof(HasKeyBoard),
                propertyType: typeof(bool),
                ownerType: typeof(Keyboard),
                typeMetadata: new PropertyMetadata(true));
        }

        public Keyboard()
        {
            _numpad = new Grid();
            _keyBoard = new Grid();
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
    }
}
