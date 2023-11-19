using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Qwilight.UIComponent
{
    public sealed partial class BundleItem
    {
        public enum BundleVariety
        {
            DefaultUI = -2,
            DefaultNotes = -1,
            UI = 1,
            Qwilight = 2,
            EventNote = 3,
            Note = 4,
            Net = 5
        }

        BundleCompetence _bundleCompetence;

        public string Date { get; init; }

        public string Avatar { get; init; }

        public string Name { get; init; }

        public long Length { get; init; }

        public BundleVariety Variety { get; init; }

        public bool IsNameVisible => Variety != BundleVariety.Qwilight;

        public BundleCompetence BundleCompetence
        {
            get => _bundleCompetence;

            set
            {
                _bundleCompetence = value;
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetBundle, new
                {
                    bundleName = Name,
                    bundleCompetence = value.Data
                });
            }
        }

        public ICollection<BundleItem> BundleItemCollection { get; set; }

        [RelayCommand]
        void OnSave() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveBundle, new
        {
            avatarID = Avatar,
            bundleName = Name
        });

        [RelayCommand]
        void OnWipe()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.WipeBundleNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                BundleItemCollection.Remove(this);
                var bundleViewModel = ViewModels.Instance.BundleValue;
                bundleViewModel.TargetValue -= Length;
                bundleViewModel.NotifyUI();
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.WipeBundle, Name);
            }
        }

        public BundleItem(BundleCompetence bundleCompetence) => _bundleCompetence = bundleCompetence;
    }
}