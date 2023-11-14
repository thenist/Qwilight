using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;

namespace Qwilight
{
    public sealed class DB
    {
        public enum EventNoteVariety
        {
            Qwilight, MD5
        }

        static readonly string _fileName = Path.Combine(QwilightComponent.QwilightEntryPath, "DB.db");
        static readonly string _faultFileName = Path.ChangeExtension(_fileName, ".db.$");

        public static readonly DB Instance = QwilightComponent.GetBuiltInData<DB>(nameof(DB));

        SQLiteConnection _db;

        public string DBFault { get; set; }

        public void Load()
        {
            _db = new(new SQLiteConnectionStringBuilder
            {
                DataSource = _fileName
            }.ToString());
            try
            {
                LoadImpl();
            }
            catch (SQLiteException e)
            {
                DBFault = $"Failed to Validate DB ({e.Message})";
                _db.Close();
                Utility.MoveFile(_fileName, _faultFileName);
                LoadImpl();
            }

            void LoadImpl()
            {
                _db.Open();

                #region COMPATIBLE
                Compatible.Compatible.DB(_db);
                #endregion

                #region 데이터베이스 정보
                using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS db_file (
					ID VARCHAR(137),
					Value TEXT,
					PRIMARY KEY (ID)
				)", _db))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion

                var date = new Version(1, 0, 0);
                using (var dbStatement = new SQLiteCommand("""
                    SELECT Value
                    FROM db_file
                    WHERE ID = "date"
                """, _db))
                {
                    using var rows = dbStatement.ExecuteReader();
                    if (rows.Read())
                    {
                        date = Version.Parse(rows["Value"] as string);
                    }
                }

                #region 오프라인 기록
                if (Utility.IsLowerDate(date, 1, 16, 6))
                {
                    Ta(() =>
                    {
                        using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS tmp_comment (
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
					        Is_P INTEGER DEFAULT 0,
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
                            Put_Note_Set INTEGER DEFAULT 25,
                            Put_Note_Set_Millis REAL DEFAULT 0.0,
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
                            Input_Flags INTEGER DEFAULT 0,
					        CHECK (Multiplier >= 0.0)
					        CHECK (Auto_Mode IN(0, 1))
					        CHECK (Note_Salt_Mode IN (0, 1, 2, 4, 11, 13))
                            CHECK (Audio_Multiplier >= 0.5 AND Audio_Multiplier <= 2.0)
					        CHECK (Faint_Note_Mode IN (0, 1, 2, 3))
					        CHECK (Judgment_Mode IN (0, 1, 2, 3, 4, 5, 6))
					        CHECK (Hit_Points_Mode IN (0, 1, 2, 3, 4, 5, 6, 7))
					        CHECK (Note_Mobility_Mode IN (0, 1, 3, 4, 5))
					        CHECK (Long_Note_Mode IN (0, 1, 2, 3))
					        CHECK (Input_Favor_Mode IN (0, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16))
					        CHECK (Note_Modify_Mode IN (0, 1, 2))
					        CHECK (BPM_Mode IN (0, 1))
                            CHECK (Lowest_Judgment_Condition_Mode IN (0, 1))
					        CHECK (Wave_Mode IN (0, 1))
					        CHECK (Set_Note_Mode IN (0, 1, 3))
					        CHECK (Stand >= 0)
					        CHECK (Band >= 0)
					        CHECK (Is_P IN (0, 1))
					        CHECK (Point >= 0.0 AND Point <= 1.0)
					        CHECK (Lowest_Long_Note_Modify >= 1.0 AND Lowest_Long_Note_Modify <= 1000.0)
					        CHECK (Highest_Long_Note_Modify >= 1.0 AND Highest_Long_Note_Modify <= 1000.0)
					        CHECK (Put_Note_Set >= 1 AND Put_Note_Set <= 100)
					        CHECK (Put_Note_Set_Millis >= 0.0 AND Put_Note_Set_Millis <= 1000.0)
					        CHECK (Is_Paused IN (0, 1))
					        CHECK (Input_Flags >= 0 AND Input_Flags <= 15)
				        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (Utility.IsLowerDate(date, 1, 14, 65))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment
                                    SELECT *
                                    FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 105))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID)
                                        SELECT *
                                        FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand(@"UPDATE tmp_comment SET Highest_Long_Note_Modify = Lowest_Long_Note_Modify", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 115))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment
                                    SELECT *
                                    FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 117))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"DELETE
                                    FROM comment
                                    WHERE Note_Salt_Mode = 3 OR Note_Salt_Mode = 5 OR Note_Salt_Mode = 9 OR Note_Salt_Mode = 10 OR Note_Mobility_Mode = 2 OR Set_Note_Mode = 4 OR Set_Note_Mode = 5 OR Input_Favor_Mode = 1 OR Input_Favor_Mode = 2 OR Input_Favor_Mode = 3 OR Input_Favor_Mode = 17 OR Input_Favor_Mode = 18 OR Input_Favor_Mode = 19", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment
                                        SELECT *
                                        FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 123))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID)
                                        SELECT *
                                        FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 124))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID, Is_Paused)
                                        SELECT *
                                        FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SQLiteCommand(@"DELETE
                                    FROM comment
                                    WHERE Note_Salt_Mode = 6 OR Note_Salt_Mode = 7 OR Note_Salt_Mode = 8 OR Note_Salt_Mode = 12 OR Note_Salt_Mode = 14 OR Set_Note_Mode = 2", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand(@"INSERT
                                    INTO tmp_comment
                                        SELECT *
                                        FROM comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SQLiteCommand("DROP TABLE comment", _db))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        using (var dbStatement = new SQLiteCommand(@"ALTER TABLE tmp_comment
                            RENAME TO comment", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE INDEX IF NOT EXISTS _comment ON comment (
					        Note_ID,
                            Event_Note_ID
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                #region 디렉토리 내 노트 파일 번호
                using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS entry (
					Entry_Path VARCHAR(260),
					Note_Position INTEGER,
					PRIMARY KEY (Entry_Path)
				)", _db))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion

                #region 컬렉션
                if (Utility.IsLowerDate(date, 1, 14, 58))
                {
                    Ta(() =>
                    {
                        if (HasTable("note"))
                        {
                            using (var dbStatement = new SQLiteCommand("DROP TABLE note", _db))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS note (
				            Note_ID VARCHAR(139),
					        Favorite_Entry TEXT
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE INDEX IF NOT EXISTS _note ON note (
					        Note_ID
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                #region 클리어 램프
                if (Utility.IsLowerDate(date, 1, 14, 58))
                {
                    Ta(() =>
                    {
                        if (HasTable("handle"))
                        {
                            using (var dbStatement = new SQLiteCommand("DROP TABLE handle", _db))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS handle (
				            Note_ID VARCHAR(137),
                            Handled INTEGER,
                            PRIMARY KEY (Note_ID)
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                if (Utility.IsLowerDate(date, 1, 14, 63))
                {
                    var comments = new Dictionary<string, BaseNoteFile.Handled>();
                    using (var dbStatement = new SQLiteCommand(@"SELECT Note_ID, Is_P, Hit_Points_Mode
				        FROM comment", _db))
                    {
                        using var rows = dbStatement.ExecuteReader();
                        while (rows.Read())
                        {
                            var handledValue = comments.GetValueOrDefault(rows["Note_ID"] as string, BaseNoteFile.Handled.Not);
                            if (handledValue != BaseNoteFile.Handled.Band1)
                            {
                                if ((long)rows["Is_P"] > 0)
                                {
                                    handledValue = BaseNoteFile.Handled.Band1;
                                }
                                else if ((ModeComponent.HitPointsMode)(int)(long)rows["Hit_Points_Mode"] == ModeComponent.HitPointsMode.Highest)
                                {
                                    handledValue = BaseNoteFile.Handled.HighestClear;
                                }
                                else if ((ModeComponent.HitPointsMode)(int)(long)rows["Hit_Points_Mode"] == ModeComponent.HitPointsMode.Higher && handledValue != BaseNoteFile.Handled.HighestClear)
                                {
                                    handledValue = BaseNoteFile.Handled.HigherClear;
                                }
                                else if (handledValue != BaseNoteFile.Handled.HigherClear && handledValue != BaseNoteFile.Handled.HighestClear)
                                {
                                    handledValue = BaseNoteFile.Handled.Clear;
                                }
                            }
                            comments[rows["Note_ID"] as string] = handledValue;
                        }
                    }
                    foreach (var comment in comments)
                    {
                        using (var dbStatement = new SQLiteCommand(@"REPLACE
                            INTO handle
				            VALUES(@noteID, @handled)", _db))
                        {
                            dbStatement.Parameters.AddWithValue("noteID", comment.Key);
                            dbStatement.Parameters.AddWithValue("handled", comment.Value);
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                }

                #region 클리어 시간들 (플레이 카운트, 마지막 플레이 정렬)
                if (Utility.IsLowerDate(date, 1, 14, 58))
                {
                    Ta(() =>
                    {
                        if (HasTable("date"))
                        {
                            using (var dbStatement = new SQLiteCommand("DROP TABLE date", _db))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS date (
				            Note_ID VARCHAR(139),
					        Event_Note_ID TEXT,
				            Date DATE
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE INDEX IF NOT EXISTS _date ON date (
					        Note_ID,
                            Event_Note_ID
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                if (Utility.IsLowerDate(date, 1, 14, 60))
                {
                    var comments = new List<(string, string, DateTime)>();
                    using (var dbStatement = new SQLiteCommand(@"SELECT Note_ID, Event_Note_ID, Date
				        FROM comment", _db))
                    {
                        using var rows = dbStatement.ExecuteReader();
                        while (rows.Read())
                        {
                            comments.Add((rows["Note_ID"] as string, rows["Event_Note_ID"] as string, (DateTime)rows["Date"]));
                        }
                    }
                    foreach (var comment in comments)
                    {
                        using (var dbStatement = new SQLiteCommand(@"INSERT
                            INTO date
				            VALUES(@noteID, @eventNoteID, @date)", _db))
                        {
                            dbStatement.Parameters.AddWithValue("noteID", comment.Item1);
                            dbStatement.Parameters.AddWithValue("eventNoteID", comment.Item2);
                            dbStatement.Parameters.AddWithValue("date", comment.Item3);
                            dbStatement.ExecuteNonQuery();
                        }
                    }
                }

                #region 레이턴시, 동영상
                if (Utility.IsLowerDate(date, 1, 14, 58))
                {
                    Ta(() =>
                    {
                        if (HasTable("wait"))
                        {
                            using (var dbStatement = new SQLiteCommand("DROP TABLE wait", _db))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS wait (
				            Note_ID VARCHAR(139),
					        Audio_Wait REAL,
					        Media_Wait REAL DEFAULT 0,
					        Media LONG DEFAULT 1,
						    CHECK (Audio_Wait >= -1000.0 AND Audio_Wait <= 1000.0)
						    CHECK (Media_Wait >= -1000.0 AND Media_Wait <= 1000.0)
					        CHECK (Media IN (0, 1))
					        PRIMARY KEY (Note_ID)
				        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                #region 텍스트 인코딩
                if (Utility.IsLowerDate(date, 1, 14, 58))
                {
                    Ta(() =>
                    {
                        if (HasTable("format"))
                        {
                            using (var dbStatement = new SQLiteCommand("DROP TABLE format", _db))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS format (
				            Note_ID VARCHAR(139),
                            Format LONG DEFAULT -1,
                            PRIMARY KEY (Note_ID)
                        )", _db))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                #region 코스
                using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS event_note (
                    Event_Note_ID TEXT,
				    Name TEXT,
				    Date DATE,
				    Variety INTEGER DEFAULT 0,
                    PRIMARY KEY (Event_Note_ID, Variety)
					CHECK (length(Event_Note_ID) > 0)
					CHECK (Variety IN (0, 1))
                )", _db))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion

                #region 코스에 사용되는 임시 데이터
                using (var dbStatement = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS event_note_data (
                    Note_ID VARCHAR(139),
                    Note_Variety INTEGER,
                    Title TEXT,
                    Artist TEXT,
                    Level INTEGER,
                    Level_Text TEXT,
                    Genre TEXT,
                    PRIMARY KEY (Note_ID)
                    )", _db))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion

                using (var dbStatement = new SQLiteCommand("""
                    REPLACE
                    INTO db_file
                    VALUES("date", @value)
                """, _db))
                {
                    dbStatement.Parameters.AddWithValue("value", QwilightComponent.DateText);
                    dbStatement.ExecuteNonQuery();
                }

                bool HasTable(string tableName)
                {
                    using var dbStatement = new SQLiteCommand(@"SELECT name
                        FROM sqlite_master
                        WHERE type = 'table' AND name = @tableName", _db);
                    dbStatement.Parameters.AddWithValue("tableName", tableName);
                    using var rows = dbStatement.ExecuteReader();
                    return rows.Read();
                }

                void Ta(Action onHandle)
                {
                    using var t = _db.BeginTransaction();
                    try
                    {
                        onHandle();
                        t.Commit();
                    }
                    catch
                    {
                        t.Rollback();
                        throw;
                    }
                }
            }
        }

        public async ValueTask<ICollection<CommentItem>> GetCommentItems(BaseNoteFile noteFile, string eventNoteID, int noteFileCount)
        {
            var data = new List<CommentItem>();
            using var dbStatement = new SQLiteCommand($"""
                SELECT Date, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Is_Paused, Input_Flags
                FROM comment
                WHERE {(string.IsNullOrEmpty(eventNoteID) ? "Note_ID = @noteID" : "Event_Note_ID = @eventNoteID")}
                ORDER BY Stand DESC
            """, _db);
            if (string.IsNullOrEmpty(eventNoteID))
            {
                dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            }
            else
            {
                dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            }
            using var rows = await dbStatement.ExecuteReaderAsync();
            while (await rows.ReadAsync())
            {
                var date = (DateTime)rows["Date"];
                var sentMultiplier = (double)rows["Multiplier"];
                var audioMultiplier = Math.Round((double)rows["Audio_Multiplier"], 2);
                data.Add(new(string.Empty, (DefaultCompute.InputFlag)(long)rows["Input_Flags"])
                {
                    NoteFileCount = noteFileCount,
                    Date = date,
                    DateText = date.ToString("yyyy-MM-dd HH:mm:ss"),
                    CommentID = rows["Comment"] as string,
                    AvatarName = rows["Name"] as string,
                    ModeComponentValue = new()
                    {
                        CanModifyMultiplier = false,
                        CanModifyAudioMultiplier = false,
                        ComputingValue = noteFile,
                        SentMultiplier = sentMultiplier,
                        MultiplierValue = noteFile.BPM * audioMultiplier * sentMultiplier,
                        AutoModeValue = (ModeComponent.AutoMode)(long)rows["Auto_Mode"],
                        NoteSaltModeValue = (ModeComponent.NoteSaltMode)(long)rows["Note_Salt_Mode"],
                        AudioMultiplier = audioMultiplier,
                        FaintNoteModeValue = (ModeComponent.FaintNoteMode)(long)rows["Faint_Note_Mode"],
                        JudgmentModeValue = (ModeComponent.JudgmentMode)(long)rows["Judgment_Mode"],
                        HitPointsModeValue = (ModeComponent.HitPointsMode)(long)rows["Hit_Points_Mode"],
                        NoteMobilityModeValue = (ModeComponent.NoteMobilityMode)(long)rows["Note_Mobility_Mode"],
                        LongNoteModeValue = (ModeComponent.LongNoteMode)(long)rows["Long_Note_Mode"],
                        InputFavorModeValue = (ModeComponent.InputFavorMode)(long)rows["Input_Favor_Mode"],
                        NoteModifyModeValue = (ModeComponent.NoteModifyMode)(long)rows["Note_Modify_Mode"],
                        BPMModeValue = (ModeComponent.BPMMode)(long)rows["BPM_Mode"],
                        WaveModeValue = (ModeComponent.WaveMode)(long)rows["Wave_Mode"],
                        SetNoteModeValue = (ModeComponent.SetNoteMode)(long)rows["Set_Note_Mode"],
                        LowestJudgmentConditionModeValue = (ModeComponent.LowestJudgmentConditionMode)(long)rows["Lowest_Judgment_Condition_Mode"],
                        Salt = (int)(long)rows["Salt"],
                        HighestJudgment0 = (double)rows["Highest_Judgment_0"],
                        HigherJudgment0 = (double)rows["Higher_Judgment_0"],
                        HighJudgment0 = (double)rows["High_Judgment_0"],
                        LowJudgment0 = (double)rows["Low_Judgment_0"],
                        LowerJudgment0 = (double)rows["Lower_Judgment_0"],
                        LowestJudgment0 = (double)rows["Lowest_Judgment_0"],
                        HighestJudgment1 = (double)rows["Highest_Judgment_1"],
                        HigherJudgment1 = (double)rows["Higher_Judgment_1"],
                        HighJudgment1 = (double)rows["High_Judgment_1"],
                        LowJudgment1 = (double)rows["Low_Judgment_1"],
                        LowerJudgment1 = (double)rows["Lower_Judgment_1"],
                        LowestJudgment1 = (double)rows["Lowest_Judgment_1"],
                        LowestLongNoteModify = (double)rows["Lowest_Long_Note_Modify"],
                        HighestLongNoteModify = (double)rows["Highest_Long_Note_Modify"],
                        PutNoteSet = (int)(long)rows["Put_Note_Set"],
                        PutNoteSetMillis = (double)rows["Put_Note_Set_Millis"],
                        HighestHitPoints0 = 100.0 * (double)rows["Highest_Hit_Points_0"],
                        HigherHitPoints0 = 100.0 * (double)rows["Higher_Hit_Points_0"],
                        HighHitPoints0 = 100.0 * (double)rows["High_Hit_Points_0"],
                        LowHitPoints0 = 100.0 * (double)rows["Low_Hit_Points_0"],
                        LowerHitPoints0 = 100.0 * (double)rows["Lower_Hit_Points_0"],
                        LowestHitPoints0 = 100.0 * (double)rows["Lowest_Hit_Points_0"],
                        HighestHitPoints1 = 100.0 * (double)rows["Highest_Hit_Points_1"],
                        HigherHitPoints1 = 100.0 * (double)rows["Higher_Hit_Points_1"],
                        HighHitPoints1 = 100.0 * (double)rows["High_Hit_Points_1"],
                        LowHitPoints1 = 100.0 * (double)rows["Low_Hit_Points_1"],
                        LowerHitPoints1 = 100.0 * (double)rows["Lower_Hit_Points_1"],
                        LowestHitPoints1 = 100.0 * (double)rows["Lowest_Hit_Points_1"],
                    },
                    Stand = (int)(long)rows["Stand"],
                    Band = (int)(long)rows["Band"],
                    IsP = (long)rows["Is_P"] > 0,
                    Point = (double)rows["Point"],
                    IsPaused = (long)rows["Is_Paused"] > 0
                });
            }
            for (var i = data.Count - 1; i >= 0; --i)
            {
                data[i].CommentPlace0Text = $"＃{i + 1}";
                data[i].CommentPlace1Text = $"／{data.Count}";
            }
            return data;
        }

        public async ValueTask SetEventNoteData(ICollection<WwwLevelGroup.WwwLevelComputing> wwwLevelComputingValues)
        {
            foreach (var wwwLevelComputingValue in wwwLevelComputingValues)
            {
                var noteVariety = (BaseNoteFile.NoteVariety)wwwLevelComputingValue.NoteVarietyValue;
                if (noteVariety != BaseNoteFile.NoteVariety.EventNote)
                {
                    using var dbStatement = new SQLiteCommand(@"REPLACE
                        INTO event_note_data
                        VALUES(@noteID, @noteVariety, @title, @artist, @level, @levelText, @genre)", _db);
                    dbStatement.Parameters.AddWithValue("noteID", wwwLevelComputingValue.NoteID);
                    dbStatement.Parameters.AddWithValue("noteVariety", noteVariety);
                    dbStatement.Parameters.AddWithValue("title", wwwLevelComputingValue.Title);
                    dbStatement.Parameters.AddWithValue("artist", wwwLevelComputingValue.Artist);
                    dbStatement.Parameters.AddWithValue("level", wwwLevelComputingValue.LevelValue);
                    dbStatement.Parameters.AddWithValue("levelText", wwwLevelComputingValue.LevelText);
                    dbStatement.Parameters.AddWithValue("genre", wwwLevelComputingValue.Genre);
                    await dbStatement.ExecuteNonQueryAsync();
                }
            }
        }

        public void GetEventNoteData(string noteID, NotAvailableNoteFile noteFile)
        {
            using var dbStatement = new SQLiteCommand(@"SELECT Note_Variety, Title, Artist, Level, Level_Text, Genre
                FROM event_note_data
                WHERE Note_ID = @noteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteID);
            using var rows = dbStatement.ExecuteReader();
            if (rows.Read())
            {
                noteFile.NotAvailableNoteVarietyValue = (BaseNoteFile.NoteVariety)(long)rows["Note_Variety"];
                if (rows["Title"] != DBNull.Value)
                {
                    noteFile.Title = rows["Title"] as string;
                }
                if (rows["Artist"] != DBNull.Value)
                {
                    noteFile.Artist = rows["Artist"] as string;
                }
                noteFile.LevelValue = (BaseNoteFile.Level)(long)rows["Level"];
                if (rows["Level_Text"] != DBNull.Value)
                {
                    noteFile.LevelText = rows["Level_Text"] as string;
                }
                if (rows["Genre"] != DBNull.Value)
                {
                    noteFile.Genre = rows["Genre"] as string;
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
            using var dbStatement = new SQLiteCommand(@"SELECT Favorite_Entry
                FROM note
                WHERE Note_ID = @noteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            var favoriteEntryItems = new List<DefaultEntryItem>();
            while (rows.Read())
            {
                favoriteEntryItems.Add(new DefaultEntryItem
                {
                    DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Favorite,
                    DefaultEntryPath = rows["Favorite_Entry"] as string
                });
            }
            return favoriteEntryItems;
        }

        public ICollection<(string, string, DateTime, EventNoteVariety)> GetEventNote()
        {
            using var dbStatement = new SQLiteCommand(@"SELECT Event_Note_ID, Name, Date, Variety
                FROM event_note", _db);
            using var rows = dbStatement.ExecuteReader();
            var eventNote = new List<(string, string, DateTime, EventNoteVariety)>();
            while (rows.Read())
            {
                eventNote.Add((rows["Event_Note_ID"] as string, rows["Name"] as string, (DateTime)rows["Date"], (EventNoteVariety)(long)rows["Variety"]));
            }
            return eventNote;
        }

        public BaseNoteFile.Handled GetHandled(BaseNoteFile noteFile)
        {
            using var dbStatement = new SQLiteCommand(@"SELECT Handled
                FROM handle
                WHERE Note_ID = @noteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? (BaseNoteFile.Handled)(long)rows["Handled"] : BaseNoteFile.Handled.Not;
        }

        public async void SetHandled(BaseNoteFile noteFile)
        {
            using var dbStatement = new SQLiteCommand(@"REPLACE
                INTO handle
                VALUES(@noteID, @handled)", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("handled", noteFile.HandledValue);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public (DateTime?, int) GetDate(BaseNoteFile noteFile, string eventNoteID)
        {
            using var dbStatement = new SQLiteCommand(@"SELECT MAX(Date) AS Latest, COUNT(Date) AS Count
                FROM date
                WHERE Note_ID = @noteID OR Event_Note_ID = @eventNoteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512());
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            using var rows = dbStatement.ExecuteReader();
            rows.Read();
            var date = (null as DateTime?, 0);
            if (rows["Latest"] != DBNull.Value)
            {
                date.Item1 = DateTime.Parse((string)rows["Latest"]);
            }
            if (rows["Count"] != DBNull.Value)
            {
                date.Item2 = (int)(long)rows["Count"];
            }
            return date;
        }

        public async void NewDate(BaseNoteFile noteFile, string eventNoteID, DateTime date)
        {
            using var dbStatement = new SQLiteCommand(@"INSERT
                INTO date
                VALUES(@noteID, @eventNoteID, @date)", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512());
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.Parameters.AddWithValue("date", date);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public int GetNotePosition(string entryPath)
        {
            using var dbStatement = new SQLiteCommand(@"SELECT Note_Position
                FROM entry
                WHERE Entry_Path = @entryPath", _db);
            dbStatement.Parameters.AddWithValue("entryPath", entryPath);
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? (int)(long)rows["Note_Position"] : 0;
        }

        public (double, double, bool?) GetWait(BaseNoteFile noteFile)
        {
            using var dbStatement = new SQLiteCommand(@"SELECT Audio_Wait, Media_Wait, Media
                FROM wait
                WHERE Note_ID = @noteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            (double, double, bool?) data = (0.0, 0.0, default);
            if (rows.Read())
            {
                if (rows["Audio_Wait"] != DBNull.Value)
                {
                    data.Item1 = (double)rows["Audio_Wait"];
                }
                data.Item2 = (double)rows["Media_Wait"];
                if (rows["Media"] != DBNull.Value)
                {
                    data.Item3 = (long)rows["Media"] > 0;
                }
            }
            return data;
        }

        public int GetFormat(BaseNoteFile noteFile)
        {
            using var dbStatement = new SQLiteCommand(@"SELECT Format
                FROM format
                WHERE Note_ID = @noteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? (int)(long)rows["Format"] : -1;
        }

        public async void SetNotePosition(EntryItem entryItem)
        {
            using var dbStatement = new SQLiteCommand(@"REPLACE
                INTO entry
                VALUES(@entryPath, @notePosition)", _db);
            dbStatement.Parameters.AddWithValue("entryPath", entryItem.EntryPath);
            dbStatement.Parameters.AddWithValue("notePosition", entryItem.NotePosition);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async void SetFavoriteEntry(BaseNoteFile noteFile)
        {
            await Ta(async () =>
            {
                using (var dbStatement = new SQLiteCommand(@"DELETE
                    FROM note
                    WHERE Note_ID = @noteID", _db))
                {
                    dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
                    await dbStatement.ExecuteNonQueryAsync();
                }
                using (var dbStatement = new SQLiteCommand(@"REPLACE
                    INTO note
                    VALUES(@noteID, @favoriteEntry)", _db))
                {
                    foreach (var favoriteEntryItem in noteFile.FavoriteEntryItems)
                    {
                        dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
                        dbStatement.Parameters.AddWithValue("favoriteEntry", favoriteEntryItem.DefaultEntryPath);
                        await dbStatement.ExecuteNonQueryAsync();
                    }
                }
            });
        }

        public async Task SetEventNote(string eventNoteID, string eventNoteName, DateTime date, EventNoteVariety eventNoteVariety)
        {
            using var dbStatement = new SQLiteCommand(@"INSERT INTO event_note
                VALUES(@eventNoteID, @eventNoteName, @date, @eventNoteVariety)", _db);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.Parameters.AddWithValue("eventNoteName", eventNoteName);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.Parameters.AddWithValue("eventNoteVariety", eventNoteVariety);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask WipeEventNote(string eventNoteID)
        {
            using var dbStatement = new SQLiteCommand(@"DELETE
                FROM event_note
                WHERE Event_Note_ID = @eventNoteID", _db);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask ModifyEventNoteName(string eventNoteID, string eventNoteName)
        {
            using var dbStatement = new SQLiteCommand(@"UPDATE event_note
                SET Name = @eventNoteName
                WHERE Event_Note_ID = @eventNoteID", _db);
            dbStatement.Parameters.AddWithValue("eventNoteName", eventNoteName);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async void SaveComment(DateTime date, BaseNoteFile noteFile, string eventNoteID, string comment, string avatar, double multiplier, double audioMultiplier, ModeComponent modeComponentValue, int stand, int band, bool isP, double point, bool isPaused, DefaultCompute.InputFlag inputFlags)
        {
            using var dbStatement = new SQLiteCommand(@"INSERT
                INTO comment
                VALUES(@date, @eventNoteID, @comment, @avatar, @multiplier, @autoMode, @noteSaltMode, @audioMultiplier, @faintNoteMode, @judgmentMode, @hitPointsMode, @noteMobilityMode, @longNoteMode, @inputFavorMode, @noteModifyMode, @bpmMode, @waveMode, @setNoteMode, @lowestJudgmentConditionMode, @stand, @band, @isP, @point, @salt, @highestJudgment0, @higherJudgment0, @highJudgment0, @lowJudgment0, @lowerJudgment0, @lowestJudgment0, @highestJudgment1, @higherJudgment1, @highJudgment1, @lowJudgment1, @lowerJudgment1, @lowestJudgment1, @lowestLongNoteModify, @highestLongNoteModify, @putNoteSet, @putNoteSetMillis, @highestHitPoints0, @higherHitPoints0, @highHitPoints0, @lowHitPoints0, @lowerHitPoints0, @lowestHitPoints0, @highestHitPoints1, @higherHitPoints1, @highHitPoints1, @lowHitPoints1, @lowerHitPoints1, @lowestHitPoints1, @noteID, @isPaused, @inputFlags)", _db);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.Parameters.AddWithValue("comment", comment);
            dbStatement.Parameters.AddWithValue("avatar", avatar);
            dbStatement.Parameters.AddWithValue("multiplier", multiplier);
            dbStatement.Parameters.AddWithValue("autoMode", modeComponentValue.AutoModeValue);
            dbStatement.Parameters.AddWithValue("noteSaltMode", modeComponentValue.NoteSaltModeValue);
            dbStatement.Parameters.AddWithValue("audioMultiplier", audioMultiplier);
            dbStatement.Parameters.AddWithValue("faintNoteMode", modeComponentValue.FaintNoteModeValue);
            dbStatement.Parameters.AddWithValue("judgmentMode", modeComponentValue.JudgmentModeValue);
            dbStatement.Parameters.AddWithValue("hitPointsMode", modeComponentValue.HandlingHitPointsModeValue);
            dbStatement.Parameters.AddWithValue("noteMobilityMode", modeComponentValue.NoteMobilityModeValue);
            dbStatement.Parameters.AddWithValue("longNoteMode", modeComponentValue.LongNoteModeValue);
            dbStatement.Parameters.AddWithValue("inputFavorMode", modeComponentValue.InputFavorModeValue);
            dbStatement.Parameters.AddWithValue("noteModifyMode", modeComponentValue.NoteModifyModeValue);
            dbStatement.Parameters.AddWithValue("bpmMode", modeComponentValue.BPMModeValue);
            dbStatement.Parameters.AddWithValue("waveMode", modeComponentValue.WaveModeValue);
            dbStatement.Parameters.AddWithValue("setNoteMode", modeComponentValue.SetNoteModeValue);
            dbStatement.Parameters.AddWithValue("lowestJudgmentConditionMode", modeComponentValue.LowestJudgmentConditionModeValue);
            dbStatement.Parameters.AddWithValue("stand", stand);
            dbStatement.Parameters.AddWithValue("band", band);
            dbStatement.Parameters.AddWithValue("isP", isP);
            dbStatement.Parameters.AddWithValue("point", point);
            dbStatement.Parameters.AddWithValue("salt", modeComponentValue.Salt);
            dbStatement.Parameters.AddWithValue("highestJudgment0", modeComponentValue.HighestJudgment0);
            dbStatement.Parameters.AddWithValue("higherJudgment0", modeComponentValue.HigherJudgment0);
            dbStatement.Parameters.AddWithValue("highJudgment0", modeComponentValue.HighJudgment0);
            dbStatement.Parameters.AddWithValue("lowJudgment0", modeComponentValue.LowJudgment0);
            dbStatement.Parameters.AddWithValue("lowerJudgment0", modeComponentValue.LowerJudgment0);
            dbStatement.Parameters.AddWithValue("lowestJudgment0", modeComponentValue.LowestJudgment0);
            dbStatement.Parameters.AddWithValue("highestJudgment1", modeComponentValue.HighestJudgment1);
            dbStatement.Parameters.AddWithValue("higherJudgment1", modeComponentValue.HigherJudgment1);
            dbStatement.Parameters.AddWithValue("highJudgment1", modeComponentValue.HighJudgment1);
            dbStatement.Parameters.AddWithValue("lowJudgment1", modeComponentValue.LowJudgment1);
            dbStatement.Parameters.AddWithValue("lowerJudgment1", modeComponentValue.LowerJudgment1);
            dbStatement.Parameters.AddWithValue("lowestJudgment1", modeComponentValue.LowestJudgment1);
            dbStatement.Parameters.AddWithValue("lowestLongNoteModify", modeComponentValue.LowestLongNoteModify);
            dbStatement.Parameters.AddWithValue("highestLongNoteModify", modeComponentValue.HighestLongNoteModify);
            dbStatement.Parameters.AddWithValue("putNoteSet", modeComponentValue.PutNoteSet);
            dbStatement.Parameters.AddWithValue("putNoteSetMillis", modeComponentValue.PutNoteSetMillis);
            dbStatement.Parameters.AddWithValue("highestHitPoints0", modeComponentValue.HighestHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("higherHitPoints0", modeComponentValue.HigherHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("highHitPoints0", modeComponentValue.HighHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("lowHitPoints0", modeComponentValue.LowHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("lowerHitPoints0", modeComponentValue.LowerHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("lowestHitPoints0", modeComponentValue.LowestHitPoints0 / 100.0);
            dbStatement.Parameters.AddWithValue("highestHitPoints1", modeComponentValue.HighestHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("higherHitPoints1", modeComponentValue.HigherHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("highHitPoints1", modeComponentValue.HighHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("lowHitPoints1", modeComponentValue.LowHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("lowerHitPoints1", modeComponentValue.LowerHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("lowestHitPoints1", modeComponentValue.LowestHitPoints1 / 100.0);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512());
            dbStatement.Parameters.AddWithValue("isPaused", isPaused);
            dbStatement.Parameters.AddWithValue("inputFlags", inputFlags);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async void SetWait(BaseNoteFile noteFile, double audioWait, double mediaWait, bool media)
        {
            using var dbStatement = new SQLiteCommand(@"REPLACE
                INTO wait
                VALUES(@noteID, @audioWait, @mediaWait, @media)", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("audioWait", audioWait);
            dbStatement.Parameters.AddWithValue("mediaWait", mediaWait);
            dbStatement.Parameters.AddWithValue("media", media);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask SetNoteFormat(BaseNoteFile noteFile, int format)
        {
            using var dbStatement = new SQLiteCommand(@"REPLACE
                INTO format
                VALUES(@noteID, @format)", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("format", format);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask WipeFavoriteEntry()
        {
            using var dbStatement = new SQLiteCommand(@"DELETE
                FROM note", _db);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask WipeHandled(BaseNoteFile noteFile)
        {
            using var dbStatement = new SQLiteCommand(@"DELETE
                FROM handle
                WHERE Note_ID = @noteID", _db);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask InitWait()
        {
            using var dbStatement = new SQLiteCommand(@"UPDATE wait
                SET Audio_Wait = NULL, Media_Wait = 0.0", _db);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask InitMedia()
        {
            using var dbStatement = new SQLiteCommand(@"UPDATE wait
                SET Media = NULL", _db);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask WipeComment(string comment)
        {
            using var dbStatement = new SQLiteCommand(@"DELETE
                FROM comment
                WHERE Comment = @comment", _db);
            dbStatement.Parameters.AddWithValue("comment", comment);
            await dbStatement.ExecuteNonQueryAsync();
        }

        public async ValueTask WipeComment()
        {
            using var dbStatement = new SQLiteCommand(@"DELETE
                FROM comment", _db);
            await dbStatement.ExecuteNonQueryAsync();
        }

        async ValueTask Ta(Action onHandle)
        {
            using var t = await _db.BeginTransactionAsync();
            try
            {
                onHandle();
                await t.CommitAsync();
            }
            catch
            {
                await t.RollbackAsync();
                throw;
            }
        }
    }
}