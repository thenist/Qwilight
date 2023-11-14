namespace Qwilight.UIComponent
{
    public struct NetSIteCommentItems
    {
        public List<NetSiteCommentItem> NetSiteCommentItems { get; init; }

        public long Date { get; set; }

        public override string ToString() => DateTimeOffset.FromUnixTimeMilliseconds(Date).LocalDateTime.ToShortTimeString();
    }
}