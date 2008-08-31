namespace DotNetXri.Syntax.Xri3.Impl {
public class XRI3Literal :XRI3SyntaxComponent, XRILiteral {

	private static final long serialVersionUID = -2070825542439606624L;

	private Rule rule;
	
	private String value;

	public XRI3Literal(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("literal", string);
		this.read();
	}

	XRI3Literal(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {
		
		this.value = null;
	}

	private void read() {

		this.reset();
		
		Object obj = this.rule;	// literal or literal_nc

		// literal of literal_nc
		
		if (obj is literal) {
			
			this.value = ((literal) obj).spelling;
		} else if (obj is literal_nc) {
			
			this.value = ((literal_nc) obj).spelling;
		}
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public String getValue() {
		
		return(this.value);
	}
}
}