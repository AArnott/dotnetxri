package org.openxri.xri3;

import junit.framework.TestCase;

import org.openxri.xri3.impl.XRI3;

public class INameTest extends TestCase {

	public INameTest(String name) {

		super(name);
	}

	public void testINames() throws Exception {

		long time = System.currentTimeMillis();

		XRI xri;

		xri = new XRI3("=markus");
		assertTrue(xri.isIName());
		assertFalse(xri.isReserved());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("@aero");
		assertTrue(xri.isIName());
		assertTrue(xri.isReserved());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=xri");
		assertTrue(xri.isIName());
		assertTrue(xri.isReserved());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname");
		assertFalse(xri.isIName());
		assertFalse(xri.isReserved());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=invalid..iname");
		assertFalse(xri.isIName());
		assertFalse(xri.isReserved());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=invalid--iname");
		assertFalse(xri.isIName());
		assertFalse(xri.isReserved());
		System.out.println(Long.toString(System.currentTimeMillis() - time) + " ms");
	}
}
