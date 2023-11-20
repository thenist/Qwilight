using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;

namespace Qwilight.ViewModel
{
    public sealed partial class BundleViewModel : BaseViewModel
    {
        string _targetBundleAvatar = string.Empty;

        public ObservableCollection<BundleItem> NoteFileBundleItemCollection { get; } = new();

        public ObservableCollection<BundleItem> UIBundleItemCollection { get; } = new();

        public ObservableCollection<BundleItem> QwilightBundleItemCollection { get; } = new();

        public ObservableCollection<BundleItem> EventNoteBundleItemCollection { get; } = new();

        public override double TargetLength => 0.8;

        public bool IsMe => TargetBundleAvatar == TwilightSystem.Instance.AvatarID;

        public double Value => BundleLength > 0L ? 100.0 * TargetValue / BundleLength : 0.0;

        public string TargetBundleAvatar
        {
            get => _targetBundleAvatar;

            set
            {
                _targetBundleAvatar = value;
                NotifyIsMe();
            }
        }

        public long TargetValue { get; set; }

        public long BundleLength { get; set; }

        public string ValueContents => $"{Utility.FormatLength(TargetValue)}／{Utility.FormatLength(BundleLength)}";

        [RelayCommand]
        static void OnSaveAsBundle() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
        {
            bundleVariety = BundleItem.BundleVariety.Qwilight
        });


        public void NotifyIsMe() => OnPropertyChanged(nameof(IsMe));

        public void NotifyUI()
        {
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(ValueContents));
        }
    }
}