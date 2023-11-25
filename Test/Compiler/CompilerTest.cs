using Qwilight;
using Qwilight.Compiler;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Text;
using Xunit;

namespace Test.Compiler
{
    [Collection("Test")]
    public sealed class CompilerTest
    {
        [Fact]
        public void Compile()
        {
            ViewModels.Instance.MainValue.ModeComponentValue.Salt = 0;
            var defaultEntryItem = new DefaultEntryItem();
            Parallel.ForEach(Utility.GetFiles(QwilightComponent.BundleEntryPath).Where(noteFilePath => QwilightComponent.NoteFileFormats.Any(format => noteFilePath.EndsWith(format, StringComparison.InvariantCultureIgnoreCase))).SelectMany(noteFilePath => BaseNoteFile.GetNoteFiles(noteFilePath, defaultEntryItem, new EntryItem
            {
                DefaultEntryItem = defaultEntryItem,
                EntryPath = Path.GetDirectoryName(noteFilePath)
            }, -1)), noteFile =>
            {
                var targetCompiler = BaseCompiler.GetCompiler(noteFile, null);
                var defaultComputer = new DefaultCompute(new[] { noteFile }, null, null, string.Empty, string.Empty)
                {
                    IsSilent = true
                };
                targetCompiler.Compile(defaultComputer, true);
                var lines = File.ReadAllLines(Path.ChangeExtension(noteFile.NoteFilePath, ".txt"), Encoding.UTF8);
                Assert.Equal(defaultComputer.IsAutoLongNote.ToString(), lines[0]);
                Assert.Equal(defaultComputer.IsBanned.ToString(), lines[1]);
                Assert.Equal(defaultComputer.InputMode.ToString(), lines[2]);
                Assert.Equal(defaultComputer.Genre, lines[3]);
                Assert.Equal(defaultComputer.Artist, lines[4]);
                Assert.Equal(defaultComputer.Title, lines[5]);
                Assert.Equal(defaultComputer.LevelText, lines[6]);
                Assert.Equal(defaultComputer.LevelTextValue.ToString(), lines[7]);
                Assert.Equal(defaultComputer.LevyingBPM.ToString(), lines[8]);
                Assert.Equal(defaultComputer.BPM.ToString(), lines[9]);
                Assert.Equal(defaultComputer.Length.ToString(), lines[10]);
                Assert.Equal(defaultComputer.TotalNotes.ToString(), lines[11]);
                Assert.Equal(defaultComputer.AutoableNotes.ToString(), lines[12]);
                Assert.Equal(defaultComputer.TrapNotes.ToString(), lines[13]);
                Assert.Equal(defaultComputer.LongNotes.ToString(), lines[14]);
                Assert.Equal(defaultComputer.JudgmentStage.ToString(), lines[15]);
                Assert.Equal(defaultComputer.HitPointsValue.ToString(), lines[16]);
                Assert.Equal(defaultComputer.LevelValue.ToString(), lines[17]);
                Assert.Equal(defaultComputer.NoteDrawingName, lines[18]);
                Assert.Equal(defaultComputer.BannerDrawingName, lines[19]);
                Assert.Equal(defaultComputer.TrailerAudioName, lines[20]);
                Assert.Equal(defaultComputer.AudioLevyingPosition.ToString(), lines[21]);
                Assert.Equal(defaultComputer.IsSalt.ToString(), lines[22]);
                Assert.Equal(defaultComputer.Tag, lines[23]);
                Assert.Equal(defaultComputer.LowestBPM.ToString(), lines[24]);
                Assert.Equal(defaultComputer.HighestBPM.ToString(), lines[25]);
                Assert.Equal(defaultComputer.HighestInputCount.ToString(), lines[26]);
                Assert.Equal(defaultComputer.IsHellBPM.ToString(), lines[27]);
            });
        }
    }
}
