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

        string _avatarID;
        string _avatarName;
        string _fax;

        public override double TargetLength => 0.2;

        public override double TargetHeight => double.NaN;

        public override HorizontalAlignment LengthSystem => HorizontalAlignment.Center;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Center;

        public string AvatarID
        {
            get => _avatarID;

            set => SetProperty(ref _avatarID, value, nameof(AvatarID));
        }

        public string AvatarName
        {
            get => _avatarName;

            set => SetProperty(ref _avatarName, value, nameof(AvatarName));
        }


        public string Fax
        {
            get => _fax;

            set => SetProperty(ref _fax, value, nameof(Fax));
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
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
                    Configure.Instance.AvatarID = AvatarID;
                    Configure.Instance.SetCipher(inputCipher);
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.LogIn, new
                    {
                        avatarID = AvatarID,
                        avatarCipher = inputCipher
                    });
                    Close();
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