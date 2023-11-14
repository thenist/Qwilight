using FMOD;
using Microsoft.Graphics.Canvas;
using Qwilight.Compute;

namespace Qwilight.Note
{
    public abstract class BaseNote : IComparable<BaseNote>
    {
        public const int LowestLayer = 0;
        public const int HighestLayer = 4;

        int _levyingInput;

        public PostableItem PostableItemValue { get; set; }

        public bool IsWiped { get; set; }

        public double LogicalY { get; set; }

        public int ID { get; set; }

        public double Wait { get; set; }

        public List<AudioNote> AudioNotes { get; } = new();

        public List<Channel> AudioChannels { get; } = new();

        public double Y { get; set; }

        public int Salt { get; set; }

        public int InputSalt { get; set; }

        public bool IsFailed { get; set; }

        public double LongWait { get; set; }

        public double LongHeight { get; set; }

        public Component.Judged Judged { get; set; } = Component.Judged.Not;

        public int LevyingInput
        {
            get => _levyingInput;

            set
            {
                _levyingInput = value;
                TargetInput = value;
            }
        }

        public int TargetInput { get; set; }

        public abstract bool HasContents { get; }

        public abstract bool HasStand { get; }

        public abstract bool HasInput { get; }

        public abstract int Layer { get; set; }

        public abstract int LogicalLayer { get; }

        public BaseNote(double logicalY, double wait)
        {
            LogicalY = logicalY;
            Wait = wait;
            Init();
        }

        public abstract bool IsVisible(DefaultCompute defaultComputer);

        public abstract bool IsVisibleHalf(DefaultCompute defaultComputer);

        public virtual void Init()
        {
            Y = double.NegativeInfinity;
            Judged = Component.Judged.Not;
            IsFailed = false;
            AudioChannels.Clear();
            IsWiped = false;
            TargetInput = LevyingInput;
        }

        public void SetItem(int salt, PostableItem[] allowedPostableItems) => PostableItemValue = allowedPostableItems[salt % allowedPostableItems.Length];

        public double GetY(DefaultCompute defaultComputer, double multiplier) => (Y - Component.StandardHeight) * multiplier + defaultComputer.DrawingComponentValue.judgmentMainPosition + Configure.Instance.UIConfigureValue.NoteWait;

        public float GetFaint(ModeComponent modeComponentValue, double judgmentMainPosition, double faintCosine) => modeComponentValue.FaintNoteModeValue switch
        {
            ModeComponent.FaintNoteMode.Fading => (float)faintCosine,
            _ => 1F,
        };

        public bool IsClose(double wait) => Wait - wait <= Component.LevyingWait;

        public double GetMultiplierAsNoteMobility(ModeComponent modeComponentValue, double noteMobilityCosine, double noteMobilityValue) => modeComponentValue.NoteMobilityModeValue switch
        {
            ModeComponent.NoteMobilityMode._4D => modeComponentValue.Multiplier + modeComponentValue.Multiplier * ((InputSalt % 11 + 5) / 10 - 1) * noteMobilityValue,
            ModeComponent.NoteMobilityMode._4DHD => modeComponentValue.Multiplier + modeComponentValue.Multiplier * ((Salt % 11 + 5) / 10 - 1) * noteMobilityValue,
            ModeComponent.NoteMobilityMode.Zip => Math.Max(1.0, modeComponentValue.Multiplier * noteMobilityCosine),
            ModeComponent.NoteMobilityMode.ZipHD => modeComponentValue.Multiplier * noteMobilityCosine,
            _ => modeComponentValue.Multiplier
        };

        public void InitY(double logicalY) => Y = LogicalY - logicalY;

        public void Move(double distance) => Y += distance;

        public int CompareTo(BaseNote other)
        {
            var value = Wait.CompareTo(other.Wait);
            if (value != 0)
            {
                return value;
            }

            value = LogicalLayer.CompareTo(other.LogicalLayer);
            if (value != 0)
            {
                return value;
            }

            value = TargetInput.CompareTo(other.TargetInput);
            if (value != 0)
            {
                return value;
            }

            return 0;
        }

        public virtual bool IsFailedAsTooLate(double wait, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate) => false;

        public virtual bool IsTooLong(double wait, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate) => false;

        public abstract void MoveInputMillis(double millisLoopUnit, DefaultCompute defaultComputer);

        public abstract void SetLayer(DefaultCompute defaultComputer);

        public abstract JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, int trapNoteJudgmentDate, bool isAutoLongNote);

        public abstract JudgedNoteData? AutoJudge(double wait);

        public abstract void Paint(CanvasDrawingSession targetSession, bool isValidNetDrawings, DefaultCompute defaultComputer, ref Bound r);

        public bool IsCollided(BaseNote note) => (Wait <= note.Wait && note.Wait <= Wait + LongWait) || (note.Wait <= Wait && Wait <= note.Wait + note.LongWait);
    }
}