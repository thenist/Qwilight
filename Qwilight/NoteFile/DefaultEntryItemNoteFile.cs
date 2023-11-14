using Qwilight.UIComponent;
using Qwilight.ViewModel;
using System.Windows.Media;

namespace Qwilight.NoteFile
{
    public sealed class DefaultEntryItemNoteFile : BaseNoteFile
    {
        public override NoteVariety NoteVarietyValue => default;

        public override bool IsLogical => true;

        public override bool Equals(object obj) => obj is DefaultEntryItemNoteFile defaultEntryItemNoteFile && defaultEntryItemNoteFile.DefaultEntryItem == DefaultEntryItem;

        public override int GetHashCode() => DefaultEntryItem.GetHashCode();

        public override ImageSource EssentialDrawing => BaseUI.Instance.DefaultEntryDrawings[(int)DefaultEntryItem.DefaultEntryVarietyValue];

        public DefaultEntryItemNoteFile(DefaultEntryItem defaultEntryItem, EntryItem entryItem, bool isEnter) : base(default, defaultEntryItem, entryItem)
        {
            Title = entryItem.Title;
            Artist = entryItem.Artist;
            OnLevyNoteFile = new(() =>
            {
                ViewModels.Instance.MainValue.SetLastDefaultEntryItem(isEnter ? DefaultEntryItem : null);
            });
        }
    }
}