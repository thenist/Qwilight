using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Qwilight.View
{
    public sealed partial class LevelWindow
    {
        public LevelWindow() => InitializeComponent();

        void OnInput(object sender, RoutedEventArgs e) => (DataContext as LevelViewModel).OnInput();

        void OnNewLevel(object sender, SelectionChangedEventArgs e) => _ = (DataContext as LevelViewModel).OnNewLevel();
    }
}