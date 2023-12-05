using Qwilight.Utilities;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qwilight
{
    public class GPUConfigure : Model
    {
        public enum GPUMode
        {
            Default, NVIDIA
        }

        public static readonly GPUConfigure Instance = new();

        static readonly JsonSerializerOptions _defaultJSONConfigure = Utility.GetJSONConfigure(defaultJSONConfigure =>
        {
            defaultJSONConfigure.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            defaultJSONConfigure.IgnoreReadOnlyProperties = true;
        });
        static readonly string _fileName = Path.Combine(QwilightComponent.QwilightEntryPath, "GPU Configure.json");
        static readonly string _faultFileName = Path.ChangeExtension(_fileName, ".json.$");
        static readonly string _tmp0FileName = Path.ChangeExtension(_fileName, ".json.tmp.0");
        static readonly string _tmp1FileName = Path.ChangeExtension(_fileName, ".json.tmp.1");

        readonly object _setSaveCSX = new();

        public void Load()
        {
            Utility.WipeFile(_tmp0FileName);
            Utility.MoveFile(_tmp1FileName, _fileName);
            try
            {
                if (File.Exists(_fileName))
                {
                    LoadImpl();
                    Validate(false);
                }
                else
                {
                    Validate(true);
                }
            }
            catch (Exception e)
            {
                GPUConfigureFault = $"Failed to Validate GPU Configure ({e.Message})";
                Validate(true);
                Utility.MoveFile(_fileName, _faultFileName);
            }

            void LoadImpl()
            {
                var textConfigure = Utility.GetJSON<GPUConfigure>(File.ReadAllText(_fileName, Encoding.UTF8), _defaultJSONConfigure);
                foreach (var value in typeof(GPUConfigure).GetProperties().Where(value => value.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Length == 0 && value.CanWrite))
                {
                    value.SetValue(this, value.GetValue(textConfigure));
                }
            }
        }

        public Version Date { get; set; }

        public GPUMode GPUModeValue { get; set; }

        [JsonIgnore]
        public string GPUConfigureFault { get; set; }

        public void Save(bool isParallel)
        {
            if (isParallel)
            {
                Task.Run(SaveImpl);
            }
            else
            {
                SaveImpl();
            }

            void SaveImpl()
            {
                lock (_setSaveCSX)
                {
                    Utility.CopyFile(_fileName, _tmp0FileName);
                    Utility.MoveFile(_tmp0FileName, _tmp1FileName);
                    File.WriteAllText(_fileName, Utility.SetJSON(this, _defaultJSONConfigure), Encoding.UTF8);
                    Utility.WipeFile(_tmp1FileName);
                }
            }
        }

        public void Validate(bool isInit)
        {
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 2))
            {
                GPUModeValue = GPUMode.Default;
            }
            Date = QwilightComponent.Date;
        }
    }
}