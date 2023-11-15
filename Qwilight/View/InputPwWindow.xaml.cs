using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class InputPwWindow
    {
        public InputPwWindow()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<GetPwWindowCipher>(this, (recipient, message) => message.Reply(InputCipher.Password));
            StrongReferenceMessenger.Default.Register<InitSignUpCipher>(this, (recipient, message) => InputCipher.Password = string.Empty);
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as InputPwViewModel).OnInputLower(e);
    }
}
