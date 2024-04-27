using Microsoft.Data.Sqlite;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Qwilight
{
    public sealed class DB
    {
        enum Date : long
        {
            DB_1_0_0, DB_1_16_9, DB_1_16_15, DB_1_16_17, Max
        }
        const Date LatestDBDate = Date.Max - 1;

        public enum EventNoteVariety
        {
            Qwilight, MD5
        }

        static readonly string _fileName = Path.Combine(QwilightComponent.QwilightEntryPath, "DB.db");
        static readonly string _faultFileName = Path.ChangeExtension(_fileName, ".db.$");
        static readonly string _tmp0FileName = Path.ChangeExtension(_fileName, ".db.tmp.0");
        static readonly string _tmp1FileName = Path.ChangeExtension(_fileName, ".db.tmp.1");

        public static readonly DB Instance = QwilightComponent.GetBuiltInData<DB>(nameof(DB));

        readonly object _setSaveCSX = new();
        SqliteConnection _fastDB;
        SqliteConnection _fileDB;

        public string DBFault { get; set; }

        public void Load()
        {
            Utility.WipeFile(_tmp0FileName);
            Utility.MoveFile(_tmp1FileName, _fileName);
            _fastDB = new(new SqliteConnectionStringBuilder
            {
                DataSource = ":memory:"
            }.ToString());
            _fileDB = new(new SqliteConnectionStringBuilder
            {
                DataSource = _fileName
            }.ToString());
            try
            {
                LoadImpl();
            }
            catch (SqliteException e)
            {
                DBFault = $"Failed to Validate DB ({e.Message})";
                _fastDB.Close();
                _fileDB.Close();
                Utility.MoveFile(_fileName, _faultFileName);
                LoadImpl();
            }

            void LoadImpl()
            {
                _fastDB.Open();
                _fileDB.Open();
                _fileDB.BackupDatabase(_fastDB);

                #region COMPATIBLE
                Compatible.Compatible.DB(_fastDB);
                #endregion

                Ta(ta =>
                {
                    #region 데이터베이스 정보
                    Date date;
                    using (var dbStatement = NewDBStatement("PRAGMA user_version", ta))
                    {
                        date = (Date)dbStatement.ExecuteScalar();
                    }
                    using (var dbStatement = NewDBStatement($"PRAGMA user_version = {((long)LatestDBDate)}", ta))
                    {
                        dbStatement.ExecuteNonQuery();
                    }
                    #endregion

                    #region 오프라인 기록
                    if (date < Date.DB_1_16_17)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_comment (
                                Date DATE,
                                Event_Note_ID TEXT,
                                Comment TEXT,
                                Name TEXT,
                                Multiplier REAL,
                                Auto_Mode INTEGER,
                                Note_Salt_Mode INTEGER,
                                Audio_Multiplier REAL,
                                Faint_Note_Mode INTEGER,
                                Judgment_Mode INTEGER,
                                Hit_Points_Mode INTEGER DEFAULT 1,
                                Note_Mobility_Mode INTEGER,
                                Long_Note_Mode INTEGER DEFAULT 0,
                                Input_Favor_Mode INTEGER DEFAULT 0,
                                Note_Modify_Mode INTEGER DEFAULT 0,
                                BPM_Mode INTEGER DEFAULT 0,
                                Wave_Mode INTEGER DEFAULT 0,
                                Set_Note_Mode INTEGER DEFAULT 0,
                                Lowest_Judgment_Condition_Mode INTEGER DEFAULT 0,
                                Stand INTEGER,
                                Band INTEGER,
                                Is_Band1 INTEGER DEFAULT 0,
                                Point REAL,
                                Salt INTEGER,
                                Highest_Judgment_0 REAL DEFAULT 0.0,
                                Higher_Judgment_0 REAL DEFAULT 0.0,
                                High_Judgment_0 REAL DEFAULT 0.0,
                                Low_Judgment_0 REAL DEFAULT 0.0,
                                Lower_Judgment_0 REAL DEFAULT 0.0,
                                Lowest_Judgment_0 REAL DEFAULT 0.0,
                                Highest_Judgment_1 REAL DEFAULT 0.0,
                                Higher_Judgment_1 REAL DEFAULT 0.0,
                                High_Judgment_1 REAL DEFAULT 0.0,
                                Low_Judgment_1 REAL DEFAULT 0.0,
                                Lower_Judgment_1 REAL DEFAULT 0.0,
                                Lowest_Judgment_1 REAL DEFAULT 0.0,
                                Lowest_Long_Note_Modify REAL DEFAULT 100.0,
                                Highest_Long_Note_Modify REAL DEFAULT 100.0,
                                Input_Favor_Labelled_Millis REAL DEFAULT 0.0,
                                Set_Note_Put REAL DEFAULT 1.0,
                                Set_Note_Put_Millis REAL DEFAULT 0.0,
                                Highest_Hit_Points_0 REAL DEFAULT 0.0,
                                Higher_Hit_Points_0 REAL DEFAULT 0.0,
                                High_Hit_Points_0 REAL DEFAULT 0.0,
                                Low_Hit_Points_0 REAL DEFAULT 0.0,
                                Lower_Hit_Points_0 REAL DEFAULT 0.0,
                                Lowest_Hit_Points_0 REAL DEFAULT 0.0,
                                Highest_Hit_Points_1 REAL DEFAULT 0.0,
                                Higher_Hit_Points_1 REAL DEFAULT 0.0,
                                High_Hit_Points_1 REAL DEFAULT 0.0,
                                Low_Hit_Points_1 REAL DEFAULT 0.0,
                                Lower_Hit_Points_1 REAL DEFAULT 0.0,
                                Lowest_Hit_Points_1 REAL DEFAULT 0.0,
                                Note_ID VARCHAR(139),
                                Is_Paused INTEGER DEFAULT 0,
                                Input_Flags INTEGER DEFAULT 0
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("comment", ta))
                        {
                            // Input_Favor_Labelled_Millis
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_Band1, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Set_Note_Put, Set_Note_Put_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID, Is_Paused, Input_Flags)
                                    SELECT *
                                    FROM comment
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE comment", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_comment
                            RENAME TO comment
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = NewDBStatement("""
                            CREATE INDEX IF NOT EXISTS _comment ON comment (
                                Note_ID,
                                Event_Note_ID
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 폴더의 선택된 노트 파일
                    if (date < Date.DB_1_16_9)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_entry (
                                Entry_Path VARCHAR(260),
                                Note_Position INTEGER,
                                PRIMARY KEY (Entry_Path)
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("entry", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_entry
                                    SELECT *
                                    FROM entry
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE entry", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_entry
                            RENAME TO entry
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 컬렉션
                    if (date < Date.DB_1_16_9)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_favorite_entry (
                                Note_ID VARCHAR(139),
                                Favorite_Entry TEXT
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("favorite_entry", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_favorite_entry
                                    SELECT *
                                    FROM favorite_entry
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE favorite_entry", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_favorite_entry
                            RENAME TO favorite_entry
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = NewDBStatement("""
                            CREATE INDEX IF NOT EXISTS _favorite_entry ON favorite_entry (
                                Note_ID
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 클리어 램프
                    if (date < Date.DB_1_16_9)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_handled (
                                Note_ID VARCHAR(137),
                                Handled INTEGER,
                                PRIMARY KEY (Note_ID)
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("handled", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_handled
                                    SELECT *
                                    FROM handled
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE handled", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_handled
                            RENAME TO handled
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 클리어 시간들 (플레이 카운트, 마지막 플레이 정렬)
                    if (date < Date.DB_1_16_9)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_date (
                                Note_ID VARCHAR(139),
                                Event_Note_ID TEXT,
                                Date DATE
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("date", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_date
                                    SELECT *
                                    FROM date
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE date", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_date
                            RENAME TO date
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = NewDBStatement("""
                            CREATE INDEX IF NOT EXISTS _date ON date (
                                Note_ID,
                                Event_Note_ID
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 노트 파일 설정 (오디오 레이턴시, BGA 레이턴시, BGA 활성화)
                    if (date < Date.DB_1_16_15)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_wait (
                                Note_ID VARCHAR(139),
                                Audio_Wait REAL,
                                Media_Wait REAL DEFAULT 0,
                                Media LONG DEFAULT 1,
                                PRIMARY KEY (Note_ID)
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("wait", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_wait
                                    SELECT *
                                    FROM wait
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE wait", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_wait
                            RENAME TO wait
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 텍스트 인코딩
                    if (date < Date.DB_1_16_9)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_format (
                                Note_ID VARCHAR(139),
                                Format LONG DEFAULT -1,
                                PRIMARY KEY (Note_ID)
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("format", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_format
                                    SELECT *
                                    FROM format
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE format", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_format
                            RENAME TO format
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 코스
                    if (date < Date.DB_1_16_15)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_event_note (
                                Event_Note_ID TEXT,
                                Name TEXT,
                                Date DATE,
                                Variety INTEGER DEFAULT 0,
                                PRIMARY KEY (Event_Note_ID, Variety)
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("event_note", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_event_note
                                    SELECT *
                                    FROM event_note
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE event_note", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_event_note
                            RENAME TO event_note
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region 코스 캐시
                    if (date < Date.DB_1_16_9)
                    {
                        using (var dbStatement = NewDBStatement("""
                            CREATE TABLE IF NOT EXISTS tmp_event_note_data (
                                Note_ID VARCHAR(139),
                                Note_Variety INTEGER,
                                Title TEXT,
                                Artist TEXT,
                                Level INTEGER,
                                Level_Text TEXT,
                                Genre TEXT,
                                PRIMARY KEY (Note_ID)
                            )
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (HasTable("event_note_data", ta))
                        {
                            using (var dbStatement = NewDBStatement("""
                                INSERT
                                INTO tmp_event_note_data
                                    SELECT *
                                    FROM event_note_data
                            """, ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                            using (var dbStatement = NewDBStatement("DROP TABLE event_note_data", ta))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = NewDBStatement("""
                            ALTER TABLE tmp_event_note_data
                            RENAME TO event_note_data
                        """, ta))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    bool HasTable(string tableName, SqliteTransaction t)
                    {
                        var dbStatement = NewDBStatement("""
                            SELECT name
                            FROM sqlite_master
                            WHERE type = "table" AND name = @tableName
                        """, t);
                        dbStatement.Parameters.AddWithValue("tableName", tableName);
                        return dbStatement.ExecuteScalar() != null;
                    }
                });
            }
        }

        public ICollection<CommentItem> GetCommentItems(BaseNoteFile noteFile, string eventNoteID, int noteFileCount)
        {
            var commentItems = new List<CommentItem>();
            var dbStatement = NewDBStatement($"""
                SELECT Date, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_Band1, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Input_Favor_Labelled_Millis, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Set_Note_Put, Set_Note_Put_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Is_Paused, Input_Flags
                FROM comment
                WHERE {(string.IsNullOrEmpty(eventNoteID) ? "Note_ID = @noteID" : "Event_Note_ID = @eventNoteID")}
                ORDER BY Stand DESC
                LIMIT 50
            """);
            if (string.IsNullOrEmpty(eventNoteID))
            {
                dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            }
            else
            {
                dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            }
            using var rows = dbStatement.ExecuteReader();
            while (rows.Read())
            {
                var date = rows.GetDateTime("Date");
                var sentMultiplier = rows.GetDouble("Multiplier");
                var audioMultiplier = Math.Round(rows.GetDouble("Audio_Multiplier"), 2);
                commentItems.Add(new(string.Empty, (DefaultCompute.InputFlag)rows.GetInt32("Input_Flags"))
                {
                    NoteFileCount = noteFileCount,
                    Date = date,
                    DateText = Utility.GetDateText(date),
                    CommentID = rows.GetString("Comment"),
                    AvatarName = rows.GetString("Name"),
                    ModeComponentValue = new()
                    {
                        CanModifyMultiplier = false,
                        CanModifyAudioMultiplier = false,
                        ComputingValue = noteFile,
                        SentMultiplier = sentMultiplier,
                        MultiplierValue = noteFile.BPM * audioMultiplier * sentMultiplier,
                        AutoModeValue = (ModeComponent.AutoMode)rows.GetInt32("Auto_Mode"),
                        NoteSaltModeValue = (ModeComponent.NoteSaltMode)rows.GetInt32("Note_Salt_Mode"),
                        AudioMultiplier = audioMultiplier,
                        FaintNoteModeValue = (ModeComponent.FaintNoteMode)rows.GetInt32("Faint_Note_Mode"),
                        JudgmentModeValue = (ModeComponent.JudgmentMode)rows.GetInt32("Judgment_Mode"),
                        HitPointsModeValue = (ModeComponent.HitPointsMode)rows.GetInt32("Hit_Points_Mode"),
                        NoteMobilityModeValue = (ModeComponent.NoteMobilityMode)rows.GetInt32("Note_Mobility_Mode"),
                        LongNoteModeValue = (ModeComponent.LongNoteMode)rows.GetInt32("Long_Note_Mode"),
                        InputFavorModeValue = (ModeComponent.InputFavorMode)rows.GetInt32("Input_Favor_Mode"),
                        NoteModifyModeValue = (ModeComponent.NoteModifyMode)rows.GetInt32("Note_Modify_Mode"),
                        BPMModeValue = (ModeComponent.BPMMode)rows.GetInt32("BPM_Mode"),
                        WaveModeValue = (ModeComponent.WaveMode)rows.GetInt32("Wave_Mode"),
                        SetNoteModeValue = (ModeComponent.SetNoteMode)rows.GetInt32("Set_Note_Mode"),
                        LowestJudgmentConditionModeValue = (ModeComponent.LowestJudgmentConditionMode)rows.GetInt32("Lowest_Judgment_Condition_Mode"),
                        Salt = rows.GetInt32("Salt"),
                        HighestJudgment0 = rows.GetDouble("Highest_Judgment_0"),
                        HigherJudgment0 = rows.GetDouble("Higher_Judgment_0"),
                        HighJudgment0 = rows.GetDouble("High_Judgment_0"),
                        LowJudgment0 = rows.GetDouble("Low_Judgment_0"),
                        LowerJudgment0 = rows.GetDouble("Lower_Judgment_0"),
                        LowestJudgment0 = rows.GetDouble("Lowest_Judgment_0"),
                        HighestJudgment1 = rows.GetDouble("Highest_Judgment_1"),
                        HigherJudgment1 = rows.GetDouble("Higher_Judgment_1"),
                        HighJudgment1 = rows.GetDouble("High_Judgment_1"),
                        LowJudgment1 = rows.GetDouble("Low_Judgment_1"),
                        LowerJudgment1 = rows.GetDouble("Lower_Judgment_1"),
                        LowestJudgment1 = rows.GetDouble("Lowest_Judgment_1"),
                        InputFavorLabelledMillis = rows.GetDouble("Input_Favor_Labelled_Millis"),
                        LowestLongNoteModify = rows.GetDouble("Lowest_Long_Note_Modify"),
                        HighestLongNoteModify = rows.GetDouble("Highest_Long_Note_Modify"),
                        SetNotePut = rows.GetDouble("Set_Note_Put"),
                        SetNotePutMillis = rows.GetDouble("Set_Note_Put_Millis"),
                        HighestHitPoints0 = 100.0 * rows.GetDouble("Highest_Hit_Points_0"),
                        HigherHitPoints0 = 100.0 * rows.GetDouble("Higher_Hit_Points_0"),
                        HighHitPoints0 = 100.0 * rows.GetDouble("High_Hit_Points_0"),
                        LowHitPoints0 = 100.0 * rows.GetDouble("Low_Hit_Points_0"),
                        LowerHitPoints0 = 100.0 * rows.GetDouble("Lower_Hit_Points_0"),
                        LowestHitPoints0 = 100.0 * rows.GetDouble("Lowest_Hit_Points_0"),
                        HighestHitPoints1 = 100.0 * rows.GetDouble("Highest_Hit_Points_1"),
                        HigherHitPoints1 = 100.0 * rows.GetDouble("Higher_Hit_Points_1"),
                        HighHitPoints1 = 100.0 * rows.GetDouble("High_Hit_Points_1"),
                        LowHitPoints1 = 100.0 * rows.GetDouble("Low_Hit_Points_1"),
                        LowerHitPoints1 = 100.0 * rows.GetDouble("Lower_Hit_Points_1"),
                        LowestHitPoints1 = 100.0 * rows.GetDouble("Lowest_Hit_Points_1"),
                    },
                    Stand = rows.GetInt32("Stand"),
                    Band = rows.GetInt32("Band"),
                    IsBand1 = rows.GetBoolean("Is_Band1"),
                    Point = rows.GetDouble("Point"),
                    IsPaused = rows.GetBoolean("Is_Paused")
                });
            }
            for (var i = commentItems.Count - 1; i >= 0; --i)
            {
                commentItems[i].CommentPlace0Text = $"＃{i + 1}";
                commentItems[i].CommentPlace1Text = $"／{commentItems.Count}";
            }
            return commentItems;
        }

        public void SetEventNoteData(ICollection<WwwLevelViewModel.WwwLevelComputing> wwwLevelComputingValues)
        {
            foreach (var wwwLevelComputingValue in wwwLevelComputingValues)
            {
                var noteVariety = (BaseNoteFile.NoteVariety)wwwLevelComputingValue.NoteVarietyValue;
                if (noteVariety != BaseNoteFile.NoteVariety.EventNote)
                {
                    var dbStatement = NewDBStatement("""
                        REPLACE
                        INTO event_note_data
                        VALUES(@noteID, @noteVariety, @title, @artist, @level, @levelText, @genre)
                    """);
                    dbStatement.Parameters.AddWithValue("noteID", wwwLevelComputingValue.GetNoteID512());
                    dbStatement.Parameters.AddWithValue("noteVariety", noteVariety);
                    dbStatement.Parameters.AddWithValue("title", wwwLevelComputingValue.Title);
                    dbStatement.Parameters.AddWithValue("artist", wwwLevelComputingValue.Artist);
                    dbStatement.Parameters.AddWithValue("level", wwwLevelComputingValue.LevelValue);
                    dbStatement.Parameters.AddWithValue("levelText", wwwLevelComputingValue.LevelText);
                    dbStatement.Parameters.AddWithValue("genre", wwwLevelComputingValue.Genre);
                    dbStatement.ExecuteNonQuery();
                }
            }
        }

        public void GetEventNoteData(string noteID, NotAvailableNoteFile noteFile)
        {
            var dbStatement = NewDBStatement("""
                SELECT Note_Variety, Title, Artist, Level, Level_Text, Genre
                FROM event_note_data
                WHERE Note_ID = @noteID
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteID);
            using var rows = dbStatement.ExecuteReader();
            if (rows.Read())
            {
                noteFile.NotAvailableNoteVarietyValue = (BaseNoteFile.NoteVariety)rows.GetInt32("Note_Variety");
                if (!rows.IsDBNull("Title"))
                {
                    noteFile.Title = rows.GetString("Title");
                }
                if (!rows.IsDBNull("Artist"))
                {
                    noteFile.Artist = rows.GetString("Artist");
                }
                noteFile.LevelValue = (BaseNoteFile.Level)rows.GetInt32("Level");
                if (!rows.IsDBNull("Level_Text"))
                {
                    noteFile.LevelText = rows.GetString("Level_Text");
                }
                if (!rows.IsDBNull("Genre"))
                {
                    noteFile.Genre = rows.GetString("Genre");
                }
            }
            else
            {
                noteFile.NotAvailableNoteVarietyValue = BaseNoteFile.NoteVariety.EventNote;
                noteFile.Title = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
                noteFile.Artist = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
                noteFile.LevelText = "❌";
                noteFile.Genre = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
            }
        }

        public ICollection<DefaultEntryItem> GetFavoriteEntryItems(BaseNoteFile noteFile)
        {
            var favoriteEntryItems = new List<DefaultEntryItem>();
            var dbStatement = NewDBStatement("""
                SELECT Favorite_Entry
                FROM favorite_entry
                WHERE Note_ID = @noteID
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            while (rows.Read())
            {
                favoriteEntryItems.Add(new DefaultEntryItem
                {
                    DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Favorite,
                    DefaultEntryPath = rows.GetString("Favorite_Entry")
                });
            }
            return favoriteEntryItems;
        }

        public ICollection<(string, string, DateTime, EventNoteVariety)> GetEventNotes()
        {
            var eventNotes = new List<(string, string, DateTime, EventNoteVariety)>();
            var dbStatement = NewDBStatement("""
                SELECT Event_Note_ID, Name, Date, Variety
                FROM event_note
            """);
            using var rows = dbStatement.ExecuteReader();
            while (rows.Read())
            {
                eventNotes.Add((rows.GetString("Event_Note_ID"), rows.GetString("Name"), rows.GetDateTime("Date"), (EventNoteVariety)rows.GetInt32("Variety")));
            }
            return eventNotes;
        }

        public BaseNoteFile.Handled GetHandled(BaseNoteFile noteFile)
        {
            var dbStatement = NewDBStatement("""
                SELECT Handled
                FROM handled
                WHERE Note_ID = @noteID
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? (BaseNoteFile.Handled)rows.GetInt32("Handled") : BaseNoteFile.Handled.Not;
        }

        public void SetHandled(BaseNoteFile noteFile)
        {
            var dbStatement = NewDBStatement("""
                REPLACE
                INTO handled
                VALUES(@noteID, @handled)
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("handled", noteFile.HandledValue);
            dbStatement.ExecuteNonQuery();
        }

        public (DateTime?, int) GetDate(BaseNoteFile noteFile, string eventNoteID)
        {
            var date = (null as DateTime?, 0);
            var dbStatement = NewDBStatement($"""
                SELECT MAX(Date) AS Latest, COUNT(Date) AS Count
                FROM date
                WHERE {(string.IsNullOrEmpty(eventNoteID) ? "Note_ID = @noteID" : "Event_Note_ID = @eventNoteID")}
            """);
            if (string.IsNullOrEmpty(eventNoteID))
            {
                dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            }
            else
            {
                dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            }
            using var rows = dbStatement.ExecuteReader();
            if (rows.Read())
            {
                if (!rows.IsDBNull("Latest"))
                {
                    date.Item1 = rows.GetDateTime("Latest");
                }
                if (!rows.IsDBNull("Count"))
                {
                    date.Item2 = rows.GetInt32("Count");
                }
            }
            return date;
        }

        public void NewDate(BaseNoteFile noteFile, string eventNoteID, DateTime date)
        {
            var dbStatement = NewDBStatement("""
                INSERT
                INTO date
                VALUES(@noteID, @eventNoteID, @date)
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512() ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.ExecuteNonQuery();
        }

        public int GetNotePosition(string entryPath)
        {
            var dbStatement = NewDBStatement("""
                SELECT Note_Position
                FROM entry
                WHERE Entry_Path = @entryPath
            """);
            dbStatement.Parameters.AddWithValue("entryPath", entryPath);
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? rows.GetInt32("Note_Position") : 0;
        }

        public (double, double, bool?) GetWait(BaseNoteFile noteFile)
        {
            (double, double, bool?) data = (0.0, 0.0, default);
            var dbStatement = NewDBStatement("""
                SELECT Audio_Wait, Media_Wait, Media
                FROM wait
                WHERE Note_ID = @noteID
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            if (rows.Read())
            {
                if (!rows.IsDBNull("Audio_Wait"))
                {
                    data.Item1 = rows.GetDouble("Audio_Wait");
                }
                data.Item2 = rows.GetDouble("Media_Wait");
                if (!rows.IsDBNull("Media"))
                {
                    data.Item3 = rows.GetInt32("Media") > 0;
                }
            }
            return data;
        }

        public int GetFormat(BaseNoteFile noteFile)
        {
            var dbStatement = NewDBStatement("""
                SELECT Format
                FROM format
                WHERE Note_ID = @noteID
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? rows.GetInt32("Format") : -1;
        }

        public void SetNotePosition(EntryItem entryItem)
        {
            var dbStatement = NewDBStatement("""
                REPLACE
                INTO entry
                VALUES(@entryPath, @notePosition)
            """);
            dbStatement.Parameters.AddWithValue("entryPath", entryItem.EntryPath);
            dbStatement.Parameters.AddWithValue("notePosition", entryItem.NotePosition);
            dbStatement.ExecuteNonQuery();
        }

        public void SetFavoriteEntry(BaseNoteFile noteFile)
        {
            Ta(ta =>
            {
                using (var dbStatement = NewDBStatement("""
                    DELETE
                    FROM favorite_entry
                    WHERE Note_ID = @noteID
                """, ta))
                {
                    dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
                    dbStatement.ExecuteNonQuery();
                }
                using (var dbStatement = NewDBStatement("""
                    REPLACE
                    INTO favorite_entry
                    VALUES(@noteID, @favoriteEntry)
                """, ta))
                {
                    foreach (var favoriteEntryItem in noteFile.FavoriteEntryItems)
                    {
                        dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
                        dbStatement.Parameters.AddWithValue("favoriteEntry", favoriteEntryItem.DefaultEntryPath);
                        dbStatement.ExecuteNonQuery();
                    }
                }
            });
        }

        public void SetEventNote(string eventNoteID, string eventNoteName, DateTime date, EventNoteVariety eventNoteVariety)
        {
            var dbStatement = NewDBStatement("""
                INSERT INTO event_note
                VALUES(@eventNoteID, @eventNoteName, @date, @eventNoteVariety)
            """);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.Parameters.AddWithValue("eventNoteName", eventNoteName);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.Parameters.AddWithValue("eventNoteVariety", eventNoteVariety);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeEventNote(string eventNoteID)
        {
            var dbStatement = NewDBStatement("""
                DELETE
                FROM event_note
                WHERE Event_Note_ID = @eventNoteID
            """);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.ExecuteNonQuery();
        }

        public void ModifyEventNoteName(string eventNoteID, string eventNoteName)
        {
            var dbStatement = NewDBStatement("""
                UPDATE event_note
                SET Name = @eventNoteName
                WHERE Event_Note_ID = @eventNoteID
            """);
            dbStatement.Parameters.AddWithValue("eventNoteName", eventNoteName);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.ExecuteNonQuery();
        }

        public void SaveComment(DateTime date, BaseNoteFile noteFile, string eventNoteID, string comment, string avatar, double multiplier, double audioMultiplier, ModeComponent modeComponent, int stand, int band, bool isBand1, double point, bool isPaused, DefaultCompute.InputFlag inputFlags)
        {
            var dbStatement = NewDBStatement("""
                INSERT
                INTO comment
                VALUES(@date, @eventNoteID, @comment, @avatar, @multiplier, @autoMode, @noteSaltMode, @audioMultiplier, @faintNoteMode, @judgmentMode, @hitPointsMode, @noteMobilityMode, @longNoteMode, @inputFavorMode, @noteModifyMode, @bpmMode, @waveMode, @setNoteMode, @lowestJudgmentConditionMode, @stand, @band, @isBand1, @point, @salt, @highestJudgment0, @higherJudgment0, @highJudgment0, @lowJudgment0, @lowerJudgment0, @lowestJudgment0, @highestJudgment1, @higherJudgment1, @highJudgment1, @lowJudgment1, @lowerJudgment1, @lowestJudgment1, @inputFavorLabelledMillis, @lowestLongNoteModify, @highestLongNoteModify, @setNotePut, @setNotePutMillis, @highestHitPoints0, @higherHitPoints0, @highHitPoints0, @lowHitPoints0, @lowerHitPoints0, @lowestHitPoints0, @highestHitPoints1, @higherHitPoints1, @highHitPoints1, @lowHitPoints1, @lowerHitPoints1, @lowestHitPoints1, @noteID, @isPaused, @inputFlags)
            """);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("comment", comment);
            dbStatement.Parameters.AddWithValue("avatar", avatar);
            dbStatement.Parameters.AddWithValue("multiplier", multiplier);
            dbStatement.Parameters.AddWithValue("autoMode", modeComponent.AutoModeValue);
            dbStatement.Parameters.AddWithValue("noteSaltMode", modeComponent.NoteSaltModeValue);
            dbStatement.Parameters.AddWithValue("audioMultiplier", audioMultiplier);
            dbStatement.Parameters.AddWithValue("faintNoteMode", modeComponent.FaintNoteModeValue);
            dbStatement.Parameters.AddWithValue("judgmentMode", modeComponent.JudgmentModeValue);
            dbStatement.Parameters.AddWithValue("hitPointsMode", modeComponent.HandlingHitPointsModeValue);
            dbStatement.Parameters.AddWithValue("noteMobilityMode", modeComponent.NoteMobilityModeValue);
            dbStatement.Parameters.AddWithValue("longNoteMode", modeComponent.LongNoteModeValue);
            dbStatement.Parameters.AddWithValue("inputFavorMode", modeComponent.InputFavorModeValue);
            dbStatement.Parameters.AddWithValue("noteModifyMode", modeComponent.NoteModifyModeValue);
            dbStatement.Parameters.AddWithValue("bpmMode", modeComponent.BPMModeValue);
            dbStatement.Parameters.AddWithValue("waveMode", modeComponent.WaveModeValue);
            dbStatement.Parameters.AddWithValue("setNoteMode", modeComponent.SetNoteModeValue);
            dbStatement.Parameters.AddWithValue("lowestJudgmentConditionMode", modeComponent.LowestJudgmentConditionModeValue);
            dbStatement.Parameters.AddWithValue("stand", stand);
            dbStatement.Parameters.AddWithValue("band", band);
            dbStatement.Parameters.AddWithValue("isBand1", isBand1);
            dbStatement.Parameters.AddWithValue("point", point);
            dbStatement.Parameters.AddWithValue("salt", modeComponent.Salt);
            dbStatement.Parameters.AddWithValue("highestJudgment0", modeComponent.HighestJudgment0);
            dbStatement.Parameters.AddWithValue("higherJudgment0", modeComponent.HigherJudgment0);
            dbStatement.Parameters.AddWithValue("highJudgment0", modeComponent.HighJudgment0);
            dbStatement.Parameters.AddWithValue("lowJudgment0", modeComponent.LowJudgment0);
            dbStatement.Parameters.AddWithValue("lowerJudgment0", modeComponent.LowerJudgment0);
            dbStatement.Parameters.AddWithValue("lowestJudgment0", modeComponent.LowestJudgment0);
            dbStatement.Parameters.AddWithValue("highestJudgment1", modeComponent.HighestJudgment1);
            dbStatement.Parameters.AddWithValue("higherJudgment1", modeComponent.HigherJudgment1);
            dbStatement.Parameters.AddWithValue("highJudgment1", modeComponent.HighJudgment1);
            dbStatement.Parameters.AddWithValue("lowJudgment1", modeComponent.LowJudgment1);
            dbStatement.Parameters.AddWithValue("lowerJudgment1", modeComponent.LowerJudgment1);
            dbStatement.Parameters.AddWithValue("lowestJudgment1", modeComponent.LowestJudgment1);
            dbStatement.Parameters.AddWithValue("inputFavorLabelledMillis", modeComponent.InputFavorLabelledMillis);
            dbStatement.Parameters.AddWithValue("lowestLongNoteModify", modeComponent.LowestLongNoteModify);
            dbStatement.Parameters.AddWithValue("highestLongNoteModify", modeComponent.HighestLongNoteModify);
            dbStatement.Parameters.AddWithValue("setNotePut", modeComponent.SetNotePut);
            dbStatement.Parameters.AddWithValue("setNotePutMillis", modeComponent.SetNotePutMillis);
            dbStatement.Parameters.AddWithValue("highestHitPoints0", modeComponent.HighestHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("higherHitPoints0", modeComponent.HigherHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("highHitPoints0", modeComponent.HighHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("lowHitPoints0", modeComponent.LowHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("lowerHitPoints0", modeComponent.LowerHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("lowestHitPoints0", modeComponent.LowestHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("highestHitPoints1", modeComponent.HighestHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("higherHitPoints1", modeComponent.HigherHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("highHitPoints1", modeComponent.HighHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("lowHitPoints1", modeComponent.LowHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("lowerHitPoints1", modeComponent.LowerHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("lowestHitPoints1", modeComponent.LowestHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512() ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("isPaused", isPaused);
            dbStatement.Parameters.AddWithValue("inputFlags", inputFlags);
            dbStatement.ExecuteNonQuery();
        }

        public void SetWait(BaseNoteFile noteFile, double audioWait, double mediaWait, bool media)
        {
            var dbStatement = NewDBStatement("""
                REPLACE
                INTO wait
                VALUES(@noteID, @audioWait, @mediaWait, @media)
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("audioWait", audioWait);
            dbStatement.Parameters.AddWithValue("mediaWait", mediaWait);
            dbStatement.Parameters.AddWithValue("media", media);
            dbStatement.ExecuteNonQuery();
        }

        public void SetNoteFormat(BaseNoteFile noteFile, int format)
        {
            var dbStatement = NewDBStatement("""
                REPLACE
                INTO format
                VALUES(@noteID, @format)
            """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("format", format);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeFavoriteEntry()
        {
            var dbStatement = NewDBStatement("""
                DELETE
                FROM favorite_entry
            """);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeHandled(BaseNoteFile noteFile)
        {
            var dbStatement = NewDBStatement("""
                DELETE
                FROM handled
                WHERE Note_ID = @noteID
             """);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.ExecuteNonQuery();
        }

        public void InitWait()
        {
            var dbStatement = NewDBStatement("""
                UPDATE wait
                SET Audio_Wait = NULL, Media_Wait = 0.0
            """);
            dbStatement.ExecuteNonQuery();
        }

        public void InitMedia()
        {
            var dbStatement = NewDBStatement("""
                UPDATE wait
                SET Media = NULL
            """);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeComment(string comment)
        {
            var dbStatement = NewDBStatement("""
                DELETE
                FROM comment
                WHERE Comment = @comment
            """);
            dbStatement.Parameters.AddWithValue("comment", comment);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeComment()
        {
            var dbStatement = NewDBStatement("""
                DELETE
                FROM comment
            """);
            dbStatement.ExecuteNonQuery();
        }

        void Ta(Action<SqliteTransaction> onHandle)
        {
            using var ta = _fastDB.BeginTransaction();
            try
            {
                onHandle(ta);
                ta.Commit();
            }
            catch
            {
                ta.Rollback();
                throw;
            }
        }

        SqliteCommand NewDBStatement(string text, SqliteTransaction ta = null)
        {
            return new(text, _fastDB, ta);
        }

        public void Save()
        {
            lock (_setSaveCSX)
            {
                Utility.CopyFile(_fileName, _tmp0FileName);
                Utility.MoveFile(_tmp0FileName, _tmp1FileName);
                _fastDB.BackupDatabase(_fileDB);
                Utility.WipeFile(_tmp1FileName);
            }
        }
    }
}