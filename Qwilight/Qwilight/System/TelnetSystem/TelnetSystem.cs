using System.IO;
using Windows.Win32;

namespace Qwilight
{
    public sealed class TelnetSystem : IHandleTelnet
    {
        public static readonly TelnetSystem Instance = new();

        bool _isAlwaysNewStand;

        public bool IsAlwaysNewStand
        {
            get => IsAvailable && _isAlwaysNewStand;

            set => _isAlwaysNewStand = value;
        }

        public bool IsAvailable { get; set; }

        DefaultTelnetSystem _defaultTelnetSystem;

        public void Toggle()
        {
            if (IsAvailable = !IsAvailable)
            {
                PInvoke.AllocConsole();
                _defaultTelnetSystem = new(this);
                _defaultTelnetSystem.HandleSystem();
            }
            else
            {
                _defaultTelnetSystem.Stop();
                _defaultTelnetSystem = null;
                PInvoke.FreeConsole();
            }
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            });
        }
    }
}