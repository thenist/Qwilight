using CommunityToolkit.Mvvm.Messaging;
using Igniter.ViewModel;
using System;
using System.Linq;
using System.Windows;

namespace Igniter.View
{
    public partial class MainWindow : IRecipient<ICC>
    {
        public MainWindow()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        void OnLoaded(object sender, RoutedEventArgs e) => (DataContext as MainViewModel).OnLoaded();

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.ViewAllowWindow:
                    var data = message.Contents as object[];
                    var r = MessageBox.Show(this, data[0] as string, "Qwilight", (MessageBoxButton)data[1], (MessageBoxImage)data[2]);
                    (data.ElementAtOrDefault(3) as Action<MessageBoxResult>)?.Invoke(r);
                    break;
            }
        }
    }
}
