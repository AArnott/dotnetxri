namespace DotNetXri.Syntax.Xri3.Impl {
public class XRI3Segment :XRI3SyntaxComponent, XRISegment {

	private static final long serialVersionUID = 8461242564541252885L;

	private Rule rule;

	private List subSegments;

	public XRI3Segment(String string) throws ParserException {

		this.rule = XRI3Util.getParser().parse("xri-segment", string);
		this.read();
	}

	XRI3Segment(Rule rule) {

		this.rule = rule;
		this.read();
	}

	private void reset() {
		
		this.subSegments = new ArrayList();
	}
	
	private void read() {

		this.reset();
		
		Object obj = this.rule;	// xri_segment or xri_segment_nz or xri_segment_nc

		// read rel_subsegs or subsegs or rel_subseg_ncs from xri_segment or xri_segment_nz or xri_segment_nc

		// xri_segment or xri_segment_nz or xri_segment_nc ?
		
		if (obj is xri_segment) {
			
			// read rel_subseg or subseg from xri_segment
			
			List list_xri_segment = ((xri_segment) obj).rules;
			if (list_xri_segment.size() < 1) return;
			obj = list_xri_segment.get(0);	// rel_subseg or subseg
			
			// rel_subseg or subseg?
			
			if (obj is rel_subseg) {
				
				this.subSegments.add(new XRI3SubSegment((rel_subseg) obj));
			} else if (obj is subseg) {
				
				this.subSegments.add(new XRI3SubSegment((subseg) obj));
			} else {
				
				throw new ClassCastException(obj.getClass().getName());
			}
			
			// read subsegs from xri_segment
			
			if (list_xri_segment.size() < 2) return;
			for (int i=1; i<list_xri_segment.size(); i++) {
				
				obj = list_xri_segment.get(i);	// subseg
				this.subSegments.add(new XRI3SubSegment((subseg) obj));
			}
		} else if (obj is xri_segment_nz) {
			
			// read rel_subseg or subseg from xri_segment_nz
			
			List list_xri_segment_nz = ((xri_segment_nz) obj).rules;
			if (list_xri_segment_nz.size() < 1) return;
			obj = list_xri_segment_nz.get(0);	// rel_subseg or subseg
			
			// rel_subseg or subseg?
			
			if (obj is rel_subseg) {
				
				this.subSegments.add(new XRI3SubSegment((rel_subseg) obj));
			} else if (obj is subseg) {
				
				this.subSegments.add(new XRI3SubSegment((subseg) obj));
			} else {
				
				throw new ClassCastException(obj.getClass().getName());
			}
			
			// read subsegs from xri_segment
			
			if (list_xri_segment_nz.size() < 2) return;
			for (int i=1; i<list_xri_segment_nz.size(); i++) {
				
				obj = list_xri_segment_nz.get(i);	// subseg
				this.subSegments.add(new XRI3SubSegment((subseg) obj));
			}
		} else if (obj is xri_segment_nc) {
			
			// read rel_subseg_nc or subseg from xri_segment_nc
			
			List list_xri_segment_nc = ((xri_segment_nc) obj).rules;
			if (list_xri_segment_nc.size() < 1) return;
			obj = list_xri_segment_nc.get(0);	// rel_subseg_nc or subseg
			
			// rel_subseg_nc or subseg?
			
			if (obj is rel_subseg_nc) {
				
				this.subSegments.add(new XRI3SubSegment((rel_subseg_nc) obj));
			} else if (obj is subseg) {
				
				this.subSegments.add(new XRI3SubSegment((subseg) obj));
			} else {
				
				throw new ClassCastException(obj.getClass().getName());
			}
			
			// read subsegs from xri_segment
			
			if (list_xri_segment_nc.size() < 2) return;
			for (int i=1; i<list_xri_segment_nc.size(); i++) {
				
				obj = list_xri_segment_nc.get(i);	// subseg
				this.subSegments.add(new XRI3SubSegment((subseg) obj));
			}
		} else {
			
			throw new ClassCastException(obj.getClass().getName());
		}
	}

	public Rule getParserObject() {

		return(this.rule);
	}

	public List getSubSegments() {

		return(this.subSegments);
	}
	
	public int getNumSubSegments() {
		
		return(this.subSegments.size());
	}
	
	public XRISubSegment getSubSegment(int i) {
		
		return((XRISubSegment) this.subSegments.get(i));
	}

	public XRISubSegment getFirstSubSegment() {

		if (this.subSegments.size() < 1) return(null);

		return((XRISubSegment) this.subSegments.get(0));
	}

	public XRISubSegment getLastSubSegment() {

		if (this.subSegments.size() < 1) return(null);

		return((XRISubSegment) this.subSegments.get(this.subSegments.size() - 1));
	}

	public bool startsWith(XRISubSegment[] subSegments) {

		if (this.subSegments.size() < subSegments.length) return(false);

		for (int i=0; i<subSegments.length; i++) {

			if (! (this.subSegments.get(i).equals(subSegments[i]))) return(false);
		}

		return(true);
	}
}
}