using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VirtualKeyboard.Enums;

namespace TestVirtualKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _counter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            keyboard.KeyboardPart = _counter switch
            {
                0 => KeyboardPart.All,
                1 => KeyboardPart.Keyboard,
                _ => KeyboardPart.Numpad
            };

            _counter++;

            if (_counter == 3)
            {
                _counter = 0;
            }
        }
    }
}
