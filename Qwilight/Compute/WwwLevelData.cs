namespace Qwilight.Compute
{
    public sealed class WwwLevelData
    {
        public bool IsStandSatisify { get; set; }

        public bool IsPointSatisify { get; set; }

        public bool IsBandSatisify { get; set; }

        public bool[] IsJudgmentsSatisify { get; } = new bool[6];

        public void SetSatisify(DefaultCompute defaultComputer)
        {
            IsStandSatisify = IsSatisify(defaultComputer.Stand.TargetValue, Stand);
            IsPointSatisify = IsSatisify(defaultComputer.Point.TargetValue, Point);
            IsBandSatisify = IsSatisify(defaultComputer.Band.TargetValue, Band);
            IsJudgmentsSatisify[(int)Component.Judged.Highest] = IsSatisify(defaultComputer.InheritedHighestJudgment + defaultComputer.Comment.HighestJudgment, Judgments?[(int)Component.Judged.Highest]);
            IsJudgmentsSatisify[(int)Component.Judged.Higher] = IsSatisify(defaultComputer.InheritedHigherJudgment + defaultComputer.Comment.HigherJudgment, Judgments?[(int)Component.Judged.Higher]);
            IsJudgmentsSatisify[(int)Component.Judged.High] = IsSatisify(defaultComputer.InheritedHighJudgment + defaultComputer.Comment.HighJudgment, Judgments?[(int)Component.Judged.High]);
            IsJudgmentsSatisify[(int)Component.Judged.Low] = IsSatisify(defaultComputer.InheritedLowJudgment + defaultComputer.Comment.LowJudgment, Judgments?[(int)Component.Judged.Low]);
            IsJudgmentsSatisify[(int)Component.Judged.Lower] = IsSatisify(defaultComputer.InheritedLowerJudgment + defaultComputer.Comment.LowerJudgment, Judgments?[(int)Component.Judged.Lower]);
            IsJudgmentsSatisify[(int)Component.Judged.Lowest] = IsSatisify(defaultComputer.InheritedLowestJudgment + defaultComputer.Comment.LowestJudgment, Judgments?[(int)Component.Judged.Lowest]);
        }

        static bool IsSatisify(int value, int[] values)
        {
            return values == null || value == Math.Clamp(value, values[0] != -1 ? values[0] : int.MinValue, values[1] != -1 ? values[1] : int.MaxValue);
        }

        static bool IsSatisify(double value, double[] values)
        {
            return values == null || value == Math.Clamp(value, values[0] != -1.0 ? values[0] : double.MinValue, values[1] != -1.0 ? values[1] : double.MaxValue);
        }

        public double LowestAudioMultiplier { get; set; }

        public double HighestAudioMultiplier { get; set; }

        public int[] Stand { get; set; }

        public string StandContents { get; set; } = string.Empty;

        public double[] Point { get; set; }

        public string PointContents { get; set; } = string.Empty;

        public int[] Band { get; set; }

        public string BandContents { get; set; } = string.Empty;

        public int[][] Judgments { get; set; }

        public string[] JudgmentContents { get; } = new string[6];

        public bool AllowPause { get; set; }
    }
}
