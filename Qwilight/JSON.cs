using Qwilight.NoteFile;
using Qwilight.Utilities;

namespace Qwilight
{
    public static class JSON
    {
        public struct AssetClient
        {
            public uint valve;
            public string platform;
        }

        public struct BMSTable
        {
            public string name = string.Empty;
            public string symbol = string.Empty;
            public string data_url = string.Empty;
            public object[] level_order = Array.Empty<object>();

            public BMSTable()
            {
            }
        }

        public struct BMSTableData
        {
            public string title = string.Empty;
            public string artist = string.Empty;
            public string url = string.Empty;
            public string url_diff = string.Empty;
            public string md5 = string.Empty;
            public object level;
            public string sha256 = string.Empty;
            public string comment = string.Empty;

            public BMSTableData()
            {
            }
        }

        public struct BMSON
        {
            public BGA bga = new BGA();
            public BPMEvent[] bpm_events = Array.Empty<BPMEvent>();
            public BMSONInfo info = new BMSONInfo();
            public Meter[] lines = Array.Empty<Meter>();
            public AudioChannel[] mine_channels = Array.Empty<AudioChannel>();
            public AudioChannel[] sound_channels = Array.Empty<AudioChannel>();
            public StopEvent[] stop_events = Array.Empty<StopEvent>();

            public BMSON()
            {
            }

            public struct BGA
            {
                public BGAEvent[] bga_events = Array.Empty<BGAEvent>();
                public BGAID[] bga_header = Array.Empty<BGAID>();
                public BGAEvent[] layer_events = Array.Empty<BGAEvent>();
                public BGAEvent[] poor_events = Array.Empty<BGAEvent>();

                public BGA()
                {
                }

                public struct BGAEvent
                {
                    public object id;
                    public long y;
                }

                public struct BGAID
                {
                    public object id;
                    public string name = string.Empty;

                    public BGAID()
                    {
                    }
                }
            }

            public struct BPMEvent
            {
                public double bpm;
                public long y;
            }

            public struct BMSONInfo
            {
                public string artist = string.Empty;
                public string back_image = string.Empty;
                public string banner_image = string.Empty;
                public string chart_name = string.Empty;
                public string eyecatch_image = string.Empty;
                public string genre = string.Empty;
                public double init_bpm = 130.0;
                public double judge_rank = 100.0;
                public long level;
                public long ln_type = 1L;
                public string mode_hint = "beat-7k";
                public long resolution = 240L;
                public string[] subartists = Array.Empty<string>();
                public string subtitle = string.Empty;
                public string title = string.Empty;
                public string title_image = string.Empty;
                public double total = 100.0;

                public BMSONInfo()
                {
                }
            }

            public struct Meter
            {
                public long y;
            }

            public struct AudioChannel
            {
                public string name = string.Empty;
                public Note[] notes = Array.Empty<Note>();

                public AudioChannel()
                {
                }

                public struct Note
                {
                    public bool c;
                    public double damage;
                    public long l;
                    public long x;
                    public long y;
                }
            }

            public struct StopEvent
            {
                public long duration;
                public long y;
            }
        }

        public struct TaehuiQwilight
        {
            public string date;
            public string hash;
            public string title;
        }

        public struct TwilightWwwComment
        {
            public bool? favor;
            public int totalFavor;
            public Comment[] comments;

            public struct Comment
            {
                public long? date;
                public string avatarID;
                public string avatarName;
                public double multiplier;
                public ModeComponent.AutoMode autoMode;
                public ModeComponent.NoteSaltMode noteSaltMode;
                public double audioMultiplier;
                public ModeComponent.FaintNoteMode faintNoteMode;
                public ModeComponent.JudgmentMode judgmentMode;
                public ModeComponent.HitPointsMode hitPointsMode;
                public ModeComponent.NoteMobilityMode noteMobilityMode;
                public ModeComponent.LongNoteMode longNoteMode;
                public ModeComponent.InputFavorMode inputFavorMode;
                public ModeComponent.NoteModifyMode noteModifyMode;
                public ModeComponent.LowestJudgmentConditionMode lowestJudgmentConditionMode;
                public int stand;
                public int band;
                public bool isP;
                public double point;
                public int salt;
                public string commentID;
                public string commentary;
                public bool isPaused;
                public int inputFlags;
            }
        }

