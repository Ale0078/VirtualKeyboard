using VirtualKeyboard.Data;
using VirtualKeyboard.Helpers;

namespace VirtualKeyboard.ViewModels
{
    public class KeyViewModel : BaseViewModel
    {
        private string _name;
        private double _height;
        private double _width;
        private KeyMetadata _keyData;

        public string Name 
        {
            get => _name;
            set 
            {
                _name = value;

                OnPropertyChanged();
            }
        }

        public double Height 
        {
            get => _height;
            set
            {
                _height = value;


                OnPropertyChanged();
            }
        }

        public double Width 
        {
            get => _width;
            set 
            {
                _width = value;

                OnPropertyChanged();
            }
        }

        public KeyMetadata KeyData 
        {
            get => _keyData;
            set 
            {
                _keyData = value;

                OnPropertyChanged();
            }
        }
    }
}
