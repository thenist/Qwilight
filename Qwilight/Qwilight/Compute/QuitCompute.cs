using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.ViewModel;

namespace Qwilight.Compute
{
    public sealed class QuitCompute : DefaultCompute
    {
        public QuitCompute(BaseNoteFile[] noteFiles, Comment[] comments, ModeComponent defaultModeComponentValue, CommentItem commentItem, EntryItem eventNoteEntryItem) : base(noteFiles, comments, defaultModeComponentValue, commentItem.AvatarWwwValue.AvatarID, commentItem.AvatarName, default, null, eventNoteEntryItem)
        {
            HighestComputingPosition = noteFiles.Length - 1;
            foreach (var noteFile in noteFiles)
            {
                InheritedTotalNotes += noteFile.TotalNotes;
            }
            foreach (var comment in comments)
            {
                InheritedHighestJudgment += comment.HighestJudgment;
                InheritedHigherJudgment += comment.HigherJudgment;
                InheritedHighJudgment += comment.HighJudgment;
                InheritedLowJudgment += comment.LowJudgment;
                InheritedLowerJudgment += comment.LowerJudgment;
                InheritedLowestJudgment += comment.LowestJudgment;
            }
            Stand.TargetValue = commentItem.Stand;
            HighestBand = commentItem.Band;
            Point.TargetValue = commentItem.Point;
            CommentPlace0Text = commentItem.CommentPlace0Text;
            CommentPlace1Text = commentItem.CommentPlace1Text;
        }

        public override void HandleWarning()
        {
        }

        public override void AtQuitMode()
        {
        }

        public override void SetCommentPlaceText()
        {
        }

        public override void SendSituation()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsNoteFileMode)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
                {
                    situationValue = (int)UbuntuItem.UbuntuSituation.QuitMode,
                    situationText = PlatformText
                });
            }
        }
    }
}