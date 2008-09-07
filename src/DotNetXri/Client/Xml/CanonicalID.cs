namespace DotNetXri.Client.Xml {

using java.net.URISyntaxException;

using org.w3c.dom.Element;

public class CanonicalID :SimpleXMLElement{

	public CanonicalID() : base(Tags.TAG_CANONICALID) {
	}
	
	public CanonicalID(String canonicalidString) : base(Tags.TAG_CANONICALID) {
		setValue(canonicalidString);
	}

	/**
	* This method constructs the obj from DOM.  It does not keep a
	* copy of the DOM around.  Whitespace information is lost in this process.
	*/
	public CanonicalID(Element elem) throws URISyntaxException
	: base(Tags.TAG_CANONICALID) {
		fromXML(elem);
	}

}
}