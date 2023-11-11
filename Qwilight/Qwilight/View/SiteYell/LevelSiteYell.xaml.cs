using Qwilight.Utilities;

namespace Qwilight.View.SiteYell
{
    public partial class LevelSiteYell : ISiteYell
    {
        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Text { get; }

        public AvatarWww AvatarWwwValue { get; }

        public LevelSiteYell(string siteYell, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            var twilightLevelSiteYell = Utility.GetJSON<JSON.TwilightLevelSiteYell>(siteYell);
            AvatarID = twilightLevelSiteYell.avatarID;
            AvatarName = twilightLevelSiteYell.avatarName;
            Date = date;
            Text = twilightLevelSiteYell.ToString();
            AvatarWwwValue = new AvatarWww(twilightLevelSiteYell.avatarID);
            InitializeComponent();
        }
    }
}
