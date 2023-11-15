using Qwilight.XOR;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public sealed class XORInt32Modifier : JsonConverter<XORInt32>
        {
            public override XORInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (XORInt32)Utility.ToInt32(reader.GetString());

            public override void Write(Utf8JsonWriter writer, XORInt32 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }

        public sealed class XORFloat64Modifier : JsonConverter<XORFloat64>
        {
            public override XORFloat64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (XORFloat64)Utility.ToFloat64(reader.GetString());

            public override void Write(Utf8JsonWriter writer, XORFloat64 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }

        static readonly JsonSerializerOptions _defaultGetJSONConfigure = new()
        {
            IncludeFields = true
        };
        static readonly JsonSerializerOptions _defaultSetJSONConfigure = new()
        {
            Converters =
            {
                new XORInt32Modifier(),
                new XORFloat64Modifier()
            },
            IncludeFields = true,
            IgnoreReadOnlyProperties = true,
            WriteIndented = QwilightComponent.IsVS
        };

        public static T GetJSON<T>(string text, JsonSerializerOptions defaultJSONConfigure = null) => JsonSerializer.Deserialize<T>(text, defaultJSONConfigure ?? _defaultGetJSONConfigure);

        public static T GetJSON<T>(byte[] data) => JsonSerializer.Deserialize<T>(data, _defaultGetJSONConfigure);

        public static async ValueTask<T> GetJSON<T>(Stream s) => await JsonSerializer.DeserializeAsync<T>(s, _defaultGetJSONConfigure);

        public static string SetJSON<T>(T data, JsonSerializerOptions defaultJSONConfigure = null) => JsonSerializer.Serialize(data, defaultJSONConfigure ?? _defaultSetJSONConfigure);
    }
}
