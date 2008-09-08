namespace DotNetXri.Client.Xml {

	using java.net.Uri;
	using java.net.URISyntaxException;
	using java.util.List;

	/**
	* This is a Forwarding Service as described by "Forwarding Service v1.0 Working Draft 03, 7 September 2006
	* All constructors produce a service endpoint that conforms to the specification.
	* It is currently not in the scope of OpenXRI to implement the actual forwarding for this service.
	* The RECOMMENDED third-level DNS name for hosting a Forwarding Service is 'forwarding',
	* e.g. 'forwarding.my-i-broker.com'
	* @author =peacekeeper
	*/
	public class ForwardingService : Service {

		public const string SERVICE_TYPE = "xri://+i-service*(+forwarding)*($v*1.0)";
		public const string INDEX_PATH = "(+index)";

		public const string[] RECOMMENDED_PERSONAL_DICTIONARY = { "+blog", "+call", "+chat", "+email", "+home", "+links", "+photos", "+resume", "+songs", "+work" };

		public const string[] RECOMMENDED_ORGANIZATIONAL_DICTIONARY = { "+about", "+account", "+blog", "+call", "+chat", "+customer.service", "+email", "+home", "+investor.relations", "+links" };

		/**
		* Constructs a new Forwarding Service endpoint for use in an authority.
		* @param forwardingPages - The Uri(s) where forwarding (HTTP redirection) is implemented.
		* @param providerID - The global i-number of the I-Broker providing this Forwarding Service.
		* @param makeDefault - Whether to make the Forwarding Service the default service.
		* @param useIndexPath - Whether the Forwarding Service implementation responds to the OPTIONAL (+index) path.
		*/
		public ForwardingService(Uri[] forwardingPages, string providerID, bool makeDefault, bool useIndexPath) {



			/*
			* According to the Forwarding Service specification, the ProviderID of the Forwarding Service 
			* SHOULD be set to the global i-number of the I-Broker.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* Set paths.
			*/
			if (useIndexPath) this.addPath(new SEPPath(INDEX_PATH, null, Boolean.TRUE));
			this.addPath(new SEPPath(null, SEPElement.MATCH_ATTR_NON_NULL, null));
			if (makeDefault) this.addPath(new SEPPath(null, SEPElement.MATCH_ATTR_NULL, null));

			/*
			* Set service types.
			*/
			this.addType(new SEPType(SERVICE_TYPE, null, Boolean.TRUE));
			this.addType(new SEPType(null, SEPElement.MATCH_ATTR_NULL, null));

			/*
			* Set media types.
			*/
			this.addMediaType(new SEPMediaType(null, SEPElement.MATCH_ATTR_DEFAULT, null));

			/*
			* These are the URIs where the Forwarding Service is implemented. The QXRI will be appended.
			* It is currently not in the scope of OpenXRI to implement the actual forwarding (HTTP redirects).
			*/
			for (int i = 0; i < forwardingPages.length; i++) {

				Uri forwardingPage = forwardingPages[i];

				try {

					this.addURI(new SEPUri(forwardingPage.ToString(), null, SEPUri.APPEND_QXRI));
				} catch (URISyntaxException ex) {

					continue;
				}
			}
		}

		public ForwardingService(Uri forwardingPage, string providerID, bool makeDefault, bool useIndexPath) {

			this(new Uri[] { forwardingPage }, providerID, makeDefault, useIndexPath);
		}

		public ForwardingService(Uri[] forwardingPages, string providerID, bool makeDefault) {

			this(forwardingPages, providerID, makeDefault, true);
		}

		public ForwardingService(Uri forwardingPage, string providerID, bool makeDefault) {

			this(new Uri[] { forwardingPage }, providerID, makeDefault, true);
		}

		public ForwardingService(Uri[] forwardingPages, string providerID) {

			this(forwardingPages, providerID, true, true);
		}

		public ForwardingService(Uri forwardingPage, string providerID) {

			this(new Uri[] { forwardingPage }, providerID, true, true);
		}

		public ForwardingService(Uri[] forwardingPages) {

			this(forwardingPages, null, true, true);
		}

		public ForwardingService(Uri forwardingPage) {

			this(new Uri[] { forwardingPage }, null, true, true);
		}

		public static bool isInstance(Service service) {

			if (service is ForwardingService) return (true);

			List serviceTypes = service.getTypes();

			for (int i = 0; i < serviceTypes.size(); i++) {

				SEPType serviceType = (SEPType)serviceTypes.get(i);

				if (SERVICE_TYPE.Equals(serviceType.getValue())) return (true);
			}

			return (false);
		}
	}
}