/*
 * Copyright 2005 OpenXRI Foundation
 * Subsequently ported and altered by Andrew Arnott
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Text;

package org.openxri.xml;

using java.io.ByteArrayInputStream;
using java.io.InputStream;
using java.io.Serializable;
using java.net.URISyntaxException;
using java.text.ParseException;
using java.util.ArrayList;
using java.util.HashMap;
using java.util.Iterator;
using java.util.List;
using java.util.Vector;

using org.apache.xerces.dom.DocumentImpl;
using org.apache.xerces.parsers.DOMParser;
using org.apache.xml.security.exceptions.XMLSecurityException;
using org.apache.xml.security.keys.KeyInfo;
using org.openxri.XRIParseException;
using org.openxri.util.DOMUtils;
using org.openxri.util.PrioritizedList;
using org.w3c.dom.Document;
using org.w3c.dom.Element;
using org.w3c.dom.Node;
using org.xml.sax.InputSource;


/**
 * This class describes the Service XML element used for XRI Authority
 * resolution.
 *
 * @author =chetan
 * @author =wil
 * @author =peacekeeper
 */
public class Service : Cloneable, Serializable
{
	private static org.apache.commons.logging.Log soLog =
		org.apache.commons.logging.LogFactory.getLog(Service.class.getName());


	private ProviderID providerID;
	private Vector           localIDs;
	private List types;
	private List paths;
	private List mediaTypes;
	private Integer priority;
	private KeyInfo keyInfo;

	private List uris;
	private PrioritizedList prioritizedURIs;

	private Vector redirects;
	private PrioritizedList prioritizedRedirects = null;

	private Vector refs;
	private PrioritizedList prioritizedRefs = null;
	
	private HashMap otherChildrenVectorMap = new HashMap();

	/**
	 * Contructs an empty Service element
	 */
	public Service()
	{
		reset();
	}


	/**
	 *  This method constructs the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
	public Service(Element oElem) throws URISyntaxException
	{
		fromDOM(oElem);
	}


	/**
	 * Resets the internal state of this obj
	 */
	public void reset()
	{
		providerID = null;
		localIDs = new Vector();
		types = new Vector();
		paths = new Vector();
		mediaTypes = new Vector();
		priority = null;
		keyInfo = null;
		uris = new Vector();
		prioritizedURIs = null;
		redirects = new Vector();
		prioritizedRedirects = null;
		refs = new Vector();
		prioritizedRefs = null;
		otherChildrenVectorMap = new HashMap();
	}


	/**
	 * This method populates the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this processs.
	 */
	public void fromDOM(Element oElem)  throws URISyntaxException
	{
		reset();

		String val = oElem.getAttribute(Tags.ATTR_PRIORITY);
		if (val != null && val.length() > 0) {
			setPriority(new Integer(val));
		}

		for (
				Element oChild = DOMUtils.getFirstChildElement(oElem); oChild != null;
				oChild = DOMUtils.getNextSiblingElement(oChild))
		{
			// pre-grab the name and text value
			String sChildName = oChild.getLocalName();
			if (sChildName == null) sChildName = oChild.getNodeName();

			if (sChildName.equals(Tags.TAG_TYPE)) {
				// TODO: validate XRI/IRI/URI (must be in URI-normal form)
				types.add(SEPType.fromXML(oChild));
			}
			else if (sChildName.equals(Tags.TAG_PROVIDERID)) {
				ProviderID p = new ProviderID();
				p.fromXML(oChild);
				this.providerID = p;
			}
			else if (sChildName.equals(Tags.TAG_PATH)) {
				paths.add(SEPPath.fromXML(oChild));
			}
			else if (sChildName.equals(Tags.TAG_MEDIATYPE)) {
				mediaTypes.add(SEPMediaType.fromXML(oChild));
			}
			else if (sChildName.equals(Tags.TAG_URI)) {
				addURI(SEPUri.fromXML(oChild));
			}
			else if (sChildName.equals(Tags.TAG_REF)) {
				addRef(new Ref(oChild));
			}
			else if (sChildName.equals(Tags.TAG_REDIRECT)) {
				addRedirect(new Redirect(oChild));
			}
			else if (sChildName.equals(Tags.TAG_LOCALID)) {
				addLocalID(new LocalID(oChild));
			}
			else if (
					(oChild.getNamespaceURI() != null) &&
					oChild.getNamespaceURI().equals(Tags.NS_XMLDSIG) &&
					(oChild.getLocalName() != null) &&
					oChild.getLocalName().equals(Tags.TAG_KEYINFO))
			{
				try {
					keyInfo = new KeyInfo(oChild, "");
				}
				catch (XMLSecurityException oEx) {
					soLog.warn("Error constructing KeyInfo.", oEx);
				}
			}
			else {
				Vector oVector =
					(Vector) otherChildrenVectorMap.get(sChildName);
				if (oVector == null) {
					oVector = new Vector();
					otherChildrenVectorMap.put(sChildName, oVector);
				}

				// Instead of Storing just the Child Value, store a clone of the complete
				// Node that if we support multiple child elements and also custom NameSpaces
				oVector.add(oChild.cloneNode(true));
			}
		}
	}


