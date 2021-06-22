using System.ComponentModel;
using System.Runtime.CompilerServices;

using VirtualKeyboard.Enums;

namespace VirtualKeyboard.Data
{
    internal class ButtonMetadata : INotifyPropertyChanged
    {
        private const double DEFAULT_SCALE = 1.0d;

        private double _widthScale;
        private double _heightScale;
        private VirtualKeyShort _keyCode;

        public event PropertyChangedEventHandler PropertyChanged;

        public double WidthScale 
        {
            get => _widthScale;
            set 
            {
                _widthScale = value;

                OnPropertyChanged();
            }
        }

        public double HeightScale 
        {
            get => _heightScale;
            set 
            {
                _heightScale = value;

                OnPropertyChanged();
            }
        }

        public VirtualKeyShort KeyCode 
        {
            get => _keyCode;
            set 
            {
                _keyCode = value;

                OnPropertyChanged();
            }
        }

        public ButtonMetadata() 
        {
            WidthScale = DEFAULT_SCALE;
            HeightScale = DEFAULT_SCALE;
        }

        public void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
