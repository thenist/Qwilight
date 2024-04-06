using CommandLine;

namespace Qwilight
{
    public static class Params
    {
        public sealed class QwilightParams
        {
            [Option("valve")]
            public bool IsValve { get; set; }

            [Option("vs")]
            public bool IsVS { get; set; }

            [Option("language")]
            public int Language { get; set; }
        }

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
    }
}
