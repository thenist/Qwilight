namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static T GetValue<T>(T[] data, int i) => i < data.Length ? data[i] : default;

        public static void SetValue<T>(this T[] data, int i, T value)
        {
            if (i < data.Length)
            {
                data[i] = value;
            }
        }

        public static void SetValue<T>(this T[][] data, int i, int j, T value)
        {
            if (i < data.Length && j < data[i].Length)
            {
                data[i][j] = value;
            }
        }

        public static void SetValue<T>(this T[][][] data, int i, int j, int m, T value)
        {
            if (i < data.Length && j < data[i].Length && m < data[i][j].Length)
            {
                data[i][j][m] = value;
            }
        }

        public static void SetValue<T>(this T[][][][] data, int i, int j, int m, int o, T value)
        {
            if (i < data.Length && j < data[i].Length && m < data[i][j].Length && o < data[i][j][m].Length)
            {
                data[i][j][m][o] = value;
            }
        }

        public static void SetValue<T>(this T[,] data, int i, int j, T value)
        {
            if (i < data.GetLength(0) && j < data.GetLength(1))
            {
                data[i, j] = value;
            }
        }
    }
}