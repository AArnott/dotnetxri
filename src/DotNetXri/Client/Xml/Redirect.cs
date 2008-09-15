using System.Xml;
namespace DotNetXri.Client.Xml {

	//using java.net.UriFormatException;
	//using org.w3c.dom.XmlElement;

	public class Redirect : SimpleXMLElement {

		public Redirect()
			: base(Tags.TAG_REDIRECT) {
		}

		public Redirect(string sValue)
			: base(Tags.TAG_REDIRECT) {
			setValue(sValue);
		}

		/**
		* This method constructs the obj from DOM.  It does not keep a
		* copy of the DOM around.  Whitespace information is lost in this process.
		*/
		public Redirect(XmlElement oElem)/* throws UriFormatException*/: base(Tags.TAG_REDIRECT) {
			fromXML(oElem);

			// make sure that the priority (if present) is valid (vommits exception if invalid)
			string val = oElem.GetAttribute(Tags.ATTR_PRIORITY);
			if (val != null && !val.Equals(""))
				int.Parse(val);
		}

		/**
		* @return Returns the priority.
		*/
		public int? getPriority() {
			string val = getAttributeValue(Tags.ATTR_PRIORITY);
			if (val == null || val.Equals(""))
				return null;
			return int.Parse(val);
		}


		public void setPriority(int? priority) {
			if (priority == null)
				this.removeAttribute(Tags.ATTR_PRIORITY);
			else
				this.addAttribute(Tags.ATTR_PRIORITY, priority.ToString());
		}

		/**
		* @return Returns the priority.
		*/
		public string getAppend() {
			string val = getAttributeValue(Tags.ATTR_APPEND);
			return val;
		}


		public void setAppend(string append) {
			if (append == null)
				this.removeAttribute(Tags.ATTR_APPEND);
			else
				this.addAttribute(Tags.ATTR_APPEND, append);
		}
	}
}