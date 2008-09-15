/*
 * Copyright 2005 OpenXRI Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
namespace DotNetXri.Client.Resolve {


using java.io.ByteArrayInputStream;
using java.io.InputStream;
using java.net.URI;
using java.net.URISyntaxException;

using junit.framework.Test;
using junit.framework.TestCase;
using junit.framework.TestSuite;
using junit.textui.TestRunner;

using org.openxri.GCSAuthority;
using org.openxri.XRI;
using org.openxri.resolve.exception.PartialResolutionException;
using org.openxri.resolve.exception.XRIResolutionException;
using org.openxri.xml.Status;
using org.openxri.xml.XRD;
using org.openxri.xml.XRDS;


/*
********************************************************************************
* Class: ProxyTest
********************************************************************************
*/ /**
* This class simulates Proxied Resolution
* @author =chetan
*/
public class ProxyTest
    :TestCase
{
    /*
    ****************************************************************************
    * main()
    ****************************************************************************
    */ /**
    *
    */
    public static void main(String[] oArgs)
    {
        // Pass control to the non-graphical test runner
        TestRunner.run(suite());

    } // main()

    /*
    ****************************************************************************
    * suite()
    ****************************************************************************
    */ /**
    *
    */
    public static Test suite()
    {
        return new TestSuite(ProxyTest.class);

    } // suite()

    /*
    ****************************************************************************
    * Class: TestResolver
    ****************************************************************************
    */ /**
    *
    */
    class TestResolver
        :Resolver
    {
        /*
        ************************************************************************
        * getDataFromURI()
        ************************************************************************
        */ /**
        *
        */
        protected InputStream getDataFromURI(URI uri, String query, ResolverFlags flags, ResolverState state)
            throws XRIResolutionException
        {
            // if we got the nonexistent "command" return a not found
            if (uri.getPath().indexOf("!nonexistent") >= 0)
            {
                XRD xrd = new XRD();
                xrd.setQuery("!nonexistent");
                xrd.setStatus(new Status(Status.AUTH_RES_NOT_FOUND));
                XRDS xrds = new XRDS();
                xrds.add(xrd);
                return new ByteArrayInputStream(xrds.toString().getBytes());
            }

            XRDS oDescs = new XRDS();

            // just the same response always
            XRD oFoo = new XRD();
            oFoo.setQuery("!foo");
            oFoo.setStatus(new Status(Status.SUCCESS));
            oDescs.add(oFoo);

            XRD oBar = new XRD();
            oBar.setQuery("!bar");
            oBar.setStatus(new Status(Status.SUCCESS));
            oDescs.add(oBar);

            if (uri.getPath().indexOf("!baz") > 0) {
            	XRD baz = new XRD();
            	baz.setQuery("!baz");
            	baz.setStatus(new Status(Status.AUTH_RES_NOT_FOUND, "Not found"));
                oDescs.add(baz);
            }
            return new ByteArrayInputStream(oDescs.toString().getBytes());

        } // getDataFromURI()

    } // Class: TestResolver

    /*
    ****************************************************************************
    * testProxy()
    ****************************************************************************
    */ /**
    *
    */
    public void testProxy()
    {
        URI oProxyURI = null;
        try
        {
            oProxyURI = new URI("http://xri.epok.net/proxy");
        }
        catch (URISyntaxException e)
        {
            fail("Unexpected initialization error");
        }

        Resolver oResolver = new TestResolver();
        oResolver.setProxyURI(oProxyURI);
        oResolver.setMaxHttpRedirects(0);

        // should not work - not enough descriptors
        try
        {
            XRD oDesc = oResolver.resolveAuthToXRD("xri://@!foo!bar!baz", new TrustType(), true);
            assertFalse("Should get a PartialResolutionException", oDesc.getStatusCode().Equals(Status.SUCCESS));
        }
        catch (Exception e)
        {
        	if (e is PartialResolutionException) {
        		String stat1 = null, stat2 = null, stat3 = null;
            	PartialResolutionException pe = (PartialResolutionException)e;
            	System.err.println(pe.getPartialXRDS().toString());

            	assertTrue("Should contain 3 XRDs, got " + pe.getPartialXRDS().getNumChildren() + " instead",
            			pe.getPartialXRDS().getNumChildren() == 3);

        		try {
        			stat1 = pe.getPartialXRDS().getDescriptorAt(0).getStatusCode();
        			stat2 = pe.getPartialXRDS().getDescriptorAt(1).getStatusCode();
        			stat3 = pe.getPartialXRDS().getDescriptorAt(2).getStatusCode();
        		}
        		catch (Exception e1) {
        			assertTrue("Got exception: " + e1.getMessage(), false);
        		}
        		
            	assertTrue("First XRD should be successful", stat1.Equals(Status.SUCCESS));
            	assertTrue("Second XRD should be successful", stat2.Equals(Status.SUCCESS));
            	assertTrue("Third XRD should be a failure", stat3.Equals(Status.AUTH_RES_NOT_FOUND));
        	}
        	else {
        		fail("Got exception while trying to resolve via proxy " + e);
        		e.printStackTrace();
        	}
        }

        // should not work - too many descriptors
        /*** [wil] test disabled - proxy resolution currently does not validate the number of XRDs returned. 
        try
        {
            XRD oDesc = oResolver.resolveAuthToXRD("xri://@!foo", new TrustType(), true);
            assertFalse("Should get a failure code for too many XRDs", oDesc.getStatusCode().Equals(Status.SUCCESS));
        }
        catch (Exception oEx)
        {
            fail("Got exception while trying to resolve via proxy  " + oEx);
        }
        */

        // should work as (null response)
        try
        {
            XRD oDesc = oResolver.resolveAuthToXRD("xri://@!foo!nonexistent", new TrustType(), true);
            assertTrue(
                "Should not have obtained XRD from proxy",
                oDesc == null);
        }
        catch (Exception e)
        {
        	if (e is PartialResolutionException) {
        		PartialResolutionException pe = (PartialResolutionException)e;
        		String stat = null;
        		try {
        			stat = pe.getPartialXRDS().getDescriptorAt(0).getStatusCode();
        		}
        		catch (Exception e1) { assertTrue("got exception: " + e1.getMessage(), false); }
        		
        		assertTrue("First XRD should fail", stat.Equals(Status.AUTH_RES_NOT_FOUND));
        	}
        	else {
        		fail("Got exception while trying to resolve via proxy " + e);
        	}
        }

        // should work
        try
        {
            XRD oDesc = oResolver.resolveAuthToXRD("xri://@!foo!bar", new TrustType(), false);
            assertTrue(
                "Failed to obtain XRD from proxy", oDesc != null);
        }
        catch (Exception oEx)
        {
            fail("Got exception while trying to resolve via proxy " + oEx);
        }

    } // testProxy()

} // Class: ProxyTest
}