﻿using Microsoft.Data.Sqlite;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Data;
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
        static readonly string _tmp0FileName = Path.ChangeExtension(_fileName, ".db.tmp.0");
        static readonly string _tmp1FileName = Path.ChangeExtension(_fileName, ".db.tmp.1");

        public static readonly DB Instance = QwilightComponent.GetBuiltInData<DB>(nameof(DB));

        SqliteConnection _fastDB;
        SqliteConnection _fileDB;

        public string DBFault { get; set; }

        public void Load()
        {
            Utility.WipeFile(_tmp0FileName);
            Utility.MoveFile(_tmp1FileName, _fileName);
            _fastDB = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.Memory
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
                _fastDB.Close();
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

                #region 데이터베이스 정보
                using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS db_file (
					ID VARCHAR(137),
					Value TEXT,
					PRIMARY KEY (ID)
				)", _fastDB))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion
                var date = new Version(1, 0, 0);
                using (var dbStatement = new SqliteCommand("""
                    SELECT Value
                    FROM db_file
                    WHERE ID = "date"
                """, _fastDB))
                {
                    using var rows = dbStatement.ExecuteReader();
                    if (rows.Read())
                    {
                        date = Version.Parse(rows.GetString("Value"));
                    }
                }

                #region 오프라인 기록
                if (Utility.IsLowerDate(date, 1, 16, 6))
                {
                    Ta(() =>
                    {
                        using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS tmp_comment (
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
				        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        if (Utility.IsLowerDate(date, 1, 14, 65))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment
                                    SELECT *
                                    FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 105))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID)
                                        SELECT *
                                        FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand(@"UPDATE tmp_comment SET Highest_Long_Note_Modify = Lowest_Long_Note_Modify", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 115))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment
                                    SELECT *
                                    FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 117))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"DELETE
                                    FROM comment
                                    WHERE Note_Salt_Mode = 3 OR Note_Salt_Mode = 5 OR Note_Salt_Mode = 9 OR Note_Salt_Mode = 10 OR Note_Mobility_Mode = 2 OR Set_Note_Mode = 4 OR Set_Note_Mode = 5 OR Input_Favor_Mode = 1 OR Input_Favor_Mode = 2 OR Input_Favor_Mode = 3 OR Input_Favor_Mode = 17 OR Input_Favor_Mode = 18 OR Input_Favor_Mode = 19", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment
                                        SELECT *
                                        FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 123))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID)
                                        SELECT *
                                        FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else if (Utility.IsLowerDate(date, 1, 14, 124))
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment(Date, Event_Note_ID, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Note_ID, Is_Paused)
                                        SELECT *
                                        FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            if (HasTable("comment"))
                            {
                                using (var dbStatement = new SqliteCommand(@"DELETE
                                    FROM comment
                                    WHERE Note_Salt_Mode = 6 OR Note_Salt_Mode = 7 OR Note_Salt_Mode = 8 OR Note_Salt_Mode = 12 OR Note_Salt_Mode = 14 OR Set_Note_Mode = 2", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand(@"INSERT
                                    INTO tmp_comment
                                        SELECT *
                                        FROM comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                                using (var dbStatement = new SqliteCommand("DROP TABLE comment", _fastDB))
                                {
                                    dbStatement.ExecuteNonQuery();
                                }
                            }
                        }
                        using (var dbStatement = new SqliteCommand(@"ALTER TABLE tmp_comment
                            RENAME TO comment", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE INDEX IF NOT EXISTS _comment ON comment (
					        Note_ID,
                            Event_Note_ID
                        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                #region 디렉토리 내 노트 파일 번호
                using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS entry (
					Entry_Path VARCHAR(260),
					Note_Position INTEGER,
					PRIMARY KEY (Entry_Path)
				)", _fastDB))
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
                            using (var dbStatement = new SqliteCommand("DROP TABLE note", _fastDB))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS note (
				            Note_ID VARCHAR(139),
					        Favorite_Entry TEXT
                        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE INDEX IF NOT EXISTS _note ON note (
					        Note_ID
                        )", _fastDB))
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
                            using (var dbStatement = new SqliteCommand("DROP TABLE handle", _fastDB))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS handle (
				            Note_ID VARCHAR(137),
                            Handled INTEGER,
                            PRIMARY KEY (Note_ID)
                        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                if (Utility.IsLowerDate(date, 1, 14, 63))
                {
                    var comments = new Dictionary<string, BaseNoteFile.Handled>();
                    using (var dbStatement = new SqliteCommand(@"SELECT Note_ID, Is_P, Hit_Points_Mode
				        FROM comment", _fastDB))
                    {
                        using var rows = dbStatement.ExecuteReader();
                        while (rows.Read())
                        {
                            var handledValue = comments.GetValueOrDefault(rows.GetString("Note_ID"), BaseNoteFile.Handled.Not);
                            if (handledValue != BaseNoteFile.Handled.Band1)
                            {
                                if (rows.GetBoolean("Is_P"))
                                {
                                    handledValue = BaseNoteFile.Handled.Band1;
                                }
                                else if ((ModeComponent.HitPointsMode)rows.GetInt32("Hit_Points_Mode") == ModeComponent.HitPointsMode.Highest)
                                {
                                    handledValue = BaseNoteFile.Handled.HighestClear;
                                }
                                else if ((ModeComponent.HitPointsMode)rows.GetInt32("Hit_Points_Mode") == ModeComponent.HitPointsMode.Higher && handledValue != BaseNoteFile.Handled.HighestClear)
                                {
                                    handledValue = BaseNoteFile.Handled.HigherClear;
                                }
                                else if (handledValue != BaseNoteFile.Handled.HigherClear && handledValue != BaseNoteFile.Handled.HighestClear)
                                {
                                    handledValue = BaseNoteFile.Handled.Clear;
                                }
                            }
                            comments[rows.GetString("Note_ID")] = handledValue;
                        }
                    }
                    foreach (var comment in comments)
                    {
                        using (var dbStatement = new SqliteCommand(@"REPLACE
                            INTO handle
				            VALUES(@noteID, @handled)", _fastDB))
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
                            using (var dbStatement = new SqliteCommand("DROP TABLE date", _fastDB))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS date (
				            Note_ID VARCHAR(139),
					        Event_Note_ID TEXT,
				            Date DATE
                        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE INDEX IF NOT EXISTS _date ON date (
					        Note_ID,
                            Event_Note_ID
                        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                if (Utility.IsLowerDate(date, 1, 14, 60))
                {
                    var comments = new List<(string, string, DateTime)>();
                    using (var dbStatement = new SqliteCommand(@"SELECT Note_ID, Event_Note_ID, Date
				        FROM comment", _fastDB))
                    {
                        using var rows = dbStatement.ExecuteReader();
                        while (rows.Read())
                        {
                            comments.Add((rows.GetString("Note_ID"), rows.GetString("Event_Note_ID"), rows.GetDateTime("Date")));
                        }
                    }
                    foreach (var comment in comments)
                    {
                        using (var dbStatement = new SqliteCommand(@"INSERT
                            INTO date
				            VALUES(@noteID, @eventNoteID, @date)", _fastDB))
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
                            using (var dbStatement = new SqliteCommand("DROP TABLE wait", _fastDB))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS wait (
				            Note_ID VARCHAR(139),
					        Audio_Wait REAL,
					        Media_Wait REAL DEFAULT 0,
					        Media LONG DEFAULT 1,
						    CHECK (Audio_Wait >= -1000.0 AND Audio_Wait <= 1000.0)
						    CHECK (Media_Wait >= -1000.0 AND Media_Wait <= 1000.0)
					        CHECK (Media IN (0, 1))
					        PRIMARY KEY (Note_ID)
				        )", _fastDB))
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
                            using (var dbStatement = new SqliteCommand("DROP TABLE format", _fastDB))
                            {
                                dbStatement.ExecuteNonQuery();
                            }
                        }
                        using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS format (
				            Note_ID VARCHAR(139),
                            Format LONG DEFAULT -1,
                            PRIMARY KEY (Note_ID)
                        )", _fastDB))
                        {
                            dbStatement.ExecuteNonQuery();
                        }
                    });
                }
                #endregion

                #region 코스
                using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS event_note (
                    Event_Note_ID TEXT,
				    Name TEXT,
				    Date DATE,
				    Variety INTEGER DEFAULT 0,
                    PRIMARY KEY (Event_Note_ID, Variety)
					CHECK (length(Event_Note_ID) > 0)
					CHECK (Variety IN (0, 1))
                )", _fastDB))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion

                #region 코스에 사용되는 임시 데이터
                using (var dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS event_note_data (
                    Note_ID VARCHAR(139),
                    Note_Variety INTEGER,
                    Title TEXT,
                    Artist TEXT,
                    Level INTEGER,
                    Level_Text TEXT,
                    Genre TEXT,
                    PRIMARY KEY (Note_ID)
                    )", _fastDB))
                {
                    dbStatement.ExecuteNonQuery();
                }
                #endregion

                using (var dbStatement = new SqliteCommand("""
                    REPLACE
                    INTO db_file
                    VALUES("date", @value)
                """, _fastDB))
                {
                    dbStatement.Parameters.AddWithValue("value", QwilightComponent.DateText);
                    dbStatement.ExecuteNonQuery();
                }

                bool HasTable(string tableName)
                {
                    var dbStatement = new SqliteCommand(@"SELECT name
                        FROM sqlite_master
                        WHERE type = 'table' AND name = @tableName", _fastDB);
                    dbStatement.Parameters.AddWithValue("tableName", tableName);
                    using var rows = dbStatement.ExecuteReader();
                    return rows.Read();
                }

                void Ta(Action onHandle)
                {
                    using var t = _fastDB.BeginTransaction();
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

        public ICollection<CommentItem> GetCommentItems(BaseNoteFile noteFile, string eventNoteID, int noteFileCount)
        {
            var data = new List<CommentItem>();
            var dbStatement = new SqliteCommand($"""
                SELECT Date, Comment, Name, Multiplier, Auto_Mode, Note_Salt_Mode, Audio_Multiplier, Faint_Note_Mode, Judgment_Mode, Hit_Points_Mode, Note_Mobility_Mode, Long_Note_Mode, Input_Favor_Mode, Note_Modify_Mode, BPM_Mode, Wave_Mode, Set_Note_Mode, Lowest_Judgment_Condition_Mode, Stand, Band, Is_P, Point, Salt, Highest_Judgment_0, Higher_Judgment_0, High_Judgment_0, Low_Judgment_0, Lower_Judgment_0, Lowest_Judgment_0, Highest_Judgment_1, Higher_Judgment_1, High_Judgment_1, Low_Judgment_1, Lower_Judgment_1, Lowest_Judgment_1, Lowest_Long_Note_Modify, Highest_Long_Note_Modify, Put_Note_Set, Put_Note_Set_Millis, Highest_Hit_Points_0, Higher_Hit_Points_0, High_Hit_Points_0, Low_Hit_Points_0, Lower_Hit_Points_0, Lowest_Hit_Points_0, Highest_Hit_Points_1, Higher_Hit_Points_1, High_Hit_Points_1, Low_Hit_Points_1, Lower_Hit_Points_1, Lowest_Hit_Points_1, Is_Paused, Input_Flags
                FROM comment
                WHERE {(string.IsNullOrEmpty(eventNoteID) ? "Note_ID = @noteID" : "Event_Note_ID = @eventNoteID")}
                ORDER BY Stand DESC
            """, _fastDB);
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
                data.Add(new(string.Empty, (DefaultCompute.InputFlag)rows.GetInt32("Input_Flags"))
                {
                    NoteFileCount = noteFileCount,
                    Date = date,
                    DateText = date.ToString("yyyy-MM-dd HH:mm:ss"),
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
                        LowestLongNoteModify = rows.GetDouble("Lowest_Long_Note_Modify"),
                        HighestLongNoteModify = rows.GetDouble("Highest_Long_Note_Modify"),
                        PutNoteSet = rows.GetInt32("Put_Note_Set"),
                        PutNoteSetMillis = rows.GetDouble("Put_Note_Set_Millis"),
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
                    IsP = rows.GetBoolean("Is_P"),
                    Point = rows.GetDouble("Point"),
                    IsPaused = rows.GetBoolean("Is_Paused")
                });
            }
            for (var i = data.Count - 1; i >= 0; --i)
            {
                data[i].CommentPlace0Text = $"＃{i + 1}";
                data[i].CommentPlace1Text = $"／{data.Count}";
            }
            return data;
        }

        public void SetEventNoteData(ICollection<WwwLevelGroup.WwwLevelComputing> wwwLevelComputingValues)
        {
            foreach (var wwwLevelComputingValue in wwwLevelComputingValues)
            {
                var noteVariety = (BaseNoteFile.NoteVariety)wwwLevelComputingValue.NoteVarietyValue;
                if (noteVariety != BaseNoteFile.NoteVariety.EventNote)
                {
                    var dbStatement = new SqliteCommand(@"REPLACE
                        INTO event_note_data
                        VALUES(@noteID, @noteVariety, @title, @artist, @level, @levelText, @genre)", _fastDB);
                    dbStatement.Parameters.AddWithValue("noteID", wwwLevelComputingValue.NoteID);
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
            var dbStatement = new SqliteCommand(@"SELECT Note_Variety, Title, Artist, Level, Level_Text, Genre
                FROM event_note_data
                WHERE Note_ID = @noteID", _fastDB);
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
            var dbStatement = new SqliteCommand(@"SELECT Favorite_Entry
                FROM note
                WHERE Note_ID = @noteID", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            var favoriteEntryItems = new List<DefaultEntryItem>();
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
            var dbStatement = new SqliteCommand(@"SELECT Event_Note_ID, Name, Date, Variety
                FROM event_note", _fastDB);
            using var rows = dbStatement.ExecuteReader();
            var eventNotes = new List<(string, string, DateTime, EventNoteVariety)>();
            while (rows.Read())
            {
                eventNotes.Add((rows.GetString("Event_Note_ID"), rows.GetString("Name"), rows.GetDateTime("Date"), (EventNoteVariety)rows.GetInt32("Variety")));
            }
            return eventNotes;
        }

        public BaseNoteFile.Handled GetHandled(BaseNoteFile noteFile)
        {
            var dbStatement = new SqliteCommand(@"SELECT Handled
                FROM handle
                WHERE Note_ID = @noteID", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? (BaseNoteFile.Handled)rows.GetInt32("Handled") : BaseNoteFile.Handled.Not;
        }

        public void SetHandled(BaseNoteFile noteFile)
        {
            var dbStatement = new SqliteCommand(@"REPLACE
                INTO handle
                VALUES(@noteID, @handled)", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("handled", noteFile.HandledValue);
            dbStatement.ExecuteNonQuery();
        }

        public (DateTime?, int) GetDate(BaseNoteFile noteFile, string eventNoteID)
        {
            var dbStatement = new SqliteCommand(@"SELECT MAX(Date) AS Latest, COUNT(Date) AS Count
                FROM date
                WHERE Note_ID = @noteID OR Event_Note_ID = @eventNoteID", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512() ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID ?? DBNull.Value as object);
            using var rows = dbStatement.ExecuteReader();
            rows.Read();
            var date = (null as DateTime?, 0);
            if (!rows.IsDBNull("Latest"))
            {
                date.Item1 = rows.GetDateTime("Latest");
            }
            if (!rows.IsDBNull("Count"))
            {
                date.Item2 = rows.GetInt32("Count");
            }
            return date;
        }

        public void NewDate(BaseNoteFile noteFile, string eventNoteID, DateTime date)
        {
            var dbStatement = new SqliteCommand(@"INSERT
                INTO date
                VALUES(@noteID, @eventNoteID, @date)", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512() ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.ExecuteNonQuery();
        }

        public int GetNotePosition(string entryPath)
        {
            var dbStatement = new SqliteCommand(@"SELECT Note_Position
                FROM entry
                WHERE Entry_Path = @entryPath", _fastDB);
            dbStatement.Parameters.AddWithValue("entryPath", entryPath);
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? rows.GetInt32("Note_Position") : 0;
        }

        public (double, double, bool?) GetWait(BaseNoteFile noteFile)
        {
            var dbStatement = new SqliteCommand(@"SELECT Audio_Wait, Media_Wait, Media
                FROM wait
                WHERE Note_ID = @noteID", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            (double, double, bool?) data = (0.0, 0.0, default);
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
            var dbStatement = new SqliteCommand(@"SELECT Format
                FROM format
                WHERE Note_ID = @noteID", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            using var rows = dbStatement.ExecuteReader();
            return rows.Read() ? rows.GetInt32("Format") : -1;
        }

        public void SetNotePosition(EntryItem entryItem)
        {
            var dbStatement = new SqliteCommand(@"REPLACE
                INTO entry
                VALUES(@entryPath, @notePosition)", _fastDB);
            dbStatement.Parameters.AddWithValue("entryPath", entryItem.EntryPath);
            dbStatement.Parameters.AddWithValue("notePosition", entryItem.NotePosition);
            dbStatement.ExecuteNonQuery();
        }

        public void SetFavoriteEntry(BaseNoteFile noteFile)
        {
            Ta(() =>
            {
                using (var dbStatement = new SqliteCommand(@"DELETE
                    FROM note
                    WHERE Note_ID = @noteID", _fastDB))
                {
                    dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
                    dbStatement.ExecuteNonQuery();
                }
                using (var dbStatement = new SqliteCommand(@"REPLACE
                    INTO note
                    VALUES(@noteID, @favoriteEntry)", _fastDB))
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
            var dbStatement = new SqliteCommand(@"INSERT INTO event_note
                VALUES(@eventNoteID, @eventNoteName, @date, @eventNoteVariety)", _fastDB);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.Parameters.AddWithValue("eventNoteName", eventNoteName);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.Parameters.AddWithValue("eventNoteVariety", eventNoteVariety);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeEventNote(string eventNoteID)
        {
            var dbStatement = new SqliteCommand(@"DELETE
                FROM event_note
                WHERE Event_Note_ID = @eventNoteID", _fastDB);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.ExecuteNonQuery();
        }

        public void ModifyEventNoteName(string eventNoteID, string eventNoteName)
        {
            var dbStatement = new SqliteCommand(@"UPDATE event_note
                SET Name = @eventNoteName
                WHERE Event_Note_ID = @eventNoteID", _fastDB);
            dbStatement.Parameters.AddWithValue("eventNoteName", eventNoteName);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID);
            dbStatement.ExecuteNonQuery();
        }

        public void SaveComment(DateTime date, BaseNoteFile noteFile, string eventNoteID, string comment, string avatar, double multiplier, double audioMultiplier, ModeComponent modeComponentValue, int stand, int band, bool isP, double point, bool isPaused, DefaultCompute.InputFlag inputFlags)
        {
            var dbStatement = new SqliteCommand(@"INSERT
                INTO comment
                VALUES(@date, @eventNoteID, @comment, @avatar, @multiplier, @autoMode, @noteSaltMode, @audioMultiplier, @faintNoteMode, @judgmentMode, @hitPointsMode, @noteMobilityMode, @longNoteMode, @inputFavorMode, @noteModifyMode, @bpmMode, @waveMode, @setNoteMode, @lowestJudgmentConditionMode, @stand, @band, @isP, @point, @salt, @highestJudgment0, @higherJudgment0, @highJudgment0, @lowJudgment0, @lowerJudgment0, @lowestJudgment0, @highestJudgment1, @higherJudgment1, @highJudgment1, @lowJudgment1, @lowerJudgment1, @lowestJudgment1, @lowestLongNoteModify, @highestLongNoteModify, @putNoteSet, @putNoteSetMillis, @highestHitPoints0, @higherHitPoints0, @highHitPoints0, @lowHitPoints0, @lowerHitPoints0, @lowestHitPoints0, @highestHitPoints1, @higherHitPoints1, @highHitPoints1, @lowHitPoints1, @lowerHitPoints1, @lowestHitPoints1, @noteID, @isPaused, @inputFlags)", _fastDB);
            dbStatement.Parameters.AddWithValue("date", date);
            dbStatement.Parameters.AddWithValue("eventNoteID", eventNoteID ?? DBNull.Value as object);
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
            dbStatement.Parameters.AddWithValue("noteID", noteFile?.GetNoteID512() ?? DBNull.Value as object);
            dbStatement.Parameters.AddWithValue("isPaused", isPaused);
            dbStatement.Parameters.AddWithValue("inputFlags", inputFlags);
            dbStatement.ExecuteNonQuery();
        }

        public void SetWait(BaseNoteFile noteFile, double audioWait, double mediaWait, bool media)
        {
            var dbStatement = new SqliteCommand(@"REPLACE
                INTO wait
                VALUES(@noteID, @audioWait, @mediaWait, @media)", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("audioWait", audioWait);
            dbStatement.Parameters.AddWithValue("mediaWait", mediaWait);
            dbStatement.Parameters.AddWithValue("media", media);
            dbStatement.ExecuteNonQuery();
        }

        public void SetNoteFormat(BaseNoteFile noteFile, int format)
        {
            var dbStatement = new SqliteCommand(@"REPLACE
                INTO format
                VALUES(@noteID, @format)", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.Parameters.AddWithValue("format", format);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeFavoriteEntry()
        {
            var dbStatement = new SqliteCommand(@"DELETE
                FROM note", _fastDB);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeHandled(BaseNoteFile noteFile)
        {
            var dbStatement = new SqliteCommand(@"DELETE
                FROM handle
                WHERE Note_ID = @noteID", _fastDB);
            dbStatement.Parameters.AddWithValue("noteID", noteFile.GetNoteID512());
            dbStatement.ExecuteNonQuery();
        }

        public void InitWait()
        {
            var dbStatement = new SqliteCommand(@"UPDATE wait
                SET Audio_Wait = NULL, Media_Wait = 0.0", _fastDB);
            dbStatement.ExecuteNonQuery();
        }

        public void InitMedia()
        {
            var dbStatement = new SqliteCommand(@"UPDATE wait
                SET Media = NULL", _fastDB);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeComment(string comment)
        {
            var dbStatement = new SqliteCommand(@"DELETE
                FROM comment
                WHERE Comment = @comment", _fastDB);
            dbStatement.Parameters.AddWithValue("comment", comment);
            dbStatement.ExecuteNonQuery();
        }

        public void WipeComment()
        {
            var dbStatement = new SqliteCommand(@"DELETE
                FROM comment", _fastDB);
            dbStatement.ExecuteNonQuery();
        }

        void Ta(Action onHandle)
        {
            using var t = _fastDB.BeginTransaction();
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

        public void Save()
        {
            Utility.CopyFile(_fileName, _tmp0FileName);
            Utility.MoveFile(_tmp0FileName, _tmp1FileName);
            _fastDB.BackupDatabase(_fileDB);
            Utility.WipeFile(_tmp1FileName);
        }
    }
}