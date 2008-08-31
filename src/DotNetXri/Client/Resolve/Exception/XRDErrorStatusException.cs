/**
 * 
 */
package org.openxri.resolve.exception;

import org.openxri.xml.XRD;

/**
 * This exception is thrown to encapsulate an XRD node with a non-100 status code.
 * @author wtan
 *
 */
public class XRDErrorStatusException :XRIResolutionException {

	protected XRD xrd = null;
	
	/**
	 * @param xrd
	 */
	public XRDErrorStatusException(XRD xrd) : base("XRD contains a non-SUCCESS status code") {
		this.xrd = xrd;
	}

	/**
	 * @return Returns the xrd.
	 */
	public XRD getXRD() {
		return xrd;
	}

}
