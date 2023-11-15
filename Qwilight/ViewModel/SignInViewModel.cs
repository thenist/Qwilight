using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
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
        static void OnSignIn()
        {
            var inputCipher = StrongReferenceMessenger.Default.Send<GetSignInCipher>().Response;
            Configure.Instance.SetCipher(inputCipher);
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SignIn, new
            {
                avatarID = Configure.Instance.AvatarID,
                avatarCipher = inputCipher
            });
        }

        [RelayCommand]
        void OnSignUp()
        {
            Close();
            ViewModels.Instance.SignUpValue.Open();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            var inputCipher = StrongReferenceMessenger.Default.Send(new SetSignInCipher
            {
                Cipher = Configure.Instance.GetCipher()
            });
        }
    }
}