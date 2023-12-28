using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class LabelledInputFavorWindow
    {
        public LabelledInputFavorWindow() => InitializeComponent();

        void OnMeterModified(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as LabelledInputFavorViewModel).OnMeterModified();

        void OnSetMeter(object sender, RoutedEventArgs e) => (DataContext as LabelledInputFavorViewModel).OnMeterModified();
    }
}