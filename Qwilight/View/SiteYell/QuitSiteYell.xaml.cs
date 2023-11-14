namespace Qwilight.View.SiteYell
{
    public partial class QuitSiteYell : ISiteYell
    {
        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public AvatarWww AvatarWwwValue { get; }

        public QuitSiteYell(string avatarID, string avatarName, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            AvatarID = avatarID;
            AvatarName = avatarName;
            Date = date;
            AvatarWwwValue = new AvatarWww(avatarID);
            InitializeComponent();
        }
    }
}
