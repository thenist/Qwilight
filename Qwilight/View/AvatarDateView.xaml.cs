using Qwilight.ViewModel;
using System.Windows.Media;

namespace Qwilight.View
{
    public partial class AvatarDateView
    {
        readonly DrawingGroup _target = new();

        public AvatarDateView()
        {
            InitializeComponent();
            IsVisibleChanged += (sender, e) =>
            {
                if ((bool)e.NewValue)
                {
                    CompositionTarget.Rendering += OnPaint;
                }
                else
                {
                    CompositionTarget.Rendering -= OnPaint;
                }
            };
        }

        void OnPaint(object sender, object e)
        {
            var viewHeight = RenderSize.Height;
            var dateValues = ViewModels.Instance.AvatarValue.DateValues;
            var dateValuesLength = dateValues.Length;
            var dateValueLength = RenderSize.Width / dateValuesLength;
            var highestDateValue = dateValues.Max();

            using var targetSession = _target.Open();
            for (var i = dateValuesLength - 1; i >= 0; --i)
            {
                targetSession.DrawRectangle(Paints.Paint4, null, new Bound(dateValueLength * i, viewHeight * (highestDateValue - dateValues[i]) / highestDateValue, dateValueLength, viewHeight * dateValues[i] / highestDateValue));
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawDrawing(_target);
        }
    }
}