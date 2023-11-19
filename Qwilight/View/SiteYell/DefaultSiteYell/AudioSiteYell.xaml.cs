using CommunityToolkit.Mvvm.ComponentModel;
using Qwilight.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Qwilight.View.SiteYell
{
    [INotifyPropertyChanged]
    public partial class AudioSiteYell : ISiteYell
    {
        bool _isStopped = true;
        double _audioPosition;

        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Target { get; }

        public AvatarWww AvatarWwwValue { get; }

        public double AudioPosition
        {
            get => _audioPosition;

            set
            {
                if (SetProperty(ref _audioPosition, value, nameof(AudioPosition)))
                {
                    Utility.SetPosition(AudioView, value);
                    OnPropertyChanged(nameof(HandledText));
                }
            }
        }

        public string HandledText => Utility.GetHandledText(AudioView);

        public bool IsStopped
        {
            get => _isStopped;

            set => SetProperty(ref _isStopped, value, nameof(IsStopped));
        }

        public AudioSiteYell(string avatarID, string avatarName, string date, string siteYell, string target, int siteYellID)
        {
            SiteYellID = siteYellID;
            AvatarID = avatarID;
            AvatarName = avatarName;
            Date = date;
            SiteYell = siteYell;
            Target = Utility.CompileSiteYells(siteYell);
            AvatarWwwValue = new(avatarID);
            InitializeComponent();

            var audioViewHandler = new DispatcherTimer(TimeSpan.FromSeconds(1.0), DispatcherPriority.Background, (sender, e) =>
            {
                if (AudioView.NaturalDuration.HasTimeSpan)
                {
                    AudioPosition = 100.0 * AudioView.Position / AudioView.NaturalDuration.TimeSpan;
                }
            }, UIHandler.Instance.Handler)
            {
                IsEnabled = false
            };
            AudioView.Loaded += (sender, e) =>
            {
                audioViewHandler.Start();
            };
            AudioView.Unloaded += (sender, e) =>
            {
                audioViewHandler.Stop();
            };
        }

        void OnHandle(object sender, RoutedEventArgs e)
        {
            if (IsStopped)
            {
                AudioView.Play();
            }
            else
            {
                AudioView.Pause();
            }
            IsStopped = !IsStopped;
        }

        void OnStop(object sender, RoutedEventArgs e)
        {
            IsStopped = true;
            AudioView.Stop();
        }
    }
}
