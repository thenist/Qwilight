namespace Qwilight.UIComponent
{
    public struct NetSiteCommentItems
    {
        public List<NetSiteCommentItem> Values { get; init; }

        public long Date { get; set; }

        public override string ToString() => DateTimeOffset.FromUnixTimeMilliseconds(Date).LocalDateTime.ToShortTimeString();
    }
}