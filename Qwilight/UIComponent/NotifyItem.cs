using System.Buffers;
using System.IO;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class NotifyItem : Model, IDisposable
    {
        NotifySystem.NotifyVariety _toNotifyVariety;
        string _text;

        public byte[] Data { get; set; }

        public MemoryStream DataFlow { get; set; }

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public DateTime Date { get; } = DateTime.Now;

        public bool IsNew { get; set; }

        public bool IsStopped { get; set; }

        public double BundleStatus { get; set; }

        public Action OnHandle { get; init; }

        public Func<bool, bool> OnStop { get; set; }

        public NotifySystem.NotifyVariety Variety
        {
            get => _toNotifyVariety;

            set => SetProperty(ref _toNotifyVariety, value, nameof(Variety));
        }

        public long LevyingStatus { get; set; }

        public long QuitStatus { get; set; }

        public void NotifyBundleStatus()
        {
            if (QuitStatus > 0)
            {
                var bundleStatus = 100.0 * LevyingStatus / QuitStatus;
                if (bundleStatus == 100.0 || Math.Abs(bundleStatus - BundleStatus) >= 1.0)
                {
                    BundleStatus = bundleStatus;
                    OnPropertyChanged(nameof(BundleStatus));
                }
            }
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