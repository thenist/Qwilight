using Qwilight.ViewModel;
using System.Text.Json.Serialization;

namespace Qwilight
{
    public sealed class UIConfigure : Model
    {
        double _mainFaint = 1.0;
        double _mediaFaint = 1.0;
        double _netItemFaint = 1.0;
        double _audioVisualizerFaint = 0.1;
        double _judgmentMainPosition1;
        double _noteWait;
        double _bandPosition;
        double _judgmentPaintPosition;

        public string[] UIConfiguresV2 { get; set; } = new string[UI.HighestUIConfigure];

        public double MainFaintV2
        {
            get => _mainFaint;

            set => SetProperty(ref _mainFaint, value, nameof(MainFaintV2));
        }

        public double MediaFaintV2
        {
            get => _mediaFaint;

            set => SetProperty(ref _mediaFaint, value, nameof(MediaFaintV2));
        }

        public double NetItemFaintV2
        {
            get => _netItemFaint;

            set => SetProperty(ref _netItemFaint, value, nameof(NetItemFaintV2));
        }

        public double AudioVisualizerFaintV2
        {
            get => _audioVisualizerFaint;

            set => SetProperty(ref _audioVisualizerFaint, value, nameof(AudioVisualizerFaintV2));
        }

        public double JudgmentMainPosition1V2
        {
            get => _judgmentMainPosition1;

            set
            {
                if (SetProperty(ref _judgmentMainPosition1, value, nameof(JudgmentMainPosition1V2)))
                {
                    OnPropertyChanged(nameof(JudgmentMainPosition1Contents));
                }
            }
        }

        public string JudgmentMainPosition1Contents => JudgmentMainPosition1V2.ToString(LanguageSystem.Instance.PointLevelContents);

        public double NoteWait
        {
            get => _noteWait;

            set
            {
                if (SetProperty(ref _noteWait, value, nameof(NoteWait)))
                {
                    OnPropertyChanged(nameof(NoteWaitContents));
                }
            }
        }

        public string NoteWaitContents => NoteWait.ToString(LanguageSystem.Instance.PointLevelContents);

        public Dictionary<int, double> MainPositions { get; set; } = new();

        [JsonIgnore]
        public double MainPosition
        {
            get => MainPositions.TryGetValue((int)InputMode, out var mainPosition) ? mainPosition : 0.0;

            set
            {
                MainPositions[(int)InputMode] = value;
                OnPropertyChanged(nameof(MainPosition));
                OnPropertyChanged(nameof(MainPositionContents));
            }
        }

        public string MainPositionContents => MainPosition.ToString(LanguageSystem.Instance.PointLevelContents);

        public Dictionary<int, double> NoteLengths { get; set; } = new();

        [JsonIgnore]
        public double NoteLength
        {
            get => NoteLengths.TryGetValue((int)InputMode, out var noteLength) ? noteLength : 0.0;

            set
            {
                NoteLengths[(int)InputMode] = value;
                OnPropertyChanged(nameof(NoteLength));
                OnPropertyChanged(nameof(NoteLengthContents));
            }
        }

        public string NoteLengthContents => NoteLength.ToString(LanguageSystem.Instance.PointLevelContents);

        public Dictionary<int, double> NoteHeights { get; set; } = new();

        [JsonIgnore]
        public double NoteHeight
        {
            get => NoteHeights.TryGetValue((int)InputMode, out var noteHeight) ? noteHeight : 0.0;

            set
            {
                NoteHeights[(int)InputMode] = value;
                OnPropertyChanged(nameof(NoteHeight));
                OnPropertyChanged(nameof(NoteHeightContents));
            }
        }

        public string NoteHeightContents => NoteHeight.ToString(LanguageSystem.Instance.PointLevelContents);

        public Component.InputMode InputMode => ViewModels.Instance.MainValue.GetHandlingComputing()?.InputMode ?? default;

        public double BandPositionV2
        {
            get => _bandPosition;

            set
            {
                if (SetProperty(ref _bandPosition, value, nameof(BandPositionV2)))
                {
                    OnPropertyChanged(nameof(BandPositionContents));
                }
            }
        }

        public string BandPositionContents => BandPositionV2.ToString(LanguageSystem.Instance.PointLevelContents);

        public double JudgmentPaintPosition
        {
            get => _judgmentPaintPosition;

            set
            {
                if (SetProperty(ref _judgmentPaintPosition, value, nameof(JudgmentPaintPosition)))
                {
                    OnPropertyChanged(nameof(JudgmentPaintPositionContents));
                }
            }
        }

        public string JudgmentPaintPositionContents => JudgmentPaintPosition.ToString(LanguageSystem.Instance.PointLevelContents);

        public Dictionary<int, double> HitNotePaintAreas { get; set; } = new();

        [JsonIgnore]
        public double HitNotePaintArea
        {
            get => HitNotePaintAreas.TryGetValue((int)InputMode, out var hitNotePaintArea) ? hitNotePaintArea : 0.0;

            set
            {
                HitNotePaintAreas[(int)InputMode] = value;
                OnPropertyChanged(nameof(HitNotePaintArea));
                OnPropertyChanged(nameof(HitNotePaintAreaContents));
            }
        }

        public string HitNotePaintAreaContents => HitNotePaintArea.ToString(LanguageSystem.Instance.PointLevelContents);

        public void NotifyInputMode()
        {
            OnPropertyChanged(nameof(MainPosition));
            OnPropertyChanged(nameof(MainPositionContents));
            OnPropertyChanged(nameof(NoteLength));
            OnPropertyChanged(nameof(NoteLengthContents));
            OnPropertyChanged(nameof(NoteHeight));
            OnPropertyChanged(nameof(NoteHeightContents));
            OnPropertyChanged(nameof(HitNotePaintArea));
            OnPropertyChanged(nameof(HitNotePaintAreaContents));
            OnPropertyChanged(nameof(InputMode));
        }
    }
}
