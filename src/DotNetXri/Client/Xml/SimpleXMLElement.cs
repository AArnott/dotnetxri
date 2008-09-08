namespace DotNetXri.Client.Xml {

using java.io.Serializable;
using java.util.Enumeration;
using java.util.Hashtable;

using org.apache.xerces.dom.XmlDocument;
using org.openxri.util.DOMUtils;
using org.w3c.dom.XmlDocument;
using org.w3c.dom.XmlElement;
using org.w3c.dom.NamedNodeMap;
using org.w3c.dom.Node;

public abstract class SimpleXMLElement : Cloneable, Serializable {

	private string value = null;
	private string tag = null;
	protected Hashtable attributes = null;


	public SimpleXMLElement (SimpleXMLElement s) {
		value = s.value;
		tag = s.tag;
		if (s.attributes != null)
			attributes = (Hashtable)s.attributes.clone();
	}

	/**
	* Creates a <code>SimpleXMLElement</code> obj with the given <code>tagname</code>
	* @param tagname
	*/
	public SimpleXMLElement(string tagname)
	{
		reset(tagname);
	}

	public SimpleXMLElement(string tagname, string sValue)
	{
		reset(tagname);
		this.value = sValue;
	}

	protected void reset(string tagName){
		value = null;
		attributes = null;
		tag = tagName;
	}

	public string getTagname(){
		return tag;
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
		this.value = value;
	}

	public void addAttribute(string attrName, string attrValue){
		if(attrName == null || attrValue == null) return;
		if(attributes == null)
			attributes = new Hashtable();
		attributes.put(attrName, attrValue);
	}

	public string getAttributeValue(string attrName){

		if(attrName == null || attributes == null) 
			return null;

		return (string)attributes.get(attrName);
	}

	public void removeAttribute(string attrName) {

		if (attributes == null) return;
		attributes.remove(attrName);
	}

	/**
	* Converts the <code>EppXriServiceEndpointRule</code> obj into an XML element
	*
	* @param doc the XML <code>XmlDocument</code> obj
	* @param tag the tag/element name for the <code>EppXriServiceEndpoint</code> obj
	*
	* @return an <code>XmlElement</code> obj
	*/
	public XmlElement toXML( XmlDocument doc, string tag )
	{
		XmlElement body = doc.CreateElement(tag);

		if(attributes != null){
			Enumeration keys = attributes.keys();
			while(keys.hasMoreElements()){
				string attrName = (string)keys.nextElement();
				string attrValue = (string)attributes.get(attrName);
				body.SetAttribute(attrName,attrValue);
			}
		}

		if( this.value != null)
		{
			body.AppendChild(doc.CreateTextNode(this.value));
		}

		return body;
	}

	public XmlElement toXML( XmlDocument doc)
	{
		return toXML(doc,this.tag);
	}
	/**
	* Sets the members of this <code>EppXriServiceEndpointRule</code> obj from the given XML element.
	* The caller of this method must make sure that the root node is of
	* the EPP XRI sepRuleType.
	*
	* @param root root node for an <code>EppXriServiceEndpointRule</code> obj in
	*             XML format
	*
	*/
	public void fromXML( Node root )
	{	
		if(root.getNodeType() == Node.ELEMENT_NODE){
			this.tag = root.LocalName;
			if (this.tag == null) this.tag = root.getNodeName();
			setValue(DOMUtils.getText(root));
			NamedNodeMap attribs = root.getAttributes();
			for(int i=0; i< attribs.getLength(); i++){
				Node attribNode = attribs.item(i);
				string attrName = attribNode.getNodeName();
				string attrValue = attribNode.getNodeValue();
				addAttribute(attrName, attrValue);
			}
		}
	}

	protected string ToString( string tag )
	{
		XmlDocument doc = new XmlDocument();
		XmlElement elm = this.toXML(doc, tag);
		doc.AppendChild(elm);
		return DOMUtils.ToString(doc);
	}

	public Object clone() //throws CloneNotSupportedException {

		return(base.clone());
	}

	public bool Equals(Object o) {

		SimpleXMLElement other = (SimpleXMLElement) o;

		if (other == null) return(false);
		if (other == this) return(true);

		if (this.tag == null && other.tag != null) return(false); 
		if (this.tag != null && ! this.tag.Equals(other.tag)) return(false);

		if (this.value == null && other.value != null) return(false);
		if (this.value != null && ! this.value.Equals(other.value)) return(false);

		if (this.attributes == null && other.attributes != null) return(false); 
		if (this.attributes != null && ! this.attributes.Equals(other.attributes)) return(false);

		return(true);
	}

	public int hashCode() {

		int h = 1;

		if (this.tag != null) h *= this.tag.hashCode();
		if (this.value != null) h *= this.value.hashCode();
		if (this.attributes != null) h *= this.attributes.hashCode();

		return(h);
	}

	public override string ToString()
	{
		XmlDocument doc = new XmlDocument();
		XmlElement elm = this.toXML(doc);
		doc.AppendChild(elm);
		return DOMUtils.ToString(doc);
	}
}
}