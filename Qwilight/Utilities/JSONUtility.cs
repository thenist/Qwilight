using Qwilight.XOR;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        sealed class XORInt32Modifier : JsonConverter<XORInt32>
        {
            public override XORInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (XORInt32)Utility.ToInt32(reader.GetString());

            public override void Write(Utf8JsonWriter writer, XORInt32 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }

        sealed class XORFloat64Modifier : JsonConverter<XORFloat64>
        {
            public override XORFloat64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (XORFloat64)Utility.ToFloat64(reader.GetString());

            public override void Write(Utf8JsonWriter writer, XORFloat64 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }

        sealed class FontFamilyModifier : JsonConverter<FontFamily>
        {
            public override FontFamily Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => new FontFamily(reader.GetString());

            public override void Write(Utf8JsonWriter writer, FontFamily value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        static readonly JsonSerializerOptions _defaultJSONConfigure = GetJSONConfigure();

        public static JsonSerializerOptions GetJSONConfigure(Action<JsonSerializerOptions> onJSONConfigure = null)
        {
            var defaultJSONConfigure = new JsonSerializerOptions
            {
                Converters =
                {
                    new XORInt32Modifier(),
                    new XORFloat64Modifier(),
                    new FontFamilyModifier()
                },
                IncludeFields = true,
                WriteIndented = QwilightComponent.IsVS
            };
            onJSONConfigure?.Invoke(defaultJSONConfigure);
            return defaultJSONConfigure;
        }

        public static T GetJSON<T>(string text, JsonSerializerOptions defaultJSONConfigure = null) => JsonSerializer.Deserialize<T>(text, defaultJSONConfigure ?? _defaultJSONConfigure);

        public static T GetJSON<T>(byte[] data) => JsonSerializer.Deserialize<T>(data, _defaultJSONConfigure);

        public static async ValueTask<T> GetJSON<T>(Stream s) => await JsonSerializer.DeserializeAsync<T>(s, _defaultJSONConfigure);

        public static string SetJSON<T>(T data, JsonSerializerOptions defaultJSONConfigure = null) => JsonSerializer.Serialize(data, defaultJSONConfigure ?? _defaultJSONConfigure);
    }
}
