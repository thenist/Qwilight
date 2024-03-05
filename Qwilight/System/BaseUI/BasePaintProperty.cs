using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.Utilities;
using Windows.UI;
using Brush = System.Windows.Media.Brush;
using DrawingContext = System.Windows.Media.DrawingContext;

namespace Qwilight
{
    public sealed class BasePaintProperty : IDisposable
    {
        public Brush EtcPaint { get; set; }

        public Color EtcColor { get; set; }

        public CanvasTextFormat Font { get; set; }

        public double DrawingMillis { get; set; }

        public int DrawingFrame { get; set; }

        public Bound PaintBound { get; set; }

        public int Frame { get; init; }

        public double Framerate { get; init; }

        public int Layer { get; init; }

        public Dictionary<int, HandledDrawingItem?[]> HandledDrawingItemMap { get; } = new();

        public HandledDrawingItem?[] HandledDrawingItems { get; set; }

        public HandledMediaItem[] HandledMediaItems { get; set; }

        public MediaHandlerItem MediaHandlerItem { get; set; }

        public int DrawingVariety { get; init; }

        public int Mode { get; init; }

        public string Etc { get; init; }

        public void Dispose()
        {
            Font?.Dispose();
        }

        public void Paint(DrawingContext targetSession, double millis, BaseNoteFile noteFile, AutoCompute autoComputer)
        {
            if (Mode == 0)
            {
                var r = PaintBound;
                var defaultMediaFaint = (int)(100 * Configure.Instance.BaseUIConfigureValue.DefaultMediaFaint);
                if (DrawingVariety < 100)
                {
                    switch (DrawingVariety)
                    {
                        case 0:
                            if (Frame > 0)
                            {
                                var framerate = 1000.0 / Framerate;
                                DrawingMillis += millis;
                                DrawingFrame = (int)(DrawingMillis / framerate) % Frame;
                                targetSession.PaintDrawing(ref r, HandledDrawingItems?.ElementAtOrDefault(DrawingFrame)?.DefaultDrawing);
                            }
                            break;
                        case 1:
                            if (noteFile != null && defaultMediaFaint > 0)
                            {
                                targetSession.PushOpacity(defaultMediaFaint / 100.0);
                                targetSession.DrawRectangle(Paints.Paint0, null, r);
                                var noteDrawing = noteFile.NoteDrawing;
                                Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, noteDrawing.Width, noteDrawing.Height, PaintBound.Position0, PaintBound.Position1, PaintBound.Length, PaintBound.Height);
                                targetSession.PaintDrawing(ref r, noteDrawing);
                                targetSession.Pop();
                            }
                            break;
                        case 2:
                            if (autoComputer != null)
                            {
                                autoComputer.PaintDefaultMedia(targetSession, ref r, defaultMediaFaint);
                            }
                            break;
                        case 3:
                            if (noteFile != null && Framerate > 0.0)
                            {
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(noteFile.Title, Framerate, EtcPaint, r.Length), PoolSystem.Instance.GetDefaultTextItem(noteFile.Title, Framerate, Paints.Paint0, r.Length), ref r);
                            }
                            break;
                        case 4:
                            if (noteFile != null && Framerate > 0.0)
                            {
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(noteFile.Artist, Framerate, EtcPaint, r.Length), PoolSystem.Instance.GetDefaultTextItem(noteFile.Artist, Framerate, Paints.Paint0, r.Length), ref r);
                            }
                            break;
                        case 5:
                            if (noteFile != null && Framerate > 0.0)
                            {
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(noteFile.GenreText, Framerate, EtcPaint, r.Length), PoolSystem.Instance.GetDefaultTextItem(noteFile.GenreText, Framerate, Paints.Paint0, r.Length), ref r);
                            }
                            break;
                        case 6:
                            if (noteFile != null && Framerate > 0.0)
                            {
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(noteFile.LevelText, Framerate, BaseUI.Instance.LevelPaints[(int)noteFile.LevelValue], r.Length), PoolSystem.Instance.GetDefaultTextItem(noteFile.LevelText, Framerate, Paints.Paint0, r.Length), ref r);
                            }
                            break;
                        case 7:
                            if (TwilightSystem.Instance.IsSignedIn && Framerate > 0.0)
                            {
                                var avatarName = TwilightSystem.Instance.GetAvatarName();
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(avatarName, Framerate, EtcPaint, r.Length), PoolSystem.Instance.GetDefaultTextItem(avatarName, Framerate, Paints.Paint0, r.Length), ref r);
                            }
                            break;
                        case 10:
                            Utility.PaintAudioVisualizer(targetSession, ref r, (int)(100 * Configure.Instance.BaseUIConfigureValue.DefaultAudioVisualizerFaint), PaintBound.Position0, PaintBound.Position1, PaintBound.Length, PaintBound.Height);
                            break;
                        case 11:
                            targetSession.DrawVideo(MediaHandlerItem?.HandledMediaItem.DefaultMedia, r);
                            break;
                        case 12:
                            if (TwilightSystem.Instance.IsSignedIn && Framerate > 0.0)
                            {
                                var avatarID = TwilightSystem.Instance.AvatarID;
                                var avatarTitle = AvatarTitleSystem.Instance.JustGetAvatarTitle(avatarID);
                                if (avatarTitle.HasValue)
                                {
                                    var avatarTitleValue = avatarTitle.Value;
                                    targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(avatarTitleValue.Title, Framerate, avatarTitleValue.TitlePaint, r.Length), PoolSystem.Instance.GetDefaultTextItem(avatarTitleValue.Title, Framerate, Paints.Paint0, r.Length), ref r);
                                }
                                else if (AvatarTitleSystem.Instance.CanCallAPI(avatarID))
                                {
                                    _ = AvatarTitleSystem.Instance.GetAvatarTitle(avatarID);
                                }
                            }
                            break;
                        case 13:
                            if (noteFile != null && Framerate > 0.0)
                            {
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetDefaultTextItem(noteFile.WantLevelID, Framerate, EtcPaint, r.Length), PoolSystem.Instance.GetDefaultTextItem(noteFile.WantLevelID, Framerate, Paints.Paint0, r.Length), ref r);
                            }
                            break;
                    }
                }
                else if (DrawingFrame >= 0)
                {
                    var framerate = 1000.0 / Framerate;
                    DrawingMillis += millis;
                    DrawingFrame = (int)(DrawingMillis / framerate);
                    if (DrawingFrame < Frame)
                    {
                        targetSession.PaintDrawing(ref r, HandledDrawingItems?.ElementAtOrDefault(DrawingFrame)?.DefaultDrawing);
                    }
                }
            }
        }

        public void Paint(CanvasDrawingSession targetSession, double millis, DefaultCompute defaultComputer, DefaultCompute handlingComputer)
        {
            if (Mode == 1)
            {
                var r = PaintBound;
                if (DrawingVariety < 100)
                {
                    var defaultMediaFaint = (float)Configure.Instance.BaseUIConfigureValue.DefaultMediaFaint;
                    switch (DrawingVariety)
                    {
                        case 0:
                            if (Frame > 0)
                            {
                                var framerate = 1000.0 / Framerate;
                                DrawingMillis += millis;
                                DrawingFrame = (int)(DrawingMillis / framerate) % Frame;
                                targetSession.PaintDrawing(ref r, HandledDrawingItems?.ElementAtOrDefault(DrawingFrame)?.Drawing);
                            }
                            break;
                        case 1:
                            if (defaultMediaFaint > 0)
                            {
                                targetSession.FillRectangle(r, DrawingSystem.Instance.FaintFilledPaints[(int)(100 * defaultMediaFaint)]);
                                lock (handlingComputer.LoadedCSX)
                                {
                                    if (handlingComputer.HasContents)
                                    {
                                        targetSession.PaintDrawing(ref r, (handlingComputer.NoteDrawing ?? DrawingSystem.Instance.DefaultDrawing).Drawing, defaultMediaFaint);
                                    }
                                }
                            }
                            break;
                        case 2:
                            handlingComputer.PaintMedia(targetSession, ref r, defaultMediaFaint);
                            break;
                        case 7:
                            if (Framerate > 0.0)
                            {
                                targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(defaultComputer.AvatarName, Font), ref r, EtcColor);
                            }
                            break;
                        case 8:
                            var avatarID = defaultComputer.AvatarID;
                            var avatarDrawing = AvatarDrawingSystem.Instance.JustGetAvatarDrawing(avatarID);
                            if (avatarDrawing.HasValue)
                            {
                                targetSession.PaintDrawing(ref r, avatarDrawing.Value.Drawing);
                            }
                            else if (AvatarDrawingSystem.Instance.CanCallAPI(avatarID))
                            {
                                _ = AvatarDrawingSystem.Instance.GetAvatarDrawing(avatarID);
                            }

                            var avatarEdge = AvatarEdgeSystem.Instance.JustGetAvatarEdge(avatarID);
                            if (avatarEdge.HasValue)
                            {
                                r.Set(r.Position0 + r.Length * Levels.EdgeXY, r.Position1 + r.Height * Levels.EdgeXY, r.Length * Levels.EdgeMargin, r.Height * Levels.EdgeMargin);
                                targetSession.PaintDrawing(ref r, avatarEdge.Value.Drawing);
                            }
                            else if (AvatarEdgeSystem.Instance.CanCallAPI(avatarID))
                            {
                                _ = AvatarEdgeSystem.Instance.GetAvatarEdge(avatarID);
                            }
                            break;
                        case 9:
                            if (Frame > 0)
                            {
                                var framerate = 1000.0 / Framerate;
                                DrawingMillis += millis;
                                DrawingFrame = (int)(DrawingMillis / framerate) % Frame;
                                switch (Etc)
                                {
                                    case "S+" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.SPlus:
                                    case "S FC" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.S && defaultComputer.IsP:
                                    case "S" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.S && !defaultComputer.IsP:
                                    case "A+ FC" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.APlus && defaultComputer.IsP:
                                    case "A+" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.APlus && !defaultComputer.IsP:
                                    case "A FC" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.A && defaultComputer.IsP:
                                    case "A" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.A && !defaultComputer.IsP:
                                    case "B FC" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.B && defaultComputer.IsP:
                                    case "B" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.B && !defaultComputer.IsP:
                                    case "C FC" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.C && defaultComputer.IsP:
                                    case "C" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.C && !defaultComputer.IsP:
                                    case "D FC" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.D && defaultComputer.IsP:
                                    case "D" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.D && !defaultComputer.IsP:
                                    case "F" when defaultComputer.QuitStatusValue == DefaultCompute.QuitStatus.F:
                                        targetSession.PaintDrawing(ref r, HandledDrawingItems?.ElementAtOrDefault(DrawingFrame)?.Drawing);
                                        break;
                                }
                            }
                            break;
                        case 10:
                            Utility.PaintAudioVisualizer(targetSession, ref r, (int)(100 * Configure.Instance.BaseUIConfigureValue.DefaultAudioVisualizerFaint), r.Position0, r.Position1, r.Length, r.Height);
                            break;
                        case 11:
                            var mediaFrame = MediaHandlerItem?.MediaFrame;
                            if (mediaFrame != null)
                            {
                                var mediaFrameBound = mediaFrame.Bounds;
                                var mediaFrameLength = mediaFrameBound.Width;
                                var mediaFrameHeight = mediaFrameBound.Height;
                                if (mediaFrameLength * mediaFrameHeight > 0.0)
                                {
                                    targetSession.PaintDrawing(ref r, mediaFrame);
                                }
                            }
                            break;
                        case 12:
                            if (Framerate > 0.0)
                            {
                                avatarID = defaultComputer.AvatarID;
                                var avatarTitle = AvatarTitleSystem.Instance.JustGetAvatarTitle(avatarID);
                                var hasAvatarTitle = avatarTitle.HasValue;
                                if (!hasAvatarTitle && AvatarTitleSystem.Instance.CanCallAPI(avatarID))
                                {
                                    _ = AvatarTitleSystem.Instance.GetAvatarTitle(avatarID);
                                }
                                if (hasAvatarTitle)
                                {
                                    var avatarTitleValue = avatarTitle.Value;
                                    targetSession.PaintVisibleText(PoolSystem.Instance.GetTextItem(avatarTitleValue.Title, Font), ref r, avatarTitleValue.TitlePaints[100], DrawingSystem.Instance.FaintFilledPaints[100]);
                                }
                            }
                            break;
                    }
                }
                else if (DrawingFrame >= 0)
                {
                    var framerate = 1000.0 / Framerate;
                    DrawingMillis += millis;
                    DrawingFrame = (int)(DrawingMillis / framerate);
                    if (DrawingFrame < Frame)
                    {
                        targetSession.PaintDrawing(ref r, HandledDrawingItems?.ElementAtOrDefault(DrawingFrame)?.Drawing);
                    }
                }
            }
        }
    }
}