        public struct TwilightEstablish
        {
            public string avatarID;

            public string avatarName;
        }

        public struct TwilightSignIn
        {
            public string totem;
            public string avatarID;
            public string avatarName;
        }

        public struct TwilightNotSignIn
        {
            public string avatarID;
            public string avatarName;
        }

        public struct TwilightWwwDefaultDate
        {
            public long date;
        }

        public struct TwilightWwwSite
        {
            public string siteID;
            public string siteName;
            public int siteConfigure;
            public bool hasCipher;
            public int avatarCount;
        }

        public class TwilightLevelUp
        {
            public int from;
            public int to;
        }

        public class TwilightAbilityUp
        {
            public Component.InputMode inputMode;
            public double ability;

            public override string ToString()
            {
                var inputModeText = inputMode switch
                {
                    Component.InputMode._5_1 => "⑤K",
                    Component.InputMode._7_1 => "⑦K",
                    Component.InputMode._9 => "9K",
                    _ => string.Empty
                };
                return ability < 0.01 ? $"{inputModeText} < 0.01 Point ↑" : $"{inputModeText} {Math.Round(ability, 2)} Point ↑";
            }
        }

        public struct TwilightSiteYell
        {
            public string siteID;
            public string avatarID;
            public string avatarName;
            public long date;
            public string siteYell;
            public int siteYellID;
        }

        public struct TwilightModifySiteYell
        {
            public string siteID;
            public int siteYellID;
            public string siteYell;
        }

        public struct TwilightWipeSiteYell
        {
            public string siteID;
            public int siteYellID;
        }

        public struct TwilightCommentSiteYell
        {
            public string avatarID;
            public string avatarName;
            public string artist;
            public string title;
            public string genre;
            public string levelText;
            public BaseNoteFile.Level level;
            public int stand;
            public int hitPointsMode;
        }

        public sealed class TwilightAbilitySiteYell : TwilightAbilityUp
        {
            public string avatarID;
            public string avatarName;
        }

        public struct TwilightLevelSiteYell
        {
            public string avatarID;
            public string avatarName;
            public string title;

            public override string ToString() => string.Format(LanguageSystem.Instance.WwwLevelClearContents, title);
        }

        public struct TwilightInviteSiteYell
        {
            public string avatarName;
            public string siteID;
            public string siteName;
        }

        public struct TwilightTVSiteYell
        {
            public string href;
            public string title;
            public string text;
        }

        public struct TwilightGetSiteYells
        {
            public string siteID;
            public TwilightSiteYellItem[] data;
        }

        public class TwilightCallSiteNet
        {
            public string bundleEntryPath;
            public string[] noteIDs;
            public string bundleName;
            public string siteID;
            public string noteID;
            public string title;
            public string artist;
            public string levelText;
            public BaseNoteFile.Level level;
            public string wantLevelID;
            public string genre;
            public double judgmentStage;
            public double hitPointsValue;
            public int totalNotes;
            public int longNotes;
            public int autoableNotes;
            public int trapNotes;
            public int highestInputCount;
            public double length;
            public double bpm;
            public double lowestBPM;
            public double highestBPM;
            public Component.InputMode inputMode;
            public bool isAutoLongNote;
            public bool isFavorNoteFile;
            public bool isFavorModeComponent;
            public bool isFavorAudioMultiplier;
            public bool isAutoSiteHand;
            public int validHunterMode;
            public int validNetMode;
            public int[] allowedPostableItems;
        }

