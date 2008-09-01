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

		System.Console.WriteLine("Resolving XRI " + xri.toString());
		System.Console.WriteLine("Listing " + xriAuthority.getNumSubSegments() + " subsegments...");

		for (int i=0; i<xriAuthority.getNumSubSegments(); i++) {

			XRISubSegment subSegment = xriAuthority.getSubSegment(i);
			System.Console.WriteLine("Subsegment #" + i + ": " + subSegment.toString());
			System.Console.WriteLine("  Global: " + subSegment.isGlobal());
			System.Console.WriteLine("  Local: " + subSegment.isLocal());
		}

		System.Console.WriteLine("Path: " + xriPath.toString());
	}
}
