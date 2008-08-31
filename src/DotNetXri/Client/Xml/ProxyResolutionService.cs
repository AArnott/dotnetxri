package org.openxri.xml;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;
import java.util.Vector;

import org.openxri.resolve.TrustType;

/**
 * This is Proxy Resolution Service.
 * @author =peacekeeper
 */
public class ProxyResolutionService extends Service {

	public static final String SERVICE_TYPE = "xri://$res*proxy*($v*2.0)";
	public static final String[] SERVICE_MEDIA_TYPES = new String[] {
		"application/xrds+xml", "application/xrd+xml", "text/uri-list" };

	public static final String TRUST_TYPE_SEPARATOR = ";";
	public static final String REFS_SEPARATOR = ";";
	public static final String SEP_SEPARATOR = ";";

	public static final Integer URI_PRIORITY_HTTPS = new Integer(1);
	public static final Integer URI_PRIORITY_DEFAULT = new Integer(2);

	/**
	 * Constructs a new Proxy Resolution endpoint.
	 * @param resolvers - The URI(s) where the proxy is implemented.
	 * @param providerID - The global i-number of the I-Broker providing this Proxy Resolution Service.
	 * @param trustType - The Trust Type supported by the proxy
	 * @param refs - Whether the proxy supports following references
	 * @param sep - Whether the proxy supports service endpoint selection
	 */
	public ProxyResolutionService(URI[] proxies, String providerID, TrustType trustType, Boolean refs, Boolean sep) {
		
		super();
		
		/*
		 * The ProviderID of a Proxy Resolution Service is OPTIONAL.
		 */
		if (providerID != null) this.setProviderId(providerID);
		
		/*
		 * This setting is REQUIRED. 
		 */
		this.addType(new SEPType(SERVICE_TYPE, null, Boolean.TRUE));
		
		/*
		 * This setting is REQUIRED. 
		 */

		for (int i=0; i<SERVICE_MEDIA_TYPES.length; i++) {

			String mediaType = SERVICE_MEDIA_TYPES[i];
			
			if (trustType != null && ! trustType.equals(TrustType.TRUST_NONE)) {
				
				mediaType += TRUST_TYPE_SEPARATOR + trustType.getParameterPair();
				if (refs != null) mediaType += REFS_SEPARATOR + refs;
				if (sep != null) mediaType += SEP_SEPARATOR + sep;
			}

			this.addMediaType(new SEPMediaType(mediaType, null, Boolean.FALSE));
		}

		/*
		 * These are the URIs where the Proxy Resolution Service is implemented.
		 */
		for (int i=0; i<proxies.length; i++) {

			URI resolver = proxies[i];
			
			try {

				Integer priority = resolver.getScheme().toLowerCase().equals("https") ? 
					URI_PRIORITY_HTTPS : URI_PRIORITY_DEFAULT;
				
				this.addURI(new SEPUri(resolver.toString(), priority, SEPUri.APPEND_NONE));
			} catch (URISyntaxException ex) {
				
				continue;
			}
		}
	}

	public ProxyResolutionService(URI proxy, String providerId, TrustType trustType, Boolean refs, Boolean sep) {
		
		this(new URI[] { proxy }, providerId, trustType, refs, sep);
	}

	public ProxyResolutionService(URI[] proxies, String providerId, TrustType trustType) {
		
		this(proxies, providerId, trustType, null, null);
	}

	public ProxyResolutionService(URI proxy, String providerId, TrustType trustType) {
		
		this(new URI[] { proxy }, providerId, trustType, null, null);
	}
	
	public ProxyResolutionService(URI[] proxies, String providerId) {
		
		this(proxies, providerId, null, null, null);
	}

	public ProxyResolutionService(URI proxy, String providerId) {
		
		this(new URI[] { proxy }, providerId, null, null, null);
	}
	
	public ProxyResolutionService(URI[] proxies) {
		
		this(proxies, null, null, null, null);
	}

	public ProxyResolutionService(URI proxy) {
		
		this(new URI[] { proxy }, null, null, null, null);
	}

	public static boolean isInstance(Service service) {
		
		if (service instanceof ForwardingService) return(true);
		
		List serviceTypes = service.getTypes();
		
		for (int i=0; i<serviceTypes.size(); i++) {
			
			SEPType serviceType = (SEPType) serviceTypes.get(i);
			
			if (SERVICE_TYPE.equals(serviceType.getValue())) return(true);
		}
		
		return(false);
	}
}
