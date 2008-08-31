namespace DotNetXri.Syntax.Xri3 {
	public interface XRIPath : XRISyntaxComponent {

		public List getSegments();
		public int getNumSegments();
		public XRISegment getSegment(int i);
		public XRISegment getFirstSegment();
		public XRISegment getLastSegment();

		public bool startsWith(XRISegment[] segments);
	}
}