package org.openxri.xml;

using java.net.URISyntaxException;

using org.w3c.dom.Element;

public class CanonicalEquivID :SimpleXMLElement {

	public CanonicalEquivID(CanonicalEquivID ceid) : base(ceid) {
	}

	public CanonicalEquivID() : base(Tags.TAG_CANONICALEQUIVID) {
	}
	
	public CanonicalEquivID(String canonicalidString) : base(Tags.TAG_CANONICALEQUIVID) {
		setValue(canonicalidString);
	}

	/**
	 * This method constructs the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
	public CanonicalEquivID(Element elem) throws URISyntaxException
	: base(Tags.TAG_CANONICALEQUIVID) {
		fromXML(elem);
	}

}
