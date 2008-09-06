package org.openxri.xri3;

using junit.framework.TestCase;

using org.openxri.xri3.impl.XRI3;

public class INameTest :TestCase {

	public INameTest(String name) :base(name) {
	}

	public void testINames() throws Exception {

		long time = System.currentTimeMillis();

		XRI xri;

		xri = new XRI3("=markus");
		assertTrue(xri.isIName());
		assertFalse(xri.isReserved());
		Logger.Info(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("@aero");
		assertTrue(xri.isIName());
		assertTrue(xri.isReserved());
		Logger.Info(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=xri");
		assertTrue(xri.isIName());
		assertTrue(xri.isReserved());
		Logger.Info(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname.this.iname.is.too.long.and.therefore.is.not.a.valid.iname");
		assertFalse(xri.isIName());
		assertFalse(xri.isReserved());
		Logger.Info(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=invalid..iname");
		assertFalse(xri.isIName());
		assertFalse(xri.isReserved());
		Logger.Info(Long.toString(System.currentTimeMillis() - time) + " ms");

		xri = new XRI3("=invalid--iname");
		assertFalse(xri.isIName());
		assertFalse(xri.isReserved());
		Logger.Info(Long.toString(System.currentTimeMillis() - time) + " ms");
	}
}
