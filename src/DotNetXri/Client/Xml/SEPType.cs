package org.openxri.xml;

import org.w3c.dom.Node;

public class SEPType extends SEPElement
{
	/**
	 * Creates a default <code>EppXriServiceEndpointType</code> object
	 */
	public SEPType()
	{
		super();
	}

	/**
	 * Creates an <code>EppXriServiceEndpointType</code> object with the specified fields
	 */
	public SEPType( String type, String match, Boolean select )
	{
		super(type, match, select);
	}

	/**
	 * Converts an XML element into an <code>EppXriServiceEndpointType</code> object.
	 * The caller of this method must make sure that the root node is of
	 * the EPP XRI sepTypeType.
	 *
	 * @param root root node for an <code>EppXriServiceEndpoint</code> object in
	 *             XML format
	 *
	 * @return an <code>EppXriServiceEndpointType</code> object, or null if the node is
	 *         invalid
	 */
	public static SEPElement fromXML( Node root )
	{
		SEPType sepType = new SEPType();
		sepType.setFromXML(root);
		return sepType;
	}

	/**
	 * Gets the value of this Type rule.
	 * This is an alias for the superclass' <code>getValue</code> method.
	 */
	public String getType()
	{
		return getValue();
	}
	/**
	 * Sets the value of this Type rule.
	 * This is an alias for the superclass' <code>setValue</code> method.
	 */
	public void setType( String type )
	{
		setValue(type);
	}


	public String toString()
	{
		return toString(Tags.TAG_TYPE);
	}

	public boolean match(String match){
		if(getValue() != null && (getValue().equals(match)))
				return true;
		return false;
	}
	
    public Object clone(){
    	return new SEPType(getType(), getMatch(), new Boolean(getSelect()) );
    }
}
