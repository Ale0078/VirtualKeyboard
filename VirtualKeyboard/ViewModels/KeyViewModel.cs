using VirtualKeyboard.Data;
using VirtualKeyboard.Helpers;

namespace VirtualKeyboard.ViewModels
{
    public class KeyViewModel : BaseViewModel
    {
        private string _name;
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
