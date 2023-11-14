using CommandLine;
using Qwilight.ViewModel;
using System.IO;
using System.IO.Pipes;

namespace Qwilight
{
    public sealed class FlintSystem
    {
        public sealed class FlintParams
        {
            [Option('P')]
            public bool Handle { get; set; }

            [Option('S')]
            public bool Stop { get; set; }

            [Option('N')]
            public int Meter { get; set; }

            [Value(0)]
            public string FileName { get; set; }
        }

        public static readonly FlintSystem Instance = new();

        public void HandleSystem()
        {
            while (true)
            {
                try
                {
                    using var ss = new NamedPipeServerStream("Qwilight", PipeDirection.In);
                    ss.WaitForConnection();
                    using var sr = new StreamReader(ss);
                    Parser.Default.ParseArguments<FlintParams>(sr.ReadLine()?.Split(" ", 3) ?? Array.Empty<string>()).WithParsed(o =>
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
                                mainViewModel.FlintNoteFile(noteFilePath, m);
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
