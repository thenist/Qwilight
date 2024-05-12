using CommunityToolkit.Mvvm.Input;
using Qwilight.UIComponent;
using System.Windows;

namespace Qwilight.ViewModel
{
    public sealed partial class ModifyModeComponentViewModel : BaseViewModel
    {
        public const int AutoModeVariety = 0;
        public const int NoteSaltModeVariety = 1;
        public const int FaintNoteModeVariety = 2;
        public const int JudgmentModeVariety = 3;
        public const int HitPointsModeVariety = 4;
        public const int NoteMobilityModeVariety = 5;
        public const int LongNoteModeVariety = 6;
        public const int InputFavorModeVariety = 7;
        public const int NoteModifyModeVariety = 8;
        public const int BPMModeVariety = 9;
        public const int WaveModeVariety = 10;
        public const int SetNoteModeVariety = 11;
        public const int LowestJudgmentConditionModeVariety = 12;

        int _modeComponentVariety;
        ModifyModeComponentItem _modeComponentItem;
        List<ModifyModeComponentItem> _modeComponentItems;

        public override double TargetLength => 0.5;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Top;

        public List<ModifyModeComponentItem>[] ModifyModeComponentItems { get; } = new List<ModifyModeComponentItem>[13];

        public List<ModifyModeComponentItem> ModeComponentItems
        {
            get => _modeComponentItems;

            set => SetProperty(ref _modeComponentItems, value, nameof(ModeComponentItems));
        }

        public bool IsHitPointsMode => ModeComponentVariety == HitPointsModeVariety;

        public int ModeComponentVariety
        {
            get => _modeComponentVariety;

            set
            {
                if (SetProperty(ref _modeComponentVariety, value))
                {
                    OnPropertyChanged(nameof(IsHitPointsMode));
                    ModeComponentItems = ModifyModeComponentItems[value];
                    var targetModeValue = value switch
                    {
                        AutoModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.AutoModeValue,
                        NoteSaltModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.NoteSaltModeValue,
                        FaintNoteModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.FaintNoteModeValue,
                        JudgmentModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.JudgmentModeValue,
                        HitPointsModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.HitPointsModeValue,
                        NoteMobilityModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.NoteMobilityModeValue,
                        LongNoteModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.LongNoteModeValue,
                        InputFavorModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.InputFavorModeValue,
                        NoteModifyModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.NoteModifyModeValue,
                        BPMModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.BPMModeValue,
                        WaveModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.WaveModeValue,
                        SetNoteModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.SetNoteModeValue,
                        LowestJudgmentConditionModeVariety => (int)ViewModels.Instance.MainValue.ModeComponentValue.LowestJudgmentConditionModeValue,
                        _ => default
                    };
                    ModeComponentItem = ModeComponentItems.Single(modeComponentItem => modeComponentItem.Value == targetModeValue);
                }
            }
        }

