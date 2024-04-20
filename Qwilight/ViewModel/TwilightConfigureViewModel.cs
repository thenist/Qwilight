using Qwilight.UIComponent;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed class TwilightConfigureViewModel : BaseViewModel
    {
        SilentSiteCompetence _silentSiteCompetence;
        NotifyUbuntuCompetence _toNotifyUbuntuCompetence;
        BundleCompetence _defaultBundleCompetence;
        IOCompetence _ioCompetence;
        NotifySaveBundleCompetence _toNotifySaveBundleCompetence;

        public override double TargetLength => 0.6;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        public SilentSiteCompetence SilentSiteCompetence
        {
            get => _silentSiteCompetence;

            set => SetProperty(ref _silentSiteCompetence, value, nameof(SilentSiteCompetence));
        }

        public NotifyUbuntuCompetence NotifyUbuntuCompetence
        {
            get => _toNotifyUbuntuCompetence;

            set => SetProperty(ref _toNotifyUbuntuCompetence, value, nameof(NotifyUbuntuCompetence));
        }


        public BundleCompetence DefaultBundleCompetence
        {
            get => _defaultBundleCompetence;

            set => SetProperty(ref _defaultBundleCompetence, value, nameof(DefaultBundleCompetence));
        }


        public IOCompetence IOCompetence
        {
            get => _ioCompetence;

            set => SetProperty(ref _ioCompetence, value, nameof(IOCompetence));
        }

        public NotifySaveBundleCompetence NotifySaveBundleCompetence
        {
            get => _toNotifySaveBundleCompetence;

            set => SetProperty(ref _toNotifySaveBundleCompetence, value, nameof(NotifySaveBundleCompetence));
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetConfigure, new
            {
                silentSiteCompetence = SilentSiteCompetence.Data,
                toNotifyUbuntuCompetence = NotifyUbuntuCompetence.Data,
                defaultBundleCompetence = DefaultBundleCompetence.Data,
                ioCompetence = IOCompetence.Data,
                toNotifySaveBundleCompetence = NotifySaveBundleCompetence.Data
            });
        }
    }
}