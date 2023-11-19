using CommunityToolkit.Mvvm.ComponentModel;
using Qwilight.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Qwilight.View.SiteYell
{
    [INotifyPropertyChanged]
    public partial class MediaSiteYell : ISiteYell
    {
        bool _isStopped = true;
        double _mediaPosition;

        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Target { get; }

        public AvatarWww AvatarWwwValue { get; }

        public double MediaPosition
        {
            get => _mediaPosition;

            set
            {
                if (SetProperty(ref _mediaPosition, value, nameof(MediaPosition)))
                {
                    Utility.SetPosition(MediaView, value);
                    OnPropertyChanged(nameof(HandledText));
                }
            }
        }

        public string HandledText => Utility.GetHandledText(MediaView);

        public bool IsStopped
        {
            get => _isStopped;

            set => SetProperty(ref _isStopped, value, nameof(IsStopped));
        }

        public Brush TargetPaint => string.IsNullOrEmpty(Target) ? Paints.Paint4 : QwilightComponent.GetBuiltInData<Brush>("SiteHrefPaint");

        public MediaSiteYell(string avatarID, string avatarName, string date, string siteYell, string target, int siteYellID)
        {
            SiteYellID = siteYellID;
            AvatarID = avatarID;
            AvatarName = avatarName;
            Date = date;
            SiteYell = siteYell;
            Target = target;
            AvatarWwwValue = new AvatarWww(avatarID);
            InitializeComponent();

            var mediaViewHandler = new DispatcherTimer(TimeSpan.FromSeconds(1.0), DispatcherPriority.Background, (sender, e) =>
            {
                if (MediaView.NaturalDuration.HasTimeSpan)
                {
                    MediaPosition = 100.0 * MediaView.Position / MediaView.NaturalDuration.TimeSpan;
                }
            }, UIHandler.Instance.Handler)
            {
                IsEnabled = false
            };
            MediaView.Loaded += (sender, e) =>
            {
                mediaViewHandler.Start();
            };
            MediaView.Unloaded += (sender, e) =>
            {
                mediaViewHandler.Stop();
            };
        }

        void OnOpenAs(object sender, MouseButtonEventArgs e) => Utility.OpenAs(Target);

        void OnHandle(object sender, RoutedEventArgs e)
        {
            if (IsStopped)
            {
                MediaView.Play();
            }
            else
            {
                MediaView.Pause();
            }
            IsStopped = !IsStopped;
        }

        void OnStop(object sender, RoutedEventArgs e)
        {
            IsStopped = true;
            MediaView.Stop();
        }
    }
}
