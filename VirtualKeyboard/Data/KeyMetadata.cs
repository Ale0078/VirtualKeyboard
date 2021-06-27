using VirtualKeyboard.Enums;

namespace VirtualKeyboard.Data
{
    public class KeyMetadata
    {
        private const double DEFAULT_SCALE = 1.0d;

        public double WidthScale { get; set; }
        public double HeightScale { get; set; }
        public VirtualKeyShort? KeyCode { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public bool IsLayoutSwitch { get; set; }
        public bool Is { get; set; }//ToDo: need know how to name left bottom button

        public KeyMetadata() 
        {
            WidthScale = DEFAULT_SCALE;
            HeightScale = DEFAULT_SCALE;
        }
    }
}
