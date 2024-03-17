using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class LogInWindow
    {
        public LogInWindow()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<GetLogInCipher>(this, (recipient, message) => message.Reply(InputCipher.Password));
            StrongReferenceMessenger.Default.Register<SetLogInCipher>(this, (recipient, message) => InputCipher.Password = message.Cipher);
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as LogInViewModel).OnInputLower(e);
    }
}