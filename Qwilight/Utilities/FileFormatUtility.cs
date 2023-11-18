using System.IO;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        [Flags]
        public enum FileFormatFlag
        {
            Not = 0,
            Audio = 1,
            Drawing = 2,
            Media = 4
        }

        public static FileFormatFlag GetFileFormat(string filePath)
        {
            foreach (var audioFileFormat in QwilightComponent.AudioFileFormats)
            {
                if (filePath.IsTailCaselsss(audioFileFormat))
                {
                    return FileFormatFlag.Audio;
                }
            }
            foreach (var mediaFileFormat in QwilightComponent.MediaFileFormats)
            {
                if (filePath.IsTailCaselsss(mediaFileFormat))
                {
                    return FileFormatFlag.Media;
                }
            }
            foreach (var drawingFileFormat in QwilightComponent.DrawingFileFormats)
            {
                if (filePath.IsTailCaselsss(drawingFileFormat))
                {
                    return FileFormatFlag.Drawing;
                }
            }
            return FileFormatFlag.Not;
        }

        public static string GetFilePath(string filePath, FileFormatFlag availableFlags)
        {
            if (File.Exists(filePath))
            {
                return filePath;
            }
            return Utility.GetFiles(Path.GetDirectoryName(filePath), $"{Path.GetFileNameWithoutExtension(filePath)}.*").Order().FirstOrDefault(targetFilePath =>
            {
                if (File.Exists(targetFilePath))
                {
                    if ((availableFlags & FileFormatFlag.Audio) == FileFormatFlag.Audio && GetFileFormat(targetFilePath) == FileFormatFlag.Audio)
                    {
                        return true;
                    }
                    if ((availableFlags & FileFormatFlag.Drawing) == FileFormatFlag.Drawing && GetFileFormat(targetFilePath) == FileFormatFlag.Drawing)
                    {
                        return true;
                    }
                    if ((availableFlags & FileFormatFlag.Media) == FileFormatFlag.Media && GetFileFormat(targetFilePath) == FileFormatFlag.Media)
                    {
                        return true;
                    }
                }
                return false;
            });
        }
    }
}
