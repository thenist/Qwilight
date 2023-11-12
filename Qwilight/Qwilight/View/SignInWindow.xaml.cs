using CommunityToolkit.Mvvm.Messaging;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class SignInWindow : IRecipient<ICC>
    {
        public SignInWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.GetSignInCipher:
                    (message.Contents as Action<string>)(InputCipher.Password);
                    break;
                case ICC.ID.SetSignInCipher:
                    InputCipher.Password = message.Contents as string;
                    break;
            }
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as SignInViewModel).OnInputLower(e);
    }
}