/**
 * 
 */
package org.openxri.resolve.exception;


/**
 * @author wtan
 *
 */
public class LimitExceededException :XRIResolutionException {

	/**
	 * @param msg
	 */
	public LimitExceededException(String msg) {
		super(msg);
	}

}
