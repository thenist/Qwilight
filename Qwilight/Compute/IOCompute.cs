using MoonSharp.Interpreter;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.ViewModel;

namespace Qwilight.Compute
{
    public sealed class IOCompute : DefaultCompute
    {
        public override bool IsPointVisible => true;

        public override bool IsPowered => false;

        public override bool CanPause => false;

        public override bool CanUndo => false;

        public override bool IsPassable => false;

        public override bool IsEscapable => false;

        public override bool CanIO => false;

        public override Configure.InputAudioVariety InputAudioVarietyValue => Configure.InputAudioVariety.IIDX;

        public IOCompute(BaseNoteFile[] noteFiles, ModeComponent defaultModeComponentValue, JSON.TwilightCallIOComponent twilightCallIOComponent) : base(noteFiles, null, defaultModeComponentValue, twilightCallIOComponent.avatarID, twilightCallIOComponent.avatarName, null, null, twilightCallIOComponent.handlerID)
        {
            IsFailMode = twilightCallIOComponent.isFailMode;
            WaitingTwilightLevel = WaitingTwilight.WaitIO;
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            IOMillis = time - ((time - twilightCallIOComponent.ioMillis) / 2 + twilightCallIOComponent.targetIOMillis);
        }

        public override void GetNetItems()
        {
            lock (IsTwilightNetItemsCSX)
            {
                if (!IsTwilightNetItems)
                {
                    base.GetNetItems();
                }
            }
        }

        public override void GetNetComments()
        {
            lock (IsTwilightNetItemsCSX)
            {
                if (!IsTwilightNetItems)
                {
                    base.GetNetComments();
                }
            }
        }

        public override void OnHandled()
        {
        }

        public override void OnGetF() => IsF.SetValue(true);

        public override void SendNotCompiled() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.CompiledIo, new
        {
            avatarID = AvatarID,
            handlerID = HandlerID,
            isCompiled = false
        });

        public override bool IsSuitableAsHwInput(int input) => false;

        public override bool IsSuitableAsAutoJudge(int input) => false;

        public override void AtNoteFileMode()
        {
            Close();
        }

        public override void SendSituation()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsNoteFileMode)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
                {
                    situationValue = (int)UbuntuItem.UbuntuSituation.IOComputing,
                    situationText = PlatformText
                });
            }
        }

        public override void HandleNetItems(double millisLoopUnit)
        {
            if (!IsTwilightNetItems)
            {
                base.HandleNetItems(millisLoopUnit);
            }
        }

        public override void OnCompiled()
        {
            if (!IsSilent)
            {
                ViewModels.Instance.MainValue.CloseAutoComputer();
                try
                {
                    SetUIMap();
                }
                catch (Exception e)
                {
                    throw new ScriptRuntimeException(e);
                }
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.CompiledIo, new
                {
                    avatarID = AvatarID,
                    handlerID = HandlerID,
                    isCompiled = true
                });
            }
        }
    }
}