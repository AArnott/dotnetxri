namespace DotNetXri.Client.Xml {

//using java.net.URISyntaxException;
//using org.w3c.dom.XmlElement;

public class CanonicalEquivID :SimpleXMLElement {

	public CanonicalEquivID(CanonicalEquivID ceid) : base(ceid) {
	}

	public CanonicalEquivID() : base(Tags.TAG_CANONICALEQUIVID) {
	}
	
	public CanonicalEquivID(string canonicalidString) : base(Tags.TAG_CANONICALEQUIVID) {
		setValue(canonicalidString);
	}

	/**
	* This method constructs the obj from DOM.  It does not keep a
	* copy of the DOM around.  Whitespace information is lost in this process.
	*/
	public CanonicalEquivID(XmlElement elem)//throws URISyntaxException
	: base(Tags.TAG_CANONICALEQUIVID) {
		fromXML(elem);
	}

}
}