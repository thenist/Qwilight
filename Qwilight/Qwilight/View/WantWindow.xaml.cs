using Qwilight.ViewModel;
using System.Windows;

namespace Qwilight.View
{
    public sealed partial class WantWindow
    {
        public WantWindow() => InitializeComponent();

        void OnLowestWantLevelTextValue(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnLowestWantLevelTextValue();

        void OnHighestWantLevelTextValue(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnHighestWantLevelTextValue();

        void OnLowestWantBPM(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnLowestWantBPM();

        void OnHighestWantBPM(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnHighestWantBPM();

        void OnLowestWantAverageInputCount(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnLowestWantAverageInputCount();

        void OnHighestWantAverageInputCount(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnHighestWantAverageInputCount();

        void OnLowestWantHighestInputCount(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnLowestWantHighestInputCount();

        void OnHighestWantHighestInputCount(object sender, RoutedPropertyChangedEventArgs<double> e) => (DataContext as WantViewModel).OnHighestWantHighestInputCount();
    }
}