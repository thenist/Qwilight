using System.Diagnostics;

namespace Qwilight.Utilities
{
    public struct TestMillis : IDisposable
    {
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        readonly double _targetMillis;
        readonly object _title;

        public TestMillis(double targetMillis = 0.0, object title = null)
        {
            _targetMillis = targetMillis;
            _title = title;
        }

        public void Dispose()
        {
            var millis = _loopingHandler.GetMillis();
            if (millis >= _targetMillis)
            {
                if (_title != null)
                {
                    Console.WriteLine("{0} {1}", _title, millis);
                }
                else
                {
                    Console.WriteLine(millis);
                }
            }
        }
    }
}
