namespace DotNetXri.Syntax.Xri3.Impl {

public class XRI3 :XRI3SyntaxComponent, XRI {

	private static final long serialVersionUID = 1556756335913091713L;
	
	private static final Set reserved = new HashSet(Arrays.asList( new String[] {
		"user",
		"users",
		"individual",
		"individuals",
		"person",
		"persons",
		"personal",
		"personal.name",
		"personal.names",
		"organization",
		"organizations",
		"organizational",
		"organizational.name",
		"organizational.names",
		"name",
		"names",
		"iname",
		"inames",
		"i-name",
		"i-names",
		"i.name",
		"i.names",
		"number",
		"numbers",
		"inumber",
		"inumbers",
		"i-number",
		"i-numbers",
		"i.number",
		"i.numbers",
		"broker",
		"brokers",
		"i-broker",
		"i-brokers",
		"i.broker",
		"i.brokers",
		"gsp",
		"grsp",
		"global.service",
		"global.services",
		"global.service.provider",
		"global.service.providers",
		"public",
		"trust",
		"federation",
		"federations",
		"global",
		"service",
		"services",
		"provider",
		"providers",
		"registry",
		"registries",
		"registrant",
		"registrants",
		"aero",
		"biz",
		"cat",
		"com",
		"coop",
		"info",
		"jobs",
		"mobi",
		"museum",
		"net",
		"org",
		"pro",
		"travel",
		"gov",
		"edu",
		"mil",
		"int",
		"www",
		"ftp",
		"mail",
		"xdi",
		"xdiorg",
		"xdi-org",
		"xdi.org",
		"xri",
		"xriorg",
		"xri-org",
		"xri.org",
		"xri.xdi",
		"xdi.xri",
		"xri-xdi",
		"xdi-xri",
		"itrust",
		"i-trust",
		"i.trust",
		"cordance",
		"cordance.corp",
		"cordance.corporation",
		"cordance.net"
	}));

	private Rule rule;

	private String scheme;
	private XRI3Authority authority;
	private XRI3Path path;
	private XRI3Query query;
	private XRI3Fragment fragment;

	public XRI3(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("xri", string);
		this.read();
	}

	public XRI3(XRI xri, XRISyntaxComponent xriPart) throws ParserException {

		StringBuffer buffer = new StringBuffer();

		buffer.append(xri.toString());
		buffer.append(xriPart.toString());

		this.rule = XRI3Util.getParser().parse("xri", buffer.toString());
		this.read();
	}

	public XRI3(XRI xri, String xriPart) throws ParserException {

		StringBuffer buffer = new StringBuffer();

		buffer.append(xri.toString());
		buffer.append(xriPart);

		this.rule = XRI3Util.getParser().parse("xri", buffer.toString());
		this.read();
	}

	public XRI3(char gcs, String uri) throws ParserException {

		StringBuffer buffer = new StringBuffer();

		buffer.append(gcs.toString());
		buffer.append(XRI3Constants.XREF_START);
		buffer.append(uri);
		buffer.append(XRI3Constants.XREF_END);

		this.rule = XRI3Util.getParser().parse("xri", buffer.toString());
		this.read();
	}