	/**
	 * Returns the media type element value
	 * @deprecated
	 */
	public String getMediaType()
	{
		soLog.warn("getMediaType - deprecated.");
		SEPMediaType mtype = getMediaTypeAt(0);
		return (mtype != null)? mtype.getValue(): null;

	}


	/**
	 * Returns the number of media types in this service
	 */
	public int getNumMediaTypes()
	{
		return (mediaTypes == null) ? 0 : mediaTypes.size();

	}


	/**
	 * Returns the media type at the given index.
	 */
	public SEPMediaType getMediaTypeAt(int n)
	{
		if(this.mediaTypes != null) return (SEPMediaType)mediaTypes.get(n);
		return null;

	}


	/**
	 * Sets the media type element value
	 * @deprecated
	 */
	public void setMediaType(String sVal)
	{
		soLog.warn("setMediaType - deprecated.");
		SEPMediaType mediaType = new SEPMediaType(sVal,null,null);
		mediaTypes.add(mediaType);

	}


	/**
	 * Adds a media type to this Service
	 */
	public void addMediaType(String sVal)
	{
		addMediaType(sVal,null,null);
	}


	/**
	 * Adds a media type to this Service with attributes
	 */
	public void addMediaType(String sVal, String match, Boolean select)
	{
		SEPMediaType mediaType = new SEPMediaType(sVal,match, select);
		mediaTypes.add(mediaType);
	}


	/**
	 * Returns the type element value
	 * @deprecated
	 */
	public String getType()
	{
		soLog.warn("getType is deprecated.");
		SEPType type = getTypeAt(0);
		return (type != null)? type.getValue(): null;

	}


	/**
	 * Returns the number of types in this service
	 */
	public int getNumTypes()
	{
		return (types == null) ? 0 : types.size();

	}


	/**
	 * Returns the type at the given index.
	 */
	public SEPType getTypeAt(int n)
	{
		if(this.types != null)
			return (SEPType) types.get(n);
		return null;

	}


	/**
	 * Sets the  type element value
	 * @deprecated
	 */
	public void setType(String sVal)
	{
		soLog.warn("setType is deprecated.");
		types.add(new SEPType(sVal,null,null));
	}


	/**
	 * Adds a type to this Service
	 */
	public void addType(String sVal)
	{
		addType(sVal,null,null);
	}


	/**
	 * Adds a type to this Service with attributes
	 */
	public void addType(String sVal, String match, Boolean select)
	{
		types.add(new SEPType(sVal,match,select));
	}


	/**
	 * Returns true if the given type is equivalent to the type of this service.
	 * TODO - this should probably compare the normalized type rather than 
	 * performing a straight string comparison. Also, there may be multiple 
	 * types associated with a service.
	 * @deprecated
	 */
	public bool matchType(String sVal)
	{
		for (int i = 0; i < getNumTypes(); i++) {
			SEPType type = (SEPType)getTypeAt(i);
			if(type.match(sVal)) return true;
		}
		return false;
	}


	/**
	 * Returns the number of  URIs
	 */
	public int getNumURIs()
	{
		return (uris == null) ? 0 : uris.size();
	}


	/**
	 * Returns the first URI
	 * @deprecated
	 */
	public SEPUri getURI()
	{
		soLog.warn("getURI is deprecated.");
		return getURIAt(0);
	}


	/**
	 * Returns the URI at the given index
	 */
	public SEPUri getURIAt(int n)
	{
		return (n < getNumURIs()) ? (SEPUri) uris.get(n) : null;
	}