        public sealed class TwilightEnterSite : TwilightCallSiteNet
        {
            public string siteNotify;
            public bool isNetSite;
            public bool isGetNotify;
            public bool isEditable;
            public bool isAudioInput;
            public TwilightSiteYellItem[] data;
            public ModeComponentData modeComponentData;
        }

        public struct TwilightSiteYellItem
        {
            public string avatarID;
            public string avatarName;
            public long date;
            public string siteYell;
            public int siteYellID;

            public override string ToString()
            {
                var ltDate = DateTimeOffset.FromUnixTimeMilliseconds(date).LocalDateTime.ToLongTimeString();
                switch (avatarName)
                {
                    case "@Enter":
                        return $"{siteYell} {ltDate} {LanguageSystem.Instance.SiteYellEnter}";
                    case "@Quit":
                        return $"{siteYell} {ltDate} {LanguageSystem.Instance.SiteYellQuit}";
                    case "@Site":
                        return $"{siteYell} {ltDate} {LanguageSystem.Instance.SiteYellNewSite}";
                    case "@Net":
                        return $"{siteYell} {ltDate} {LanguageSystem.Instance.SiteYellNewNetSite}";
                    case "@Notify":
                        return $"{LanguageSystem.Instance.SiteYellTaehui} {ltDate} {siteYell}";
                    case "@Invite":
                        var twilightInviteSiteYell = Utility.GetJSON<TwilightInviteSiteYell>(siteYell);
                        return $"{twilightInviteSiteYell.avatarName} {ltDate} {string.Format(LanguageSystem.Instance.NotifySiteYellInvite, twilightInviteSiteYell.siteName)}";
                    case "@TV":
                        var twilightTVSiteYell = Utility.GetJSON<TwilightTVSiteYell>(siteYell);
                        return $"{twilightTVSiteYell.text} {ltDate} {string.Format(LanguageSystem.Instance.NotifySiteYellTV, twilightTVSiteYell.title)}";
                    case "@Wiped":
                        return $"{avatarName} {ltDate} {LanguageSystem.Instance.WipedSiteYell}";
                    case "@Comment":
                        var twilightCommentSiteYell = Utility.GetJSON<TwilightCommentSiteYell>(siteYell);
                        return $"{twilightCommentSiteYell.avatarName} {ltDate} {Utility.GetPlatformText(twilightCommentSiteYell.title, twilightCommentSiteYell.artist, Utility.GetGenreText(twilightCommentSiteYell.genre), twilightCommentSiteYell.levelText)} {twilightCommentSiteYell.stand.ToString(LanguageSystem.Instance.StandContents)}";
                    case "@Ability":
                        return Utility.GetJSON<TwilightAbilitySiteYell>(siteYell).ToString();
                    case "@Level":
                        return Utility.GetJSON<TwilightLevelSiteYell>(siteYell).ToString();
                    case "":
                        return siteYell;
                    default:
                        return $"{avatarName} {ltDate} {siteYell}";
                }
            }
        }

        public struct TwilightCallBundle
        {
            public string targetAvatar;
            public bool isSilent;
            public long targetValue;
            public long bundleLength;
            public BundleDataItem[] data;
        }

        public struct BundleDataItem
        {
            public int bundleVariety;
            public long date;
            public string bundleName;
            public long bundleLength;
            public int bundleCompetence;
        }

        public struct TwilightCallConfigure
        {
            public int silentSiteCompetence;
            public int toNotifyUbuntuCompetence;
            public int defaultBundleCompetence;
            public int ioCompetence;
            public int toNotifySaveBundle;
        }

        public struct TwilightCallUbuntu
        {
            public string ubuntuID;
            public string ubuntuName;
            public int situationValue;
            public string situationText;
        }

        public struct TwilightCallSiteAvatar
        {
            public string siteID;
            public string siteName;
            public string siteHand;
            public int situationValue;
            public bool setNoteFile;
            public CallSiteAvatarItem[] data;
        }

        public struct TwilightCallSiteModeComponent
        {
            public string siteID;
            public ModeComponentData modeComponentData;
        }