        public ModifyModeComponentItem ModeComponentItem
        {
            get => _modeComponentItem;

            set
            {
                if (SetProperty(ref _modeComponentItem, value, nameof(ModeComponentItem)) && value != null)
                {
                    var modeComponent = ViewModels.Instance.MainValue.ModeComponentValue;
                    switch (ModeComponentVariety)
                    {
                        case AutoModeVariety:
                            modeComponent.AutoModeValue = (ModeComponent.AutoMode)value.Value;
                            break;
                        case NoteSaltModeVariety:
                            modeComponent.NoteSaltModeValue = (ModeComponent.NoteSaltMode)value.Value;
                            break;
                        case FaintNoteModeVariety:
                            modeComponent.FaintNoteModeValue = (ModeComponent.FaintNoteMode)value.Value;
                            break;
                        case JudgmentModeVariety:
                            modeComponent.JudgmentModeValue = (ModeComponent.JudgmentMode)value.Value;
                            break;
                        case HitPointsModeVariety:
                            modeComponent.HitPointsModeValue = (ModeComponent.HitPointsMode)value.Value;
                            SetHitPointsMode();
                            break;
                        case NoteMobilityModeVariety:
                            modeComponent.NoteMobilityModeValue = (ModeComponent.NoteMobilityMode)value.Value;
                            break;
                        case LongNoteModeVariety:
                            modeComponent.LongNoteModeValue = (ModeComponent.LongNoteMode)value.Value;
                            break;
                        case InputFavorModeVariety:
                            modeComponent.InputFavorModeValue = (ModeComponent.InputFavorMode)value.Value;
                            break;
                        case NoteModifyModeVariety:
                            modeComponent.NoteModifyModeValue = (ModeComponent.NoteModifyMode)value.Value;
                            break;
                        case BPMModeVariety:
                            modeComponent.BPMModeValue = (ModeComponent.BPMMode)value.Value;
                            break;
                        case WaveModeVariety:
                            modeComponent.WaveModeValue = (ModeComponent.WaveMode)value.Value;
                            break;
                        case SetNoteModeVariety:
                            modeComponent.SetNoteModeValue = (ModeComponent.SetNoteMode)value.Value;
                            break;
                        case LowestJudgmentConditionModeVariety:
                            modeComponent.LowestJudgmentConditionModeValue = (ModeComponent.LowestJudgmentConditionMode)value.Value;
                            break;
                    }
                }
            }
        }

