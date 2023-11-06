using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class AudioConfigureWindow
    {
        public AudioConfigureWindow() => InitializeComponent();

        void OnTotalVolume(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as ConfigureViewModel).OnTotalVolume((float)e.NewValue);

        void OnMainVolume(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as ConfigureViewModel).OnMainVolume((float)e.NewValue);

        void OnInputVolume(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as ConfigureViewModel).OnInputVolume((float)e.NewValue);

        void OnSEVolume(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as ConfigureViewModel).OnSEVolume((float)e.NewValue);
    }
}