	/**
	 * Returns the first URI for the given scheme
	 */
	public SEPUri getURIForScheme(String sScheme)
	{
		if (sScheme == null) return null;

		for (int i = 0; i < getNumURIs(); i++)
		{
			// just return the first URI that matches the
			// requested scheme
			SEPUri oURI = (SEPUri)getURIAt(i);
			if (oURI != null && oURI.getURI() != null &&
					oURI.getURI().getScheme().equalsIgnoreCase(sScheme))
				return oURI;
		}

		return null;
	}


	/**
	 * Returns the a vector of URIs
	 */
	public List getURIs()
	{
		return uris;
	}


	/**
	 * Returns the URIs in sorted in priority order
	 * @return
	 */
	public ArrayList getPrioritizedURIs()
	{
		if (prioritizedURIs == null)
			return new ArrayList();
		return prioritizedURIs.getList();
	}


	/**
	 * Adds a URI to the service
	 */
	public void addURI(String sURI)
	{
		addURI(sURI, null, null);
	}


	/**
	 * Adds a URI to the service with attributes
	 */
	public void addURI(String sURI, Integer priority, String append)
	{    	 
		try {
			SEPUri uri = new SEPUri(sURI, priority, append);
			addURI(uri);
		}
		catch (Exception e) {
			throw new XRIParseException("BadURI", e);
		}
	}


	/**
	 * Adds an SEPUri obj to the list of URIs
	 * @param uri
	 */
	public void addURI(SEPUri uri)
	{
		if (prioritizedURIs == null)
		{
			prioritizedURIs = new PrioritizedList();
		}

		uris.add(uri);
		Integer priority = uri.getPriority();
		prioritizedURIs.addObject((priority == null)? PrioritizedList.PRIORITY_NULL : priority.toString(), uri);
	}


	/**
	 * Get a Servie Path
	 */
	public SEPPath getPathAt(int n)
	{
		return (n < getNumPaths()) ? (SEPPath) paths.get(n) : null;
	}


	/**
	 * Adds a URI to the service
	 */
	public void addPath(String sPath)
	{
		addPath(sPath,null,null);
	}


	/**
	 * Adds a URI to the service with attributes
	 */
	public void addPath(String sPath, String match, Boolean select)
	{
		try {
			paths.add(new SEPPath(sPath,match,select));
		}catch (Exception e) {
			throw new XRIParseException("BadPath", e);
		}
	}


	/**
	 * Returns the number of  URIs
	 */
	public int getNumPaths()
	{
		return (paths == null) ? 0 : paths.size();
	}


	/**
	 * Returns the authority id element value
	 */
	public String getProviderId()
	{
		return (providerID != null) ? providerID.getValue(): null;
	}


	/**
	 * Sets the authority id element value
	 */
	public void setProviderId(String val)
	{
		providerID = new ProviderID(val);
	}


	/**
	 * Sets the key info element
	 */

	public void setKeyInfo(KeyInfo oKeyInfo)
	{
		keyInfo = oKeyInfo;
	}


	/**
	 * Returns the key info element
	 */
	public KeyInfo getKeyInfo()
	{
		return keyInfo;
	}


	/**
	 * Stores simple elements in the Service by Tag
	 *
	 * Here we are converting the String obj that is being passed into XML
	 * Element before storing it into otherChildrenVectorMap Vector. The reason
	 * we are doing this is, we need to preserve NameSpaces, and also support a scenario
	 * where a Child Element under Service Element, can have Sub Elements. With this
	 * it will preserve all the Text Nodes under the Sub Element.
	 *
	 * @param sTag - The tag name. Needs to be the Fully Qualified Name of the XML Element.
	 *
	 *                    For Example "usrns1:info1"  or "info1" (If not using NameSpaces)
	 *
	 * @param sTagValue - The tag values. Needs to be valid XML String like --
	 *
	 *            "<usrns1:info1 xmlns:usrns1=\"xri://$user1*schema/localinfo\" >Newton</usrns1:info1>"

	 * @return -- Boolean - -True if the String could be Successfully Parsed and Stored, Else it will return false
	 *
	 */
	public bool setOtherTagValues(String sTag, String sTagValue)
	{
		String xmlStr =
			"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sTagValue;
		bool returnValue = false;

		try
		{
			InputStream oIn = new ByteArrayInputStream(xmlStr.getBytes());
			DOMParser oDOMParser = DOMUtils.getDOMParser();
			oDOMParser.parse(new InputSource(oIn));
			Document oDOMDoc = oDOMParser.getDocument();
			Element oElement = oDOMDoc.getDocumentElement();

			Vector oVector = (Vector) otherChildrenVectorMap.get(sTag);

			if (oVector == null)
			{
				oVector = new Vector();
				otherChildrenVectorMap.put(sTag, oVector);
			}

			oVector.add(oElement.cloneNode(true));

			returnValue = true;
		}

		catch (Exception exp)
		{
			exp.printStackTrace();
			returnValue = false;
		}

		return returnValue;
	}