        public sealed class ModeComponentData
        {
            public int salt;
            public ModeComponent.AutoMode autoMode;
            public ModeComponent.NoteSaltMode noteSaltMode;
            public double audioMultiplier;
            public ModeComponent.FaintNoteMode faintNoteMode;
            public ModeComponent.JudgmentMode judgmentMode;
            public ModeComponent.HitPointsMode hitPointsMode;
            public ModeComponent.NoteMobilityMode noteMobilityMode;
            public ModeComponent.LongNoteMode longNoteMode;
            public ModeComponent.InputFavorMode inputFavorMode;
            public ModeComponent.NoteModifyMode noteModifyMode;
            public ModeComponent.BPMMode bpmMode;
            public ModeComponent.WaveMode waveMode;
            public ModeComponent.SetNoteMode setNoteMode;
            public ModeComponent.LowestJudgmentConditionMode lowestJudgmentConditionMode;
            public ModeComponent.PutCopyNotes putCopyNotes;
            public double highestJudgment0;
            public double higherJudgment0;
            public double highJudgment0;
            public double lowJudgment0;
            public double lowerJudgment0;
            public double lowestJudgment0;
            public double highestJudgment1;
            public double higherJudgment1;
            public double highJudgment1;
            public double lowJudgment1;
            public double lowerJudgment1;
            public double lowestJudgment1;
            public double lowestLongNoteModify;
            public double highestLongNoteModify;
            public double putNoteSet;
            public double putNoteSetMillis;
            public double highestHitPoints0;
            public double higherHitPoints0;
            public double highHitPoints0;
            public double lowHitPoints0;
            public double lowerHitPoints0;
            public double lowestHitPoints0;
            public double highestHitPoints1;
            public double higherHitPoints1;
            public double highHitPoints1;
            public double lowHitPoints1;
            public double lowerHitPoints1;
            public double lowestHitPoints1;
        }

        public struct CallSiteAvatarItem
        {
            public string avatarID;
            public int avatarConfigure;
            public string avatarName;
            public int avatarGroup;
            public bool isValve;
            public bool isAudioInput;
        }

        public struct TwilightSaveBundle
        {
            public string bundleID;
            public int bundleVariety;
            public int bundleLength;
        }

        public struct TwilightSavedBundle
        {
            public string bundleID;
            public int bundleVariety;
            public string bundleName;
            public string etc;
            public bool isLastDefault;
        }

        public struct TwilightLevyNet
        {
            public string siteID;
            public string[] noteIDs;
            public string handlerID;
            public bool isSiteHand;
            public bool isFavorModeComponent;
            public bool isFavorAudioMultiplier;
            public int validNetMode;
            public int avatarsCount;
            public int[] allowedPostableItems;
            public ModeComponentData modeComponentData;
        }

        public struct TwilightQuitNet
        {
            public string handlerID;
            public QuitNetItem[] quitNetItems;

            public struct QuitNetItem
            {
                public string avatarID;
                public string avatarName;
                public string title;
                public string artist;
                public string genre;
                public BaseNoteFile.Level level;
                public string levelText;
                public string wantLevelID;
                public ModeComponent.AutoMode autoMode;
                public ModeComponent.NoteSaltMode noteSaltMode;
                public double audioMultiplier;
                public ModeComponent.FaintNoteMode faintNoteMode;
                public ModeComponent.HitPointsMode hitPointsMode;
                public ModeComponent.JudgmentMode judgmentMode;
                public ModeComponent.NoteMobilityMode noteMobilityMode;
                public ModeComponent.InputFavorMode inputFavorMode;
                public ModeComponent.LongNoteMode longNoteMode;
                public ModeComponent.NoteModifyMode noteModifyMode;
                public ModeComponent.BPMMode bpmMode;
                public ModeComponent.WaveMode waveMode;
                public ModeComponent.SetNoteMode setNoteMode;
                public ModeComponent.LowestJudgmentConditionMode lowestJudgmentConditionMode;
                public int totalNotes;
                public double judgmentStage;
                public double hitPointsValue;
                public int highestInputCount;
                public double length;
                public double bpm;
                public double multiplier;
                public Component.InputMode inputMode;
                public int stand;
                public int highestBand;
                public double point;
                public double hitPoints;
                public bool isF;
                public int netPosition;
                public double highestJudgment0;
                public double higherJudgment0;
                public double highJudgment0;
                public double lowJudgment0;
                public double lowerJudgment0;
                public double lowestJudgment0;
                public double highestJudgment1;
                public double higherJudgment1;
                public double highJudgment1;
                public double lowJudgment1;
                public double lowerJudgment1;
                public double lowestJudgment1;
            }
        }

