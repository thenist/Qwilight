namespace Qwilight
{
    public interface IMediaHandler
    {
        public double LoopingCounter { get; }

        public bool IsPausing { get; }

        public bool IsCounterWave { get; }

        public double AudioMultiplier { get; }
    }
}