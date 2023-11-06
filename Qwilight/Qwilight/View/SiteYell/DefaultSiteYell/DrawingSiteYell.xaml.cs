using Qwilight.Utilities;
using System.Windows.Input;

namespace Qwilight.View.SiteYell
{
    public partial class DrawingSiteYell : ISiteYell
    {
        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Target { get; }

        public bool HasAudio { get; }

        public bool HasDrawing { get; }

        public bool HasMedia { get; }

        public bool HasNotTarget { get; }

        public AvatarWww AvatarWwwValue { get; }

        public DrawingSiteYell(string avatarID, string avatarName, string date, string siteYell, string target, int siteYellID)
        {
            SiteYellID = siteYellID;
            AvatarID = avatarID;
            AvatarName = avatarName;
            Date = date;
            SiteYell = siteYell;
            Target = target;
            AvatarWwwValue = new AvatarWww(avatarID);
            InitializeComponent();
        }

        void OnPointLower(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.OpenAs(Target);
            }
        }
    }
}
