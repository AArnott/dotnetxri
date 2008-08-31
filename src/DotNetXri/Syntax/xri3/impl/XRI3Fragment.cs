namespace DotNetXri.Syntax.Xri3.Impl {

public class XRI3Fragment :XRI3SyntaxComponent, XRIFragment {

	private static final long serialVersionUID = 5034503145058610708L;

	private Rule rule;
	
	private String value;

	public XRI3Fragment(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("ifragment", string);
		this.read();
	}

	XRI3Fragment(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {
		
		this.value = null;
	}
	
	private void read() {

		this.reset();
		
		Object obj = this.rule;	// ifragment

		this.value = ((ifragment) obj).spelling;
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public String getValue() {
		
		return(this.value);
	}
}
}