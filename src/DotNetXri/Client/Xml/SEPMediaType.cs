package org.openxri.xml;

using org.w3c.dom.Node;


public class SEPMediaType :SEPElement
{

	/**
	 * Creates a default <code>EppXriServiceEndpointMediaType</code> obj
	 */
	public SEPMediaType()
	{
		
	}

	/**
	 * Creates an <code>EppXriServiceEndpointMediaType</code> obj with the specified fields
	 */
	public SEPMediaType( String mediaType, String match, Boolean select )
	: base(mediaType, match, select) {
	}

	/**
	 * Converts an XML element into an <code>EppXriServiceEndpointMediaType</code> obj.
	 * The caller of this method must make sure that the root node is of
	 * the EPP XRI sepMediaTypeType.
	 *
	 * @param root root node for an <code>EppXriServiceEndpoint</code> obj in
	 *             XML format
	 *
	 * @return an <code>EppXriServiceEndpointMediaType</code> obj, or null if the node is
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