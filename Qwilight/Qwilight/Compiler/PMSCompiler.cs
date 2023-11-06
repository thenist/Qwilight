using Qwilight.NoteFile;

namespace Qwilight.Compiler
{
    public sealed class PMSCompiler : BMSCompiler
    {
        public PMSCompiler(PMSFile pmsFile, CancellationTokenSource setCancelCompiler) : base(pmsFile, setCancelCompiler)
        {
        }

        public override Component.InputMode GetInputMode(ICollection<int> inputSet) => Component.InputMode.InputMode9;
    }
}