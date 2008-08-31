package org.openxri.xri3.impl.tutorial;

import org.openxri.xri3.XRI;
import org.openxri.xri3.XRIAuthority;
import org.openxri.xri3.XRIPath;
import org.openxri.xri3.XRISubSegment;
import org.openxri.xri3.impl.XRI3;

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

		System.out.println("Resolving XRI " + xri.toString());
		System.out.println("Listing " + xriAuthority.getNumSubSegments() + " subsegments...");

		for (int i=0; i<xriAuthority.getNumSubSegments(); i++) {

			XRISubSegment subSegment = xriAuthority.getSubSegment(i);
			System.out.println("Subsegment #" + i + ": " + subSegment.toString());
			System.out.println("  Global: " + subSegment.isGlobal());
			System.out.println("  Local: " + subSegment.isLocal());
		}

		System.out.println("Path: " + xriPath.toString());
	}
}
