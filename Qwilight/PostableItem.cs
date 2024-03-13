using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.UI;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight
{
    public sealed class PostableItem
    {
        public enum Variety
        {
            PositiveJudgment,
            PositiveHitPoints,
            PositiveHitPointsLevel,
            PositiveAegis,
            NegativeFaint,
            NegativeFading,
            NegativeJudgment,
            NegativeHitPoints,
            NegativeHitPointsLevel,
            Negative4D,
            NegativeZip,
            NegativeTrapNotes,
            NegativeAutoableNotes,
            NegativeSalt,
            LowerAudioMultiplier,
            HigherAudioMultiplier,
            Pause
        }

        public static readonly PostableItem[] Values = new[]
        {
            new PostableItem
            {
                VarietyValue = Variety.PositiveJudgment,
                IsPositive = true,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.PositiveHitPoints,
                IsPositive = true,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.PositiveHitPointsLevel,
                IsPositive = true,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.PositiveAegis,
                IsPositive = true,
                LowestWait = double.MaxValue,
                HighestWait = double.MaxValue
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeFaint,
                IsPositive = false,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeFading,
                IsPositive = false,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeJudgment,
                IsPositive = false,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeHitPoints,
                IsPositive = false,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeHitPointsLevel,
                IsPositive = false,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.Negative4D,
                IsPositive = false,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeZip,
                IsPositive = false,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeTrapNotes,
                IsPositive = false,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeAutoableNotes,
                IsPositive = false,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.NegativeSalt,
                IsPositive = false,
                LowestWait = 60.0,
                HighestWait = 1000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.LowerAudioMultiplier,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.HigherAudioMultiplier,
                LowestWait = 1000.0,
                HighestWait = 6000.0
            },
            new PostableItem
            {
                VarietyValue = Variety.Pause,
                LowestWait = 60.0,
                HighestWait = 1000.0
            }
        };

        public Variety VarietyValue { get; set; }

        public bool? IsPositive { get; set; }

        public double LowestWait { get; set; }

        public double HighestWait { get; set; }

        public Color ItemColor => IsPositive switch
        {
            true => Colors.DeepSkyBlue,
            false => Colors.DeepPink,
            null => Colors.Gray
        };

        public Brush ItemPaint => DrawingSystem.Instance.GetDefaultPaint(ItemColor);

        public ICanvasBrush[] ItemPaints => IsPositive switch
        {
            true => DrawingSystem.Instance.FaintPositiveItemPaints,
            false => DrawingSystem.Instance.FaintNegativeItemPaints,
            null => DrawingSystem.Instance.FaintNeutralItemPaints
        };

        public override string ToString() => VarietyValue switch
        {
            Variety.PositiveJudgment => "💊 EASY JUDGMENT",
            Variety.PositiveHitPoints => "💊 REGENERATION",
            Variety.PositiveHitPointsLevel => "💊 EASY GAUGE",
            Variety.PositiveAegis => "💊 SHIELD",
            Variety.NegativeJudgment => "💣 HARD JUDGMENT",
            Variety.NegativeHitPoints => "💣 BLEEDING",
            Variety.NegativeHitPointsLevel => "💣 HARD GAUGE",
            Variety.NegativeFaint => "💣 FADE OUT",
            Variety.NegativeFading => "💣 BLINK",
            Variety.Negative4D => "💣 4D",
            Variety.NegativeZip => "💣 COMPRESSION",
            Variety.NegativeTrapNotes => "💣 MINEFIELD",
            Variety.NegativeAutoableNotes => "💣 SCRATCH",
            Variety.NegativeSalt => "💣 RANDOM",
            Variety.LowerAudioMultiplier => "🌐 SLOWER",
            Variety.HigherAudioMultiplier => "🌐 FASTER",
            Variety.Pause => "🌐 PAUSE",
            _ => default
        };
    }
}
