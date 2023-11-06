namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static bool IsLowerDate(Version date, int targetMajor, int targetMinor, int targetBuild)
        {
            if (date != null)
            {
                var dateMajor = date.Major;
                if (dateMajor != targetMajor)
                {
                    return dateMajor < targetMajor;
                }
                var dateMinor = date.Minor;
                if (dateMinor != targetMinor)
                {
                    return dateMinor < targetMinor;
                }
                return date.Build < targetBuild;
            }
            else
            {
                return true;
            }
        }
    }
}