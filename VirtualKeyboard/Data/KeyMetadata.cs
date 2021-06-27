using VirtualKeyboard.Enums;
using VirtualKeyboard.Helpers;

namespace VirtualKeyboard.Data
{
    public class KeyMetadata : BaseViewModel
    {
        private const double DEFAULT_SCALE = 1.0d;

        private double _widthScale;
        private double _heightScale;
        private VirtualKeyShort? _keyCode;
        private int _column;
        private int _row;
        private bool _isLayoutSwitch;
        private bool _is;//ToDo: need know how to name left bottom button

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

        public VirtualKeyShort? KeyCode
        {
            get => _keyCode;
            set
            {
                _keyCode = value;

                OnPropertyChanged();
            }
        }

        public int Column
        {
            get => _column;
            set
            {
                _column = value;

                OnPropertyChanged();
            }
        }

        public int Row
        {
            get => _row;
            set
            {
                _row = value;

                OnPropertyChanged();
            }
        }

        public bool IsLayoutSwitch
        {
            get => _isLayoutSwitch;
            set
            {
                _isLayoutSwitch = value;

                OnPropertyChanged();
            }
        }

        public bool Is
        {
            get => _is;
            set
            {
                _is = value;

                OnPropertyChanged();
            }
        }

        public KeyMetadata() 
        {
            WidthScale = DEFAULT_SCALE;
            HeightScale = DEFAULT_SCALE;
        }
    }
}
