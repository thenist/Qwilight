using CommunityToolkit.Mvvm.Input;
using Qwilight.Utilities;

namespace Qwilight.ViewModel
{
    public sealed partial class AssistViewModel : BaseViewModel
    {
        public override double TargetLength => 0.8;

        [RelayCommand]
        static void OnOpenAs(string e) => Utility.OpenAs(e);

        [RelayCommand]
        static void OnNewBMS() => ViewModels.Instance.VoteValue.Open();

        [RelayCommand]
        static void OnLoadBMS() => ViewModels.Instance.ModifyDefaultEntryValue.Open();

        [RelayCommand]
        static void OnSetUI()
        {
            ViewModels.Instance.ConfigureValue.TabPosition = 0;
            ViewModels.Instance.ConfigureValue.TabPositionComputing = 1;
            ViewModels.Instance.ConfigureValue.Open();
        }

        [RelayCommand]
        static void OnSetInput()
        {
            ViewModels.Instance.ConfigureValue.TabPosition = 1;
            ViewModels.Instance.ConfigureValue.Open();
            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.Tutorial220);
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            Configure.Instance.NotifyTutorial(Configure.TutorialID.F1Assist);
        }
    }
}