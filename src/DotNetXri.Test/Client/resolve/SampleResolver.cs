package org.openxri.resolve;

import java.util.ArrayList;

import org.openxri.resolve.*;
import org.openxri.resolve.exception.*;
import org.openxri.xml.*;

public class SampleResolver {

	public SampleResolver() {
		super();
		// TODO Auto-generated constructor stub
	}




	public static Resolver setupResolver()
	{
	  // instantiate a Resolver obj
	  Resolver resolver = new Resolver();

	  // configure roots
	  XRD eqRoot = createAuthRoot("http://equal.xri.net");
	  resolver.setAuthority("=", eqRoot);
	  XRD atRoot = createAuthRoot("http://at.xri.net");
	  resolver.setAuthority("@", atRoot);

	  return resolver;
	}

//	 creates an XRD obj that contains an authority resolution service endpoint with the given URI
	public static XRD createAuthRoot(String uri)
	{
	  XRD xrd = new XRD();

	  // construct an authority resolution service
	  Service srv = new Service();
	  TrustType tt = new TrustType(); // default trust type
	  String authMediaType = Tags.CONTENT_TYPE_XRDS + ";" + tt.getParameterPair();
	  srv.addMediaType(authMediaType, SEPElement.MATCH_ATTR_CONTENT, Boolean.FALSE);
	  srv.addType(Tags.SERVICE_AUTH_RES);
	  srv.addURI(uri);

	  // add it to the XRD
	  xrd.addService(srv);

	  return xrd;
	}


	public static void main (String[] args)
	{
	  Resolver resolver = setupResolver();

	  try {
	    // get the XRDS document for =foo with default trust type (none), following Refs if needed
	    XRDS xrds = resolver.resolveAuthToXRDS("@xrid*wil", new TrustType(), true);

	    XRD xrd = xrds.getFinalXRD();
	    
	    CanonicalID cid = xrd.getCanonicalidAt(0);
	    if (cid == null) {
	    	// no CID? raise error or throw exception here 
	    }
	    
	    System.out.println("CID=" + cid.getValue());
	    
	    // resolution completed successfully if we did not catch an exception
	    System.out.println("XRDS = " + xrds.toString());
	    
	    // select a service using a set of criteria 
	    ArrayList uris = resolver.resolveSEPToURIList("=foo/bar", new TrustType(), "xri://+some-type", "some/media-type", true);
	    for (int i = 0; i < uris.size(); i++) {
	    	System.out.println("Resolved URI[" + i + "] = " + uris.get(0).toString());
	    }
	  }
	  catch (PartialResolutionException prex)
	  {
	    // get the partially resolved results
	    XRDS errXRDS = prex.getPartialXRDS();
	    XRD errXRD = errXRDS.getFinalXRD();
	    Status stat = errXRD.getStatus();
	    String statusCode = (stat == null) ? "unknown" : stat.getCode();

	    // the resolution did not complete successfully
	    System.err.println("Resolution error code: " + statusCode);
	    System.err.println("Full error XRDS: " + errXRDS.toString());
	  }
	}
}
