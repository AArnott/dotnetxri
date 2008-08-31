package org.openxri.xml;

import java.net.URISyntaxException;

import org.w3c.dom.Element;

public class LocalID :SimpleXMLElement{

  public LocalID(){
	  super(Tags.TAG_LOCALID);
  }
  
  public LocalID(String localidString){
	  super(Tags.TAG_LOCALID);
	  setValue(localidString);
  }
  
  /*
   ****************************************************************************
   * Constructor()
   ****************************************************************************
   */ /**
   *  This method constructs the object from DOM.  It does not keep a
   * copy of the DOM around.  Whitespace information is lost in this process.
   */
   public LocalID(Element oElem) throws URISyntaxException
   {
	   super(Tags.TAG_LOCALID);
       fromXML(oElem);
		
		// make sure that the priority (if present) is valid (vommits exception if invalid)
		String val = oElem.getAttribute(Tags.ATTR_PRIORITY);
		if (val != null && !val.equals(""))
			Integer.parseInt(val); 
   } // Constructor()

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
		
		if (priority == null) {
			
			this.removeAttribute(Tags.ATTR_PRIORITY);
		} else {
		
			this.addAttribute(Tags.ATTR_PRIORITY, priority.toString());
		}
	}
}
