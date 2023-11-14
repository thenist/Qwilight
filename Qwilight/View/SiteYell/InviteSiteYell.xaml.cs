using Qwilight.Utilities;
using System.Windows.Input;

namespace Qwilight.View.SiteYell
{
    public partial class InviteSiteYell : ISiteYell
    {
        readonly string _siteID;

        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string SiteYellInvite { get; }

        public AvatarWww AvatarWwwValue { get; }

        public InviteSiteYell(string avatarID, string siteYell, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            var twilightInviteSiteYell = Utility.GetJSON<JSON.TwilightInviteSiteYell>(siteYell);
            _siteID = twilightInviteSiteYell.siteID;
            SiteYellInvite = string.Format(LanguageSystem.Instance.SiteYellInvite, twilightInviteSiteYell.siteName);
            AvatarID = avatarID;
            AvatarName = twilightInviteSiteYell.avatarName;
            Date = date;
            AvatarWwwValue = new AvatarWww(avatarID);
            InitializeComponent();
        }

        void OnPointLower(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.EnterSite, new
                {
                    siteID = _siteID,
                    siteCipher = string.Empty
                });
            }
        }
    }
}
