using Qwilight.Utilities;
using System.Windows.Media;

namespace Qwilight.View.SiteYell
{
    public partial class CommentSiteYell : ISiteYell
    {
        public int SiteYellID { get; }

        public string SiteYell { get; set; }

        public string AvatarID { get; }

        public string AvatarName { get; }

        public string Date { get; }

        public string Artist { get; }

        public string Title { get; }

        public string Genre { get; }

        public string LevelText { get; }

        public Brush LevelTextPaint { get; }

        public string Stand { get; }

        public Brush StandPaint { get; }

        public AvatarWww AvatarWwwValue { get; }

        public CommentSiteYell(string siteYell, string date, int siteYellID)
        {
            SiteYellID = siteYellID;
            var twilightCommentSiteYell = Utility.GetJSON<JSON.TwilightCommentSiteYell>(siteYell);
            AvatarID = twilightCommentSiteYell.avatarID;
            AvatarName = twilightCommentSiteYell.avatarName;
            Date = date;
            Artist = twilightCommentSiteYell.artist;
            Title = twilightCommentSiteYell.title;
            Genre = Utility.GetGenreText(twilightCommentSiteYell.genre);
            LevelText = twilightCommentSiteYell.levelText;
            LevelTextPaint = BaseUI.Instance.LevelPaints[(int)twilightCommentSiteYell.level];
            Stand = twilightCommentSiteYell.stand.ToString(LanguageSystem.Instance.StandContents);
            StandPaint = (ModeComponent.HitPointsMode)twilightCommentSiteYell.hitPointsMode switch
            {
                ModeComponent.HitPointsMode.Lowest => BaseUI.Instance.LevelPaints[1],
                ModeComponent.HitPointsMode.Lower => BaseUI.Instance.LevelPaints[2],
                ModeComponent.HitPointsMode.Higher => BaseUI.Instance.LevelPaints[4],
                ModeComponent.HitPointsMode.Highest => BaseUI.Instance.LevelPaints[5],
                ModeComponent.HitPointsMode.Failed => BaseUI.Instance.LevelPaints[5],
                _ => QwilightComponent.GetBuiltInData<Brush>("SiteStandPaint")
            };
            AvatarWwwValue = new AvatarWww(twilightCommentSiteYell.avatarID);
            InitializeComponent();
        }
    }
}
