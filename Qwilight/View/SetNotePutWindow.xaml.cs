using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class SetNotePutWindow
    {
        public SetNotePutWindow() => InitializeComponent();

        void OnMeterModified(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as SetNotePutViewModel).OnMeterModified();

        void OnSetMeter(object sender, RoutedEventArgs e) => (DataContext as SetNotePutViewModel).OnMeterModified();
    }
}