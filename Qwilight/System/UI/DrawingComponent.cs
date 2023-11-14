using MoonSharp.Interpreter;
using Qwilight.Compute;
using Qwilight.Utilities;
using System.Text;

namespace Qwilight
{
    public sealed class DrawingComponent
    {
        public int judgmentPaintComposition;
        public int hitNotePaintComposition;
        public int hitInputPaintComposition;

        public float audioVisualizerPosition0;
        public float audioVisualizerPosition1;
        public float audioVisualizerLength;
        public float audioVisualizerHeight;

        public float mediaPosition0;
        public float mediaPosition1;
        public float mediaLength;
        public float mediaHeight;

        public float mainPosition1;
        public float mainHeight;

        public float mainPosition;
        public float p2Position;

        public float judgmentMainPosition;

        public float mainWall0Position1;
        public float mainWall0Length;
        public float mainWall0Height;

        public float mainWall1Position1;
        public float mainWall1Length;
        public float mainWall1Height;

        public float hitPointsPosition0;
        public float hitPointsPosition1;
        public float hitPointsLength;
        public float hitPointsHeight;
        public int hitPointsSystem;

        public float levelPosition0;
        public float levelPosition1;
        public float levelLength;
        public float levelHeight;

        public float bandPosition0;
        public float bandPosition1;
        public float binBandLength;
        public float binBandHeight;
        public int bandSystem;
        public float enlargeBand;

        public float judgmentMeterPosition0;
        public float judgmentMeterPosition1;
        public float binJudgmentMeterLength;
        public float binJudgmentMeterHeight;
        public float judgmentMeterFrontDrawingLength;
        public float judgmentMeterUnitDrawingLength;
        public int judgmentMeterSystem;

        public float hunterPosition0;
        public float hunterPosition1;
        public float binHunterLength;
        public float binHunterHeight;
        public float hunterFrontDrawingLength;
        public int hunterSystem;

        public float standCommaDrawingLength;
        public float standPosition0;
        public float standPosition1;
        public float binStandLength;
        public float binStandHeight;
        public float standSystem;

        public float pointStopPointDrawingLength;
        public float pointPosition0;
        public float pointPosition1;
        public float binPointLength;
        public float binPointHeight;
        public float pointUnitDrawingLength;
        public int pointSystem;

        public float bpmPosition0;
        public float bpmPosition1;
        public float binBPMLength;
        public float binBPMHeight;
        public float bpmUnitDrawingLength;
        public int bpmSystem;

        public float multiplierPosition0;
        public float multiplierPosition1;
        public float binMultiplierLength;
        public float binMultiplierHeight;
        public float multiplierStopPointDrawingLength;
        public float multiplierUnitDrawingLength;
        public int multiplierSystem;

        public float netPosition0;
        public float netPosition1;
        public int netSystem;

        public float pausePosition0;
        public float pausePosition1;
        public float pauseLength;
        public float pauseHeight;
        public int pauseSystem;

        public float statusPosition0;
        public float statusPosition1;
        public float statusLength;
        public float statusHeight;
        public int statusSystem;

        public float statusSliderPosition0;
        public float statusSliderPosition1;
        public float statusSliderLength;
        public float statusSliderHeight;
        public float statusSliderContentsLength;
        public float statusSliderContentsHeight;
        public int statusSliderSystem;

        public float hmsPosition0;
        public float hmsPosition1;
        public float binHmsLength;
        public float binHmsHeight;
        public int hmsSystem;
        public float hmsColonDrawingLength;
        public float hmsSlashDrawingLength;

        public float judgmentPointsPosition0;
        public float judgmentPointsPosition1;
        public float judgmentPointsLength;
        public float judgmentPointsHeight;
        public int judgmentPointsSystem;

        public float band1Position0;
        public float band1Position1;
        public float band1Length;
        public float band1Height;
        public int band1System;

        public float lastPosition0;
        public float lastPosition1;
        public float lastLength;
        public float lastHeight;
        public int lastSystem;

        public float autoMainPosition0;
        public float autoMainPosition1;
        public float autoMainLength;
        public float autoMainHeight;
        public int autoMainSystem;

