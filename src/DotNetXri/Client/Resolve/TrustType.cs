/**
 * 
 */
package org.openxri.resolve;

import java.io.Serializable;

import org.openxri.resolve.exception.IllegalTrustTypeException;

/**
 * @author wtan
 *
 */
public class TrustType implements Serializable {
	public static final String TRUST_NONE       = "none";
	public static final String TRUST_SAML       = "saml";
	public static final String TRUST_HTTPS      = "https";
	public static final String TRUST_SAML_HTTPS = "saml+https";
	public static final String TRUST            = "trust";
	
	protected String type = TRUST_NONE;
	
	/**
	 * Constructor. Creates a TrustType
	 * @param type
	 */
	public TrustType() 
	{
	}

	/**
	 * Constructor. Creates a TrustType
	 * @param type
	 */
	public TrustType(String type)
		throws IllegalTrustTypeException
	{
		setType(type);
	}

	/**
	 * @return Returns the type as a String.
	 */
	public String getType() {
		return type;
	}

	/**
	 * @return Returns a String representation of the parameter name value pair.
	 */
	public String getParameterPair() {
		String https = (type.equals(TRUST_HTTPS) || type.equals(TRUST_SAML_HTTPS))? "true" : "false";
		String saml  = (type.equals(TRUST_SAML) || type.equals(TRUST_SAML_HTTPS))? "true" : "false";
		return MimeType.PARAM_HTTPS + "=" + https + ";" + MimeType.PARAM_SAML + "=" + saml;
	}
	
	public void setParameterPair(boolean isHttps, boolean isSaml) {
		if (isHttps) {
			if (isSaml)
				type = TRUST_SAML_HTTPS;
			else
				type = TRUST_HTTPS;
		}
		else {
			if (isSaml)
				type = TRUST_SAML;
			else
				type = TRUST_NONE;
		}
	}

	/**
	 * @param type The type to set.
	 */
	public void setType(String type) 
		throws IllegalTrustTypeException
	{
		type = type.toLowerCase();
		if ((!type.equals(TRUST_NONE))
				&& !type.equals(TRUST_SAML)
				&& !type.equals(TRUST_HTTPS)
				&& !type.equals(TRUST_SAML_HTTPS)) {
			throw new IllegalTrustTypeException(type);
		}
		
		this.type = type;
	}

	/**
	 * @return Returns <code>true</code> if the type is <code>https</code> or <code>saml+https</code>, <code>false</code> otherwise. 
	 */
	public boolean isHTTPS()
	{
		return (type.equals(TRUST_HTTPS) || type.equals(TRUST_SAML_HTTPS));
	}

	/**
	 * @return Returns <code>true</code> if the type is <code>saml</code> or <code>saml+https</code>, <code>false</code> otherwise.
	 */
	public boolean isSAML()
	{
		return (type.equals(TRUST_SAML) || type.equals(TRUST_SAML_HTTPS));
	}
	
	/**
	 * Compares with a String representation of the trust type.
	 * @param trustType
	 */
	public boolean equals(String trustType) {
		return this.type.equalsIgnoreCase(trustType);
	}
	
	public String toString() {
		return getType();
	}
}
