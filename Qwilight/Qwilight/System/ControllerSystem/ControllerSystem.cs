using Qwilight.Compute;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.IO;
using Vortice.DirectInput;
using Vortice.XInput;
using Windows.System.Power;
using GamepadButtons = Windows.Gaming.Input.GamepadButtons;
using GamepadReading = Windows.Gaming.Input.GamepadReading;
using GamepadVibration = Windows.Gaming.Input.GamepadVibration;
using RawGameController = Windows.Gaming.Input.RawGameController;

namespace Qwilight
{
    public sealed class ControllerSystem : Model
    {
        public enum DInputControllerVariety
        {
            BMS, IIDX
        }

        public enum VibrationMode
        {
            Not, Input, Failed
        }

        public enum InputAPI
        {
            DInput, XInput, WGI
        }

        sealed class DInputIDEquality : IEqualityComparer<DeviceInstance>
        {
            public static readonly IEqualityComparer<DeviceInstance> Default = new DInputIDEquality();

            public bool Equals(DeviceInstance x, DeviceInstance y) => x.InstanceGuid == y.InstanceGuid;

            public int GetHashCode(DeviceInstance obj) => obj.InstanceGuid.GetHashCode();
        }

        sealed class DInputEquality : IEqualityComparer<IDirectInputDevice8>
        {
            public static readonly IEqualityComparer<IDirectInputDevice8> Default = new DInputEquality();

            public bool Equals(IDirectInputDevice8 x, IDirectInputDevice8 y) => x.DeviceInfo.InstanceGuid == y.DeviceInfo.InstanceGuid;

            public int GetHashCode(IDirectInputDevice8 obj) => obj.DeviceInfo.InstanceGuid.GetHashCode();
        }

        public static readonly ControllerSystem Instance = QwilightComponent.GetBuiltInData<ControllerSystem>(nameof(ControllerSystem));

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(ControllerSystem));

        readonly HandlingController<HwDInput> _handlingControllerDInput = new HandlingController<HwDInput>(rawInput => ViewModels.Instance.InputValue.OnDInputLower(rawInput), rawInput => ViewModels.Instance.InputStandardControllerValue.OnDInputLower(rawInput), DefaultCompute.InputFlag.Controller);
        readonly HandlingController<HwXInput> _handlingControllerXInput = new HandlingController<HwXInput>(rawInput => ViewModels.Instance.InputValue.OnXInputLower(rawInput), rawInput => ViewModels.Instance.InputStandardControllerValue.OnXInputLower(rawInput), DefaultCompute.InputFlag.Controller);
        readonly HandlingController<WGI> handlingControllerWGI = new HandlingController<WGI>(rawInput => ViewModels.Instance.InputValue.OnWGILower(rawInput), rawInput => ViewModels.Instance.InputStandardControllerValue.OnWGILower(rawInput), DefaultCompute.InputFlag.Controller);
        double _failedVibration;

        public string[] Controllers { get; set; } = Array.Empty<string>();

        public string MainControllerPower { get; set; }

        public string LastControllerInput { get; set; }

        public void Vibrate() => _failedVibration = 125.0;

        public string ControllerContents => string.Join(", ", Controllers);

        public string ControllerCountContents => string.Format(LanguageSystem.Instance.ControllerCountContents, Controllers.Length);

        public void Init()
        {
            _handlingControllerDInput.Init();
            _handlingControllerXInput.Init();
            handlingControllerWGI.Init();
        }

