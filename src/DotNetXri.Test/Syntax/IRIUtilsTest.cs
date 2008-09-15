package org.openxri;

using java.io.UnsupportedEncodingException;

using junit.framework.TestCase;

public class IRIUtilsTest :TestCase
{

	/*
	 * Test method for 'org.openxri.IRIUtils.IRItoXRI(String)'
	 */
	public void testIRItoXRI() throws UnsupportedEncodingException
	{
		String r;
		r = IRIUtils.IRItoXRI("http://xri.net/");
		assertTrue(r.Equals("http://xri.net/"));

		r = IRIUtils.IRItoXRI("http://xri.net/@foo%25bar");
		assertTrue(r.Equals("http://xri.net/@foo%bar"));
		
		// transform the result once again, should not change
		r = IRIUtils.IRItoXRI(r);
		assertTrue(r.Equals("http://xri.net/@foo%bar"));
		
		String u = "=%E6%97%A0%E8%81%8A";
		String i = IRIUtils.URItoIRI(u);
		assertTrue(IRIUtils.IRItoURI(i).Equals(u));
	}

	/*
	 * Test method for 'org.openxri.IRIUtils.URItoIRI(String)'
	 */
	public void testURItoIRI()
	{
		String r;
		
		try {
			r = IRIUtils.URItoIRI("");
			assertTrue(r.length() == 0);

			r = IRIUtils.URItoIRI("http://xri.net/");
			assertTrue(r.Equals("http://xri.net/"));

			r = IRIUtils.URItoIRI("http://www.example.org/%44%c3%bCrst");
			assertTrue(r.Equals("http://www.example.org/D\u00FCrst"));
			
			r = IRIUtils.URItoIRI("http://r%C3%a9sum%c3%A9.example.org");
			assertTrue(r.Equals("http://r\u00E9sum\u00E9.example.org"));

			r = IRIUtils.URItoIRI("xri://@example*(http:%2F%2Fexample.org%2F)/");
			assertTrue(r.Equals("xri://@example*(http://example.org/)/"));
			
			// %-encoded BiDi - should not be converted
			r = IRIUtils.URItoIRI("http://example.org/%E2%80%AA-blah");
			assertTrue(r.Equals("http://example.org/%E2%80%AA-blah"));
			
			// non-IRI
			r = IRIUtils.URItoIRI("http://www.example.org/D%FCrst");
			assertTrue(r.Equals("http://www.example.org/D%FCrst"));

			// Han
			r = IRIUtils.XRItoIRI("http://xri.net/=\u8B19", false);
			assertTrue(r.Equals("http://xri.net/=\u8B19"));
			r = IRIUtils.IRItoURI(r);
			assertTrue(r.Equals("http://xri.net/=%E8%AC%99"));
			
			// XRI
			XRI xri = new XRI("=\u8B19");
			assertTrue(xri.toURINormalForm().Equals("xri://=%E8%AC%99"));

			// invalid Percent encoding
			try {
				r = IRIUtils.URItoIRI("http://www.example.org/D%Frst");
				assertTrue("Expected exception here", false);				
			}
			catch (XRIParseException e) {
				// ok
			}
		}
		catch (UnsupportedEncodingException e) {
			e.printStackTrace();
			assertTrue("exception caught!", false);
		}
	}

}
