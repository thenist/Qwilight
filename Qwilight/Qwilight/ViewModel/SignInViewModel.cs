using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class SignInViewModel : BaseViewModel
    {
        public override double TargetLength => 0.2;

        public override double TargetHeight => 0.3;

        public override HorizontalAlignment TargetLengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Center;

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSignIn();
            }
        }

        [RelayCommand]
        void OnSignIn() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.GetSignInCipher,
            Contents = new Action<string>(inputCipher =>
            {
                Configure.Instance.SetCipher(inputCipher);
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SignIn, new
                {
                    avatarID = Configure.Instance.AvatarID,
                    avatarCipher = inputCipher
                });
            })
        });

        [RelayCommand]
        void OnSignUp()
        {
            Close();
            ViewModels.Instance.SignUpValue.Open();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.SetSignInCipher,
                Contents = Configure.Instance.GetCipher()
            });
        }
    }
}