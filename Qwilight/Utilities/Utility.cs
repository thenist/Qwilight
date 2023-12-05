using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using SharpGen.Runtime;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using IDirectInputDevice8 = Vortice.DirectInput.IDirectInputDevice8;
using IStateUpdate = Vortice.DirectInput.IStateUpdate;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        [GeneratedRegex("(http|https|mailto):\\/\\/[^ ]+", RegexOptions.IgnoreCase)]
        private static partial Regex GetSiteYellsComputer();

        public static string CompileSiteYells(string siteYells)
        {
            var m = GetSiteYellsComputer().Matches(siteYells);
            return m.Count < 2 ? m.SingleOrDefault()?.Value ?? string.Empty : string.Empty;
        }

        static readonly char[] _delimiters = { '[', '(' };

        public static int GetDigit(int value) => value > 0 ? (int)(Math.Log10(value) + 1) : 1;

        public static string GetTitle(string title)
        {
            var titlePosition = title.LastIndexOfAny(_delimiters);
            return (titlePosition >= 0 ? title.Substring(0, titlePosition) : title).Trim();
        }

        public static void HandleUIAudio(string audioFileName, string defaultFileName = null, PausableAudioHandler pausableAudioHandler = null, double fadeInLength = 0.0)
        {
            if (!BaseUI.Instance.HandleAudio(audioFileName, defaultFileName, pausableAudioHandler, fadeInLength))
            {
                UI.Instance.HandleAudio(audioFileName, defaultFileName, pausableAudioHandler, fadeInLength);
            }
        }

        public static string FormatLength(long value)
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

        public static void ViewItems(FrameworkElement element)
        {
            var elements = (element as FrameworkElement).ContextMenu;
            elements.DataContext = (element as FrameworkElement).DataContext;
            elements.Placement = PlacementMode.Mouse;
            elements.IsOpen = true;
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

        public static double GetMove(double target, double src, double framerate = 60.0)
        {
            var distance = target - src;
            if (distance != 0.0)
            {
                if (distance > 0.0)
                {
                    if (distance > 0.1)
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
                    if (distance < -0.1)
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
                UIHandler.Instance.HandleParallel(Hande);
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
                if (dataSet.TryGetValue(dataItem, out int value))
                {
                    dataSet[dataItem] = ++value;
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

        public static double SetCommentWait(Component.CommentWaitDate commentWaitDate, double audioMultiplier, double wait)
        {
            if (commentWaitDate < Component.CommentWaitDate._1_3_11)
            {
                wait -= 3000.0;
            }
            if (commentWaitDate < Component.CommentWaitDate._1_6_4)
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

        public static void NewValue<TKey, TValue, U>(this IDictionary<TKey, U> valueMap, TKey i, TValue value) where U : ICollection<TValue>, new()
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

        public static string GetHighestInputCountText(double averageInputCount, int highestInputCount, double audioMultiplier) => $"{(audioMultiplier * averageInputCount).ToString("#,##0.## / s")} (PEAK: {(audioMultiplier * highestInputCount).ToString("#,##0 / s")})";

        public static string GetGenreText(string genre) => string.IsNullOrEmpty(genre) || genre.StartsWith('#') ? genre : $"#{genre}";

        public static string GetBPMText(double bpm, double multiplier, double audioMultiplier) => $"{Math.Round(bpm * audioMultiplier, 2)} BPM×{multiplier.ToString("0.0")}＝{Math.Round(bpm * audioMultiplier * multiplier)} BPM (×{audioMultiplier.ToString("0.00")})";

        public static string GetBPMText(double bpm, double audioMultiplier) => $"{Math.Round(bpm * audioMultiplier)} BPM (×{audioMultiplier.ToString("0.00")})";

        public static string GetLengthText(double length)
        {
            var sLength = length / 1000.0;
            return $"{((int)(sLength / 60))}：{((int)(sLength % 60)).ToString("00")}";
        }

        public static string GetPlatformText(string title, string artist, string genreText, string levelText) => $"{levelText} {artist} - {title} {genreText}";

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

        public static CommentItem[] GetCommentItems(JSON.TwilightWwwComment.Comment[] www, BaseNoteFile noteFile)
        {
            var data = www.Select(data =>
            {
                var date = data.date;
                var dateValue = date.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(date.Value).LocalDateTime : null as DateTime?;
                var audioMultiplier = Math.Round(data.audioMultiplier, 2);
                var sentMultiplier = data.multiplier;
                return new CommentItem(data.avatarID, (DefaultCompute.InputFlag)data.inputFlags)
                {
                    IsTwilightComment = true,
                    NoteFileCount = 1,
                    Date = dateValue ?? DateTime.MinValue,
                    DateText = dateValue?.ToString("yyyy-MM-dd HH:mm:ss") ?? "❌",
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

        public static T[] GetDInputData<T>(IDirectInputDevice8 dInputController) where T : unmanaged, IStateUpdate
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

        public static T GetDate<T>(Version date, params string[] dates)
        {
            for (var i = dates.Length - 1; i >= 0; --i)
            {
                if (new Version(dates[i]) <= date)
                {
                    return (T)(object)(i + 1);
                }
            }
            return (T)(object)0;
        }
    }
}