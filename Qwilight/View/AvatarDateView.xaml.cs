using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.View
{
    public partial class AvatarDateView
    {
        readonly VisualCollection _targets;
        readonly DrawingVisual _target = new();

        public AvatarDateView()
        {
            _targets = new(this);
            _targets.Add(_target);

            InitializeComponent();
        }

        void OnVisibilityModified(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                WPF.Paint += OnPaint;
            }
            else
            {
                WPF.Paint -= OnPaint;
            }
        }

        protected override int VisualChildrenCount => _targets.Count;

        protected override Visual GetVisualChild(int index) => _targets[index];

        void OnPaint(object sender, object e)
        {
            var defaultHeight = RenderSize.Height;
            var dateValues = ViewModels.Instance.AvatarValue.DateValues;
            var dateValuesLength = dateValues.Length;
            var dateValueLength = RenderSize.Width / dateValuesLength;
            var dateValuesMax = dateValues.Max();

            using var targetSession = _target.RenderOpen();
            for (var i = dateValuesLength - 1; i >= 0; --i)
            {
                targetSession.DrawRectangle(Paints.Paint4, null, new Bound(dateValueLength * i, defaultHeight * (dateValuesMax - dateValues[i]) / dateValuesMax, dateValueLength, defaultHeight * dateValues[i] / dateValuesMax));
            }
        }
    }
}