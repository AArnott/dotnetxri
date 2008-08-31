package org.openxri.xri3;

import junit.framework.TestCase;

import org.openxri.xri3.impl.XRI3;
import org.openxri.xri3.impl.XRI3Authority;
import org.openxri.xri3.impl.XRI3Constants;
import org.openxri.xri3.impl.XRI3SubSegment;

public class ParseTest extends TestCase {

	public ParseTest(String name) {

		super(name);
	}

	public void testXRI3() throws Exception {

		long time = System.currentTimeMillis();

		XRI xri;

		xri = new XRI3("=peacekeeper");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "=peacekeeper");
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_EQUALS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertTrue(xri.getAuthority().getSubSegment(0).hasLiteral());
		assertFalse(xri.getAuthority().getSubSegment(0).hasXRef());
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "peacekeeper");
		assertFalse(xri.hasScheme());
		assertTrue(xri.hasAuthority());
		assertFalse(xri.hasPath());
		assertFalse(xri.hasQuery());
		assertFalse(xri.hasFragment());
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("xri:@cordance");
		assertEquals(xri.getScheme(), "xri:");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "@cordance");
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_AT);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "cordance");
		assertTrue(xri.hasScheme());
		assertTrue(xri.hasAuthority());
		assertFalse(xri.hasPath());
		assertFalse(xri.hasQuery());
		assertFalse(xri.hasFragment());
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("@cordance*drummond");
		assertEquals(xri.getAuthority().getNumSubSegments(), 2);
		assertEquals(xri.getAuthority().getSubSegment(0), "@cordance");
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_AT);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "cordance");
		assertTrue(xri.getAuthority().getSubSegment(0).isGlobal());
		assertFalse(xri.getAuthority().getSubSegment(0).isLocal());
		assertTrue(xri.getAuthority().getSubSegment(0).isReassignable());
		assertFalse(xri.getAuthority().getSubSegment(0).isPersistent());
		assertEquals(xri.getAuthority().getSubSegment(1), "*drummond");
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), XRI3Constants.LCS_STAR);
		assertEquals(xri.getAuthority().getSubSegment(1).getLiteral(), "drummond");
		assertFalse(xri.getAuthority().getSubSegment(1).isGlobal());
		assertTrue(xri.getAuthority().getSubSegment(1).isLocal());
		assertTrue(xri.getAuthority().getSubSegment(1).isReassignable());
		assertFalse(xri.getAuthority().getSubSegment(1).isPersistent());
		assertFalse(xri.hasScheme());
		assertTrue(xri.hasAuthority());
		assertFalse(xri.hasPath());
		assertFalse(xri.hasQuery());
		assertFalse(xri.hasFragment());
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("@cordance/+hr");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "@cordance");
		assertTrue(xri.hasPath());
		assertEquals(xri.getPath().getNumSegments(), 1);
		assertEquals(xri.getPath().getSegment(0), "+hr");
		assertEquals(xri.getPath().getSegment(0).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0), "+hr");
		assertTrue(xri.getPath().getSegment(0).getSubSegment(0).hasLiteral());
		assertFalse(xri.getPath().getSegment(0).getSubSegment(0).hasXRef());
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getLiteral(), "hr");
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getXRef(), null);
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("@cordance/(+hr)");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "@cordance");
		assertTrue(xri.hasPath());
		assertEquals(xri.getPath().getNumSegments(), 1);
		assertEquals(xri.getPath().getSegment(0), "(+hr)");
		assertEquals(xri.getPath().getSegment(0).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0), "(+hr)");
		assertFalse(xri.getPath().getSegment(0).getSubSegment(0).hasLiteral());
		assertTrue(xri.getPath().getSegment(0).getSubSegment(0).hasXRef());
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getLiteral(), null);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getXRef(), "(+hr)");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("@cordance/documentation/xri?page=overview#introduction");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "@cordance");
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_AT);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "cordance");
		assertEquals(xri.getPath().getNumSegments(), 2);
		assertEquals(xri.getPath().getSegment(0).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0), "documentation");
		assertEquals(xri.getPath().getSegment(1).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0), "xri");
		assertFalse(xri.hasScheme());
		assertTrue(xri.hasAuthority());
		assertTrue(xri.hasPath());
		assertTrue(xri.hasQuery());
		assertTrue(xri.hasFragment());
		assertEquals(xri.getQuery(), "page=overview");
		assertEquals(xri.getFragment(), "introduction");
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("+!123");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), XRI3Constants.LCS_BANG);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "123");
		assertFalse(xri.isIName());
		assertTrue(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=!B7BD.2A1D.1040.58CD!2000");
		assertEquals(xri.getAuthority().getNumSubSegments(), 2);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_EQUALS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), XRI3Constants.LCS_BANG);
		assertTrue(xri.getAuthority().getSubSegment(0).isGlobal());
		assertFalse(xri.getAuthority().getSubSegment(0).isLocal());
		assertFalse(xri.getAuthority().getSubSegment(0).isReassignable());
		assertTrue(xri.getAuthority().getSubSegment(0).isPersistent());
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), XRI3Constants.LCS_BANG);
		assertFalse(xri.getAuthority().getSubSegment(1).isGlobal());
		assertTrue(xri.getAuthority().getSubSegment(1).isLocal());
		assertFalse(xri.getAuthority().getSubSegment(1).isReassignable());
		assertTrue(xri.getAuthority().getSubSegment(1).isPersistent());
		assertFalse(xri.isIName());
		assertTrue(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("+person");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "person");
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("+person+name");
		assertEquals(xri.getAuthority().getNumSubSegments(), 2);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "person");
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(1).getLiteral(), "name");
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("+person+address+street");
		assertEquals(xri.getAuthority().getNumSubSegments(), 3);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "person");
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(1).getLiteral(), "address");
		assertEquals(xri.getAuthority().getSubSegment(2).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(2).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(2).getLiteral(), "street");
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("+person/$has/+name");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "+person");
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "person");
		assertEquals(xri.getPath().getNumSegments(), 2);
		assertEquals(xri.getPath().getSegment(0).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getLCS(), null);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getLiteral(), "has");
		assertEquals(xri.getPath().getSegment(1).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0).getLCS(), null);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0).getLiteral(), "name");
		assertFalse(xri.hasQuery());
		assertFalse(xri.hasFragment());
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=markus/$is$a/+person");
		assertEquals(xri.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri.getAuthority().getSubSegment(0), "=markus");
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_EQUALS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "markus");
		assertEquals(xri.getPath().getNumSegments(), 2);
		assertEquals(xri.getPath().getSegment(0).getNumSubSegments(), 2);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getLCS(), null);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(0).getLiteral(), "is");
		assertEquals(xri.getPath().getSegment(0).getSubSegment(1).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(1).getLCS(), null);
		assertEquals(xri.getPath().getSegment(0).getSubSegment(1).getLiteral(), "a");
		assertEquals(xri.getPath().getSegment(1).getNumSubSegments(), 1);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0).getLCS(), null);
		assertEquals(xri.getPath().getSegment(1).getSubSegment(0).getLiteral(), "person");
		assertFalse(xri.hasQuery());
		assertFalse(xri.hasFragment());
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("+!15+!16$v!3");
		assertEquals(xri.getAuthority().getNumSubSegments(), 4);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), XRI3Constants.LCS_BANG);
		assertTrue(xri.getAuthority().getSubSegment(0).isGlobal());
		assertFalse(xri.getAuthority().getSubSegment(0).isLocal());
		assertFalse(xri.getAuthority().getSubSegment(0).isReassignable());
		assertTrue(xri.getAuthority().getSubSegment(0).isPersistent());
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), XRI3Constants.LCS_BANG);
		assertTrue(xri.getAuthority().getSubSegment(1).isGlobal());
		assertFalse(xri.getAuthority().getSubSegment(1).isLocal());
		assertFalse(xri.getAuthority().getSubSegment(1).isReassignable());
		assertTrue(xri.getAuthority().getSubSegment(1).isPersistent());
		assertEquals(xri.getAuthority().getSubSegment(2).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getAuthority().getSubSegment(2).getLCS(), null);
		assertTrue(xri.getAuthority().getSubSegment(2).isGlobal());
		assertFalse(xri.getAuthority().getSubSegment(2).isLocal());
		assertTrue(xri.getAuthority().getSubSegment(2).isReassignable());
		assertFalse(xri.getAuthority().getSubSegment(2).isPersistent());
		assertEquals(xri.getAuthority().getSubSegment(3).getGCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(3).getLCS(), XRI3Constants.LCS_BANG);
		assertFalse(xri.getAuthority().getSubSegment(3).isGlobal());
		assertTrue(xri.getAuthority().getSubSegment(3).isLocal());
		assertFalse(xri.getAuthority().getSubSegment(3).isReassignable());
		assertTrue(xri.getAuthority().getSubSegment(3).isPersistent());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("$type*mime+text");
		assertEquals(xri.getAuthority().getNumSubSegments(), 3);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(0).getLiteral(), "type");
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), XRI3Constants.LCS_STAR);
		assertEquals(xri.getAuthority().getSubSegment(1).getLiteral(), "mime");
		assertEquals(xri.getAuthority().getSubSegment(2).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(2).getLCS(), null);
		assertEquals(xri.getAuthority().getSubSegment(2).getLiteral(), "text");
		assertTrue(xri.isIName());
		assertFalse(xri.isINumber());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("$is$type+(http://schemas.xmlsoap.org)");
		assertEquals(xri.getAuthority().getNumSubSegments(), 3);
		assertEquals(xri.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getAuthority().getSubSegment(0).getLCS(), null);
		assertTrue(xri.getAuthority().getSubSegment(0).hasLiteral());
		assertFalse(xri.getAuthority().getSubSegment(0).hasXRef());
		assertEquals(xri.getAuthority().getSubSegment(1).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(xri.getAuthority().getSubSegment(1).getLCS(), null);
		assertTrue(xri.getAuthority().getSubSegment(1).hasLiteral());
		assertFalse(xri.getAuthority().getSubSegment(1).hasXRef());
		assertEquals(xri.getAuthority().getSubSegment(2).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(xri.getAuthority().getSubSegment(2).getLCS(), null);
		assertFalse(xri.getAuthority().getSubSegment(2).hasLiteral());
		assertTrue(xri.getAuthority().getSubSegment(2).hasXRef());
		assertEquals(xri.getAuthority().getSubSegment(2).getXRef(), "(http://schemas.xmlsoap.org)");
		assertFalse(xri.getAuthority().getSubSegment(2).getXRef().hasXRIReference());
		assertTrue(xri.getAuthority().getSubSegment(2).getXRef().hasIRI());
		assertEquals(xri.getAuthority().getSubSegment(2).getXRef().getIRI(), "http://schemas.xmlsoap.org");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
		
		xri = new XRI3("=markus+(http://test#f)?query");
		assertTrue(xri.hasQuery());
		assertFalse(xri.hasFragment());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
		
		xri = new XRI3("=markus+(http://test?q)#fragment");
		assertFalse(xri.hasQuery());
		assertTrue(xri.hasFragment());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
		
		xri = new XRI3("=markus?query#fragment");
		assertTrue(xri.hasQuery());
		assertTrue(xri.hasFragment());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
	}

	public void testXRI3Authority() throws Exception {

		long time = System.currentTimeMillis();

		XRIAuthority authority;

		authority = new XRI3Authority("+!15+!16$v!3");
		assertEquals(authority.getNumSubSegments(), 4);
		assertEquals(authority.getSubSegment(0).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(authority.getSubSegment(0).getLCS(), XRI3Constants.LCS_BANG);
		assertTrue(authority.getSubSegment(0).isGlobal());
		assertFalse(authority.getSubSegment(0).isLocal());
		assertFalse(authority.getSubSegment(0).isReassignable());
		assertTrue(authority.getSubSegment(0).isPersistent());
		assertEquals(authority.getSubSegment(1).getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(authority.getSubSegment(1).getLCS(), XRI3Constants.LCS_BANG);
		assertTrue(authority.getSubSegment(1).isGlobal());
		assertFalse(authority.getSubSegment(1).isLocal());
		assertFalse(authority.getSubSegment(1).isReassignable());
		assertTrue(authority.getSubSegment(1).isPersistent());
		assertEquals(authority.getSubSegment(2).getGCS(), XRI3Constants.GCS_DOLLAR);
		assertEquals(authority.getSubSegment(2).getLCS(), null);
		assertTrue(authority.getSubSegment(2).isGlobal());
		assertFalse(authority.getSubSegment(2).isLocal());
		assertTrue(authority.getSubSegment(2).isReassignable());
		assertFalse(authority.getSubSegment(2).isPersistent());
		assertEquals(authority.getSubSegment(3).getGCS(), null);
		assertEquals(authority.getSubSegment(3).getLCS(), XRI3Constants.LCS_BANG);
		assertFalse(authority.getSubSegment(3).isGlobal());
		assertTrue(authority.getSubSegment(3).isLocal());
		assertFalse(authority.getSubSegment(3).isReassignable());
		assertTrue(authority.getSubSegment(3).isPersistent());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
	}

	public void testXRI3SubSegment() throws Exception {

		long time = System.currentTimeMillis();

		XRISubSegment subSegment;

		subSegment = new XRI3SubSegment("*earth");
		assertFalse(subSegment.hasGCS());
		assertTrue(subSegment.hasLCS());
		assertFalse(subSegment.isGlobal());
		assertTrue(subSegment.isLocal());
		assertTrue(subSegment.isReassignable());
		assertFalse(subSegment.isPersistent());
		assertEquals(subSegment.getLiteral(), "earth");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		subSegment = new XRI3SubSegment("@free");
		assertTrue(subSegment.hasGCS());
		assertFalse(subSegment.hasLCS());
		assertTrue(subSegment.isGlobal());
		assertFalse(subSegment.isLocal());
		assertTrue(subSegment.isReassignable());
		assertFalse(subSegment.isPersistent());
		assertEquals(subSegment.getLiteral(), "free");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		subSegment = new XRI3SubSegment("+!16");
		assertTrue(subSegment.hasGCS());
		assertTrue(subSegment.hasLCS());
		assertTrue(subSegment.isGlobal());
		assertFalse(subSegment.isLocal());
		assertFalse(subSegment.isReassignable());
		assertTrue(subSegment.isPersistent());
		assertEquals(subSegment.getLiteral(), "16");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		subSegment = new XRI3SubSegment("!canonical");
		assertFalse(subSegment.hasGCS());
		assertTrue(subSegment.hasLCS());
		assertFalse(subSegment.isGlobal());
		assertTrue(subSegment.isLocal());
		assertFalse(subSegment.isReassignable());
		assertTrue(subSegment.isPersistent());
		assertEquals(subSegment.getLiteral(), "canonical");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		subSegment = new XRI3SubSegment("+(@free*earth*moon)");
		assertEquals(subSegment.getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(subSegment.getLCS(), null);
		assertFalse(subSegment.hasLiteral());
		assertTrue(subSegment.hasXRef());
		assertEquals(subSegment.getXRef(), "(@free*earth*moon)");
		assertTrue(subSegment.getXRef().hasXRIReference());
		assertFalse(subSegment.getXRef().hasIRI());
		assertEquals(subSegment.getXRef().getXRIReference(), "@free*earth*moon");
		assertEquals(subSegment.getXRef().getXRIReference().getAuthority().getNumSubSegments(), 3);
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		subSegment = new XRI3SubSegment("+(http://schemas.xmlsoap.org)");
		assertEquals(subSegment.getGCS(), XRI3Constants.GCS_PLUS);
		assertEquals(subSegment.getLCS(), null);
		assertFalse(subSegment.hasLiteral());
		assertTrue(subSegment.hasXRef());
		assertEquals(subSegment.getXRef(), "(http://schemas.xmlsoap.org)");
		assertFalse(subSegment.getXRef().hasXRIReference());
		assertTrue(subSegment.getXRef().hasIRI());
		assertEquals(subSegment.getXRef().getIRI(), "http://schemas.xmlsoap.org");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
	}
}
