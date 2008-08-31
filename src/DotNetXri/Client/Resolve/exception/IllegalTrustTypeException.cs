/**
 * 
 */
package org.openxri.resolve.exception;


/**
 * @author wtan
 *
 */
public class IllegalTrustTypeException extends XRIResolutionException {
	private String type = null;

	/**
	 * @param type
	 * @param sMsg
	 */
	public IllegalTrustTypeException(String type, String sMsg) {
		super("Illegal Trust Type (" + type + ") - " + sMsg);
		this.type = type;
	}
	
	/**
	 * @param type
	 */
	public IllegalTrustTypeException(String type) {
		super("Illegal Trust Type (" + type + ")");
		this.type = type;
	}

	/**
	 * @return Returns the type.
	 */
	public String getType() {
		return type;
	}

	/**
	 * @param type The type to set.
	 */
	public void setType(String type) {
		this.type = type;
	}
	

	public String toString() {
		return super.toString();

	}
}
