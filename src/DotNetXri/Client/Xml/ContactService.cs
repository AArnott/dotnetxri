namespace DotNetXri.Client.Xml {

	using java.net.Uri;
	using java.net.URISyntaxException;
	using java.util.List;

	/**
	* This is a Contact Service as described by "Contact Service v1.0 Working Draft 05, 7 September 2006
	* All constructors produce a service endpoint that conforms to the specification.
	* It is currently not in the scope of OpenXRI to implement the actual contact page for this service.
	* The RECOMMENDED third-level DNS name for hosting a Contact Service is 'contact',
	* e.g. 'contact.my-i-broker.com'
	* @author =peacekeeper
	*/
	public class ContactService : Service {

		public const string SERVICE_TYPE = "xri://+i-service*(+contact)*($v*1.0)";
		public const string CONTACT_PATH = "(+contact)";

		/**
		* Constructs a new Contact Service endpoint for use in an authority.
		* @param contactPages - The Uri(s) where a contact page is implemented.
		* @param providerID - The global i-number of the I-Broker providing this Contact Service.
		* @param makeDefault - Whether to make the Contact Service the default service.
		*/
		public ContactService(Uri[] contactPages, string providerID, bool makeDefault) {



			/*
			* According to the Contact Service specification, the ProviderID of the Contact Service
			* SHOULD be set to the global i-number of the I-Broker.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* Set paths.
			*/
			this.addPath(new SEPPath(CONTACT_PATH, null, Boolean.TRUE));
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
			* These are the Uri where the Contact Service is implemented. The QXRI will be appended.
			* It is currently not in the scope of OpenXRI to implement the actual contact page.
			*/
			for (int i = 0; i < contactPages.length; i++) {

				Uri contactPage = contactPages[i];

				try {

					this.addURI(new SEPUri(contactPage.ToString(), null, SEPUri.APPEND_QXRI));
				} catch (URISyntaxException ex) {

					continue;
				}
			}
		}

		public ContactService(Uri contactPage, string providerID, bool makeDefault) {

			this(new Uri[] { contactPage }, providerID, makeDefault);
		}

		public ContactService(Uri[] contactPages, string providerID) {

			this(contactPages, providerID, true);
		}

		public ContactService(Uri contactPage, string providerID) {

			this(new Uri[] { contactPage }, providerID, true);
		}

		public ContactService(Uri[] contactPages) {

			this(contactPages, null, true);
		}

		public ContactService(Uri contactPage) {

			this(new Uri[] { contactPage }, null, true);
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