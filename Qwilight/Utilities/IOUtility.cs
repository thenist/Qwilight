using System.Diagnostics;
using System.IO;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static bool WaitUntilCanOpen(string filePath, double waitMax = double.PositiveInfinity)
        {
            var loopingHandler = Stopwatch.StartNew();
            while (loopingHandler.GetMillis() < waitMax)
            {
                try
                {
                    using var fs = File.OpenRead(filePath);
                    return true;
                }
                catch
                {
                    Thread.Sleep(1);
                }
            }
            return false;
        }

        public static void MoveFile(string src, string target)
        {
            try
            {
                if (File.Exists(src))
                {
                    WipeFile(target);
                    WipeEntry(target);
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    File.Move(src, target);
                }
            }
            catch
            {
            }
        }

        public static void CopyFile(string src, string target)
        {
            try
            {
                if (File.Exists(src))
                {
                    if (File.Exists(target))
                    {
                        using (var fs0 = File.OpenRead(src))
                        using (var fs1 = File.OpenRead(target))
                        {
                            if (Utility.GetID128(fs0) == Utility.GetID128(fs1))
                            {
                                return;
                            }
                        }

                        WipeFile(target);
                        WipeEntry(target);
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(target));
                    File.Copy(src, target, true);
                }
            }
            catch
            {
            }
        }

        public static void WipeFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }
        }

        public static void WipeEntry(string entryPath)
        {
            try
            {
                if (Directory.Exists(entryPath))
                {
                    Directory.Delete(entryPath, true);
                }
            }
            catch
            {
            }
        }

        public static void CopyEntry(string src, string target)
        {
            foreach (var filePath in GetFiles(src))
            {
                CopyFile(filePath, Path.Combine(target, Path.GetFileName(filePath)));
            }
            foreach (var entryPath in GetEntry(src))
            {
                CopyEntry(entryPath, Path.Combine(target, Path.GetFileName(entryPath)));
            }
        }

        public static string[] GetFiles(string entryPath, string o = "")
        {
            try
            {
                return Directory.Exists(entryPath) ? Directory.GetFiles(entryPath, o) : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        public static string[] GetEntry(string entryPath, string o = "")
        {
            try
            {
                return Directory.Exists(entryPath) ? Directory.GetDirectories(entryPath, o) : Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
    }
}