        public float audioMultiplierPosition0;
        public float audioMultiplierPosition1;
        public float binAudioMultiplierLength;
        public float binAudioMultiplierHeight;
        public float audioMultiplierStopPointDrawingLength;
        public float audioMultiplierUnitDrawingLength;
        public int audioMultiplierSystem;

        public float hitPointsVisualizerPosition0;
        public float hitPointsVisualizerPosition1;
        public float binHitPointsVisualizerLength;
        public float binHitPointsVisualizerHeight;
        public float hitPointsVisualizerUnitDrawingLength;
        public int hitPointsVisualizerSystem;

        public float limiterPosition1;
        public float limiterLength;
        public float limiterHeight;

        public float judgmentVisualizerPosition0;
        public float judgmentVisualizerPosition1;
        public float judgmentVisualizerLength;
        public float judgmentVisualizerHeight;
        public float judgmentVisualizerContentsLength;
        public float judgmentVisualizerContentsHeight;
        public int judgmentVisualizerSystem;

        public float judgmentInputVisualizerPosition0;
        public float judgmentInputVisualizerPosition1;
        public float judgmentInputVisualizerLength;
        public float judgmentInputVisualizerHeight;

        public int highestJudgmentValueSystem;
        public float highestJudgmentValuePosition0;
        public float highestJudgmentValuePosition1;
        public float binHighestJudgmentValueLength;
        public float binHighestJudgmentValueHeight;

        public int higherJudgmentValueSystem;
        public float higherJudgmentValuePosition0;
        public float higherJudgmentValuePosition1;
        public float binHigherJudgmentValueLength;
        public float binHigherJudgmentValueHeight;

        public int highJudgmentValueSystem;
        public float highJudgmentValuePosition0;
        public float highJudgmentValuePosition1;
        public float binHighJudgmentValueLength;
        public float binHighJudgmentValueHeight;

        public int lowJudgmentValueSystem;
        public float lowJudgmentValuePosition0;
        public float lowJudgmentValuePosition1;
        public float binLowJudgmentValueLength;
        public float binLowJudgmentValueHeight;

        public int lowerJudgmentValueSystem;
        public float lowerJudgmentValuePosition0;
        public float lowerJudgmentValuePosition1;
        public float binLowerJudgmentValueLength;
        public float binLowerJudgmentValueHeight;

        public int lowestJudgmentValueSystem;
        public float lowestJudgmentValuePosition0;
        public float lowestJudgmentValuePosition1;
        public float binLowestJudgmentValueLength;
        public float binLowestJudgmentValueHeight;

        public int highestBandSystem;
        public float highestBandPosition0;
        public float highestBandPosition1;
        public float binHighestBandLength;
        public float binHighestBandHeight;

        public int inputVisualizerSystem;
        public float inputVisualizerPosition0;
        public float inputVisualizerPosition1;
        public float binInputVisualizerLength;
        public float binInputVisualizerHeight;

        public float titlePosition0;
        public float titlePosition1;
        public float titleLength;
        public float titleHeight;
        public int titleSystem0;
        public int titleSystem1;

        public float artistPosition0;
        public float artistPosition1;
        public float artistLength;
        public float artistHeight;
        public int artistSystem0;
        public int artistSystem1;

        public float genrePosition0;
        public float genrePosition1;
        public float genreLength;
        public float genreHeight;
        public int genreSystem0;
        public int genreSystem1;

        public float levelTextPosition0;
        public float levelTextPosition1;
        public float levelTextLength;
        public float levelTextHeight;
        public int levelTextSystem0;
        public int levelTextSystem1;

        public float wantLevelPosition0;
        public float wantLevelPosition1;
        public float wantLevelLength;
        public float wantLevelHeight;
        public int wantLevelSystem0;
        public int wantLevelSystem1;

        public float judgmentPaintPosition0;
        public float judgmentPaintPosition1;
        public float judgmentPaintLength;
        public float judgmentPaintHeight;
        public int judgmentPaintSystem;

        public int earlyValueSystem;
        public float earlyValuePosition0;
        public float earlyValuePosition1;
        public float binEarlyValueLength;
        public float binEarlyValueHeight;

        public int lateValueSystem;
        public float lateValuePosition0;
        public float lateValuePosition1;
        public float binLateValueLength;
        public float binLateValueHeight;

