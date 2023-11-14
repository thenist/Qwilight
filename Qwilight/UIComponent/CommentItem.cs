using Qwilight.Compute;
using Qwilight.Utilities;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class CommentItem : Model
    {
        string _twilightCommentary;
        DefaultCompute.InputFlag _inputFlags;

        public DefaultCompute.QuitStatus Quit => Utility.GetQuitStatusValue(Point, Stand, 1.0, NoteFileCount);

        public Brush PointedPaint => BaseUI.Instance.CommentViewPaints[(int)Quit];

        public int NoteFileCount { get; init; }

        public DateTime Date { get; init; }

        public string DateText { get; init; }

        public string CommentPlace0Text { get; set; }

        public string CommentPlace1Text { get; set; }

        public Brush StandPaint => ModeComponentValue.HitPointsModeValue switch
        {
            ModeComponent.HitPointsMode.Lowest => BaseUI.Instance.LevelPaints[1],
            ModeComponent.HitPointsMode.Lower => BaseUI.Instance.LevelPaints[2],
            ModeComponent.HitPointsMode.Higher => BaseUI.Instance.LevelPaints[4],
            ModeComponent.HitPointsMode.Highest => BaseUI.Instance.LevelPaints[5],
            ModeComponent.HitPointsMode.Failed => BaseUI.Instance.LevelPaints[5],
            _ => QwilightComponent.GetBuiltInData<Brush>("CommentStandPaint")
        };

        public Brush PointPaint => ModeComponentValue.JudgmentModeValue switch
        {
            ModeComponent.JudgmentMode.Lowest => BaseUI.Instance.LevelPaints[1],
            ModeComponent.JudgmentMode.Lower => BaseUI.Instance.LevelPaints[2],
            ModeComponent.JudgmentMode.Higher => BaseUI.Instance.LevelPaints[4],
            ModeComponent.JudgmentMode.Highest => BaseUI.Instance.LevelPaints[5],
            _ => QwilightComponent.GetBuiltInData<Brush>("CommentPointPaint")
        };

        public ImageSource QuitDrawing => BaseUI.Instance.QuitDrawings[(int)Quit][IsP ? 1 : 0]?.DefaultDrawing;

        public bool IsTwilightComment { get; init; }

        public string CommentID { get; init; }

        public string AvatarName { get; init; }

        public ModeComponent ModeComponentValue { get; init; }

        public int Stand { get; init; }

        public string StandText => Stand.ToString(LanguageSystem.Instance.StandContents);

        public int Band { get; init; }

        public double Point { get; init; }

        public bool IsP { get; init; }

        public string TwilightCommentary
        {
            get => _twilightCommentary;

            set => SetProperty(ref _twilightCommentary, value, nameof(TwilightCommentary));
        }

        public bool IsPaused { get; init; }

        public bool DefaultControllerComputed => (_inputFlags & DefaultCompute.InputFlag.DefaultController) == DefaultCompute.InputFlag.DefaultController;

        public bool ControllerComputed => (_inputFlags & DefaultCompute.InputFlag.Controller) == DefaultCompute.InputFlag.Controller;

        public bool MIDIComputed => (_inputFlags & DefaultCompute.InputFlag.MIDI) == DefaultCompute.InputFlag.MIDI;

        public bool PointerComputed => (_inputFlags & DefaultCompute.InputFlag.Pointer) == DefaultCompute.InputFlag.Pointer;

        public AvatarWww AvatarWwwValue { get; }

        public CommentItem(string avatarID, DefaultCompute.InputFlag inputFlags)
        {
            AvatarWwwValue = new AvatarWww(avatarID);
            _inputFlags = inputFlags;
        }
    }
}