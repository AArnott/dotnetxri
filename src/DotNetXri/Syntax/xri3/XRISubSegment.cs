namespace DotNetXri.Syntax.Xri3 {
	public interface XRISubSegment : XRISyntaxComponent {

		public bool hasGCS();
		public bool hasLCS();
		public bool hasLiteral();
		public bool hasXRef();

		public char getGCS();
		public char getLCS();
		public XRILiteral getLiteral();
		public XRIXRef getXRef();

		public bool isGlobal();
		public bool isLocal();
		public bool isPersistent();
		public bool isReassignable();
	}
}