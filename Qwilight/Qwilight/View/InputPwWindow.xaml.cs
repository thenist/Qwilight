using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class InputPwWindow : IRecipient<ICC>
    {
        public InputPwWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.GetPwWindowCipher:
                    (message.Contents as Action<string>)(InputCipher.Password);
                    break;
                case ICC.ID.ClearPwWindowCipher:
                    InputCipher.Password = string.Empty;
                    break;
            }
        }

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as InputPwViewModel).OnInputLower(e);
    }
}
