using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;

namespace Qwilight.Compute
{
    public sealed class CommentCompute : DefaultCompute
    {
        public override bool IsPassable => false;

        public override bool CanIO => false;

        public override bool CanSetPosition => true;

        public override string PlatformVarietyContents => LanguageSystem.Instance.PlatformCommentComputing;

        public override void HandleWarning()
        {
            if (ModeComponentValue.IsNoteSaltModeWarning(Comment.Date))
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InputSaltCommentWarning);
            }
        }

        public CommentCompute(BaseNoteFile[] noteFiles, Comment[] comments, ModeComponent defaultModeComponentValue, string avatarID, string avatarName, string handlerID, EntryItem eventNoteEntryItem, DefaultCompute lastComputer, double levyingWait) : base(noteFiles, comments, defaultModeComponentValue, avatarID, avatarName, null, handlerID, eventNoteEntryItem, lastComputer)
        {
            if (Utility.IsLowerDate(Version.Parse(Comment.Date), 1, 1, 0))
            {
                Comment.LoopUnit = 1000;
            }
            EventComment = Comment.Clone();
            LevyingWait = levyingWait;
        }

        public override ICollection<CommentItem> HandleTwilightNetItems(CommentItem[] commentItems) => commentItems.Where(commentItem => commentItem.AvatarWwwValue.AvatarID != AvatarID).ToArray();

        public override void InitPassable()
        {
            ModeComponentValue.SentMultiplier = LevyingMultiplier;
            ModeComponentValue.AudioMultiplier = LevyingAudioMultiplier;
        }

        public override void AtNoteFileMode()
        {
            Close();
        }

        public override void AtQuitMode()
        {
        }

        public override void SendSituation()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsNoteFileMode)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
                {
                    situationValue = (int)(mainViewModel.IsQuitMode ? UbuntuItem.UbuntuSituation.QuitMode : UbuntuItem.UbuntuSituation.CommentComputing),
                    situationText = PlatformText
                });
            }
        }

        public override bool IsSuitableAsHwInput(int input) => false;
    }
}