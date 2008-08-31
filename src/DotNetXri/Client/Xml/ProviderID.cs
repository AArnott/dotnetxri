package org.openxri.xml;

public class ProviderID :SimpleXMLElement{
	
	public ProviderID(ProviderID p) : base(p) {
	}
	
	public ProviderID(String provideridString) : base(Tags.TAG_PROVIDERID) {
		setValue(provideridString);
	}

	public ProviderID() : base(Tags.TAG_PROVIDERID) {
	}
}
