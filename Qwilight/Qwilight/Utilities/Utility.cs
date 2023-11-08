using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using SharpGen.Runtime;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using YamlDotNet.RepresentationModel;
using DrawingContext = System.Windows.Media.DrawingContext;
using IDirectInputDevice8 = Vortice.DirectInput.IDirectInputDevice8;
using IStateUpdate = Vortice.DirectInput.IStateUpdate;
using Point = System.Windows.Point;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        [GeneratedRegex("(http|https|mailto):\\/\\/[^ ]+", RegexOptions.IgnoreCase)]
        private static partial Regex GetSiteYellsComputer();

        [Flags]
        public enum AvailableFlag
        {
            Not = 0,
            Audio = 1,
            Drawing = 2,
            Media = 4
        }

        static readonly char[] _delimiters = { '[', '(' };

        public static string GetTitle(string title)
        {
            var titlePosition = title.LastIndexOfAny(_delimiters);
            return (titlePosition >= 0 ? title.Substring(0, titlePosition) : title).Trim();
        }

        public static void HandleUIAudio(string audioFileName, string defaultFileName = null, PausableAudioHandler pausableAudioHandler = null, double fadeInLength = 0.0) => Task.Run(() =>
        {
            if (!BaseUI.Instance.HandleAudio(audioFileName, defaultFileName, pausableAudioHandler, fadeInLength))
            {
                UI.Instance.HandleAudio(audioFileName, defaultFileName, pausableAudioHandler, fadeInLength);
            }
        });

        public static bool IsPoint(double[] point, double position0, double position1) => IsPoint(point[0], point[1], point[2], point[3], position0, position1);

        public static bool IsPoint(double pointPosition0, double pointPosition1, double pointLength, double pointHeight, double position0, double position1)
        {
            return pointPosition0 <= position0 &&
                position0 < pointPosition0 + pointLength &&
                pointPosition1 <= position1 &&
                position1 < pointPosition1 + pointHeight;
        }

        public static string FormatUnit(long value)
        {
            if (value < 1000)
            {
                return $"{(value):#,##0.##} B";
            }
            else if (value < 1000 * 1000)
            {
                return $"{(value / 1000.0):#,##0.##} KB";
            }
            else if (value < 1000 * 1000 * 1000)
            {
                return $"{(value / 1000.0 / 1000.0):#,##0.##} MB";
            }
            else
            {
                return $"{(value / 1000.0 / 1000.0 / 1000.0):#,##0.##} GB";
            }
        }

        public static string GetHandledText(MediaElement view)
        {
            if (view.NaturalDuration.HasTimeSpan)
            {
                return $"{view.Position.Minutes}:{view.Position.Seconds.ToString("00")}／{view.NaturalDuration.TimeSpan.Minutes}:{view.NaturalDuration.TimeSpan.Seconds.ToString("00")}";
            }
            else
            {
                return string.Empty;
            }
        }

        public static void ViewItems(FrameworkElement element)
        {
            var elements = (element as FrameworkElement).ContextMenu;
            elements.DataContext = (element as FrameworkElement).DataContext;
            elements.Placement = PlacementMode.Mouse;
            elements.IsOpen = true;
        }

        public static void SetPosition(MediaElement view, double value)
        {
            if (value < 100.0)
            {
                if (view.NaturalDuration.HasTimeSpan)
                {
                    var targetPosition = view.NaturalDuration.TimeSpan * value / 100.0;
                    if (Math.Abs((view.Position - targetPosition).TotalSeconds) >= 1.0)
                    {
                        view.Position = view.NaturalDuration.TimeSpan * value / 100.0;
                    }
                }
            }
            else
            {
                view.Stop();
            }
        }

        public static bool AllowInput<T>(InputBundle<T> inputBundles, T e, Component.InputMode? inputMode = null) where T : new()
        {
            if (inputMode.HasValue)
            {
                var inputModeValue = (int)inputMode.Value;
                var inputs = inputBundles.Inputs;
                for (var i = inputs[(int)inputModeValue].Length - 1; i > 0; --i)
                {
                    for (var j = inputs[(int)inputModeValue][i].Length - 1; j >= 0; --j)
                    {
                        if (inputs[(int)inputModeValue][i][j].Equals(e))
                        {
                            inputs[(int)inputModeValue][i][j] = new();
                            break;
                        }
                    }
                }
            }
            return true;
        }

        public static float GetMove(float target, float src, double framerate = 60.0)
        {
            var distance = target - src;
            if (distance != 0.0)
            {
                if (distance > 0.0)
                {
                    if (distance > 0.01)
                    {
                        return (float)(distance / framerate);
                    }
                    else
                    {
                        return distance;
                    }
                }
                else
                {
                    if (distance < -0.01)
                    {
                        return (float)(distance / framerate);
                    }
                    else
                    {
                        return distance;
                    }
                }
            }
            return 0F;
        }

        public static double GetMove(double target, double src, double framerate = 60.0)
        {
            var distance = target - src;
            if (distance != 0.0)
            {
                if (distance > 0.0)
                {
                    if (distance > 0.01)
                    {
                        return distance / framerate;
                    }
                    else
                    {
                        return distance;
                    }
                }
                else
                {
                    if (distance < -0.01)
                    {
                        return distance / framerate;
                    }
                    else
                    {
                        return distance;
                    }
                }
            }
            return 0.0;
        }

        public static int GetMove(int target, int src, double framerate = 60.0)
        {
            var distance = target - src;
            if (distance != 0)
            {
                if (distance > 0)
                {
                    return Math.Max(1, (int)(distance / framerate));
                }
                else
                {
                    return Math.Min(-1, (int)(distance / framerate));
                }
            }
            return 0;
        }

        public static void SetUICollection<T>(ICollection<T> target, ICollection<T> src, Action<T> onWipe = null, Action<T> onSet = null, Action<T, T> onModify = null, IEqualityComparer<T> onEqual = null)
        {
            onEqual ??= EqualityComparer<T>.Default;
            if (target is INotifyCollectionChanged)
            {
                HandlingUISystem.Instance.HandleParallel(Hande);
            }
            else
            {
                Hande();
            }
            void Hande()
            {
                var toWipeValues = target.Except(src, onEqual).ToArray();
                var toSetValues = src.Except(target, onEqual).ToArray();
                var toModifyValues = onModify != null ? src.Intersect(target, onEqual).ToArray() : Array.Empty<T>();
                foreach (var value in toWipeValues)
                {
                    onWipe?.Invoke(value);
                    target.Remove(value);
                }
                foreach (var value in toSetValues)
                {
                    onSet?.Invoke(value);
                    target.Add(value);
                }
                foreach (var value in toModifyValues)
                {
                    onModify.Invoke(target.Single(targetValue => onEqual.Equals(value, targetValue)), value);
                }
            }
        }

        public static double GetMillis(this Stopwatch handler) => 1000.0 * handler.ElapsedTicks / Stopwatch.Frequency;

        public static void Set(this ref Vector2 v, float position0, float position1)
        {
            v.X = position0;
            v.Y = position1;
        }

        public static void Set(this ref Point p, double position0, double position1)
        {
            p.X = position0;
            p.Y = position1;
        }

        public static void OpenAs(string fileName, string parameter = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    using (Process.Start(new ProcessStartInfo(fileName, parameter)
                    {
                        UseShellExecute = true
                    }))
                    {
                    }
                }
                catch
                {
                }
            }
        }

        public static T GetFavoriteItem<T>(IEnumerable<T> data)
        {
            var dataSet = new Dictionary<T, int>();
            foreach (var dataItem in data)
            {
                if (dataSet.ContainsKey(dataItem))
                {
                    dataSet[dataItem]++;
                }
                else
                {
                    dataSet.Add(dataItem, 1);
                }
            }
            var favoriteItem = default(T);
            var favoriteValue = 0;
            foreach (var dataItemValueID in dataSet.Keys)
            {
                var dataItemValue = dataSet[dataItemValueID];
                if (dataItemValue > favoriteValue)
                {
                    favoriteItem = dataItemValueID;
                    favoriteValue = dataItemValue;
                }
            }
            return favoriteItem;
        }

        public static double SetCommentWait(int commentWaitDate, double audioMultiplier, double wait)
        {
            if (commentWaitDate < Component.CommentWaitDate1311)
            {
                wait -= 3000.0;
            }
            if (commentWaitDate < Component.CommentWaitDate164)
            {
                wait *= audioMultiplier;
            }
            return wait;
        }

        public static DefaultCompute.QuitStatus GetQuitStatusValue(double point, int stand, double hitPoints, int noteFileCount)
        {
            if (hitPoints == 0.0)
            {
                return DefaultCompute.QuitStatus.F;
            }
            if (point < 0.8)
            {
                return DefaultCompute.QuitStatus.D;
            }
            if (point < 0.85)
            {
                return DefaultCompute.QuitStatus.C;
            }
            if (point < 0.9)
            {
                return DefaultCompute.QuitStatus.B;
            }
            if (point < 0.95)
            {
                return DefaultCompute.QuitStatus.A;
            }
            return stand < noteFileCount * 900000 ? DefaultCompute.QuitStatus.APlus : point < 1.0 ? DefaultCompute.QuitStatus.S : DefaultCompute.QuitStatus.SPlus;
        }

        public static AvailableFlag GetAvailable(string filePath)
        {
            foreach (var audioFileFormat in QwilightComponent.AudioFileFormatItems)
            {
                if (filePath.IsTailCaselsss(audioFileFormat))
                {
                    return AvailableFlag.Audio;
                }
            }
            foreach (var mediaFileFormat in QwilightComponent.MediaFileFormats)
            {
                if (filePath.IsTailCaselsss(mediaFileFormat))
                {
                    return AvailableFlag.Media;
                }
            }
            foreach (var drawingFileFormat in QwilightComponent.DrawingFileFormats)
            {
                if (filePath.IsTailCaselsss(drawingFileFormat))
                {
                    return AvailableFlag.Drawing;
                }
            }
            return AvailableFlag.Not;
        }

        public static string GetAvailable(string filePath, AvailableFlag availableFlags)
        {
            if (File.Exists(filePath))
            {
                return filePath;
            }
            return Utility.GetFiles(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}.*").Order().FirstOrDefault(targetFilePath =>
            {
                if (File.Exists(targetFilePath))
                {
                    if ((availableFlags & AvailableFlag.Audio) == AvailableFlag.Audio && GetAvailable(targetFilePath) == AvailableFlag.Audio)
                    {
                        return true;
                    }
                    if ((availableFlags & AvailableFlag.Drawing) == AvailableFlag.Drawing && GetAvailable(targetFilePath) == AvailableFlag.Drawing)
                    {
                        return true;
                    }
                    if ((availableFlags & AvailableFlag.Media) == AvailableFlag.Media && GetAvailable(targetFilePath) == AvailableFlag.Media)
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        public static string CompileSiteYells(string siteYells)
        {
            var m = GetSiteYellsComputer().Matches(siteYells);
            return m.Count < 2 ? m.SingleOrDefault()?.Value ?? string.Empty : string.Empty;
        }

        public static string GetFault(Exception e)
        {
            var builder = new StringBuilder();
            var fault = e;
            builder.AppendLine(fault.ToString());
            while ((fault = fault.InnerException) != null)
            {
                builder.AppendLine();
                builder.AppendLine(fault.ToString());
            }
            return builder.ToString();
        }

        public static double GetDistance(Component value, Queue<KeyValuePair<double, double>> waitBPMMap, double loopingCounter, double targetLoopingCounter, out double lastBPM)
        {
            lastBPM = double.NaN;
            var distance = 0.0;
            while (waitBPMMap.Count > 0)
            {
                var (wait, bpm) = waitBPMMap.Peek();
                if (wait < targetLoopingCounter)
                {
                    distance += value.LogicalYMillis * (wait - loopingCounter);
                    loopingCounter = wait;
                    value.SetBPM(bpm);
                    lastBPM = bpm;
                    waitBPMMap.Dequeue();
                }
                else
                {
                    break;
                }
            }
            return distance + value.LogicalYMillis * (targetLoopingCounter - loopingCounter);
        }

        public static void Into<TKey, TValue>(this IDictionary<TKey, List<TValue>> valueMap, TKey i, TValue value)
        {
            if (valueMap.TryGetValue(i, out var values))
            {
                values.Add(value);
                valueMap[i] = values;
            }
            else
            {
                valueMap[i] = new()
                {
                    value
                };
            }
        }

        public static void Into<TKey, TValue>(this IDictionary<TKey, SortedSet<TValue>> valueMap, TKey i, TValue value)
        {
            if (valueMap.TryGetValue(i, out var values))
            {
                values.Add(value);
                valueMap[i] = values;
            }
            else
            {
                valueMap[i] = new()
                {
                    value
                };
            }
        }

        public static string GetHighestInputCountText(double averageInputCount, int highestInputCount, double audioMultiplier) => $"{(audioMultiplier * averageInputCount).ToString("#,##0.## / s")} (PEAK: {(audioMultiplier * highestInputCount).ToString("#,##0.## / s")})";

        public static string GetGenreText(string genre) => string.IsNullOrEmpty(genre) || genre.StartsWith('#') ? genre : $"#{genre}";

        public static string GetBPMText(double bpm, double multiplier, double audioMultiplier) => $"{Math.Round(bpm * audioMultiplier, 2)} BPM×{multiplier.ToString("0.0")}＝{Math.Round(bpm * audioMultiplier * multiplier)} BPM (×{audioMultiplier.ToString("0.00")})";

        public static string GetBPMText(double bpm, double audioMultiplier) => $"{Math.Round(bpm * audioMultiplier)} BPM (×{audioMultiplier.ToString("0.00")})";

        public static string GetLengthText(double length)
        {
            var sLength = length / 1000.0;
            return $"{((int)(sLength / 60))}：{((int)(sLength % 60)).ToString("00")}";
        }

        public static void LoopBefore<TValue>(Queue<KeyValuePair<double, TValue>> waitMap, double loopingCounter, double delta, Action<double, TValue> onHandle)
        {
            while (waitMap.Count > 0)
            {
                var (wait, value) = waitMap.Peek();
                var waitModified = wait + delta;
                if (waitModified < loopingCounter)
                {
                    onHandle(waitModified, value);
                    waitMap.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        public static string GetText(YamlNode yamlNode, string target, string defaultValue = null)
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

        public static CommentItem[] GetCommentItems(JSON.TwilightWwwComment.Comment[] www, BaseNoteFile noteFile)
        {
            var data = www.Select(data =>
            {
                var date = data.date.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(data.date.Value).LocalDateTime : null as DateTime?;
                var audioMultiplier = Math.Round(data.audioMultiplier, 2);
                var sentMultiplier = data.multiplier;
                return new CommentItem(data.avatarID, (DefaultCompute.InputFlag)data.inputFlags)
                {
                    IsTwilightComment = true,
                    NoteFileCount = 1,
                    Date = date ?? DateTime.MinValue,
                    DateText = date?.ToString("yyyy-MM-dd HH:mm:ss") ?? "❌",
                    CommentID = data.commentID,
                    AvatarName = data.avatarName,
                    ModeComponentValue = new()
                    {
                        CanModifyMultiplier = false,
                        CanModifyAudioMultiplier = false,
                        ComputingValue = noteFile,
                        SentMultiplier = sentMultiplier,
                        MultiplierValue = noteFile.BPM * audioMultiplier * sentMultiplier,
                        AutoModeValue = (ModeComponent.AutoMode)data.autoMode,
                        NoteSaltModeValue = (ModeComponent.NoteSaltMode)data.noteSaltMode,
                        AudioMultiplier = audioMultiplier,
                        FaintNoteModeValue = (ModeComponent.FaintNoteMode)data.faintNoteMode,
                        JudgmentModeValue = (ModeComponent.JudgmentMode)data.judgmentMode,
                        HitPointsModeValue = (ModeComponent.HitPointsMode)data.hitPointsMode,
                        NoteMobilityModeValue = (ModeComponent.NoteMobilityMode)data.noteMobilityMode,
                        LongNoteModeValue = (ModeComponent.LongNoteMode)data.longNoteMode,
                        InputFavorModeValue = (ModeComponent.InputFavorMode)data.inputFavorMode,
                        NoteModifyModeValue = (ModeComponent.NoteModifyMode)data.noteModifyMode,
                        LowestJudgmentConditionModeValue = (ModeComponent.LowestJudgmentConditionMode)data.lowestJudgmentConditionMode,
                        Salt = data.salt
                    },
                    Stand = data.stand,
                    Band = data.band,
                    IsP = data.isP,
                    Point = data.point,
                    TwilightCommentary = string.IsNullOrEmpty(data.commentary) ? string.Empty : $"💬 {data.commentary}",
                    IsPaused = data.isPaused
                };
            }).ToArray();
            for (var i = data.Length - 1; i >= 0; --i)
            {
                data[i].CommentPlace0Text = $"＃{i + 1}";
                data[i].CommentPlace1Text = $"／{data.Length}";
            }
            return data;
        }

        public static void SetFilledMediaDrawing(ref Bound r, bool isMediaFill, double mediaSoftwareLength, double mediaSoftwareHeight, double mediaPosition0, double mediaPosition1, double mediaLength, double mediaHeight)
        {
            if (isMediaFill)
            {
                r.Set(mediaPosition0, mediaPosition1, mediaLength, mediaHeight);
            }
            else
            {
                if (mediaLength / mediaSoftwareLength > mediaHeight / mediaSoftwareHeight)
                {
                    mediaSoftwareLength = mediaHeight * mediaSoftwareLength / mediaSoftwareHeight;
                    r.Set(mediaPosition0 + (mediaLength - mediaSoftwareLength) / 2, mediaPosition1, mediaSoftwareLength, mediaHeight);
                }
                else
                {
                    mediaSoftwareHeight = mediaLength * mediaSoftwareHeight / mediaSoftwareLength;
                    r.Set(mediaPosition0, mediaPosition1 + (mediaHeight - mediaSoftwareHeight) / 2, mediaLength, mediaSoftwareHeight);
                }
            }
        }

        public static void PaintAudioVisualizer(CanvasDrawingSession targetSession, ref Bound r, int audioVisualizerFaint, double audioVisualizerPosition0, double audioVisualizerPosition1, double audioVisualizerLength, double audioVisualizerHeight)
        {
            if (Configure.Instance.AudioVisualizer && audioVisualizerFaint > 0)
            {
                var audioMainVisualizerPaint = DrawingSystem.Instance.AudioVisualizerMainPaints[audioVisualizerFaint];
                var audioInputVisualizerPaint = DrawingSystem.Instance.AudioVisualizerInputPaints[audioVisualizerFaint];
                var audioVisualizerCount = Configure.Instance.AudioVisualizerCount;
                var audioVisualizerUnitLength = audioVisualizerLength / audioVisualizerCount;
                for (var i = audioVisualizerCount - 1; i >= 0; --i)
                {
                    var mainAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.MainAudio, i);
                    var inputAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.InputAudio, i);
                    if (mainAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, mainAudioVisualizerValue), audioVisualizerUnitLength, mainAudioVisualizerValue);
                        targetSession.FillRectangle(r, audioMainVisualizerPaint);
                    }
                    if (inputAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, inputAudioVisualizerValue), audioVisualizerUnitLength, inputAudioVisualizerValue);
                        targetSession.FillRectangle(r, audioInputVisualizerPaint);
                    }
                }
            }
        }

        public static void PaintAudioVisualizer(DrawingContext targetSession, ref Bound r, int audioVisualizerFaint, double audioVisualizerPosition0, double audioVisualizerPosition1, double audioVisualizerLength, double audioVisualizerHeight)
        {
            if (Configure.Instance.AudioVisualizer && audioVisualizerFaint > 0)
            {
                var audioMainVisualizerPaint = Configure.Instance.AudioVisualizerMainPaints[audioVisualizerFaint];
                var audioInputVisualizerPaint = Configure.Instance.AudioVisualizerInputPaints[audioVisualizerFaint];
                var audioVisualizerCount = Configure.Instance.AudioVisualizerCount;
                var audioVisualizerUnitLength = audioVisualizerLength / audioVisualizerCount;
                for (var i = audioVisualizerCount - 1; i >= 0; --i)
                {
                    var mainAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.MainAudio, i);
                    var inputAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.InputAudio, i);
                    if (mainAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, mainAudioVisualizerValue), audioVisualizerUnitLength, mainAudioVisualizerValue);
                        targetSession.DrawRectangle(audioMainVisualizerPaint, null, r);
                    }
                    if (inputAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, inputAudioVisualizerValue), audioVisualizerUnitLength, inputAudioVisualizerValue);
                        targetSession.DrawRectangle(audioInputVisualizerPaint, null, r);
                    }
                }
            }
        }

        public static bool? IsItemsEqual<T>(ICollection<T> values, ICollection<T> lastValues, IEqualityComparer<T> onEqual = null)
        {
            if (values.Count != lastValues.Count)
            {
                return false;
            }
            else
            {
                onEqual ??= EqualityComparer<T>.Default;
                for (var i = values.Count - 1; i >= 0; --i)
                {
                    var value = values.ElementAt(i);
                    if (lastValues.Contains(value, onEqual))
                    {
                        if (!onEqual.Equals(lastValues.ElementAt(i), value))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void SetBaseUIItem(UIItem src, UIItem target)
        {
            if (!Configure.Instance.BaseUIConfigureValues.ContainsKey(target.Title))
            {
                Configure.Instance.BaseUIConfigureValues[target.Title] = new();
            }
            Configure.Instance.BaseUIItemValue = target;
            foreach (var toNotifyUIItem in BaseUI.Instance.UIItems)
            {
                toNotifyUIItem.NotifyUI();
            }
            BaseUI.Instance.LoadUI(src, target);
        }

        public static void SetUIItem(UIItem src, UIItem target)
        {
            if (!Configure.Instance.UIConfigureValuesV2.ContainsKey(target.Title))
            {
                Configure.Instance.UIConfigureValuesV2[target.Title] = new();
            }
            Configure.Instance.UIItemValue = target;
            foreach (var toNotifyUIItem in UI.Instance.UIItems)
            {
                toNotifyUIItem.NotifyUI();
            }
            UI.Instance.LoadUI(src, target);
        }

        public static string GetDefaultAvatarID(string avatarID) => avatarID?.Substring(avatarID.IndexOf('@') + 1);

        public static string GetLetter(Key rawInput)
        {
            if (Key.A <= rawInput && rawInput <= Key.Z)
            {
                return ((char)(21 + rawInput)).ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static T[] GetInputData<T>(IDirectInputDevice8 dInputController) where T : unmanaged, IStateUpdate
        {
            var data = Array.Empty<T>();
            try
            {
                if (!dInputController.Acquire().Failure && !dInputController.Poll().Failure)
                {
                    data = dInputController.GetBufferedData<T>();
                }
            }
            catch (SharpGenException)
            {
            }
            return data;
        }

        public static T Max<T>(T value0, T value1, T value2) where T : IComparable<T>
        {
            var value = value0;
            if (value.CompareTo(value1) < 0)
            {
                value = value1;
            }
            if (value.CompareTo(value2) < 0)
            {
                value = value2;
            }
            return value;
        }

        public static string GetSiteName(string siteName) => siteName switch
        {
            "@Comment" => LanguageSystem.Instance.CommentSiteName,
            "@Default" => LanguageSystem.Instance.DefaultSiteName,
            "@Platform" => LanguageSystem.Instance.PlatformSiteName,
            _ => siteName
        };

        public static string GetPlatformText(string title, string artist, string genre, string levelText) => $"{levelText} {artist} - {title} {GetGenreText(genre)}";

        public static bool HasInput(VirtualKey rawInput) => PInvoke.GetAsyncKeyState((int)rawInput) <= -32767;

        public static void ModifyHwMode(HwMode hwMode)
        {
            if (Configure.Instance.SetHwMode)
            {
                var rawHwModeBefore = new DEVMODEW();
                PInvoke.EnumDisplaySettings(null, ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref rawHwModeBefore);
                if (hwMode != new HwMode(rawHwModeBefore.dmPelsWidth, rawHwModeBefore.dmPelsHeight, rawHwModeBefore.dmDisplayFrequency))
                {
                    var rawHwMode = new DEVMODEW
                    {
                        dmSize = (ushort)Marshal.SizeOf<DEVMODEW>(),
                        dmFields = DEVMODE_FIELD_FLAGS.DM_PELSWIDTH | DEVMODE_FIELD_FLAGS.DM_PELSHEIGHT | DEVMODE_FIELD_FLAGS.DM_DISPLAYFREQUENCY,
                        dmPelsWidth = hwMode.Length,
                        dmPelsHeight = hwMode.Height,
                        dmDisplayFrequency = hwMode.Hz,
                    };
                    PInvoke.ChangeDisplaySettings(rawHwMode, 0);
                }
            }
        }
    }
}