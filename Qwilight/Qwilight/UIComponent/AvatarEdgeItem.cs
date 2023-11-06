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
                        using var s = await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/edge?edgeID={EdgeID}");
                        if (s.Length > 0)
                        {
                            try
                            {
                                SetProperty(ref _drawing, DrawingSystem.Instance.LoadDefault(s, null), nameof(Drawing));
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            SetProperty(ref _drawing, null, nameof(Drawing));
                        }
                    });
                }
                return _drawing;
            }
        }
    }
}
