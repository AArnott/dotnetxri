using System.Text;

namespace DotNetXri.Syntax.Xri3.Impl {
public class XRI3Reference :XRI3SyntaxComponent, XRIReference {

	private static final long serialVersionUID = 4191016969141944835L;

	private Rule rule;

	private XRI3 xri;
	private XRI3Path path;
	private XRI3Query query;
	private XRI3Fragment fragment;

	public XRI3Reference(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("xri-reference", string);
		this.read();
	}

	public XRI3Reference(XRIReference xriReference, XRISyntaxComponent xriPart) throws ParserException {

		StringBuilder buffer = new StringBuilder();

		buffer.append(xriReference.toString());
		buffer.append(xriPart.toString());

		this.rule = XRI3Util.getParser().parse("xri-reference", buffer.toString());
		this.read();
	}

	public XRI3Reference(XRIReference xriReference, String xriPart) throws ParserException {

		StringBuilder buffer = new StringBuilder();

		buffer.append(xriReference.toString());
		buffer.append(xriPart);

		this.rule = XRI3Util.getParser().parse("xri-reference", buffer.toString());
		this.read();
	}

	XRI3Reference(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {

		this.xri = null;
		this.path = null;
		this.query = null;
		this.fragment = null;
	}

	private void read() {

		this.reset();

		Object obj = this.rule;	// xri_reference

		// read xri or relative_xri_ref from xri_reference

		List list_xri_reference = ((xri_reference) obj).rules;
		if (list_xri_reference.size() < 1) return;
		obj = list_xri_reference.get(0);	// xri or relative_xri_ref

		// xri or relative_xri_ref ?

		if (obj is xri) {

			this.xri = new XRI3((xri) obj);
		} else if (obj is relative_xri_ref) {

			// read relative_xri_part from relative_xri_ref

			List list_relative_xri_ref = ((relative_xri_ref) obj).rules;
			if (list_relative_xri_ref.size() < 1) return;
			obj = list_relative_xri_ref.get(0);	// relative_xri_part

			// read xri_path_abs or xri_path_noscheme or ipath_empty from relative_xri_part

			List list_relative_xri_part = ((relative_xri_part) obj).rules;
			if (list_relative_xri_part.size() < 1) return;
			obj = list_relative_xri_part.get(0);	// xri_path_abs or xri_path_noscheme or ipath_empty	

			// read xri_path_abs or xri_path_noscheme or ipath_emptry ?

			if (obj is xri_path_abs) {

				this.path = new XRI3Path((xri_path_abs) obj);
			} else if (obj is xri_path_noscheme) {

				this.path = new XRI3Path((xri_path_noscheme) obj);
			} else if (obj is ipath_empty) {

				this.path = new XRI3Path((ipath_empty) obj);
			} else {

				throw new ClassCastException(obj.getClass().getName());
			}

			// read iquery from relative_xri_ref

			if (list_relative_xri_ref.size() < 3) return;
			obj = list_relative_xri_ref.get(2);	// iquery
			this.query = new XRI3Query((iquery) obj);

			// read ifragment from relative_xri_ref

			if (list_relative_xri_ref.size() < 5) return;
			obj = list_relative_xri_ref.get(4);	// ifragment
			this.fragment = new XRI3Fragment((ifragment) obj);
		} else {

			throw new ClassCastException(obj.getClass().getName());
		}
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public bool hasScheme() {

		if (this.xri != null) return(this.xri.hasScheme());

		return(false);
	}

	public bool hasAuthority() {

		if (this.xri != null) return(this.xri.hasAuthority());

		return(false);
	}

	public bool hasPath() {

		if (this.xri != null) return(this.xri.hasPath());

		return(this.path != null);
	}

	public bool hasQuery() {

		if (this.xri != null) return(this.xri.hasQuery());

		return(this.query != null);
	}

	public bool hasFragment() {

		if (this.xri != null) return(this.xri.hasFragment());

		return(this.fragment != null);
	}

	public String getScheme() {

		if (this.xri != null) return(this.xri.getScheme());

		return(null);
	}

	public XRIAuthority getAuthority() {

		if (this.xri != null) return(this.xri.getAuthority());

		return(null);
	}

	public XRIPath getPath() {

		if (this.xri != null) return(this.xri.getPath());

		return(this.path);
	}

	public XRIQuery getQuery() {

		if (this.xri != null) return(this.xri.getQuery());

		return(this.query);
	}

	public XRIFragment getFragment() {

		if (this.xri != null) return(this.xri.getFragment());

		return(this.fragment);
	}

	public String toIRINormalForm() {

		if (this.xri != null) return(this.xri.toIRINormalForm());

		return(base.toIRINormalForm());
	}

	public bool isValidXRI() {

		XRI xri;
		
		try {
			
			xri = this.toXRI();
		} catch (Exception ex) {
			
			return(false);
		}
		
		return(xri != null);
	}
	
	public XRI toXRI() throws ParserException {

		return(new XRI3(this.toString()));
	}

	public XRI3 toXRI3() throws ParserException {

		return(new XRI3(this.toString()));
	}
}
}