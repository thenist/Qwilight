using Qwilight.NoteFile;
using Qwilight.ViewModel;

namespace Qwilight.Compute
{
    public class FlintCompute : AutoCompute
    {
        public override bool IsMeterVisible => true;

        public FlintCompute(BaseNoteFile noteFile, ModeComponent defaultModeComponentValue, string avatarID, string avatarName, int levyingMeter) : base(new[] { noteFile }, defaultModeComponentValue, avatarID, avatarName, levyingMeter, double.NaN)
        {
        }

        public override void AtNoteFileMode()
        {
            Close();
            var mainViewModel = ViewModels.Instance.MainValue;
            var entryItemValue = mainViewModel.EntryItemValue;
            if (entryItemValue != null)
            {
                mainViewModel.LoadEntryItem(entryItemValue.DefaultEntryItem, entryItemValue.EntryPath);
            }
        }
    }
}