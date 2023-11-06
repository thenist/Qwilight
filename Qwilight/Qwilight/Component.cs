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
            InputMode4 = 4, InputMode5, InputMode6, InputMode7, InputMode8, InputMode9, InputMode51, InputMode71, InputMode102, InputMode142, InputMode10, InputMode242, InputMode484
        }

        public enum Judged
        {
            Not = -1, Highest, Higher, High, Low, Lower, Lowest, Band1 = 9, Last = 10
        }

        public const double StandardHeight = 720.0;
        public const double StandardBPM = 130.0;
        public const int StandardMeter = 192;
        public const double LevyingWait = 3000.0;
        public const double QuitWait = 2000.0;
        public const double PassableWait = 1000.0;

        public const int HitPointsMode100 = 0;
        public const int HitPointsMode123 = 1;
        public const int HitPointsMode11034 = 2;
        public const int HitPointsMode11035 = 3;
        public const int HitPointsMode11462 = 4;
        public const int HitPointsMap100 = 0;
        public const int HitPointsMap167 = 1;
        public const int HitPointsMap170 = 2;
        public const int HitPointsMap1132 = 3;
        public const int JudgmentMap100 = 0;
        public const int JudgmentMap130 = 1;
        public const int JudgmentMap167 = 2;
        public const int JudgmentMap168 = 3;
        public const int JudgmentMap11034 = 4;
        public const int JudgmentMap11035 = 5;
        public const int JudgmentMap1110 = 6;
        public const int LatestJudgmentMap = JudgmentMap1110;
        public const int JudgmentMode100 = 0;
        public const int JudgmentMode167 = 1;
        public const int JudgmentMode11034 = 2;
        public const int JudgmentMode11035 = 3;
        public const int JudgmentMode1146 = 4;
        public const int LatestJudgmentMode = JudgmentMode1146;
        public const int LongNoteAssist100 = 0;
        public const int LongNoteAssist167 = 1;
        public const int LongNoteAssist11034 = 2;
        public const int LongNoteAssist11035 = 3;
        public const int LatestLongNoteAssist = LongNoteAssist11035;
        public const int Point100 = 0;
        public const int Point167 = 1;
        public const int StandMode100 = 0;
        public const int StandMode167 = 1;
        public const int StandMode114118 = 2;
        public const int StandMap100 = 0;
        public const int StandMap114118 = 1;
        public const int CommentWaitDate100 = 0;
        public const int CommentWaitDate1311 = 1;
        public const int CommentWaitDate164 = 2;
        public const int TooLongLongNote100 = 0;
        public const int TooLongLongNote113107 = 1;
        public const int TooLongLongNote11420 = 2;
        public const int TooLongLongNote11429 = 3;
        public const int TrapNoteJudgmentDate100 = 0;
        public const int TrapNoteJudgmentDate1146 = 1;
        public const int LongNoteMode100 = 0;
        public const int LongNoteMode11420 = 1;
        public const int LongNoteMode1164 = 2;
        public const int LatestLongNoteMode = LongNoteMode1164;
        public const int PaintEventsDate100 = 0;
        public const int PaintEventsDate11491 = 1;

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
        public static readonly int[,][] FavorInputs = new int[17, 20][];
        public static readonly int[,][] VoidPutInputs = new int[17, 17][];
        public static readonly bool[][] IsIn2P = new bool[17][];
        public static readonly int[] Input1PCounts = new int[17];
        public static readonly bool[] Has2P = new bool[17];
        public static readonly int[,,][] LimiterCenterMap = new int[4, 17, 2][];
        public static readonly bool[][] Limiter57Map = new bool[17][];

        static Component()
        {
            #region v1.0.0
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 0.9;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 0.3;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = 0.1 / 3;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Highest] = 1.5;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Higher] = 1.35;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.High] = 0.45;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Low] = 0.15;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = 0.025;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Highest] = 0.75;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Higher] = 0.675;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.High] = 0.225;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Low] = 0.075;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = 0.05;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Highest] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Higher] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.High] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Low] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap100, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = 1.0;
            #endregion
            #region v1.6.7
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 0.9;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 0.3;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = -0.027;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Highest] = 2.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Higher] = 1.8;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.High] = 0.6;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Low] = 0.2;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lowest] = -0.0135;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Highest] = 1.5;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Higher] = 1.35;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.High] = 0.45;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Low] = 0.15;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = -0.02025;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Highest] = 0.75;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Higher] = 0.675;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.High] = 0.225;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Low] = 0.075;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = -0.0405;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Highest] = 0.5;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Higher] = 0.45;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.High] = 0.15;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Low] = 0.05;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lowest] = -0.054;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Highest] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Higher] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.High] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Low] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap167, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = -1.0;
            #endregion
            #region v1.7.0
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 0.2 / 3;
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 0.1 / 3;
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = 0.1 / 3;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Lowest, i] = 2.0 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Lower, i] = 1.5 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Higher, i] = 0.75 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Highest, i] = 0.5 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Failed, i] = 0.0;
            }
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lowest] = 0.5 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = 0.75 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = 1.5 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lowest] = 2.0 * HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap170, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = 1.0;
            #endregion
            #region v1.13.2
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest] = 1.0;
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher] = 2.0 / 3;
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High] = 1.0 / 3;
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low] = 0.1;
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower] = 0.0;
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest] = 0.1 / 3;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Lowest, i] = 2.0 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Lower, i] = 1.5 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Higher, i] = 0.75 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Highest, i] = 0.5 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, i];
                HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Failed, i] = 0.0;
            }
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Lowest, (int)Judged.Lowest] = 0.5 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest] = 0.75 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Higher, (int)Judged.Lowest] = 1.5 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Highest, (int)Judged.Lowest] = 2.0 * HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Failed, (int)Judged.Lowest] = 1.0;
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Lowest] = HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Lower, (int)Judged.Lowest];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Lower] = HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Lower];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Low] = HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Low];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Test, (int)Judged.High] = HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.High];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Higher] = HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Higher];
            HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Test, (int)Judged.Highest] = HitPointsMap[HitPointsMap1132, (int)ModeComponent.HitPointsMode.Default, (int)Judged.Highest];
            #endregion

            #region v1.0.0
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -16.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -64.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -97.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -127.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -151.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -211.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 0] = -22.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 0] = -89.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 0] = -135.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 0] = -177.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 0] = -211.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = -211.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 0] = -11.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 0] = -45.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 0] = -69.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 0] = -90.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 0] = -107.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = -211.5;

            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 1] = 16.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 1] = 64.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 1] = 97.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 1] = 127.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 1] = 151.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 1] = 211.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 1] = 22.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 1] = 89.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 1] = 135.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 1] = 177.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 1] = 211.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 1] = 211.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 1] = 11.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 1] = 45.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 1] = 69.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 1] = 90.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 1] = 107.5;
            JudgmentMap[JudgmentMap100, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 1] = 211.5;
            #endregion
            #region v1.3.0
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -16.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -64.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -97.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -127.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -151.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -169.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 0] = -22.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 0] = -89.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 0] = -135.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 0] = -177.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 0] = -211.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = -236.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 0] = -11.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 0] = -45.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 0] = -69.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 0] = -90.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 0] = -107.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = -121.5;

            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 1] = 16.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 1] = 64.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 1] = 97.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 1] = 127.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 1] = 151.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 1] = 169.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Highest, 1] = 22.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Higher, 1] = 89.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.High, 1] = 135.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Low, 1] = 177.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lower, 1] = 211.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 1] = 236.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Highest, 1] = 11.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Higher, 1] = 45.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.High, 1] = 69.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Low, 1] = 90.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lower, 1] = 107.5;
            JudgmentMap[JudgmentMap130, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 1] = 121.5;
            #endregion
            #region v1.6.7
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -15.0;
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -60.0;
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -90.0;
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -125.0;
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -210.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = 0.75 * JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = 0.5 * JudgmentMap[JudgmentMap167, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[JudgmentMap167, i, j, 1] = -JudgmentMap[JudgmentMap167, i, j, 0];
                }
            }
            #endregion
            #region v1.6.8
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -25.0;
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -65.0;
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -100.0;
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -130.0;
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -205.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = 0.75 * JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = 0.5 * JudgmentMap[JudgmentMap168, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[JudgmentMap168, i, j, 1] = -JudgmentMap[JudgmentMap168, i, j, 0];
                }
            }
            #endregion
            #region v1.10.34
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -20.0;
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -45.0;
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -75.0;
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -110.0;
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -150.0;
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -195.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11034, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[JudgmentMap11034, i, j, 1] = -JudgmentMap[JudgmentMap11034, i, j, 0];
                }
            }
            #endregion
            #region v1.10.35
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -25.0;
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -65.0;
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -100.0;
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -130.0;
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -205.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap11035, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[JudgmentMap11035, i, j, 1] = -JudgmentMap[JudgmentMap11035, i, j, 0];
                }
            }
            #endregion
            #region v1.11.0
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Highest, 0] = -25.0;
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Higher, 0] = -60.0;
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.High, 0] = -95.0;
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Low, 0] = -130.0;
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lower, 0] = -165.0;
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0] = -200.0;
            for (var i = (int)Judged.Lower; i >= (int)Judged.Highest; --i)
            {
                JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Lowest, i, 0] = 2.0 * JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Lower, i, 0] = 1.5 * JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Higher, i, 0] = 0.75 * JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, i, 0];
                JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Highest, i, 0] = 0.5 * JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, i, 0];
            }
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Lowest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Lower, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Higher, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Highest, (int)Judged.Lowest, 0] = JudgmentMap[JudgmentMap1110, (int)ModeComponent.JudgmentMode.Default, (int)Judged.Lowest, 0];
            for (var i = JudgmentMap.GetLength(1) - 1; i >= 0; --i)
            {
                for (var j = JudgmentMap.GetLength(2) - 1; j >= 0; --j)
                {
                    JudgmentMap[JudgmentMap1110, i, j, 1] = -JudgmentMap[JudgmentMap1110, i, j, 0];
                }
            }
            #endregion

            JudgmentCalibrateMap[(int)ModeComponent.JudgmentMode.Lower] = 4.2;
            JudgmentCalibrateMap[(int)ModeComponent.JudgmentMode.Default] = 3.0;
            JudgmentCalibrateMap[(int)ModeComponent.JudgmentMode.Higher] = 2.1;

            #region v1.0.0
            PointMap[Point100, (int)Judged.Highest] = 1.0;
            PointMap[Point100, (int)Judged.Higher] = 1.0;
            PointMap[Point100, (int)Judged.High] = 2.0 / 3;
            PointMap[Point100, (int)Judged.Low] = 1.0 / 3;
            PointMap[Point100, (int)Judged.Lower] = 1.0 / 6;
            PointMap[Point100, (int)Judged.Lowest] = 0.0;
            #endregion
            #region v1.6.7
            PointMap[Point167, (int)Judged.Highest] = 1.0;
            PointMap[Point167, (int)Judged.Higher] = 1.0;
            PointMap[Point167, (int)Judged.High] = 0.7;
            PointMap[Point167, (int)Judged.Low] = 0.5;
            PointMap[Point167, (int)Judged.Lower] = 0.3;
            PointMap[Point167, (int)Judged.Lowest] = 0.0;
            #endregion

            LongNoteAssistMap[LongNoteAssist100] = 1.0;
            LongNoteAssistMap[LongNoteAssist167] = 1.5;
            LongNoteAssistMap[LongNoteAssist11034] = 2.0;
            LongNoteAssistMap[LongNoteAssist11035] = 1.5;

            #region v1.0.0
            StandMap[StandMap100, (int)Judged.Highest] = 1.0;
            StandMap[StandMap100, (int)Judged.Higher] = 0.9;
            StandMap[StandMap100, (int)Judged.High] = 0.3;
            StandMap[StandMap100, (int)Judged.Low] = 0.1;
            StandMap[StandMap100, (int)Judged.Lower] = 0.0;
            StandMap[StandMap100, (int)Judged.Lowest] = 0.0;
            #endregion
            #region v1.14.118
            StandMap[StandMap114118, (int)Judged.Highest] = 1.0;
            StandMap[StandMap114118, (int)Judged.Higher] = 0.9;
            StandMap[StandMap114118, (int)Judged.High] = 0.1;
            StandMap[StandMap114118, (int)Judged.Low] = 0.01;
            StandMap[StandMap114118, (int)Judged.Lower] = 0.0;
            StandMap[StandMap114118, (int)Judged.Lowest] = 0.0;
            #endregion

            InputCounts[(int)InputMode.InputMode4] = 4;
            InputCounts[(int)InputMode.InputMode5] = 5;
            InputCounts[(int)InputMode.InputMode6] = 6;
            InputCounts[(int)InputMode.InputMode7] = 7;
            InputCounts[(int)InputMode.InputMode8] = 8;
            InputCounts[(int)InputMode.InputMode9] = 9;
            InputCounts[(int)InputMode.InputMode10] = 10;
            InputCounts[(int)InputMode.InputMode51] = 6;
            InputCounts[(int)InputMode.InputMode71] = 8;
            InputCounts[(int)InputMode.InputMode102] = 12;
            InputCounts[(int)InputMode.InputMode142] = 16;
            InputCounts[(int)InputMode.InputMode242] = 26;
            InputCounts[(int)InputMode.InputMode484] = 52;

            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode51] = new[] { default, 1, 2, 3, 4, 5, 6 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode102] = new[] { default, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 7 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode142] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16, 9 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode242] = new[] { default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 2 };
            InputMappingValues[(int)InputMapping.Mapping0, (int)InputMode.InputMode484] = new[] { default, 1, 2, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 3, 4 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode51] = new[] { default, 1, 2, 3, 4, 5, 6 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode102] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode142] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode242] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
            InputMappingValues[(int)InputMapping.Mapping1, (int)InputMode.InputMode484] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode51] = new[] { default, 6, 1, 2, 3, 4, 5 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode71] = new[] { default, 8, 1, 2, 3, 4, 5, 6, 7 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode102] = new[] { default, 6, 1, 2, 3, 4, 5, 8, 9, 10, 11, 12, 7 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode142] = new[] { default, 8, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 9 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode242] = new[] { default, 26, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 1 };
            InputMappingValues[(int)InputMapping.Mapping2, (int)InputMode.InputMode484] = new[] { default, 52, 51, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 2, 1 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode51] = new[] { default, 6, 1, 2, 3, 4, 5 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode71] = new[] { default, 8, 1, 2, 3, 4, 5, 6, 7 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode102] = new[] { default, 6, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode142] = new[] { default, 8, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 16 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode242] = new[] { default, 26, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            InputMappingValues[(int)InputMapping.Mapping3, (int)InputMode.InputMode484] = new[] { default, 52, 51, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 50, 49 };

            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode51] = new[] { default, 1, 2, 3, 4, 5, 6 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode102] = new[] { default, 1, 2, 3, 4, 5, 6, 12, 7, 8, 9, 10, 11 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode142] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 16, 9, 10, 11, 12, 13, 14, 15 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode242] = new[] { default, 1, 26, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            BasePaintMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode484] = new[] { default, 1, 2, 51, 52, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode51] = new[] { default, 1, 2, 3, 4, 5, 6 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode102] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode142] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode242] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
            BasePaintMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode484] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode51] = new[] { default, 2, 3, 4, 5, 6, 1 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode71] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 1 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode102] = new[] { default, 2, 3, 4, 5, 6, 1, 12, 7, 8, 9, 10, 11 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode142] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 1, 16, 9, 10, 11, 12, 13, 14, 15 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode242] = new[] { default, 26, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 1 };
            BasePaintMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode484] = new[] { default, 52, 51, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 2, 1 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode4] = new[] { default, 1, 2, 3, 4 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode5] = new[] { default, 1, 2, 3, 4, 5 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode51] = new[] { default, 2, 3, 4, 5, 6, 1 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode71] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 1 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode102] = new[] { default, 2, 3, 4, 5, 6, 1, 7, 8, 9, 10, 11, 12 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode142] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 1, 9, 10, 11, 12, 13, 14, 15, 16 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode242] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 1 };
            BasePaintMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode484] = new[] { default, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 52, 51, 2, 1 };

            AutoableInputs[(int)InputMode.InputMode4] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode5] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode6] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode7] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode8] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode9] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode10] = Array.Empty<int>();
            AutoableInputs[(int)InputMode.InputMode51] = new[] { 1 };
            AutoableInputs[(int)InputMode.InputMode71] = new[] { 1 };
            AutoableInputs[(int)InputMode.InputMode102] = new[] { 1, 12 };
            AutoableInputs[(int)InputMode.InputMode142] = new[] { 1, 16 };
            AutoableInputs[(int)InputMode.InputMode242] = new[] { 1, 26 };
            AutoableInputs[(int)InputMode.InputMode484] = new[] { 1, 2, 51, 52 };

            AutoableInputCounts[(int)InputMode.InputMode51] = 1;
            AutoableInputCounts[(int)InputMode.InputMode71] = 1;
            AutoableInputCounts[(int)InputMode.InputMode102] = 2;
            AutoableInputCounts[(int)InputMode.InputMode142] = 2;
            AutoableInputCounts[(int)InputMode.InputMode242] = 2;
            AutoableInputCounts[(int)InputMode.InputMode484] = 4;

            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6 };

            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4, 0 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6, 7 };

            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4, 0, 0 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4, 5, 0 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5, 6, 0 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6, 7, 8 };

            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4, 5, 0, 0 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4, 5, 6, 0 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5, 6, 0, 0 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6, 7, 8, 9 };

            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4, 5, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 0 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5, 6, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 0 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9 };
            FavorInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6, 7, 8, 9, 10 };

            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4, 5, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 0, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5, 6, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 0, 0 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            FavorInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            FavorInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 3, 4, 5, 6, 7, 9, 9, 10, 11, 12 };

            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 0, 1, 2, 3, 4, 0 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 0, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 0, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 0, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 0, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 0, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 0, 1, 2, 3, 4, 5 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 1, 2, 3, 4, 5, 6 };
            FavorInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 1, 3, 4, 5, 6, 7 };

            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 0, 1, 2, 3, 4, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 0, 1, 2, 3, 4, 5, 0, 0 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 0 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8 };
            FavorInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 1, 3, 4, 5, 6, 7, 8, 9 };

            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 1, 2, 3, 4, 5, 6, 9, 10, 11, 12, 13, 16 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 26 };
            FavorInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 52 };

            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 1, 2, 3, 4, 5, 6, 0, 0, 7, 8, 9, 10, 11, 0, 0, 12 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 26 };
            FavorInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 52 };

            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 1, 12, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 1, 16, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
            FavorInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 1, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 52 };

            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode4] = new[] { default, 0, 0, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode5] = new[] { default, 0, 0, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode6] = new[] { default, 0, 0, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode7] = new[] { default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode8] = new[] { default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode9] = new[] { default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode10] = new[] { default, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode51] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode71] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode102] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode142] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode242] = new[] { default, 1, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 26 };
            FavorInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode484] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52 };

            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode5] = new[] { 5 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode6] = new[] { 5, 6 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode7] = new[] { 5, 6, 7 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode8] = new[] { 5, 6, 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 5, 6, 7, 8, 9 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 5, 6, 7, 8, 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode51] = new[] { 6 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode71] = new[] { 6, 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 6, 7, 8, 9, 10, 11 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode4, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode6] = new[] { 6 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode7] = new[] { 6, 7 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode8] = new[] { 6, 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 6, 7, 8, 9 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 6, 7, 8, 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode71] = new[] { 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 7, 8, 9, 10, 11 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode5, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode7] = new[] { 7 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode8] = new[] { 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 7, 8, 9 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 7, 8, 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode71] = new[] { 8 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 8, 9, 10, 11 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 8, 9, 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode6, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode8] = new[] { 8 };
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 8, 9 };
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 8, 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 9, 10, 11 };
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 9, 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode7, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 9 };
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 10, 11 };
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode8, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 10 };
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 11 };
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode9, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 12 };
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode10, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode6] = new[] { 6 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode7] = new[] { 6, 7 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode8] = new[] { 6, 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 6, 7, 8, 9 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 6, 7, 8, 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode71] = new[] { 7, 8 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 7, 8, 9, 10, 11, 12 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode51, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode8] = new[] { 8 };
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode9] = new[] { 8, 9 };
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode10] = new[] { 8, 9, 10 };
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode102] = new[] { 9, 10, 11 };
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 9, 10, 11, 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode71, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode102] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode142] = new[] { 12, 13, 14, 15 };
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode102, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode102] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode142] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode242] = new[] { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            VoidPutInputs[(int)InputMode.InputMode142, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode102] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode142] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode242] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode242, (int)ModeComponent.InputFavorMode.Mode484] = new[] { 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50 };

            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode4] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode5] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode6] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode7] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode8] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode9] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode10] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode51] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode71] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode102] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode142] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode242] = Array.Empty<int>();
            VoidPutInputs[(int)InputMode.InputMode484, (int)ModeComponent.InputFavorMode.Mode484] = Array.Empty<int>();

            IsIn2P[(int)InputMode.InputMode4] = new bool[5];
            IsIn2P[(int)InputMode.InputMode5] = new bool[6];
            IsIn2P[(int)InputMode.InputMode6] = new bool[7];
            IsIn2P[(int)InputMode.InputMode7] = new bool[8];
            IsIn2P[(int)InputMode.InputMode8] = new bool[9];
            IsIn2P[(int)InputMode.InputMode9] = new bool[10];
            IsIn2P[(int)InputMode.InputMode10] = new bool[11];
            IsIn2P[(int)InputMode.InputMode51] = new bool[7];
            IsIn2P[(int)InputMode.InputMode71] = new bool[9];
            IsIn2P[(int)InputMode.InputMode102] = new[] { default, false, false, false, false, false, false, true, true, true, true, true, true };
            IsIn2P[(int)InputMode.InputMode142] = new[] { default, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true };
            IsIn2P[(int)InputMode.InputMode242] = new bool[27];
            IsIn2P[(int)InputMode.InputMode484] = new bool[53];

            Input1PCounts[(int)InputMode.InputMode4] = 4;
            Input1PCounts[(int)InputMode.InputMode5] = 5;
            Input1PCounts[(int)InputMode.InputMode6] = 6;
            Input1PCounts[(int)InputMode.InputMode7] = 7;
            Input1PCounts[(int)InputMode.InputMode8] = 8;
            Input1PCounts[(int)InputMode.InputMode9] = 9;
            Input1PCounts[(int)InputMode.InputMode10] = 10;
            Input1PCounts[(int)InputMode.InputMode51] = 6;
            Input1PCounts[(int)InputMode.InputMode71] = 8;
            Input1PCounts[(int)InputMode.InputMode102] = 6;
            Input1PCounts[(int)InputMode.InputMode142] = 8;
            Input1PCounts[(int)InputMode.InputMode242] = 26;
            Input1PCounts[(int)InputMode.InputMode484] = 52;

            Has2P[(int)InputMode.InputMode102] = true;
            Has2P[(int)InputMode.InputMode142] = true;

            for (var i = (int)InputMapping.Mapping3; i >= (int)InputMapping.Mapping0; --i)
            {
                for (var j = 1; j >= 0; --j)
                {
                    LimiterCenterMap[i, (int)InputMode.InputMode4, j] = new[] { default, 0, 0, 1, 0 };
                    LimiterCenterMap[i, (int)InputMode.InputMode5, j] = new[] { default, 0, 0, 2, 0, 0 };
                    LimiterCenterMap[i, (int)InputMode.InputMode6, j] = new[] { default, 0, 0, 0, 1, 0, 0 };
                    LimiterCenterMap[i, (int)InputMode.InputMode7, j] = new[] { default, 0, 0, 0, 2, 0, 0, 0 };
                    LimiterCenterMap[i, (int)InputMode.InputMode8, j] = new[] { default, 0, 0, 0, 0, 1, 0, 0, 0 };
                    LimiterCenterMap[i, (int)InputMode.InputMode9, j] = new[] { default, 0, 0, 0, 0, 2, 0, 0, 0, 0 };
                    LimiterCenterMap[i, (int)InputMode.InputMode10, j] = new[] { default, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
                }
            }
            for (var i = 1; i >= 0; --i)
            {
                for (var j = (int)InputMapping.Mapping3; j >= (int)InputMapping.Mapping0; --j)
                {
                    LimiterCenterMap[j, (int)InputMode.InputMode51, i] = new[] { default, 0, 0, 0, 2, 0, 0 };
                    LimiterCenterMap[j, (int)InputMode.InputMode71, i] = new[] { default, 0, 0, 0, 0, 2, 0, 0, 0 };
                }
                LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode242, i] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode484, i] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (var j = (int)InputMapping.Mapping2; j >= (int)InputMapping.Mapping1; --j)
                {
                    LimiterCenterMap[j, (int)InputMode.InputMode242, i] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    LimiterCenterMap[j, (int)InputMode.InputMode484, i] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                }
                LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode242, i] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode484, i] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode102, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode102, 1] = new[] { default, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode142, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            LimiterCenterMap[(int)InputMapping.Mapping0, (int)InputMode.InputMode142, 1] = new[] { default, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode102, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode102, 1] = new[] { default, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode142, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping1, (int)InputMode.InputMode142, 1] = new[] { default, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode102, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode102, 1] = new[] { default, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode142, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            LimiterCenterMap[(int)InputMapping.Mapping2, (int)InputMode.InputMode142, 1] = new[] { default, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode102, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode102, 1] = new[] { default, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode142, 0] = new[] { default, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            LimiterCenterMap[(int)InputMapping.Mapping3, (int)InputMode.InputMode142, 1] = new[] { default, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2, 20, 0, 0, 0 };

            Limiter57Map[(int)InputMode.InputMode242] = new[] { default, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, false };
            Limiter57Map[(int)InputMode.InputMode484] = new[] { default, false, false, false, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, true, false, false, false, false, true, false, false, false, false, false, false, false, false };
        }

        public static double GetJudgmentMillis(Judged judged, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, int i, JudgmentAssist judgmentAssist = JudgmentAssist.Default)
        {
            var judgmentModeValue = (int)modeComponentValue.JudgmentModeValue;
            var judgmentMillis = 0.0;
            switch (judgmentModeDate)
            {
                case JudgmentMode100:
                    judgmentMillis = JudgmentMap[judgmentMapDate, judgmentModeValue, (int)judged, i];
                    if (judged != (int)Judged.Highest && judged != Judged.Lowest)
                    {
                        judgmentMillis -= Math.Sign(judgmentMillis) * JudgmentCalibrateMap[judgmentModeValue] * judgmentStage;
                    }
                    break;
                case JudgmentMode167:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[judgmentMapDate, judgmentModeValue, (int)judged, i] / (judgmentStage / 10 + 1);
                    }
                    break;
                case JudgmentMode11034:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[judgmentMapDate, judgmentModeValue, (int)judged, i];
                        if (judged != Judged.Lowest)
                        {
                            judgmentMillis *= -judgmentStage / 20 + 1;
                        }
                    }
                    break;
                case JudgmentMode11035:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[judgmentMapDate, judgmentModeValue, (int)judged, i];
                        if (judged != Judged.Lowest)
                        {
                            judgmentMillis *= 10 / (judgmentStage + 10);
                        }
                    }
                    break;
                case JudgmentMode1146:
                    if (modeComponentValue.JudgmentModeValue == ModeComponent.JudgmentMode.Favor)
                    {
                        judgmentMillis = modeComponentValue.FavorJudgments[(int)judged][i];
                    }
                    else
                    {
                        judgmentMillis = JudgmentMap[judgmentMapDate, judgmentModeValue, (int)judged, i] * 10 / (judgmentStage + 10);
                    }
                    break;
            }
            switch (judgmentAssist)
            {
                case JudgmentAssist.LongNoteUp:
                    judgmentMillis *= LongNoteAssistMap[longNoteAssistDate];
                    break;
            }
            return judgmentMillis;
        }

        public static bool GetIsJudgment(double loopingCounter, Judged judged, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, JudgmentAssist judgmentAssist) => GetJudgmentMillis(judged, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, 0, judgmentAssist) <= loopingCounter && loopingCounter <= GetJudgmentMillis(judged, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, 1, judgmentAssist);

        public static Judged GetJudged(double judgmentMeter, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, JudgmentAssist judgmentAssist = JudgmentAssist.Default)
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