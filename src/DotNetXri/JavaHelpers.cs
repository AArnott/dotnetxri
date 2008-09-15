using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DotNetXri {
	internal static class JavaHelpers {

		internal static bool hasAttributeNS(this XmlElement element, string ns, string localName) {
			return element.HasAttribute(localName, ns);
		}

		internal static string getAttributeNS(this XmlElement element, string ns, string localName) {
			return element.GetAttribute(localName, ns);
		}

		internal static XmlElement createElementNS(this XmlDocument doc, string ns, string localName) {
			return doc.CreateElement(localName, ns);
		}
	}
}
