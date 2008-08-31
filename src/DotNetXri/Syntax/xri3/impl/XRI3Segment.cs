package org.openxri.xri3.impl;

import java.util.ArrayList;
import java.util.List;

import org.openxri.xri3.XRISegment;
import org.openxri.xri3.XRISubSegment;
import org.openxri.xri3.impl.parser.ParserException;
import org.openxri.xri3.impl.parser.Rule;
import org.openxri.xri3.impl.parser.Parser.rel_subseg;
import org.openxri.xri3.impl.parser.Parser.rel_subseg_nc;
import org.openxri.xri3.impl.parser.Parser.subseg;
import org.openxri.xri3.impl.parser.Parser.xri_segment;
import org.openxri.xri3.impl.parser.Parser.xri_segment_nc;
import org.openxri.xri3.impl.parser.Parser.xri_segment_nz;

public class XRI3Segment extends XRI3SyntaxComponent implements XRISegment {

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
		
		Object object = this.rule;	// xri_segment or xri_segment_nz or xri_segment_nc

		// read rel_subsegs or subsegs or rel_subseg_ncs from xri_segment or xri_segment_nz or xri_segment_nc

		// xri_segment or xri_segment_nz or xri_segment_nc ?
		
		if (object instanceof xri_segment) {
			
			// read rel_subseg or subseg from xri_segment
			
			List list_xri_segment = ((xri_segment) object).rules;
			if (list_xri_segment.size() < 1) return;
			object = list_xri_segment.get(0);	// rel_subseg or subseg
			
			// rel_subseg or subseg?
			
			if (object instanceof rel_subseg) {
				
				this.subSegments.add(new XRI3SubSegment((rel_subseg) object));
			} else if (object instanceof subseg) {
				
				this.subSegments.add(new XRI3SubSegment((subseg) object));
			} else {
				
				throw new ClassCastException(object.getClass().getName());
			}
			
			// read subsegs from xri_segment
			
			if (list_xri_segment.size() < 2) return;
			for (int i=1; i<list_xri_segment.size(); i++) {
				
				object = list_xri_segment.get(i);	// subseg
				this.subSegments.add(new XRI3SubSegment((subseg) object));
			}
		} else if (object instanceof xri_segment_nz) {
			
			// read rel_subseg or subseg from xri_segment_nz
			
			List list_xri_segment_nz = ((xri_segment_nz) object).rules;
			if (list_xri_segment_nz.size() < 1) return;
			object = list_xri_segment_nz.get(0);	// rel_subseg or subseg
			
			// rel_subseg or subseg?
			
			if (object instanceof rel_subseg) {
				
				this.subSegments.add(new XRI3SubSegment((rel_subseg) object));
			} else if (object instanceof subseg) {
				
				this.subSegments.add(new XRI3SubSegment((subseg) object));
			} else {
				
				throw new ClassCastException(object.getClass().getName());
			}
			
			// read subsegs from xri_segment
			
			if (list_xri_segment_nz.size() < 2) return;
			for (int i=1; i<list_xri_segment_nz.size(); i++) {
				
				object = list_xri_segment_nz.get(i);	// subseg
				this.subSegments.add(new XRI3SubSegment((subseg) object));
			}
		} else if (object instanceof xri_segment_nc) {
			
			// read rel_subseg_nc or subseg from xri_segment_nc
			
			List list_xri_segment_nc = ((xri_segment_nc) object).rules;
			if (list_xri_segment_nc.size() < 1) return;
			object = list_xri_segment_nc.get(0);	// rel_subseg_nc or subseg
			
			// rel_subseg_nc or subseg?
			
			if (object instanceof rel_subseg_nc) {
				
				this.subSegments.add(new XRI3SubSegment((rel_subseg_nc) object));
			} else if (object instanceof subseg) {
				
				this.subSegments.add(new XRI3SubSegment((subseg) object));
			} else {
				
				throw new ClassCastException(object.getClass().getName());
			}
			
			// read subsegs from xri_segment
			
			if (list_xri_segment_nc.size() < 2) return;
			for (int i=1; i<list_xri_segment_nc.size(); i++) {
				
				object = list_xri_segment_nc.get(i);	// subseg
				this.subSegments.add(new XRI3SubSegment((subseg) object));
			}
		} else {
			
			throw new ClassCastException(object.getClass().getName());
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

	public boolean startsWith(XRISubSegment[] subSegments) {

		if (this.subSegments.size() < subSegments.length) return(false);

		for (int i=0; i<subSegments.length; i++) {

			if (! (this.subSegments.get(i).equals(subSegments[i]))) return(false);
		}

		return(true);
	}
}
