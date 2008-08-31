/**
 * 
 */
package org.openxri.resolve.exception;


/**
 * @author wtan
 *
 */
public class RefNotFollowedException :XRIResolutionException {
	protected String unresolved = null;

	/**
	 * @param unresolved
	 * @param sMsg
	 */
	public RefNotFollowedException(String unresolved, String sMsg) {
		super(sMsg + " (" + unresolved + ")");
		this.unresolved = unresolved;
	}
	
	/**
	 * @param unresolved
	 */
	public RefNotFollowedException(String unresolved) {
		super("Ref not followed while unresolved segment exists: " + unresolved);
		this.unresolved = unresolved;
	}
}
