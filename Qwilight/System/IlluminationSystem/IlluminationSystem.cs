using Qwilight.Compute;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.IO;
using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public sealed class IlluminationSystem : IDisposable
    {
        public static readonly IlluminationSystem Instance = new();

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(IlluminationSystem));

        readonly BaseIlluminationSystem[] _targetSystems = new BaseIlluminationSystem[]
        {
            BWSystem.Instance, AuraSystem.Instance, K70System.Instance, LSSystem.Instance
        };

        readonly object _availableCSX = new();

        void Init()
        {
            foreach (var targetSystem in _targetSystems)
            {
                if (!targetSystem.IsHandling && targetSystem.IsAvailable)
                {
                    targetSystem.IsHandling = targetSystem.Init();
                }
            }
        }

        void OnBeforeHandle()
        {
            foreach (var targetSystem in _targetSystems)
            {
                if (targetSystem.IsHandling)
                {
                    targetSystem.OnBeforeHandle();
                }
            }
        }

        void SetInputColor(VirtualKey rawInput, uint value)
        {
            foreach (var targetSystem in _targetSystems)
            {
                if (targetSystem.IsHandling)
                {
                    targetSystem.SetInputColor(rawInput, value);
                }
            }
        }

        void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3)
        {
            foreach (var targetSystem in _targetSystems)
            {
                if (targetSystem.IsHandling)
                {
                    targetSystem.SetStatusColors(status, value0, value1, value2, value3);
                }
            }
        }

        void SetEtcColor(double failedValue, double meterValue)
        {
            foreach (var targetSystem in _targetSystems)
            {
                if (targetSystem.IsHandling)
                {
                    if (failedValue > 0.0)
                    {
                        targetSystem.SetEtcColor(GetFailedColor(failedValue));
                    }
                    else if (meterValue > 0.0)
                    {
                        targetSystem.SetEtcColor(GetValueColor(targetSystem.GetMeterColor(), meterValue));
                    }
                }
            }
        }

        void OnHandled()
        {
            foreach (var targetSystem in _targetSystems)
            {
                if (targetSystem.IsHandling)
                {
                    targetSystem.OnHandled();
                }
            }
        }

        public object HandlingCSX { get; } = new();

        public bool[] HasInputValues { get; } = new bool[53];

        public double[] InputValues { get; } = new double[53];

        public bool IsMeter { get; set; }

        public bool IsFailed { get; set; }

        bool IsAvailable => _targetSystems.Any(targetSystem => targetSystem.IsAvailable);

        public void HandleIfAvailable()
        {
            lock (_availableCSX)
            {
                if (IsAvailable)
                {
                    Monitor.Pulse(_availableCSX);
                }
            }
        }

        public void HandleSystem()
        {
            var voteViewModel = ViewModels.Instance.VoteValue;
            var wwwLevelViewModel = ViewModels.Instance.WwwLevelValue;
            var assistViewModel = ViewModels.Instance.AssistValue;
            var mainViewModel = ViewModels.Instance.MainValue;
            var inputViewModel = ViewModels.Instance.InputValue;
            var toNotifyViewModel = ViewModels.Instance.NotifyValue;
            var inputStandardViewModel = ViewModels.Instance.InputStandardValue;
            var siteContainerViewModel = ViewModels.Instance.SiteContainerValue;
            var valueConfigureViewModel = ViewModels.Instance.ConfigureValue;
            var defaultStatusInputs = new[] {
                VirtualKey.Number1,
                VirtualKey.Number2,
                VirtualKey.Number3,
                VirtualKey.Number4,
                VirtualKey.Number5,
                VirtualKey.Number6,
                VirtualKey.Number7,
                VirtualKey.Number8,
                VirtualKey.Number9,
                VirtualKey.Number0,
            };
            var defaultStatusInputsLength = defaultStatusInputs.Length;
            var defaultStatusInputsUnit = 1.0 / defaultStatusInputsLength;
            var mainHitPointsInputs = new[] {
                VirtualKey.Number1,
                VirtualKey.Number2,
                VirtualKey.Number3,
                VirtualKey.Number4,
                VirtualKey.Number5,
            };
            var mainHitPointsInputsLength = mainHitPointsInputs.Length;
            var mainHitPointsInputsUnit = 1.0 / mainHitPointsInputsLength;
            var mainStatusInputs = new[] {
                VirtualKey.Number6,
                VirtualKey.Number7,
                VirtualKey.Number8,
                VirtualKey.Number9,
                VirtualKey.Number0,
            };
            var mainStatusInputsLength = mainStatusInputs.Length;
            var mainStatusInputsUnit = 1.0 / mainStatusInputsLength;
            var audioVisualizerInputs = new VirtualKey[10][] {
                new[]
                {
                    VirtualKey.Z,
                    VirtualKey.A,
                    VirtualKey.Q,
                },
                new[]
                {
                    VirtualKey.X,
                    VirtualKey.S,
                    VirtualKey.W,
                },
                new[]
                {
                    VirtualKey.C,
                    VirtualKey.D,
                    VirtualKey.E,
                },
                new[]
                {
                    VirtualKey.V,
                    VirtualKey.F,
                    VirtualKey.R,
                },
                new[]
                {
                    VirtualKey.B,
                    VirtualKey.G,
                    VirtualKey.T,
                },
                new[]
                {
                    VirtualKey.N,
                    VirtualKey.H,
                    VirtualKey.Y,
                },
                new[]
                {
                    VirtualKey.M,
                    VirtualKey.J,
                    VirtualKey.U,
                },
                new[]
                {
                    (VirtualKey)188,
                    VirtualKey.K,
                    VirtualKey.I,
                },
                new[]
                {
                    (VirtualKey)190,
                    VirtualKey.L,
                    VirtualKey.O,
                },
                new[]
                {
                    (VirtualKey)191,
                    (VirtualKey)186,
                    VirtualKey.P,
                }
            };
            var audioVisualizerInputsLength = audioVisualizerInputs.Length;

            var audioMainVisualizerValues = new double[audioVisualizerInputsLength];
            var audioMainVisualizerFrames = new double[audioVisualizerInputsLength];
            var lastAudioVisualizerMainValues = new double[audioVisualizerInputsLength];
            var audioInputVisualizerValues = new double[audioVisualizerInputsLength];
            var audioInputVisualizerFrames = new double[audioVisualizerInputsLength];
            var lastAudioVisualizerInputValues = new double[audioVisualizerInputsLength];
            var audioVisualizerColors = new Color[audioVisualizerInputs.Length, 3, 2];

            var meterValue = 0.0;
            var failedValue = 0.0;

            var handlingColor = GetHandlingColor(1.0);
            var inputStandardColor = GetInputStandardColor(1.0);
            var defaultColor = GetDefaultColor(1.0);
            var inputColor = GetInputColor(0.0);

            var loopingCounter = 0.0;
            var loopingHandler = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    lock (HandlingCSX)
                    {
                        Init();
                        OnBeforeHandle();

                        SetInputColor(VirtualKey.F6, defaultColor);
                        SetInputColor(VirtualKey.F7, voteViewModel.IsOpened ? handlingColor : defaultColor);
                        SetInputColor(VirtualKey.F8, siteContainerViewModel.IsOpened ? handlingColor : defaultColor);
                        SetInputColor(VirtualKey.F10, wwwLevelViewModel.IsOpened ? handlingColor : defaultColor);
                        SetInputColor(VirtualKey.F11, toNotifyViewModel.IsOpened ? handlingColor : defaultColor);
                        if (!QwilightComponent.IsValve)
                        {
                            SetInputColor(VirtualKey.F12, defaultColor);
                        }

                        var isInputStandardWindowOpened = inputStandardViewModel.IsOpened;
                        var isDefaultInputWindowOpened = inputViewModel.IsOpened && inputViewModel.ControllerModeValue == InputViewModel.ControllerMode.DefaultInput;

                        var defaultComputer = mainViewModel.Computer;
                        switch (mainViewModel.ModeValue)
                        {
                            case MainViewModel.Mode.NoteFile:
                                PaintInputAudioVisualizer();

                                PaintEtc();

                                var autoComputer = mainViewModel.AutoComputer;
                                if (autoComputer != null)
                                {
                                    PaintInputStatus(autoComputer);
                                    PaintStatusStatus(autoComputer);
                                }

                                SetInputColor(VirtualKey.Escape, defaultColor);
                                SetInputColor(VirtualKey.Enter, defaultColor);
                                SetInputColor(VirtualKey.Delete, defaultColor);
                                SetInputColor(VirtualKey.Left, defaultColor);
                                SetInputColor(VirtualKey.Right, defaultColor);
                                SetInputColor(VirtualKey.Up, defaultColor);
                                SetInputColor(VirtualKey.Down, defaultColor);
                                SetInputColor(VirtualKey.Space, defaultColor);

                                SetInputColor(VirtualKey.F1, assistViewModel.IsOpened ? handlingColor : defaultColor);
                                SetInputColor(VirtualKey.F5, mainViewModel.IsDefaultEntryLoading ? handlingColor : defaultColor);
                                SetInputColor(VirtualKey.F9, mainViewModel.EntryItemValue.NoteFile.HasFavoriteEntryItem ? handlingColor : defaultColor);
                                break;
                            case MainViewModel.Mode.Computing:
                                PaintEtc();

                                var handlingHitPointsColor = defaultComputer.ModeComponentValue.HandlingHitPointsColor;
                                var hitPoints = defaultComputer.HitPoints.Value;
                                for (var i = mainHitPointsInputsLength - 1; i >= 0; --i)
                                {
                                    var value = Math.Clamp((hitPoints - ((double)i / mainHitPointsInputsLength)) / mainHitPointsInputsUnit, 0, 1);
                                    SetInputColor(mainHitPointsInputs[i], GetValueColor(handlingHitPointsColor, value));
                                }

                                SetStatusColors(hitPoints, handlingHitPointsColor.R, handlingHitPointsColor.G, handlingHitPointsColor.B, handlingHitPointsColor.A);

                                var status = defaultComputer.Status;
                                for (var i = mainStatusInputsLength - 1; i >= 0; --i)
                                {
                                    var value = Math.Clamp((status - (double)i / mainStatusInputsLength) / mainStatusInputsUnit, 0, 1);
                                    SetInputColor(mainStatusInputs[i], GetStatusColor(value * meterValue));
                                }

                                SetInputColor(VirtualKey.Escape, defaultComputer.IsPausingWindowOpened ? handlingColor : defaultColor);
                                SetInputColor(VirtualKey.Enter, defaultComputer.IsPassable || defaultComputer.IsPausingWindowOpened ? defaultColor : handlingColor);

                                if (defaultComputer.IsPausingWindowOpened)
                                {
                                    SetInputColor(VirtualKey.Up, defaultColor);
                                    SetInputColor(VirtualKey.Down, defaultColor);
                                    SetInputColor(VirtualKey.Space, defaultColor);
                                }

                                if (!isInputStandardWindowOpened)
                                {
                                    for (var i = Configure.Instance.DefaultInputBundlesV6.StandardInputs.Length - 1; i >= 0; --i)
                                    {
                                        switch (i)
                                        {
                                            case InputStandardViewModel.ModifyAutoMode:
                                                if (defaultComputer.CanModifyAutoMode)
                                                {
                                                    SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[i].Data, defaultComputer.IsAutoMode ? inputStandardColor : handlingColor);
                                                }
                                                break;
                                            case InputStandardViewModel.MediaMode:
                                                SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[i].Data, Configure.Instance.LoadedMedia && Configure.Instance.Media ? inputStandardColor : handlingColor);
                                                break;
                                            case InputStandardViewModel.PostItem0:
                                            case InputStandardViewModel.PostItem1:
                                                if (defaultComputer.PostableItems[0] != null || defaultComputer.PostableItems[1] != null)
                                                {
                                                    SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[i].Data, inputStandardColor);
                                                }
                                                break;
                                            default:
                                                SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[i].Data, inputStandardColor);
                                                break;
                                        }
                                    }
                                }

                                if (!isDefaultInputWindowOpened)
                                {
                                    var inputs = Configure.Instance.DefaultInputBundlesV6.Inputs[(int)defaultComputer.InputMode];
                                    if (inputs != null)
                                    {
                                        for (var i = inputs.Length - 1; i > 0; --i)
                                        {
                                            foreach (var defaultInput in inputs[i])
                                            {
                                                SetInputColor(defaultInput.Data, failedValue > 0.0 ? GetFailedColor(failedValue) : GetInputColor(InputValues[i]));
                                            }
                                        }
                                    }
                                }

                                for (var i = InputValues.Length - 1; i > 0; --i)
                                {
                                    if (!HasInputValues[i])
                                    {
                                        InputValues[i] = Math.Max(0.0, InputValues[i] - (60 / 1000.0));
                                    }
                                }
                                break;
                            case MainViewModel.Mode.Quit:
                                PaintInputAudioVisualizer();
                                PaintEtc();

                                SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.MediaMode].Data, Configure.Instance.Media ? inputStandardColor : handlingColor);
                                SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HandleUndo].Data, inputStandardColor);

                                SetInputColor(VirtualKey.Escape, defaultColor);
                                SetInputColor(VirtualKey.Enter, defaultColor);
                                if (0 < defaultComputer.LevyingComputingPosition)
                                {
                                    SetInputColor(VirtualKey.Left, defaultColor);
                                }
                                if (defaultComputer.LevyingComputingPosition < defaultComputer.HighestComputingPosition)
                                {
                                    SetInputColor(VirtualKey.Right, defaultColor);
                                }
                                break;
                        }

                        if (isInputStandardWindowOpened)
                        {
                            for (var i = Configure.Instance.DefaultInputBundlesV6.StandardInputs.Length - 1; i >= 0; --i)
                            {
                                SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[i].Data, inputStandardColor);
                            }
                        }
                        else
                        {
                            SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.LowerMultiplier].Data, inputStandardColor);
                            SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HigherMultiplier].Data, inputStandardColor);
                            SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.LowerAudioMultiplier].Data, inputStandardColor);
                            SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HigherAudioMultiplier].Data, inputStandardColor);
                            SetInputColor(Configure.Instance.DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.MediaMode].Data, Configure.Instance.Media ? inputStandardColor : handlingColor);
                        }

                        if (isDefaultInputWindowOpened)
                        {
                            var inputControllerInputs = Configure.Instance.DefaultInputBundlesV6.Inputs[(int)inputViewModel.InputMode];
                            if (inputControllerInputs != null)
                            {
                                foreach (var inputControllerInput in inputControllerInputs)
                                {
                                    if (inputControllerInput != null)
                                    {
                                        foreach (var defaultInput in inputControllerInput)
                                        {
                                            SetInputColor(defaultInput.Data, inputColor);
                                        }
                                    }
                                }
                            }
                        }

                        var modeComponent = mainViewModel.ModeComponentValue;
                        if (valueConfigureViewModel.IsOpened && valueConfigureViewModel.TabPosition == 0 && valueConfigureViewModel.TabPositionComputing == 0 && (mainViewModel.CanModifyModeComponent || modeComponent.CanModifyMultiplier || modeComponent.CanModifyAudioMultiplier))
                        {
                            SetInputColor(VirtualKey.F1, defaultColor);
                            SetInputColor(VirtualKey.F2, defaultColor);
                            SetInputColor(VirtualKey.F3, defaultColor);
                            SetInputColor(VirtualKey.F4, defaultColor);
                            SetInputColor(VirtualKey.F5, defaultColor);
                            SetInputColor(VirtualKey.F6, defaultColor);
                            SetInputColor(VirtualKey.F7, defaultColor);
                            SetInputColor(VirtualKey.F8, defaultColor);
                        }

                        if (IsMeter)
                        {
                            meterValue = 1.0;
                            IsMeter = false;
                        }
                        else
                        {
                            meterValue = Math.Max(0.0, meterValue - 60 / 1000.0);
                        }

                        if (IsFailed)
                        {
                            failedValue = 1.0;
                            IsFailed = false;
                        }
                        else
                        {
                            failedValue = Math.Max(0.0, failedValue - 60 / QwilightComponent.StandardWaitMillis);
                        }

                        OnHandled();
                    }

                    loopingCounter += 1000.0 / 60;
                    var toWait = loopingCounter - loopingHandler.GetMillis();
                    if (toWait > 0.0)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(toWait));
                    }

                    lock (_availableCSX)
                    {
                        if (!IsAvailable)
                        {
                            Monitor.Wait(_availableCSX);
                        }
                    }

                    void PaintEtc()
                    {
                        SetEtcColor(failedValue, meterValue);
                    }

                    void PaintStatusStatus(DefaultCompute defaultComputer)
                    {
                        var statusColor = defaultComputer.IsHandling ? defaultComputer.IsPausing ? BaseUI.Instance.StatusPausedColor : BaseUI.Instance.StatusHandlingColor : BaseUI.Instance.StatusLoadingNoteFileColor;
                        var handlingMeterValue = defaultComputer.IsHandling && !defaultComputer.IsPausing ? meterValue : 1.0;
                        SetStatusColors(defaultComputer.Status, (uint)(statusColor.R * handlingMeterValue), (uint)(statusColor.G * handlingMeterValue), (uint)(statusColor.B * handlingMeterValue), (uint)(statusColor.A * handlingMeterValue));
                    }

                    void PaintInputStatus(DefaultCompute defaultComputer)
                    {
                        var statusColor = defaultComputer.IsHandling ? defaultComputer.IsPausing ? BaseUI.Instance.StatusPausedColor : BaseUI.Instance.StatusHandlingColor : BaseUI.Instance.StatusLoadingNoteFileColor;
                        var handlingMeterValue = defaultComputer.IsHandling && !defaultComputer.IsPausing ? meterValue : 1.0;
                        for (var i = defaultStatusInputsLength - 1; i >= 0; --i)
                        {
                            SetInputColor(defaultStatusInputs[i], GetValueColor(statusColor, handlingMeterValue * Math.Clamp((defaultComputer.Status - (double)i / defaultStatusInputsLength) / defaultStatusInputsUnit, 0, 1)));
                        }
                    }

                    void PaintInputAudioVisualizer()
                    {
                        for (var i = audioVisualizerInputsLength - 1; i >= 0; --i)
                        {
                            var audioMainVisualizerValue = 0.0;
                            var audioInputVisualizerValue = 0.0;
                            for (var j = ((i + 1) * Configure.Instance.AudioVisualizerCount / audioVisualizerInputsLength) - 1; j >= i * Configure.Instance.AudioVisualizerCount / audioVisualizerInputsLength; --j)
                            {
                                audioMainVisualizerValue = Math.Max(audioMainVisualizerValue, AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.MainAudio, j));
                                audioInputVisualizerValue = Math.Max(audioInputVisualizerValue, AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.InputAudio, j));
                            }
                            audioMainVisualizerValues[i] = audioMainVisualizerValue;
                            audioInputVisualizerValues[i] = audioInputVisualizerValue;

                            var distance = lastAudioVisualizerMainValues[i] - audioMainVisualizerValues[i];
                            audioMainVisualizerFrames[i] = Math.Clamp(audioMainVisualizerFrames[i] + (distance == 0.0 ? 0.0 : distance > 0 ? -1 / 3.0 : 1 / 3.0), 0.0, 3.0);
                            distance = lastAudioVisualizerInputValues[i] - audioInputVisualizerValues[i];
                            audioInputVisualizerFrames[i] = Math.Clamp(audioInputVisualizerFrames[i] + (distance == 0.0 ? 0.0 : distance > 0 ? -1 / 3.0 : 1 / 3.0), 0.0, 3.0);
                        }

                        Array.Copy(audioMainVisualizerValues, lastAudioVisualizerMainValues, audioVisualizerInputsLength);
                        Array.Copy(audioInputVisualizerValues, lastAudioVisualizerInputValues, audioVisualizerInputsLength);

                        Array.Clear(audioVisualizerColors, 0, audioVisualizerColors.Length);
                        var audioVisualizerMainColor = Configure.Instance.AudioVisualizerMainColor;
                        var audioVisualizerInputColor = Configure.Instance.AudioVisualizerInputColor;
                        for (var i = audioVisualizerInputsLength - 1; i >= 0; --i)
                        {
                            var value = audioMainVisualizerFrames[i];
                            var valueInt = (int)value;
                            var remainder = value - valueInt;
                            if (remainder > 0.0)
                            {
                                audioVisualizerColors[i, valueInt, 0] = new()
                                {
                                    R = (byte)(audioVisualizerMainColor.R * remainder),
                                    G = (byte)(audioVisualizerMainColor.R * remainder),
                                    B = (byte)(audioVisualizerMainColor.R * remainder),
                                };
                            }
                            for (var j = valueInt - 1; j >= 0; --j)
                            {
                                audioVisualizerColors[i, j, 0] = audioVisualizerMainColor;
                            }

                            value = audioInputVisualizerFrames[i];
                            valueInt = (int)value;
                            remainder = value - valueInt;
                            if (remainder > 0.0)
                            {
                                audioVisualizerColors[i, valueInt, 1] = new()
                                {
                                    R = (byte)(audioVisualizerInputColor.R * remainder),
                                    G = (byte)(audioVisualizerInputColor.R * remainder),
                                    B = (byte)(audioVisualizerInputColor.R * remainder),
                                };
                            }
                            for (var j = valueInt - 1; j >= 0; --j)
                            {
                                audioVisualizerColors[i, j, 1] = audioVisualizerInputColor;
                            }

                            for (var j = audioVisualizerColors.GetLength(0) - 1; j >= 0; --j)
                            {
                                for (var m = audioVisualizerColors.GetLength(1) - 1; m >= 0; --m)
                                {
                                    SetInputColor(audioVisualizerInputs[j][m], GetValueColor(new()
                                    {
                                        A = (byte)((audioVisualizerColors[j, m, 0].A + audioVisualizerColors[j, m, 1].A) / 2),
                                        R = (byte)((audioVisualizerColors[j, m, 0].R + audioVisualizerColors[j, m, 1].R) / 2),
                                        G = (byte)((audioVisualizerColors[j, m, 0].G + audioVisualizerColors[j, m, 1].G) / 2),
                                        B = (byte)((audioVisualizerColors[j, m, 0].B + audioVisualizerColors[j, m, 1].B) / 2)
                                    }, 1.0));
                                }
                            }
                        }
                    }
                }
                catch (ThreadInterruptedException)
                {
                }
                catch (Exception e)
                {
                    Utility.SaveFaultFile(FaultEntryPath, e);
                }
            }
        }

        uint GetDefaultColor(double status) => Utility.GetColor((uint)(0 * status), (uint)(127 * status), (uint)(255 * status), 255);

        uint GetHandlingColor(double status) => Utility.GetColor((uint)(255 * status), (uint)(127 * status), (uint)(0 * status), 255);

        uint GetInputColor(double status) => Utility.GetColor((uint)(0 * status), (uint)(255 * (1.0 - status)), (uint)(255 * status), 255);

        uint GetInputStandardColor(double status) => Utility.GetColor((uint)(255 * status), (uint)(255 * status), (uint)(0 * status), 255);

        uint GetFailedColor(double status) => Utility.GetColor((uint)(255 * status), (uint)(0 * status), (uint)(0 * status), 255);

        uint GetStatusColor(double status) => Utility.GetColor((uint)(0 * status), (uint)(255 * status), (uint)(255 * status), 255);

        uint GetValueColor(Color value, double status) => Utility.GetColor((uint)(value.R * status), (uint)(value.G * status), (uint)(value.B * status), 255);

        public void Dispose()
        {
            foreach (var targetSystem in _targetSystems)
            {
                targetSystem.Dispose();
            }
        }
    }
}