	/**
	 * Returns unspecified simple elements in the Service by Tag
	 * @param sTag - The tag name to get values for
	 * @return a vector of text values whose element tag names match sTag
	 */
	public Vector getOtherTagValues(String sTag)
	{
		return (Vector) otherChildrenVectorMap.get(sTag);
	}

	public void setExtension(String extension) throws URISyntaxException, ParseException {

		String xmlStr = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
		"<xrd xmlns=\"xri://$xrd*($v*2.0)\"><Service>" +
		extension
		+ "</Service></xrd>";
		
		Service tempService = XRD.parseXRD(xmlStr, false).getServiceAt(0);
		
		this.otherChildrenVectorMap = tempService.otherChildrenVectorMap;
	}

	public String getExtension() {

		StringBuilder extension = new StringBuilder();
		
		Iterator oCustomTags = otherChildrenVectorMap.keySet().iterator();
		while (oCustomTags.hasNext())
		{
			String sTag = (String) oCustomTags.next();
			Vector oValues = (Vector) otherChildrenVectorMap.get(sTag);
			for (int i = 0; i < oValues.size(); i++)
			{
				Node oChild = (Node) oValues.get(i);
				
				extension.append(DOMUtils.toString((Element) oChild, true, true));
			}
		}

		return extension.toString();
	}


	/**
	 * This method will make DOM using the specified document.  If any DOM state
	 * has been stored with the obj, it will not be used in this method.
	 * This method generates a reference-free copy of new DOM.
	 * 
	 * @param oDoc - The document to use for generating DOM
	 */
	public Node toDOM(Document oDoc)
	{
		return toDOM(oDoc, false);
	}


	/**
	 * This method will make DOM using the specified document.  If any DOM state
	 * has been stored with the obj, it will not be used in this method.
	 * This method generates a reference-free copy of new DOM.
	 * 
	 * @param doc - The document to use for generating DOM
	 * @param wantFiltered - If true, the URIs will be sorted according to priority
	 */
	public Node toDOM(Document doc, bool wantFiltered)
	{
		Element elem =
			//name space tag is not required any more
			doc.createElementNS(Tags.NS_XRD_V2, Tags.TAG_SERVICE);

		if (getPriority() != null) {
			elem.setAttribute(Tags.ATTR_PRIORITY, getPriority().toString());
		}

		if (providerID != null && providerID.getValue() != null) {
			elem.appendChild(this.providerID.toXML(doc));
		}
		for (int i = 0; i < getNumTypes(); i++)
		{
			SEPElement type = (SEPElement)getTypeAt(i);
			elem.appendChild(type.toXML(doc, Tags.TAG_TYPE));
		}
		for (int i = 0; i < getNumPaths(); i++)
		{
			SEPElement path = (SEPElement)getPathAt(i);
			elem.appendChild(path.toXML(doc, Tags.TAG_PATH));
		}
		for (int i = 0; i < getNumMediaTypes(); i++)
		{
			SEPElement mtype = (SEPElement)getMediaTypeAt(i);
			elem.appendChild(mtype.toXML(doc, Tags.TAG_MEDIATYPE));
		}

		if (wantFiltered) {
			ArrayList uris = getPrioritizedURIs();
			for (int i = 0; i < uris.size(); i++) {
				SEPUri u = (SEPUri)uris.get(i);
				elem.appendChild(u.toXML(doc, Tags.TAG_URI));
			}
		}
		else {
			for (int i = 0; i < getNumURIs(); i++)
			{
				SEPUri uri = getURIAt(i);
				elem.appendChild(uri.toXML(doc, Tags.TAG_URI));
			}
		}

		for (int i = 0; i < getNumRedirects(); i++)
		{
			Redirect redir = getRedirectAt(i);
			elem.appendChild(redir.toXML(doc, Tags.TAG_REDIRECT));
		}

		for (int i = 0; i < getNumRefs(); i++)
		{
			Ref ref = getRefAt(i);
			elem.appendChild(ref.toXML(doc, Tags.TAG_REF));
		}


		for (int i = 0; i < getNumLocalIDs(); i++)
		{
			Element localID = (Element) getLocalIDAt(i).toXML(doc);
			elem.appendChild(localID);
		}
		
		if (getKeyInfo() != null)
		{
			Node oChild = doc.importNode(getKeyInfo().getElement(), true);
			elem.appendChild(oChild);
		}
		
		// this does not preserve the order and only works for text elements
		// TBD: Add namespace support for these
		Iterator oCustomTags = otherChildrenVectorMap.keySet().iterator();
		while (oCustomTags.hasNext())
		{
			String sTag = (String) oCustomTags.next();
			Vector oValues = (Vector) otherChildrenVectorMap.get(sTag);
			for (int i = 0; i < oValues.size(); i++)
			{
				// Importing the Child Node into New Document and also adding it to the
				// Service Element as a Child Node
				Node oChild = (Node) oValues.get(i);
				Node oChild2 = doc.importNode(oChild, true);
				elem.appendChild(oChild2);
			}
		}

		return elem;
	}


