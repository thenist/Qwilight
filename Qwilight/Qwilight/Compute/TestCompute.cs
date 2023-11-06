using Qwilight.NoteFile;

namespace Qwilight.Compute
{
    public sealed class TestCompute : DefaultCompute
    {
        public TestCompute(BaseNoteFile noteFile) : base(new[] { noteFile }, null, null, string.Empty, string.Empty)
        {
        }

        public override bool LoadContents => false;

        public override void OnCompiled()
        {
        }
    }
}