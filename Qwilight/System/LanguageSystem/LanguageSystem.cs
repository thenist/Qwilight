using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Qwilight
{
    public sealed partial class LanguageSystem : Model
    {
        public static readonly LanguageSystem Instance = QwilightComponent.GetBuiltInData<LanguageSystem>(nameof(LanguageSystem));

        public string[] AutoModeTexts { get; } = new string[2];
        public string[] NoteSaltModeTexts { get; } = new string[15];
        public string[] FaintNoteModeTexts { get; } = new string[4];
        public string[] JudgmentModeTexts { get; } = new string[6];
        public string[] HitPointsModeTexts { get; } = new string[8];
        public string[] NoteMobilityModeTexts { get; } = new string[6];
        public string[] LongNoteModeTexts { get; } = new string[4];
        public string[] InputFavorModeTexts { get; } = new string[30];
        public string[] NoteModifyModeTexts { get; } = new string[3];
        public string[] BPMModeTexts { get; } = new string[2];
        public string[] WaveModeTexts { get; } = new string[2];
        public string[] SetNoteModeTexts { get; } = new string[6];
        public string[] LowestJudgmentConditionModeTexts { get; } = new string[2];

        public string GetSiteName(string siteName) => siteName switch
        {
            "@Comment" => CommentSiteName,
            "@Default" => DefaultSiteName,
            "@Platform" => PlatformSiteName,
            _ => siteName
        };

        public void Init(string language)
        {
            var languageSystem = typeof(LanguageSystem);
            foreach (var property in languageSystem.GetProperties().Where(property => property.PropertyType == typeof(string)))
            {
                property.SetValue(this, default);
            }
            language = QwilightComponent.TestLanguage ?? language;
            var textHeight = 0;
            var lastLanguage = string.Empty;
            PropertyInfo lastPropertyInfo = null;
            var defaultValue = string.Empty;
            var r = new Utf8JsonReader(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Assets", "Language.json")));
            while (r.Read())
            {
                if (r.TokenType == JsonTokenType.StartObject)
                {
                    if (++textHeight == 2)
                    {
                        defaultValue = string.Empty;
                    }
                    continue;
                }

                if (r.TokenType == JsonTokenType.EndObject)
                {
                    if (--textHeight == 1 && lastPropertyInfo != null && string.IsNullOrEmpty(lastPropertyInfo.GetValue(this) as string))
                    {
                        lastPropertyInfo?.SetValue(this, defaultValue);
                    }
                    continue;
                }

                if (textHeight == 1)
                {
                    if (r.TokenType == JsonTokenType.PropertyName)
                    {
                        lastPropertyInfo = languageSystem.GetProperty(r.GetString());
                        continue;
                    }
                }

                if (textHeight == 2)
                {
                    switch (r.TokenType)
                    {
                        case JsonTokenType.PropertyName:
                            lastLanguage = r.GetString();
                            break;
                        case JsonTokenType.String:
                            if (lastLanguage == "en-US")
                            {
                                defaultValue = r.GetString();
                            }
                            if (lastLanguage == language)
                            {
                                lastPropertyInfo?.SetValue(this, r.GetString());
                            }
                            break;
                    }
                }
            }

            AutoModeTexts[(int)ModeComponent.AutoMode.Default] = DefaultModeContents;
            AutoModeTexts[(int)ModeComponent.AutoMode.Autoable] = AutoableModeContents;

            FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Default] = DefaultFaintNoteModeContents;
            FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Faint] = FaintNoteModeContents;
            FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.Fading] = FadingNoteModeContents;
            FaintNoteModeTexts[(int)ModeComponent.FaintNoteMode.TotalFading] = TotalFadingNoteModeContents;

            JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Lower] = LowerJudgmentModeContents;
            JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Default] = DefaultJudgmentModeContents;
            JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Higher] = HigherJudgmentModeContents;
            JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Lowest] = LowestJudgmentModeContents;
            JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Highest] = HighestJudgmentModeContents;
            JudgmentModeTexts[(int)ModeComponent.JudgmentMode.Favor] = FavorJudgmentModeContents;

            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Lower] = LowerHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Default] = DefaultHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Higher] = HigherHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Failed] = FailedHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Lowest] = LowestHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Highest] = HighestHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Favor] = FavorHitPointsModeContents;
            HitPointsModeTexts[(int)ModeComponent.HitPointsMode.Test] = TestHitPointsModeContents;

            NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.Default] = DefaultMobilityMode;
            NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode._4DHD] = _4DModeContents;
            NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.ZipHD] = ZipModeContents;
            NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode._4D] = Easy4DModeContents;
            NoteMobilityModeTexts[(int)ModeComponent.NoteMobilityMode.Zip] = EasyZipModeContents;

            LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Default] = DefaultLongNoteMode;
            LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Auto] = AutoLongNoteMode;
            LongNoteModeTexts[(int)ModeComponent.LongNoteMode.Input] = HigherLongNoteMode;

            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Default] = InputDefaultMode;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._4] = InputFavorMode4;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._5] = InputFavorMode5;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._6] = InputFavorMode6;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._7] = InputFavorMode7;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._8] = InputFavorMode8;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._9] = InputFavorMode9;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._10] = InputFavorMode10;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._5_1] = InputFavorMode51;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._7_1] = InputFavorMode71;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._10_2] = InputFavorMode102;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._14_2] = InputFavorMode142;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._24_2] = InputFavorMode242;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode._48_4] = InputFavorMode484;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled4] = InputFavorModeFill4;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled5] = InputFavorModeFill5;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled6] = InputFavorModeFill6;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled7] = InputFavorModeFill7;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled8] = InputFavorModeFill8;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled9] = InputFavorModeFill9;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled10] = InputFavorModeFill10;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled5_1] = InputFavorModeFill51;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled7_1] = InputFavorModeFill71;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled10_2] = InputFavorModeFill102;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled14_2] = InputFavorModeFill142;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled24_2] = InputFavorModeFill242;
            InputFavorModeTexts[(int)ModeComponent.InputFavorMode.Labelled48_4] = InputFavorModeFill484;

            NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Default] = DefaultSaltModeContents;
            NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Symmetric] = SymmetricModeContents;
            NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.Salt] = SaltModeContents;
            NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.InputSalt] = InputSaltModeContents;
            NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.MeterSalt] = MeterSaltModeContents;
            NoteSaltModeTexts[(int)ModeComponent.NoteSaltMode.HalfInputSalt] = HalfInputSaltModeContents;

            NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.Default] = DefaultNoteModifyContents;
            NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.InputNote] = InputNoteSetContents;
            NoteModifyModeTexts[(int)ModeComponent.NoteModifyMode.LongNote] = LongNoteModifyContents;

            BPMModeTexts[(int)ModeComponent.BPMMode.Default] = DefaultBPMModeContents;
            BPMModeTexts[(int)ModeComponent.BPMMode.Not] = NotBPMModeContents;

            WaveModeTexts[(int)ModeComponent.WaveMode.Default] = DefaultWaveModeContents;
            WaveModeTexts[(int)ModeComponent.WaveMode.Counter] = CounterWaveModeContents;

            SetNoteModeTexts[(int)ModeComponent.SetNoteMode.Default] = DefaultSetNoteModeContents;
            SetNoteModeTexts[(int)ModeComponent.SetNoteMode.Put] = PutSetNoteModeContents;
            SetNoteModeTexts[(int)ModeComponent.SetNoteMode.VoidPut] = VoidPutSetNoteModeContents;

            LowestJudgmentConditionModeTexts[(int)ModeComponent.LowestJudgmentConditionMode.Default] = DefaultLowestJudgmentConditionModeContents;
            LowestJudgmentConditionModeTexts[(int)ModeComponent.LowestJudgmentConditionMode.Wrong] = WrongLowestJudgmentConditionModeContents;
        }
    }
}
