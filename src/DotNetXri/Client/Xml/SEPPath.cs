package org.openxri.xml;

import org.w3c.dom.Node;


public class SEPPath extends SEPElement
{

	/**
	 * Creates a default <code>EppXriServiceEndpointPath</code> object
	 */
	public SEPPath()
	{
		super();
	}

	/**
	 * Creates an <code>EppXriServiceEndpointPath</code> object with the specified fields
	 */
	public SEPPath( String path, String match, Boolean select )
	{
		super(path, match, select);
	}

	/**
	 * Converts an XML element into an <code>EppXriServiceEndpointPath</code> object.
	 * The caller of this method must make sure that the root node is of
	 * the EPP XRI sepTypeType.
	 *
	 * @param root root node for an <code>EppXriServiceEndpoint</code> object in
	 *             XML format
	 *
	 * @return an <code>EppXriServiceEndpointPath</code> object, or null if the node is
	 *         invalid
	 */
	public static SEPPath fromXML( Node root )
	{
		SEPPath sepPath = new SEPPath();
		sepPath.setFromXML(root);
		return sepPath;
	}

	/**
	 * Gets the value of this Path rule.
	 * This is an alias for the superclass' <code>getValue</code> method.
	 */
	public String getPath()
	{
		return getValue();
	}

	/**
	 * Sets the value of this Path rule.
	 * This is an alias for the superclass' <code>setValue</code> method.
	 */
	public void setPath( String path )
	{
		setValue(path);
	}


	public String toString()
	{
		return toString(Tags.TAG_PATH);
	}
	
    public Object clone(){
    	return new SEPPath(getPath(), getMatch(), new Boolean(getSelect()) );
    }
}