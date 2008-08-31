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
package org.openxri.resolve;

import java.io.ByteArrayInputStream;
import java.io.InputStream;
import java.net.URI;

import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;
import junit.textui.TestRunner;

import org.openxri.XRI;
import org.openxri.resolve.exception.PartialResolutionException;
import org.openxri.resolve.exception.XRIResolutionException;
import org.openxri.xml.Ref;
import org.openxri.xml.XRD;
import org.openxri.xml.XRDS;


/*
********************************************************************************
* Class: IRITest
********************************************************************************
*/ /**
* This class simulates IRI Authority Resolution
* @author =chetan
*/
public class IRITest
    extends TestCase
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
        return new TestSuite(IRITest.class);

    } // suite()

    /*
    ****************************************************************************
    * Class: IRIResolver
    ****************************************************************************
    */ /**
    *
    */
    class IRIResolver
        extends Resolver
    {

        protected InputStream getDataFromURI(URI oURI, String query, ResolverFlags flags, ResolverState state)
            throws XRIResolutionException
        {
            // the path better be empty
            if ((oURI.getPath() != null) && (oURI.getPath().length() > 0))
                return null;

            XRD oDesc = new XRD();
            XRI oExternal = new XRI("xri://@foo");
            oDesc.addRef(new Ref(oExternal.toString()));
            XRDS oDescs = new XRDS();
            oDescs.add(oDesc);
            
			state.pushResolved(oURI.toString(), flags.toString(), oDesc.toString(), oURI);

            return new ByteArrayInputStream(oDescs.toString().getBytes());

        }

    }


    public void testFakeIRI()
    {
        Resolver oResolver = new IRIResolver();
    	ResolverState state = new ResolverState();
        oResolver.setMaxHttpRedirects(0);
        try
        {
            XRD oDesc = oResolver.resolveAuthToXRD(
            		"xri://www.epok.net/!foo!bar", new TrustType(), true, state);
            assertTrue(
                "Failed to obtain XRD for IRI Authority",
                oDesc != null);
            assertTrue(
                "Failed to obtain correct XRD for IRI Authority",
                oDesc.getRefAt(0).getValue().equals(
                    "xri://@foo"));
        }
        catch (Exception oEx)
        {
        	oEx.printStackTrace();
        	if (oEx instanceof PartialResolutionException) {
        		System.err.println("Partial = " + ((PartialResolutionException)oEx).getPartialXRDS());
        	}
        	System.err.println("Resolver state = " + state);
            fail("Got exception while trying to resolve IRI " + oEx);
        }

        try
        {
            XRD oDesc = oResolver.resolveAuthToXRD(
            		"xri://www.epok.net/!foo!bar", new TrustType(TrustType.TRUST_SAML), true);
            assertTrue(
                "Failed to get exception for trusted IRI Authority resolution",
                false);
        }
        catch (XRIResolutionException oEx) {}
        catch (Exception oEx)
        {
            fail("Got wrong exception while trying to resolve IRI " + oEx);
        }

    } // testFakeIRI()

} // Class: IRITest
