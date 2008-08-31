namespace DotNetXri.Syntax.Xri3 {

	public interface XRI : XRISyntaxComponent {

		public bool hasScheme();
		public bool hasAuthority();
		public bool hasPath();
		public bool hasQuery();
		public bool hasFragment();

		public String getScheme();
		public XRIAuthority getAuthority();
		public XRIPath getPath();
		public XRIQuery getQuery();
		public XRIFragment getFragment();

		public bool isIName();
		public bool isINumber();
		public bool isReserved();

		public bool isValidXRIReference();
		public XRIReference toXRIReference();

		public bool startsWith(XRI xri);
	}
}