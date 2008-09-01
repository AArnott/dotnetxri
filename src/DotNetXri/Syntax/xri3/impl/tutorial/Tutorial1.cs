package org.openxri.xri3.impl.tutorial;

using org.openxri.xri3.XRI;
using org.openxri.xri3.XRIAuthority;
using org.openxri.xri3.XRIPath;
using org.openxri.xri3.XRISubSegment;
using org.openxri.xri3.impl.XRI3;

public class Tutorial1 {

	public static void main(String[] args) {

		// Let's assume we are a resolver that got a simple XRI to resolve.
		// The XRI is: =parity*markus/+contact
		// The resolver needs to know the following:
		// - list of subsegments
		// - path

		XRI xri = new XRI3("=parity*markus/+contact");
		XRIAuthority xriAuthority = xri.getAuthority();
		XRIPath xriPath = xri.getPath();

		Logger.Info("Resolving XRI " + xri.toString());
		Logger.Info("Listing " + xriAuthority.getNumSubSegments() + " subsegments...");

		for (int i=0; i<xriAuthority.getNumSubSegments(); i++) {

			XRISubSegment subSegment = xriAuthority.getSubSegment(i);
			Logger.Info("Subsegment #" + i + ": " + subSegment.toString());
			Logger.Info("  Global: " + subSegment.isGlobal());
			Logger.Info("  Local: " + subSegment.isLocal());
		}

		Logger.Info("Path: " + xriPath.toString());
	}
}
