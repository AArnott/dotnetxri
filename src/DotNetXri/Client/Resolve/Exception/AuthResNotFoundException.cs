/**
 * 
 */
package org.openxri.resolve.exception;


/**
 * This exception is raised during the authority resolution process when
 * neither an authority resolution service nor Ref is found.
 *  
 * @author wtan
 *
 */
public class AuthResNotFoundException :XRIResolutionException {

	/**
	 * @param msg
	 */
	public AuthResNotFoundException(String msg) : base(msg) {
	}
	
}
