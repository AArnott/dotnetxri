namespace DotNetXri.Syntax.Xri3 {
	public interface XRIFragment : XRISyntaxComponent {

		/**
		 * Returns the fragment. In XRI 3.0, this corresponds to the ifragment rule.
		 * @return The fragment excluding the # character.
		 */
		public String getValue();
	}
}