        public int judgmentVSVisualizerSystem;
        public float judgmentVSVisualizerPosition0;
        public float judgmentVSVisualizerPosition1;
        public float binJudgmentVSVisualizerLength;
        public float binJudgmentVSVisualizerHeight;
        public float judgmentVSVisualizerStopPointDrawingLength;

        public float pausedUnpausePosition0;
        public float pausedUnpausePosition1;
        public float pausedUnpauseLength;
        public float pausedUnpauseHeight;

        public float pausedStopPosition0;
        public float pausedStopPosition1;
        public float pausedStopLength;
        public float pausedStopHeight;

        public float pausedUndoPosition0;
        public float pausedUndoPosition1;
        public float pausedUndoLength;
        public float pausedUndoHeight;

        public float pausedConfigurePosition0;
        public float pausedConfigurePosition1;
        public float pausedConfigureLength;
        public float pausedConfigureHeight;

        public float assistTextPosition1;
        public float inputAssistTextPosition1;

        public int autoMainFrame;
        public double autoMainFramerate;
        public int judgmentMainFrame;
        public int noteFrame;
        public double noteFramerate;
        public int pauseFrame;
        public int mainFrame;
        public double mainFramerate;
        public double judgmentMainFramerate;
        public int mainJudgmentMeterFrame;
        public double mainJudgmentMeterFramerate;
        public int noteHitFrame;
        public double noteHitFramerate;
        public int longNoteHitFrame;
        public double longNoteHitFramerate;
        public int inputFrame;
        public double inputFramerate;
        public int levelFrame;
        public double levelFramerate;
        public int hitInputPaintFrame;
        public double hitInputPaintFramerate;
        public int bandFrame;
        public double bandFramerate;
        public int judgmentFrame;
        public double judgmentFramerate;
        public int band1Frame;
        public double band1Framerate;
        public int lastFrame;
        public double lastFramerate;
        public int longNoteHitLoopFrame;
        public int lastEnlargedBandLoopFrame;

        public int altWall0;
        public int altWall1;
        public int altHitPoints;
        public int altBand;
        public int altJudgmentMeter;
        public int altStand;
        public int altPoint;
        public int altBPM;
        public int altMultiplier;
        public int altNet;
        public int altPause;
        public int altStatus;
        public int altStatusSlider;
        public int altHms;
        public int altJudgmentPoints;
        public int altLevel;
        public int altAudioMultiplier;
        public int altHitPointsVisualizer;
        public int altJudgmentVisualizer;
        public int altHighestJudgmentValue;
        public int altHigherJudgmentValue;
        public int altHighJudgmentValue;
        public int altLowJudgmentValue;
        public int altLowerJudgmentValue;
        public int altLowestJudgmentValue;
        public int altHighestBand;
        public int altInputVisualizer;
        public int altBand1;
        public int altLast;
        public int altHunter;
        public int altAutoMain;
        public int altEarlyValue;
        public int altLateValue;
        public int altJudgmentVSVisualizer;
        public int altJudgmentInputVisualizer;
        public int altMedia;
        public int altTitle;
        public int altArtist;
        public int altGenre;
        public int altLevelText;
        public int altWantLevel;

        public float[] noteHeights = new float[53];
        public float[] noteHeightJudgments = new float[53];
        public float[] inputPosition0s = new float[53];
        public float[] inputPosition1s = new float[53];
        public float[] inputLengths = new float[53];
        public float[] inputHeights = new float[53];
        public float[] longNoteTailEdgeHeights = new float[53];
        public float[] longNoteTailEdgePositions = new float[53];
        public float[] longNoteTailContentsHeights = new float[53];
        public float[] longNoteFrontEdgeHeights = new float[53];
        public float[] longNoteFrontEdgePositions = new float[53];
        public float[] longNoteFrontContentsHeights = new float[53];
        public float[] autoInputPosition1s = new float[53];
        public float[] autoInputHeights = new float[53];
        public float[] judgmentMainPosition1s = new float[53];
        public float[] judgmentMainHeights = new float[53];
        public float[] hitNotePaintPosition0s = new float[53];
        public float[] hitNotePaintPosition1s = new float[53];
        public float[] hitNotePaintLengths = new float[53];
        public float[] hitNotePaintHeights = new float[53];
        public float[] hitLongNotePaintPosition0s = new float[53];
        public float[] hitLongNotePaintPosition1s = new float[53];
        public float[] hitLongNotePaintLengths = new float[53];
        public float[] hitLongNotePaintHeights = new float[53];
        public float[] mainJudgmentMeterPosition1s = new float[53];
        public float[] mainJudgmentMeterHeights = new float[53];

