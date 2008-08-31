package org.openxri.xml;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;

/**
 * This is an Authentication Service as described by "Authentication Service v1.0 Working Draft 02, 7 September 2006
 * All constructors produce a service endpoint that conforms to the specification.
 * It is currently not in the scope of OpenXRI to implement the authentication for this service.
 * The RECOMMENDED third-level DNS name for hosting an Authentication Service is 'authn',
 * e.g. 'authn.my-i-broker.com'
 * It is REQUIRED that at least one HTTPS URI is used with an Authentication Service.
 * @author =peacekeeper
 */
public class AuthenticationService :org.openxri.xml.Service {

	public const String SERVICE_TYPE1 = "http://openid.net/signon/1.0";
	public const String SERVICE_TYPE2 = "http://specs.openid.net/auth/2.0/signon";
	public const String LOGIN_PATH = "(+login)";
	public static final Integer URI_PRIORITY_HTTPS = new Integer(1);
	public static final Integer URI_PRIORITY_DEFAULT = new Integer(2);

	/**
	 * Constructs a new Authentication endpoint for use in an authority.
	 * @param authenticationPages - The URI(s) where OpenID authentication is implemented.
	 * @param providerID - The global i-number of the I-Broker providing this Authentication Service.
	 * @param delegate - The identity to use for authenticating at the OpenID IdP; null if the XRI itself is to be used.
	 * @param useLoginPath - Whether the Authentication Service implementation responds to the OPTIONAL (+login) path.
	 */
	public AuthenticationService(URI[] authenticationPages, String providerID, String delegate, bool useLoginPath) {
		
		super();
		
		/*
		 * According to the Authentication Service specification, the ProviderID of the Authentication Service 
		 * SHOULD be set to the global i-number of the I-Broker.
		 */
		if (providerID != null) this.setProviderId(providerID);
		
		/*
		 * According to the Authentication Service specification, this setting is REQUIRED to 
		 * establish the Authentication Service.
		 */
		this.addType(new SEPType(SERVICE_TYPE1, null, Boolean.TRUE));
		this.addType(new SEPType(SERVICE_TYPE2, null, Boolean.TRUE));

		/*
		 * According to the Authentication Service specification, setting a media type to default
		 * is not strictly necessary, since this setting is implied anyway, if no other
		 * media type is specified. On the other hand, it can't really harm.
		 */
		this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, null));
		
		/*
		 * If the user already has an OpenID, we can point to it (delegation) instead of using the XRI itself
		 * as an OpenID. In that case, the provided URI(s) must point to the OpenID IdP which is authorative
		 * for the delegated identity.
		 */
		if (delegate != null) {
			
			this.addLocalID(new LocalID(delegate));
		}
		
		/*
		 * According to the Authentication Service specification, an Authentication Service can OPTIONALLY
		 * responde to the path (+login).
		 */
		if (useLoginPath) {

			this.addPath(new SEPPath(LOGIN_PATH, null, Boolean.TRUE));
			this.addPath(new SEPPath(null, SEPPath.MATCH_ATTR_DEFAULT, null));
		}

		/*
		 * These are the URIs where the Authentication Service is implemented. Nothing will be appended.
		 * According to the Authentication Service specification, the HTTPS URI(s) must have the
		 * highest priority.
		 * It is currently not in the scope of OpenXRI to implement the actual OpenID authentication.
		 */
		for (int i=0; i<authenticationPages.length; i++) {

			URI authenticationPage = authenticationPages[i];
			
			try {

				Integer priority;
				
				if (authenticationPage.getScheme() != null &&
					authenticationPage.getScheme().toLowerCase().equals("https")) {
					
					priority = URI_PRIORITY_HTTPS;
				} else {
					
					priority = URI_PRIORITY_DEFAULT;
				}
				
				this.addURI(new SEPUri(authenticationPage.toString(), priority, SEPUri.APPEND_NONE));
			} catch (URISyntaxException ex) {
				
				continue;
			}
		}
	}

	public AuthenticationService(URI authenticationPage, String providerID, String delegate, bool useLoginPath) {
		
		this(new URI[] { authenticationPage }, providerID, delegate, useLoginPath);
	}

	public AuthenticationService(URI[] authenticationPages, String providerID, String delegate) {
		
		this(authenticationPages, providerID, delegate, true);
	}

	public AuthenticationService(URI authenticationPage, String providerID, String delegate) {
		
		this(new URI[] { authenticationPage }, providerID, delegate, true);
	}

	public AuthenticationService(URI[] authenticationPages, String providerID) {
		
		this(authenticationPages, providerID, null, true);
	}

	public AuthenticationService(URI authenticationPage, String providerID) {
		
		this(new URI[] { authenticationPage }, providerID, null, true);
	}

	public AuthenticationService(URI[] authenticationPages) {
		
		this(authenticationPages, null, null, true);
	}

	public AuthenticationService(URI authenticationPage) {
		
		this(new URI[] { authenticationPage }, null, null, true);
	}

	public static bool isInstance(Service service) {
		
		if (service instanceof ForwardingService) return(true);
		
		List serviceTypes = service.getTypes();
		
		for (int i=0; i<serviceTypes.size(); i++) {
			
			SEPType serviceType = (SEPType) serviceTypes.get(i);
			
			if (SERVICE_TYPE1.equals(serviceType.getValue())) return(true);
			if (SERVICE_TYPE2.equals(serviceType.getValue())) return(true);
		}
		
		return(false);
	}
}