        void SetHitPointsMode()
        {
            UIHandler.Instance.Handler.InvokeAsync(() =>
            {
                switch ((ModeComponent.HitPointsMode)ModeComponentItem.Value)
                {
                    case ModeComponent.HitPointsMode.Default:
                        switch (Configure.Instance.GASLevel)
                        {
                            case 0:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Yell);
                                break;
                            case 1:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Failed);
                                break;
                            case 2:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Highest);
                                break;
                            case 3:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Higher);
                                break;
                        }
                        break;
                    case ModeComponent.HitPointsMode.Higher:
                        switch (Configure.Instance.GASLevel)
                        {
                            case 0:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Yell);
                                break;
                            case 1:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Failed);
                                break;
                            case 2:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Highest);
                                break;
                        }
                        break;
                    case ModeComponent.HitPointsMode.Highest:
                        switch (Configure.Instance.GASLevel)
                        {
                            case 0:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Yell);
                                break;
                            case 1:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Failed);
                                break;
                        }
                        break;
                    case ModeComponent.HitPointsMode.Failed:
                        switch (Configure.Instance.GASLevel)
                        {
                            case 0:
                                ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Yell);
                                break;
                        }
                        break;
                }
            });
        }

        public void OnPointLower() => Close();

        [RelayCommand]
        void OnModifyModeComponent(int? e)
        {
            if (e.HasValue)
            {
                ModeComponentVariety = e.Value;
            }
        }

        [RelayCommand]
        static void OnIsFailMode() => Configure.Instance.IsFailMode = !Configure.Instance.IsFailMode;

        public void SetModeComponentItems()
        {
            for (var i = ModifyModeComponentItems.Length - 1; i >= 0; --i)
            {
                ModifyModeComponentItems[i] = new();
            }

            ModifyModeComponentItems[AutoModeVariety].Add(new()
            {
                Value = (int)ModeComponent.AutoMode.Default,
                Data = LanguageSystem.Instance.AutoModeTexts[(int)ModeComponent.AutoMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[AutoModeVariety][(int)ModeComponent.AutoMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[AutoModeVariety].Add(new()
            {
                Value = (int)ModeComponent.AutoMode.Autoable,
                Data = LanguageSystem.Instance.AutoModeTexts[(int)ModeComponent.AutoMode.Autoable],
                Drawing = BaseUI.Instance.ModeComponentDrawings[AutoModeVariety][(int)ModeComponent.AutoMode.Autoable]?.DefaultDrawing,
                PointedPaintID = 1
            });

            ModifyModeComponentItems[NoteSaltModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteSaltMode.Default,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteSaltModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteSaltMode.Symmetric,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Symmetric],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.Symmetric]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteSaltModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteSaltMode.InputSalt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.InputSalt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.InputSalt]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteSaltModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteSaltMode.MeterSalt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.MeterSalt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.MeterSalt]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteSaltModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteSaltMode.HalfInputSalt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.HalfInputSalt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.HalfInputSalt]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteSaltModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteSaltMode.Salt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Salt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.Salt]?.DefaultDrawing
            });

            ModifyModeComponentItems[FaintNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.FaintNoteMode.Default,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[FaintNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.FaintNoteMode.Faint,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Faint],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.Faint]?.DefaultDrawing
            });
            ModifyModeComponentItems[FaintNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.FaintNoteMode.Fading,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Fading],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.Fading]?.DefaultDrawing
            });
            ModifyModeComponentItems[FaintNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.FaintNoteMode.TotalFading,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.TotalFading],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.TotalFading]?.DefaultDrawing
            });

            ModifyModeComponentItems[JudgmentModeVariety].Add(new()
            {
                Value = (int)ModeComponent.JudgmentMode.Lowest,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Lowest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Lowest]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[JudgmentModeVariety].Add(new()
            {
                Value = (int)ModeComponent.JudgmentMode.Lower,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Lower],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Lower]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[JudgmentModeVariety].Add(new()
            {
                Value = (int)ModeComponent.JudgmentMode.Default,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[JudgmentModeVariety].Add(new()
            {
                Value = (int)ModeComponent.JudgmentMode.Higher,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Higher],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Higher]?.DefaultDrawing
            });
            ModifyModeComponentItems[JudgmentModeVariety].Add(new()
            {
                Value = (int)ModeComponent.JudgmentMode.Highest,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Highest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Highest]?.DefaultDrawing
            });
            ModifyModeComponentItems[JudgmentModeVariety].Add(new()
            {
                Value = (int)ModeComponent.JudgmentMode.Favor,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Favor],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Favor]?.DefaultDrawing,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.FavorJudgmentValue.Open();
                    ModeComponentVariety = JudgmentModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.JudgmentMode.Favor);
                }),
                PointedPaintID = 2
            });

            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Lowest,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Lowest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Lowest]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Lower,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Lower],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Lower]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Default,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Default]?.DefaultDrawing,
                OnVConfigure = new(() =>
                {
                    Configure.Instance.GASLevel = 4;
                    SetHitPointsMode();
                }),
                GetIsVConfigure = () => Configure.Instance.GASLevel >= 4
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Higher,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Higher],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Higher]?.DefaultDrawing,
                OnVConfigure = new(() =>
                {
                    Configure.Instance.GASLevel = 3;
                    SetHitPointsMode();
                }),
                GetIsVConfigure = () => Configure.Instance.GASLevel >= 3
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Highest,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Highest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Highest]?.DefaultDrawing,
                OnVConfigure = new(() =>
                {
                    Configure.Instance.GASLevel = 2;
                    SetHitPointsMode();
                }),
                GetIsVConfigure = () => Configure.Instance.GASLevel >= 2
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Failed,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Failed],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Failed]?.DefaultDrawing,
                OnVConfigure = new(() =>
                {
                    Configure.Instance.GASLevel = 1;
                    SetHitPointsMode();
                }),
                GetIsVConfigure = () => Configure.Instance.GASLevel >= 1
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Yell,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Yell],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Yell]?.DefaultDrawing,
                OnVConfigure = new(() =>
                {
                    Configure.Instance.GASLevel = 0;
                    SetHitPointsMode();
                }),
                GetIsVConfigure = () => Configure.Instance.GASLevel >= 0
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Favor,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Favor],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Favor]?.DefaultDrawing,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.FavorHitPointsValue.Open();
                    ModeComponentVariety = HitPointsModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Favor);
                }),
                PointedPaintID = 2
            });
            ModifyModeComponentItems[HitPointsModeVariety].Add(new()
            {
                Value = (int)ModeComponent.HitPointsMode.Test,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Test],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Test]?.DefaultDrawing,
                PointedPaintID = 2
            });

            ModifyModeComponentItems[NoteMobilityModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteMobilityMode.Default,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteMobilityModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteMobilityMode._4D,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode._4D],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode._4D]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteMobilityModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteMobilityMode._4DHD,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode._4DHD],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode._4DHD]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteMobilityModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteMobilityMode.Zip,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.Zip],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode.Zip]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteMobilityModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteMobilityMode.ZipHD,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.ZipHD],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode.ZipHD]?.DefaultDrawing
            });

            ModifyModeComponentItems[LongNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.LongNoteMode.Default,
                Data = LanguageSystem.Instance.LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[LongNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.LongNoteMode.Auto,
                Data = LanguageSystem.Instance.LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Auto],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Auto]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[LongNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.LongNoteMode.Input,
                Data = LanguageSystem.Instance.LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Input],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Input]?.DefaultDrawing,
                PointedPaintID = 2
            });

            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Default,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._4,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._4],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._4]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled4,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled4],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled4]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled4);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._5,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._5],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._5]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled5,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled5],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled5]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled5);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._6,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._6],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._6]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled6,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled6],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled6]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled6);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._7,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._7],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._7]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled7,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled7],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled7]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled7);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._8,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._8],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._8]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled8,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled8],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled8]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled8);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._9,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._9],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._9]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled9,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled9],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled9]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled9);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._10,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._10],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._10]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled10,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled10],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled10]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled10);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._5_1,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._5_1],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._5_1]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled5_1,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled5_1],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled5_1]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled5_1);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._7_1,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._7_1],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._7_1]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled7_1,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled7_1],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled7_1]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled7_1);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._10_2,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._10_2],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._10_2]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled10_2,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled10_2],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled10_2]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled10_2);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._14_2,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._14_2],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._14_2]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled14_2,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled14_2],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled14_2]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled14_2);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._24_2,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._24_2],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._24_2]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled24_2,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled24_2],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled24_2]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled24_2);
                })
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode._48_4,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode._48_4],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode._48_4]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[InputFavorModeVariety].Add(new()
            {
                Value = (int)ModeComponent.InputFavorMode.Labelled48_4,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled48_4],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Labelled48_4]?.DefaultDrawing,
                PointedPaintID = 2,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.InputFavorLabelledValue.Open();
                    ModeComponentVariety = InputFavorModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.InputFavorMode.Labelled48_4);
                })
            });

            ModifyModeComponentItems[NoteModifyModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteModifyMode.Default,
                Data = LanguageSystem.Instance.DefaultNoteModifyContents,
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteModifyModeVariety][(int)ModeComponent.NoteModifyMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[NoteModifyModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteModifyMode.InputNote,
                Data = LanguageSystem.Instance.NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.InputNote],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteModifyModeVariety][(int)ModeComponent.NoteModifyMode.InputNote]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModifyModeComponentItems[NoteModifyModeVariety].Add(new()
            {
                Value = (int)ModeComponent.NoteModifyMode.LongNote,
                Data = LanguageSystem.Instance.NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.LongNote],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteModifyModeVariety][(int)ModeComponent.NoteModifyMode.LongNote]?.DefaultDrawing,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.LongNoteModifyValue.Open();
                    ModeComponentVariety = NoteModifyModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.NoteModifyMode.LongNote);
                }),
                PointedPaintID = 2
            });

            ModifyModeComponentItems[BPMModeVariety].Add(new()
            {
                Value = (int)ModeComponent.BPMMode.Default,
                Data = LanguageSystem.Instance.BPMModeTexts[(int)ModeComponent.BPMMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[BPMModeVariety][(int)ModeComponent.BPMMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[BPMModeVariety].Add(new()
            {
                Value = (int)ModeComponent.BPMMode.Not,
                Data = LanguageSystem.Instance.BPMModeTexts[(int)ModeComponent.BPMMode.Not],
                Drawing = BaseUI.Instance.ModeComponentDrawings[BPMModeVariety][(int)ModeComponent.BPMMode.Not]?.DefaultDrawing,
                PointedPaintID = 2
            });

            ModifyModeComponentItems[WaveModeVariety].Add(new()
            {
                Value = (int)ModeComponent.WaveMode.Default,
                Data = LanguageSystem.Instance.WaveModeTexts[(int)ModeComponent.WaveMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[WaveModeVariety][(int)ModeComponent.WaveMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[WaveModeVariety].Add(new()
            {
                Value = (int)ModeComponent.WaveMode.Counter,
                Data = LanguageSystem.Instance.WaveModeTexts[(int)ModeComponent.WaveMode.Counter],
                Drawing = BaseUI.Instance.ModeComponentDrawings[WaveModeVariety][(int)ModeComponent.WaveMode.Counter]?.DefaultDrawing,
                PointedPaintID = 2
            });

            ModifyModeComponentItems[SetNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.SetNoteMode.Default,
                Data = LanguageSystem.Instance.SetNoteModeTexts[(int)ModeComponent.SetNoteMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[SetNoteModeVariety][(int)ModeComponent.SetNoteMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[SetNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.SetNoteMode.Put,
                Data = LanguageSystem.Instance.SetNoteModeTexts[(int)ModeComponent.SetNoteMode.Put],
                Drawing = BaseUI.Instance.ModeComponentDrawings[SetNoteModeVariety][(int)ModeComponent.SetNoteMode.Put]?.DefaultDrawing,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.SetNotePutValue.Open();
                    ModeComponentVariety = SetNoteModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.SetNoteMode.Put);
                }),
                PointedPaintID = 2
            });
            ModifyModeComponentItems[SetNoteModeVariety].Add(new()
            {
                Value = (int)ModeComponent.SetNoteMode.VoidPut,
                Data = LanguageSystem.Instance.SetNoteModeTexts[(int)ModeComponent.SetNoteMode.VoidPut],
                Drawing = BaseUI.Instance.ModeComponentDrawings[SetNoteModeVariety][(int)ModeComponent.SetNoteMode.VoidPut]?.DefaultDrawing,
                OnConfigure = new(() =>
                {
                    ViewModels.Instance.SetNotePutValue.Open();
                    ModeComponentVariety = SetNoteModeVariety;
                    ModeComponentItem = ModeComponentItems.Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.SetNoteMode.VoidPut);
                }),
                PointedPaintID = 2
            });

            ModifyModeComponentItems[LowestJudgmentConditionModeVariety].Add(new()
            {
                Value = (int)ModeComponent.LowestJudgmentConditionMode.Default,
                Data = LanguageSystem.Instance.LowestJudgmentConditionModeTexts[(int)ModeComponent.LowestJudgmentConditionMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LowestJudgmentConditionModeVariety][(int)ModeComponent.LowestJudgmentConditionMode.Default]?.DefaultDrawing
            });
            ModifyModeComponentItems[LowestJudgmentConditionModeVariety].Add(new()
            {
                Value = (int)ModeComponent.LowestJudgmentConditionMode.Wrong,
                Data = LanguageSystem.Instance.LowestJudgmentConditionModeTexts[(int)ModeComponent.LowestJudgmentConditionMode.Wrong],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LowestJudgmentConditionModeVariety][(int)ModeComponent.LowestJudgmentConditionMode.Wrong]?.DefaultDrawing
            });
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            var mainViewModel = ViewModels.Instance.MainValue;
            if (ModeComponentVariety == HitPointsModeVariety && mainViewModel.ModeComponentValue.IsGASWarning)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.GASWarning);
            }
            mainViewModel.OnJudgmentMeterMillisModified();
            mainViewModel.HandleAutoComputer();
            ViewModels.Instance.SiteContainerValue.CallSetModeComponent();
        }
    }
}