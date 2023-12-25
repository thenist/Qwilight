namespace Qwilight
{
    public sealed class Component
    {
        public enum JudgmentAssist
        {
            Default, LongNoteUp
        }

        public enum InputMapping
        {
            Mapping0, Mapping1, Mapping2, Mapping3
        }

        public enum InputMode
        {
            _4 = 4, _5, _6, _7, _8, _9, _5_1, _7_1, _10_2, _14_2, _10, _24_2, _48_4
        }

        public enum Judged
        {
            Not = -1, Highest, Higher, High, Low, Lower, Lowest, Band1 = 9, Last = 10
        }

        public const double StandardLength = 1280.0;
        public const double StandardHeight = 720.0;
        public const double StandardBPM = 130.0;
        public const int StandardMeter = 192;
        public const double LevyingWait = 3000.0;
        public const double QuitWait = 2000.0;
        public const double PassableWait = 1000.0;

        public enum HitPointsModeDate
        {
            _1_0_0, _1_2_3, _1_10_34, _1_10_35, _1_14_62
        };

        public enum HitPointsMapDate
        {
            _1_0_0, _1_6_7, _1_7_0, _1_13_2
        };

        public enum PointMapDate
        {
            _1_0_0, _1_6_7
        };

        public enum JudgmentMapDate
        {
            _1_0_0, _1_3_0, _1_6_7, _1_6_8, _1_10_34, _1_10_35, _1_11_0, Max
        };
        public const JudgmentMapDate LatestJudgmentMapDate = JudgmentMapDate.Max - 1;

        public enum JudgmentModeDate
        {
            _1_0_0, _1_6_7, _1_10_34, _1_10_35, _1_14_6, Max
        };
        public const JudgmentModeDate LatestJudgmentModeDate = JudgmentModeDate.Max - 1;

        public enum LongNoteAssistDate
        {
            _1_0_0, _1_6_7, _1_10_34, _1_10_35, Max
        };
        public const LongNoteAssistDate LatestLongNoteAssistDate = LongNoteAssistDate.Max - 1;

        public enum StandModeDate
        {
            _1_0_0, _1_6_7, _1_14_118
        };

        public enum StandMapDate
        {
            _1_0_0, _1_14_118
        };

        public enum CommentWaitDate
        {
            _1_0_0, _1_3_11, _1_6_4
        };

        public enum TooLongLongNoteDate
        {
            _1_0_0, _1_13_107, _1_14_20, _1_14_29
        };

        public enum TrapNoteJudgmentDate
        {
            _1_0_0, _1_14_6
        };

        public enum LongNoteModeDate
        {
            _1_0_0, _1_14_20, _1_16_4, Max
        };
        public const LongNoteModeDate LatestLongNoteModeDate = LongNoteModeDate.Max - 1;

        public enum PaintEventsDate
        {
            _1_0_0, _1_14_91
        };

        public enum NoteSaltModeDate
        {
            _1_0_0, _1_14_27, _1_6_11
        };

        static readonly double[,,,] JudgmentMap = new double[7, 5, 6, 2];
        static readonly double[] LongNoteAssistMap = new double[4];
        static readonly double[] JudgmentCalibrateMap = new double[3];

        public static readonly double[,] StandMap = new double[2, 6];
        public static readonly double[,] PointMap = new double[2, 6];
        public static readonly double[,,] HitPointsMap = new double[4, 8, 6];
        public static readonly int[] InputCounts = new int[17];
        public static readonly int[][] AutoableInputs = new int[17][];
        public static readonly int[] AutoableInputCounts = new int[17];
        public static readonly int[,][] InputMappingValues = new int[4, 17][];
        public static readonly int[,][] BasePaintMap = new int[4, 17][];
        public static readonly int[,][] FavorInputs = new int[17, 17][];
        public static readonly Func<int, double, double>[,] FillLambdas = new Func<int, double, double>[17, 30];
        public static readonly int[,][] VoidPutInputs = new int[17, 17][];
        public static readonly bool[][] IsIn2P = new bool[17][];
        public static readonly int[] Input1PCounts = new int[17];
        public static readonly bool[] Has2P = new bool[17];
        public static readonly int[,,][] LimiterCenterMap = new int[4, 17, 2][];
        public static readonly bool[][] Limiter57Map = new bool[17][];

        static Component()
        {
            #region v1.0.0
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 0.9;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 0.3;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = 0.1 / 3;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Highest] = 1.5;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Higher] = 1.35;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.High] = 0.45;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Low] = 0.15;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = 0.025;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Highest] = 0.75;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Higher] = 0.675;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.High] = 0.225;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Low] = 0.075;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = 0.05;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Highest] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Higher] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.High] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Low] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_0_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = 1.0;
            #endregion
            #region v1.6.7
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 0.9;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 0.3;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = -0.027;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Highest] = 2.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Higher] = 1.8;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.High] = 0.6;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Low] = 0.2;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lowest] = -0.0135;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Highest] = 1.5;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Higher] = 1.35;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.High] = 0.45;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Low] = 0.15;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = -0.02025;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Highest] = 0.75;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Higher] = 0.675;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.High] = 0.225;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Low] = 0.075;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = -0.0405;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Highest] = 0.5;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Higher] = 0.45;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.High] = 0.15;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Low] = 0.05;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lowest] = -0.054;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Highest] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Higher] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.High] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Low] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_6_7, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = -1.0;
            #endregion
            #region v1.7.0
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 0.2 / 3;
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 0.1 / 3;
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = 0.1 / 3;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Lowest, i] = 2.0 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Lower, i] = 1.5 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Higher, i] = 0.75 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Highest, i] = 0.5 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Failed, i] = 0.0;
            }
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lowest] = 0.5 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = 0.75 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = 1.5 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lowest] = 2.0 * HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_7_0, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = 1.0;
            #endregion
            #region v1.13.2
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 2.0 / 3;
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 1.0 / 3;
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = 0.1 / 3;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Lowest, i] = 2.0 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Lower, i] = 1.5 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Higher, i] = 0.75 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Highest, i] = 0.5 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Failed, i] = 0.0;
            }
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lowest] = 0.5 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = 0.75 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = 1.5 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lowest] = 2.0 * HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = 1.0;
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Lowest] = HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Lower] = HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Low] = HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Test, (int)Judged.High] = HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Higher] = HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher];
            HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Highest] = HitPointsMap[(int)HitPointsMapDate._1_13_2, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest];
            #endregion

            #region v1.0.0
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -16.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -64.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -97.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -127.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -151.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -211.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 0] = -22.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 0] = -89.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 0] = -135.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 0] = -177.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 0] = -211.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = -211.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 0] = -11.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 0] = -45.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 0] = -69.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 0] = -90.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 0] = -107.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = -211.5;

            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 1] = 16.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 1] = 64.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 1] = 97.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 1] = 127.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 1] = 151.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 1] = 211.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 1] = 22.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 1] = 89.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 1] = 135.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 1] = 177.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 1] = 211.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 1] = 211.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 1] = 11.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 1] = 45.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 1] = 69.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 1] = 90.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 1] = 107.5;
            JudgmentMap[(int)JudgmentMapDate._1_0_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 1] = 211.5;
            #endregion
            #region v1.3.0
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -16.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -64.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -97.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -127.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -151.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -169.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 0] = -22.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 0] = -89.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 0] = -135.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 0] = -177.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 0] = -211.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = -236.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 0] = -11.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 0] = -45.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 0] = -69.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 0] = -90.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 0] = -107.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = -121.5;

            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 1] = 16.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 1] = 64.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 1] = 97.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 1] = 127.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 1] = 151.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 1] = 169.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 1] = 22.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 1] = 89.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 1] = 135.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 1] = 177.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 1] = 211.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 1] = 236.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 1] = 11.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 1] = 45.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 1] = 69.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 1] = 90.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 1] = 107.5;
            JudgmentMap[(int)JudgmentMapDate._1_3_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 1] = 121.5;
            #endregion
            #region v1.6.7
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -15.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -60.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -90.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -125.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -210.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_6_7, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[(int)JudgmentMapDate._1_6_7, i, j, 1] = -JudgmentMap[(int)JudgmentMapDate._1_6_7, i, j, 0];
                }
            }
            #endregion
            #region v1.6.8
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -25.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -65.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -100.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -130.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -205.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_6_8, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[(int)JudgmentMapDate._1_6_8, i, j, 1] = -JudgmentMap[(int)JudgmentMapDate._1_6_8, i, j, 0];
                }
            }
            #endregion
            #region v1.10.34
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -20.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -45.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -75.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -110.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -150.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -195.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_34, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[(int)JudgmentMapDate._1_10_34, i, j, 1] = -JudgmentMap[(int)JudgmentMapDate._1_10_34, i, j, 0];
                }
            }
            #endregion
            #region v1.10.35
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -25.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -65.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -100.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -130.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -205.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_10_35, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[(int)JudgmentMapDate._1_10_35, i, j, 1] = -JudgmentMap[(int)JudgmentMapDate._1_10_35, i, j, 0];
                }
            }
            #endregion
            #region v1.11.0
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -25.0;
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -60.0;
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -95.0;
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -130.0;
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -200.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = JudgmentMap[(int)JudgmentMapDate._1_11_0, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[(int)JudgmentMapDate._1_11_0, i, j, 1] = -JudgmentMap[(int)JudgmentMapDate._1_11_0, i, j, 0];
                }
            }
            #endregion

            JudgmentCalibrateMap[(int)ModeComponent.JudgmentMode.Lower] = 4.2;
            JudgmentCalibrateMap[(int)ModeComponent.JudgmentMode.Default] = 3.0;
            JudgmentCalibrateMap[(int)ModeComponent.JudgmentMode.Higher] = 2.1;

            #region v1.0.0
            PointMap[(int)PointMapDate._1_0_0, (int)Judged.Highest] = 1.0;
            PointMap[(int)PointMapDate._1_0_0, (int)Judged.Higher] = 1.0;
            PointMap[(int)PointMapDate._1_0_0, (int)Judged.High] = 2.0 / 3;
            PointMap[(int)PointMapDate._1_0_0, (int)Judged.Low] = 1.0 / 3;
            PointMap[(int)PointMapDate._1_0_0, (int)Judged.Lower] = 1.0 / 6;
            PointMap[(int)PointMapDate._1_0_0, (int)Judged.Lowest] = 0.0;
            #endregion
            #region v1.6.7
            PointMap[(int)PointMapDate._1_6_7, (int)Judged.Highest] = 1.0;
            PointMap[(int)PointMapDate._1_6_7, (int)Judged.Higher] = 1.0;
            PointMap[(int)PointMapDate._1_6_7, (int)Judged.High] = 0.7;
            PointMap[(int)PointMapDate._1_6_7, (int)Judged.Low] = 0.5;
            PointMap[(int)PointMapDate._1_6_7, (int)Judged.Lower] = 0.3;
            PointMap[(int)PointMapDate._1_6_7, (int)Judged.Lowest] = 0.0;
            #endregion

            LongNoteAssistMap[(int)LongNoteAssistDate._1_0_0] = 1.0;
            LongNoteAssistMap[(int)LongNoteAssistDate._1_6_7] = 1.5;
            LongNoteAssistMap[(int)LongNoteAssistDate._1_10_34] = 2.0;
            LongNoteAssistMap[(int)LongNoteAssistDate._1_10_35] = 1.5;

            #region v1.0.0
            StandMap[(int)StandMapDate._1_0_0, (int)Judged.Highest] = 1.0;
            StandMap[(int)StandMapDate._1_0_0, (int)Judged.Higher] = 0.9;
            StandMap[(int)StandMapDate._1_0_0, (int)Judged.High] = 0.3;
            StandMap[(int)StandMapDate._1_0_0, (int)Judged.Low] = 0.1;
            StandMap[(int)StandMapDate._1_0_0, (int)Judged.Lower] = 0.0;
            StandMap[(int)StandMapDate._1_0_0, (int)Judged.Lowest] = 0.0;
            #endregion
            #region v1.14.118
            StandMap[(int)StandMapDate._1_14_118, (int)Judged.Highest] = 1.0;
            StandMap[(int)StandMapDate._1_14_118, (int)Judged.Higher] = 0.9;
            StandMap[(int)StandMapDate._1_14_118, (int)Judged.High] = 0.1;
            StandMap[(int)StandMapDate._1_14_118, (int)Judged.Low] = 0.01;
            StandMap[(int)StandMapDate._1_14_118, (int)Judged.Lower] = 0.0;
            StandMap[(int)StandMapDate._1_14_118, (int)Judged.Lowest] = 0.0;
            #endregion

            InputCounts[(int)InputMode._4] = 4;
            InputCounts[(int)InputMode._5] = 5;
            InputCounts[(int)InputMode._6] = 6;
            InputCounts[(int)InputMode._7] = 7;
            InputCounts[(int)InputMode._8] = 8;
            InputCounts[(int)InputMode._9] = 9;
            InputCounts[(int)InputMode._10] = 10;
            InputCounts[(int)InputMode._5_1] = 6;
            InputCounts[(int)InputMode._7_1] = 8;
            InputCounts[(int)InputMode._10_2] = 12;
            InputCounts[(int)InputMode._14_2] = 16;
            InputCounts[(int)InputMode._24_2] = 26;
            InputCounts[(int)InputMode._48_4] = 52;

            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._4] = [default, 1, 2, 3, 4];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._5_1] = [default, 1, 2, 3, 4, 5, 6];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 7];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16, 9];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._24_2] = [default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 2];
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode._48_4] = [default, 1, 2, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 3, 4];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._4] = [default, 1, 2, 3, 4];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._5_1] = [default, 1, 2, 3, 4, 5, 6];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._24_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26];
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode._48_4] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._4] = [default, 1, 2, 3, 4];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._5_1] = [default, 6, 1, 2, 3, 4, 5];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._7_1] = [default, 8, 1, 2, 3, 4, 5, 6, 7];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._10_2] = [default, 6, 1, 2, 3, 4, 5, 8, 9, 10, 11, 12, 7];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._14_2] = [default, 8, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 9];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._24_2] = [default, 26, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 1];
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode._48_4] = [default, 52, 51, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 2, 1];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._4] = [default, 1, 2, 3, 4];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._5_1] = [default, 6, 1, 2, 3, 4, 5];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._7_1] = [default, 8, 1, 2, 3, 4, 5, 6, 7];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._10_2] = [default, 6, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._14_2] = [default, 8, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._24_2] = [default, 26, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode._48_4] = [default, 52, 51, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 50, 49];

            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._4] = [default, 1, 2, 3, 4];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._5_1] = [default, 1, 2, 3, 4, 5, 6];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 12, 7, 8, 9, 10, 11];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 16, 9, 10, 11, 12, 13, 14, 15];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._24_2] = [default, 1, 26, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode._48_4] = [default, 1, 2, 51, 52, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._4] = [default, 1, 2, 3, 4];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._5_1] = [default, 1, 2, 3, 4, 5, 6];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._24_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26];
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode._48_4] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._4] = [default, 1, 2, 3, 4];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._5_1] = [default, 2, 3, 4, 5, 6, 1];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._7_1] = [default, 2, 3, 4, 5, 6, 7, 8, 1];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._10_2] = [default, 2, 3, 4, 5, 6, 1, 12, 7, 8, 9, 10, 11];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._14_2] = [default, 2, 3, 4, 5, 6, 7, 8, 1, 16, 9, 10, 11, 12, 13, 14, 15];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._24_2] = [default, 26, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 1];
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode._48_4] = [default, 52, 51, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 2, 1];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._4] = [default, 1, 2, 3, 4];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._5] = [default, 1, 2, 3, 4, 5];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._6] = [default, 1, 2, 3, 4, 5, 6];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._5_1] = [default, 2, 3, 4, 5, 6, 1];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._7_1] = [default, 2, 3, 4, 5, 6, 7, 8, 1];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._10_2] = [default, 2, 3, 4, 5, 6, 1, 7, 8, 9, 10, 11, 12];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._14_2] = [default, 2, 3, 4, 5, 6, 7, 8, 1, 9, 10, 11, 12, 13, 14, 15, 16];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._24_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 1];
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode._48_4] = [default, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 52, 51, 2, 1];

            AutoableInputs[(int)InputMode._4] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._5] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._6] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._7] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._8] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._9] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._10] = Array.Empty<int>();
            AutoableInputs[(int)InputMode._5_1] = [1];
            AutoableInputs[(int)InputMode._7_1] = [1];
            AutoableInputs[(int)InputMode._10_2] = [1, 12];
            AutoableInputs[(int)InputMode._14_2] = [1, 16];
            AutoableInputs[(int)InputMode._24_2] = [1, 26];
            AutoableInputs[(int)InputMode._48_4] = [1, 2, 51, 52];

            AutoableInputCounts[(int)InputMode._5_1] = 1;
            AutoableInputCounts[(int)InputMode._7_1] = 1;
            AutoableInputCounts[(int)InputMode._10_2] = 2;
            AutoableInputCounts[(int)InputMode._14_2] = 2;
            AutoableInputCounts[(int)InputMode._24_2] = 2;
            AutoableInputCounts[(int)InputMode._48_4] = 4;

            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._4, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6];

            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4, 0];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6, 7];

            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4, 0, 0];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4, 5, 0];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5, 6, 0];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._6, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6, 7, 8];

            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4, 0, 0, 0];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4, 5, 0, 0];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4, 5, 6, 0];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5, 6, 0, 0];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6, 7, 8, 9];

            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4, 5, 0, 0, 0];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4, 5, 6, 0, 0];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4, 5, 6, 7, 0];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5, 6, 0, 0, 0];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5, 6, 7, 8, 0];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9];
            FavorInputs[(int)InputMode._8, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6, 7, 8, 9, 10];

            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4, 5, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4, 5, 6, 0, 0, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4, 5, 6, 7, 0, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5, 6, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5, 6, 7, 8, 0, 0];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            FavorInputs[(int)InputMode._9, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6, 7, 8, 9, 10, 11];

            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._4] = [default, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._5] = [default, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._6] = [default, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._7] = [default, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._8] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._9] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._10] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._5_1] = [default, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._7_1] = [default, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._10_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._14_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._24_2] = [default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
            FavorInputs[(int)InputMode._10, (int)ModeComponent.InputFavorMode._48_4] = [default, 3, 4, 5, 6, 7, 9, 9, 10, 11, 12];

            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._4] = [default, 0, 1, 2, 3, 4, 0];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._5] = [default, 0, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._6] = [default, 0, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._7] = [default, 0, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._8] = [default, 0, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._9] = [default, 0, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._10] = [default, 0, 1, 2, 3, 4, 5];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._5_1] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._7_1] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._10_2] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._14_2] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._24_2] = [default, 1, 2, 3, 4, 5, 6];
            FavorInputs[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode._48_4] = [default, 1, 3, 4, 5, 6, 7];

            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._4] = [default, 0, 1, 2, 3, 4, 0, 0, 0];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._5] = [default, 0, 1, 2, 3, 4, 5, 0, 0];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._6] = [default, 0, 1, 2, 3, 4, 5, 6, 0];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._7] = [default, 0, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._8] = [default, 0, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._9] = [default, 0, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._10] = [default, 0, 1, 2, 3, 4, 5, 6, 7];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._5_1] = [default, 1, 2, 3, 4, 5, 6, 0, 0];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._24_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8];
            FavorInputs[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode._48_4] = [default, 1, 3, 4, 5, 6, 7, 8, 9];

            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._4] = [default, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._5] = [default, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._6] = [default, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._7] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._8] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._9] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._10] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._5_1] = [default, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 9, 10, 11, 12, 13, 16];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._24_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 26];
            FavorInputs[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode._48_4] = [default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 52];

            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._4] = [default, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._5] = [default, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._6] = [default, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._7] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._8] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._9] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._10] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._5_1] = [default, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._7_1] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._10_2] = [default, 1, 2, 3, 4, 5, 6, 0, 0, 7, 8, 9, 10, 11, 0, 0, 12];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._14_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._24_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 26];
            FavorInputs[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode._48_4] = [default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 52];

            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._4] = [default, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._5] = [default, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._6] = [default, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._7] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._8] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._9] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._10] = [default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._5_1] = [default, 1, 0, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._7_1] = [default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._10_2] = [default, 1, 12, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._14_2] = [default, 1, 16, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._24_2] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26];
            FavorInputs[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode._48_4] = [default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 52];

            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._4] = [default, 0, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._5] = [default, 0, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._6] = [default, 0, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._7] = [default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._8] = [default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._9] = [default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._10] = [default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._5_1] = [default, 1, 0, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._7_1] = [default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._10_2] = [default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._14_2] = [default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._24_2] = [default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 26];
            FavorInputs[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode._48_4] = [default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52];

            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 4.0 * (input - 1) + 1) + 3.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 4.0 * (input - 1) + 1) + 4.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 4.0 * (input - 1) + 1) + 5.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 4.0 * (input - 1) + 1) + 6.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 4.0 * (input - 1) + 1) + 7.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 4.0 * (input - 1) + 1) + 8.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 4.0 * (input - 1) + 1) + 9.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 4.0 * (input - 1) + 2) + 4.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 4.0 * (input - 1) + 2) + 6.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 4.0 * (input - 1) + 2) + 9.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 4.0 * (input - 1) + 2) + 13.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 4.0 * (input - 1) + 2) + 23.0 / 4.0 * random;
            FillLambdas[(int)InputMode._4, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 4.0 * (input - 1) + 3) + 47.0 / 4.0 * random;

            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 5.0 * (input - 1) + 1) + 3.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 5.0 * (input - 1) + 1) + 4.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 5.0 * (input - 1) + 1) + 5.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 5.0 * (input - 1) + 1) + 6.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 5.0 * (input - 1) + 1) + 7.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 5.0 * (input - 1) + 1) + 8.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 5.0 * (input - 1) + 1) + 9.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 5.0 * (input - 1) + 2) + 4.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 5.0 * (input - 1) + 2) + 6.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 5.0 * (input - 1) + 2) + 9.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 5.0 * (input - 1) + 2) + 13.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 5.0 * (input - 1) + 2) + 23.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 5.0 * (input - 1) + 3) + 47.0 / 5.0 * random;

            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 6.0 * (input - 1) + 1) + 3.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 6.0 * (input - 1) + 1) + 4.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 6.0 * (input - 1) + 1) + 5.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 6.0 * (input - 1) + 1) + 6.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 6.0 * (input - 1) + 1) + 7.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 6.0 * (input - 1) + 1) + 8.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 6.0 * (input - 1) + 1) + 9.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 6.0 * (input - 1) + 2) + 4.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 6.0 * (input - 1) + 2) + 6.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 6.0 * (input - 1) + 2) + 9.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 6.0 * (input - 1) + 2) + 13.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 6.0 * (input - 1) + 2) + 23.0 / 6.0 * random;
            FillLambdas[(int)InputMode._6, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 6.0 * (input - 1) + 3) + 47.0 / 6.0 * random;

            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 7.0 * (input - 1) + 1) + 3.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 7.0 * (input - 1) + 1) + 4.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 7.0 * (input - 1) + 1) + 5.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 7.0 * (input - 1) + 1) + 6.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 7.0 * (input - 1) + 1) + 7.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 7.0 * (input - 1) + 1) + 8.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 7.0 * (input - 1) + 1) + 9.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 7.0 * (input - 1) + 2) + 4.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 7.0 * (input - 1) + 2) + 6.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 7.0 * (input - 1) + 2) + 9.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 7.0 * (input - 1) + 2) + 13.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 7.0 * (input - 1) + 2) + 23.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 7.0 * (input - 1) + 3) + 47.0 / 7.0 * random;

            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 8.0 * (input - 1) + 1) + 3.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 8.0 * (input - 1) + 1) + 4.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 8.0 * (input - 1) + 1) + 5.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 8.0 * (input - 1) + 1) + 6.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 8.0 * (input - 1) + 1) + 7.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 8.0 * (input - 1) + 1) + 8.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 8.0 * (input - 1) + 1) + 9.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 8.0 * (input - 1) + 2) + 4.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 8.0 * (input - 1) + 2) + 6.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 8.0 * (input - 1) + 2) + 9.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 8.0 * (input - 1) + 2) + 13.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 8.0 * (input - 1) + 2) + 23.0 / 8.0 * random;
            FillLambdas[(int)InputMode._8, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 8.0 * (input - 1) + 3) + 47.0 / 8.0 * random;

            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 9.0 * (input - 1) + 1) + 3.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 9.0 * (input - 1) + 1) + 4.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 9.0 * (input - 1) + 1) + 5.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 9.0 * (input - 1) + 1) + 6.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 9.0 * (input - 1) + 1) + 7.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 9.0 * (input - 1) + 1) + 8.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 9.0 * (input - 1) + 1) + 9.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 9.0 * (input - 1) + 2) + 4.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 9.0 * (input - 1) + 2) + 6.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 9.0 * (input - 1) + 2) + 9.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 9.0 * (input - 1) + 2) + 13.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 9.0 * (input - 1) + 2) + 23.0 / 9.0 * random;
            FillLambdas[(int)InputMode._9, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 9.0 * (input - 1) + 3) + 47.0 / 9.0 * random;

            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 10.0 * (input - 1) + 1) + 3.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 10.0 * (input - 1) + 1) + 4.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 10.0 * (input - 1) + 1) + 5.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 10.0 * (input - 1) + 1) + 6.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 10.0 * (input - 1) + 1) + 7.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 10.0 * (input - 1) + 1) + 8.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 10.0 * (input - 1) + 1) + 9.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 10.0 * (input - 1) + 2) + 4.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 10.0 * (input - 1) + 2) + 6.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 10.0 * (input - 1) + 2) + 9.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 10.0 * (input - 1) + 2) + 13.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 10.0 * (input - 1) + 2) + 23.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 10.0 * (input - 1) + 3) + 47.0 / 10.0 * random;

            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 5.0 * (input - 2) + 1) + 3.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 5.0 * (input - 2) + 1) + 4.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 5.0 * (input - 2) + 1) + 5.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 5.0 * (input - 2) + 1) + 6.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 5.0 * (input - 2) + 1) + 7.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 5.0 * (input - 2) + 1) + 8.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 5.0 * (input - 2) + 1) + 9.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 5.0 * (input - 2) + 2) + 4.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 5.0 * (input - 2) + 2) + 6.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 5.0 * (input - 2) + 2) + 9.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 5.0 * (input - 2) + 2) + 13.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 5.0 * (input - 2) + 2) + 23.0 / 5.0 * random;
            FillLambdas[(int)InputMode._5_1, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 5.0 * (input - 2) + 3) + 47.0 / 5.0 * random;

            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 7.0 * (input - 2) + 1) + 3.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 7.0 * (input - 2) + 1) + 4.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 7.0 * (input - 2) + 1) + 5.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 7.0 * (input - 2) + 1) + 6.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 7.0 * (input - 2) + 1) + 7.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 7.0 * (input - 2) + 1) + 8.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 7.0 * (input - 2) + 1) + 9.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 7.0 * (input - 2) + 2) + 4.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 7.0 * (input - 2) + 2) + 6.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 7.0 * (input - 2) + 2) + 9.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 7.0 * (input - 2) + 2) + 13.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 7.0 * (input - 2) + 2) + 23.0 / 7.0 * random;
            FillLambdas[(int)InputMode._7_1, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 7.0 * (input - 2) + 3) + 47.0 / 7.0 * random;

            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 10.0 * (input - 2) + 1) + 3.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 10.0 * (input - 2) + 1) + 4.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 10.0 * (input - 2) + 1) + 5.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 10.0 * (input - 2) + 1) + 6.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 10.0 * (input - 2) + 1) + 7.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 10.0 * (input - 2) + 1) + 8.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 10.0 * (input - 2) + 1) + 9.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 10.0 * (input - 2) + 2) + 4.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 10.0 * (input - 2) + 2) + 6.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 10.0 * (input - 2) + 2) + 9.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 10.0 * (input - 2) + 2) + 13.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 10.0 * (input - 2) + 2) + 23.0 / 10.0 * random;
            FillLambdas[(int)InputMode._10_2, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 10.0 * (input - 2) + 3) + 47.0 / 10.0 * random;

            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 14.0 * (input - 2) + 1) + 3.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 14.0 * (input - 2) + 1) + 4.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 14.0 * (input - 2) + 1) + 5.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 14.0 * (input - 2) + 1) + 6.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 14.0 * (input - 2) + 1) + 7.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 14.0 * (input - 2) + 1) + 8.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 14.0 * (input - 2) + 1) + 9.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 14.0 * (input - 2) + 2) + 4.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 14.0 * (input - 2) + 2) + 6.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 14.0 * (input - 2) + 2) + 9.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 14.0 * (input - 2) + 2) + 13.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 14.0 * (input - 2) + 2) + 23.0 / 14.0 * random;
            FillLambdas[(int)InputMode._14_2, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 14.0 * (input - 2) + 3) + 47.0 / 14.0 * random;

            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 24.0 * (input - 2) + 1) + 3.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 24.0 * (input - 2) + 1) + 4.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 24.0 * (input - 2) + 1) + 5.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 24.0 * (input - 2) + 1) + 6.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 24.0 * (input - 2) + 1) + 7.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 24.0 * (input - 2) + 1) + 8.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 24.0 * (input - 2) + 1) + 9.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 24.0 * (input - 2) + 2) + 4.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 24.0 * (input - 2) + 2) + 6.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 24.0 * (input - 2) + 2) + 9.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 24.0 * (input - 2) + 2) + 13.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 24.0 * (input - 2) + 2) + 23.0 / 24.0 * random;
            FillLambdas[(int)InputMode._24_2, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 24.0 * (input - 2) + 3) + 47.0 / 24.0 * random;

            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill4] = (input, random) => (3.0 / 48.0 * (input - 3) + 1) + 3.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill5] = (input, random) => (4.0 / 48.0 * (input - 3) + 1) + 4.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill6] = (input, random) => (5.0 / 48.0 * (input - 3) + 1) + 5.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill7] = (input, random) => (6.0 / 48.0 * (input - 3) + 1) + 6.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill8] = (input, random) => (7.0 / 48.0 * (input - 3) + 1) + 7.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill9] = (input, random) => (8.0 / 48.0 * (input - 3) + 1) + 8.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill10] = (input, random) => (9.0 / 48.0 * (input - 3) + 1) + 9.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill5_1] = (input, random) => (4.0 / 48.0 * (input - 3) + 2) + 4.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill7_1] = (input, random) => (6.0 / 48.0 * (input - 3) + 2) + 6.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill10_2] = (input, random) => (9.0 / 48.0 * (input - 3) + 2) + 9.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill14_2] = (input, random) => (13.0 / 48.0 * (input - 3) + 2) + 13.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill24_2] = (input, random) => (23.0 / 48.0 * (input - 3) + 2) + 23.0 / 48.0 * random;
            FillLambdas[(int)InputMode._48_4, (int)ModeComponent.InputFavorMode.Fill48_4] = (input, random) => (47.0 / 48.0 * (input - 3) + 3) + 47.0 / 48.0 * random;

            VoidPutInputs[(int)InputMode._4, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._4, (int)InputMode._5] = [5];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._6] = [5, 6];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._7] = [5, 6, 7];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._8] = [5, 6, 7, 8];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._9] = [5, 6, 7, 8, 9];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._10] = [5, 6, 7, 8, 9, 10];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._5_1] = [6];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._7_1] = [6, 7, 8];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._10_2] = [6, 7, 8, 9, 10, 11];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._14_2] = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._24_2] = [6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._4, (int)InputMode._48_4] = [7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._5, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._5, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._5, (int)InputMode._6] = [6];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._7] = [6, 7];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._8] = [6, 7, 8];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._9] = [6, 7, 8, 9];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._10] = [6, 7, 8, 9, 10];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._5, (int)InputMode._7_1] = [7, 8];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._10_2] = [7, 8, 9, 10, 11];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._14_2] = [7, 8, 9, 10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._24_2] = [7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._5, (int)InputMode._48_4] = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._6, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._6, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._6, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._6, (int)InputMode._7] = [7];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._8] = [7, 8];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._9] = [7, 8, 9];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._10] = [7, 8, 9, 10];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._6, (int)InputMode._7_1] = [8];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._10_2] = [8, 9, 10, 11];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._14_2] = [8, 9, 10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._24_2] = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._6, (int)InputMode._48_4] = [9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._7, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7, (int)InputMode._8] = [8];
            VoidPutInputs[(int)InputMode._7, (int)InputMode._9] = [8, 9];
            VoidPutInputs[(int)InputMode._7, (int)InputMode._10] = [8, 9, 10];
            VoidPutInputs[(int)InputMode._7, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7, (int)InputMode._10_2] = [9, 10, 11];
            VoidPutInputs[(int)InputMode._7, (int)InputMode._14_2] = [9, 10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._7, (int)InputMode._24_2] = [9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._7, (int)InputMode._48_4] = [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._8, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._9] = [9];
            VoidPutInputs[(int)InputMode._8, (int)InputMode._10] = [9, 10];
            VoidPutInputs[(int)InputMode._8, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._8, (int)InputMode._10_2] = [10, 11];
            VoidPutInputs[(int)InputMode._8, (int)InputMode._14_2] = [10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._8, (int)InputMode._24_2] = [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._8, (int)InputMode._48_4] = [11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._9, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._10] = [10];
            VoidPutInputs[(int)InputMode._9, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._9, (int)InputMode._10_2] = [11];
            VoidPutInputs[(int)InputMode._9, (int)InputMode._14_2] = [11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._9, (int)InputMode._24_2] = [11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._9, (int)InputMode._48_4] = [12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._10, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10, (int)InputMode._10_2] = [12];
            VoidPutInputs[(int)InputMode._10, (int)InputMode._14_2] = [12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._10, (int)InputMode._24_2] = [12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._10, (int)InputMode._48_4] = [13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._6] = [6];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._7] = [6, 7];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._8] = [6, 7, 8];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._9] = [6, 7, 8, 9];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._10] = [6, 7, 8, 9, 10];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._7_1] = [7, 8];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._10_2] = [7, 8, 9, 10, 11, 12];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._14_2] = [7, 8, 9, 10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._24_2] = [7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._5_1, (int)InputMode._48_4] = [8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._8] = [8];
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._9] = [8, 9];
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._10] = [8, 9, 10];
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._10_2] = [9, 10, 11];
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._14_2] = [9, 10, 11, 12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._24_2] = [9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._7_1, (int)InputMode._48_4] = [10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._10_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._14_2] = [12, 13, 14, 15];
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._24_2] = [12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._10_2, (int)InputMode._48_4] = [13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._10_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._14_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._24_2] = [16, 17, 18, 19, 20, 21, 22, 23, 24, 25];
            VoidPutInputs[(int)InputMode._14_2, (int)InputMode._48_4] = [17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._10_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._14_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._24_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._24_2, (int)InputMode._48_4] = [27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50];

            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._5_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._7_1] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._10_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._14_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._24_2] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode._48_4, (int)InputMode._48_4] = Array.Empty<int>();

            IsIn2P[(int)InputMode._4] = new bool[5];
            IsIn2P[(int)InputMode._5] = new bool[6];
            IsIn2P[(int)InputMode._6] = new bool[7];
            IsIn2P[(int)InputMode._7] = new bool[8];
            IsIn2P[(int)InputMode._8] = new bool[9];
            IsIn2P[(int)InputMode._9] = new bool[10];
            IsIn2P[(int)InputMode._10] = new bool[11];
            IsIn2P[(int)InputMode._5_1] = new bool[7];
            IsIn2P[(int)InputMode._7_1] = new bool[9];
            IsIn2P[(int)InputMode._10_2] = [default, false, false, false, false, false, false, true, true, true, true, true, true];
            IsIn2P[(int)InputMode._14_2] = [default, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true];
            IsIn2P[(int)InputMode._24_2] = new bool[27];
            IsIn2P[(int)InputMode._48_4] = new bool[53];

            Input1PCounts[(int)InputMode._4] = 4;
            Input1PCounts[(int)InputMode._5] = 5;
            Input1PCounts[(int)InputMode._6] = 6;
            Input1PCounts[(int)InputMode._7] = 7;
            Input1PCounts[(int)InputMode._8] = 8;
            Input1PCounts[(int)InputMode._9] = 9;
            Input1PCounts[(int)InputMode._10] = 10;
            Input1PCounts[(int)InputMode._5_1] = 6;
            Input1PCounts[(int)InputMode._7_1] = 8;
            Input1PCounts[(int)InputMode._10_2] = 6;
            Input1PCounts[(int)InputMode._14_2] = 8;
            Input1PCounts[(int)InputMode._24_2] = 26;
            Input1PCounts[(int)InputMode._48_4] = 52;

            Has2P[(int)InputMode._10_2] = true;
            Has2P[(int)InputMode._14_2] = true;

            for (var i = (int)InputMapping.Mapping3; i >= (int)InputMapping.Mapping0; --i)
            {
                for (var j = 1; j >= 0; --j)
                {
                    LimiterCenterMap[i, (int)InputMode._4, j] = [default, 0, 0, 1, 0];
                    LimiterCenterMap[i, (int)InputMode._5, j] = [default, 0, 0, 2, 0, 0];
                    LimiterCenterMap[i, (int)InputMode._6, j] = [default, 0, 0, 0, 1, 0, 0];
                    LimiterCenterMap[i, (int)InputMode._7, j] = [default, 0, 0, 0, 2, 0, 0, 0];
                    LimiterCenterMap[i, (int)InputMode._8, j] = [default, 0, 0, 0, 0, 1, 0, 0, 0];
                    LimiterCenterMap[i, (int)InputMode._9, j] = [default, 0, 0, 0, 0, 2, 0, 0, 0, 0];
                    LimiterCenterMap[i, (int)InputMode._10, j] = [default, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0];
                }
            }
            for (var i = 1; i >= 0; --i)
            {
                for (var j = (int)InputMapping.Mapping3; j >= (int)InputMapping.Mapping0; --j)
                {
                    LimiterCenterMap[j, (int)InputMode._5_1, i] = [default, 0, 0, 0, 2, 0, 0];
                    LimiterCenterMap[j, (int)InputMode._7_1, i] = [default, 0, 0, 0, 0, 2, 0, 0, 0];
                }
                LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode._24_2, i] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode._48_4, i] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                for (var j = (int)InputMapping.Mapping2; j >= (int)InputMapping.Mapping1; --j)
                {
                    LimiterCenterMap[j, (int)InputMode._24_2, i] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                    LimiterCenterMap[j, (int)InputMode._48_4, i] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                }
                LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode._24_2, i] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
                LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode._48_4, i] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            }
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode._10_2, 0] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1];
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode._10_2, 1] = [default, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode._14_2, 0] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1];
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode._14_2, 1] = [default, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode._10_2, 0] = [default, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode._10_2, 1] = [default, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode._14_2, 0] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode._14_2, 1] = [default, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode._10_2, 0] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1];
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode._10_2, 1] = [default, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode._14_2, 0] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1];
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode._14_2, 1] = [default, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode._10_2, 0] = [default, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode._10_2, 1] = [default, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode._14_2, 0] = [default, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0];
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode._14_2, 1] = [default, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2, 20, 0, 0, 0];

            Limiter57Map[(int)InputMode._24_2] = [default, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, false];
            Limiter57Map[(int)InputMode._48_4] = [default, false, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, false, false];
        }

        public static double GetJudgmentMillis(Judged judged, ModeComponent modeComponentValue, double judgmentStage, JudgmentModeDate judgmentModeDate, JudgmentMapDate judgmentMapDate, LongNoteAssistDate longNoteAssistDate, int i, JudgmentAssist judgmentAssist = JudgmentAssist.Default)
        {
            var judgmentModeValue = (int)modeComponentValue.JudgmentModeValue;
            var judgmentMillis = 0.0;
            switch (judgmentModeDate)
            {
                case JudgmentModeDate._1_0_0:
                    judgmentMillis = JudgmentMap[(int)judgmentMapDate, judgmentModeValue, (int)judged, i];
                    if (judged != (int)Judged.Highest && judged != Judged.Lowest)
                    {
                        judgmentMillis -= Math.Sign(judgmentMillis) * JudgmentCalibrateMap[judgmentModeValue] * judgmentStage;
                    }
                    break;
                case JudgmentModeDate._1_6_7:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[(int)judgmentMapDate, judgmentModeValue, (int)judged, i] / (judgmentStage / 10 + 1);
                    }
                    break;
                case JudgmentModeDate._1_10_34:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[(int)judgmentMapDate, judgmentModeValue, (int)judged, i];
                        if (judged != Judged.Lowest)
                        {
                            judgmentMillis *= -judgmentStage / 20 + 1;
                        }
                    }
                    break;
                case JudgmentModeDate._1_10_35:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[(int)judgmentMapDate, judgmentModeValue, (int)judged, i];
                        if (judged != Judged.Lowest)
                        {
                            judgmentMillis *= 10 / (judgmentStage + 10);
                        }
                    }
                    break;
                case JudgmentModeDate._1_14_6:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[(int)judgmentMapDate, judgmentModeValue, (int)judged, i] * 10 / (judgmentStage + 10);
                    }
                    break;
            }
            switch (judgmentAssist)
            {
                case JudgmentAssist.LongNoteUp:
                    judgmentMillis *= LongNoteAssistMap[(int)longNoteAssistDate];
                    break;
            }
            return judgmentMillis;
        }

        public static bool GetIsJudgment(double loopingCounter, Judged judged, ModeComponent modeComponentValue, double judgmentStage, JudgmentModeDate judgmentModeDate, JudgmentMapDate judgmentMapDate, LongNoteAssistDate longNoteAssistDate, JudgmentAssist judgmentAssist) => GetJudgmentMillis(judged, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, 0, judgmentAssist) <= loopingCounter && loopingCounter <= GetJudgmentMillis(judged, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, 1, judgmentAssist);

        public static Judged GetJudged(double judgmentMeter, ModeComponent modeComponentValue, double judgmentStage, JudgmentModeDate judgmentModeDate, JudgmentMapDate judgmentMapDate, LongNoteAssistDate longNoteAssistDate, JudgmentAssist judgmentAssist = JudgmentAssist.Default)
        {
            if (GetIsJudgment(judgmentMeter, Judged.Highest, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, judgmentAssist))
            {
                return Judged.Highest;
            }
            else if (GetIsJudgment(judgmentMeter, Judged.Higher, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, judgmentAssist))
            {
                return Judged.Higher;
            }
            else if (GetIsJudgment(judgmentMeter, Judged.High, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, judgmentAssist))
            {
                return Judged.High;
            }
            else if (GetIsJudgment(judgmentMeter, Judged.Low, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, judgmentAssist))
            {
                return Judged.Low;
            }
            else if (GetIsJudgment(judgmentMeter, Judged.Lower, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, judgmentAssist))
            {
                return Judged.Lower;
            }
            else if (GetIsJudgment(judgmentMeter, Judged.Lowest, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, judgmentAssist))
            {
                return Judged.Lowest;
            }
            return Judged.Not;
        }

        public double MillisLoopUnit { get; }

        public double LevyingHeight { get; }

        public double LogicalYMillis { get; set; }

        public double LogicalYLoopUnit { get; set; }

        public double LogicalYMeter { get; set; }

        public double MillisMeter { get; set; }

        public Component(double bpm, int loopUnit)
        {
            MillisLoopUnit = 1000.0 / loopUnit;
            SetBPM(bpm);
            LevyingHeight = StandardHeight - LogicalYMillis * LevyingWait;
        }

        public Component(double bpm)
        {
            SetBPM(bpm);
            LevyingHeight = StandardHeight - LogicalYMillis * LevyingWait;
        }

        public void SetBPM(double bpm)
        {
            LogicalYMeter = 192.0 * Math.Sign(bpm);
            MillisMeter = 240000.0 / Math.Abs(bpm);
            LogicalYMillis = LogicalYMeter / MillisMeter;
            LogicalYLoopUnit = MillisLoopUnit * LogicalYMillis;
        }
    }
}