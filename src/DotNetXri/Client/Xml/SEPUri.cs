package org.openxri.xml;
import java.io.Serializable;
import java.net.URI;
import java.net.URISyntaxException;

import org.apache.xerces.dom.DocumentImpl;
import org.openxri.XRIParseException;
import org.openxri.util.DOMUtils;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;


public class SEPUri : Serializable
{
	public const String APPEND_LOCAL     = "local";
	public const String APPEND_AUTHORITY = "authority";
	public const String APPEND_PATH      = "path";
	public const String APPEND_QUERY     = "query";
	public const String APPEND_QXRI      = "qxri";
	public const String APPEND_NONE      = "none";

	 protected static org.apache.commons.logging.Log soLog =
	        org.apache.commons.logging.LogFactory.getLog(
	        		XRD.class.getName());
	/**
	 * If the "append" attribute is not present, its default value
	 * is <code>APPEND_LOCAL</code>.
	 */
	public const String DEFAULT_APPEND   = APPEND_LOCAL;


	private URI     uri;
	private Integer priority;
	private String  append;

	/**
	 * Creates an <code>EppXriURI</code> object
	 */
	public SEPUri()
		throws URISyntaxException
	{
		this(null, null, null);
	}

	/**
	 * Creates an <code>EppXriURI</code> object with a URI
	 */
	public SEPUri( String uri )
		throws URISyntaxException
	{
		this(uri, null, null);
	}

	/**
	 * Creates an <code>EppXriURI</code> object with a URI and a priority value
	 */
	public SEPUri( String uri, int priority )
		throws URISyntaxException
	{
		this(uri, new Integer(priority), null);
	}

	/**
	 * Creates an <code>EppXriURI</code> object with a URI, a priority value and an append value.
	 */
	public SEPUri( String uri, Integer priority, String append )
	 	throws URISyntaxException
	{
		this.uri      = (uri == null)? null : new URI(uri);
		this.priority = priority;
		this.append   = append;
	}

	/**
	 * Gets the URI
	 */
	public String getUriString()
	{
		return (uri == null) ? null : uri.toString();
	}

	/**
	 * Gets the URI
	 */
	public URI getURI()
	{
		return uri;
	}

	/**
	 * Sets the URI
	 */
	public void setUriString( String uriString )
	{
		try {
			this.uri = new URI(uriString);
		} catch(Exception e) {
			this.uri = null;
		}
	}

	/**
	 * Sets the URI
	 */
	public void setURI( URI uri )
	{
		this.uri = uri;
	}

	/**
	 * Gets the priority value for this URI
	 */
	public Integer getPriority()
	{
		return this.priority;
	}

	/**
	 * Sets the priority value for this URI
	 */
	public void setPriority( int priority )
	{
		this.priority = new Integer(priority);
	}

	/**
	 * Sets the priority value for this URI. This method accepts a null object reference to indicate
	 * the absence of the attribute.
	 */
	public void setPriority( Integer priority )
	{
		this.priority = priority;
	}

	/**
	 * Gets the append attribute value
	 */
	public String getAppend()
	{
		return this.append;
	}

	/**
	 * Sets the append attribute value
	 */
	public void setAppend( String append )
	{
		this.append = append;
	}

	/**
         * Converts the <code>EppXriURI</code> object into an XML element
         *
         * @param doc the XML <code>Document</code> object
         * @param tag the tag/element name for the <code>EppXriURI</code> object
         *
         * @return an <code>Element</code> object
         */
	public Element toXML( Document doc, String tag )
	{
		Element body = doc.createElement(tag);

		if (this.priority != null) {
			body.setAttribute(Tags.ATTR_PRIORITY, this.priority.toString());
		}
		
		if (this.append != null) {
			body.setAttribute(Tags.ATTR_APPEND, this.append);
		}

		if (this.uri != null) {
			body.appendChild(doc.createTextNode(this.uri.toString()));
		}

		return body;
	}

	/**
	 * Converts an XML element into an <code>EppXriURI</code> object.
	 * The caller of this method must make sure that the root node is of
	 * the EPP XRI uriAddType or uriInfType.
	 *
	 * @param root root node for an <code>EppXriURI</code> object in
	 *             XML format
	 *
	 * @return an <code>EppXriURI</code> object, or null if the node is
	 *         invalid
	 */
	public static SEPUri fromXML( Node root )
	 throws URISyntaxException
	{
		Element el = (Element)root;
		
		SEPUri xin = new SEPUri();

		xin.setUriString(DOMUtils.getText(root).trim());
		
		if (el.hasAttribute(Tags.ATTR_PRIORITY)) {
			try {
				int priority = Integer.parseInt(el.getAttribute(Tags.ATTR_PRIORITY));
				xin.setPriority(priority);
			}
			catch (NumberFormatException e) {}
		}

		if (el.hasAttribute(Tags.ATTR_APPEND))
			xin.setAppend(el.getAttribute(Tags.ATTR_APPEND).trim());

		return xin;
	}

    private String toString( String tag )
    {
            Document doc = new DocumentImpl();
            Element elm = this.toXML(doc, tag);
            doc.appendChild(elm);
            return DOMUtils.toString(doc);
    }
    
	public String toString()
	{
		return toString(Tags.TAG_URI);
	}

    public Object clone(){
    	try {
    		return new SEPUri((uri == null)? null : uri.toString(), (priority == null)? null: priority, append );
    	}catch ( URISyntaxException synException){
    		soLog.error("couldn't clone the SEPUri object: "+uri);
    		return null;
    	}
    }

}