	XRI3(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {

		this.scheme = null;
		this.authority = null;
		this.path = null;
		this.query = null;
		this.fragment = null;
	}

	private void read() {

		this.reset();

		Object obj = this.rule;	// xri

		// read xri_scheme or xri_noscheme from xri

		List list_xri = ((xri) obj).rules;
		if (list_xri.size() < 1) return;
		obj = list_xri.get(0);	// xri_scheme or xri_noscheme

		// xri_scheme or xri_noscheme ?

		if (obj is xri_scheme) {

			// read "xri:" from xri_scheme

			List list_xri_scheme = ((xri_scheme) obj).rules;
			if (list_xri_scheme.size() < 1) return;
			obj = list_xri_scheme.get(0);	// "xri:"
			this.scheme = ((StringValue) obj).spelling;

			// read xri_noscheme from xri_scheme

			if (list_xri_scheme.size() < 2) return;
			obj = list_xri_scheme.get(1);	// xri_noscheme
		} else if (obj is xri_noscheme) {

		} else {

			throw new ClassCastException(obj.getClass().getName());
		}

		// read xri_hier_part from xri_noscheme

		List list_xri_noscheme = ((xri_noscheme) obj).rules;
		if (list_xri_noscheme.size() < 1) return;
		obj = list_xri_noscheme.get(0);	// xri_hier_part

		// read xri_authority from xri_hier_part

		List list_xri_hier_part = ((xri_hier_part) obj).rules;
		if (list_xri_hier_part.size() < 1) return;
		obj = list_xri_hier_part.get(0);	// xri_authority
		this.authority = new XRI3Authority((xri_authority) obj);
		if (this.authority.getParserObject().spelling.length() < 1) this.authority = null;

		// read xri_path_abempty from xri_hier_part

		if (list_xri_hier_part.size() < 2) return;
		obj = list_xri_hier_part.get(1);	// xri_path_abempty
		this.path = new XRI3Path((xri_path_abempty) obj);
		if (this.path.getParserObject().spelling.length() < 1) this.path = null;

		// read iquery or ifragment from xri_noscheme

		if (list_xri_noscheme.size() < 3) return;
		obj = list_xri_noscheme.get(2);	// iquery or ifragment

		// iquery or ifragment ?

		if (obj is iquery) {

			this.query = new XRI3Query((iquery) obj);
			if (this.query.getParserObject().spelling.length() < 1) this.query = null;

			// read ifragment from xri_noscheme

			if (list_xri_noscheme.size() < 5) return;
			obj = list_xri_noscheme.get(4);	// ifragment
			this.fragment = new XRI3Fragment((ifragment) obj);
			if (this.fragment.getParserObject().spelling.length() < 1) this.fragment = null;
		} else if (obj is ifragment) {

			this.fragment = new XRI3Fragment((ifragment) obj);
			if (this.fragment.getParserObject().spelling.length() < 1) this.fragment = null;
		} else {

			throw new ClassCastException(obj.getClass().getName());
		}
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public bool hasScheme() {

		return(this.scheme != null);
	}

	public bool hasAuthority() {

		return(this.authority != null);
	}

	public bool hasPath() {

		return(this.path != null);
	}

	public bool hasQuery() {

		return(this.query != null);
	}

	public bool hasFragment() {

		return(this.fragment != null);
	}

	public String getScheme() {

		return(this.scheme);
	}

	public XRIAuthority getAuthority() {

		return(this.authority);
	}

	public XRIPath getPath() {

		return(this.path);
	}

	public XRIQuery getQuery() {

		return(this.query);
	}

	public XRIFragment getFragment() {

		return(this.fragment);
	}

	public bool isIName() {

		List subSegments = this.authority.getSubSegments();

		// all subsegments must be reassignable

		for (int i=0; i<subSegments.size(); i++) {

			XRI3SubSegment subSegment = (XRI3SubSegment) subSegments.get(i);
			if (! subSegment.isReassignable()) return(false);
		}

		// some additional rules for i-names

		String spelling = this.authority.toString();

		if (spelling.startsWith(".")) return(false);
		if (spelling.endsWith(".")) return(false);
		if (spelling.startsWith("-")) return(false);
		if (spelling.endsWith("-")) return(false);
		if (spelling.indexOf("..") >= 0) return(false);
		if (spelling.indexOf("--") >= 0) return(false);
		if (spelling.indexOf(".-") >= 0) return(false);
		if (spelling.indexOf("-.") >= 0) return(false);
		if (spelling.indexOf('%') >= 0) return(false);
		if (spelling.indexOf('_') >= 0) return(false);
		if (spelling.length() > 254) return(false);

		return(true);
	}
	
	public bool isINumber() {

		List subSegments = this.authority.getSubSegments();

		// all subsegments must be persistent

		for (int i=0; i<subSegments.size(); i++) {

			XRI3SubSegment subSegment = (XRI3SubSegment) subSegments.get(i);
			if (! subSegment.isPersistent()) return(false);
		}

		return(true);
	}

	public bool isReserved() {
		
		String spelling = this.authority.toString();
		
		return(reserved.contains(spelling.substring(1)) | reserved.contains(spelling.substring(1)));
	}

	public String toIRINormalForm() {

		StringBuffer iri = new StringBuffer();

		// authority

		if (this.authority != null) {

			iri.append(XRI3Constants.XRI_SCHEME);
			iri.append(XRI3Constants.AUTHORITY_PREFIX).append(this.authority.toIRINormalForm());
		}

		// path

		if (this.path != null) {

			iri.append(XRI3Constants.PATH_PREFIX).append(this.path.toIRINormalForm());
		}

		// query

		if (this.query != null) {

			iri.append(XRI3Constants.QUERY_PREFIX).append(this.query.toIRINormalForm());
		}

		// fragment

		if (this.fragment != null) {

			iri.append(XRI3Constants.FRAGMENT_PREFIX).append(this.fragment.toIRINormalForm());
		}

		// done

		return(iri.toString());
	}

	public bool isValidXRIReference() {

		XRIReference xriReference;

		try {

			xriReference = this.toXRIReference();
		} catch (Exception ex) {

			return(false);
		}

		return(xriReference != null);
	}

	public XRIReference toXRIReference() throws ParserException {

		return(new XRI3Reference(this.toString()));
	}

	public XRI3Reference toXRI3Reference() throws ParserException {

		return(new XRI3Reference(this.toString()));
	}

	public bool startsWith(XRI xri) {

		if (xri.getAuthority() == null) return(true);
		if (xri.getAuthority() != null && this.getAuthority() == null) return(false);

		if (! this.getAuthority().equals(xri.getAuthority())) return(false);

		if (xri.getPath() == null) return(true);
		if (xri.getPath() != null && this.getPath() == null) return(false);

		List thisSegments = this.getPath().getSegments();
		List xriSegments = xri.getPath().getSegments();

		if (thisSegments.size() < xriSegments.size()) return(false);

		for (int i=0; i<xriSegments.size(); i++) {

			if (! (thisSegments.get(i).equals(xriSegments.get(i)))) return(false);
		}

		return(true);
	}
}
}