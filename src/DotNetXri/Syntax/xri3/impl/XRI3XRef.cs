namespace DotNetXri.Syntax.Xri3.Impl {
public class XRI3XRef :XRI3SyntaxComponent, XRIXRef {

	private static final long serialVersionUID = 5499307555025868602L;

	private Rule rule;

	private XRI3Reference xriReference;
	private String iri;

	public XRI3XRef(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("xref", string);
		this.read();
	}

	XRI3XRef(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {

		this.xriReference = null;
		this.iri = null;
	}

	private void read() {

		this.reset();

		Object obj = this.rule;	// xref or xref_empty or xref_xri_reference or xref_IRI

		// xref or xref_empty or xref_xri_reference or xref_IRI ?

		if (obj instanceof xref) {

			List list_xref = ((xref) obj).rules;
			if (list_xref.size() < 1) return;
			obj = list_xref.get(0);	// xref_empty or xref_xri_reference or xref_IRI
		} else if (obj instanceof xref_empty) {

		} else if (obj instanceof xref_xri_reference) {

		} else if (obj instanceof xref_IRI) {

		} else {

			throw new ClassCastException(obj.getClass().getName());
		}

		// xref_empty or xref_xri_reference or xref_IRI ?


		if (obj instanceof xref_empty) {

		} else if (obj instanceof xref_xri_reference) {

			// read xri_reference from xref_xri_reference
			
			List list_xref_xri_reference = ((xref_xri_reference) obj).rules;
			if (list_xref_xri_reference.size() < 2) return;
			obj = list_xref_xri_reference.get(1);	// xri_reference
			this.xriReference = new XRI3Reference((xri_reference) obj);
		} else if (obj instanceof xref_IRI) {

			// read IRI from xref_IRI
			
			List list_xref_IRI = ((xref_IRI) obj).rules;
			if (list_xref_IRI.size() < 2) return;
			obj = list_xref_IRI.get(1);	// IRI
			this.iri = ((IRI) obj).spelling;
		} else {

			throw new ClassCastException(obj.getClass().getName());
		}
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public bool hasXRIReference() {

		return(this.xriReference != null);
	}

	public bool hasIRI() {

		return(this.iri != null);
	}

	public XRIReference getXRIReference() {

		return(this.xriReference);
	}

	public String getIRI() {

		return(this.iri);
	}
}
}