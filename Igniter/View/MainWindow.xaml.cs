using CommunityToolkit.Mvvm.Messaging;
using Igniter.MSG;
using Igniter.ViewModel;
using System.Windows;

namespace Igniter.View
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<ViewAllowWindow>(this, async (recipient, message) => message.Reply(await Dispatcher.InvokeAsync(() => MessageBox.Show(this, message.Text, "Qwilight", message.Input, message.Drawing))));
        }

        void OnLoaded(object sender, RoutedEventArgs e) => _ = (DataContext as MainViewModel).OnLoaded();
    }
}
