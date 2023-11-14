using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SiteWindow
    {
        public SiteWindow() => InitializeComponent();

        void OnEnterSite(object sender, MouseButtonEventArgs e) => (DataContext as SiteWindowViewModel).OnEnterSite();
    }
}