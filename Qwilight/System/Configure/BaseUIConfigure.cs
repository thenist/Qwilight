namespace Qwilight
{
    public sealed class BaseUIConfigure : Model
    {
        double _defaultMediaFaint = 0.25;
        double _defaultAudioVisualizerFaint = 0.1;

        public string[] UIConfigures { get; set; } = new string[BaseUI.HighestBaseUIConfigure];

        public double DefaultMediaFaint
        {
            get => _defaultMediaFaint;

            set => SetProperty(ref _defaultMediaFaint, value, nameof(DefaultMediaFaint));
        }

        public double DefaultAudioVisualizerFaint
        {
            get => _defaultAudioVisualizerFaint;

            set => SetProperty(ref _defaultAudioVisualizerFaint, value, nameof(DefaultAudioVisualizerFaint));
        }
    }
}
