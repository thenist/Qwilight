using Qwilight.Utilities;
using System.Diagnostics;
using System.IO;
using Vortice.DirectInput;
using Windows.System;

namespace Qwilight
{
    public sealed class DInputSystem : IDisposable
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(DInputSystem));

        readonly IHandleInput _handleInput;
        bool _isAvailable = true;

        public DInputSystem(IHandleInput handleInput) => _handleInput = handleInput;

        public void HandleSystem(nint handle) => Utility.HandleLongParallel(() =>
        {
            while (_isAvailable)
            {
                try
                {
                    var loopingCounter = 0.0;
                    var loopingHandler = Stopwatch.StartNew();
                    using var dInputSystem = DInput.DirectInput8Create();
                    var defaultController = dInputSystem.CreateDevice(PredefinedDevice.SysKeyboard);
                    defaultController.SetCooperativeLevel(handle, CooperativeLevel.NonExclusive | CooperativeLevel.Foreground);
                    defaultController.SetDataFormat<RawKeyboardState>();
                    defaultController.Properties.BufferSize = 16;
                    while (_isAvailable)
                    {
                        foreach (var data in Utility.GetInputData<KeyboardUpdate>(defaultController))
                        {
                            _handleInput.HandleInput(data.Key switch
                            {
                                Vortice.DirectInput.Key.Escape => VirtualKey.Escape,
                                Vortice.DirectInput.Key.D0 => VirtualKey.Number0,
                                Vortice.DirectInput.Key.D1 => VirtualKey.Number1,
                                Vortice.DirectInput.Key.D2 => VirtualKey.Number2,
                                Vortice.DirectInput.Key.D3 => VirtualKey.Number3,
                                Vortice.DirectInput.Key.D4 => VirtualKey.Number4,
                                Vortice.DirectInput.Key.D5 => VirtualKey.Number5,
                                Vortice.DirectInput.Key.D6 => VirtualKey.Number6,
                                Vortice.DirectInput.Key.D7 => VirtualKey.Number7,
                                Vortice.DirectInput.Key.D8 => VirtualKey.Number8,
                                Vortice.DirectInput.Key.D9 => VirtualKey.Number9,
                                Vortice.DirectInput.Key.Minus => (VirtualKey)189,
                                Vortice.DirectInput.Key.Equals => (VirtualKey)187,
                                Vortice.DirectInput.Key.Back => VirtualKey.Back,
                                Vortice.DirectInput.Key.Tab => VirtualKey.Tab,
                                Vortice.DirectInput.Key.A => VirtualKey.A,
                                Vortice.DirectInput.Key.B => VirtualKey.B,
                                Vortice.DirectInput.Key.C => VirtualKey.C,
                                Vortice.DirectInput.Key.D => VirtualKey.D,
                                Vortice.DirectInput.Key.E => VirtualKey.E,
                                Vortice.DirectInput.Key.F => VirtualKey.F,
                                Vortice.DirectInput.Key.G => VirtualKey.G,
                                Vortice.DirectInput.Key.H => VirtualKey.H,
                                Vortice.DirectInput.Key.I => VirtualKey.I,
                                Vortice.DirectInput.Key.J => VirtualKey.J,
                                Vortice.DirectInput.Key.K => VirtualKey.K,
                                Vortice.DirectInput.Key.L => VirtualKey.L,
                                Vortice.DirectInput.Key.M => VirtualKey.M,
                                Vortice.DirectInput.Key.N => VirtualKey.N,
                                Vortice.DirectInput.Key.O => VirtualKey.O,
                                Vortice.DirectInput.Key.P => VirtualKey.P,
                                Vortice.DirectInput.Key.Q => VirtualKey.Q,
                                Vortice.DirectInput.Key.R => VirtualKey.R,
                                Vortice.DirectInput.Key.S => VirtualKey.S,
                                Vortice.DirectInput.Key.T => VirtualKey.T,
                                Vortice.DirectInput.Key.U => VirtualKey.U,
                                Vortice.DirectInput.Key.V => VirtualKey.V,
                                Vortice.DirectInput.Key.W => VirtualKey.W,
                                Vortice.DirectInput.Key.X => VirtualKey.X,
                                Vortice.DirectInput.Key.Y => VirtualKey.Y,
                                Vortice.DirectInput.Key.Z => VirtualKey.Z,
                                Vortice.DirectInput.Key.LeftBracket => (VirtualKey)219,
                                Vortice.DirectInput.Key.RightBracket => (VirtualKey)221,
                                Vortice.DirectInput.Key.Return => VirtualKey.Enter,
                                Vortice.DirectInput.Key.LeftControl => VirtualKey.LeftControl,
                                Vortice.DirectInput.Key.Semicolon => (VirtualKey)186,
                                Vortice.DirectInput.Key.Apostrophe => (VirtualKey)222,
                                Vortice.DirectInput.Key.Grave => (VirtualKey)192,
                                Vortice.DirectInput.Key.LeftShift => VirtualKey.LeftShift,
                                Vortice.DirectInput.Key.Backslash => (VirtualKey)220,
                                Vortice.DirectInput.Key.Comma => (VirtualKey)188,
                                Vortice.DirectInput.Key.Period => (VirtualKey)190,
                                Vortice.DirectInput.Key.Slash => (VirtualKey)191,
                                Vortice.DirectInput.Key.RightShift => VirtualKey.RightShift,
                                Vortice.DirectInput.Key.Multiply => VirtualKey.Multiply,
                                Vortice.DirectInput.Key.LeftAlt => VirtualKey.LeftMenu,
                                Vortice.DirectInput.Key.Space => VirtualKey.Space,
                                Vortice.DirectInput.Key.Capital => VirtualKey.CapitalLock,
                                Vortice.DirectInput.Key.F1 => VirtualKey.F1,
                                Vortice.DirectInput.Key.F2 => VirtualKey.F2,
                                Vortice.DirectInput.Key.F3 => VirtualKey.F3,
                                Vortice.DirectInput.Key.F4 => VirtualKey.F4,
                                Vortice.DirectInput.Key.F5 => VirtualKey.F5,
                                Vortice.DirectInput.Key.F6 => VirtualKey.F6,
                                Vortice.DirectInput.Key.F7 => VirtualKey.F7,
                                Vortice.DirectInput.Key.F8 => VirtualKey.F8,
                                Vortice.DirectInput.Key.F9 => VirtualKey.F9,
                                Vortice.DirectInput.Key.F10 => VirtualKey.F10,
                                Vortice.DirectInput.Key.F11 => VirtualKey.F11,
                                Vortice.DirectInput.Key.F12 => VirtualKey.F12,
                                Vortice.DirectInput.Key.F13 => VirtualKey.F13,
                                Vortice.DirectInput.Key.F14 => VirtualKey.F14,
                                Vortice.DirectInput.Key.F15 => VirtualKey.F15,
                                Vortice.DirectInput.Key.NumberLock => VirtualKey.NumberKeyLock,
                                Vortice.DirectInput.Key.ScrollLock => VirtualKey.Scroll,
                                Vortice.DirectInput.Key.NumberPad0 => VirtualKey.NumberPad0,
                                Vortice.DirectInput.Key.NumberPad1 => VirtualKey.NumberPad1,
                                Vortice.DirectInput.Key.NumberPad2 => VirtualKey.NumberPad2,
                                Vortice.DirectInput.Key.NumberPad3 => VirtualKey.NumberPad3,
                                Vortice.DirectInput.Key.NumberPad4 => VirtualKey.NumberPad4,
                                Vortice.DirectInput.Key.NumberPad5 => VirtualKey.NumberPad5,
                                Vortice.DirectInput.Key.NumberPad6 => VirtualKey.NumberPad6,
                                Vortice.DirectInput.Key.NumberPad7 => VirtualKey.NumberPad7,
                                Vortice.DirectInput.Key.NumberPad8 => VirtualKey.NumberPad8,
                                Vortice.DirectInput.Key.NumberPad9 => VirtualKey.NumberPad9,
                                Vortice.DirectInput.Key.NumberPadEnter => VirtualKey.Enter,
                                Vortice.DirectInput.Key.Subtract => VirtualKey.Subtract,
                                Vortice.DirectInput.Key.Add => VirtualKey.Add,
                                Vortice.DirectInput.Key.Decimal => VirtualKey.Decimal,
                                Vortice.DirectInput.Key.Kana => VirtualKey.Kana,
                                Vortice.DirectInput.Key.Convert => VirtualKey.Convert,
                                Vortice.DirectInput.Key.NoConvert => VirtualKey.NonConvert,
                                Vortice.DirectInput.Key.PreviousTrack => (VirtualKey)177,
                                Vortice.DirectInput.Key.Kanji => VirtualKey.Kanji,
                                Vortice.DirectInput.Key.Stop => VirtualKey.Stop,
                                Vortice.DirectInput.Key.NextTrack => (VirtualKey)176,
                                Vortice.DirectInput.Key.RightControl => VirtualKey.RightControl,
                                Vortice.DirectInput.Key.PlayPause => (VirtualKey)179,
                                Vortice.DirectInput.Key.VolumeDown => (VirtualKey)174,
                                Vortice.DirectInput.Key.VolumeUp => (VirtualKey)175,
                                Vortice.DirectInput.Key.WebHome => VirtualKey.GoHome,
                                Vortice.DirectInput.Key.Divide => VirtualKey.Divide,
                                Vortice.DirectInput.Key.PrintScreen => VirtualKey.Snapshot,
                                Vortice.DirectInput.Key.RightAlt => VirtualKey.RightMenu,
                                Vortice.DirectInput.Key.Pause => VirtualKey.Pause,
                                Vortice.DirectInput.Key.Home => VirtualKey.Home,
                                Vortice.DirectInput.Key.Up => VirtualKey.Up,
                                Vortice.DirectInput.Key.PageUp => VirtualKey.PageUp,
                                Vortice.DirectInput.Key.Left => VirtualKey.Left,
                                Vortice.DirectInput.Key.Right => VirtualKey.Right,
                                Vortice.DirectInput.Key.End => VirtualKey.End,
                                Vortice.DirectInput.Key.Down => VirtualKey.Down,
                                Vortice.DirectInput.Key.PageDown => VirtualKey.PageDown,
                                Vortice.DirectInput.Key.Insert => VirtualKey.Insert,
                                Vortice.DirectInput.Key.Delete => VirtualKey.Delete,
                                Vortice.DirectInput.Key.LeftWindowsKey => VirtualKey.LeftWindows,
                                Vortice.DirectInput.Key.RightWindowsKey => VirtualKey.RightWindows,
                                Vortice.DirectInput.Key.Applications => VirtualKey.Application,
                                Vortice.DirectInput.Key.Sleep => VirtualKey.Sleep,
                                Vortice.DirectInput.Key.WebSearch => VirtualKey.Search,
                                Vortice.DirectInput.Key.WebFavorites => VirtualKey.Favorites,
                                Vortice.DirectInput.Key.WebRefresh => VirtualKey.Refresh,
                                Vortice.DirectInput.Key.WebForward => VirtualKey.GoForward,
                                Vortice.DirectInput.Key.WebBack => VirtualKey.GoBack,
                                Vortice.DirectInput.Key.MediaSelect => VirtualKey.Select,
                                _ => VirtualKey.None
                            },
                            data.IsPressed);
                        }

                        loopingCounter += 1000.0 / Configure.Instance.LoopUnit;
                        var toWait = loopingCounter - loopingHandler.GetMillis();
                        if (toWait > 0.0)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(toWait));
                        }
                    }
                }
                catch (Exception e)
                {
                    Utility.SetFault(FaultEntryPath, e);
                }
            }
        });

        public void Dispose()
        {
            _isAvailable = false;
        }
    }
}