package org.openxri.xml;

import java.io.Serializable;
import java.util.Enumeration;
import java.util.Hashtable;

import org.apache.xerces.dom.DocumentImpl;
import org.openxri.util.DOMUtils;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;

public abstract class SimpleXMLElement implements Cloneable, Serializable {

	private String value = null;
	private String tag = null;
	protected Hashtable attributes = null;


	public SimpleXMLElement (SimpleXMLElement s) {
		value = s.value;
		tag = s.tag;
		if (s.attributes != null)
			attributes = (Hashtable)s.attributes.clone();
	}

	/**
	 * Creates a <code>SimpleXMLElement</code> object with the given <code>tagname</code>
	 * @param tagname
	 */
	public SimpleXMLElement(String tagname)
	{
		reset(tagname);
	}

	public SimpleXMLElement(String tagname, String sValue)
	{
		reset(tagname);
		this.value = sValue;
	}

	protected void reset(String tagName){
		value = null;
		attributes = null;
		tag = tagName;
	}

	public String getTagname(){
		return tag;
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
		this.value = value;
	}

	public void addAttribute(String attrName, String attrValue){
		if(attrName == null || attrValue == null) return;
		if(attributes == null)
			attributes = new Hashtable();
		attributes.put(attrName, attrValue);
	}

	public String getAttributeValue(String attrName){

		if(attrName == null || attributes == null) 
			return null;

		return (String)attributes.get(attrName);
	}

	public void removeAttribute(String attrName) {

		if (attributes == null) return;
		attributes.remove(attrName);
	}

	/**
	 * Converts the <code>EppXriServiceEndpointRule</code> object into an XML element
	 *
	 * @param doc the XML <code>Document</code> object
	 * @param tag the tag/element name for the <code>EppXriServiceEndpoint</code> object
	 *
	 * @return an <code>Element</code> object
	 */
	public Element toXML( Document doc, String tag )
	{
		Element body = doc.createElement(tag);

		if(attributes != null){
			Enumeration keys = attributes.keys();
			while(keys.hasMoreElements()){
				String attrName = (String)keys.nextElement();
				String attrValue = (String)attributes.get(attrName);
				body.setAttribute(attrName,attrValue);
			}
		}

		if( this.value != null)
		{
			body.appendChild(doc.createTextNode(this.value));
		}

		return body;
	}

	public Element toXML( Document doc)
	{
		return toXML(doc,this.tag);
	}
	/**
	 * Sets the members of this <code>EppXriServiceEndpointRule</code> object from the given XML element.
	 * The caller of this method must make sure that the root node is of
	 * the EPP XRI sepRuleType.
	 *
	 * @param root root node for an <code>EppXriServiceEndpointRule</code> object in
	 *             XML format
	 *
	 */
	public void fromXML( Node root )
	{	
		if(root.getNodeType() == Node.ELEMENT_NODE){
			this.tag = root.getLocalName();
			if (this.tag == null) this.tag = root.getNodeName();
			setValue(DOMUtils.getText(root));
			NamedNodeMap attribs = root.getAttributes();
			for(int i=0; i< attribs.getLength(); i++){
				Node attribNode = attribs.item(i);
				String attrName = attribNode.getNodeName();
				String attrValue = attribNode.getNodeValue();
				addAttribute(attrName, attrValue);
			}
		}
	}

	protected String toString( String tag )
	{
		Document doc = new DocumentImpl();
		Element elm = this.toXML(doc, tag);
		doc.appendChild(elm);
		return DOMUtils.toString(doc);
	}

	public Object clone() throws CloneNotSupportedException {

		return(super.clone());
	}

	public boolean equals(Object o) {

		SimpleXMLElement other = (SimpleXMLElement) o;

		if (other == null) return(false);
		if (other == this) return(true);

		if (this.tag == null && other.tag != null) return(false); 
		if (this.tag != null && ! this.tag.equals(other.tag)) return(false);

		if (this.value == null && other.value != null) return(false);
		if (this.value != null && ! this.value.equals(other.value)) return(false);

		if (this.attributes == null && other.attributes != null) return(false); 
		if (this.attributes != null && ! this.attributes.equals(other.attributes)) return(false);

		return(true);
	}

	public int hashCode() {

		int h = 1;

		if (this.tag != null) h *= this.tag.hashCode();
		if (this.value != null) h *= this.value.hashCode();
		if (this.attributes != null) h *= this.attributes.hashCode();

		return(h);
	}

	public String toString()
	{
		Document doc = new DocumentImpl();
		Element elm = this.toXML(doc);
		doc.appendChild(elm);
		return DOMUtils.toString(doc);
	}
}
