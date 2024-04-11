using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class LogInViewModel : BaseViewModel
    {
        public override double TargetLength => 0.2;

        public override double TargetHeight => double.NaN;

        public override HorizontalAlignment LengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Center;

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnLogIn();
            }
        }

        [RelayCommand]
        static void OnLogIn()
        {
            var avatarCipher = StrongReferenceMessenger.Default.Send<GetLogInCipher>().Response;
            Configure.Instance.SetCipher(avatarCipher);
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.LogIn, new
            {
                avatarID = Configure.Instance.AvatarID,
                avatarCipher = avatarCipher
            });
        }

        [RelayCommand]
        void OnEnroll()
        {
            Close();
            ViewModels.Instance.EnrollValue.Open();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            var inputCipher = StrongReferenceMessenger.Default.Send(new SetLogInCipher
            {
                Cipher = Configure.Instance.GetCipher()
            });
        }
    }
}