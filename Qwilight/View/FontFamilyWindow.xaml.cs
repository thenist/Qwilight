using Qwilight.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class FontFamilyWindow
    {
        public FontFamilyWindow() => InitializeComponent();

        void OnWant(object sender, TextChangedEventArgs e) => (DataContext as FontFamilyViewModel).OnWant();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as FontFamilyViewModel).OnPointLower();
    }
}
