namespace DotNetXri.Syntax.Xri3 {
	public interface XRISyntaxComponent : Serializable, Cloneable, Comparable {

		public String toURINormalForm();
		public String toIRINormalForm();
	}
}