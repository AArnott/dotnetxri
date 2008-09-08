namespace DotNetXri.Client.Xml {

using java.net.Uri;
using java.net.URISyntaxException;
using java.util.List;
using java.util.ArrayList;

using org.openxri.resolve.TrustType;

/**
	* This is Proxy Resolution Service.
	* @author =peacekeeper
	*/
public class ProxyResolutionService : Service {

	public const string SERVICE_TYPE = "xri://$res*proxy*($v*2.0)";
	public const string[] SERVICE_MEDIA_TYPES = new string[] {
		"application/xrds+xml", "application/xrd+xml", "text/uri-list" };

	public const string TRUST_TYPE_SEPARATOR = ";";
	public const string REFS_SEPARATOR = ";";
	public const string SEP_SEPARATOR = ";";

	public const int? URI_PRIORITY_HTTPS = new int?(1);
	public const int? URI_PRIORITY_DEFAULT = new int?(2);

	/**
		* Constructs a new Proxy Resolution endpoint.
		* @param resolvers - The Uri(s) where the proxy is implemented.
		* @param providerID - The global i-number of the I-Broker providing this Proxy Resolution Service.
		* @param trustType - The Trust Type supported by the proxy
		* @param refs - Whether the proxy supports following references
		* @param sep - Whether the proxy supports service endpoint selection
		*/
	public ProxyResolutionService(Uri[] proxies, string providerID, TrustType trustType, Boolean refs, Boolean sep) {



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

		for (int i = 0; i < SERVICE_MEDIA_TYPES.length; i++) {

			string mediaType = SERVICE_MEDIA_TYPES[i];

			if (trustType != null && !trustType.Equals(TrustType.TRUST_NONE)) {

				mediaType += TRUST_TYPE_SEPARATOR + trustType.getParameterPair();
				if (refs != null) mediaType += REFS_SEPARATOR + refs;
				if (sep != null) mediaType += SEP_SEPARATOR + sep;
			}

			this.addMediaType(new SEPMediaType(mediaType, null, Boolean.FALSE));
		}

		/*
			* These are the URIs where the Proxy Resolution Service is implemented.
			*/
		for (int i = 0; i < proxies.length; i++) {

			Uri resolver = proxies[i];

			try {

				int? priority = resolver.getScheme().toLowerCase().Equals("https") ?
					URI_PRIORITY_HTTPS : URI_PRIORITY_DEFAULT;

				this.addURI(new SEPUri(resolver.ToString(), priority, SEPUri.APPEND_NONE));
			} catch (URISyntaxException ex) {

				continue;
			}
		}
	}

	public ProxyResolutionService(Uri proxy, string providerId, TrustType trustType, Boolean refs, Boolean sep) {

		this(new Uri[] { proxy }, providerId, trustType, refs, sep);
	}

	public ProxyResolutionService(Uri[] proxies, string providerId, TrustType trustType) {

		this(proxies, providerId, trustType, null, null);
	}

	public ProxyResolutionService(Uri proxy, string providerId, TrustType trustType) {

		this(new Uri[] { proxy }, providerId, trustType, null, null);
	}

	public ProxyResolutionService(Uri[] proxies, string providerId) {

		this(proxies, providerId, null, null, null);
	}

	public ProxyResolutionService(Uri proxy, string providerId) {

		this(new Uri[] { proxy }, providerId, null, null, null);
	}

	public ProxyResolutionService(Uri[] proxies) {

		this(proxies, null, null, null, null);
	}

	public ProxyResolutionService(Uri proxy) {

		this(new Uri[] { proxy }, null, null, null, null);
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