using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SignUpWindow
    {
        public SignUpWindow()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<GetSignUpCipher>(this, (recipient, message) => message.Reply((InputCipher.Password, InputCipherTest.Password)));
            StrongReferenceMessenger.Default.Register<InitSignUpCipher>(this, (recipient, message) =>
            {
                InputCipher.Password = string.Empty;
                InputCipherTest.Password = string.Empty;
            });
        }

        void OnInputLower(object sender, KeyEventArgs e) => _ = (DataContext as SignUpViewModel).OnInputLower(e);
    }
}