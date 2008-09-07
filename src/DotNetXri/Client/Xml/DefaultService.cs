namespace DotNetXri.Client.Xml {

	using java.net.URI;
	using java.net.URISyntaxException;
	using java.util.List;
	using java.util.Vector;

	/**
	* This is a service that matches a resolution request without service type, media type and path.
	* @author =peacekeeper
	*/
	public class DefaultService : Service {

		/**
		* Constructs a new default service endpoint for use in an authority. Nothing will be appended to them.
		* @param pages - The default URI(s).
		*/
		public DefaultService(URI[] pages, String providerID) {



			/*
			* According to the Contact Service specification, the ProviderID of the Contact Service
			* SHOULD be set to the global i-number of the I-Broker.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* Set up default matching.
			*/

			this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, Boolean.FALSE));
			this.addType(new SEPType(null, SEPType.MATCH_ATTR_DEFAULT, Boolean.FALSE));
			this.addPath(new SEPPath(null, SEPPath.MATCH_ATTR_DEFAULT, Boolean.FALSE));

			/*
			* These are the default URIs.
			*/
			for (int i = 0; i < pages.length; i++) {

				URI page = pages[i];

				try {

					this.addURI(new SEPUri(page.toString(), null, SEPUri.APPEND_NONE));
				} catch (URISyntaxException ex) {

					continue;
				}
			}
		}

		public DefaultService(URI page, String providerID) {

			this(new URI[] { page }, providerID);
		}

		public DefaultService(URI[] pages) {

			this(pages, null);
		}

		public DefaultService(URI page) {

			this(new URI[] { page }, null);
		}

		public static bool isInstance(Service service) {

			if (service is DefaultService) return (true);

			bool mediaTypeNull = false, serviceTypeNull = false, pathNull = false;

			List mediaTypes = service.getMediaTypes();
			List serviceTypes = service.getTypes();
			List paths = service.getPaths();

			for (int i = 0; i < mediaTypes.size(); i++) {

				SEPMediaType mediaType = (SEPMediaType)mediaTypes.get(i);

				if (mediaType.getMatch().equals(SEPMediaType.MATCH_ATTR_NULL)) mediaTypeNull = true;
			}

			for (int i = 0; i < serviceTypes.size(); i++) {

				SEPType serviceType = (SEPType)serviceTypes.get(i);

				if (serviceType.getMatch().equals(SEPType.MATCH_ATTR_NULL)) serviceTypeNull = true;
			}

			for (int i = 0; i < paths.size(); i++) {

				SEPPath path = (SEPPath)paths.get(i);

				if (path.getMatch().equals(SEPMediaType.MATCH_ATTR_NULL)) pathNull = true;
			}

			return (mediaTypeNull && serviceTypeNull && pathNull);
		}
	}
}