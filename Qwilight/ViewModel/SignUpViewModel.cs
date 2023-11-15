using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.Utilities;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class SignUpViewModel : BaseViewModel
    {
        public override double TargetLength => 0.2;

        public override double TargetHeight => double.NaN;

        public override HorizontalAlignment LengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Center;

        public string AvatarID { get; set; }

        public string AvatarName { get; set; }

        public string Fax { get; set; }

        public override void OnOpened()
        {
            base.OnOpened();
            AvatarID = string.Empty;
            WeakReferenceMessenger.Default.Send<ICC>(new()
            {
                IDValue = ICC.ID.InitSignUpCipher
            });
            AvatarName = string.Empty;
            Fax = string.Empty;
        }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnSignUp();
            }
        }

        [RelayCommand]
        void OnSignUp() => WeakReferenceMessenger.Default.Send<ICC>(new()
        {
            IDValue = ICC.ID.GetSignUpCipher,
            Contents = new Action<string, string>(async (inputCipher, inputCipherTest) =>
            {
                if (!string.IsNullOrEmpty(AvatarID) && !string.IsNullOrEmpty(inputCipher) && inputCipher == inputCipherTest && !string.IsNullOrEmpty(AvatarName) && (string.IsNullOrEmpty(Fax) || Regex.IsMatch(Fax, "^.+@.+$")))
                {
                    if (await TwilightSystem.Instance.PostWwwParallel($"{QwilightComponent.TaehuiNetAPI}/avatar", Utility.SetJSON(new
                    {
                        avatarID = AvatarID,
                        avatarCipher = inputCipher,
                        avatarName = AvatarName,
                        fax = Fax
                    }), "application/json").ConfigureAwait(false))
                    {
                        Close();
                        Configure.Instance.AvatarID = AvatarID;
                        Configure.Instance.SetCipher(inputCipher);
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.SignIn, new
                        {
                            avatarID = AvatarID,
                            avatarCipher = inputCipher
                        });
                    }
                    else
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.AlreadyAvatarID);
                    }
                }
                else
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.FailedValidation);
                }
            })
        });
    }
}