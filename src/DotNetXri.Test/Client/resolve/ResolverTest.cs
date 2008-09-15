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
using java.util.ArrayList;

using junit.framework.Test;
using junit.framework.TestCase;
using junit.framework.TestSuite;
using junit.textui.TestRunner;

using org.openxri.XRI;
using org.openxri.resolve.exception.PartialResolutionException;
using org.openxri.xml.CanonicalID;
using org.openxri.xml.SEPElement;
using org.openxri.xml.Service;
using org.openxri.xml.Status;
using org.openxri.xml.Tags;
using org.openxri.xml.XRD;
using org.openxri.xml.XRDS;


public class ResolverTest
	:TestCase
{
	public static void main(String[] oArgs)
	{
		// Pass control to the non-graphical test runner
		TestRunner.run(suite());
	}
    
    
    public static Test suite() {
		return new TestSuite(ResolverTest.class);
	}

    
	public void testResolve() {
		String qxri;
		
		try {
			Resolver resolver = setupResolver();
			qxri = "@xrid*test*freeid*null-provider";
			qxri = "=les.chasen";
			qxri = "=kermit*\u65e5\u672c\u7248";
			XRI xri = XRI.fromIRINormalForm(qxri);
			
			ResolverFlags flags = new ResolverFlags();
			flags.setUric(true);
			flags.setHttps(false);
			resolver.resolveSEPToXRD(xri, null, null, flags, new ResolverState());
			
			
			ArrayList uris = resolver.resolveSEPToURIList(qxri, new TrustType(), null, "text/xml", true);


		}
		catch (Exception e) {
			e.printStackTrace();
			fail("Not expecting an exception here: " + e);
		}
	}
	
	public void testCID() {
		String qxri;
		
		try {
			Resolver resolver = setupResolver();
			qxri = "@xrid*test*live.unit.tests*0003-badcid*kid";			
			XRI xri = XRI.fromIRINormalForm(qxri);
			
			ResolverFlags flags = new ResolverFlags();
			XRDS xrds = resolver.resolveAuthToXRDS(xri, flags, new ResolverState());
			// Logger.Info(xrds);

			assertTrue("Expected 5 XRDs", xrds.getNumChildren() == 5);
			assertTrue("subseg[3] should be *0003-badcid", xrds.getDescriptorAt(3).getQuery().Equals("*0003-badcid"));
			
			Status s3 = xrds.getDescriptorAt(3).getStatus();
			assertTrue("subseg[3].status.cid should be 'failed'", s3.getCID().Equals("failed"));
			
			Status s4 = xrds.getDescriptorAt(4).getStatus();
			assertTrue("subseg[4].status.cid should be 'failed'", s4.getCID().Equals("failed"));

			
			
			qxri = "@xrid*test*live.unit.tests*0001-simple";
			xri = XRI.fromIRINormalForm(qxri);
			xrds = resolver.resolveAuthToXRDS(xri, flags, new ResolverState());
			assertTrue("Expected 4 XRDs", xrds.getNumChildren() == 4);
			XRD xrd3 = xrds.getDescriptorAt(3);
			assertTrue("subseg[3] should be *0001-simple", xrd3.getQuery().Equals("*0001-simple"));
			assertTrue("subseg[3] should be CID verified", xrd3.getStatus().getCID().Equals(Status.CID_VERIFIED));
			assertTrue("subseg[3] should be CEID verified", xrd3.getStatus().getCEID().Equals(Status.CID_VERIFIED));
		}
		catch (Exception e) {
			fail("Not expecting an exception here: " + e);
			e.printStackTrace();
		}
	}

	public void testLiveRef() {
		String qxri;
		try {
			Resolver resolver = setupResolver();
			qxri = "@xrid*test*live.unit.tests*0002-ref";
			XRI xri = XRI.fromIRINormalForm(qxri);
			
			ResolverFlags flags = new ResolverFlags();
			XRDS xrds = resolver.resolveAuthToXRDS(xri, flags, new ResolverState());
			assertTrue("There should be 5 child elements", xrds.getNumChildren() == 5);
			assertTrue("The last child should be an XRDS element because it followed a Ref", xrds.isXRDSAt(4));
		}
		catch (PartialResolutionException e) {
			fail("Not expecting PRE. PartialXRDS=" + e.getPartialXRDS());
		}
		catch (Exception e) {
			fail("Not expecting an exception here: " + e);
		}
	}
	
	public void testLiveRedirect() {
		String qxri = "@xrid*test*live.unit.tests*0004-redirect/(+blog)";
		try {
			Resolver resolver = setupResolver();
			XRI xri = XRI.fromIRINormalForm(qxri);
			
			ResolverFlags flags = new ResolverFlags();
			XRDS xrds = resolver.resolveSEPToXRDS(xri, null, null, flags, new ResolverState());
			Logger.Info(xrds);
			assertTrue("There should be 8 child elements", xrds.getNumChildren() == 8);
			assertTrue("The first child should be an XRD element", xrds.isXRDAt(0));
			assertTrue("The second child should be an XRD element", xrds.isXRDAt(1));
			assertTrue("The third child should be an XRD element", xrds.isXRDAt(2));
			assertTrue("The fourth child should be an XRD element", xrds.isXRDAt(3));

			assertTrue("The fifth child should be an XRDS element because it followed a Redirect", xrds.isXRDSAt(4));
			XRDS redirXRDS = xrds.getXRDSAt(4);
			assertTrue("Wrong redirect followed in the fifth child", redirXRDS.getRedirect().Equals("http://auth.xrid.net/!330/"));
			
			assertTrue("The fifth child should have 2 children", redirXRDS.getNumChildren() == 2);
			assertTrue("The fifth child's first child should be an XRD", redirXRDS.isXRDAt(0));
			assertTrue("The fifth child's second child should be an XRDS", redirXRDS.isXRDSAt(1));
			redirXRDS = redirXRDS.getXRDSAt(1);
			assertTrue("Wrong redirect followed in the fifth child's second child", redirXRDS.getRedirect().Equals("http://does.not.exist/"));
			assertFalse("Fifth child should have failed", redirXRDS.getFinalXRD().getStatusCode().Equals(Status.SUCCESS));
			
			assertTrue("The sixth child should be an XRDS element because it followed a Redirect", xrds.isXRDSAt(5));
			redirXRDS = xrds.getXRDSAt(5);
			assertTrue("Wrong redirect followed in the sixth child", redirXRDS.getRedirect().Equals("http://auth.xrid.net/!333/"));
			

			assertTrue("The seventh child should be an XRDS element because it followed a Redirect", xrds.isXRDSAt(6));
			redirXRDS = xrds.getXRDSAt(6);
			assertTrue("Wrong redirect followed on the seventh child", redirXRDS.getRedirect().Equals("http://auth.xrid.net/!331/"));
			assertTrue("Seventh child should have succeeded", redirXRDS.getFinalXRD().getStatusCode().Equals(Status.SUCCESS));
			
			assertTrue("The eighth child should be an XRDS element because it followed a Service-level Redirect", xrds.isXRDSAt(7));
			redirXRDS = xrds.getXRDSAt(7);
			assertTrue("Wrong redirect followed on the eighth child", redirXRDS.getRedirect().Equals("http://auth.xrid.net/!332/"));
			assertTrue("Eighth child should have succeeded", redirXRDS.getFinalXRD().getStatusCode().Equals(Status.SUCCESS));
			assertTrue("Should be one selected Service on eighth child", redirXRDS.getFinalXRD().getSelectedServices().getList().size() == 1);
			Service srv = (Service)redirXRDS.getFinalXRD().getSelectedServices().getList().get(0);
			assertTrue("In correct URI in selected service on eighth child", srv.getURIAt(0).getUriString().Equals("http://my.blog.com"));
		}
		catch (Exception e) {
			e.printStackTrace();
			fail("Not expecting exception: " + e);
		}
	}

	
	public void testConstructURI() {
		Resolver resolver = new Resolver();

		String qxri = "xri://@a*b*c/d/e?f=g";

		try {
			URI sepURI = new URI("http://example.com/hello");
			String result = resolver.constructURI(sepURI, "local",
					new XRI(qxri));
			assertTrue("Invalid constructed URI for append=local '" + result
					+ "'", result.Equals(sepURI.toString() + "/d/e?f=g"));
			result = resolver.constructURI(sepURI, "qxri", new XRI(qxri));
			assertTrue("Invalid constructed URI for append=qxri '" + result
					+ "'", result.Equals(sepURI.toString() + "@a*b*c/d/e?f=g"));
			Logger.Info("result = " + result);
		} catch (Exception oEx) {
			fail("Got wrong exception while trying to resolve IRI " + oEx);
		}
	}

	
	public static XRD createAuthRoot(String uri) {
		XRD xrd = new XRD();

		// construct an authority resolution service
		Service srv = new Service();
		TrustType tt = new TrustType(); // default trust type
		String authMediaType = Tags.CONTENT_TYPE_XRDS + ";"
				+ tt.getParameterPair();
		srv.addMediaType(authMediaType, null, false);
		srv.addType(Tags.SERVICE_AUTH_RES);
		srv.addURI(uri);

		// add it to the XRD
		xrd.addService(srv);

		return xrd;
	}

	public static Resolver setupResolver()
	{
	  // instantiate a Resolver obj
	  Resolver resolver = new Resolver();

	  // configure roots
	  XRD eqRoot = createAuthRoot("http://equal.xri.net/");
	  eqRoot.setCanonicalID(new CanonicalID("="));
	  Status eqRootStatus = new Status(Status.SUCCESS);
	  eqRootStatus.setCID(Status.CID_VERIFIED);
	  eqRoot.setStatus(eqRootStatus);
	  resolver.setAuthority("=", eqRoot);

	  XRD atRoot = createAuthRoot("http://at.xri.net/");
	  atRoot.setCanonicalID(new CanonicalID("@"));
	  Status atRootStatus = new Status(Status.SUCCESS);
	  atRootStatus.setCID(Status.CID_VERIFIED);
	  atRoot.setStatus(atRootStatus);
	  resolver.setAuthority("@", atRoot);

	  return resolver;
	}


}
}