	/**
	 * Returns formatted obj.  Do not use if signature needs to be preserved.
	 */
	public String toString()
	{
		return dump();
	}


	/**
	 * Returns obj as a formatted XML string.
	 * @param sTab - The characters to prepend before each new line
	 */
	public String dump()
	{
		Document doc = new DocumentImpl();
		Node elm = this.toDOM(doc);
		doc.appendChild(elm);
		return DOMUtils.toString(doc);

	}


	/**
	 * @return Returns the priority.
	 */
	public Integer getPriority() {
		return priority;
	}


	/**
	 * @param priority The priority to set.
	 */
	public void setPriority(Integer priority) {
		this.priority = priority;
	}


	/**
	 * @param priority The priority to set.
	 */
	public void setPriority(String priority) {
		this.priority = new Integer(priority);
	}


	/**
	 * @return Returns the mediaTypes.
	 */
	public List getMediaTypes() {
		return mediaTypes;
	}


	/**
	 * @return Returns the otherChildrenVectorMap.
	 */
	public HashMap getOtherChildrenVectorMap() {
		return otherChildrenVectorMap;
	}


	/**
	 * @return Returns the paths.
	 */
	public List getPaths() {
		return paths;
	}


	/**
	 * @return Returns the types.
	 */
	public List getTypes() {
		return types;
	}


	public void addType(SEPType type){
		if(type == null) return;
		types.add(type);
	}


	public void addMediaType(SEPMediaType mtype){
		if(mtype == null) return;
		mediaTypes.add(mtype);
	}


	public void addPath(SEPPath path){
		if(path == null) return;
		paths.add(path);
	}


	/**
	 * @param prioritizedURIs The prioritizedURIs to set.
	 */
	public void setPrioritizedURIs(PrioritizedList prioritizedURIs) {
		this.prioritizedURIs = prioritizedURIs;
	}


	public Object clone() throws CloneNotSupportedException{
		Service srvc = new Service();

		/* for efficiency purpose didn't clone all the elements */
		srvc.keyInfo = keyInfo;
		srvc.otherChildrenVectorMap = this.otherChildrenVectorMap;
		srvc.prioritizedURIs = this.prioritizedURIs;
		srvc.priority = this.priority;
		srvc.providerID = this.providerID;
		srvc.refs = this.refs;
		srvc.prioritizedRefs = this.prioritizedRefs;
		srvc.redirects = this.redirects;
		srvc.prioritizedRedirects = this.prioritizedRedirects;
		srvc.localIDs = this.localIDs;
		srvc.paths= srvc.mediaTypes = srvc.types = srvc.uris = null;

		/* cloned types, mediatypes, path & uris cloned */
		Vector elements = null;
		if(types != null){
			elements = new Vector();
			for(int i =0; i< types.size(); i++){
				SEPElement element = (SEPElement) types.get(i);
				elements.add(element.clone());
			}
			srvc.types = elements;
		}

		if(mediaTypes != null){
			elements = new Vector();
			for(int i =0; i< mediaTypes.size(); i++){
				SEPElement element = (SEPElement) mediaTypes.get(i);
				elements.add(element.clone());
			}
			srvc.mediaTypes = elements;
		}

		if(paths != null){
			elements = new Vector();
			for(int i =0; i< paths.size(); i++){
				SEPElement element = (SEPElement) paths.get(i);
				elements.add(element.clone());
			}
			srvc.paths = elements;
		}

		if(uris != null){
			elements = new Vector();
			for(int i =0; i< uris.size(); i++){
				SEPUri element = (SEPUri) uris.get(i);
				elements.add(element.clone());
			}
			srvc.uris = elements;
		}

		return srvc;
	}


