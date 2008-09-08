namespace DotNetXri.Client.Xml {

public class ProviderID : SimpleXMLElement {

	public ProviderID(ProviderID p)
		: base(p) {
	}

	public ProviderID(string provideridString)
		: base(Tags.TAG_PROVIDERID) {
		setValue(provideridString);
	}

	public ProviderID()
		: base(Tags.TAG_PROVIDERID) {
	}
}
}