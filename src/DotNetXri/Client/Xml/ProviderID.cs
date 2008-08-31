package org.openxri.xml;

public class ProviderID extends SimpleXMLElement{
	
	public ProviderID(ProviderID p) {
		super(p);
	}
	
	public ProviderID(String provideridString) {
		super(Tags.TAG_PROVIDERID);
		setValue(provideridString);
	}

	public ProviderID() {
		super(Tags.TAG_PROVIDERID);
	}
}
