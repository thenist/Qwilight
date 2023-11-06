using FMOD;

namespace Qwilight
{
    public struct AudioItem : IDisposable
    {
        public nint System { get; set; }

        public Sound AudioData { get; set; }

        public string BMSID { get; set; }

        public float AudioVolume { get; set; }

        public uint Length { get; set; }

        public void Dispose() => AudioData.release();
    }
}