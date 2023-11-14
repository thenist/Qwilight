using System.IO;
using System.Security.Cryptography;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static byte[] GetID512(byte[] data)
        {
            using var shaComputer = SHA512.Create();
            return shaComputer.ComputeHash(data);
        }

        public static string GetID512s(byte[] data) => ModifyID(GetID512(data));

        public static byte[] GetID256(byte[] data)
        {
            using var shaComputer = SHA256.Create();
            return shaComputer.ComputeHash(data);
        }

        public static string GetID256s(byte[] data) => ModifyID(GetID256(data));

        public static string GetID128s(Stream s)
        {
            s.Position = 0;
            using var hashComputer = MD5.Create();
            return ModifyID(hashComputer.ComputeHash(s));
        }

        public static string GetID128s(byte[] data)
        {
            using var hashComputer = MD5.Create();
            return ModifyID(hashComputer.ComputeHash(data));
        }

        public static string ModifyID(byte[] hash) => BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}
