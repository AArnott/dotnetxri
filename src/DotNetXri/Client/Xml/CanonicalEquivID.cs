package org.openxri.xml;

import java.net.URISyntaxException;

import org.w3c.dom.Element;

public class CanonicalEquivID extends SimpleXMLElement {

	public CanonicalEquivID(CanonicalEquivID ceid) {
		super(ceid);
	}

	public CanonicalEquivID() {
		super(Tags.TAG_CANONICALEQUIVID);
	}
	
	public CanonicalEquivID(String canonicalidString) {
		super(Tags.TAG_CANONICALEQUIVID);
		setValue(canonicalidString);
	}

	/**
	 * This method constructs the object from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
	public CanonicalEquivID(Element elem) throws URISyntaxException
	{
		super(Tags.TAG_CANONICALEQUIVID);
		fromXML(elem);
	}

}
