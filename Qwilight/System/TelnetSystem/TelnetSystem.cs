using System.IO;
using Windows.Win32;

namespace Qwilight
{
    public sealed class TelnetSystem : Model, IHandleTelnet
    {
        public static readonly TelnetSystem Instance = QwilightComponent.GetBuiltInData<TelnetSystem>(nameof(TelnetSystem));

        bool _isAlwaysNewStand;
        bool _isAvailable;

        public bool IsAlwaysNewStand
        {
            get => IsAvailable && _isAlwaysNewStand;

            set => _isAlwaysNewStand = value;
        }

        public bool IsAvailable
        {
            get => _isAvailable;

            set => SetProperty(ref _isAvailable, value, nameof(IsAvailable));
        }

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