using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SignInWindow
    {
        public SignInWindow()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<GetSignInCipher>(this, (recipient, message) => message.Reply(InputCipher.Password));
            StrongReferenceMessenger.Default.Register<SetSignInCipher>(this, (recipient, message) => InputCipher.Password = message.Cipher);
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as SignInViewModel).OnInputLower(e);
    }
}