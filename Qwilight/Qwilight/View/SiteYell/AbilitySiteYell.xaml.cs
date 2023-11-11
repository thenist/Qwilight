using Qwilight.Utilities;

namespace Qwilight.View.SiteYell
{
    public partial class AbilitySiteYell : ISiteYell
    {
        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Text { get; }

        public AvatarWww AvatarWwwValue { get; }

        public AbilitySiteYell(string siteYell, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            var twilightAbilitySiteYell = Utility.GetJSON<JSON.TwilightAbilitySiteYell>(siteYell);
            AvatarID = twilightAbilitySiteYell.avatarID;
            AvatarName = twilightAbilitySiteYell.avatarName;
            Date = date;
            Text = twilightAbilitySiteYell.ToString();
            AvatarWwwValue = new AvatarWww(twilightAbilitySiteYell.avatarID);
            InitializeComponent();
        }
    }
}
