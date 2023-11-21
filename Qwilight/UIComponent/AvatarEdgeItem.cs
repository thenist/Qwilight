using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class AvatarEdgeItem : Model
    {
        bool _wantDrawing = true;
        ImageSource _drawing;

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string EdgeID { get; init; }

        public ImageSource Drawing
        {
            get
            {
                if (_wantDrawing)
                {
                    _wantDrawing = false;

                    Task.Run(async () =>
                    {
                        try
                        {
                            SetProperty(ref _drawing, DrawingSystem.Instance.LoadDefault(await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/edge?edgeID={EdgeID}").ConfigureAwait(false), null), nameof(Drawing));
                        }
                        catch
                        {
                        }
                    });
                }
                return _drawing;
            }
        }
    }
}