        public struct TwilightCallNetSiteComments
        {
            public long date;
            public CallNetSIteCommentItem[] data;

            public struct CallNetSIteCommentItem
            {
                public int avatarNetStatus;
                public string avatarID;
                public string avatarName;
                public int stand;
                public int band;
                public double point;
                public int highestJudgment;
                public int higherJudgment;
                public int highJudgment;
                public int lowJudgment;
                public int lowerJudgment;
                public int lowestJudgment;
            }
        }

        public struct TwilightSaveAsBundle
        {
            public string bundleID;
            public int bundleVariety;
            public string bundleName;
            public string bundleEntryPath;
        }

        public struct TwilightCallIO
        {
            public string avatarID;
            public string handlerID;
            public double ioMillis;
        }

        public struct TwilightCallIOComponent
        {
            public string noteID;
            public string handlerID;
            public string avatarID;
            public ModeComponentData data;
            public string avatarName;
            public string ioHandlerID;
            public bool isFailMode;
            public double ioMillis;
            public double targetIOMillis;
        }

        public struct TwilightCompiledIO
        {
            public string avatarID;
            public string avatarName;
            public string handlerID;
            public bool isCompiled;
        }

        public struct TwilightLevyIO
        {
            public string handlerID;
            public double levyingWait;
            public int lastStand;
            public bool isF;
            public double multiplier;
            public double audioMultiplier;
            public double ioMillis;
        }

        public struct TwilightIOQuit
        {
            public string handlerID;
            public string avatarID;
        }

        public struct TwilightIOPause
        {
            public string handlerID;
            public bool isPaused;
        }

        public struct TwilightWwwVote
        {
            public BaseNoteFile.NoteVariety noteVariety;
            public string title;
            public string artist;
            public string genre;
            public string www;
            public bool isFavorite;
        }

        public struct TwilightWwwTitle
        {
            public string title;
            public string titleColor;
        }

        public struct TwilightWwwTitles
        {
            public string titleID;
            public string title;
            public string titleColor;
        }

        public struct TwilightWwwLevelAvatars
        {
            public Avatar[] avatars;
            public string[] levelIDs;

            public struct Avatar
            {
                public string avatarID;
                public string avatarName;
            }
        }

        public struct TwilightWwwLevels
        {
            public string levelID;
            public string noteID;
            public string title;
            public string comment;
            public string levelText;
            public BaseNoteFile.Level level;
            public double avatars;
        }

        public struct TwilightWwwLevel
        {
            public LevelItem[] levelNote;
            public int[] stand;
            public double[] point;
            public int[] band;
            public int[][] judgments;
            public ModeComponent.AutoMode[] autoMode;
            public ModeComponent.NoteSaltMode[] noteSaltMode;
            public double[] audioMultiplier;
            public ModeComponent.FaintNoteMode[] faintNoteMode;
            public ModeComponent.JudgmentMode[] judgmentMode;
            public ModeComponent.HitPointsMode[] hitPointsMode;
            public ModeComponent.NoteMobilityMode[] noteMobilityMode;
            public ModeComponent.LongNoteMode[] longNoteMode;
            public ModeComponent.InputFavorMode[] inputFavorMode;
            public ModeComponent.NoteModifyMode[] noteModifyMode;
            public ModeComponent.BPMMode[] bpmMode;
            public ModeComponent.WaveMode[] waveMode;
            public ModeComponent.SetNoteMode[] setNoteMode;
            public ModeComponent.LowestJudgmentConditionMode[] lowestJudgmentConditionMode;
            public bool allowPause;
            public Title[] titles;
            public string[] edgeIDs;

