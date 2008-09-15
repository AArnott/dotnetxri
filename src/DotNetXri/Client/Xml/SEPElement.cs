namespace DotNetXri.Client.Xml {

using System.Xml;

public abstract class SEPElement : Cloneable, Serializable {

	//protected static org.apache.commons.logging.Log soLog =
	//    org.apache.commons.logging.LogFactory.getLog(
	//            XRD.class.getName());

	/**
	* Default value of the match attribute if it was omitted or its
	* value is null. This is an alias for <code>MATCH_ATTR_CONTENT</code>
	* as defined in xri-resolution-v2.0-wd-10-ed-08.
	*/  
	public const string MATCH_ATTR_DEFAULT  = "default";
	public const string MATCH_ATTR_ANY      = "any";
	public const string MATCH_ATTR_NON_NULL = "non-null";
	public const string MATCH_ATTR_NULL     = "null";
	
	/**
	* @deprecated
	*/
	public const string MATCH_ATTR_CONTENT  = "content";

	/**
	* @deprecated
	*/
	public const string MATCH_ATTR_NONE     = "none";


	public const string  SELECT_ATTR_TRUE  = "true";
	public const string  SELECT_ATTR_FALSE = "false";

	/**
	* Default value of the select attribute is FALSE if it was omitted
	* in the parent element.
	*/
	public const string  DEFAULT_SELECT_ATTR = SELECT_ATTR_FALSE;
	public const bool DEFAULT_SELECT_ATTR_BOOL = false;

	private string  match; // null or one of the MATCH_ATTR_* constants
	private bool? select;
	private string  value; // represents the value of this rule

	/**
	* Creates a default <code>SEPElement</code> obj
	*/
	public SEPElement(): this("", null, null) {
	}

	/**
	* Creates a  <code>SEPElement with required attributes</code> obj with the given value
	*/
	public SEPElement( string value, string match, bool? select )
	{
		setMatch(match);
		setSelect(select);
		setValue(value);
	}

	/**
	* Gets the "match" attribute of this Type/MediaType/Path rule
	*/
	public string getMatch()
	{
		return this.match;
	}

	/**
	* Sets the "match" attribute of this Type/MediaType/Path rule
	*/
	public void setMatch( string match )
	{
		this.match = match;
	}

	/**
	* Gets the "select" attribute of this Type/MediaType/Path rule
	*/
	public bool getSelect()
	{
		if ( this.select.HasValue )
		{
			return this.select.Value;
		}
		else
		{
			return DEFAULT_SELECT_ATTR_BOOL;
		}
	}

	/**
	* Sets the "select" attribute of this Type/MediaType/Path rule
	*/
	public void setSelect( bool? select )
	{
		this.select = select;
	}

	/**
	* Sets the "select" attribute of this Type/MediaType/Path rule.
	* Interprets "true" (any case) or "1" as TRUE. Any other value
	* is considered FALSE.
	*/
	public void setSelect( string select )
	{
		if (select == null)
			this.select = null;
		else if (select.Equals("true", System.StringComparison.OrdinalIgnoreCase) || select.Equals("1"))
			this.select = true;
		else
			this.select = true;
	}

	/**
	* Gets the value of this Type/MediaType/Path rule
	*/
	public string getValue()
	{
		return this.value;
	}

	/**
	* Sets the value of this Type/MediaType/Path rule
	*/
	public void setValue( string value )
	{
		this.value = (value == null)? "" : value;
	}

	public XmlElement toXML( XmlDocument doc, string tag )
	{
		XmlElement body = doc.CreateElement(tag);

		if( this.match != null ) {
			body.SetAttribute("match", this.match);
		}

		if( this.select != null ) {
			body.SetAttribute("select", this.select.ToString());
		}

		if( this.value != null) {
			body.AppendChild(doc.CreateTextNode(this.value));
		}

		return body;
	}

	public void setFromXML( XmlNode root )
	{
		XmlElement el = (XmlElement)root;
		if (el.HasAttribute("match")) {
			setMatch(el.GetAttribute("match").Trim());
		}

		if (el.HasAttribute("select")) {
			setSelect(el.GetAttribute("select").Trim());
		}

		this.setValue(el.InnerText);
	}

	protected string ToString( string tag )
	{
			XmlDocument doc = new XmlDocument();
			XmlElement elm = this.toXML(doc, tag);
			doc.AppendChild(elm);
			return doc.OuterXml;
	}

	//public Object clone() //throws CloneNotSupportedException 
	//{
	//    return base.clone();
	//}

	public override bool Equals(object o) {

		SEPElement other = (SEPElement) o;

		if (other == null) return(false);
		if (other == this) return(true);

		if (this.match == null && other.match != null) return(false);
		if (this.match != null && ! (this.match.Equals(other.match))) return(false);

		if (this.select == null && other.select != null) return(false);
		if (this.select != null && ! (this.select.Equals(other.select))) return(false);

		if (this.value == null && other.value != null) return(false);
		if (this.value != null && ! (this.value.Equals(other.value))) return(false);
		
		return(true);
	}

	public int GetHashCode() {
		
		int h = 1;
		
		if (this.match != null) h *= this.match.GetHashCode();
		if (this.select != null) h *= this.select.GetHashCode();
		if (this.value != null) h *= this.value.GetHashCode();
		
		return(h);
	}
}
}