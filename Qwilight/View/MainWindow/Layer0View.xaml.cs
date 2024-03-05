using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.View
{
    public sealed partial class Layer0View
    {
        readonly VisualCollection _targets;
        readonly DrawingVisual _target = new();
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        double _lastMillis;

        public Layer0View()
        {
            _targets = new(this);
            _targets.Add(_target);

            InitializeComponent();
        }

        protected override int VisualChildrenCount => _targets.Count;

        protected override Visual GetVisualChild(int index) => _targets[index];

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

        void OnPaint(object sender, EventArgs e)
        {
            var millis = _loopingHandler.GetMillis();
            var distanceMillis = millis - _lastMillis;
            _lastMillis = millis;

            using var targetSession = _target.RenderOpen();
            var r = new Bound();
            var s = new Bound();
            var mainViewModel = (DataContext as MainViewModel);
            if (mainViewModel.IsNoteFileMode)
            {
                var noteFile = mainViewModel.EntryItemValue?.NoteFile;
                var autoComputer = mainViewModel.AutoComputer;

                foreach (var paintProperty in BaseUI.Instance.PaintProperties)
                {
                    if (paintProperty?.Layer == 0)
                    {
                        paintProperty.Paint(targetSession, distanceMillis, noteFile, autoComputer);
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
                        var targetInputCount = Component.InputCounts1P[(int)autoComputer.InputMode] * 6;

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
                            r.SetPosition(inputNoteCountViewPosition0 + inputNoteCountViewLength * statusValue, inputNoteCountViewPosition1);
                            s.SetPosition(r.Position0, r.Position1 + inputNoteCountViewHeight);
                            targetSession.DrawLine(Paints.Pen4, r, s);
                        }

                        var inputCountText = PoolSystem.Instance.GetValueText(inputCount, "NPS: #,##0 / s");
                        var defaultTextItem = PoolSystem.Instance.GetDefaultTextItem(inputCountText, Levels.FontLevel0, Paints.Paint4);
                        var defaultTextVisibleItem = PoolSystem.Instance.GetDefaultTextItem(inputCountText, Levels.FontLevel0, Paints.Paint0);
                        r.SetPosition(Levels.StandardMargin + inputNoteCountViewPosition0, Levels.StandardMargin + inputNoteCountViewPosition1);
                        targetSession.PaintVisibleText(defaultTextItem, defaultTextVisibleItem, ref r);

                        var sLength = (int)(autoComputer.Length / 1000.0);
                        var sLoopingCounter = Math.Clamp((int)(autoComputer.LoopingCounter / 1000.0), 0, sLength);
                        inputCountText = PoolSystem.Instance.GetFormattedText("{0}:{1} / {2}:{3}",
                            PoolSystem.Instance.GetValueText(sLoopingCounter / 60, string.Empty),
                            PoolSystem.Instance.GetValueText(sLoopingCounter % 60, "00"),
                            PoolSystem.Instance.GetValueText(sLength / 60, string.Empty),
                            PoolSystem.Instance.GetValueText(sLength % 60, "00")
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
}