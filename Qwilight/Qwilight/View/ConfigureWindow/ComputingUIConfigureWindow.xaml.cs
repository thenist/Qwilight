using Qwilight.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ComputingUIConfigureWindow
    {
        public ComputingUIConfigureWindow() => InitializeComponent();

        void OnPointBaseUI(object sender, SelectionChangedEventArgs e) => (DataContext as ConfigureViewModel).OnPointBaseUI();

        void OnPointUI(object sender, SelectionChangedEventArgs e) => (DataContext as ConfigureViewModel).OnPointUI();

        void OnBaseUIConfigure(object sender, SelectionChangedEventArgs e) => (DataContext as ConfigureViewModel).OnBaseUIConfigure();

        void OnUIConfigure(object sender, SelectionChangedEventArgs e) => (DataContext as ConfigureViewModel).OnUIConfigure();

        void OnSetBaseUI(object sender, MouseButtonEventArgs e) => (DataContext as ConfigureViewModel).OnSetBaseUI();

        void OnSetUI(object sender, MouseButtonEventArgs e) => (DataContext as ConfigureViewModel).OnSetUI();
    }
}