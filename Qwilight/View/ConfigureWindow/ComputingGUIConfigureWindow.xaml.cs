using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ComputingGUIConfigureWindow
    {

        public ComputingGUIConfigureWindow() => InitializeComponent();

        void OnVeilDrawing(object sender, MouseButtonEventArgs e) => _ = (DataContext as ConfigureViewModel).OnVeilDrawing();

        void OnComputingPointed(object sender, DragStartedEventArgs e) => (DataContext as ConfigureViewModel).OnComputingPointed();

        void OnComputingNotPointed(object sender, DragCompletedEventArgs e) => (DataContext as ConfigureViewModel).OnComputingNotPointed();

        void OnComputingModified(object sender, RoutedEventArgs e) => (DataContext as ConfigureViewModel).OnComputingModified();
    }
}