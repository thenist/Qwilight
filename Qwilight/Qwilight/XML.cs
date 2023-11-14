using System.Xml.Serialization;

namespace Qwilight
{
    public static class XML
    {
        [XmlRoot(ElementName = "courselist")]
        public sealed class Courselist
        {
            [XmlElement(ElementName = "course")]
            public Course[] Course { get; set; }
        }

        public struct Course
        {
            public string title;
            public string hash;
        }
    }
}
