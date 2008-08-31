package org.openxri.xml;

import java.net.URISyntaxException;

import org.w3c.dom.Element;

public class CanonicalID extends SimpleXMLElement{

	public CanonicalID() {
		super(Tags.TAG_CANONICALID);
	}
	
	public CanonicalID(String canonicalidString) {
		super(Tags.TAG_CANONICALID);
		setValue(canonicalidString);
	}

	/**
	 * This method constructs the object from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
	public CanonicalID(Element elem) throws URISyntaxException
	{
		super(Tags.TAG_CANONICALID);
		fromXML(elem);
	}

}
