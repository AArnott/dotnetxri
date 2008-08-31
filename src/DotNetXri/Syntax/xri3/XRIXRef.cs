namespace DotNetXri.Syntax.Xri3 {
	public interface XRIXRef : XRISyntaxComponent {

		public bool hasXRIReference();
		public bool hasIRI();

		public XRIReference getXRIReference();
		public String getIRI();
	}
}