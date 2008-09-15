using System;
using DotNetXri.Client.Resolve;
using System.Collections;
namespace DotNetXri.Client.Xml {

	//using java.net.Uri;
	//using java.net.URISyntaxException;
	//using java.util.ArrayList;

	//using org.openxri.resolve.TrustType;

	/**
	* This is an Authority Resolution 2.0 Service.
	* @author =peacekeeper
	*/
	public class AuthorityResolutionService : Service {

		public const string SERVICE_TYPE = "xri://$res*auth*($v*2.0)";
		public const string SERVICE_MEDIA_TYPE = "application/xrds+xml";
		public const string TRUST_TYPE_SEPARATOR = ";";

		public const int? URI_PRIORITY_HTTPS = new int?(1);
		public const int? URI_PRIORITY_DEFAULT = new int?(2);

		/**
		* Constructs a new Authority Resolution 2.0 endpoint.
		* @param resolvers - The Uri(s) where the authority will be resolved.
		* @param providerID - The global i-number of the I-Broker providing this Authority Resolution Service.
		* @param append - The append attribute to use for the URIs.
		*/
		public AuthorityResolutionService(Uri[] resolvers, string providerID, TrustType trustType, string append) {



			if (append == null) append = SEPUri.APPEND_NONE;

			/*
			* The ProviderID of an untrusted Authority Resolution Service is OPTIONAL, otherwise REQUIRED.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* This setting is REQUIRED. 
			*/
			this.addType(new SEPType(SERVICE_TYPE, null, true));

			/*
			* This setting is REQUIRED. 
			*/

			string mediaType = SERVICE_MEDIA_TYPE;
			if (trustType != null && !trustType.Equals(TrustType.TRUST_NONE)) {

				mediaType += TRUST_TYPE_SEPARATOR + trustType.getParameterPair();
			}

			this.addMediaType(new SEPMediaType(mediaType, null, false));

			/*
			* These are the URIs where the Authority Resolution Service is implemented.
			*/
			for (int i = 0; i < resolvers.Length; i++) {

				Uri resolver = resolvers[i];

				try {

					int? priority = resolver.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ?
							URI_PRIORITY_HTTPS : URI_PRIORITY_DEFAULT;

					this.addURI(new SEPUri(resolver.ToString(), priority, append));
				} catch (UriFormatException) {

					continue;
				}
			}
		}

		public AuthorityResolutionService(Uri resolver, string providerId, TrustType trustType, string append) :
			this(new Uri[] { resolver }, providerId, trustType, append) {
		}

		public AuthorityResolutionService(Uri[] resolvers, string providerId, TrustType trustType) :
			this(resolvers, providerId, trustType, null) {
		}

		public AuthorityResolutionService(Uri resolver, string providerId, TrustType trustType) :
			this(new Uri[] { resolver }, providerId, trustType, null) {
		}

		public AuthorityResolutionService(Uri[] resolvers, string providerId) :
			this(resolvers, providerId, null, null) {
		}

		public AuthorityResolutionService(Uri resolver, string providerId) :
			this(new Uri[] { resolver }, providerId, null, null) {
		}

		public AuthorityResolutionService(Uri[] resolvers)
			: this(resolvers, null, null, null) {
		}

		public AuthorityResolutionService(Uri resolver)
			: this(new Uri[] { resolver }, null, null, null) {
		}

		public static bool isInstance(Service service) {

			if (service is ForwardingService) return (true);

			ArrayList serviceTypes = service.getTypes();

			for (int i = 0; i < serviceTypes.Count; i++) {

				SEPType serviceType = (SEPType)serviceTypes[i];

				if (SERVICE_TYPE.Equals(serviceType.getValue())) return (true);
			}

			return (false);
		}
	}
}