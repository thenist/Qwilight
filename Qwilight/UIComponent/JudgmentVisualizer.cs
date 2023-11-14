namespace Qwilight.UIComponent
{
    public sealed class JudgmentVisualizer
    {
        double _loopingCounter;

        public double LoopingCounter { get; set; }

        public double Judgment { get; init; }

        public Component.Judged Judged { get; init; }

        public double Status => LoopingCounter / _loopingCounter;

        public JudgmentVisualizer(double judgment, Component.Judged judged)
        {
            _loopingCounter = Configure.Instance.JudgmentVisualizerMillis;
            LoopingCounter = _loopingCounter;
            Judgment = judgment;
            Judged = judged;
        }
    }
}
