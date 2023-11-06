using Qwilight.UIComponent;

namespace Qwilight.NoteFile
{
    public sealed class NotAvailableNoteFile : BaseNoteFile
    {
        readonly string _noteID;

        public override NoteVariety NoteVarietyValue => NotAvailableNoteVarietyValue;

        public NoteVariety NotAvailableNoteVarietyValue { get; set; }

        public override bool IsLogical => true;

        public override bool Equals(object obj) => obj is NotAvailableNoteFile notAvailableNoteFile && _noteID == notAvailableNoteFile._noteID;

        public override int GetHashCode() => _noteID.GetHashCode();

        public NotAvailableNoteFile(string noteID, DefaultEntryItem defaultEntryItem, EntryItem entryItem) : base(default, defaultEntryItem, entryItem)
        {
            _noteID = noteID;
            IsBanned = true;
            DB.Instance.GetEventNoteData(noteID, this);
        }
    }
}