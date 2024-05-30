using MoonSharp.Interpreter;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.ViewModel;

namespace Qwilight.Compute
{
    public class AutoCompute : DefaultCompute
    {
        public override bool IsPointVisible => true;

        public override bool CanPause => false;

        public override bool IsPassable => false;

        public override bool IsEscapable => false;

        public override bool CanIO => false;

        public override bool CanSetPosition => true;

        public override bool CanModifyAutoMode => true;

        public override LastStatus LastStatusValue => TelnetSystem.Instance.IsAvailable ? base.LastStatusValue : LastStatus.Not;

        public override double HandledLength => AudioLength;

        public override bool HandleFailedAudio => Configure.Instance.HandleFailedAudio.Data == ViewItem.Always;

        public override bool ViewFailedDrawing => Configure.Instance.ViewFailedDrawing.Data == ViewItem.Always;

        public override bool ViewLowestJudgment => Configure.Instance.ViewLowestJudgment.Data == ViewItem.Always;

        public override void HandleWarning()
        {
        }

        public AutoCompute(BaseNoteFile[] noteFiles, ModeComponent defaultModeComponentValue, string avatarID, string avatarName, int levyingMeter, double levyingWait) : base(noteFiles, null, defaultModeComponentValue, avatarID, $"{avatarName} (Auto)", default, null, null, null)
        {
            IsFailMode = false;
            LevyingMeter = levyingMeter;
            LevyingWait = levyingWait;
            IsAutoMode = true;
        }

        public override void OnGetF()
        {
        }

        public override void OnHandled()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            UIHandler.Instance.HandleParallel(() =>
            {
                if (!mainViewModel.IsQuitMode && Configure.Instance.SaltAuto && mainViewModel.EntryItems.Count(entryItem => !entryItem.IsLogical) > 1)
                {
                    mainViewModel.SaltEntryView();
                    mainViewModel.HandleAutoComputerImmediately(false);
                }
                else
                {
                    SetUndoValue = DefaultCompute.SetUndo.Just;
                }
            });
        }

        public override bool IsSuitableAsHwInput(int input) => !IsAutoMode && base.IsSuitableAsHwInput(input);

        public override bool IsSuitableAsAutoJudge(int input) => IsAutoMode || !IsSuitableAsInput(input);

        public override void SendSituation()
        {
            if (!ViewModels.Instance.MainValue.IsNoteFileMode)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
                {
                    situationValue = (int)UbuntuItem.UbuntuSituation.AutoComputing,
                    situationText = PlatformText
                });
            }
        }

        public override void AtNoteFileMode()
        {
            SetAutoNoteWait();
        }

        public override void SetNoteFileMode(string faultText = null)
        {
            IsAutoMode = true;
            ViewModels.Instance.MainValue.SetNoteFileMode(faultText);
        }

        public override void AtQuitMode()
        {
        }

        public override void OnFault(ScriptRuntimeException e)
        {
        }

        public override void OnCompiled()
        {
            if (!IsSilent)
            {
                ViewModels.Instance.MainValue.ClosePausableAudioHandler();
                try
                {
                    SetUIMap();
                }
                catch
                {
                }
                if (double.IsNaN(LevyingWait))
                {
                    LevyingWait = Configure.Instance.AutoHighlight ? AudioLevyingPosition : 0.0;
                }
                HandleComputer();
            }
        }
    }
}