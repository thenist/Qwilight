namespace Qwilight.System16
{
    public static class System16Components
    {
        public static bool Is1221 { get; set; } = DateTime.Now.Month == 12 && DateTime.Now.Day == 21;

        public static bool Is1225 { get; set; } = DateTime.Now.Month == 12 && (DateTime.Now.Day == 24 | DateTime.Now.Day == 25);
    }
}
