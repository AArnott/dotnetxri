package org.openxri.xml;

using java.io.Serializable;

using org.apache.xerces.dom.DocumentImpl;
using org.openxri.util.DOMUtils;
using org.w3c.dom.Document;
using org.w3c.dom.Element;
using org.w3c.dom.Node;

public abstract class SEPElement : Cloneable, Serializable {

    protected static org.apache.commons.logging.Log soLog =
        org.apache.commons.logging.LogFactory.getLog(
        		XRD.class.getName());

	/**
	 * Default value of the match attribute if it was omitted or its
	 * value is null. This is an alias for <code>MATCH_ATTR_CONTENT</code>
	 * as defined in xri-resolution-v2.0-wd-10-ed-08.
	 */  
	public const String MATCH_ATTR_DEFAULT  = "default";
	public const String MATCH_ATTR_ANY      = "any";
	public const String MATCH_ATTR_NON_NULL = "non-null";
	public const String MATCH_ATTR_NULL     = "null";
	
	/**
	 * @deprecated
	 */
	public const String MATCH_ATTR_CONTENT  = "content";

	/**
	 * @deprecated
	 */
	public const String MATCH_ATTR_NONE     = "none";


	public const String  SELECT_ATTR_TRUE  = "true";
	public const String  SELECT_ATTR_FALSE = "false";

	/**
	 * Default value of the select attribute is FALSE if it was omitted
	 * in the parent element.
	 */
	public const String  DEFAULT_SELECT_ATTR = SELECT_ATTR_FALSE;
	public const bool DEFAULT_SELECT_ATTR_BOOL = false;

	private String  match; // null or one of the MATCH_ATTR_* constants
	private Boolean select;
	private String  value; // represents the value of this rule

	/**
	 * Creates a default <code>SEPElement</code> obj
	 */
	public SEPElement()
	{
		this("", null, null);
	}

	/**
	 * Creates a  <code>SEPElement with required attributes</code> obj with the given value
	 */
	public SEPElement( String value, String match, Boolean select )
	{
		setMatch(match);
		setSelect(select);
		setValue(value);
	}

	/**
	 * Gets the "match" attribute of this Type/MediaType/Path rule
	 */
	public String getMatch()
	{
		return this.match;
	}

	/**
	 * Sets the "match" attribute of this Type/MediaType/Path rule
	 */
	public void setMatch( String match )
	{
		this.match = match;
	}

	/**
	 * Gets the "select" attribute of this Type/MediaType/Path rule
	 */
	public bool getSelect()
	{
		if ( this.select != null )
		{
			return this.select.booleanValue();
		}
		else
		{
			return DEFAULT_SELECT_ATTR_BOOL;
		}
	}

	/**
	 * Sets the "select" attribute of this Type/MediaType/Path rule
	 */
	public void setSelect( bool select )
	{
		this.select = Boolean.valueOf(select);
	}

	/**
	 * Sets the "select" attribute of this Type/MediaType/Path rule
	 */
	public void setSelect( Boolean select )
	{
		this.select = select;
	}

	/**
	 * Sets the "select" attribute of this Type/MediaType/Path rule.
	 * Interprets "true" (any case) or "1" as TRUE. Any other value
	 * is considered FALSE.
	 */
	public void setSelect( String select )
	{
		if (select == null)
			this.select = null;
		else if (select.equalsIgnoreCase("true") || select.equals("1"))
			this.select = Boolean.TRUE;
		else
			this.select = Boolean.FALSE;
	}

	/**
	 * Gets the value of this Type/MediaType/Path rule
	 */
	public String getValue()
	{
		return this.value;
	}

	/**
	 * Sets the value of this Type/MediaType/Path rule
	 */
	public void setValue( String value )
	{
		this.value = (value == null)? "" : value;
	}

	public Element toXML( Document doc, String tag )
	{
		Element body = doc.createElement(tag);

		if( this.match != null ) {
			body.setAttribute("match", this.match);
		}

		if( this.select != null ) {
			body.setAttribute("select", this.select.toString());
		}

		if( this.value != null) {
			body.appendChild(doc.createTextNode(this.value));
		}

		return body;
	}

	public void setFromXML( Node root )
	{
		Element el = (Element)root;
		if (el.hasAttribute("match")) {
			setMatch(el.getAttribute("match").trim());
		}

		if (el.hasAttribute("select")) {
			setSelect(el.getAttribute("select").trim());
		}

		this.setValue(DOMUtils.getText(root));
	}

    protected String toString( String tag )
    {
            Document doc = new DocumentImpl();
            Element elm = this.toXML(doc, tag);
            doc.appendChild(elm);
            return DOMUtils.toString(doc);
    }

    public Object clone()throws CloneNotSupportedException {
    	return base.clone();
    }

	public bool equals(Object o) {

		SEPElement other = (SEPElement) o;

		if (other == null) return(false);
		if (other == this) return(true);

		if (this.match == null && other.match != null) return(false);
		if (this.match != null && ! (this.match.equals(other.match))) return(false);

		if (this.select == null && other.select != null) return(false);
		if (this.select != null && ! (this.select.equals(other.select))) return(false);

		if (this.value == null && other.value != null) return(false);
		if (this.value != null && ! (this.value.equals(other.value))) return(false);
		
		return(true);
	}

	public int hashCode() {
		
		int h = 1;
		
		if (this.match != null) h *= this.match.hashCode();
		if (this.select != null) h *= this.select.hashCode();
		if (this.value != null) h *= this.value.hashCode();
		
		return(h);
	}
}
