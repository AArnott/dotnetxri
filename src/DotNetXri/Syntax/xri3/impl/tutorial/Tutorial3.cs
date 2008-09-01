package org.openxri.xri3.impl.tutorial;

using org.openxri.xri3.XRI;
using org.openxri.xri3.XRIAuthority;
using org.openxri.xri3.XRIPath;
using org.openxri.xri3.impl.XRI3;

public class Tutorial3 {

	public static void main(String[] args) {

		// Something like this may happen when working with XDI.
		// We use the following XRI address: +name+first/$is/+!3
		// We want to know the following
		// - XDI subject
		// - XDI predicate
		// - XDI reference

		XRI xri = new XRI3("+name+first/$is/+!3");
		XRIAuthority xriAuthority = xri.getAuthority();
		XRIPath xriPath = xri.getPath();

		Logger.Info("Checking XDI address " + xri.toString());
		
		Logger.Info("XDI Subject: " + xriAuthority.toString());
		Logger.Info("XDI Predicate: " + xriPath.getSegment(0).toString());
		Logger.Info("XDI Reference: " + xriPath.getSegment(1).toString());
	}
}
