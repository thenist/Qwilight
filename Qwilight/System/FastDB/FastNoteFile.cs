using Qwilight.NoteFile;

namespace Qwilight
{
    public struct FastNoteFile
    {
        public Component.InputMode inputMode = Component.InputMode.InputMode51;
        public string artist = string.Empty;
        public string title = string.Empty;
        public string genre = string.Empty;
        public string levelText = string.Empty;
        public double levelTextValue = double.NaN;
        public double bpm = Component.StandardBPM;
        public double judgmentStage = 5.0;
        public double hitPointsValue = 0.01;
        public BaseNoteFile.Level levelValue = BaseNoteFile.Level.Level0;
        public string noteDrawingName = string.Empty;
        public string bannerDrawingName = string.Empty;
        public string trailerAudioName = string.Empty;
        public int totalNotes;
        public int autoableNotes;
        public int longNotes;
        public int trapNotes;
        public double length;
        public string tag = string.Empty;
        public bool isBanned;
        public double lowestBPM = Component.StandardBPM;
        public double highestBPM = Component.StandardBPM;
        public int highestInputCount;
        public bool isHellBPM;
        public bool isAutoLongNote;
        public bool isSalt;
        public string assistFileName = string.Empty;

        public FastNoteFile()
        {
        }
    }
}
