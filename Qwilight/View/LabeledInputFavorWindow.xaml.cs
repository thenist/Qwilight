using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class InputFavorLabelledWindow
    {
        public InputFavorLabelledWindow() => InitializeComponent();

        void OnMeterModified(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as InputFavorLabelledViewModel).OnMeterModified();

        void OnSetMeter(object sender, RoutedEventArgs e) => (DataContext as InputFavorLabelledViewModel).OnMeterModified();
    }
}