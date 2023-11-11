namespace Qwilight
{
    public sealed class HandlableAudioHandler : IAudioHandler
    {
        bool? _isHandling;

        public bool? IsHandling
        {
            get => _isHandling;

            set
            {
                _isHandling = value;
                if (value == false)
                {
                    AudioSystem.Instance.Stop(this);
                }
            }
        }
    }
}