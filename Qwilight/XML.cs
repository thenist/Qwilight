using System.Xml.Serialization;

namespace Qwilight
{
    public static class XML
    {
        [XmlRoot(ElementName = "courselist")]
        public sealed class LR2CRS
        {
            [XmlElement(ElementName = "course")]
            public Item[] Items { get; set; }

            public struct Item
            {
                public string title;
                public string hash;
            }
        }
    }
}
