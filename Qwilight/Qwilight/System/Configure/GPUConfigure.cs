using Qwilight.Utilities;
using System.IO;
using System.Text;
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

        static readonly string _fileName = Path.Combine(QwilightComponent.QwilightEntryPath, "GPU Configure.json");
        static readonly string _faultFileName = Path.ChangeExtension(_fileName, ".json.$");
        static readonly string _tmp0FileName = Path.ChangeExtension(_fileName, ".json.tmp.0");
        static readonly string _tmp1FileName = Path.ChangeExtension(_fileName, ".json.tmp.1");

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
                var textConfigure = Utility.GetJSON<GPUConfigure>(File.ReadAllText(_fileName, Encoding.UTF8));
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

        public void Save()
        {
            Utility.CopyFile(_fileName, _tmp0FileName);
            Utility.MoveFile(_tmp0FileName, _tmp1FileName);
            Utility.SaveText(_fileName, Utility.SetJSON(this, new JsonSerializerOptions
            {
                WriteIndented = QwilightComponent.IsVS
            }));
            Utility.WipeFile(_tmp1FileName);
        }

        public void Validate(bool isUndo)
        {
            if (isUndo || Utility.IsLowerDate(Date, 1, 16, 2))
            {
                GPUModeValue = GPUMode.Default;
            }
            Date = QwilightComponent.Date;
        }
    }
}