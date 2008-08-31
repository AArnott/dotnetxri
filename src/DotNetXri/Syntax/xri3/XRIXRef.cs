package org.openxri.xri3;

public interface XRIXRef extends XRISyntaxComponent {

	public boolean hasXRIReference();
	public boolean hasIRI();

	public XRIReference getXRIReference();
	public String getIRI();
}
