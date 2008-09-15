using System;
using System.Collections;
namespace DotNetXri.Client.Xml {

	//using java.net.Uri;
	//using java.net.URISyntaxException;
	//using java.util.List;

	/**
	* This is an Authentication Service as described by "Authentication Service v1.0 Working Draft 02, 7 September 2006
	* All constructors produce a service endpoint that conforms to the specification.
	* It is currently not in the scope of OpenXRI to implement the authentication for this service.
	* The RECOMMENDED third-level DNS name for hosting an Authentication Service is 'authn',
	* e.g. 'authn.my-i-broker.com'
	* It is REQUIRED that at least one HTTPS Uri is used with an Authentication Service.
	* @author =peacekeeper
	*/
	public class AuthenticationService : Service {

		public const string SERVICE_TYPE1 = "http://openid.net/signon/1.0";
		public const string SERVICE_TYPE2 = "http://specs.openid.net/auth/2.0/signon";
		public const string LOGIN_PATH = "(+login)";
		public const int? URI_PRIORITY_HTTPS = new int?(1);
		public const int? URI_PRIORITY_DEFAULT = new int?(2);

		/**
		* Constructs a new Authentication endpoint for use in an authority.
		* @param authenticationPages - The Uri(s) where OpenID authentication is implemented.
		* @param providerID - The global i-number of the I-Broker providing this Authentication Service.
		* @param _delegate - The identity to use for authenticating at the OpenID IdP; null if the XRI itself is to be used.
		* @param useLoginPath - Whether the Authentication Service implementation responds to the OPTIONAL (+login) path.
		*/
		public AuthenticationService(Uri[] authenticationPages, string providerID, string _delegate, bool useLoginPath) {
			/*
			* According to the Authentication Service specification, the ProviderID of the Authentication Service 
			* SHOULD be set to the global i-number of the I-Broker.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* According to the Authentication Service specification, this setting is REQUIRED to 
			* establish the Authentication Service.
			*/
			this.addType(new SEPType(SERVICE_TYPE1, null, true));
			this.addType(new SEPType(SERVICE_TYPE2, null, true));

			/*
			* According to the Authentication Service specification, setting a media type to default
			* is not strictly necessary, since this setting is implied anyway, if no other
			* media type is specified. On the other hand, it can't really harm.
			*/
			this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, null));

			/*
			* If the user already has an OpenID, we can point to it (delegation) instead of using the XRI itself
			* as an OpenID. In that case, the provided Uri(s) must point to the OpenID IdP which is authorative
			* for the delegated identity.
			*/
			if (_delegate != null) {

				this.addLocalID(new LocalID(_delegate));
			}

			/*
			* According to the Authentication Service specification, an Authentication Service can OPTIONALLY
			* responde to the path (+login).
			*/
			if (useLoginPath) {
				this.addPath(new SEPPath(LOGIN_PATH, null, true));
				this.addPath(new SEPPath(null, SEPPath.MATCH_ATTR_DEFAULT, null));
			}

			/*
			* These are the URIs where the Authentication Service is implemented. Nothing will be appended.
			* According to the Authentication Service specification, the HTTPS Uri(s) must have the
			* highest priority.
			* It is currently not in the scope of OpenXRI to implement the actual OpenID authentication.
			*/
			for (int i = 0; i < authenticationPages.Length; i++) {

				Uri authenticationPage = authenticationPages[i];

				try {
					int? priority;

					if (authenticationPage.Scheme != null &&
						authenticationPage.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) {
						priority = URI_PRIORITY_HTTPS;
					} else {
						priority = URI_PRIORITY_DEFAULT;
					}

					this.addURI(new SEPUri(authenticationPage.ToString(), priority, SEPUri.APPEND_NONE));
				} catch (UriFormatException) {
					continue;
				}
			}
		}

		public AuthenticationService(Uri authenticationPage, string providerID, string _delegate, bool useLoginPath) :
			this(new Uri[] { authenticationPage }, providerID, _delegate, useLoginPath) {
		}

		public AuthenticationService(Uri[] authenticationPages, string providerID, string _delegate) :
			this(authenticationPages, providerID, _delegate, true) {
		}

		public AuthenticationService(Uri authenticationPage, string providerID, string _delegate) :
			this(new Uri[] { authenticationPage }, providerID, _delegate, true) {
		}

		public AuthenticationService(Uri[] authenticationPages, string providerID) :
			this(authenticationPages, providerID, null, true) {
		}

		public AuthenticationService(Uri authenticationPage, string providerID) :
			this(new Uri[] { authenticationPage }, providerID, null, true) {
		}

		public AuthenticationService(Uri[] authenticationPages) :
			this(authenticationPages, null, null, true) {
		}

		public AuthenticationService(Uri authenticationPage) :
			this(new Uri[] { authenticationPage }, null, null, true) {
		}

		public static bool isInstance(Service service) {
			if (service is ForwardingService) return (true);

			ArrayList serviceTypes = service.getTypes();

			for (int i = 0; i < serviceTypes.Count; i++) {
				SEPType serviceType = (SEPType)serviceTypes[i];

				if (SERVICE_TYPE1.Equals(serviceType.getValue())) return (true);
				if (SERVICE_TYPE2.Equals(serviceType.getValue())) return (true);
			}

			return (false);
		}
	}
}