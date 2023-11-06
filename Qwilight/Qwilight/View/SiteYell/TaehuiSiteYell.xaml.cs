using CommunityToolkit.Mvvm.ComponentModel;
using Qwilight.Utilities;
using System.Windows.Input;
using System.Windows.Media;

namespace Qwilight.View.SiteYell
{
    [INotifyPropertyChanged]
    public partial class TaehuiSiteYell : ISiteYell
    {
        string _siteYell;

        public int SiteYellID { get; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string SiteYell
        {
            get => _siteYell;

            set => SetProperty(ref _siteYell, value, nameof(SiteYell));
        }

        public string Date { get; }

        public string Href { get; }

        public Brush HrefPaint => string.IsNullOrEmpty(Href) ? Paints.Paint4 : QwilightComponent.GetBuiltInData<Brush>("SiteHrefPaint");

        public TaehuiSiteYell(string siteYell, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            SiteYell = siteYell;
            Date = date;
            Href = Utility.CompileSiteYells(siteYell);
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
