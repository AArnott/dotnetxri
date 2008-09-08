namespace DotNetXri.Client.Xml {
	//using java.net.URISyntaxException;
	//using org.w3c.dom.XmlElement;

	public class Ref : SimpleXMLElement {

		public Ref()
			: base(Tags.TAG_REF) {
		}

		public Ref(string sValue)
			: base(Tags.TAG_REF) {
			setValue(sValue);
		}

		/**
		*  This method constructs the obj from DOM.  It does not keep a
		* copy of the DOM around.  Whitespace information is lost in this process.
		*/
		public Ref(XmlElement oElem) /*throws URISyntaxException */: base(Tags.TAG_REF) {
			fromXML(oElem);

			// make sure that the priority (if present) is valid (vommits exception if invalid)
			string val = oElem.GetAttribute(Tags.ATTR_PRIORITY);
			if (val != null && !val.Equals(""))
				int.Parse(val);
		} // Constructor()

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

			if (priority == null) {

				this.removeAttribute(Tags.ATTR_PRIORITY);
			} else {

				this.addAttribute(Tags.ATTR_PRIORITY, priority.ToString());
			}
		}
	}
}