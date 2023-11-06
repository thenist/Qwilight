/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Xml2CSharp
{
	[XmlRoot(ElementName = "course")]
	public class Course
	{
		[XmlElement(ElementName = "title")]
		public string Title { get; set; }
		[XmlElement(ElementName = "line")]
		public string Line { get; set; }
		[XmlElement(ElementName = "hash")]
		public string Hash { get; set; }
		[XmlElement(ElementName = "type")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "courselist")]
	public class Courselist
	{
		[XmlElement(ElementName = "course")]
		public List<Course> Course { get; set; }
	}

}
