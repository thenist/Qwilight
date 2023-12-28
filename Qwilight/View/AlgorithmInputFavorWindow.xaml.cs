using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class AlgorithmInputFavorWindow
    {
        public AlgorithmInputFavorWindow() => InitializeComponent();

        void OnMeterModified(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as AlgorithmInputFavorViewModel).OnMeterModified();

        void OnSetMeter(object sender, RoutedEventArgs e) => (DataContext as AlgorithmInputFavorViewModel).OnMeterModified();
    }
}