        public readonly object PaintPropertyCSX = new();
        public readonly List<int> PaintPropertyIDs = new();
        public readonly Dictionary<PaintProperty.ID, int>[] PaintPropertyIntMap = new Dictionary<PaintProperty.ID, int>[UI.HighestPaintPropertyID];
        public readonly Dictionary<PaintProperty.ID, double>[] PaintPropertyValueMap = new Dictionary<PaintProperty.ID, double>[UI.HighestPaintPropertyID];
        /// <summary>
        /// 라인 길이
        /// 플로팅 노트는 라인 길이가 0 이다.
        /// </summary>
        public readonly float[] MainNoteLengthMap = new float[53];
        /// <summary>
        /// 노트 길이
        /// </summary>
        public readonly float[] DrawingNoteLengthMap = new float[53];
        /// <summary>
        /// 노트는 여기서부터 그린다.
        /// </summary>
        public readonly float[] MainNoteLengthLevyingMap = new float[53];
        /// <summary>
        /// 노트는 여기까지 그린다.
        /// </summary>
        public readonly float[] MainNoteLengthBuiltMap = new float[53];

        public float p1BuiltLength;
        public float p2BuiltLength;

        public DrawingComponent()
        {
            for (var i = PaintPropertyValueMap.Length - 1; i >= 0; --i)
            {
                PaintPropertyValueMap[i] = new();
            }
            for (var i = PaintPropertyIntMap.Length - 1; i >= 0; --i)
            {
                PaintPropertyIntMap[i] = new();
            }
        }

