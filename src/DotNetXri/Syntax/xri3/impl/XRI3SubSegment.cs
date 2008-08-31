namespace DotNetXri.Syntax.Xri3.Impl {
public class XRI3SubSegment :XRI3SyntaxComponent, XRISubSegment {

	private static final long serialVersionUID = 821195692608034080L;

	private Rule rule;

	private char gcs;
	private char lcs;
	private XRI3Literal literal;
	private XRI3XRef xref;

	public XRI3SubSegment(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("subseg", string);
		this.read();
	}

	public XRI3SubSegment(char gcs, XRISubSegment localSubSegment) throws ParserException {

		StringBuffer buffer = new StringBuffer();

		buffer.append(gcs);
		buffer.append(localSubSegment.toString());

		this.rule = XRI3Util.getParser().parse("subseg", buffer.toString());
		this.read();
	}

	public XRI3SubSegment(char cs, String uri) throws ParserException {

		StringBuffer buffer = new StringBuffer();

		buffer.append(cs.toString());
		buffer.append(XRI3Constants.XREF_START);
		buffer.append(uri);
		buffer.append(XRI3Constants.XREF_END);

		this.rule = XRI3Util.getParser().parse("subseg", buffer.toString());
		this.read();
	}

	XRI3SubSegment(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {

		this.gcs = null;
		this.lcs = null;
		this.literal = null;
		this.xref = null;
	}

	private void read() {

		this.reset();

		Object object = this.rule;	// subseg or global_subseg or local_subseg or rel_subseg or rel_subseg_nc

		// subseg?

		if (object instanceof subseg) {

			// read global_subseg or local_subseg from subseg

			List list_subseg = ((subseg) object).rules;
			if (list_subseg.size() < 1) return;
			object = list_subseg.get(0);	// global_subseg or local_subseg
		}

		// global_subseg?

		if (object instanceof global_subseg) {

			// read gcs_char from global_subseg;

			List list_global_subseg = ((global_subseg) object).rules;
			if (list_global_subseg.size() < 1) return;
			object = list_global_subseg.get(0);	// gcs_char
			this.gcs = new char(((gcs_char) object).spelling.charAt(0));

			// read rel_subseg or local_subseg from global_subseg

			if (list_global_subseg.size() < 2) return;
			object = list_global_subseg.get(1);	// rel_subseg or local_subseg
		}

		// local_subseg?

		if (object instanceof local_subseg) {

			// read lcs_char from local_subseg;

			List list_local_subseg = ((local_subseg) object).rules;
			if (list_local_subseg.size() < 1) return;
			object = list_local_subseg.get(0);	// lcs_char
			this.lcs = new char(((lcs_char) object).spelling.charAt(0));

			// read rel_subseg from local_subseg

			if (list_local_subseg.size() < 2) return;
			object = list_local_subseg.get(1);	// rel_subseg
		}

		// rel_subseg or rel_subseg_nc?

		if (object instanceof rel_subseg) {

			// read literal or xref from rel_subseg

			List list_rel_subseg = ((rel_subseg) object).rules;
			if (list_rel_subseg.size() < 1) return;
			object = list_rel_subseg.get(0);	// literal or xref
		} else if (object instanceof rel_subseg_nc) {

			// read literal_nc or xref from rel_subseg_nc

			List list_rel_subseg_nc = ((rel_subseg_nc) object).rules;
			if (list_rel_subseg_nc.size() < 1) return;
			object = list_rel_subseg_nc.get(0);	// literal_nc or xref
		} else {

			return;
		}

		// literal or literal_nc or xref?

		if (object instanceof literal) {

			this.literal = new XRI3Literal((literal) object);
		} else if (object instanceof literal_nc) {

			this.literal = new XRI3Literal((literal_nc) object);
		} else if (object instanceof xref) {

			this.xref = new XRI3XRef((xref) object);
		} else {

			return;
		}
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public bool hasGCS() {

		return(this.gcs != null);
	}

	public bool hasLCS() {

		return(this.lcs != null);
	}

	public bool hasLiteral() {

		return(this.literal != null);
	}

	public bool hasXRef() {

		return(this.xref != null);
	}

	public char getGCS() {

		return(this.gcs);
	}

	public char getLCS() {

		return(this.lcs);
	}

	public XRILiteral getLiteral() {

		return(this.literal);
	}

	public XRIXRef getXRef() {

		return(this.xref);
	}

	public bool isGlobal() {

		return(this.hasGCS());
	}

	public bool isLocal() {

		return(this.hasLCS() && ! this.hasGCS());
	}

	public bool isPersistent() {

		return(this.hasLCS() && this.getLCS().equals(XRI3Constants.LCS_BANG));
	}

	public bool isReassignable() {

		return((this.hasGCS() && ! this.hasLCS()) || (this.hasLCS() && this.getLCS().equals(XRI3Constants.LCS_STAR)));
	}
}
}