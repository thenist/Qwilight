using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.Utilities;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class EnrollViewModel : BaseViewModel
    {
        [GeneratedRegex("^.+@.+$")]
        private static partial Regex GetFaxComputer();

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
            StrongReferenceMessenger.Default.Send<InitEnrollCipher>();
            AvatarName = string.Empty;
            Fax = string.Empty;
        }

        public async Task OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await OnEnroll();
            }
        }

        [RelayCommand]
        async Task OnEnroll()
        {
            var (inputCipher, inputCipherTest) = StrongReferenceMessenger.Default.Send<GetEnrollCipher>().Response;
            if (!string.IsNullOrEmpty(AvatarID) && !string.IsNullOrEmpty(inputCipher) && inputCipher == inputCipherTest && !string.IsNullOrEmpty(AvatarName) && (string.IsNullOrEmpty(Fax) || GetFaxComputer().IsMatch(Fax)))
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
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.LogIn, new
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
        }
    }
}