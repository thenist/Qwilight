using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class GUIConfigureWindow
    {
        public GUIConfigureWindow() => InitializeComponent();

        void OnWindowArea(object sender, RoutedEventArgs e) => (DataContext as ConfigureViewModel).OnWindowArea();

        void OnDefaultDrawing(object sender, MouseButtonEventArgs e) => (DataContext as ConfigureViewModel).OnDefaultDrawing();
    }
}