using Google.Protobuf;
using MoonSharp.Interpreter;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using Qwilight.XOR;

namespace Qwilight.Compute
{
    public class NetCompute : DefaultCompute
    {
        bool _hasPendingFailed;

        public override bool CanPause => false;

        public override bool CanUndo => false;

        public override bool IsPassable => false;

        public override QuitStatus QuitStatusValue => Utility.GetQuitStatusValue(Point.TargetValue, Stand.TargetValue, IsF ? 0.0 : HitPoints.TargetValue, 1);

        public override string TotalNotesInQuit => TotalNotes.ToString();

        public override string HighestJudgmentInQuit => Comment.HighestJudgment.ToString();

        public override string HigherJudgmentInQuit => Comment.HigherJudgment.ToString();

        public override string HighJudgmentInQuit => Comment.HighJudgment.ToString();

        public override string LowJudgmentInQuit => Comment.LowJudgment.ToString();

        public override string LowerJudgmentInQuit => Comment.LowerJudgment.ToString();

        public override string LowestJudgmentInQuit => Comment.LowestJudgment.ToString();

        public NetCompute(BaseNoteFile[] noteFiles, ModeComponent modeComponentValue, string avatarID, string avatarName, JSON.TwilightLevyNet twilightLevyNet) : base(noteFiles, null, modeComponentValue, avatarID, avatarName, default, twilightLevyNet.handlerID)
        {
            WaitingTwilightLevel = WaitingTwilight.Net;
            NetDrawings = new Queue<Event.Types.NetDrawing>();
            IsFailMode = false;
            SiteID = twilightLevyNet.siteID;
            ValidNetMode = twilightLevyNet.validNetMode;
            AvatarsCount = twilightLevyNet.avatarsCount;
            AllowedPostableItems = twilightLevyNet.allowedPostableItems.Select(i => PostableItem.Values[i]).ToArray();
            if (IsPostableItemMode && AvatarsCount == 1)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.PostableItemModeWarning);
            }
        }

        public override void AtQuitMode()
        {
            if (!IsPostableItemMode)
            {
                base.AtQuitMode();
            }
            SetPendingQuitNetItems();
        }

        public void SetPendingQuitNetItems()
        {
            NoteFiles = PendingQuitNetItems.Select(quitNetItem => new NetNoteFile(quitNetItem)).ToArray();
            Comments = PendingQuitNetComments;
            Stands = PendingQuitNetItems.Select(quitNetItem => new MoveValue<XORInt32>(quitNetItem.stand)).ToArray();
            Points = PendingQuitNetItems.Select(quitNetItem => new MoveValue<XORFloat64>(quitNetItem.point)).ToArray();
            HitPointsValues = PendingQuitNetItems.Select(quitNetItem => new MoveValue<XORFloat64>(quitNetItem.hitPoints)).ToArray();
            IsFs = PendingQuitNetItems.Select(quitNetItem => new Primitive<bool>(quitNetItem.isF)).ToArray();
            ModeComponentValues = PendingQuitNetItems.Select((quitNetItem, i) => new ModeComponent(NoteFiles[i], quitNetItem)).ToArray();
            AvatarIDs = PendingQuitNetItems.Select(quitNetItem => quitNetItem.avatarID).ToArray();
            AvatarNames = PendingQuitNetItems.Select(quitNetItem => quitNetItem.avatarName).ToArray();
            HighestComputingPosition = NoteFiles.Length - 1;
            LevyingComputingPosition = Array.IndexOf(PendingQuitNetItems, PendingQuitNetItems.Single(netItem => netItem.avatarID == AvatarID));
            Configure.Instance.NotifyTutorial(Configure.TutorialID.NetQuitMode);
        }

        public override void GetNetItems()
        {
        }

        public override void GetNetComments()
        {
        }

        public override void OnFailed()
        {
            base.OnFailed();
            _hasPendingFailed = true;
        }

        public override void HandleNetItems()
        {
            if (!IsF)
            {
                SendCallNetEvent(Event.Types.AvatarNetStatus.Default);
            }
        }

        public override void OnHandled()
        {
            if (!IsF)
            {
                SetCommentFile();
                SendCallNetEvent(Event.Types.AvatarNetStatus.Clear);
            }
        }

        public override void OnGetF()
        {
            if (!IsF)
            {
                IsF.SetValue(true);
                SendCallNetEvent(Event.Types.AvatarNetStatus.Failed);
            }
        }

        public override void SendNotCompiled() => TwilightSystem.Instance.SendParallel(Event.Types.EventID.Compiled, new
        {
            siteID = SiteID,
            handlerID = HandlerID,
            isCompiled = false
        });

        public override void SendSituation()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsNoteFileMode)
            {
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetSituation, new
                {
                    situationValue = (int)(mainViewModel.IsQuitMode ? UbuntuItem.UbuntuSituation.QuitMode : UbuntuItem.UbuntuSituation.NetComputing),
                    situationText = PlatformText
                });
            }
        }

        public override void SetCommentPlaceText()
        {
            CommentPlace0Text = $"＃{NetItems.Single(netItem => netItem.AvatarID == AvatarID).TargetPosition + 1}";
            CommentPlace1Text = $"／{NetItems.Count}";
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
                TwilightSystem.Instance.SendParallel(Event.Types.EventID.Compiled, new
                {
                    siteID = SiteID,
                    handlerID = HandlerID,
                    isCompiled = true
                });
            }
        }

        public void SendCallNetEvent(Event.Types.AvatarNetStatus avatarNetStatus)
        {
            if (IsValidNetDrawings || avatarNetStatus != Event.Types.AvatarNetStatus.Default)
            {
                var eventItem = new Event();
                eventItem.EventID = Event.Types.EventID.CallNet;
                var qwilightCallNet = new Event.Types.QwilightCallNet();
                qwilightCallNet.SiteID = SiteID;
                qwilightCallNet.HandlerID = HandlerID;
                qwilightCallNet.HitPointsMode = (int)ModeComponentValue.HandlingHitPointsModeValue;
                qwilightCallNet.AvatarNetStatus = avatarNetStatus;
                qwilightCallNet.Stand = Stand.TargetValue;
                qwilightCallNet.HighestBand = HighestBand;
                qwilightCallNet.Point = Point.TargetValue;
                qwilightCallNet.HitPoints = HitPoints.TargetValue;
                qwilightCallNet.IsFailed = _hasPendingFailed;
                qwilightCallNet.LastJudged = (int)LastJudged;
                if (avatarNetStatus == Event.Types.AvatarNetStatus.Default)
                {
                    qwilightCallNet.Drawings.AddRange(NetDrawings);
                    NetDrawings.Clear();
                    IsValidNetDrawings = false;
                }
                else
                {
                    qwilightCallNet.Title = Title;
                    qwilightCallNet.Artist = Artist;
                    qwilightCallNet.Genre = Genre;
                    qwilightCallNet.Level = (int)LevelValue;
                    qwilightCallNet.LevelText = LevelText;
                    qwilightCallNet.WantLevelID = NoteFile.WantLevelID;
                    qwilightCallNet.AutoMode = (int)ModeComponentValue.AutoModeValue;
                    qwilightCallNet.NoteSaltMode = (int)ModeComponentValue.NoteSaltModeValue;
                    qwilightCallNet.AudioMultiplier = Comment.LevyingAudioMultiplier;
                    qwilightCallNet.FaintNoteMode = (int)ModeComponentValue.FaintNoteModeValue;
                    qwilightCallNet.JudgmentMode = (int)ModeComponentValue.JudgmentModeValue;
                    qwilightCallNet.NoteMobilityMode = (int)ModeComponentValue.NoteMobilityModeValue;
                    qwilightCallNet.LongNoteMode = (int)ModeComponentValue.LongNoteModeValue;
                    qwilightCallNet.InputFavorMode = (int)ModeComponentValue.InputFavorModeValue;
                    qwilightCallNet.NoteModifyMode = (int)ModeComponentValue.NoteModifyModeValue;
                    qwilightCallNet.BpmMode = (int)ModeComponentValue.BPMModeValue;
                    qwilightCallNet.WaveMode = (int)ModeComponentValue.WaveModeValue;
                    qwilightCallNet.SetNoteMode = (int)ModeComponentValue.SetNoteModeValue;
                    qwilightCallNet.LowestJudgmentConditionMode = (int)ModeComponentValue.LowestJudgmentConditionModeValue;
                    qwilightCallNet.TotalNotes = TotalNotes;
                    qwilightCallNet.JudgmentStage = JudgmentStage;
                    qwilightCallNet.HitPointsValue = HitPointsValue;
                    qwilightCallNet.HighestInputCount = HighestInputCount;
                    qwilightCallNet.Length = Length;
                    qwilightCallNet.Bpm = BPM;
                    qwilightCallNet.Multiplier = Comment.LevyingMultiplier;
                    qwilightCallNet.InputMode = (int)InputMode;
                    qwilightCallNet.HighestJudgment0 = ModeComponentValue.HighestJudgment0;
                    qwilightCallNet.HigherJudgment0 = ModeComponentValue.HigherJudgment0;
                    qwilightCallNet.HighJudgment0 = ModeComponentValue.HighJudgment0;
                    qwilightCallNet.LowJudgment0 = ModeComponentValue.LowJudgment0;
                    qwilightCallNet.LowerJudgment0 = ModeComponentValue.LowerJudgment0;
                    qwilightCallNet.LowestJudgment0 = ModeComponentValue.LowestJudgment0;
                    qwilightCallNet.HighestJudgment1 = ModeComponentValue.HighestJudgment1;
                    qwilightCallNet.HigherJudgment1 = ModeComponentValue.HigherJudgment1;
                    qwilightCallNet.HighJudgment1 = ModeComponentValue.HighJudgment1;
                    qwilightCallNet.LowJudgment1 = ModeComponentValue.LowJudgment1;
                    qwilightCallNet.LowerJudgment1 = ModeComponentValue.LowerJudgment1;
                    qwilightCallNet.LowestJudgment1 = ModeComponentValue.LowestJudgment1;
                    eventItem.Data.Add(Comment.ToByteString());
                }
                qwilightCallNet.HighestJudgment = Comment.HighestJudgment;
                qwilightCallNet.HigherJudgment = Comment.HigherJudgment;
                qwilightCallNet.HighJudgment = Comment.HighJudgment;
                qwilightCallNet.LowJudgment = Comment.LowJudgment;
                qwilightCallNet.LowerJudgment = Comment.LowerJudgment;
                qwilightCallNet.LowestJudgment = Comment.LowestJudgment;
                qwilightCallNet.DrawingComponent = NetDrawingComponentValue;
                eventItem.QwilightCallNet = qwilightCallNet;
                TwilightSystem.Instance.SendParallel(eventItem);
                _hasPendingFailed = false;
            }
        }
    }
}