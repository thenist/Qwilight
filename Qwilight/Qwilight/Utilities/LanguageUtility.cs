namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static string GetLanguage(int language)
        {
            return language switch
            {
                1042 => "ko-KR",
                _ => "en-US",
            };
        }

        public static int GetLCID(string language)
        {
            return language switch
            {
                "ko-KR" => 1042,
                _ => 1033,
            };
        }
    }
}
