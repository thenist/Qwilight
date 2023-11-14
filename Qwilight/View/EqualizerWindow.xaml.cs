using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class EqualizerWindow
    {
        public EqualizerWindow() => InitializeComponent();

        void OnEqualizer0(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizer0(e);

        void OnEqualizer1(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizer1(e);

        void OnEqualizer2(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizer2(e);

        void OnEqualizer3(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizer3(e);

        void OnEqualizer4(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizer4(e);

        void OnEqualizerHz0(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizerHz0(e);

        void OnEqualizerHz1(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizerHz1(e);

        void OnEqualizerHz2(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizerHz2(e);

        void OnEqualizerHz3(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizerHz3(e);

        void OnEqualizerHz4(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as EqualizerViewModel).OnEqualizerHz4(e);
    }
}