        public void SetValue(DefaultCompute defaultComputer)
        {
            var valueMap = new Dictionary<string, double>();
            var intMap = new Dictionary<string, int>();
            var has2P = defaultComputer.Has2P;
            var inputMode = defaultComputer.InputMode;
            var inputCount = Component.InputCounts[(int)inputMode];
            var autoableInputCount = Component.AutoableInputCounts[(int)inputMode];
            var noteLengths = new float[inputCount + 1];
            var floatingNotePosition0s = new float[inputCount + 1];
            var floatingNoteLengths = new float[inputCount + 1];
            var slashNotePosition0s = new float[inputCount + 1];
            var p1BuiltLengthFunc = new Func<double, float>(e => (float)(MainNoteLengthBuiltMap.Skip(1).Take(defaultComputer.Input1PCount).DefaultIfEmpty(0).Max() * e));
            var p2BuiltLengthFunc = new Func<double, float>(e => (float)(MainNoteLengthBuiltMap.Skip(1).Max() * e));
            var p1Length = new Func<double, float>(e => (float)(MainNoteLengthBuiltMap.Skip(1).Take(Component.Input1PCounts[(int)inputMode]).DefaultIfEmpty(0).Max() * e));
            var lsCaller = new Script();
            lsCaller.Globals["judgmentStage"] = defaultComputer.JudgmentStage;
            lsCaller.Globals["autoableInputCount"] = autoableInputCount;
            lsCaller.Globals["defaultInputCount"] = inputCount - autoableInputCount;
            lsCaller.Globals["inputCount"] = inputCount;
            lsCaller.Globals["has2P"] = has2P;
            var judgmentMainPosition1 = Configure.Instance.UIConfigureValue.JudgmentMainPosition1V2;
            lsCaller.Globals["judgmentMainPosition"] = judgmentMainPosition1 + 600.0;
            lsCaller.Globals["judgmentMainPositionV2"] = 1.0 + judgmentMainPosition1 / 600.0;
            lsCaller.Globals["judgmentMainPositionV3"] = judgmentMainPosition1;
            lsCaller.Globals["mainPosition"] = Configure.Instance.UIConfigureValue.MainPositions.GetValueOrDefault((int)inputMode);
            lsCaller.Globals["noteLength"] = Configure.Instance.UIConfigureValue.NoteLengths.GetValueOrDefault((int)inputMode);
            lsCaller.Globals["noteHeight"] = Configure.Instance.UIConfigureValue.NoteHeights.GetValueOrDefault((int)inputMode);
            lsCaller.Globals["bandPosition"] = Configure.Instance.UIConfigureValue.BandPositionV2;
            lsCaller.Globals["judgmentPaintPosition"] = Configure.Instance.UIConfigureValue.JudgmentPaintPosition;
            lsCaller.Globals["hitNotePaintArea"] = Configure.Instance.UIConfigureValue.HitNotePaintArea;
            lsCaller.Globals["inputMapping"] = (int)Configure.Instance.InputMappingValue;
            lsCaller.Globals["P1Length"] = p1Length;
            lsCaller.Globals["P1BuiltLength"] = p1BuiltLengthFunc;
            lsCaller.Globals["P2BuiltLength"] = p2BuiltLengthFunc;
            lsCaller.Globals["Wall"] = new Func<double, float>(e => (float)(e + valueMap["mainPosition"] + p1BuiltLength));
            lsCaller.Globals["Contents"] = new Func<double, float>(e => (float)(valueMap["mainPosition"] + p1BuiltLengthFunc(e)));
            UI.Instance.SetConfigures(lsCaller);

            if (!string.IsNullOrEmpty(UI.Instance.UILS))
            {
                lsCaller.DoString(UI.Instance.UILS);
            }

            foreach (var (valueID, value) in UI.Instance.ValueMap)
            {
                valueMap[valueID] = value;
            }
            foreach (var (valueID, value) in UI.Instance.IntMap)
            {
                intMap[valueID] = value;
            }
            foreach (var (valueID, value) in UI.Instance.AltMap)
            {
                intMap[valueID] = has2P ? value : 0;
            }

            // 이 항목들은 Wall, Contents 함수등 기반이 되므로 먼저 처리
            SaveValueMap("mainPosition");
            for (var i = UI.HighestNoteID; i > 0; --i)
            {
                SaveValueMap($"noteLength{i}");
                SaveValueMap($"floatingNotePosition0{i}");
                SaveValueMap($"floatingNoteLength{i}");
                SaveValueMap($"slashNotePosition0{i}");
            }
            SaveSplitMap("noteLength", ref noteLengths);
            SaveSplitMap("floatingNotePosition0", ref floatingNotePosition0s);
            SaveSplitMap("floatingNoteLength", ref floatingNoteLengths);
            SaveSplitMap("slashNotePosition0", ref slashNotePosition0s);

            Array.Clear(DrawingNoteLengthMap, 0, DrawingNoteLengthMap.Length);
            Array.Clear(MainNoteLengthMap, 0, MainNoteLengthMap.Length);
            for (var i = inputCount; i > 0; --i)
            {
                if (floatingNoteLengths[i] > 0.0)
                {
                    DrawingNoteLengthMap[i] = floatingNoteLengths[i];
                    MainNoteLengthMap[i] = 0F;
                }
                else
                {
                    DrawingNoteLengthMap[i] = noteLengths[i];
                    MainNoteLengthMap[i] = noteLengths[i];
                }
            }

            Array.Clear(MainNoteLengthLevyingMap, 0, MainNoteLengthLevyingMap.Length);
            Array.Clear(MainNoteLengthBuiltMap, 0, MainNoteLengthBuiltMap.Length);
            for (var i = 1; i <= inputCount; ++i)
            {
                if (floatingNoteLengths[i] > 0.0)
                {
                    MainNoteLengthLevyingMap[i] = floatingNotePosition0s[i];
                    MainNoteLengthBuiltMap[i] = MainNoteLengthLevyingMap[i] + floatingNoteLengths[i];
                }
                else
                {
                    var inputMappingValue = (int)defaultComputer.InputMappingValue;
                    var j = Component.InputMappingValues[inputMappingValue, (int)inputMode][i];
                    MainNoteLengthLevyingMap[i] += slashNotePosition0s[i];
                    for (var m = 1; m < j; ++m)
                    {
                        var o = Component.BasePaintMap[inputMappingValue, (int)inputMode][m];
                        MainNoteLengthLevyingMap[i] += MainNoteLengthMap[o] + slashNotePosition0s[o];
                    }
                    MainNoteLengthBuiltMap[i] = MainNoteLengthLevyingMap[i] + MainNoteLengthMap[Component.BasePaintMap[inputMappingValue, (int)inputMode][j]];
                }
            }
            p1BuiltLength = p1BuiltLengthFunc(1.0);
            p2BuiltLength = p2BuiltLengthFunc(1.0);

            // 위에서 여러 항목이 계산되므로 모든 항목을 다시 설정한다.
            foreach (var valueID in UI.Instance.ValueCallMap.Keys)
            {
                SaveValueMap(valueID, true);
            }
            foreach (var (valueID, values) in UI.Instance.IntCallMap)
            {
                if (!intMap.ContainsKey(valueID))
                {
                    try
                    {
                        intMap[valueID] = (int)lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                    }
                    catch
                    {
                        throw new ArgumentException($"{values[0]}({string.Join(", ", values.Skip(1))})");
                    }
                }
            }
            foreach (var (valueID, values) in UI.Instance.AltCallMap)
            {
                if (!intMap.ContainsKey(valueID))
                {
                    try
                    {
                        intMap[valueID] = has2P ? (int)lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number switch
                        {
                            0 => 0,
                            1 => 3,
                            2 => 2,
                            _ => 0
                        } : 0;
                    }
                    catch
                    {
                        throw new ArgumentException($"{values[0]}({string.Join(", ", values.Skip(1))})");
                    }
                }
            }

            void SaveValueMap(string target, bool doForce = false)
            {
                if ((doForce || !valueMap.ContainsKey(target)) && UI.Instance.ValueCallMap.TryGetValue(target, out var values))
                {
                    try
                    {
                        valueMap[target] = lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                    }
                    catch
                    {
                        throw new ArgumentException($"{values[0]}({string.Join(", ", values.Skip(1))})");
                    }
                }
            }

            SaveSplitMap("noteHeight", ref noteHeights);
            SaveSplitMap("noteHeightJudgment", ref noteHeightJudgments);
            SaveSplitMap("inputPosition0", ref inputPosition0s);
            SaveSplitMap("inputPosition1", ref inputPosition1s);
            SaveSplitMap("inputLength", ref inputLengths);
            SaveSplitMap("inputHeight", ref inputHeights);
            SaveSplitMap("longNoteTailEdgeHeight", ref longNoteTailEdgeHeights);
            SaveSplitMap("longNoteTailEdgePosition", ref longNoteTailEdgePositions);
            SaveSplitMap("longNoteTailContentsHeight", ref longNoteTailContentsHeights);
            SaveSplitMap("longNoteFrontEdgeHeight", ref longNoteFrontEdgeHeights);
            SaveSplitMap("longNoteFrontEdgePosition", ref longNoteFrontEdgePositions);
            SaveSplitMap("longNoteFrontContentsHeight", ref longNoteFrontContentsHeights);
            SaveSplitMap("autoInputPosition1", ref autoInputPosition1s);
            SaveSplitMap("autoInputHeight", ref autoInputHeights);
            SaveSplitMap("judgmentMainPosition1", ref judgmentMainPosition1s);
            SaveSplitMap("judgmentMainHeight", ref judgmentMainHeights);
            SaveSplitMap("hitNotePaintPosition0", ref hitNotePaintPosition0s);
            SaveSplitMap("hitNotePaintPosition1", ref hitNotePaintPosition1s);
            SaveSplitMap("hitNotePaintLength", ref hitNotePaintLengths);
            SaveSplitMap("hitNotePaintHeight", ref hitNotePaintHeights);
            SaveSplitMap("hitLongNotePaintPosition0", ref hitLongNotePaintPosition0s);
            SaveSplitMap("hitLongNotePaintPosition1", ref hitLongNotePaintPosition1s);
            SaveSplitMap("hitLongNotePaintLength", ref hitLongNotePaintLengths);
            SaveSplitMap("hitLongNotePaintHeight", ref hitLongNotePaintHeights);
            SaveSplitMap("mainJudgmentMeterPosition1", ref mainJudgmentMeterPosition1s);
            SaveSplitMap("mainJudgmentMeterHeight", ref mainJudgmentMeterHeights);

            void SaveSplitMap(string target, ref float[] data)
            {
                var drawingInputMode = UI.Instance.DrawingInputModeMap[(int)inputMode];
                for (var i = inputCount; i > 0; --i)
                {
                    data[i] = (float)valueMap[$"{target}{drawingInputMode[i]}"];
                }
            }

            lock (PaintPropertyCSX)
            {
                PaintPropertyIDs.Clear();
                for (var i = UI.Instance.PaintPropertyValues.Length - 1; i >= 0; --i)
                {
                    PaintPropertyValueMap[i].Clear();
                    PaintPropertyIntMap[i].Clear();

                    var paintPropertyValue = UI.Instance.PaintPropertyValues[i];
                    if (paintPropertyValue != null)
                    {
                        PaintPropertyIDs.Add(i);

                        foreach (var (valueID, value) in paintPropertyValue.ValueMap)
                        {
                            PaintPropertyValueMap[i][valueID] = value;
                        }
                        foreach (var (valueID, value) in paintPropertyValue.AltMap)
                        {
                            PaintPropertyIntMap[i][valueID] = value;
                        }
                        foreach (var (valueID, value) in paintPropertyValue.IntMap)
                        {
                            PaintPropertyIntMap[i][valueID] = value;
                        }
                        foreach (var (valueID, values) in paintPropertyValue.ValueCallMap)
                        {
                            if (!PaintPropertyValueMap[i].ContainsKey(valueID))
                            {
                                try
                                {
                                    PaintPropertyValueMap[i][valueID] = lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                                }
                                catch
                                {
                                    throw new ArgumentException($"{values[0]}({string.Join(", ", values.Skip(1))})");
                                }
                            }
                        }
                        foreach (var (valueID, values) in paintPropertyValue.IntCallMap)
                        {
                            if (!PaintPropertyIntMap[i].ContainsKey(valueID))
                            {
                                try
                                {
                                    PaintPropertyIntMap[i][valueID] = (int)lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number;
                                }
                                catch
                                {
                                    throw new ArgumentException($"{values[0]}({string.Join(", ", values.Skip(1))})");
                                }
                            }
                        }
                        foreach (var (valueID, values) in paintPropertyValue.AltCallMap)
                        {
                            if (!PaintPropertyIntMap[i].ContainsKey(valueID))
                            {
                                try
                                {
                                    PaintPropertyIntMap[i][valueID] = has2P ? (int)lsCaller.Call(lsCaller.Globals[values[0]], values.Skip(1).Select(value => Utility.ToFloat64(value) as object).ToArray()).Number switch
                                    {
                                        0 => 0,
                                        1 => 3,
                                        2 => 2,
                                        _ => 0
                                    } : 0;
                                }
                                catch
                                {
                                    throw new ArgumentException($"{values[0]}({string.Join(", ", values.Skip(1))})");
                                }
                            }
                        }
                    }
                }
            }

            var drawingComponent = typeof(DrawingComponent);
            foreach (var value in drawingComponent.GetFields().Where(value => !value.FieldType.IsArray && !value.IsInitOnly && value.Name != nameof(p1BuiltLength) && value.Name != nameof(p2BuiltLength)))
            {
                value.SetValue(this, default);
            }
            foreach (var (valueID, value) in valueMap)
            {
                drawingComponent.GetField(GetProperty(valueID))?.SetValue(this, (float)value);
            }
            foreach (var (valueID, value) in intMap)
            {
                drawingComponent.GetField(GetProperty(valueID))?.SetValue(this, value);
            }

            static string GetProperty(string property)
            {
                switch (property)
                {
                    case "alt-judgment-vs-visualizer":
                        return nameof(altJudgmentVSVisualizer);
                    case "alt-bpm":
                        return nameof(altBPM);
                    default:
                        property = property.Replace('!', '1');
                        var propertyLength = property.Length;
                        var builder = new StringBuilder();
                        for (var i = 0; i < propertyLength; ++i)
                        {
                            if (property[i] == '-')
                            {
                                builder.Append(property.Substring(i++ + 1, 1).ToUpperInvariant());
                            }
                            else
                            {
                                builder.Append(property[i]);
                            }
                        }
                        return builder.ToString();
                }
            }
        }
    }
}
