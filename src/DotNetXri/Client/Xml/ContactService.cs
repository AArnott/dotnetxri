package org.openxri.xml;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;

/**
 * This is a Contact Service as described by "Contact Service v1.0 Working Draft 05, 7 September 2006
 * All constructors produce a service endpoint that conforms to the specification.
 * It is currently not in the scope of OpenXRI to implement the actual contact page for this service.
 * The RECOMMENDED third-level DNS name for hosting a Contact Service is 'contact',
 * e.g. 'contact.my-i-broker.com'
 * @author =peacekeeper
 */
public class ContactService :Service {

	public const String SERVICE_TYPE = "xri://+i-service*(+contact)*($v*1.0)";
	public const String CONTACT_PATH = "(+contact)";

	/**
	 * Constructs a new Contact Service endpoint for use in an authority.
	 * @param contactPages - The URI(s) where a contact page is implemented.
	 * @param providerID - The global i-number of the I-Broker providing this Contact Service.
	 * @param makeDefault - Whether to make the Contact Service the default service.
	 */
	public ContactService(URI[] contactPages, String providerID, bool makeDefault) {
		
		
		
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
		 * These are the URI where the Contact Service is implemented. The QXRI will be appended.
		 * It is currently not in the scope of OpenXRI to implement the actual contact page.
		 */
		for (int i=0; i<contactPages.length; i++) {

			URI contactPage = contactPages[i];
			
			try {

				this.addURI(new SEPUri(contactPage.toString(), null, SEPUri.APPEND_QXRI));
			} catch (URISyntaxException ex) {
				
				continue;
			}
		}
	}
	
	public ContactService(URI contactPage, String providerID, bool makeDefault) {
	
		this(new URI[] { contactPage }, providerID, makeDefault);
	}
		
	public ContactService(URI[] contactPages, String providerID) {
		
		this(contactPages, providerID, true);
	}
		
	public ContactService(URI contactPage, String providerID) {
		
		this(new URI[] { contactPage }, providerID, true);
	}
		
	public ContactService(URI[] contactPages) {
		
		this(contactPages, null, true);
	}
	
	public ContactService(URI contactPage) {
		
		this(new URI[] { contactPage }, null, true);
	}

	public static bool isInstance(Service service) {
		
		if (service is ForwardingService) return(true);
		
		List serviceTypes = service.getTypes();
		
		for (int i=0; i<serviceTypes.size(); i++) {
			
			SEPType serviceType = (SEPType) serviceTypes.get(i);
			
			if (SERVICE_TYPE.equals(serviceType.getValue())) return(true);
		}
		
		return(false);
	}
}
