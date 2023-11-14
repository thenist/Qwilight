using CommunityToolkit.Mvvm.ComponentModel;
using Qwilight.Utilities;
using System.Windows.Input;
using System.Windows.Media;

namespace Qwilight.View.SiteYell
{
    [INotifyPropertyChanged]
    public partial class DefaultSiteYell : ISiteYell
    {
        string _siteYell;

        public int SiteYellID { get; }

        public string SiteYell
        {
            get => _siteYell;

            set => SetProperty(ref _siteYell, value, nameof(SiteYell));
        }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Href { get; }

        public AvatarWww AvatarWwwValue { get; }

        public Brush HrefPaint => string.IsNullOrEmpty(Href) ? Paints.Paint4 : QwilightComponent.GetBuiltInData<Brush>("SiteHrefPaint");

        public DefaultSiteYell(string avatarID, string avatarName, string date, string siteYell, string href, int siteYellID)
        {
            SiteYellID = siteYellID;
            AvatarID = avatarID;
            AvatarName = avatarName;
            Date = date;
            SiteYell = siteYell;
            Href = href;
            AvatarWwwValue = new AvatarWww(avatarID);
            InitializeComponent();
        }

        void OnOpenAs(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.OpenAs(Href);
            }
        }
    }
}
