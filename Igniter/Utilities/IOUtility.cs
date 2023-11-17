using System.IO;

namespace Igniter.Utilities
{
    public static partial class Utility
    {
        public static bool WipeFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}