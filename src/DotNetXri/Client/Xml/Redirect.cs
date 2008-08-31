package org.openxri.xml;

import java.net.URISyntaxException;

import org.w3c.dom.Element;

public class Redirect :SimpleXMLElement {

	public Redirect() : base(Tags.TAG_REDIRECT) {
	}

	public Redirect(String sValue) : base(Tags.TAG_REDIRECT) {
		setValue(sValue);
	}

	/**
	 * This method constructs the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
	public Redirect(Element oElem) throws URISyntaxException : base(Tags.TAG_REDIRECT) {
		fromXML(oElem);
		
		// make sure that the priority (if present) is valid (vommits exception if invalid)
		String val = oElem.getAttribute(Tags.ATTR_PRIORITY);
		if (val != null && !val.equals(""))
			Integer.parseInt(val); 
	}

	/**
	 * @return Returns the priority.
	 */
	public Integer getPriority() {
		String val = getAttributeValue(Tags.ATTR_PRIORITY);
		if (val == null || val.equals(""))
			return null;
		return new Integer(val);
	}


	public void setPriority(Integer priority) {
		if (priority == null)
			this.removeAttribute(Tags.ATTR_PRIORITY);
		else
			this.addAttribute(Tags.ATTR_PRIORITY, priority.toString());
	}

	/**
	 * @return Returns the priority.
	 */
	public String getAppend() {
		String val = getAttributeValue(Tags.ATTR_APPEND);
		return val;
	}


	public void setAppend(String append) {
		if (append == null)
			this.removeAttribute(Tags.ATTR_APPEND);
		else
			this.addAttribute(Tags.ATTR_APPEND, append);
	}
}
