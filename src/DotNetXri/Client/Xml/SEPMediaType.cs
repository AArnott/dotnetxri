using System.Xml;
namespace DotNetXri.Client.Xml {

	//using org.w3c.dom.Node;


	public class SEPMediaType : SEPElement {

		/**
			* Creates a default <code>EppXriServiceEndpointMediaType</code> obj
			*/
		public SEPMediaType() {

		}

		/**
			* Creates an <code>EppXriServiceEndpointMediaType</code> obj with the specified fields
			*/
		public SEPMediaType(string mediaType, string match, bool? select)
			: base(mediaType, match, select) {
		}

		/**
			* Converts an XML element into an <code>EppXriServiceEndpointMediaType</code> obj.
			* The caller of this method must make sure that the root node is of
			* the EPP XRI sepMediaTypeType.
			*
			* @param root root node for an <code>EppXriServiceEndpoint</code> obj in
			*             XML format
			*
			* @return an <code>EppXriServiceEndpointMediaType</code> obj, or null if the node is
			*         invalid
			*/
		public static SEPMediaType fromXML(XmlNode root) {
			SEPMediaType sepMType = new SEPMediaType();
			sepMType.setFromXML(root);
			return sepMType;
		}

		/**
			* Gets the value of this MediaType rule.
			* This is an alias for the superclass' <code>getValue</code> method.
			*/
		public string getMediaType() {
			return getValue();
		}

		/**
			* Sets the value of this MediaType rule.
			* This is an alias for the superclass' <code>setValue</code> method.
			*/
		public void setMediaType(string mediaType) {
			setValue(mediaType);
		}

		public override string ToString() {
			return ToString(Tags.TAG_MEDIATYPE);
		}

		public Object clone() {
			return new SEPMediaType(getMediaType(), getMatch(), getSelect());
		}
	}
}