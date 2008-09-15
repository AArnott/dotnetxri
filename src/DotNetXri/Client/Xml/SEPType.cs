using System.Xml;
namespace DotNetXri.Client.Xml {

	//using org.w3c.dom.XmlNode;

	public class SEPType : SEPElement {
		/**
			* Creates a default <code>EppXriServiceEndpointType</code> obj
			*/
		public SEPType() {

		}

		/**
			* Creates an <code>EppXriServiceEndpointType</code> obj with the specified fields
			*/
		public SEPType(string type, string match, bool? select)
			: base(type, match, select) {
		}

		/**
			* Converts an XML element into an <code>EppXriServiceEndpointType</code> obj.
			* The caller of this method must make sure that the root node is of
			* the EPP XRI sepTypeType.
			*
			* @param root root node for an <code>EppXriServiceEndpoint</code> obj in
			*             XML format
			*
			* @return an <code>EppXriServiceEndpointType</code> obj, or null if the node is
			*         invalid
			*/
		public static SEPElement fromXML(XmlNode root) {
			SEPType sepType = new SEPType();
			sepType.setFromXML(root);
			return sepType;
		}

		/**
			* Gets the value of this Type rule.
			* This is an alias for the superclass' <code>getValue</code> method.
			*/
		public string getType() {
			return getValue();
		}
		/**
			* Sets the value of this Type rule.
			* This is an alias for the superclass' <code>setValue</code> method.
			*/
		public void setType(string type) {
			setValue(type);
		}


		public override string ToString() {
			return ToString(Tags.TAG_TYPE);
		}

		public bool match(string match) {
			if (getValue() != null && (getValue().Equals(match)))
				return true;
			return false;
		}

		public Object clone() {
			return new SEPType(getType(), getMatch(), getSelect());
		}
	}
}