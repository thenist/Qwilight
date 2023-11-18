using System.IO;
using System.Text;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static (string, string) SaveFaultFile(string faultEntryPath, Exception e)
        {
            var faultFilePath = Path.Combine(faultEntryPath, Path.GetInvalidFileNameChars().Aggregate($"{DateTime.Now:F}.log", (faultFileName, target) => faultFileName.Replace(target, ' ')));
            var builder = new StringBuilder();
            var fault = e;
            builder.AppendLine(fault.ToString());
            while ((fault = fault.InnerException) != null)
            {
                builder.AppendLine();
                builder.AppendLine(fault.ToString());
            }
            var faultText = builder.ToString();
            Console.WriteLine(faultText);
            Directory.CreateDirectory(Path.GetDirectoryName(faultFilePath));
            File.WriteAllText(faultFilePath, faultText, Encoding.UTF8);
            return (faultFilePath, faultText);
        }
    }
}
