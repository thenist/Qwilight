using System;
using System.IO;

namespace Igniter
{
    public static partial class IgniterComponent
    {
        public static readonly string QwilightFilePath;

        public static Func<string, object> OnGetBuiltInData { get; set; }

        public static T GetBuiltInData<T>(string data)
        {
            var value = OnGetBuiltInData(data);
            return value != null ? (T)value : default;
        }

        static IgniterComponent()
        {
            QwilightFilePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "Qwilight.exe");
        }
    }
}