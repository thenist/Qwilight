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

        int _modeComponentValueVariety;
        ModifyModeComponentItem _modeComponentItem;
        List<ModifyModeComponentItem> _modeComponentItems;

        public override double TargetLength => 0.5;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment TargetHeightSystem => VerticalAlignment.Top;

        public List<ModifyModeComponentItem>[] ModeComponentValues { get; } = new List<ModifyModeComponentItem>[13];

        public List<ModifyModeComponentItem> ModeComponentItems
        {
            get => _modeComponentItems;

            set => SetProperty(ref _modeComponentItems, value, nameof(ModeComponentItems));
        }

        public int ModeComponentVariety
        {
            get => _modeComponentValueVariety;

            set
            {
                _modeComponentValueVariety = value;
                var noteFile = ViewModels.Instance.MainValue?.EntryItemValue?.NoteFile;
                var inputMode = noteFile?.InputMode ?? default;
                var autoableInputCount = Component.AutoableInputCounts[(int)inputMode];
                switch (value)
                {
                    case AutoModeVariety:
                        var autoModeComponent = ModeComponentValues[AutoModeVariety][(int)ModeComponent.AutoMode.Autoable];
                        if (autoableInputCount > 0 && noteFile?.AutoableNotes > 0)
                        {
                            autoModeComponent.PointedPaintID = 1;
                        }
                        else
                        {
                            autoModeComponent.PointedPaintID = 0;
                        }
                        break;
                    case InputFavorModeVariety:
                        var inputCount = Component.InputCounts[(int)inputMode];
                        foreach (var inputFavorModeComponentValue in ModeComponentValues[InputFavorModeVariety])
                        {
                            var inputFavorModeValue = (ModeComponent.InputFavorMode)inputFavorModeComponentValue.Value;
                            if (inputFavorModeValue != ModeComponent.InputFavorMode.Default && (Component.AutoableInputCounts[(int)inputFavorModeValue] < autoableInputCount || Component.InputCounts[(int)inputFavorModeValue] < inputCount))
                            {
                                inputFavorModeComponentValue.PointedPaintID = 1;
                            }
                            else
                            {
                                inputFavorModeComponentValue.PointedPaintID = 0;
                            }
                        }
                        break;
                    case LongNoteModeVariety:
                        var longNoteModeComponent = ModeComponentValues[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Auto];
                        if (noteFile?.LongNotes > 0)
                        {
                            longNoteModeComponent.PointedPaintID = 1;
                        }
                        else
                        {
                            longNoteModeComponent.PointedPaintID = 0;
                        }
                        break;
                }
                ModeComponentItems = ModeComponentValues[value];
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

        public ModifyModeComponentItem ModeComponentItem
        {
            get => _modeComponentItem;

            set
            {
                if (SetProperty(ref _modeComponentItem, value, nameof(ModeComponentItem)) && value != null)
                {
                    var modeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;
                    switch (ModeComponentVariety)
                    {
                        case AutoModeVariety:
                            modeComponentValue.AutoModeValue = (ModeComponent.AutoMode)value.Value;
                            break;
                        case NoteSaltModeVariety:
                            modeComponentValue.NoteSaltModeValue = (ModeComponent.NoteSaltMode)value.Value;
                            break;
                        case FaintNoteModeVariety:
                            modeComponentValue.FaintNoteModeValue = (ModeComponent.FaintNoteMode)value.Value;
                            break;
                        case JudgmentModeVariety:
                            modeComponentValue.JudgmentModeValue = (ModeComponent.JudgmentMode)value.Value;
                            break;
                        case HitPointsModeVariety:
                            modeComponentValue.HitPointsModeValue = (ModeComponent.HitPointsMode)value.Value;
                            break;
                        case NoteMobilityModeVariety:
                            modeComponentValue.NoteMobilityModeValue = (ModeComponent.NoteMobilityMode)value.Value;
                            break;
                        case LongNoteModeVariety:
                            modeComponentValue.LongNoteModeValue = (ModeComponent.LongNoteMode)value.Value;
                            break;
                        case InputFavorModeVariety:
                            modeComponentValue.InputFavorModeValue = (ModeComponent.InputFavorMode)value.Value;
                            break;
                        case NoteModifyModeVariety:
                            modeComponentValue.NoteModifyModeValue = (ModeComponent.NoteModifyMode)value.Value;
                            break;
                        case BPMModeVariety:
                            modeComponentValue.BPMModeValue = (ModeComponent.BPMMode)value.Value;
                            break;
                        case WaveModeVariety:
                            modeComponentValue.WaveModeValue = (ModeComponent.WaveMode)value.Value;
                            break;
                        case SetNoteModeVariety:
                            modeComponentValue.SetNoteModeValue = (ModeComponent.SetNoteMode)value.Value;
                            break;
                        case LowestJudgmentConditionModeVariety:
                            modeComponentValue.LowestJudgmentConditionModeValue = (ModeComponent.LowestJudgmentConditionMode)value.Value;
                            break;
                    }
                }
            }
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

        public void SetModeComponentValues()
        {
            for (var i = ModeComponentValues.Length - 1; i >= 0; --i)
            {
                ModeComponentValues[i] = new();
            }
            ModeComponentValues[AutoModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.AutoMode.Default,
                Data = LanguageSystem.Instance.AutoModeTexts[(int)ModeComponent.AutoMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[AutoModeVariety][(int)ModeComponent.AutoMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[AutoModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.AutoMode.Autoable,
                Data = LanguageSystem.Instance.AutoModeTexts[(int)ModeComponent.AutoMode.Autoable],
                Drawing = BaseUI.Instance.ModeComponentDrawings[AutoModeVariety][(int)ModeComponent.AutoMode.Autoable]?.DefaultDrawing
            });
            ModeComponentValues[NoteSaltModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteSaltMode.Default,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[NoteSaltModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteSaltMode.Symmetric,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Symmetric],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.Symmetric]?.DefaultDrawing
            });
            ModeComponentValues[NoteSaltModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteSaltMode.InputSalt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.InputSalt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.InputSalt]?.DefaultDrawing
            });
            ModeComponentValues[NoteSaltModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteSaltMode.MeterSalt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.MeterSalt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.MeterSalt]?.DefaultDrawing
            });
            ModeComponentValues[NoteSaltModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteSaltMode.HalfInputSalt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.HalfInputSalt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.HalfInputSalt]?.DefaultDrawing
            });
            ModeComponentValues[NoteSaltModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteSaltMode.Salt,
                Data = LanguageSystem.Instance.NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Salt],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteSaltModeVariety][(int)ModeComponent.NoteSaltMode.Salt]?.DefaultDrawing
            });
            ModeComponentValues[FaintNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.FaintNoteMode.Default,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[FaintNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.FaintNoteMode.Faint,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Faint],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.Faint]?.DefaultDrawing
            });
            ModeComponentValues[FaintNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.FaintNoteMode.Fading,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Fading],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.Fading]?.DefaultDrawing
            });
            ModeComponentValues[FaintNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.FaintNoteMode.TotalFading,
                Data = LanguageSystem.Instance.FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.TotalFading],
                Drawing = BaseUI.Instance.ModeComponentDrawings[FaintNoteModeVariety][(int)ModeComponent.FaintNoteMode.TotalFading]?.DefaultDrawing
            });
            ModeComponentValues[JudgmentModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.JudgmentMode.Lowest,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Lowest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Lowest]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModeComponentValues[JudgmentModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.JudgmentMode.Lower,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Lower],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Lower]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModeComponentValues[JudgmentModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.JudgmentMode.Default,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[JudgmentModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.JudgmentMode.Higher,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Higher],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Higher]?.DefaultDrawing
            });
            ModeComponentValues[JudgmentModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.JudgmentMode.Highest,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Highest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Highest]?.DefaultDrawing
            });
            ModeComponentValues[JudgmentModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.JudgmentMode.Favor,
                Data = LanguageSystem.Instance.JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Favor],
                Drawing = BaseUI.Instance.ModeComponentDrawings[JudgmentModeVariety][(int)ModeComponent.JudgmentMode.Favor]?.DefaultDrawing,
                OnConfigure = new RelayCommand(() =>
                {
                    ViewModels.Instance.FavorJudgmentValue.Open();
                    ModeComponentVariety = JudgmentModeVariety;
                    ModeComponentItem = ModeComponentValues[JudgmentModeVariety].Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.JudgmentMode.Favor);
                }),
                PointedPaintID = 2
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Lowest,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Lowest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Lowest]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Lower,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Lower],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Lower]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Default,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Higher,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Higher],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Higher]?.DefaultDrawing
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Highest,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Highest],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Highest]?.DefaultDrawing
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Failed,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Failed],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Failed]?.DefaultDrawing
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Favor,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Favor],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Favor]?.DefaultDrawing,
                OnConfigure = new RelayCommand(() =>
                {
                    ViewModels.Instance.FavorHitPointsValue.Open();
                    ModeComponentVariety = HitPointsModeVariety;
                    ModeComponentItem = ModeComponentValues[HitPointsModeVariety].Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.HitPointsMode.Favor);
                }),
                PointedPaintID = 2
            });
            ModeComponentValues[HitPointsModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.HitPointsMode.Test,
                Data = LanguageSystem.Instance.HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Test],
                Drawing = BaseUI.Instance.ModeComponentDrawings[HitPointsModeVariety][(int)ModeComponent.HitPointsMode.Test]?.DefaultDrawing,
                PointedPaintID = 2
            });
            ModeComponentValues[NoteMobilityModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteMobilityMode.Default,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[NoteMobilityModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteMobilityMode._4D,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode._4D],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode._4D]?.DefaultDrawing
            });
            ModeComponentValues[NoteMobilityModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteMobilityMode._4DHD,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode._4DHD],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode._4DHD]?.DefaultDrawing
            });
            ModeComponentValues[NoteMobilityModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteMobilityMode.Zip,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.Zip],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode.Zip]?.DefaultDrawing
            });
            ModeComponentValues[NoteMobilityModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteMobilityMode.ZipHD,
                Data = LanguageSystem.Instance.NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.ZipHD],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteMobilityModeVariety][(int)ModeComponent.NoteMobilityMode.ZipHD]?.DefaultDrawing
            });
            ModeComponentValues[LongNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.LongNoteMode.Default,
                Data = LanguageSystem.Instance.LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[LongNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.LongNoteMode.Auto,
                Data = LanguageSystem.Instance.LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Auto],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Auto]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModeComponentValues[LongNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.LongNoteMode.Input,
                Data = LanguageSystem.Instance.LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Input],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LongNoteModeVariety][(int)ModeComponent.LongNoteMode.Input]?.DefaultDrawing,
                PointedPaintID = 2
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Default,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode4,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode4],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode4]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode5,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode5],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode5]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode6,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode6],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode6]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode7,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode7],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode7]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode8,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode8],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode8]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode9,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode9],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode9]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode10,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode10],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode10]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode51,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode51],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode51]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode71,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode71],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode71]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode102,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode102],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode102]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode142,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode142],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode142]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode242,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode242],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode242]?.DefaultDrawing
            });
            ModeComponentValues[InputFavorModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.InputFavorMode.Mode484,
                Data = LanguageSystem.Instance.InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Mode484],
                Drawing = BaseUI.Instance.ModeComponentDrawings[InputFavorModeVariety][(int)ModeComponent.InputFavorMode.Mode484]?.DefaultDrawing
            });
            ModeComponentValues[NoteModifyModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteModifyMode.Default,
                Data = LanguageSystem.Instance.DefaultNoteModifyContents,
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteModifyModeVariety][(int)ModeComponent.NoteModifyMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[NoteModifyModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteModifyMode.InputNote,
                Data = LanguageSystem.Instance.NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.InputNote],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteModifyModeVariety][(int)ModeComponent.NoteModifyMode.InputNote]?.DefaultDrawing,
                PointedPaintID = 1
            });
            ModeComponentValues[NoteModifyModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.NoteModifyMode.LongNote,
                Data = LanguageSystem.Instance.NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.LongNote],
                Drawing = BaseUI.Instance.ModeComponentDrawings[NoteModifyModeVariety][(int)ModeComponent.NoteModifyMode.LongNote]?.DefaultDrawing,
                OnConfigure = new RelayCommand(() =>
                {
                    ViewModels.Instance.LongNoteModifyValue.Open();
                    ModeComponentVariety = NoteModifyModeVariety;
                    ModeComponentItem = ModeComponentValues[NoteModifyModeVariety].Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.NoteModifyMode.LongNote);
                }),
                PointedPaintID = 2
            });
            ModeComponentValues[BPMModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.BPMMode.Default,
                Data = LanguageSystem.Instance.BPMModeTexts[(int)ModeComponent.BPMMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[BPMModeVariety][(int)ModeComponent.BPMMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[BPMModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.BPMMode.Not,
                Data = LanguageSystem.Instance.BPMModeTexts[(int)ModeComponent.BPMMode.Not],
                Drawing = BaseUI.Instance.ModeComponentDrawings[BPMModeVariety][(int)ModeComponent.BPMMode.Not]?.DefaultDrawing,
                PointedPaintID = 2
            });
            ModeComponentValues[WaveModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.WaveMode.Default,
                Data = LanguageSystem.Instance.WaveModeTexts[(int)ModeComponent.WaveMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[WaveModeVariety][(int)ModeComponent.WaveMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[WaveModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.WaveMode.Counter,
                Data = LanguageSystem.Instance.WaveModeTexts[(int)ModeComponent.WaveMode.Counter],
                Drawing = BaseUI.Instance.ModeComponentDrawings[WaveModeVariety][(int)ModeComponent.WaveMode.Counter]?.DefaultDrawing,
                PointedPaintID = 2
            });
            ModeComponentValues[SetNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.SetNoteMode.Default,
                Data = LanguageSystem.Instance.SetNoteModeTexts[(int)ModeComponent.SetNoteMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[SetNoteModeVariety][(int)ModeComponent.SetNoteMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[SetNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.SetNoteMode.Put,
                Data = LanguageSystem.Instance.SetNoteModeTexts[(int)ModeComponent.SetNoteMode.Put],
                Drawing = BaseUI.Instance.ModeComponentDrawings[SetNoteModeVariety][(int)ModeComponent.SetNoteMode.Put]?.DefaultDrawing,
                OnConfigure = new RelayCommand(() =>
                {
                    ViewModels.Instance.PutNoteSetValue.Open();
                    ModeComponentVariety = SetNoteModeVariety;
                    ModeComponentItem = ModeComponentValues[SetNoteModeVariety].Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.SetNoteMode.Put);
                }),
                PointedPaintID = 2
            });
            ModeComponentValues[SetNoteModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.SetNoteMode.VoidPut,
                Data = LanguageSystem.Instance.SetNoteModeTexts[(int)ModeComponent.SetNoteMode.VoidPut],
                Drawing = BaseUI.Instance.ModeComponentDrawings[SetNoteModeVariety][(int)ModeComponent.SetNoteMode.VoidPut]?.DefaultDrawing,
                OnConfigure = new RelayCommand(() =>
                {
                    ViewModels.Instance.PutNoteSetValue.Open();
                    ModeComponentVariety = SetNoteModeVariety;
                    ModeComponentItem = ModeComponentValues[SetNoteModeVariety].Find(modeComponentItem => modeComponentItem.Value == (int)ModeComponent.SetNoteMode.VoidPut);
                }),
                PointedPaintID = 2
            });
            ModeComponentValues[LowestJudgmentConditionModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.LowestJudgmentConditionMode.Default,
                Data = LanguageSystem.Instance.LowestJudgmentConditionModeTexts[(int)ModeComponent.LowestJudgmentConditionMode.Default],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LowestJudgmentConditionModeVariety][(int)ModeComponent.LowestJudgmentConditionMode.Default]?.DefaultDrawing
            });
            ModeComponentValues[LowestJudgmentConditionModeVariety].Add(new ModifyModeComponentItem
            {
                Value = (int)ModeComponent.LowestJudgmentConditionMode.Wrong,
                Data = LanguageSystem.Instance.LowestJudgmentConditionModeTexts[(int)ModeComponent.LowestJudgmentConditionMode.Wrong],
                Drawing = BaseUI.Instance.ModeComponentDrawings[LowestJudgmentConditionModeVariety][(int)ModeComponent.LowestJudgmentConditionMode.Wrong]?.DefaultDrawing
            });
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
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