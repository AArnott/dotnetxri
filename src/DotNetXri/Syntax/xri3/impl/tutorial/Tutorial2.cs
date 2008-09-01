package org.openxri.xri3.impl.tutorial;

using org.openxri.xri3.XRI;
using org.openxri.xri3.XRIReference;
using org.openxri.xri3.impl.XRI3;
using org.openxri.xri3.impl.XRI3Reference;

public class Tutorial2 {

	public static void main(String[] args) {

		// The library can also construct new XRIs or XRI components.
		// For example, if we have an XRI +name, and a relative XRI reference +first,
		// we can construct a new XRI +name+first

		XRI xri = new XRI3("+name");
		XRIReference xriReference = new XRI3Reference("+first");

		Logger.Info("Got XRI " + xri.toString());
		Logger.Info("Got XRI reference " + xriReference.toString());

		XRI xriNew = new XRI3(xri, xriReference);
		
		Logger.Info("Constructed new XRI " + xriNew.toString());
	}
}
