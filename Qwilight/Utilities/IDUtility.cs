using System.IO;
using System.Security.Cryptography;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static string GetID512(byte[] data) => ModifyID(SHA512.HashData(data));

        public static string GetID256(byte[] data) => ModifyID(SHA256.HashData(data));

        public static string GetID128(Stream s)
        {
            s.Position = 0;
            return ModifyID(MD5.HashData(s));
        }

        public static string GetID128(byte[] data) => ModifyID(MD5.HashData(data));

        static string ModifyID(byte[] hash) => BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}
