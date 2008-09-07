namespace DotNetXri.Client.Xml {

	using java.net.URI;
	using java.net.URISyntaxException;
	using java.util.List;

	using org.openxri.resolve.TrustType;

	/**
	* This is an Authority Resolution 2.0 Service.
	* @author =peacekeeper
	*/
	public class AuthorityResolutionService : Service {

		public const String SERVICE_TYPE = "xri://$res*auth*($v*2.0)";
		public const String SERVICE_MEDIA_TYPE = "application/xrds+xml";
		public const String TRUST_TYPE_SEPARATOR = ";";

		public const Integer URI_PRIORITY_HTTPS = new Integer(1);
		public const Integer URI_PRIORITY_DEFAULT = new Integer(2);

		/**
		* Constructs a new Authority Resolution 2.0 endpoint.
		* @param resolvers - The URI(s) where the authority will be resolved.
		* @param providerID - The global i-number of the I-Broker providing this Authority Resolution Service.
		* @param append - The append attribute to use for the URIs.
		*/
		public AuthorityResolutionService(URI[] resolvers, String providerID, TrustType trustType, String append) {



			if (append == null) append = SEPUri.APPEND_NONE;

			/*
			* The ProviderID of an untrusted Authority Resolution Service is OPTIONAL, otherwise REQUIRED.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* This setting is REQUIRED. 
			*/
			this.addType(new SEPType(SERVICE_TYPE, null, Boolean.TRUE));

			/*
			* This setting is REQUIRED. 
			*/

			String mediaType = SERVICE_MEDIA_TYPE;
			if (trustType != null && !trustType.equals(TrustType.TRUST_NONE)) {

				mediaType += TRUST_TYPE_SEPARATOR + trustType.getParameterPair();
			}

			this.addMediaType(new SEPMediaType(mediaType, null, Boolean.FALSE));

			/*
			* These are the URIs where the Authority Resolution Service is implemented.
			*/
			for (int i = 0; i < resolvers.length; i++) {

				URI resolver = resolvers[i];

				try {

					Integer priority = resolver.getScheme().toLowerCase().equals("https") ?
							URI_PRIORITY_HTTPS : URI_PRIORITY_DEFAULT;

					this.addURI(new SEPUri(resolver.toString(), priority, append));
				} catch (URISyntaxException ex) {

					continue;
				}
			}
		}

		public AuthorityResolutionService(URI resolver, String providerId, TrustType trustType, String append) {

			this(new URI[] { resolver }, providerId, trustType, append);
		}

		public AuthorityResolutionService(URI[] resolvers, String providerId, TrustType trustType) {

			this(resolvers, providerId, trustType, null);
		}

		public AuthorityResolutionService(URI resolver, String providerId, TrustType trustType) {

			this(new URI[] { resolver }, providerId, trustType, null);
		}

		public AuthorityResolutionService(URI[] resolvers, String providerId) {

			this(resolvers, providerId, null, null);
		}

		public AuthorityResolutionService(URI resolver, String providerId) {

			this(new URI[] { resolver }, providerId, null, null);
		}

		public AuthorityResolutionService(URI[] resolvers) {

			this(resolvers, null, null, null);
		}

		public AuthorityResolutionService(URI resolver) {

			this(new URI[] { resolver }, null, null, null);
		}

		public static bool isInstance(Service service) {

			if (service is ForwardingService) return (true);

			List serviceTypes = service.getTypes();

			for (int i = 0; i < serviceTypes.size(); i++) {

				SEPType serviceType = (SEPType)serviceTypes.get(i);

				if (SERVICE_TYPE.equals(serviceType.getValue())) return (true);
			}

			return (false);
		}
	}
}