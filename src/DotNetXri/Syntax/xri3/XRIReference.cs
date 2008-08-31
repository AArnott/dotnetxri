package org.openxri.xri3;

public interface XRIReference extends XRISyntaxComponent {

	public boolean hasScheme();
	public boolean hasAuthority();
	public boolean hasPath();
	public boolean hasQuery();
	public boolean hasFragment();

	public String getScheme();
	public XRIAuthority getAuthority();
	public XRIPath getPath();
	public XRIQuery getQuery();
	public XRIFragment getFragment();

	public boolean isValidXRI();
	public XRI toXRI();
}
