using Qwilight.Utilities;
using System.Windows.Input;

namespace Qwilight.View.SiteYell
{
    public partial class TVSiteYell : ISiteYell
    {
        public string _href;

        public int SiteYellID { get; }

        public string AvatarID { get; }

        public string SiteYell { get; set; }

        public string AvatarName { get; }

        public string Date { get; }

        public string SiteYellTV { get; }

        public TVSiteYell(string avatarID, string siteYell, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            var twilightTVSiteYell = Utility.GetJSON<JSON.TwilightTVSiteYell>(siteYell);
            _href = twilightTVSiteYell.href;
            SiteYellTV = string.Format(LanguageSystem.Instance.SiteYellTV, twilightTVSiteYell.title);
            AvatarName = twilightTVSiteYell.text;
            Date = date;
            InitializeComponent();
        }

        void OnPointLower(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.OpenAs(_href);
            }
        }
    }
}
