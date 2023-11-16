/** 쓰레기 코드임 */

using Ionic.Zip;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace Compatible
{
    public static partial class Compatible
    {
        [GeneratedRegex("(\\$[a-zA-Z\\d]+)( @*[\\d.+\\-]+)*")]
        private static partial Regex GetFuncComputer();

        [GeneratedRegex("Out\\(\\d+.*\\)")]
        private static partial Regex GetWallFunc();

        [GeneratedRegex("In\\(\\d+.*\\)")]
        private static partial Regex GetContentsFunc();

        static int HighestNoteID = 64;
        static int Mode0 = 0;
        static int Mode484 = 16;

        public static void Qwilight(string qwilightEntry)
        {
            MoveFile(Path.Combine(qwilightEntry, "Qwilight.json"), Path.Combine(qwilightEntry, "Configure.json"));
            MoveFile(Path.Combine(qwilightEntry, "Qwilight.db"), Path.Combine(qwilightEntry, "DB.db"));

            var CommentEntry = Path.Combine(qwilightEntry, "Comment");

            foreach (var commentFilePath in GetFiles(CommentEntry, "*.pb"))
            {
                MoveFile(commentFilePath, Path.ChangeExtension(commentFilePath, null));
            }
        }

        public static void DB(SqliteConnection fastDB)
        {
            if (HasTable("comment"))
            {
                var dbStatement = new SqliteCommand(@"UPDATE comment
                    SET Comment = REPLACE(Comment, '.pb', '')", fastDB);
                dbStatement.ExecuteNonQuery();
            }
            if (HasTable("component"))
            {
                try
                {
                    var date = string.Empty;
                    var dbStatement = new SqliteCommand(@"SELECT Date
                        FROM component", fastDB);
                    using (var rows = dbStatement.ExecuteReader())
                    {
                        if (rows.Read())
                        {
                            date = rows.GetString("Date");
                        }
                    }

                    dbStatement = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS db_file (
					    ID TEXT,
					    Value TEXT,
					    PRIMARY KEY (ID)
				    )", fastDB);
                    dbStatement.ExecuteNonQuery();

                    dbStatement = new SqliteCommand("""
                        REPLACE INTO db_file
                        VALUES("date", @value)
                    """, fastDB);
                    dbStatement.Parameters.AddWithValue("value", date);
                    dbStatement.ExecuteNonQuery();

                    dbStatement = new SqliteCommand(@"DROP TABLE component", fastDB);
                    dbStatement.ExecuteNonQuery();
                }
                catch
                {
                }
            }

            if (HasTable("event_note_data"))
            {
                try
                {
                    var dbStatement = new SqliteCommand(@"ALTER TABLE event_note_data
                        RENAME COLUMN Composer TO Artist", fastDB);
                    dbStatement.ExecuteNonQuery();
                }
                catch
                {
                }
                try
                {
                    var dbStatement = new SqliteCommand(@"ALTER TABLE event_note_data
                        RENAME COLUMN Level_Contents TO Level_Text", fastDB);
                    dbStatement.ExecuteNonQuery();
                }
                catch
                {
                }
            }

            bool HasTable(string tableName)
            {
                var dbStatement = new SqliteCommand(@"SELECT name
                    FROM sqlite_master
                    WHERE type = 'table' AND name = @tableName", fastDB);
                dbStatement.Parameters.AddWithValue("tableName", tableName);
                using var rows = dbStatement.ExecuteReader();
                return rows.Read();
            }
        }

        public static void UI(string UIEntry, string yamlFilePath, string yamlName, string uiEntry)
        {
            var ys = new YamlStream();
            try
            {
                var yamlContents0 = string.Empty;
                var yamlContents1 = string.Empty;
                using (var sr = File.OpenText(yamlFilePath))
                {
                    yamlContents0 = sr.ReadToEnd();
                    yamlContents1 = yamlContents0
                        .Replace("function:", "func:")
                        .Replace("default-length:", "defaultLength:")
                        .Replace("default-height:", "defaultHeight:")
                        .Replace("longNoteHigherEdgeHeight:", "longNoteTailEdgeHeight:")
                        .Replace("longNoteLowerEdgeHeight:", "longNoteFrontEdgeHeight:")
                        .Replace("longNoteHigherEdgePosition:", "longNoteTailEdgePosition:")
                        .Replace("longNoteLowerEdgePosition:", "longNoteFrontEdgePosition:")
                        .Replace("longNoteHigherContentsHeight:", "longNoteTailContentsHeight:")
                        .Replace("longNoteLowerContentsHeight:", "longNoteFrontContentsHeight:")
                        .Replace("maintain-long-note-lower-edge:", "maintainLongNoteFrontEdge:")
                        .Replace("maintainLongNoteLowerEdge:", "maintainLongNoteFrontEdge:")
                        .Replace("set-judgment-main-position:", "setJudgmentMainPosition:")
                        .Replace("set-main-position:", "setMainPosition:")
                        .Replace("set-note-length:", "setNoteLength:")
                        .Replace("set-band-position:", "setBandPosition:")
                        .Replace("long-note-hit-update-frame:", "long-note-hit-loop-frame:")
                        .Replace("compound-frame:", "band-frame:")
                        .Replace("compound-framerate:", "band-framerate:")
                        .Replace("dual-hit-points:", "alt-hit-points:")
                        .Replace("migrate-hit-points:", "alt-hit-points:")
                        .Replace("dual-status:", "alt-status:")
                        .Replace("migrate-status:", "alt-status:")
                        .Replace("dual-band:", "alt-band:")
                        .Replace("migrate-band:", "alt-band:")
                        .Replace("dual-bpm:", "alt-bpm:")
                        .Replace("migrate-bpm:", "alt-bpm:")
                        .Replace("dual-multiplier:", "alt-multiplier:")
                        .Replace("migrate-multiplier:", "alt-multiplier:")
                        .Replace("dual-stand:", "alt-stand:")
                        .Replace("migrate-stand:", "alt-stand:")
                        .Replace("dual-point:", "alt-point:")
                        .Replace("migrate-point:", "alt-point:")
                        .Replace("dual-enjoy:", "alt-net:")
                        .Replace("migrate-enjoy:", "alt-net:")
                        .Replace("dual-mmo:", "alt-net:")
                        .Replace("migrate-mmo:", "alt-net:")
                        .Replace("dual-net:", "alt-net:")
                        .Replace("migrate-net:", "alt-net:")
                        .Replace("dual-pause:", "alt-pause:")
                        .Replace("migrate-pause:", "alt-pause:")
                        .Replace("dual-sw:", "alt-hms:")
                        .Replace("migrate-sw:", "alt-hms:")
                        .Replace("dual-now:", "alt-hms:")
                        .Replace("migrate-now:", "alt-hms:")
                        .Replace("alt-sec:", "alt-hms:")
                        .Replace("dual-judgment-meter:", "alt-judgment-meter:")
                        .Replace("migrate-judgment-meter:", "alt-judgment-meter:")
                        .Replace("dual-judgment-points:", "alt-judgment-points:")
                        .Replace("migrate-judgment-points:", "alt-judgment-points:")
                        .Replace("dual-last:", "alt-last:")
                        .Replace("migrate-last:", "alt-last:")
                        .Replace("dual-band!:", "alt-band!:")
                        .Replace("migrate-band!:", "alt-band!:")
                        .Replace("dual-wall-0:", "alt-wall-0:")
                        .Replace("migrate-wall-0:", "alt-wall-0:")
                        .Replace("dual-wall-1:", "alt-wall-1:")
                        .Replace("migrate-wall-1:", "alt-wall-1:")
                        .Replace("dual-audio-multiplier:", "alt-audio-multiplier:")
                        .Replace("migrate-audio-multiplier:", "alt-audio-multiplier:")
                        .Replace("migrate-hit-points-sign:", "alt-hit-points-visualizer:")
                        .Replace("migrate-hit-points-sgmt:", "alt-hit-points-visualizer:")
                        .Replace("migrate-highest-judgment-value:", "alt-highest-judgment-value:")
                        .Replace("migrate-higher-judgment-value:", "alt-higher-judgment-value:")
                        .Replace("migrate-high-judgment-value:", "alt-high-judgment-value:")
                        .Replace("migrate-low-judgment-value:", "alt-low-judgment-value:")
                        .Replace("migrate-lower-judgment-value:", "alt-lower-judgment-value:")
                        .Replace("migrate-lowest-judgment-value:", "alt-lowest-judgment-value:")
                        .Replace("judgmentCount:", "judgmentMainPosition:")
                        .Replace("mainWall0:", "mainWall0Length:")
                        .Replace("mainWall1:", "mainWall1Length:")
                        .Replace("numLength:", "binLength:")
                        .Replace("numHeight:", "binHeight:")
                        .Replace("standAlignment:", "standSystem:")
                        .Replace("standWave:", "standSystem:")
                        .Replace("pointAlignment:", "pointSystem:")
                        .Replace("pointWave:", "pointSystem:")
                        .Replace("unionAlignment:", "bandSystem:")
                        .Replace("comboAlignment:", "bandSystem:")
                        .Replace("compoundAlignment:", "bandSystem:")
                        .Replace("bandAlignment:", "bandSystem:")
                        .Replace("bandWave:", "bandSystem:")
                        .Replace("comboPosition0:", "bandPosition0:")
                        .Replace("unionPosition0:", "bandPosition0:")
                        .Replace("compoundPosition0:", "bandPosition0:")
                        .Replace("comboPosition1:", "bandPosition1:")
                        .Replace("unionPosition1:", "bandPosition1:")
                        .Replace("compoundPosition1:", "bandPosition1:")
                        .Replace("judgmentAlignment:", "judgmentSystem:")
                        .Replace("judgmentWave:", "judgmentSystem:")
                        .Replace("bpmAlignment:", "bpmSystem:")
                        .Replace("bpmWave:", "bpmSystem:")
                        .Replace("judgmentMeterAlignment:", "judgmentMeterSystem:")
                        .Replace("judgmentMeterWave:", "judgmentMeterSystem:")
                        .Replace("hitPointsAlignment:", "hitPointsSystem:")
                        .Replace("hitPointsWave:", "hitPointsSystem:")
                        .Replace("multiplierAlignment:", "multiplierSystem:")
                        .Replace("multiplierWave:", "multiplierSystem:")
                        .Replace("enjoyPosition0:", "netPosition0:")
                        .Replace("enjoyPosition1:", "netPosition1:")
                        .Replace("enjoyLength:", "netLength:")
                        .Replace("enjoyHeight0:", "netHeight0:")
                        .Replace("enjoyHeight1:", "netHeight1:")
                        .Replace("mmoPosition0:", "netPosition0:")
                        .Replace("mmoPosition1:", "netPosition1:")
                        .Replace("mmoLength:", "netLength:")
                        .Replace("mmoHeight0:", "netHeight0:")
                        .Replace("mmoHeight1:", "netHeight1:")
                        .Replace("pauseAlignment:", "pauseSystem:")
                        .Replace("pauseWave:", "pauseSystem:")
                        .Replace("statusAlignment:", "statusSystem:")
                        .Replace("statusWave:", "statusSystem:")
                        .Replace("numComboLength:", "binBandLength:")
                        .Replace("binCompoundLength:", "binBandLength:")
                        .Replace("numComboHeight:", "binBandHeight:")
                        .Replace("binCompoundHeight:", "binBandHeight:")
                        .Replace("newCompound:", "enlargeBand:")
                        .Replace("enjoy-font", "net-font")
                        .Replace("mmo-font", "net-font")
                        .Replace("lastAlignment:", "lastSystem:")
                        .Replace("lastWave:", "lastSystem:")
                        .Replace("ac-frame:", "band!-frame:")
                        .Replace("ac-framerate:", "band!-framerate:")
                        .Replace("acAlignment:", "band!System:")
                        .Replace("band!Alignment:", "band!System:")
                        .Replace("band!Wave:", "band!System:")
                        .Replace("acPosition0:", "band!Position0:")
                        .Replace("acPosition1:", "band!Position1:")
                        .Replace("acLength:", "band!Length:")
                        .Replace("acHeight:", "band!Height:")
                        .Replace("swAlignment:", "hmsSystem:")
                        .Replace("swAlignment:", "hmsSystem:")
                        .Replace("nowAlignment:", "hmsSystem:")
                        .Replace("nowWave:", "hmsSystem:")
                        .Replace("nowSystem:", "hmsSystem:")
                        .Replace("secSystem:", "hmsSystem:")
                        .Replace("swPosition0:", "hmsPosition0:")
                        .Replace("nowPosition0:", "hmsPosition0:")
                        .Replace("secPosition0:", "hmsPosition0:")
                        .Replace("swPosition1:", "hmsPosition1:")
                        .Replace("nowPosition1:", "hmsPosition1:")
                        .Replace("secPosition1:", "hmsPosition1:")
                        .Replace("binNowLength:", "binHmsLength:")
                        .Replace("binSecLength:", "binHmsLength:")
                        .Replace("binNowHeight:", "binHmsHeight:")
                        .Replace("binSecHeight:", "binHmsHeight:")
                        .Replace("autoPosition1:", "autoInputPosition1:")
                        .Replace("autoHeight:", "autoInputHeight:")
                        .Replace("judgmentPointsAlignment:", "judgmentPointsSystem:")
                        .Replace("judgmentPointsWave:", "judgmentPointsSystem:")
                        .Replace("commaImageLength:", "standCommaDrawingLength:")
                        .Replace("commaDrawingLength:", "standCommaDrawingLength:")
                        .Replace("standDelimiterDrawingLength:", "standCommaDrawingLength:")
                        .Replace("standCommaDrawingLength:", "standCommaDrawingLength:")
                        .Replace("pointDotImageLength:", "pointStopPointDrawingLength:")
                        .Replace("pointDotDrawingLength:", "pointStopPointDrawingLength:")
                        .Replace("pointStDrawingLength:", "pointStopPointDrawingLength:")
                        .Replace("multiplierStDrawingLength:", "multiplierStopPointDrawingLength:")
                        .Replace("audioMultiplierStDrawingLength:", "audioMultiplierStopPointDrawingLength:")
                        .Replace("pointImageLength:", "pointUnitDrawingLength:")
                        .Replace("pointDrawingLength:", "pointUnitDrawingLength:")
                        .Replace("bpmImageLength:", "bpmUnitDrawingLength:")
                        .Replace("bpmDrawingLength:", "bpmUnitDrawingLength:")
                        .Replace("msImageLength:", "judgmentMeterUnitDrawingLength:")
                        .Replace("msDrawingLength:", "judgmentMeterUnitDrawingLength:")
                        .Replace("dotImageLength:", "stopPointDrawingLength:")
                        .Replace("dotDrawingLength:", "stopPointDrawingLength:")
                        .Replace("stDrawingLength:", "stopPointDrawingLength:")
                        .Replace("dotDotImageLength:", "hmsColonDrawingLength:")
                        .Replace("dotDotDrawingLength:", "hmsColonDrawingLength:")
                        .Replace("nowDelimiterDrawingLength:", "hmsColonDrawingLength:")
                        .Replace("secColonDrawingLength:", "hmsColonDrawingLength:")
                        .Replace("slashImageLength:", "hmsSlashDrawingLength:")
                        .Replace("slashDrawingLength:", "hmsSlashDrawingLength:")
                        .Replace("secSlashDrawingLength:", "hmsSlashDrawingLength:")
                        .Replace("drawing-pipeline:", "drawingPipeline:")
                        .Replace("ui-input-mode-system:", "drawingInputModeSystem:")
                        .Replace("hitPointsSignSystem:", "hitPointsVisualizerSystem:")
                        .Replace("hitPointsSignPosition0:", "hitPointsVisualizerPosition0:")
                        .Replace("hitPointsSignPosition1:", "hitPointsVisualizerPosition1:")
                        .Replace("binHitPointsSignLength:", "binHitPointsVisualizerLength:")
                        .Replace("binHitPointsSignHeight:", "binHitPointsVisualizerHeight:")
                        .Replace("hitPointsSignDrawingLength:", "hitPointsVisualizerUnitDrawingLength:")
                        .Replace("hitPointsSignUnitDrawingLength:", "hitPointsVisualizerUnitDrawingLength:")
                        .Replace("hitPointsSgmtSystem:", "hitPointsVisualizerSystem:")
                        .Replace("hitPointsSgmtPosition0:", "hitPointsVisualizerPosition0:")
                        .Replace("hitPointsSgmtPosition1:", "hitPointsVisualizerPosition1:")
                        .Replace("binHitPointsSgmtLength:", "binHitPointsVisualizerLength:")
                        .Replace("binHitPointsSgmtHeight:", "binHitPointsVisualizerHeight:")
                        .Replace("hitPointsSgmtDrawingLength:", "hitPointsVisualizerUnitDrawingLength:")
                        .Replace("hitPointsSgmtUnitDrawingLength:", "hitPointsVisualizerUnitDrawingLength:")
                        .Replace("highestJudgmentValueLength:", "binHighestJudgmentValueLength:")
                        .Replace("highestJudgmentValueHeight:", "binHighestJudgmentValueHeight:")
                        .Replace("higherJudgmentValueLength:", "binHigherJudgmentValueLength:")
                        .Replace("higherJudgmentValueHeight:", "binHigherJudgmentValueHeight:")
                        .Replace("highJudgmentValueLength:", "binHighJudgmentValueLength:")
                        .Replace("highJudgmentValueHeight:", "binHighJudgmentValueHeight:")
                        .Replace("lowJudgmentValueLength:", "binLowJudgmentValueLength:")
                        .Replace("lowJudgmentValueHeight:", "binLowJudgmentValueHeight:")
                        .Replace("lowerJudgmentValueLength:", "binLowerJudgmentValueLength:")
                        .Replace("lowerJudgmentValueHeight:", "binLowerJudgmentValueHeight:")
                        .Replace("lowestJudgmentValueLength:", "binLowestJudgmentValueLength:")
                        .Replace("lowestJudgmentValueHeight:", "binLowestJudgmentValueHeight:")
                        .Replace("inputSgmtSystem:", "inputVisualizerSystem:")
                        .Replace("inputSgmtPosition0:", "inputVisualizerPosition0:")
                        .Replace("inputSgmtPosition1:", "inputVisualizerPosition1:")
                        .Replace("binInputSgmtLength:", "binInputVisualizerLength:")
                        .Replace("binInputSgmtHeight:", "binInputVisualizerHeight:")
                        .Replace("alt-input-sgmt:", "alt-input-visualizer:")
                        .Replace("judgmentSgmtSystem:", "judgmentVisualizerSystem:")
                        .Replace("judgmentSgmtPosition0:", "judgmentVisualizerPosition0:")
                        .Replace("judgmentSgmtPosition1:", "judgmentVisualizerPosition1:")
                        .Replace("judgmentSgmtLength:", "judgmentVisualizerLength:")
                        .Replace("judgmentSgmtHeight:", "judgmentVisualizerHeight:")
                        .Replace("judgmentSgmtContentLength:", "judgmentVisualizerContentLength:")
                        .Replace("judgmentSgmtContentHeight:", "judgmentVisualizerContentHeight:")
                        .Replace("alt-judgment-sgmt:", "alt-judgment-visualizer:")
                        .Replace("audio-data:", "audio-visualizer:")
                        .Replace("audio-main-data:", "audio-main-visualizer:")
                        .Replace("audio-input-data:", "audio-input-visualizer:")
                        .Replace("alt-hunter:", "alt-hunter:")
                        .Replace("huntSystem:", "hunterSystem:")
                        .Replace("huntPosition0:", "hunterPosition0:")
                        .Replace("huntPosition1:", "hunterPosition1:")
                        .Replace("binHuntLength:", "binHunterLength:")
                        .Replace("binHuntHeight:", "binHunterHeight:")
                        .Replace("huntFrontDrawingLength:", "hunterFrontDrawingLength:")
                        .Replace("titleSystem:", "titleSystem0:")
                        .Replace("composerSystem:", "artistSystem0:")
                        .Replace("composerSystem0:", "artistSystem0:")
                        .Replace("composerPosition0:", "artistPosition0:")
                        .Replace("composerPosition1:", "artistPosition1:")
                        .Replace("composerLength:", "artistLength:")
                        .Replace("composerHeight:", "artistHeight:")
                        .Replace("slideNotePosition0:", "slashNotePosition0:")
                        .Replace("judgmentVisualizerContentLength:", "judgmentVisualizerContentsLength:")
                        .Replace("judgmentVisualizerContentHeight:", "judgmentVisualizerContentsHeight:")
                        .Replace("longNoteHigherContentHeight:", "longNoteHigherContentsHeight:")
                        .Replace("longNoteLowerContentHeight:", "longNoteLowerContentsHeight:")
                        .Replace("levelContentsPosition0:", "levelTextPosition0:")
                        .Replace("levelContentsPosition1:", "levelTextPosition1:")
                        .Replace("levelContentsLength:", "levelTextLength:")
                        .Replace("levelContenteHeight:", "levelTextHeight:")
                        .Replace("levelContentsSystem0:", "levelTextSystem0:")
                        .Replace("levelContentsSystem1:", "levelTextSystem1:")
                        .Replace("judgmentSystem:", "judgmentPaintSystem:")
                        .Replace("judgmentPosition0:", "judgmentPaintPosition0:")
                        .Replace("judgmentPosition1:", "judgmentPaintPosition1:")
                        .Replace("judgmentLength:", "judgmentPaintLength:")
                        .Replace("judgmentHeight:", "judgmentPaintHeight:")
                        .Replace("title-level:", "titleLevel:")
                        .Replace("composer-level:", "artistLevel:")
                        .Replace("composerLevel:", "artistLevel:")
                        .Replace("levelContentsLevel:", "levelTextLevel:")
                        .Replace("$In", "$Contents")
                        .Replace("$Out", "$Wall");
                    for (var i = HighestNoteID; i > 0; --i)
                    {
                        yamlContents1 = yamlContents1
                            .Replace($"ui-image-{i}", $"drawing{i}")
                            .Replace($"ui-drawing-{i}", $"drawing{i}");
                    }
                    for (var i = Mode484; i > Mode0; --i)
                    {
                        yamlContents1 = yamlContents1
                            .Replace($"ui-input-mode-{i}", $"drawingInputMode{i}");
                    }
                }
                using (var sr = new StringReader(yamlContents1))
                {
                    var targetUIBuilder = new StringBuilder();
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var delimitedCommaData = line.Split(",");
                        for (var i = delimitedCommaData.Length - 1; i >= 0; --i)
                        {
                            if (i > 0)
                            {
                                delimitedCommaData[i] = delimitedCommaData[i].Trim();
                            }
                            foreach (Match m in GetFuncComputer().Matches(delimitedCommaData[i]))
                            {
                                var values = m.Value.Split(" ");
                                var length = values.Length;
                                var builder = new StringBuilder(values[0][1..]);
                                builder.Append('(');
                                builder.Append(string.Join(", ", values.Select(value => value[(value.IndexOf("@") + 1)..]).ToArray(), 1, length - 1));
                                builder.Append(')');
                                delimitedCommaData[i] = delimitedCommaData[i].Replace(m.Value, builder.ToString());
                            }
                            line = string.Join(", ", delimitedCommaData);
                        }
                        targetUIBuilder.AppendLine(line);
                    }
                    yamlContents1 = targetUIBuilder.ToString();
                }
                if (yamlContents0 != yamlContents1)
                {
                    using var sw = new StreamWriter(yamlFilePath);
                    sw.Write(yamlContents1);
                }

                string zipName;
                string luaName;
                using (var sr = File.OpenText(yamlFilePath))
                {
                    ys.Load(sr);
                    var mNode = ys.Documents[0].RootNode;
                    var formatNode = mNode[new YamlScalarNode("format")];
                    zipName = GetText(formatNode, "zip", yamlName);
                    luaName = GetText(formatNode, "lua", yamlName);
                }
                var zipFilePath = Path.Combine(UIEntry, uiEntry, Path.ChangeExtension(zipName, "zip"));
                if (File.Exists(zipFilePath))
                {
                    var wasModified = false;
                    using var zipFile = new ZipFile(zipFilePath);
                    foreach (var zipEntry in zipFile.ToArray())
                    {
                        if (zipEntry.IsDirectory)
                        {
                            switch (zipEntry.FileName)
                            {
                                case "Num/":
                                    zipEntry.FileName = "Bin/";
                                    wasModified = true;
                                    break;
                                case "Image/":
                                    zipEntry.FileName = "Drawing/";
                                    wasModified = true;
                                    break;
                            }
                        }
                        else
                        {
                            for (var i = 9; i >= 0; --i)
                            {
                                if (zipEntry.FileName.StartsWith($"Bin/H {i}."))
                                {
                                    zipEntry.FileName = $"Bin/HP {i}{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            for (var i = 1; i >= 0; --i)
                            {
                                if (zipEntry.FileName.StartsWith($"Main/S {i}."))
                                {
                                    zipEntry.FileName = $"Main/W {i}{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            for (var i = 3; i > 0; --i)
                            {
                                if (zipEntry.FileName.StartsWith($"Pause/{i}."))
                                {
                                    zipEntry.FileName = $"Drawing/PS {i - 1} 0{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            switch (Path.GetDirectoryName(zipEntry.FileName))
                            {
                                case "Num":
                                    zipEntry.FileName = $"Bin/{Path.GetFileName(zipEntry.FileName)}";
                                    wasModified = true;
                                    break;
                                case "Image":
                                    zipEntry.FileName = $"Drawing/{Path.GetFileName(zipEntry.FileName)}";
                                    wasModified = true;
                                    break;
                            }
                            if (zipEntry.FileName.StartsWith("_Default."))
                            {
                                zipEntry.FileName = $"_{yamlName}{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            if (zipEntry.FileName.StartsWith("Drawing/Hit Points."))
                            {
                                zipEntry.FileName = $"Drawing/HP 1{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                        }
                    }
                    if (wasModified)
                    {
                        zipFile.Save();
                    }
                }

                var luaFilePath = luaName == "Default" ? Path.Combine(UIEntry, "Default.lua") : Path.Combine(UIEntry, uiEntry, Path.ChangeExtension(luaName, "lua"));
                if (File.Exists(luaFilePath))
                {
                    var luaContents0 = string.Empty;
                    var luaContents1 = string.Empty;
                    using (var sr = File.OpenText(luaFilePath))
                    {
                        luaContents0 = sr.ReadToEnd();
                        luaContents1 = luaContents0
                            .Replace("inputSemiCount", "autoableInputCount")
                            .Replace("etudeInputCount", "autoableInputCount")
                            .Replace("inputDefaultCount", "defaultInputCount")
                            .Replace("_GetAuto(", "_GetAutoInput(")
                            .Replace("_GetNotePaint(", "_GetHitNotePaint(")
                            .Replace("_GetLongNotePaint(", "_GetHitLongNotePaint(");
                        foreach (Match m in GetWallFunc().Matches(luaContents1))
                        {
                            luaContents1 = luaContents1.Replace(m.Value, "Wall(" + m.Value[4..]);
                        }
                        foreach (Match m in GetContentsFunc().Matches(luaContents1))
                        {
                            luaContents1 = luaContents1.Replace(m.Value, "Contents(" + m.Value[3..]);
                        }
                    }
                    if (luaContents0 != luaContents1)
                    {
                        using var sw = new StreamWriter(luaFilePath);
                        sw.Write(luaContents1);
                    }
                }
            }
            catch
            {
            }
        }

        public static void BaseUI(string UIEntry, string yamlFilePath, string yamlName, string uiEntry)
        {
            var ys = new YamlStream();
            try
            {
                var yamlContents0 = string.Empty;
                var yamlContents1 = string.Empty;
                using (var sr = File.OpenText(yamlFilePath))
                {
                    yamlContents0 = sr.ReadToEnd();
                    yamlContents1 = yamlContents0
                        .Replace("uniteModeContents:", "fittedContents:")
                        .Replace("uniteModeContentsLevel:", "fittedContentsLevel:")
                        .Replace("siteViewer:", "siteHref:")
                        .Replace("entryViewerTitleMargin:", "entryViewTitleMargin:")
                        .Replace("commentViewer:", "commentView:")
                        .Replace("entryViewer:", "entryView:")
                        .Replace("inputCountViewer:", "inputNoteCountView:")
                        .Replace("inputCountView:", "inputNoteCountView:")
                        .Replace("assistViewer:", "assistView:")
                        .Replace("composer:", "artist:")
                        .Replace("composerQuit:", "artistQuit:")
                        .Replace("composerLevel:", "artistLevel:")
                        .Replace("entireNotes:", "totalNotes:")
                        .Replace("entireNotesContents:", "totalNotesContents:")
                        .Replace("entireNotesQuit:", "totalNotesJudgmentQuit:")
                        .Replace("entireNotesContentsQuit:", "totalNotesJudgmentContentsQuit:")
                        .Replace("totalNotesQuit:", "totalNotesJudgmentQuit:")
                        .Replace("totalNotesContentsQuit:", "totalNotesJudgmentContentsQuit:")
                        .Replace("statusEntryOpening:", "statusDefaultEntry:")
                        .Replace("statusDefaultEntryOpening:", "statusDefaultEntry:")
                        .Replace("eventerDrawing:", "avatarDrawing:")
                        .Replace("showCommentInput:", "viewComment:")
                        .Replace("commentInput:", "viewComment:")
                        .Replace("undoInput:", "handleUndo:")
                        .Replace("quitView:", "statusView:")
                        .Replace("commentNameLevel:", "commentAvatarNameLevel:")
                        .Replace("quitDrawingQuit:", "quitDrawingV2:")
                        .Replace("highestJudgment:", "highestJudgmentQuit:")
                        .Replace("higherJudgment:", "higherJudgmentQuit:")
                        .Replace("highJudgment:", "highJudgmentQuit:")
                        .Replace("lowJudgment:", "lowJudgmentQuit:")
                        .Replace("lowerJudgment:", "lowerJudgmentQuit:")
                        .Replace("lowestJudgment:", "lowestJudgmentQuit:")
                        .Replace("highestJudgmentView:", "highestJudgmentV2:")
                        .Replace("higherJudgmentView:", "higherJudgmentV2:")
                        .Replace("highJudgmentView:", "highJudgmentV2:")
                        .Replace("lowJudgmentView:", "lowJudgmentV2:")
                        .Replace("lowerJudgmentView:", "lowerJudgmentV2:")
                        .Replace("lowestJudgmentView:", "lowestJudgmentV2:")
                        .Replace("entryPositionLevel:", "entryItemPositionLevel:");
                }
                if (yamlContents0 != yamlContents1)
                {
                    using var sw = new StreamWriter(yamlFilePath);
                    sw.Write(yamlContents1);
                }

                string zipName;
                using (var sr = File.OpenText(yamlFilePath))
                {
                    ys.Load(sr);
                    var mNode = ys.Documents[0].RootNode;
                    var formatNode = mNode[new YamlScalarNode("format")];
                    zipName = $"@{GetText(formatNode, "zip")}";
                }
                var zipFilePath = Path.Combine(UIEntry, uiEntry, Path.ChangeExtension(zipName, "zip"));
                if (File.Exists(zipFilePath))
                {
                    var wasModified = false;
                    var tmpEntryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    using var zipFile = new ZipFile(zipFilePath);
                    foreach (var zipEntry in zipFile.ToArray())
                    {
                        if (!zipEntry.IsDirectory)
                        {
                            foreach (var fileName in new[] { "Stand", "Highest Band", "Point", "New Stand", "View Comment" })
                            {
                                if (zipEntry.FileName.StartsWith(fileName))
                                {
                                    try
                                    {
                                        zipEntry.FileName = $"Quit Mode/{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                    }
                                    catch
                                    {
                                    }
                                    wasModified = true;
                                }
                            }
                            if (zipEntry.FileName.StartsWith("Undo"))
                            {
                                try
                                {
                                    zipEntry.FileName = $"Quit Mode/Handle Undo{Path.GetExtension(zipEntry.FileName)}";
                                }
                                catch
                                {
                                }
                                wasModified = true;
                            }
                            foreach (var fileName in new[] { "Title", "Artist", "Modified Date", "Level Contents", "Entry", "Latest Date", "Handled Count", "Twilight Comment Count" })
                            {
                                if (zipEntry.FileName.StartsWith(fileName))
                                {
                                    try
                                    {
                                        zipEntry.FileName = $"FallIn/{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                    }
                                    catch
                                    {
                                    }
                                    wasModified = true;
                                }
                            }
                            foreach (var fileName in new[] { "Highest Input Count", "Length", "BPM" })
                            {
                                var target = $"{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                if (zipEntry.FileName.StartsWith(fileName) && !zipEntry.FileName.StartsWith("BPM!") && (!zipFile.Any(zipEntry => zipEntry.FileName == $"Quit Mode/{target}") || !zipFile.Any(zipEntry => zipEntry.FileName == $"Net Site/{target}") || !zipFile.Any(zipEntry => zipEntry.FileName == $"FallIn/{target}")))
                                {
                                    Directory.CreateDirectory(tmpEntryPath);
                                    var tmpFileName = Path.GetTempFileName();
                                    File.Move(tmpFileName, tmpFileName = Path.Combine(tmpEntryPath, Path.GetFileName(tmpFileName)));
                                    using (var ms = File.OpenWrite(tmpFileName))
                                    {
                                        zipEntry.Extract(ms);
                                    }
                                    try
                                    {
                                        var targetFileName = Path.Combine(Path.GetDirectoryName(tmpFileName), target);
                                        File.Move(tmpFileName, targetFileName);
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "Quit Mode");
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "Net Site");
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "FallIn");
                                        }
                                        catch
                                        {
                                        }
                                        wasModified = true;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            foreach (var fileName in new[] { "Judgment Stage" })
                            {
                                var target = $"{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                if (zipEntry.FileName.StartsWith(fileName) && (!zipFile.Any(zipEntry => zipEntry.FileName == $"Quit Mode/{target}") || !zipFile.Any(zipEntry => zipEntry.FileName == $"Net Site/{target}")))
                                {
                                    Directory.CreateDirectory(tmpEntryPath);
                                    var tmpFileName = Path.GetTempFileName();
                                    File.Move(tmpFileName, tmpFileName = Path.Combine(tmpEntryPath, Path.GetFileName(tmpFileName)));
                                    using (var ms = File.OpenWrite(tmpFileName))
                                    {
                                        zipEntry.Extract(ms);
                                    }
                                    try
                                    {
                                        var targetFileName = Path.Combine(Path.GetDirectoryName(tmpFileName), target);
                                        File.Move(tmpFileName, targetFileName);
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "Quit Mode");
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "Net Site");
                                        }
                                        catch
                                        {
                                        }
                                        wasModified = true;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            foreach (var fileName in new[] { "Input Mode" })
                            {
                                var target = $"{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                if (zipEntry.FileName.StartsWith(fileName) && !zipEntry.FileName.StartsWith("BPM!") && !zipFile.Any(zipEntry => zipEntry.FileName == $"Quit Mode/{target}"))
                                {
                                    Directory.CreateDirectory(tmpEntryPath);
                                    var tmpFileName = Path.GetTempFileName();
                                    File.Move(tmpFileName, tmpFileName = Path.Combine(tmpEntryPath, Path.GetFileName(tmpFileName)));
                                    using (var ms = File.OpenWrite(tmpFileName))
                                    {
                                        zipEntry.Extract(ms);
                                    }
                                    try
                                    {
                                        var targetFileName = Path.Combine(Path.GetDirectoryName(tmpFileName), target);
                                        File.Move(tmpFileName, targetFileName);
                                        zipFile.AddFile(targetFileName, "Quit Mode");
                                    }
                                    catch
                                    {
                                    }
                                    wasModified = true;
                                }
                            }
                            foreach (var fileName in new[] { "Total Notes" })
                            {
                                var target = $"{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                if (zipEntry.FileName.StartsWith(fileName) && (!zipFile.Any(zipEntry => zipEntry.FileName == $"FallIn/{target}") || !zipFile.Any(zipEntry => zipEntry.FileName == $"Net Site/{target}")))
                                {
                                    Directory.CreateDirectory(tmpEntryPath);
                                    var tmpFileName = Path.GetTempFileName();
                                    File.Move(tmpFileName, tmpFileName = Path.Combine(tmpEntryPath, Path.GetFileName(tmpFileName)));
                                    using (var ms = File.OpenWrite(tmpFileName))
                                    {
                                        zipEntry.Extract(ms);
                                    }
                                    try
                                    {
                                        var targetFileName = Path.Combine(Path.GetDirectoryName(tmpFileName), target);
                                        File.Move(tmpFileName, targetFileName);
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "FallIn");
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            zipFile.AddFile(targetFileName, "Net Site");
                                        }
                                        catch
                                        {
                                        }
                                        wasModified = true;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            if (zipEntry.FileName.StartsWith($"Audio/Begin Note File."))
                            {
                                zipEntry.FileName = $"Audio/Levy Note File{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            if (zipEntry.FileName.StartsWith($"Audio/Lower Entry."))
                            {
                                zipEntry.FileName = $"Audio/Lower Entry Item{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            if (zipEntry.FileName.StartsWith($"Audio/Higher Entry."))
                            {
                                zipEntry.FileName = $"Audio/Higher Entry Item{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            for (var i = 1; i >= 0; --i)
                            {
                                if (zipEntry.FileName.StartsWith($"Audio/Controller {i}."))
                                {
                                    zipEntry.FileName = $"Audio/Window {i}{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            for (var i = 2; i >= 0; --i)
                            {
                                if (zipEntry.FileName.StartsWith($"Net Site/NS {i}."))
                                {
                                    zipEntry.FileName = $"Site Situation/SS {i}{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            for (var i = 2; i >= 0; --i)
                            {
                                if (zipEntry.FileName.StartsWith($"Saving Bundle/SB {i}."))
                                {
                                    zipEntry.FileName = $"Notify/N {i + 4}{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            foreach (var fileName in new[] { "Artist", "BPM", "Entry", "Handled Count", "Highest Input Count", "Latest Date", "Length", "Level Contents", "Modified Date", "Title", "Total Notes", "Twilight Comment Count" })
                            {
                                if (zipEntry.FileName.StartsWith($"Unite/{fileName}."))
                                {
                                    zipEntry.FileName = $"Fit/{fileName}{Path.GetExtension(zipEntry.FileName)}";
                                    wasModified = true;
                                }
                            }
                            if (zipEntry.FileName.StartsWith("Fit/Level Contents."))
                            {
                                zipEntry.FileName = $"Fit/Level Text Value{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            if (zipEntry.FileName.StartsWith("Fit/Entry."))
                            {
                                zipEntry.FileName = $"Fit/Entry Path{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit Mode/Undo."))
                            {
                                zipEntry.FileName = $"Quit Mode/Handle Undo{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Audio/Sign out."))
                            {
                                zipEntry.FileName = $"Audio/Not Sign in{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Judgment/J 6."))
                            {
                                zipEntry.FileName = $"Judgment/Total Notes{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Judgment/Entire Notes."))
                            {
                                zipEntry.FileName = $"Judgment/Total Notes{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Eventer Configure/EC 0."))
                            {
                                zipEntry.FileName = $"Avatar Configure/AC 0{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Eventer Configure/EC 1."))
                            {
                                zipEntry.FileName = $"Avatar Configure/AC 1{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Eventer Configure/EC 2."))
                            {
                                zipEntry.FileName = $"Avatar Configure/AC 2{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Eventer Configure/EC 3."))
                            {
                                zipEntry.FileName = $"Avatar Configure/AC 3{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Eventer Configure/EC 4."))
                            {
                                zipEntry.FileName = $"Avatar Configure/AC 4{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Eventer Configure/EC 5."))
                            {
                                zipEntry.FileName = $"Avatar Configure/AC 5{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("_Default."))
                            {
                                zipEntry.FileName = $"_{yamlName.Substring(1)}{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith($"_{yamlName}."))
                            {
                                zipEntry.FileName = $"_{yamlName.Substring(1)}{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/X."))
                            {
                                zipEntry.FileName = $"Quit v2/S+{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/U."))
                            {
                                zipEntry.FileName = $"Quit v2/S{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/S."))
                            {
                                zipEntry.FileName = $"Quit v2/A+{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/A."))
                            {
                                zipEntry.FileName = $"Quit v2/A{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/B."))
                            {
                                zipEntry.FileName = $"Quit v2/B{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/C."))
                            {
                                zipEntry.FileName = $"Quit v2/C{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/D."))
                            {
                                zipEntry.FileName = $"Quit v2/D{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/F."))
                            {
                                zipEntry.FileName = $"Quit v2/F{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/U+."))
                            {
                                zipEntry.FileName = $"Quit v2/S FC{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/S+."))
                            {
                                zipEntry.FileName = $"Quit v2/A+ FC{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/A+."))
                            {
                                zipEntry.FileName = $"Quit v2/A FC{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/B+."))
                            {
                                zipEntry.FileName = $"Quit v2/B FC{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/C+."))
                            {
                                zipEntry.FileName = $"Quit v2/C FC{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                            else if (zipEntry.FileName.StartsWith("Quit/D+."))
                            {
                                zipEntry.FileName = $"Quit v2/D FC{Path.GetExtension(zipEntry.FileName)}";
                                wasModified = true;
                            }
                        }
                    }
                    if (wasModified)
                    {
                        zipFile.Save();
                    }
                }
            }
            catch
            {
            }
        }
        static void EraseFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }
        }

        static void EraseEntry(string entryPath)
        {
            try
            {
                if (Directory.Exists(entryPath))
                {
                    Directory.Delete(entryPath, true);
                }
            }
            catch
            {
            }
        }

        static string[] GetFiles(string entryPath, string o = "")
        {
            try
            {
                return Directory.Exists(entryPath) ? Directory.GetFiles(entryPath, o) : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        static void MoveFile(string src, string target)
        {
            try
            {
                if (File.Exists(src))
                {
                    EraseFile(target);
                    EraseEntry(target);
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    File.Move(src, target);
                }
            }
            catch
            {
            }
        }

        static int ModifyInt(object o) => Convert.ToInt32(o, CultureInfo.InvariantCulture);

        static string GetText(YamlNode yamlNode, string target, string defaultValue = null)
        {
            if ((yamlNode as YamlMappingNode)?.Children?.TryGetValue(new YamlScalarNode(target), out var value) == true)
            {
                var text = value.ToString().Trim();
                return string.IsNullOrEmpty(text) ? defaultValue : text;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}