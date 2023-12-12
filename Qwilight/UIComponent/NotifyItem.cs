using System.Buffers;
using System.IO;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class NotifyItem : Model, IDisposable
    {
        NotifySystem.NotifyVariety _toNotifyVariety;
        long _levyingStatus;
        long _quitStatus;
        string _text;

        public byte[] Data { get; set; }

        public MemoryStream DataFlow { get; set; }

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public DateTime Date { get; } = DateTime.Now;

        public bool IsNew { get; set; }

        public bool IsStopped { get; set; }

        public double BundleStatus => MaxStatus > 0L ? 100.0 * Status / MaxStatus : 0.0;

        public Action OnHandle { get; init; }

        public Func<bool, bool> OnStop { get; set; }

        public NotifySystem.NotifyVariety Variety
        {
            get => _toNotifyVariety;

            set => SetProperty(ref _toNotifyVariety, value, nameof(Variety));
        }

        public long Status
        {
            get => _levyingStatus;

            set => SetProperty(ref _levyingStatus, value, nameof(BundleStatus));
        }

        public long MaxStatus
        {
            get => _quitStatus;

            set => SetProperty(ref _quitStatus, value, nameof(BundleStatus));
        }

        public string Text
        {
            get => _text;

            set => SetProperty(ref _text, value, nameof(Text));
        }

        public void Dispose()
        {
            if (DataFlow != null)
            {
                DataFlow.Dispose();
                DataFlow = null;
            }
            if (Data != null)
            {
                ArrayPool<byte>.Shared.Return(Data);
                Data = null;
            }
        }
    }
}