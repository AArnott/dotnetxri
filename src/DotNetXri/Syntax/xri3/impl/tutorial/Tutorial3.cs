package org.openxri.xri3.impl.tutorial;

import org.openxri.xri3.XRI;
import org.openxri.xri3.XRIAuthority;
import org.openxri.xri3.XRIPath;
import org.openxri.xri3.impl.XRI3;

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

		System.out.println("Checking XDI address " + xri.toString());
		
		System.out.println("XDI Subject: " + xriAuthority.toString());
		System.out.println("XDI Predicate: " + xriPath.getSegment(0).toString());
		System.out.println("XDI Reference: " + xriPath.getSegment(1).toString());
	}
}
