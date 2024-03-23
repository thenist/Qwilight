using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class EnrollWindow
    {
        public EnrollWindow()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<GetEnrollCipher>(this, (recipient, message) => message.Reply((InputCipher.Password, InputCipherTest.Password)));
            StrongReferenceMessenger.Default.Register<InitEnrollCipher>(this, (recipient, message) =>
            {
                InputCipher.Password = string.Empty;
                InputCipherTest.Password = string.Empty;
            });
        }

        void OnInputLower(object sender, KeyEventArgs e) => _ = (DataContext as EnrollViewModel).OnInputLower(e);
    }
}