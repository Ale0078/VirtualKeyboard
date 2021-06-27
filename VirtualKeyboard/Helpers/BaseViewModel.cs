using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VirtualKeyboard.Helpers
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string property = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
