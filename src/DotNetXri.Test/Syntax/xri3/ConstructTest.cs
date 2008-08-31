package org.openxri.xri3;

import junit.framework.TestCase;

import org.openxri.xri3.impl.XRI3;
import org.openxri.xri3.impl.XRI3Authority;
import org.openxri.xri3.impl.XRI3Constants;
import org.openxri.xri3.impl.XRI3Reference;
import org.openxri.xri3.impl.XRI3SubSegment;

public class ConstructTest extends TestCase {

	public ConstructTest(String name) {

		super(name);
	}

	public void testXRI3() throws Exception {

		long time = System.currentTimeMillis();

		Character character;
		String string;
		XRI xri1, xri2;
		XRIReference xriReference;
		XRIAuthority xriAuthority1, xriAuthority2;
		XRISubSegment xriSubSegment1, xriSubSegment2;

		xri1 = new XRI3("+name");
		xriReference = new XRI3Reference("+first");
		xri2 = new XRI3(xri1, xriReference);
		assertEquals(xri2, "+name+first");
		assertEquals(xri2.getAuthority().getNumSubSegments(), 2);
		assertEquals(xri2.getAuthority().getSubSegment(0), "+name");
		assertEquals(xri2.getAuthority().getSubSegment(0).getLiteral(), "name");
		assertEquals(xri2.getAuthority().getSubSegment(1), "+first");
		assertEquals(xri2.getAuthority().getSubSegment(1).getLiteral(), "first");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		character = new Character('=');
		string = new String("http://markus.openid.net");
		xri1 = new XRI3(character, string);
		assertEquals(xri1, "=(http://markus.openid.net)");
		assertEquals(xri1.getAuthority().getNumSubSegments(), 1);
		assertEquals(xri1.getAuthority().getSubSegment(0), "=(http://markus.openid.net)");
		assertEquals(xri1.getAuthority().getSubSegment(0).getGCS(), XRI3Constants.GCS_EQUALS);
		assertEquals(xri1.getAuthority().getSubSegment(0).getLCS(), null);
		assertFalse(xri1.getAuthority().getSubSegment(0).hasLiteral());
		assertTrue(xri1.getAuthority().getSubSegment(0).hasXRef());
		assertFalse(xri1.getAuthority().getSubSegment(0).getXRef().hasXRIReference());
		assertTrue(xri1.getAuthority().getSubSegment(0).getXRef().hasIRI());
		assertEquals(xri1.getAuthority().getSubSegment(0).getXRef().getXRIReference(), null);
		assertEquals(xri1.getAuthority().getSubSegment(0).getXRef().getIRI(), "http://markus.openid.net");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xriAuthority1 = new XRI3Authority("=!B7BD.2A1D.1040.58CD");
		xriSubSegment1 = new XRI3SubSegment("!2000");
		xriAuthority2 = new XRI3Authority(xriAuthority1, xriSubSegment1);
		assertEquals(xriAuthority2, "=!B7BD.2A1D.1040.58CD!2000");
		assertEquals(xriAuthority2.getNumSubSegments(), 2);
		assertEquals(xriAuthority2.getSubSegment(0), "=!B7BD.2A1D.1040.58CD");
		assertEquals(xriAuthority2.getSubSegment(0).getGCS(), XRI3Constants.GCS_EQUALS);
		assertEquals(xriAuthority2.getSubSegment(0).getLCS(), XRI3Constants.LCS_BANG);
		assertEquals(xriAuthority2.getSubSegment(0).getLiteral(), "B7BD.2A1D.1040.58CD");
		assertEquals(xriAuthority2.getSubSegment(1), "!2000");
		assertEquals(xriAuthority2.getSubSegment(1).getGCS(), null);
		assertEquals(xriAuthority2.getSubSegment(1).getLCS(), XRI3Constants.LCS_BANG);
		assertEquals(xriAuthority2.getSubSegment(1).getLiteral(), "2000");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		character = new Character('=');
		xriSubSegment1 = new XRI3SubSegment("!B7BD.2A1D.1040.58CD");
		xriSubSegment2 = new XRI3SubSegment(character, xriSubSegment1);
		assertEquals(xriSubSegment2, "=!B7BD.2A1D.1040.58CD");
		assertEquals(xriSubSegment2, "=!B7BD.2A1D.1040.58CD");
		assertEquals(xriSubSegment2.getGCS(), XRI3Constants.GCS_EQUALS);
		assertEquals(xriSubSegment2.getLCS(), XRI3Constants.LCS_BANG);
		assertEquals(xriSubSegment2.getLiteral(), "B7BD.2A1D.1040.58CD");
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
	}
}
