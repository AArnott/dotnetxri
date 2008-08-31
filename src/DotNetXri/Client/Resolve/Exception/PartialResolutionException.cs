/**
 * 
 */
package org.openxri.resolve.exception;

import org.openxri.xml.XRD;
import org.openxri.xml.XRDS;

/**
 * This exception is thrown by the top level XRI resolution APIs to indicate
 * that resolution was not completed successfully.
 * @author wtan
 *
 */
public class PartialResolutionException :XRIResolutionException {
	private XRDS xrds = null;
	
	/**
	 * Constructs an obj of this class. 
	 * @param msg Error message
	 * @param xrds The partial resolution results
	 */
	public PartialResolutionException(XRDS xrds) {
		super("Resolution did not complete successfully.");
		this.xrds = xrds;
	}

	public PartialResolutionException(XRD xrd)
	{
		super(xrd.getStatus().getText());
		this.xrds = new XRDS();
		this.xrds.add(xrd);
	}
	
	/**
	 * Gets the partial resolution result.
	 * @return XRDS of the partial resolution result.
	 */
	public XRDS getPartialXRDS() {
		return xrds;
	}
}
