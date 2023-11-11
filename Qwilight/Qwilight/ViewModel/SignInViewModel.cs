using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class SignInViewModel : BaseViewModel
    {
        public override double TargetLength => 0.2;

        public override double TargetHeight => double.NaN;

        public override HorizontalAlignment LengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Center;

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