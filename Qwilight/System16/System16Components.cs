namespace Qwilight.System16
{
    public static class System16Components
    {
        public static readonly bool Is1221 = DateTime.Now.Month == 12 && DateTime.Now.Day == 21;
        public static readonly bool Is1225 = DateTime.Now.Month == 12 && (DateTime.Now.Day == 24 | DateTime.Now.Day == 25);
    }
}