	/**
	 * @param uris The uris to set.
	 */
	public void setURIs(List uris) {
		this.uris = uris;
	}


	/**
	 * @param mediaTypes The mediaTypes to set.
	 */
	public void setMediaTypes(List mediaTypes) {
		this.mediaTypes = mediaTypes;
	}


	/**
	 * @param paths The paths to set.
	 */
	public void setPaths(List paths) {
		this.paths = paths;
	}


	/**
	 * @param types The types to set.
	 */
	public void setTypes(List types) {
		this.types = types;
	}

	
	/**
	 * @return Returns a copy of the collection of Refs in the order as it appears in the original XRD
	 */
	public Vector getRefs() {
		return (Vector)refs.clone();
	}

	
	public Ref getRefAt(int n){
		return (Ref)refs.get(n);
	}
	
	
	public int getNumRefs(){
		return refs.size();
	}
	
	
	public void addRef(Ref ref) {
		if (prioritizedRefs == null)
			prioritizedRefs = new PrioritizedList();
		
		Integer priority = ref.getPriority();		
		refs.add(ref);
		prioritizedRefs.addObject((priority == null)? "null" : priority.toString(), ref);
	}

	public ArrayList getPrioritizedRefs() {
		return prioritizedRefs.getList();
	}
	

	/**
	 * @return Returns a copy of the collection of Redirects in the order as it appears in the original XRD
	 */
	public Vector getRedirects() {
		return (Vector)redirects.clone();
	}

	
	public Redirect getRedirectAt(int n){
		return (Redirect) redirects.get(n);
	}
	
	
	public int getNumRedirects(){
		return redirects.size();
	}
	
	
	public void addRedirect(Redirect redirect) {
		if (prioritizedRedirects == null)
			prioritizedRedirects = new PrioritizedList();
		
		Integer priority = redirect.getPriority();		
		redirects.add(redirect);
		prioritizedRedirects.addObject((priority == null)? "null" : priority.toString(), redirect);
	}

	public ArrayList getPrioritizedRedirects() {
		return prioritizedRedirects.getList();
	}

	
	public int getNumLocalIDs(){
		return localIDs.size();
	}
	
	public LocalID getLocalIDAt(int n) {
		return (LocalID) localIDs.get(n);
	}
	
	public void addLocalID(LocalID localId) {
		localIDs.add(localId);
	}
	
	


	public bool equals(Object o) {

		if (! (o is Service)) return(false);
		
		Service other = (Service) o;

		if (other == null) return(false);
		if (other == this) return(true);

		if (this.providerID == null && other.providerID != null) return(false);
		if (this.providerID != null && ! (this.providerID.equals(other.providerID))) return(false);

		if (this.priority == null && other.priority != null) return(false);
		if (this.priority != null && ! (this.priority.equals(other.priority))) return(false);

		if (this.types == null && other.types != null) return(false);
		if (this.types != null && ! (this.types.equals(other.types))) return(false);

		if (this.paths == null && other.paths != null) return(false); 
		if (this.paths != null && ! (this.paths.equals(other.paths))) return(false);

		if (this.mediaTypes == null && other.mediaTypes != null) return(false); 
		if (this.mediaTypes != null &&! (this.mediaTypes.equals(other.mediaTypes))) return(false);

		if (this.uris == null && other.uris != null) return(false); 
		if (this.uris != null && ! (this.uris.equals(other.uris))) return(false);

		if (this.otherChildrenVectorMap == null && other.otherChildrenVectorMap != null) return(false); 
		if (this.otherChildrenVectorMap != null && ! (this.otherChildrenVectorMap.equals(other.otherChildrenVectorMap))) return(false);

		if (this.prioritizedURIs == null && other.prioritizedURIs != null) return(false); 
		if (this.prioritizedURIs != null && ! (this.prioritizedURIs.equals(other.prioritizedURIs))) return(false);

		// TODO: should we compare the KeyInfo too ?

		return(true);
	}
}
