using CommandLine;
using Qwilight.ViewModel;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Qwilight
{
    public sealed class FlintSystem
    {
        public static readonly FlintSystem Instance = new();

        public void HandleSystem()
        {
            while (true)
            {
                try
                {
                    using var ss = new NamedPipeServerStream("Qwilight", PipeDirection.In);
                    ss.WaitForConnection();
                    using var sr = new StreamReader(ss, Encoding.UTF8);
                    Parser.Default.ParseArguments<Params.FlintParams>(sr.ReadLine()?.Split(" ", 3) ?? Array.Empty<string>()).WithParsed(o =>
                    {
                        var mainViewModel = ViewModels.Instance.MainValue;
                        var defaultComputer = mainViewModel.Computer;
                        if (o.Stop)
                        {
                            if (defaultComputer != null)
                            {
                                defaultComputer.SetPause = true;
                            }
                        }
                        else if (o.Handle)
                        {
                            var m = o.Meter;
                            var noteFile = defaultComputer?.NoteFile;
                            var noteFilePath = o.FileName.Replace("\"", string.Empty);
                            if (defaultComputer?.IsHandling == true && noteFile?.NoteFilePath == noteFilePath && noteFile.IsValid())
                            {
                                defaultComputer.LevyingMeter = m;
                                defaultComputer.SetUndo = true;
                            }
                            else
                            {
                                mainViewModel.FlintNoteFile(noteFilePath, -1, m);
                            }
                        }
                    });
                }
                catch (IOException)
                {
                    return;
                }
                catch
                {
                }
            }
        }
    }
}
