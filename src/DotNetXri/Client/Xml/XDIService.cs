namespace DotNetXri.Client.Xml {

//using java.net.Uri;
//using java.net.UriFormatException;
//using java.util.List;

/**
	* This is an XDI Service.
	* @author =peacekeeper
	*/
public class XDIService : Service {

	public const string SERVICE_TYPE = "xri://$xdi!($v!1)";
	public const string SERVICE_PATH = "($context)!($xdi)!($card)!($v!1) ";

	/**
		* Constructs a new XDI Service endpoint for use in an authority.
		* @param providerID - The global i-number of the provider of this service.
		*/
	public XDIService(Uri[] uris, string providerID) {



		/*
			* The ProviderID of the XDI Service 
			* SHOULD be set to the global i-number of the provider.
			*/
		if (providerID != null) this.setProviderId(providerID);

		/*
			* This setting is REQUIRED to establish the XDI Service.
			*/
		this.addType(new SEPType(SERVICE_TYPE, null, true));

		/*
			* This setting is REQUIRED to establish the XDI Service.
			*/
		this.addPath(new SEPPath(SERVICE_PATH, null, true));

		/*
			* Setting a media type to default
			* is not strictly necessary, since this setting is implied anyway, if no other
			* media type is specified. On the other hand, it can't really harm.
			*/
		this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, null));

		/*
			* These are the URIs where the XDI Service is implemented. Nothing will be appended.
			*/
		for (int i = 0; i < uris.Length; i++) {

			Uri uri = uris[i];

			try {

				this.addURI(new SEPUri(uri.ToString(), null, SEPUri.APPEND_NONE));
			} catch (UriFormatException ex) {

				continue;
			}
		}
	}

	public XDIService(Uri uri, string providerID) {

		this(new Uri[] { uri }, providerID);
	}

	public XDIService(Uri[] uris) {

		this(uris, null);
	}

	public XDIService(Uri uri) {

		this(new Uri[] { uri }, null);
	}

	/**
		* Constructs a new XDI Service endpoint for use in an authority.
		* @param providerID - The global i-number of the provider of this service.
		*/
	public XDIService(Ref[] xdiRefs, string providerID) {



		/*
			* The ProviderID of the XDI Service 
			* SHOULD be set to the global i-number of the provider.
			*/
		if (providerID != null) this.setProviderId(providerID);

		/*
			* This setting is REQUIRED to establish the XDI Service.
			*/
		this.addType(new SEPType(SERVICE_TYPE, null, true));

		/*
			* This setting is REQUIRED to establish the XDI Service.
			*/
		this.addPath(new SEPPath(SERVICE_PATH, null, true));

		/*
			* Setting a media type to default
			* is not strictly necessary, since this setting is implied anyway, if no other
			* media type is specified. On the other hand, it can't really harm.
			*/
		this.addMediaType(new SEPMediaType(null, SEPMediaType.MATCH_ATTR_DEFAULT, null));

		/*
			* These are the Refs where the XDI Service is implemented. Nothing will be appended.
			*/
		for (int i = 0; i < xdiRefs.Length; i++) {

			Ref xdiRef = xdiRefs[i];

			this.addRef(xdiRef);
		}
	}

	public XDIService(Ref xdiRef, string providerID) {

		this(new Ref[] { xdiRef }, providerID);
	}

	public XDIService(Ref[] xdiRefs) {

		this(xdiRefs, null);
	}

	public XDIService(Ref xdiRef) {

		this(new Ref[] { xdiRef }, null);
	}

	public static bool isInstance(Service service) {

		if (service is XDIService) return (true);

		ArrayList serviceTypes = service.getTypes();

		for (int i = 0; i < serviceTypes.size(); i++) {

			SEPType serviceType = (SEPType)serviceTypes[i];

			if (SERVICE_TYPE.Equals(serviceType.getValue())) return (true);
		}

		return (false);
	}
}
}