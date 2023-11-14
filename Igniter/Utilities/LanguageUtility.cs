namespace Igniter.Utilities
{
    public static partial class Utility
    {
        public static string GetLanguage(int language)
        {
            switch (language)
            {
                case 1042:
                    return "ko-KR";
                default:
                    return "en-US";
            }
        }
    }
}
