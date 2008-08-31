package org.openxri.xml;

import org.w3c.dom.Node;


public class SEPMediaType extends SEPElement
{

	/**
	 * Creates a default <code>EppXriServiceEndpointMediaType</code> object
	 */
	public SEPMediaType()
	{
		super();
	}

	/**
	 * Creates an <code>EppXriServiceEndpointMediaType</code> object with the specified fields
	 */
	public SEPMediaType( String mediaType, String match, Boolean select )
	{
		super(mediaType, match, select);
	}

	/**
	 * Converts an XML element into an <code>EppXriServiceEndpointMediaType</code> object.
	 * The caller of this method must make sure that the root node is of
	 * the EPP XRI sepMediaTypeType.
	 *
	 * @param root root node for an <code>EppXriServiceEndpoint</code> object in
	 *             XML format
	 *
	 * @return an <code>EppXriServiceEndpointMediaType</code> object, or null if the node is
	 *         invalid
	 */
	public static SEPMediaType fromXML( Node root )
	{
		SEPMediaType sepMType = new SEPMediaType();
		sepMType.setFromXML(root);
		return sepMType;
	}

	/**
	 * Gets the value of this MediaType rule.
	 * This is an alias for the superclass' <code>getValue</code> method.
	 */
	public String getMediaType()
	{
		return getValue();
	}

	/**
	 * Sets the value of this MediaType rule.
	 * This is an alias for the superclass' <code>setValue</code> method.
	 */
	public void setMediaType( String mediaType )
	{
		setValue(mediaType);
	}

	public String toString()
	{
		return toString(Tags.TAG_MEDIATYPE);
	}
	
    public Object clone(){
    	return new SEPMediaType(getMediaType(), getMatch(), new Boolean(getSelect()) );
    }
}