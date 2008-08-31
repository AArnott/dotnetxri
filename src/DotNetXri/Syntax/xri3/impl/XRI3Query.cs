namespace DotNetXri.Syntax.Xri3.Impl {
	public class XRI3Query :XRI3SyntaxComponent, XRIQuery {

	private static final long serialVersionUID = 8838957773108506171L;

	private Rule rule;
	
	private String value;

	public XRI3Query(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("iquery", string);
		this.read();
	}

	XRI3Query(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {
		
		this.value = null;
	}

	private void read() {

		this.reset();
		
		Object obj = this.rule;	// iquery

		this.value = ((iquery) obj).spelling;
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public String getValue() {
		
		return(this.value);
	}
}
}