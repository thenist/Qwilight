using AuraServiceLib;
using Microsoft.UI;
using Qwilight.Utilities;
using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public sealed class AuraSystem : BaseIlluminationSystem
    {
        public static readonly AuraSystem Instance = new();

        static readonly Dictionary<ushort, VirtualKey> _inputMap = new();

        static AuraSystem()
        {
            _inputMap[1] = VirtualKey.Escape;
            _inputMap[2] = VirtualKey.Number1;
            _inputMap[3] = VirtualKey.Number2;
            _inputMap[4] = VirtualKey.Number3;
            _inputMap[5] = VirtualKey.Number4;
            _inputMap[6] = VirtualKey.Number5;
            _inputMap[7] = VirtualKey.Number6;
            _inputMap[8] = VirtualKey.Number7;
            _inputMap[9] = VirtualKey.Number8;
            _inputMap[10] = VirtualKey.Number9;
            _inputMap[11] = VirtualKey.Number0;
            _inputMap[12] = (VirtualKey)189;
            _inputMap[13] = (VirtualKey)187;
            _inputMap[14] = VirtualKey.Back;
            _inputMap[15] = VirtualKey.Tab;
            _inputMap[16] = VirtualKey.Q;
            _inputMap[17] = VirtualKey.W;
            _inputMap[18] = VirtualKey.E;
            _inputMap[19] = VirtualKey.R;
            _inputMap[20] = VirtualKey.T;
            _inputMap[21] = VirtualKey.Y;
            _inputMap[22] = VirtualKey.U;
            _inputMap[23] = VirtualKey.I;
            _inputMap[24] = VirtualKey.O;
            _inputMap[25] = VirtualKey.P;
            _inputMap[26] = (VirtualKey)219;
            _inputMap[27] = (VirtualKey)221;
            _inputMap[28] = VirtualKey.Enter;
            _inputMap[29] = VirtualKey.LeftControl;
            _inputMap[30] = VirtualKey.A;
            _inputMap[31] = VirtualKey.S;
            _inputMap[32] = VirtualKey.D;
            _inputMap[33] = VirtualKey.F;
            _inputMap[34] = VirtualKey.G;
            _inputMap[35] = VirtualKey.H;
            _inputMap[36] = VirtualKey.J;
            _inputMap[37] = VirtualKey.K;
            _inputMap[38] = VirtualKey.L;
            _inputMap[39] = (VirtualKey)186;
            _inputMap[40] = (VirtualKey)222;
            _inputMap[41] = (VirtualKey)192;
            _inputMap[42] = VirtualKey.LeftShift;
            _inputMap[43] = (VirtualKey)220;
            _inputMap[44] = VirtualKey.Z;
            _inputMap[45] = VirtualKey.X;
            _inputMap[46] = VirtualKey.C;
            _inputMap[47] = VirtualKey.V;
            _inputMap[48] = VirtualKey.B;
            _inputMap[49] = VirtualKey.N;
            _inputMap[50] = VirtualKey.M;
            _inputMap[51] = (VirtualKey)188;
            _inputMap[52] = (VirtualKey)190;
            _inputMap[53] = (VirtualKey)191;
            _inputMap[54] = VirtualKey.RightShift;
            _inputMap[55] = VirtualKey.Multiply;

            _inputMap[57] = VirtualKey.Space;
            _inputMap[58] = VirtualKey.CapitalLock;
            _inputMap[59] = VirtualKey.F1;
            _inputMap[60] = VirtualKey.F2;
            _inputMap[61] = VirtualKey.F3;
            _inputMap[62] = VirtualKey.F4;
            _inputMap[63] = VirtualKey.F5;
            _inputMap[64] = VirtualKey.F6;
            _inputMap[65] = VirtualKey.F7;
            _inputMap[66] = VirtualKey.F8;
            _inputMap[67] = VirtualKey.F9;
            _inputMap[68] = VirtualKey.F10;
            _inputMap[69] = VirtualKey.NumberKeyLock;
            _inputMap[70] = VirtualKey.Scroll;
            _inputMap[71] = VirtualKey.NumberPad7;
            _inputMap[72] = VirtualKey.NumberPad8;
            _inputMap[73] = VirtualKey.NumberPad9;

            _inputMap[75] = VirtualKey.NumberPad4;
            _inputMap[76] = VirtualKey.NumberPad5;
            _inputMap[77] = VirtualKey.NumberPad6;

            _inputMap[79] = VirtualKey.NumberPad1;
            _inputMap[80] = VirtualKey.NumberPad2;
            _inputMap[81] = VirtualKey.NumberPad3;
            _inputMap[82] = VirtualKey.NumberPad0;

            _inputMap[87] = VirtualKey.F11;
            _inputMap[88] = VirtualKey.F12;

            _inputMap[157] = VirtualKey.RightControl;

            _inputMap[197] = VirtualKey.Pause;
            _inputMap[199] = VirtualKey.Home;
            _inputMap[200] = VirtualKey.Up;
            _inputMap[201] = VirtualKey.PageUp;
            _inputMap[203] = VirtualKey.Left;
            _inputMap[205] = VirtualKey.Right;
            _inputMap[207] = VirtualKey.End;
            _inputMap[208] = VirtualKey.Down;
            _inputMap[209] = VirtualKey.PageDown;
            _inputMap[210] = VirtualKey.Insert;
            _inputMap[211] = VirtualKey.Delete;
            _inputMap[219] = VirtualKey.LeftWindows;
            _inputMap[221] = VirtualKey.Application;
        }

        readonly Dictionary<int, List<IAuraRgbLight>> _statusItems = new();
        readonly Dictionary<VirtualKey, List<IAuraRgbLight>> _inputItems = new();
        readonly List<IAuraRgbLight> _etcItems = new();
        readonly List<IAuraRgbLight> _illuminatedItems = new();
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
                    _illuminatedItems.Clear();
                    foreach (IAuraSyncDevice auraItem in _auraItems)
                    {
                        foreach (IAuraRgbLight ledItem in auraItem.Lights)
                        {
                            _illuminatedItems.Add(ledItem);
                        }
                        if (auraItem is IAuraSyncKeyboard auraInput)
                        {
                            foreach (IAuraRgbKey ledItem in auraInput.Keys)
                            {
                                if (_inputMap.TryGetValue(ledItem.Code, out var input))
                                {
                                    Utility.Into(_inputItems, input, auraInput.Key[ledItem.Code]);
                                }
                            }
                        }
                        else
                        {
                            if (auraItem.Name.IsFrontCaselsss("ENE_RGB_For_ASUS"))
                            {
                                for (var i = auraItem.Lights.Count - 1; i >= 0; --i)
                                {
                                    Utility.Into(_statusItems, i, auraItem.Lights[i]);
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
            foreach (IAuraRgbLight ledItem in _illuminatedItems)
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
            lock (IlluminationSystem.Instance.HandlingCSX)
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
