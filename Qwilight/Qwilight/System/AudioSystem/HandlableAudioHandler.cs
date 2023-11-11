namespace Qwilight
{
    public sealed class HandlableAudioHandler : IAudioHandler
    {
        public bool? IsHandling { get; set; }

        public void Stop()
        {
            if (IsHandling.HasValue)
            {
                AudioSystem.Instance.Stop(this);
                IsHandling = false;
            }
        }
    }
}