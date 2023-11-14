using Qwilight.ViewModel;
using System.Windows.Media;

namespace Qwilight.NoteFile
{
    public sealed class LevelNoteFile : BaseNoteFile
    {
        readonly string _save128 = string.Empty;
        readonly string _save256 = string.Empty;

        public override NoteVariety NoteVarietyValue { get; } = NoteVariety.BMS;

        public override ImageSource HandledWallDrawing => null;

        public override ImageSource EssentialDrawing => DrawingSystem.Instance.ClearedDrawing.DefaultDrawing;

        public override bool IsLogical => true;

        public override bool Equals(object obj) => obj is LevelNoteFile levelNoteFile && _save128 == levelNoteFile._save128 && _save256 == levelNoteFile._save256;

        public override int GetHashCode() => HashCode.Combine(_save128, _save256);

        public LevelNoteFile(JSON.BMSTableData levelTableData, string wantLevelID) : base(default, default, default)
        {
            Title = levelTableData.title ?? string.Empty;
            Artist = levelTableData.artist ?? string.Empty;
            WantLevelID = wantLevelID;
            Tag = levelTableData.comment ?? string.Empty;
            if (!string.IsNullOrEmpty(levelTableData.md5))
            {
                _save128 = $"{levelTableData.md5}:{DataID}";
            }
            if (!string.IsNullOrEmpty(levelTableData.sha256))
            {
                _save256 = $"{levelTableData.sha256}:{DataID}";
            }
            OnLevyNoteFile = new(() =>
            {
                try
                {
                    ViewModels.Instance.LevelVoteValue.Www0 = new Uri(levelTableData.url_diff);
                    ViewModels.Instance.LevelVoteValue.IsWww0Visible = true;
                }
                catch
                {
                    ViewModels.Instance.LevelVoteValue.IsWww0Visible = false;
                }
                try
                {
                    ViewModels.Instance.LevelVoteValue.Www1 = new Uri(levelTableData.url_diff);
                    ViewModels.Instance.LevelVoteValue.IsWww1Visible = true;
                }
                catch
                {
                    ViewModels.Instance.LevelVoteValue.IsWww1Visible = false;
                }
                if (ViewModels.Instance.LevelVoteValue.IsWww0Visible || ViewModels.Instance.LevelVoteValue.IsWww1Visible)
                {
                    ViewModels.Instance.LevelVoteValue.Open();
                }
                else
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAvailableLevelWww);
                }
            });
        }

        public override string GetNoteID128(byte[] data = null) => _save128;

        public override string GetNoteID256(byte[] data = null) => _save256;
    }
}