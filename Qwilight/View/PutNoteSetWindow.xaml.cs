using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class PutNoteSetWindow
    {
        public PutNoteSetWindow() => InitializeComponent();

        void OnMeterModified(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as PutNoteSetViewModel).OnMeterModified();

        void OnSetMeter(object sender, RoutedEventArgs e) => (DataContext as PutNoteSetViewModel).OnMeterModified();
    }
}