namespace Qwilight
{
    public struct FastEntryItem
    {
        public string noteFilePath;
        public string noteID128;
        public string noteID256;
        public string noteID512;
        public IList<int> dataIDs = Array.Empty<int>();

        public FastEntryItem()
        {
        }
    }
}