        public void HandleSystem(nint handle)
        {
            var mainViewModel = ViewModels.Instance.MainValue;

            #region DInput
            IDirectInput8 dInputSystem = null;

            var lastDInputControllerIDs = new List<DeviceInstance>();
            var lastValues = new Dictionary<IDirectInputDevice8, Dictionary<JoystickOffset, int>>();
            var lastXyzHwDInputs = new Dictionary<IDirectInputDevice8, Dictionary<JoystickOffset, HwDInput>>();
            var hwIDs = new Dictionary<IDirectInputDevice8, string>();
            var dInputControllers = new List<IDirectInputDevice8>();
            var dInputXyzHandler = Stopwatch.StartNew();
            var lastDInputXyzMillis = new Dictionary<IDirectInputDevice8, double>();
            var onHandleDInput1000 = new Action(() =>
            {
                var dInputs = dInputSystem.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
                if (Utility.IsItemsEqual(dInputs, lastDInputControllerIDs, DInputIDEquality.Default) == false)
                {
                    lock (dInputControllers)
                    {
                        Utility.SetUICollection(dInputControllers, dInputs.Select(dInputControllerID =>
                        {
                            var dInputController = dInputSystem.CreateDevice(dInputControllerID.InstanceGuid);
                            dInputController.SetCooperativeLevel(handle, CooperativeLevel.NonExclusive | CooperativeLevel.Foreground);
                            dInputController.SetDataFormat<RawJoystickState>();
                            dInputController.Properties.BufferSize = 16;
                            lastValues[dInputController] = new();
                            lastXyzHwDInputs[dInputController] = new();
                            hwIDs[dInputController] = dInputControllerID.InstanceGuid.ToString();
                            return dInputController;
                        }).ToArray(), dInputController => dInputController.Dispose(), null, null, DInputEquality.Default);
                        Controllers = dInputs.Select(dInput => dInput.InstanceName).ToArray();
                        OnPropertyChanged(nameof(Controllers));
                        OnPropertyChanged(nameof(ControllerContents));
                        OnPropertyChanged(nameof(ControllerCountContents));
                        lastDInputControllerIDs.Clear();
                        lastDInputControllerIDs.AddRange(dInputs);
                    }
                }
            });
            var onHandleDInput = new Action(() =>
            {
                lock (dInputControllers)
                {
                    foreach (var dInputController in dInputControllers)
                    {
                        if (Configure.Instance.DInputControllerVarietyValue == DInputControllerVariety.IIDX)
                        {
                            if (dInputXyzHandler.GetMillis() - lastDInputXyzMillis.GetValueOrDefault(dInputController, 0.0) > Configure.Instance.DInputIIDXSensitivity)
                            {
                                foreach (var lastHwDInput in lastXyzHwDInputs[dInputController].Values)
                                {
                                    _handlingControllerDInput.Input(lastHwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                }
                                lastXyzHwDInputs[dInputController].Clear();
                            }
                        }
                        foreach (var data in Utility.GetInputData<JoystickUpdate>(dInputController))
                        {
                            if (ViewModels.Instance.ConfigureValue.IsOpened)
                            {
                                LastControllerInput = $"{nameof(data.Offset)}: {data.Offset}, {nameof(data.RawOffset)}: {data.RawOffset}, {nameof(data.Value)}: {data.Value}";
                                OnPropertyChanged(nameof(LastControllerInput));
                            }
                            var o = data.Offset;
                            var value = data.Value;
                            var hwID = hwIDs[dInputController];
                            switch (o)
                            {
                                case JoystickOffset.X:
                                case JoystickOffset.RotationX:
                                case JoystickOffset.VelocityX:
                                case JoystickOffset.AngularVelocityX:
                                case JoystickOffset.AccelerationX:
                                case JoystickOffset.AngularAccelerationX:
                                case JoystickOffset.ForceX:
                                case JoystickOffset.TorqueX:
                                case JoystickOffset.Y:
                                case JoystickOffset.RotationY:
                                case JoystickOffset.VelocityY:
                                case JoystickOffset.AngularVelocityY:
                                case JoystickOffset.AccelerationY:
                                case JoystickOffset.AngularAccelerationY:
                                case JoystickOffset.ForceY:
                                case JoystickOffset.TorqueY:
                                case JoystickOffset.Z:
                                case JoystickOffset.RotationZ:
                                case JoystickOffset.VelocityZ:
                                case JoystickOffset.AngularVelocityZ:
                                case JoystickOffset.AccelerationZ:
                                case JoystickOffset.AngularAccelerationZ:
                                case JoystickOffset.ForceZ:
                                case JoystickOffset.TorqueZ:
                                    lastValues[dInputController].TryGetValue(o, out var lastValue);
                                    switch (Configure.Instance.DInputControllerVarietyValue)
                                    {
                                        case DInputControllerVariety.BMS:
                                            var targetValue = 16383 * Configure.Instance.DInputXyzSensitivityV2 / 100;
                                            if (0 <= value && value <= targetValue && lastValue > value)
                                            {
                                                var hwDInput = new HwDInput
                                                {
                                                    Data = o,
                                                    Value = 0,
                                                    HwID = hwID
                                                };
                                                if (!lastXyzHwDInputs[dInputController].TryGetValue(o, out var lastHwDInput) || hwDInput != lastHwDInput)
                                                {
                                                    if (lastHwDInput.Data != default)
                                                    {
                                                        _handlingControllerDInput.Input(lastHwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                                    }
                                                    _handlingControllerDInput.Input(hwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                                    lastXyzHwDInputs[dInputController][o] = hwDInput;
                                                }
                                            }
                                            else if (65535 - targetValue <= value && value <= 65535 && lastValue < value)
                                            {
                                                var hwDInput = new HwDInput
                                                {
                                                    Data = o,
                                                    Value = 65535,
                                                    HwID = hwID
                                                };
                                                if (!lastXyzHwDInputs[dInputController].TryGetValue(o, out var lastHwDInput) || hwDInput != lastHwDInput)
                                                {
                                                    if (lastHwDInput.Data != default)
                                                    {
                                                        _handlingControllerDInput.Input(lastHwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                                    }
                                                    _handlingControllerDInput.Input(hwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                                    lastXyzHwDInputs[dInputController][o] = hwDInput;
                                                }
                                            }
                                            else
                                            {
                                                if (lastValue < value)
                                                {
                                                    _handlingControllerDInput.Input(new HwDInput
                                                    {
                                                        Data = o,
                                                        Value = 0,
                                                        HwID = hwID
                                                    }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                                    lastXyzHwDInputs[dInputController].Remove(o);
                                                }
                                                else if (lastValue > value)
                                                {
                                                    _handlingControllerDInput.Input(new HwDInput
                                                    {
                                                        Data = o,
                                                        Value = 65535,
                                                        HwID = hwID
                                                    }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                                    lastXyzHwDInputs[dInputController].Remove(o);
                                                }
                                            }
                                            break;
                                        case DInputControllerVariety.IIDX:
                                            if ((lastValue > value && lastValue != 65535) || (lastValue == 0 && value == 65535))
                                            {
                                                var hwDInput = new HwDInput
                                                {
                                                    Data = o,
                                                    Value = 0,
                                                    HwID = hwID
                                                };
                                                if (!lastXyzHwDInputs[dInputController].TryGetValue(o, out var lastHwDInput) || hwDInput != lastHwDInput)
                                                {
                                                    if (lastHwDInput.Data != default)
                                                    {
                                                        _handlingControllerDInput.Input(lastHwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                                    }
                                                    _handlingControllerDInput.Input(hwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                                    lastXyzHwDInputs[dInputController][o] = hwDInput;
                                                }
                                                lastDInputXyzMillis[dInputController] = dInputXyzHandler.GetMillis();
                                            }
                                            else if ((lastValue < value && lastValue != 0) || (lastValue == 65535 && value == 0))
                                            {
                                                var hwDInput = new HwDInput
                                                {
                                                    Data = o,
                                                    Value = 65535,
                                                    HwID = hwID
                                                };
                                                if (!lastXyzHwDInputs[dInputController].TryGetValue(o, out var lastHwDInput) || hwDInput != lastHwDInput)
                                                {
                                                    if (lastHwDInput.Data != default)
                                                    {
                                                        _handlingControllerDInput.Input(lastHwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                                    }
                                                    _handlingControllerDInput.Input(hwDInput, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                                    lastXyzHwDInputs[dInputController][o] = hwDInput;
                                                }
                                                lastDInputXyzMillis[dInputController] = dInputXyzHandler.GetMillis();
                                            }
                                            break;
                                    }
                                    lastValues[dInputController][o] = value;
                                    break;
                                case JoystickOffset.PointOfViewControllers0:
                                case JoystickOffset.PointOfViewControllers1:
                                case JoystickOffset.PointOfViewControllers2:
                                case JoystickOffset.PointOfViewControllers3:
                                    switch (value)
                                    {
                                        case 0:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 4500:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 9000:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 13500:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 18000:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 22500:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 27000:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case 31500:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, true);
                                            break;
                                        case -1:
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 0,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 9000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 18000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            _handlingControllerDInput.Input(new HwDInput
                                            {
                                                Data = o,
                                                Value = 27000,
                                                HwID = hwID
                                            }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, false);
                                            break;
                                    }
                                    break;
                                default:
                                    _handlingControllerDInput.Input(new HwDInput
                                    {
                                        Data = o,
                                        Value = 128,
                                        HwID = hwID
                                    }, Configure.Instance.DInputBundlesV4.StandardInputs, Configure.Instance.DInputBundlesV4.Inputs, value == 128);
                                    break;
                            }
                        }
                    }
                }
            });
            #endregion

            #region XInput
            var lastXInputs = new List<int>();
            var mainXInputLower0 = new[] { Vortice.XInput.GamepadButtons.DPadDown.ToString(), Vortice.XInput.GamepadButtons.DPadLeft.ToString(), Vortice.XInput.GamepadButtons.DPadRight.ToString(), Vortice.XInput.GamepadButtons.DPadUp.ToString(), Vortice.XInput.GamepadButtons.LeftShoulder.ToString(), Vortice.XInput.GamepadButtons.LeftThumb.ToString(), Vortice.XInput.GamepadButtons.Back.ToString(), "+LT", "-LTX", "-LTY", "+LTX", "+LTY" };
            var mainXInputLower1 = new[] { Vortice.XInput.GamepadButtons.A.ToString(), Vortice.XInput.GamepadButtons.B.ToString(), Vortice.XInput.GamepadButtons.X.ToString(), Vortice.XInput.GamepadButtons.Y.ToString(), Vortice.XInput.GamepadButtons.RightShoulder.ToString(), Vortice.XInput.GamepadButtons.RightThumb.ToString(), Vortice.XInput.GamepadButtons.Start.ToString(), "+RT", "-RTX", "-RTY", "+RTX", "+RTY" };
            var targetXInputControllers = new List<int>();
            var mainXInputLowerValues = new Dictionary<int, HashSet<string>>();
            var lastXInputInputs = new Dictionary<int, State>();
            var lastXInputVibrations = new Dictionary<int, Vibration>();
            var onHandleXInput1000 = new Action(() =>
            {
                var targetXInputs = Enumerable.Range(0, 4).Where(targetXInput => XInput.GetCapabilities(targetXInput, DeviceQueryType.Gamepad, out _)).ToArray();
                if (Utility.IsItemsEqual(targetXInputs, lastXInputs) == false)
                {
                    lock (targetXInputControllers)
                    {
                        Utility.SetUICollection(targetXInputControllers, targetXInputs, null, targetXInput =>
                        {
                            mainXInputLowerValues[targetXInput] = new();
                            lastXInputInputs[targetXInput] = default;
                            lastXInputVibrations[targetXInput] = default;
                        });
                        Controllers = targetXInputs.Select(targetXInput => XInput.GetCapabilities(0, DeviceQueryType.Gamepad, out var targetXInputCapabilities) ? targetXInputCapabilities.SubType.ToString() : string.Empty).ToArray();
                        OnPropertyChanged(nameof(Controllers));
                        OnPropertyChanged(nameof(ControllerContents));
                        OnPropertyChanged(nameof(ControllerCountContents));
                        lastXInputs.Clear();
                        lastXInputs.AddRange(targetXInputs);
                    }
                }
            });
            var onHandleXInput = new Action(() =>
            {
                lock (targetXInputControllers)
                {
                    foreach (var targetXInputController in targetXInputControllers)
                    {
                        var mainXInputLower = mainXInputLowerValues[targetXInputController];
                        var lastInput = lastXInputInputs[targetXInputController];
                        var lastVibration = lastXInputVibrations[targetXInputController];
                        XInput.GetState(targetXInputController, out var data);
                        if (ViewModels.Instance.ConfigureValue.IsOpened)
                        {
                            LastControllerInput = $"{nameof(data.Gamepad.Buttons)}: {data.Gamepad.Buttons}, LT: {data.Gamepad.LeftTrigger}, RT: {data.Gamepad.RightTrigger}, LTX: {data.Gamepad.LeftThumbX}, LTY: {data.Gamepad.LeftThumbY}, RTX: {data.Gamepad.RightThumbX}, RTY: {data.Gamepad.RightThumbY}";
                            OnPropertyChanged(nameof(LastControllerInput));
                        }
                        if (lastInput.PacketNumber < data.PacketNumber)
                        {
                            for (var i = 1; i < 65535; i <<= 1)
                            {
                                var distance = ((ushort)lastInput.Gamepad.Buttons & i) - ((ushort)data.Gamepad.Buttons & i);
                                if (distance == i)
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            Buttons = (Vortice.XInput.GamepadButtons)i
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                    mainXInputLower.Remove(((Vortice.XInput.GamepadButtons)i).ToString());
                                }
                                else if (distance == -i)
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            Buttons = (Vortice.XInput.GamepadButtons)i
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                    mainXInputLower.Add(((Vortice.XInput.GamepadButtons)i).ToString());
                                }
                            }
                            if (data.Gamepad.LeftTrigger == 0 && lastInput.Gamepad.LeftTrigger > data.Gamepad.LeftTrigger && mainXInputLower.Remove("+LT"))
                            {
                                _handlingControllerXInput.Input(new HwXInput
                                {
                                    Data = new Gamepad
                                    {
                                        LeftTrigger = byte.MaxValue
                                    }
                                }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                            }
                            else if (data.Gamepad.LeftTrigger == byte.MaxValue && lastInput.Gamepad.LeftTrigger < data.Gamepad.LeftTrigger && mainXInputLower.Add("+LT"))
                            {
                                _handlingControllerXInput.Input(new HwXInput
                                {
                                    Data = new Gamepad
                                    {
                                        LeftTrigger = byte.MaxValue
                                    }
                                }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                            }
                            if (data.Gamepad.RightTrigger == 0 && lastInput.Gamepad.RightTrigger > data.Gamepad.RightTrigger && mainXInputLower.Remove("+RT"))
                            {
                                _handlingControllerXInput.Input(new HwXInput
                                {
                                    Data = new Gamepad
                                    {
                                        RightTrigger = byte.MaxValue
                                    }
                                }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                            }
                            else if (data.Gamepad.RightTrigger == byte.MaxValue && lastInput.Gamepad.RightTrigger < data.Gamepad.RightTrigger && mainXInputLower.Add("+RT"))
                            {
                                _handlingControllerXInput.Input(new HwXInput
                                {
                                    Data = new Gamepad
                                    {
                                        RightTrigger = byte.MaxValue
                                    }
                                }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                            }
                            if (data.Gamepad.LeftThumbX == short.MinValue)
                            {
                                if (lastInput.Gamepad.LeftThumbX > data.Gamepad.LeftThumbX && mainXInputLower.Add("-LTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbX = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else if (data.Gamepad.LeftThumbX == short.MaxValue)
                            {
                                if (lastInput.Gamepad.LeftThumbX < data.Gamepad.LeftThumbX && mainXInputLower.Add("+LTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbX = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else
                            {
                                if (lastInput.Gamepad.LeftThumbX < data.Gamepad.LeftThumbX && mainXInputLower.Remove("-LTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbX = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                                else if (lastInput.Gamepad.LeftThumbX > data.Gamepad.LeftThumbX && mainXInputLower.Remove("+LTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbX = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                            }
                            if (data.Gamepad.LeftThumbY == short.MinValue)
                            {
                                if (lastInput.Gamepad.LeftThumbY > data.Gamepad.LeftThumbY && mainXInputLower.Add("-LTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbY = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else if (data.Gamepad.LeftThumbY == short.MaxValue)
                            {
                                if (lastInput.Gamepad.LeftThumbY < data.Gamepad.LeftThumbY && mainXInputLower.Add("+LTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbY = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else
                            {
                                if (lastInput.Gamepad.LeftThumbY < data.Gamepad.LeftThumbY && mainXInputLower.Remove("-LTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbY = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                                else if (lastInput.Gamepad.LeftThumbY > data.Gamepad.LeftThumbY && mainXInputLower.Remove("+LTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            LeftThumbY = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                            }
                            if (data.Gamepad.RightThumbX == short.MinValue)
                            {
                                if (lastInput.Gamepad.RightThumbX > data.Gamepad.RightThumbX && mainXInputLower.Add("-RTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbX = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else if (data.Gamepad.RightThumbX == short.MaxValue)
                            {
                                if (lastInput.Gamepad.RightThumbX < data.Gamepad.RightThumbX && mainXInputLower.Add("+RTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbX = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else
                            {
                                if (lastInput.Gamepad.RightThumbX < data.Gamepad.RightThumbX && mainXInputLower.Remove("-RTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbX = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                                else if (lastInput.Gamepad.RightThumbX > data.Gamepad.RightThumbX && mainXInputLower.Remove("+RTX"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbX = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                            }
                            if (data.Gamepad.RightThumbY == short.MinValue)
                            {
                                if (lastInput.Gamepad.RightThumbY > data.Gamepad.RightThumbY && mainXInputLower.Add("-RTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbY = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else if (data.Gamepad.RightThumbY == short.MaxValue)
                            {
                                if (lastInput.Gamepad.RightThumbY < data.Gamepad.RightThumbY && mainXInputLower.Add("+RTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbY = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, true);
                                }
                            }
                            else
                            {
                                if (lastInput.Gamepad.RightThumbY < data.Gamepad.RightThumbY && mainXInputLower.Remove("-RTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbY = short.MinValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                                else if (lastInput.Gamepad.RightThumbY > data.Gamepad.RightThumbY && mainXInputLower.Remove("+RTY"))
                                {
                                    _handlingControllerXInput.Input(new HwXInput
                                    {
                                        Data = new Gamepad
                                        {
                                            RightThumbY = short.MaxValue
                                        }
                                    }, Configure.Instance.XInputBundlesV4.StandardInputs, Configure.Instance.XInputBundlesV4.Inputs, false);
                                }
                            }
                            lastXInputInputs[targetXInputController] = data;
                        }
                        Vibration vibration = default;
                        switch (Configure.Instance.VibrationModeValue)
                        {
                            case VibrationMode.Input:
                                foreach (var mainXInput in mainXInputLower)
                                {
                                    vibration = new Vibration(mainXInputLower0.Contains(mainXInput) ? (ushort)(ushort.MaxValue * Configure.Instance.Vibration0) : (ushort)0, mainXInputLower1.Contains(mainXInput) ? (ushort)(ushort.MaxValue * Configure.Instance.Vibration1) : (ushort)0);
                                }
                                break;
                            case VibrationMode.Failed:
                                if (_failedVibration > 0)
                                {
                                    vibration = new Vibration((ushort)(ushort.MaxValue * Configure.Instance.Vibration0), (ushort)(ushort.MaxValue * Configure.Instance.Vibration1));
                                }
                                break;
                        }
                        if (vibration.LeftMotorSpeed != lastVibration.LeftMotorSpeed || vibration.RightMotorSpeed != lastVibration.RightMotorSpeed)
                        {
                            XInput.SetVibration(targetXInputController, vibration);
                            lastXInputVibrations[targetXInputController] = vibration;
                        }
                        if (MainControllerPower == null)
                        {
                            var mainControllerPower = XInput.GetBatteryInformation(targetXInputController, BatteryDeviceType.Gamepad);
                            MainControllerPower = string.Format(mainControllerPower.BatteryType switch
                            {
                                BatteryType.Alkaline => LanguageSystem.Instance.Power00,
                                BatteryType.Disconnected => LanguageSystem.Instance.Power01,
                                BatteryType.Nimh => LanguageSystem.Instance.Power02,
                                BatteryType.Unknown => LanguageSystem.Instance.Power03,
                                BatteryType.Wired => LanguageSystem.Instance.Power04,
                                _ => default
                            }, mainControllerPower.BatteryLevel switch
                            {
                                BatteryLevel.Low => (200.0 / 3),
                                BatteryLevel.Medium => (100.0 / 3),
                                BatteryLevel.Full => 100.0,
                                BatteryLevel.Empty => 0.0,
                                _ => default
                            });
                        }
                    }
                    if (_failedVibration > 0.0)
                    {
                        _failedVibration -= 1000.0 / Configure.Instance.LoopUnit;
                    }
                }
            });
            #endregion

            #region WGI
            var lastWGIs = new List<(string, Windows.Gaming.Input.Gamepad)>();
            var mainWGILower0 = new[] { GamepadButtons.DPadDown.ToString(), GamepadButtons.DPadLeft.ToString(), GamepadButtons.DPadRight.ToString(), GamepadButtons.DPadUp.ToString(), GamepadButtons.LeftShoulder.ToString(), GamepadButtons.LeftThumbstick.ToString(), GamepadButtons.Menu.ToString(), "-LTX", "-LTY", "+LTX", "+LTY" };
            var mainWGILower1 = new[] { GamepadButtons.A.ToString(), GamepadButtons.B.ToString(), GamepadButtons.X.ToString(), GamepadButtons.Y.ToString(), GamepadButtons.RightShoulder.ToString(), GamepadButtons.RightThumbstick.ToString(), GamepadButtons.View.ToString(), "-RTX", "-RTY", "+RTX", "+RTY" };
            const string mainWGILower2 = "+LT";
            const string mainWGILower3 = "+RT";
            var mainWGILowerValues = new Dictionary<Windows.Gaming.Input.Gamepad, HashSet<string>>();
            var lastWGIInputs = new Dictionary<Windows.Gaming.Input.Gamepad, GamepadReading>();
            var lastWGIVibrations = new Dictionary<Windows.Gaming.Input.Gamepad, GamepadVibration>();
            var targetWGIControllers = new List<Windows.Gaming.Input.Gamepad>();
            var onHandleWGI1000 = new Action(() =>
            {
                var targetWGIs = RawGameController.RawGameControllers.Select(targetWGI => (targetWGI.DisplayName, Windows.Gaming.Input.Gamepad.FromGameController(targetWGI))).Where(targetWGI => targetWGI.Item2 != null).ToList();
                if (Utility.IsItemsEqual(targetWGIs, lastWGIs) == false)
                {
                    lock (targetWGIControllers)
                    {
                        Utility.SetUICollection(targetWGIControllers, targetWGIs.Select(targetWGI => targetWGI.Item2).ToArray(), null, targetWGI =>
                        {
                            mainWGILowerValues[targetWGI] = new();
                            lastWGIInputs[targetWGI] = default;
                            lastWGIVibrations[targetWGI] = default;
                        });
                        Controllers = targetWGIs.Select(targetWGI => targetWGI.DisplayName).ToArray();
                        OnPropertyChanged(nameof(Controllers));
                        OnPropertyChanged(nameof(ControllerContents));
                        OnPropertyChanged(nameof(ControllerCountContents));
                        lastWGIs.Clear();
                        lastWGIs.AddRange(targetWGIs);
                    }
                }
            });
            var onHandleWGI = new Action(() =>
            {
                lock (targetWGIControllers)
                {
                    foreach (var targetWGIController in targetWGIControllers)
                    {
                        var mainWGILower = mainWGILowerValues[targetWGIController];
                        var lastInput = lastWGIInputs[targetWGIController];
                        var lastVibration = lastWGIVibrations[targetWGIController];
                        var data = targetWGIController.GetCurrentReading();
                        if (ViewModels.Instance.ConfigureValue.IsOpened)
                        {
                            LastControllerInput = $"{nameof(data.Buttons)}: {data.Buttons}, LT: {Math.Round(data.LeftTrigger, 3)}, RT: {Math.Round(data.RightTrigger, 3)}, LTX: {Math.Round(data.LeftThumbstickX, 3)}, LTY: {Math.Round(data.LeftThumbstickY, 3)}, RTX: {Math.Round(data.RightThumbstickX, 3)}, RTY: {Math.Round(data.RightThumbstickY, 3)}";
                            OnPropertyChanged(nameof(LastControllerInput));
                        }
                        for (var i = 1; i < 262143; i <<= 1)
                        {
                            var d = ((ushort)lastInput.Buttons & i) - ((ushort)data.Buttons & i);
                            if (d == i)
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        Buttons = (GamepadButtons)i
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                                mainWGILower.Remove(((GamepadButtons)i).ToString());
                            }
                            else if (d == -i)
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        Buttons = (GamepadButtons)i
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                                mainWGILower.Add(((GamepadButtons)i).ToString());
                            }
                        }
                        if (data.LeftTrigger == 0.0 && lastInput.LeftTrigger > data.LeftTrigger && mainWGILower.Remove("+LT"))
                        {
                            handlingControllerWGI.Input(new WGI
                            {
                                Data = new GamepadReading
                                {
                                    LeftTrigger = 1.0
                                }
                            }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                        }
                        else if (data.LeftTrigger == 1.0 && lastInput.LeftTrigger < data.LeftTrigger && mainWGILower.Add("+LT"))
                        {
                            handlingControllerWGI.Input(new WGI
                            {
                                Data = new GamepadReading
                                {
                                    LeftTrigger = 1.0
                                }
                            }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                        }
                        if (data.RightTrigger == 0.0 && lastInput.RightTrigger > data.RightTrigger && mainWGILower.Remove("+RT"))
                        {
                            handlingControllerWGI.Input(new WGI
                            {
                                Data = new GamepadReading
                                {
                                    RightTrigger = 1.0
                                }
                            }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                        }
                        else if (data.RightTrigger == 1.0 && lastInput.RightTrigger < data.RightTrigger && mainWGILower.Add("+RT"))
                        {
                            handlingControllerWGI.Input(new WGI
                            {
                                Data = new GamepadReading
                                {
                                    RightTrigger = 1.0
                                }
                            }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                        }
                        if (data.LeftThumbstickX == -1.0)
                        {
                            if (lastInput.LeftThumbstickX > data.LeftThumbstickX && mainWGILower.Add("-LTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickX = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else if (data.LeftThumbstickX == 1.0)
                        {
                            if (lastInput.LeftThumbstickX < data.LeftThumbstickX && mainWGILower.Add("+LTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickX = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else
                        {
                            if (lastInput.LeftThumbstickX < data.LeftThumbstickX && mainWGILower.Remove("-LTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickX = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                            else if (lastInput.LeftThumbstickX > data.LeftThumbstickX && mainWGILower.Remove("+LTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickX = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                        }
                        if (data.LeftThumbstickY == -1.0)
                        {
                            if (lastInput.LeftThumbstickY > data.LeftThumbstickY && mainWGILower.Add("-LTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickY = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else if (data.LeftThumbstickY == 1.0)
                        {
                            if (lastInput.LeftThumbstickY < data.LeftThumbstickY && mainWGILower.Add("+LTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickY = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else
                        {
                            if (lastInput.LeftThumbstickY < data.LeftThumbstickY && mainWGILower.Remove("-LTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickY = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                            else if (lastInput.LeftThumbstickY > data.LeftThumbstickY && mainWGILower.Remove("+LTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        LeftThumbstickY = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                        }
                        if (data.RightThumbstickX == -1.0)
                        {
                            if (lastInput.RightThumbstickX > data.RightThumbstickX && mainWGILower.Add("-RTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickX = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else if (data.RightThumbstickX == 1.0)
                        {
                            if (lastInput.RightThumbstickX < data.RightThumbstickX && mainWGILower.Add("+RTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickX = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else
                        {
                            if (lastInput.RightThumbstickX < data.RightThumbstickX && mainWGILower.Remove("-RTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickX = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                            else if (lastInput.RightThumbstickX > data.RightThumbstickX && mainWGILower.Remove("+RTX"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickX = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                        }
                        if (data.RightThumbstickY == -1.0)
                        {
                            if (lastInput.RightThumbstickY > data.RightThumbstickY && mainWGILower.Add("-RTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickY = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else if (data.RightThumbstickY == 1.0)
                        {
                            if (lastInput.RightThumbstickY < data.RightThumbstickY && mainWGILower.Add("+RTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickY = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, true);
                            }
                        }
                        else
                        {
                            if (lastInput.RightThumbstickY < data.RightThumbstickY && mainWGILower.Remove("-RTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickY = -1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                            else if (lastInput.RightThumbstickY > data.RightThumbstickY && mainWGILower.Remove("+RTY"))
                            {
                                handlingControllerWGI.Input(new WGI
                                {
                                    Data = new GamepadReading
                                    {
                                        RightThumbstickY = 1.0
                                    }
                                }, Configure.Instance.WGIBundlesV3.StandardInputs, Configure.Instance.WGIBundlesV3.Inputs, false);
                            }
                        }
                        lastWGIInputs[targetWGIController] = data;
                        GamepadVibration vibration = default;
                        switch (Configure.Instance.VibrationModeValue)
                        {
                            case VibrationMode.Input:
                                foreach (var mainWGI in mainWGILower)
                                {
                                    if (mainWGILower0.Contains(mainWGI))
                                    {
                                        vibration.LeftMotor = Configure.Instance.Vibration0;
                                    }
                                    if (mainWGILower1.Contains(mainWGI))
                                    {
                                        vibration.RightMotor = Configure.Instance.Vibration1;
                                    }
                                    switch (mainWGI)
                                    {
                                        case mainWGILower2:
                                            vibration.LeftTrigger = Configure.Instance.Vibration2;
                                            break;
                                        case mainWGILower3:
                                            vibration.RightTrigger = Configure.Instance.Vibration3;
                                            break;
                                    }
                                }
                                break;
                            case VibrationMode.Failed:
                                if (_failedVibration > 0)
                                {
                                    vibration.LeftMotor = Configure.Instance.Vibration0;
                                    vibration.RightMotor = Configure.Instance.Vibration1;
                                    vibration.LeftTrigger = Configure.Instance.Vibration2;
                                    vibration.RightTrigger = Configure.Instance.Vibration3;
                                }
                                break;
                        }
                        if (vibration.LeftMotor != lastVibration.LeftMotor || vibration.RightMotor != lastVibration.RightMotor || vibration.LeftTrigger != lastVibration.LeftTrigger || vibration.RightTrigger != lastVibration.RightTrigger)
                        {
                            targetWGIController.Vibration = vibration;
                            lastWGIVibrations[targetWGIController] = vibration;
                        }
                        if (MainControllerPower == null)
                        {
                            var mainControllerPower = targetWGIController.TryGetBatteryReport();
                            if (mainControllerPower != null)
                            {
                                MainControllerPower = string.Format(mainControllerPower.Status switch
                                {
                                    BatteryStatus.Charging => LanguageSystem.Instance.Power10,
                                    BatteryStatus.Discharging => LanguageSystem.Instance.Power11,
                                    BatteryStatus.Idle => LanguageSystem.Instance.Power12,
                                    BatteryStatus.NotPresent => LanguageSystem.Instance.Power13,
                                    _ => default
                                }, 100.0 * mainControllerPower.RemainingCapacityInMilliwattHours / mainControllerPower.FullChargeCapacityInMilliwattHours);
                            }
                        }
                    }
                    if (_failedVibration > 0.0)
                    {
                        _failedVibration -= 1000.0 / Configure.Instance.LoopUnit;
                    }
                }
            });
            #endregion

            while (true)
            {
                try
                {
                    switch (Configure.Instance.ControllerInputAPI)
                    {
                        case InputAPI.DInput:
                            try
                            {
                                using (dInputSystem = DInput.DirectInput8Create())
                                {
                                    while (Configure.Instance.ControllerInputAPI == InputAPI.DInput)
                                    {
                                        _handlingControllerDInput.HandleLooping(dInputControllers.Count > 0, onHandleDInput1000, onHandleDInput);
                                    }
                                }
                            }
                            finally
                            {
                                lock (dInputControllers)
                                {
                                    lastDInputControllerIDs.Clear();
                                    lastValues.Clear();
                                    lastXyzHwDInputs.Clear();
                                    hwIDs.Clear();
                                    dInputControllers.Clear();
                                }
                            }
                            break;
                        case InputAPI.XInput:
                            try
                            {
                                while (Configure.Instance.ControllerInputAPI == InputAPI.XInput)
                                {
                                    _handlingControllerXInput.HandleLooping(targetXInputControllers.Count > 0, onHandleXInput1000, onHandleXInput);
                                }
                            }
                            finally
                            {
                                lock (targetXInputControllers)
                                {
                                    foreach (var targetXInputController in targetXInputControllers)
                                    {
                                        XInput.SetVibration(targetXInputController, new Vibration());
                                    }
                                    lastXInputs.Clear();
                                    targetXInputControllers.Clear();
                                    mainXInputLowerValues.Clear();
                                    lastXInputInputs.Clear();
                                    lastXInputVibrations.Clear();
                                }
                            }
                            break;
                        case InputAPI.WGI:
                            try
                            {
                                while (Configure.Instance.ControllerInputAPI == InputAPI.WGI)
                                {
                                    handlingControllerWGI.HandleLooping(targetWGIControllers.Count > 0, onHandleWGI1000, onHandleWGI);
                                }
                            }
                            finally
                            {
                                lock (targetWGIControllers)
                                {
                                    foreach (var targetWGIController in targetWGIControllers)
                                    {
                                        targetWGIController.Vibration = new GamepadVibration();
                                    }
                                    lastWGIs.Clear();
                                    mainWGILowerValues.Clear();
                                    lastWGIInputs.Clear();
                                    lastWGIVibrations.Clear();
                                    targetWGIControllers.Clear();
                                }
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Utility.SetFault(FaultEntryPath, e);
                }
                finally
                {
                    Controllers = Array.Empty<string>();
                    OnPropertyChanged(nameof(Controllers));
                    OnPropertyChanged(nameof(ControllerContents));
                    OnPropertyChanged(nameof(ControllerCountContents));

                    Init();
                }
            }
        }
    }
}