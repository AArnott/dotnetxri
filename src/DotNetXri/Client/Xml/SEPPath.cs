namespace DotNetXri.Client.Xml {

	//using org.w3c.dom.Node;


	public class SEPPath : SEPElement {

		/**
			* Creates a default <code>EppXriServiceEndpointPath</code> obj
			*/
		public SEPPath() {

		}

		/**
			* Creates an <code>EppXriServiceEndpointPath</code> obj with the specified fields
			*/
		public SEPPath(string path, string match, bool? select)
			: base(path, match, select) {
		}

		/**
			* Converts an XML element into an <code>EppXriServiceEndpointPath</code> obj.
			* The caller of this method must make sure that the root node is of
			* the EPP XRI sepTypeType.
			*
			* @param root root node for an <code>EppXriServiceEndpoint</code> obj in
			*             XML format
			*
			* @return an <code>EppXriServiceEndpointPath</code> obj, or null if the node is
			*         invalid
			*/
		public static SEPPath fromXML(XmlNode root) {
			SEPPath sepPath = new SEPPath();
			sepPath.setFromXML(root);
			return sepPath;
		}

		/**
			* Gets the value of this Path rule.
			* This is an alias for the superclass' <code>getValue</code> method.
			*/
		public string getPath() {
			return getValue();
		}

		/**
			* Sets the value of this Path rule.
			* This is an alias for the superclass' <code>setValue</code> method.
			*/
		public void setPath(string path) {
			setValue(path);
		}


		public override string ToString() {
			return ToString(Tags.TAG_PATH);
		}

		public Object clone() {
			return new SEPPath(getPath(), getMatch(), getSelect());
		}
	}
}