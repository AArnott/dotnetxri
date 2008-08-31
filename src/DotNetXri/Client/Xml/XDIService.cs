package org.openxri.xml;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;

/**
 * This is an XDI Service.
 * @author =peacekeeper
 */
public class XDIService extends Service {

	public static final String SERVICE_TYPE = "xri://$xdi!($v!1)";
	public static final String SERVICE_PATH = "($context)!($xdi)!($card)!($v!1) ";

	/**
	 * Constructs a new XDI Service endpoint for use in an authority.
	 * @param providerID - The global i-number of the provider of this service.
	 */
	public XDIService(URI[] uris, String providerID) {
		
		super();
		
		/*
		 * The ProviderID of the XDI Service 
		 * SHOULD be set to the global i-number of the provider.
		 */
		if (providerID != null) this.setProviderId(providerID);
		
		/*
		 * This setting is REQUIRED to establish the XDI Service.
		 */
		this.addType(new SEPType(SERVICE_TYPE, null, Boolean.TRUE));
		
		/*
		 * This setting is REQUIRED to establish the XDI Service.
		 */
		this.addPath(new SEPPath(SERVICE_PATH, null, Boolean.TRUE));

		/*
		 * Setting a media type to default
		 * is not strictly necessary, since this setting is implied anyway, if no other
		 * media type is specified. On the other hand, it can't really harm.
		 */
		this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, null));

		/*
		 * These are the URIs where the XDI Service is implemented. Nothing will be appended.
		 */
		for (int i=0; i<uris.length; i++) {

			URI uri = uris[i];
			
			try {

				this.addURI(new SEPUri(uri.toString(), null, SEPUri.APPEND_NONE));
			} catch (URISyntaxException ex) {
				
				continue;
			}
		}
	}

	public XDIService(URI uri, String providerID) {
		
		this(new URI[] { uri }, providerID);
	}

	public XDIService(URI[] uris) {
		
		this(uris, null);
	}

	public XDIService(URI uri) {
		
		this(new URI[] { uri }, null);
	}

	/**
	 * Constructs a new XDI Service endpoint for use in an authority.
	 * @param providerID - The global i-number of the provider of this service.
	 */
	public XDIService(Ref[] xdiRefs, String providerID) {
		
		super();
		
		/*
		 * The ProviderID of the XDI Service 
		 * SHOULD be set to the global i-number of the provider.
		 */
		if (providerID != null) this.setProviderId(providerID);
		
		/*
		 * This setting is REQUIRED to establish the XDI Service.
		 */
		this.addType(new SEPType(SERVICE_TYPE, null, Boolean.TRUE));
		
		/*
		 * This setting is REQUIRED to establish the XDI Service.
		 */
		this.addPath(new SEPPath(SERVICE_PATH, null, Boolean.TRUE));

		/*
		 * Setting a media type to default
		 * is not strictly necessary, since this setting is implied anyway, if no other
		 * media type is specified. On the other hand, it can't really harm.
		 */
		this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, null));

		/*
		 * These are the Refs where the XDI Service is implemented. Nothing will be appended.
		 */
		for (int i=0; i<xdiRefs.length; i++) {

			Ref xdiRef = xdiRefs[i];
			
			this.addRef(xdiRef);
		}
	}

	public XDIService(Ref xdiRef, String providerID) {
		
		this(new Ref[] { xdiRef }, providerID);
	}

	public XDIService(Ref[] xdiRefs) {
		
		this(xdiRefs, null);
	}

	public XDIService(Ref xdiRef) {
		
		this(new Ref[] { xdiRef }, null);
	}

	public static boolean isInstance(Service service) {
		
		if (service instanceof XDIService) return(true);
		
		List serviceTypes = service.getTypes();
		
		for (int i=0; i<serviceTypes.size(); i++) {
			
			SEPType serviceType = (SEPType) serviceTypes.get(i);
			
			if (SERVICE_TYPE.equals(serviceType.getValue())) return(true);
		}
		
		return(false);
	}
}
