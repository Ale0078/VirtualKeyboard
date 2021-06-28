using System.Windows;
using System.Windows.Controls;

using VirtualKeyboard.ViewModels;
using VirtualKeyboard.Enums;

namespace VirtualKeyboard.Selectors
{
    public class KeyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultKeyTemplate { get; set; }
        public DataTemplate LShiftKeyTemplate { get; set; }
        public DataTemplate LayoutKeyTemplate { get; set; }
        public DataTemplate IsKeyTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) => ((KeyViewModel)item).KeyData switch
        {
            { Is: true } => IsKeyTemplate,
            { IsLayoutSwitch: true } => LayoutKeyTemplate,
            { KeyCode: VirtualKeyShort.LSHIFT } => LShiftKeyTemplate,
            _ => DefaultKeyTemplate
        };
    }
}