            public struct LevelItem
            {
                public string noteID;
                public BaseNoteFile.NoteVariety noteVariety;
                public string artist;
                public string title;
                public string genre;
                public string levelText;
                public BaseNoteFile.Level level;
            }

            public struct Title
            {
                public string titleID;
                public string title;
                public string titleColor;
            }
        }

        public struct TwilightWwwAvatar
        {
            public string avatarID;
            public string avatarName;
            public string avatarIntro;
            public int totalCount;
            public double totalLength;
            public int highestCount;
            public long date;
            public int[] avatarLevels;
            public double avatarAbility5K;
            public double avatarAbility5KClass;
            public int avatarAbility5KPlace;
            public int avatarAbility5KCount;
            public double avatarAbility7K;
            public double avatarAbility7KClass;
            public int avatarAbility7KPlace;
            public int avatarAbility7KCount;
            public double avatarAbility9K;
            public double avatarAbility9KClass;
            public int avatarAbility9KPlace;
            public int avatarAbility9KCount;
            public WwwLevel[] wwwLevels;
            public Last[] lasts;
            public AvatarAbility[] abilities5K;
            public AvatarAbility[] abilities7K;
            public AvatarAbility[] abilities9K;
            public Favorite[] favorites;
            public long[] dateSet;
            public int[] dateValues;
            public int[] quitStatusValues;

            public struct WwwLevel
            {
                public string levelID;
                public string title;
                public string comment;
                public string levelText;
                public BaseNoteFile.Level level;
                public long date;
            }

            public struct Last
            {
                public string noteID;
                public BaseNoteFile.NoteVariety noteVariety;
                public string artist;
                public string title;
                public string genre;
                public string levelText;
                public BaseNoteFile.Level level;
                public long date;
            }

            public struct Favorite
            {
                public string noteID;
                public BaseNoteFile.NoteVariety noteVariety;
                public string artist;
                public string title;
                public string genre;
                public string levelText;
                public BaseNoteFile.Level level;
                public int totalCount;
            }

            public struct AvatarAbility
            {
                public string noteID;
                public BaseNoteFile.NoteVariety noteVariety;
                public string artist;
                public string title;
                public string genre;
                public string levelText;
                public BaseNoteFile.Level level;
                public int stand;
                public double ability;
            }
        }

        public struct TwilightWwwAvatarWwwLevel
        {
            public string levelID;
            public string title;
            public string comment;
            public string levelText;
            public BaseNoteFile.Level level;
            public long date;
        }

        public struct TwilightWwwAvatarLast
        {
            public string noteID;
            public BaseNoteFile.NoteVariety noteVariety;
            public string artist;
            public string title;
            public string genre;
            public string levelText;
            public BaseNoteFile.Level level;
            public long date;
        }

        public struct TwilightWwwAvatarFavorite
        {
            public string noteID;
            public BaseNoteFile.NoteVariety noteVariety;
            public string artist;
            public string title;
            public string genre;
            public string levelText;
            public BaseNoteFile.Level level;
            public int totalCount;
        }

        public struct TwilightWwwAvatarAbility
        {
            public string noteID;
            public BaseNoteFile.NoteVariety noteVariety;
            public string artist;
            public string title;
            public string genre;
            public string levelText;
            public BaseNoteFile.Level level;
            public int stand;
            public double ability;
        }

        public struct TwilightWwwHOF
        {
            public string avatarID;
            public string avatarName;
            public double value;
        }
    }
}
