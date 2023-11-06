using Qwilight.ViewModel;
using System.Collections.Concurrent;

namespace Qwilight
{
    public sealed class PausableAudioHandler : IAudioHandler
    {
        readonly ConcurrentDictionary<string, uint> _audioPositions = new();
        bool _isPausing;

        public string AudioFileName { get; set; }

        public bool IsPausing
        {
            get => _isPausing;

            set
            {
                _isPausing = value;
                AudioSystem.Instance.Pause(this, value);
                ViewModels.Instance.MainValue.NotifyIsPausing();
            }
        }

        public void SetAudioPosition(uint audioPosition)
        {
            if (!string.IsNullOrEmpty(AudioFileName))
            {
                _audioPositions[AudioFileName] = audioPosition;
            }
        }

        public uint GetAudioPosition()
        {
            return _audioPositions.GetValueOrDefault(AudioFileName, 0U);
        }
    }
}