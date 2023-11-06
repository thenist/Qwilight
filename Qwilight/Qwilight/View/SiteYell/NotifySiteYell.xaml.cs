namespace Qwilight.View.SiteYell
{
    public partial class NotifySiteYell : ISiteYell
    {
        public int SiteYellID { get; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string SiteYell { get; set; }

        public NotifySiteYell(string siteYell, int siteYellID)
        {
            SiteYellID = siteYellID;
            SiteYell = siteYell;
            InitializeComponent();
        }
    }
}
