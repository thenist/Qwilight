using AuraServiceLib;
using Microsoft.UI;
using Qwilight.Utilities;
using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public sealed class AuraSystem : BaseRGBSystem
    {
        public static readonly AuraSystem Instance = new();

        static VirtualKey GetInput(ushort rawInput) => rawInput switch
        {
            1 => VirtualKey.Escape,
            2 => VirtualKey.Number1,
            3 => VirtualKey.Number2,
            4 => VirtualKey.Number3,
            5 => VirtualKey.Number4,
            6 => VirtualKey.Number5,
            7 => VirtualKey.Number6,
            8 => VirtualKey.Number7,
            9 => VirtualKey.Number8,
            10 => VirtualKey.Number9,
            11 => VirtualKey.Number0,
            12 => (VirtualKey)189,
            13 => (VirtualKey)187,
            14 => VirtualKey.Back,
            15 => VirtualKey.Tab,
            16 => VirtualKey.Q,
            17 => VirtualKey.W,
            18 => VirtualKey.E,
            19 => VirtualKey.R,
            20 => VirtualKey.T,
            21 => VirtualKey.Y,
            22 => VirtualKey.U,
            23 => VirtualKey.I,
            24 => VirtualKey.O,
            25 => VirtualKey.P,
            26 => (VirtualKey)219,
            27 => (VirtualKey)221,
            28 => VirtualKey.Enter,
            29 => VirtualKey.LeftControl,
            30 => VirtualKey.A,
            31 => VirtualKey.S,
            32 => VirtualKey.D,
            33 => VirtualKey.F,
            34 => VirtualKey.G,
            35 => VirtualKey.H,
            36 => VirtualKey.J,
            37 => VirtualKey.K,
            38 => VirtualKey.L,
            39 => (VirtualKey)186,
            40 => (VirtualKey)222,
            41 => (VirtualKey)192,
            42 => VirtualKey.LeftShift,
            43 => (VirtualKey)220,
            44 => VirtualKey.Z,
            45 => VirtualKey.X,
            46 => VirtualKey.C,
            47 => VirtualKey.V,
            48 => VirtualKey.B,
            49 => VirtualKey.N,
            50 => VirtualKey.M,
            51 => (VirtualKey)188,
            52 => (VirtualKey)190,
            53 => (VirtualKey)191,
            54 => VirtualKey.RightShift,
            55 => VirtualKey.Multiply,

            57 => VirtualKey.Space,
            58 => VirtualKey.CapitalLock,
            59 => VirtualKey.F1,
            60 => VirtualKey.F2,
            61 => VirtualKey.F3,
            62 => VirtualKey.F4,
            63 => VirtualKey.F5,
            64 => VirtualKey.F6,
            65 => VirtualKey.F7,
            66 => VirtualKey.F8,
            67 => VirtualKey.F9,
            68 => VirtualKey.F10,
            69 => VirtualKey.NumberKeyLock,
            70 => VirtualKey.Scroll,
            71 => VirtualKey.NumberPad7,
            72 => VirtualKey.NumberPad8,
            73 => VirtualKey.NumberPad9,

            75 => VirtualKey.NumberPad4,
            76 => VirtualKey.NumberPad5,
            77 => VirtualKey.NumberPad6,

            79 => VirtualKey.NumberPad1,
            80 => VirtualKey.NumberPad2,
            81 => VirtualKey.NumberPad3,
            82 => VirtualKey.NumberPad0,

            87 => VirtualKey.F11,
            88 => VirtualKey.F12,

            157 => VirtualKey.RightControl,

            197 => VirtualKey.Pause,
            199 => VirtualKey.Home,
            200 => VirtualKey.Up,
            201 => VirtualKey.PageUp,
            203 => VirtualKey.Left,
            205 => VirtualKey.Right,
            207 => VirtualKey.End,
            208 => VirtualKey.Down,
            209 => VirtualKey.PageDown,
            210 => VirtualKey.Insert,
            211 => VirtualKey.Delete,
            219 => VirtualKey.LeftWindows,
            221 => VirtualKey.Application,

            _ => VirtualKey.None
        };

        readonly Dictionary<int, List<IAuraRgbLight>> _statusItems = new();
        readonly Dictionary<VirtualKey, List<IAuraRgbLight>> _inputItems = new();
        readonly List<IAuraRgbLight> _etcItems = new();
        readonly List<IAuraRgbLight> _rgbItems = new();
        int _highestStatusCount;
        IAuraSdk2 _auraSDK;
        IAuraSyncDeviceCollection _auraItems;

        public override bool IsAvailable => Configure.Instance.Aura;

        public override bool Init()
        {
            try
            {
                _auraSDK = new AuraSdk() as IAuraSdk2;
                _auraSDK.SwitchMode();
                _auraItems = _auraSDK.Enumerate(0);
                if (_auraItems.Count > 0)
                {
                    _statusItems.Clear();
                    _inputItems.Clear();
                    _etcItems.Clear();
                    _rgbItems.Clear();
                    foreach (IAuraSyncDevice auraItem in _auraItems)
                    {
                        foreach (IAuraRgbLight ledItem in auraItem.Lights)
                        {
                            _rgbItems.Add(ledItem);
                        }
                        if (auraItem is IAuraSyncKeyboard auraInput)
                        {
                            foreach (IAuraRgbKey ledItem in auraInput.Keys)
                            {
                                var input = GetInput(ledItem.Code);
                                if (input != VirtualKey.None)
                                {
                                    Utility.NewValue(_inputItems, input, auraInput.Key[ledItem.Code]);
                                }
                            }
                        }
                        else
                        {
                            if (auraItem.Name.IsFrontCaselsss("ENE_RGB_For_ASUS"))
                            {
                                for (var i = auraItem.Lights.Count - 1; i >= 0; --i)
                                {
                                    Utility.NewValue(_statusItems, i, auraItem.Lights[i]);
                                }
                            }
                            else
                            {
                                foreach (IAuraRgbLight ledItem in auraItem.Lights)
                                {
                                    _etcItems.Add(ledItem);
                                }
                            }
                        }
                    }
                    _highestStatusCount = _statusItems.Keys.Count > 0 ? _statusItems.Keys.Max() : 0;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void SetInputColor(VirtualKey input, uint value)
        {
            if (_inputItems.TryGetValue(input, out var inputItems))
            {
                foreach (var inputItem in inputItems)
                {
                    inputItem.Color = value;
                }
            }
        }

        public override void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3)
        {
            if (!double.IsNaN(status))
            {
                var value = _highestStatusCount * status;
                var valueInt = (int)value;
                var remainder = value - valueInt;
                if (remainder > 0.0 && _statusItems.TryGetValue(valueInt, out var faintStatusItems))
                {
                    foreach (var statusItem in faintStatusItems)
                    {
                        statusItem.Color = Utility.GetColor((uint)(value0 * remainder), (uint)(value1 * remainder), (uint)(value2 * remainder), (uint)(value3 * remainder));
                    }
                }
                for (var i = valueInt - 1; i >= 0; --i)
                {
                    if (_statusItems.TryGetValue(valueInt, out var statusItems))
                    {
                        foreach (var statusItem in statusItems)
                        {
                            statusItem.Color = Utility.GetColor(value0, value1, value2, value3);
                        }
                    }
                }
            }
        }

        public override void SetEtcColor(uint value)
        {
            foreach (var etcItem in _etcItems)
            {
                etcItem.Color = value;
            }
        }

        public override void OnBeforeHandle()
        {
            foreach (IAuraRgbLight ledItem in _rgbItems)
            {
                ledItem.Color = 0;
            }
        }

        public override void OnHandled()
        {
            foreach (IAuraSyncDevice auraItem in _auraItems)
            {
                auraItem.Apply();
            }
        }

        public override Color GetMeterColor() => Colors.Red;

        public override void Dispose()
        {
            lock (RGBSystem.Instance.HandlingCSX)
            {
                if (IsHandling)
                {
                    IsHandling = false;
                    _auraSDK.ReleaseControl(0);
                    _auraSDK = null;
                }
            }
        }

        public override void Toggle()
        {
            Configure.Instance.Aura = !Configure.Instance.Aura;
            base.Toggle();
        }
    }
}
