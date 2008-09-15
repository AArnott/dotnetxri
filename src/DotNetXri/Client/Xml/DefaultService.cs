namespace DotNetXri.Client.Xml {

	//using java.net.Uri;
	//using java.net.UriFormatException;
	//using java.util.ArrayList;
	//using java.util.ArrayList;
	using System;

	/**
	* This is a service that matches a resolution request without service type, media type and path.
	* @author =peacekeeper
	*/
	public class DefaultService : Service {

		/**
		* Constructs a new default service endpoint for use in an authority. Nothing will be appended to them.
		* @param pages - The default Uri(s).
		*/
		public DefaultService(Uri[] pages, string providerID) {



			/*
			* According to the Contact Service specification, the ProviderID of the Contact Service
			* SHOULD be set to the global i-number of the I-Broker.
			*/
			if (providerID != null) this.setProviderId(providerID);

			/*
			* Set up default matching.
			*/

			this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, false));
			this.addType(new SEPType(null, SEPType.MATCH_ATTR_DEFAULT, false));
			this.addPath(new SEPPath(null, SEPPath.MATCH_ATTR_DEFAULT, false));

			/*
			* These are the default URIs.
			*/
			for (int i = 0; i < pages.Length; i++) {

				Uri page = pages[i];

				try {

					this.addURI(new SEPUri(page.ToString(), null, SEPUri.APPEND_NONE));
				} catch (UriFormatException ex) {

					continue;
				}
			}
		}

		public DefaultService(Uri page, string providerID) {

			this(new Uri[] { page }, providerID);
		}

		public DefaultService(Uri[] pages) {

			this(pages, null);
		}

		public DefaultService(Uri page) {

			this(new Uri[] { page }, null);
		}

		public static bool isInstance(Service service) {

			if (service is DefaultService) return (true);

			bool mediaTypeNull = false, serviceTypeNull = false, pathNull = false;

			ArrayList mediaTypes = service.getMediaTypes();
			ArrayList serviceTypes = service.getTypes();
			ArrayList paths = service.getPaths();

			for (int i = 0; i < mediaTypes.size(); i++) {

				SEPMediaType mediaType = (SEPMediaType)mediaTypes[i];

				if (mediaType.getMatch().Equals(SEPMediaType.MATCH_ATTR_NULL)) mediaTypeNull = true;
			}

			for (int i = 0; i < serviceTypes.size(); i++) {

				SEPType serviceType = (SEPType)serviceTypes[i];

				if (serviceType.getMatch().Equals(SEPType.MATCH_ATTR_NULL)) serviceTypeNull = true;
			}

			for (int i = 0; i < paths.size(); i++) {

				SEPPath path = (SEPPath)paths[i];

				if (path.getMatch().Equals(SEPMediaType.MATCH_ATTR_NULL)) pathNull = true;
			}

			return (mediaTypeNull && serviceTypeNull && pathNull);
		}
	}
}