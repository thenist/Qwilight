using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.View
{
    public sealed partial class Layer0View
    {
        readonly DrawingGroup _target = new();
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        double _lastMillis;

        public Layer0View()
        {
            InitializeComponent();
            IsVisibleChanged += OnVisibilityModified;
        }

        void OnVisibilityModified(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CompositionTarget.Rendering += OnPaint;
            }
            else
            {
                CompositionTarget.Rendering -= OnPaint;
            }
        }

        void OnPaint(object sender, object e)
        {
            var millis = _loopingHandler.GetMillis();
            var distanceMillis = millis - _lastMillis;
            _lastMillis = millis;

            using (var targetSession = _target.Open())
            {
                var r = new Bound();
                var mainViewModel = (DataContext as MainViewModel);
                if (mainViewModel.IsNoteFileMode)
                {
                    var p1 = new Point();
                    var p2 = new Point();
                    var noteFile = mainViewModel.EntryItemValue?.NoteFile;
                    var autoComputer = mainViewModel.AutoComputer;

                    foreach (var paintPropertyValue in BaseUI.Instance.PaintPropertyValues)
                    {
                        if (paintPropertyValue?.Layer == 0)
                        {
                            paintPropertyValue.Paint(targetSession, noteFile, autoComputer, distanceMillis);
                        }
                    }

                    var statusPoint = BaseUI.Instance.StatusPoint;
                    var status = autoComputer?.Status;
                    if (statusPoint?.Length > 4 && status > 0.0)
                    {
                        var statusValue = status.Value;
                        var statusPosition0 = statusPoint[0];
                        var statusPosition1 = statusPoint[1];
                        var statusLength = statusPoint[2];
                        var statusHeight = statusPoint[3];
                        switch (statusPoint[4])
                        {
                            case 0:
                                r.Set(statusPosition0, statusPosition1 + statusHeight * (1 - statusValue), statusLength, statusHeight * statusValue);
                                break;
                            case 1:
                                r.Set(statusPosition0, statusPosition1, statusLength, statusHeight * statusValue);
                                break;
                            case 2:
                                r.Set(statusPosition0 + statusLength * (1 - statusValue), statusPosition1, statusLength * statusValue, statusHeight);
                                break;
                            case 3:
                                r.Set(statusPosition0, statusPosition1, statusLength * statusValue, statusHeight);
                                break;
                        }
                        targetSession.DrawRectangle(autoComputer.IsHandling ? autoComputer.IsPausing ? BaseUI.Instance.StatusPausedPaint : BaseUI.Instance.StatusHandlingPaint : BaseUI.Instance.StatusLoadingNoteFilePaint, null, r);
                    }

                    var statusDefaultEntryPoint = BaseUI.Instance.StatusDefaultEntryPoint;
                    var defaultEntryStatus = mainViewModel.Status;
                    if (statusDefaultEntryPoint?.Length > 4 && defaultEntryStatus > 0.0)
                    {
                        var defaultEntryPosition0 = statusDefaultEntryPoint[0];
                        var defaultEntryPosition1 = statusDefaultEntryPoint[1];
                        var defaultEntryLength = statusDefaultEntryPoint[2];
                        var defaultEntryHeight = statusDefaultEntryPoint[3];
                        switch (statusDefaultEntryPoint[4])
                        {
                            case 0:
                                r.Set(defaultEntryPosition0, defaultEntryPosition1 + defaultEntryHeight * (1 - defaultEntryStatus), defaultEntryLength, defaultEntryHeight * defaultEntryStatus);
                                break;
                            case 1:
                                r.Set(defaultEntryPosition0, defaultEntryPosition1, defaultEntryLength, defaultEntryHeight * defaultEntryStatus);
                                break;
                            case 2:
                                r.Set(defaultEntryPosition0 + defaultEntryLength * (1 - defaultEntryStatus), defaultEntryPosition1, defaultEntryLength * defaultEntryStatus, defaultEntryHeight);
                                break;
                            case 3:
                                r.Set(defaultEntryPosition0, defaultEntryPosition1, defaultEntryLength * defaultEntryStatus, defaultEntryHeight);
                                break;
                        }
                        targetSession.DrawRectangle(BaseUI.Instance.StatusLoadingDefaultEntryPaint, null, r);
                    }

                    var inputNoteCountViewPoint = BaseUI.Instance.InputNoteCountViewPoint;
                    if (inputNoteCountViewPoint?.Length > 3)
                    {
                        var inputNoteCountViewPosition0 = inputNoteCountViewPoint[0];
                        var inputNoteCountViewPosition1 = inputNoteCountViewPoint[1];
                        var inputNoteCountViewLength = inputNoteCountViewPoint[2];
                        var inputNoteCountViewHeight = inputNoteCountViewPoint[3];

                        if (autoComputer?.IsHandling == true)
                        {
                            var statusValue = status.Value;
                            var targetInputCount = Component.Input1PCounts[(int)autoComputer.InputMode] * 6;

                            if (targetInputCount > 0)
                            {
                                var inputNoteCountViewPaint = Configure.Instance.InputNoteCountViewPaint;
                                var inputNoteCounts = autoComputer.InputNoteCounts;
                                var inputNoteCountsCount = inputNoteCounts.Count;
                                var inputNoteCountsUnitLength = inputNoteCountViewLength / inputNoteCountsCount;
                                for (var i = inputNoteCountsCount - 1; i >= 0; --i)
                                {
                                    var inputNoteCount = Math.Min(inputNoteCounts[i] * autoComputer.AudioMultiplier, targetInputCount);
                                    if (inputNoteCount > 0.0)
                                    {
                                        r.Set(inputNoteCountViewPosition0 + inputNoteCountsUnitLength * i, inputNoteCountViewPosition1 + inputNoteCountViewHeight - inputNoteCountViewHeight * inputNoteCount / targetInputCount, inputNoteCountsUnitLength, inputNoteCountViewHeight * inputNoteCount / targetInputCount);
                                        targetSession.DrawRectangle(inputNoteCountViewPaint, null, r);
                                    }
                                }

                                var autoableInputNoteCountViewPaint = Configure.Instance.AutoableInputNoteCountViewPaint;
                                var autoableInputNoteCounts = autoComputer.AutoableInputNoteCounts;
                                var autoableInputNoteCountsCount = autoableInputNoteCounts.Count;
                                var autoableInputNoteCountsUnitLength = inputNoteCountViewLength / autoableInputNoteCountsCount;
                                for (var i = autoableInputNoteCountsCount - 1; i >= 0; --i)
                                {
                                    var autoableInputNoteCount = Math.Min(autoableInputNoteCounts[i] * autoComputer.AudioMultiplier, targetInputCount);
                                    if (autoableInputNoteCount > 0.0)
                                    {
                                        r.Set(inputNoteCountViewPosition0 + autoableInputNoteCountsUnitLength * i, inputNoteCountViewPosition1 + inputNoteCountViewHeight - inputNoteCountViewHeight * autoableInputNoteCount / targetInputCount, autoableInputNoteCountsUnitLength, inputNoteCountViewHeight * autoableInputNoteCount / targetInputCount);
                                        targetSession.DrawRectangle(autoableInputNoteCountViewPaint, null, r);
                                    }
                                }
                            }

                            var inputCount = autoComputer.InputCountQueue.Count;
                            if (statusValue > 0.0 && statusValue < 1.0)
                            {
                                p1.Set(inputNoteCountViewPosition0 + inputNoteCountViewLength * statusValue, inputNoteCountViewPosition1);
                                p2.Set(p1.X, p1.Y + inputNoteCountViewHeight);
                                targetSession.DrawLine(Paints.Pen4, p1, p2);
                            }

                            var inputCountText = PoolSystem.Instance.GetValueText(inputCount, "NPS: #,##0 / s");
                            var defaultTextItem = PoolSystem.Instance.GetDefaultTextItem(inputCountText, Levels.FontLevel0, Paints.Paint4);
                            var defaultTextVisibleItem = PoolSystem.Instance.GetDefaultTextItem(inputCountText, Levels.FontLevel0, Paints.Paint0);
                            r.SetPosition(Levels.StandardMargin + inputNoteCountViewPosition0, Levels.StandardMargin + inputNoteCountViewPosition1);
                            targetSession.PaintVisibleText(defaultTextItem, defaultTextVisibleItem, ref r);

                            var length = (int)(autoComputer.Length / 1000.0);
                            var wait = Math.Clamp((int)(autoComputer.LoopingCounter / 1000.0), 0, length);
                            inputCountText = PoolSystem.Instance.GetFormattedText("{0}:{1} / {2}:{3}",
                                PoolSystem.Instance.GetValueText(wait / 60, string.Empty),
                                PoolSystem.Instance.GetValueText(wait % 60, "00"),
                                PoolSystem.Instance.GetValueText(length / 60, string.Empty),
                                PoolSystem.Instance.GetValueText(length % 60, "00")
                            );
                            defaultTextItem = PoolSystem.Instance.GetDefaultTextItem(inputCountText, Levels.FontLevel0, Paints.Paint4);
                            defaultTextVisibleItem = PoolSystem.Instance.GetDefaultTextItem(inputCountText, Levels.FontLevel0, Paints.Paint0);
                            r.SetPosition(inputNoteCountViewPosition0 + inputNoteCountViewLength - defaultTextItem.Width - Levels.StandardMargin, Levels.StandardMargin + inputNoteCountViewPosition1);
                            targetSession.PaintVisibleText(defaultTextItem, defaultTextVisibleItem, ref r);
                        }
                    }
                }
                else
                {
                    r.SetArea(mainViewModel.DefaultLength, mainViewModel.DefaultHeight);
                    targetSession.DrawImage(DrawingSystem.Instance.D3D9Drawing, r);
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawDrawing(_target);
        }
    }
}