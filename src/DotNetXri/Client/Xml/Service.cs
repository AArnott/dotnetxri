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

namespace DotNetXri.Client.Xml {

//using java.io.ByteArrayInputStream;
//using java.io.InputStream;
//using java.io.Serializable;
//using java.net.URISyntaxException;
//using java.text.ParseException;
//using java.util.ArrayList;
//using java.util.Hashtable;
//using java.util.Iterator;
//using java.util.List;
//using java.util.ArrayList;

//using org.apache.xerces.dom.XmlDocument;
//using org.apache.xerces.parsers.DOMParser;
//using org.apache.xml.security.exceptions.XMLSecurityException;
//using org.apache.xml.security.keys.KeyInfo;
//using org.openxri.XRIParseException;
//using org.openxri.util.DOMUtils;
//using org.openxri.util.PrioritizedList;
//using org.w3c.dom.XmlDocument;
//using org.w3c.dom.XmlElement;
//using org.w3c.dom.Node;
//using org.xml.sax.InputSource;
using System;
using System.Collections;
using DotNetXri.Loggers;
using System.Xml;


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
	private static ILog soLog = Logger.Create(typeof(Service));


	private ProviderID providerID;
	private ArrayList           localIDs;
	private ArrayList types;
	private ArrayList paths;
	private ArrayList mediaTypes;
	private int? priority;
	private KeyInfo keyInfo;

	private ArrayList uris;
	private PrioritizedList prioritizedURIs;

	private ArrayList redirects;
	private PrioritizedList prioritizedRedirects = null;

	private ArrayList refs;
	private PrioritizedList prioritizedRefs = null;
	
	private Hashtable otherChildrenVectorMap = new Hashtable();

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
	public Service(XmlElement oElem) //throws URISyntaxException
	{
		fromDOM(oElem);
	}


	/**
	* Resets the internal state of this obj
	*/
	public void reset()
	{
		providerID = null;
		localIDs = new ArrayList();
		types = new ArrayList();
		paths = new ArrayList();
		mediaTypes = new ArrayList();
		priority = null;
		keyInfo = null;
		uris = new ArrayList();
		prioritizedURIs = null;
		redirects = new ArrayList();
		prioritizedRedirects = null;
		refs = new ArrayList();
		prioritizedRefs = null;
		otherChildrenVectorMap = new Hashtable();
	}


	/**
	* This method populates the obj from DOM.  It does not keep a
	* copy of the DOM around.  Whitespace information is lost in this processs.
	*/
	public void fromDOM(XmlElement oElem)  //throws URISyntaxException
	{
		reset();

		string val = oElem.GetAttribute(Tags.ATTR_PRIORITY);
		if (val != null && val.Length > 0) {
			setPriority(val);
		}

		for (
				XmlElement oChild = DOMUtils.getFirstChildElement(oElem); oChild != null;
				oChild = DOMUtils.getNextSiblingElement(oChild))
		{
			// pre-grab the name and text value
			string sChildName = oChild.LocalName;
			if (sChildName == null) sChildName = oChild.getNodeName();

			if (sChildName.Equals(Tags.TAG_TYPE)) {
				// TODO: validate XRI/IRI/Uri (must be in Uri-normal form)
				types.add(SEPType.fromXML(oChild));
			}
			else if (sChildName.Equals(Tags.TAG_PROVIDERID)) {
				ProviderID p = new ProviderID();
				p.fromXML(oChild);
				this.providerID = p;
			}
			else if (sChildName.Equals(Tags.TAG_PATH)) {
				paths.add(SEPPath.fromXML(oChild));
			}
			else if (sChildName.Equals(Tags.TAG_MEDIATYPE)) {
				mediaTypes.add(SEPMediaType.fromXML(oChild));
			}
			else if (sChildName.Equals(Tags.TAG_URI)) {
				addURI(SEPUri.fromXML(oChild));
			}
			else if (sChildName.Equals(Tags.TAG_REF)) {
				addRef(new Ref(oChild));
			}
			else if (sChildName.Equals(Tags.TAG_REDIRECT)) {
				addRedirect(new Redirect(oChild));
			}
			else if (sChildName.Equals(Tags.TAG_LOCALID)) {
				addLocalID(new LocalID(oChild));
			}
			else if (
					(oChild.NamespaceURI != null) &&
					oChild.NamespaceURI.Equals(Tags.NS_XMLDSIG) &&
					(oChild.LocalName != null) &&
					oChild.LocalName.Equals(Tags.TAG_KEYINFO))
			{
				try {
					keyInfo = new KeyInfo(oChild, "");
				}
				catch (XMLSecurityException oEx) {
					soLog.Warn("Error constructing KeyInfo.", oEx);
				}
			}
			else {
				ArrayList oVector =
					(ArrayList) otherChildrenVectorMap.get(sChildName);
				if (oVector == null) {
					oVector = new ArrayList();
					otherChildrenVectorMap.put(sChildName, oVector);
				}

				// Instead of Storing just the Child Value, store a clone of the complete
				// Node that if we support multiple child elements and also custom NameSpaces
				oVector.Add(oChild.CloneNode(true));
			}
		}
	}


	/**
	* Returns the media type element value
	* @deprecated
	*/
	public string getMediaType()
	{
		soLog.Warn("getMediaType - deprecated.");
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
	public void setMediaType(string sVal)
	{
		soLog.Warn("setMediaType - deprecated.");
		SEPMediaType mediaType = new SEPMediaType(sVal,null,null);
		mediaTypes.add(mediaType);

	}


	/**
	* Adds a media type to this Service
	*/
	public void addMediaType(string sVal)
	{
		addMediaType(sVal,null,null);
	}


	/**
	* Adds a media type to this Service with attributes
	*/
	public void addMediaType(string sVal, string match, Boolean select)
	{
		SEPMediaType mediaType = new SEPMediaType(sVal,match, select);
		mediaTypes.add(mediaType);
	}


	/**
	* Returns the type element value
	* @deprecated
	*/
	public string getType()
	{
		soLog.Warn("getType is deprecated.");
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
	public void setType(string sVal)
	{
		soLog.Warn("setType is deprecated.");
		types.add(new SEPType(sVal,null,null));
	}


	/**
	* Adds a type to this Service
	*/
	public void addType(string sVal)
	{
		addType(sVal,null,null);
	}


	/**
	* Adds a type to this Service with attributes
	*/
	public void addType(string sVal, string match, Boolean select)
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
	public bool matchType(string sVal)
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
	* Returns the first Uri
	* @deprecated
	*/
	public SEPUri getURI()
	{
		soLog.Warn("getURI is deprecated.");
		return getURIAt(0);
	}


	/**
	* Returns the Uri at the given index
	*/
	public SEPUri getURIAt(int n)
	{
		return (n < getNumURIs()) ? (SEPUri) uris.get(n) : null;
	}


	/**
	* Returns the first Uri for the given scheme
	*/
	public SEPUri getURIForScheme(string sScheme)
	{
		if (sScheme == null) return null;

		for (int i = 0; i < getNumURIs(); i++)
		{
			// just return the first Uri that matches the
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
	public ArrayList getURIs()
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
	* Adds a Uri to the service
	*/
	public void addURI(string sURI)
	{
		addURI(sURI, null, null);
	}


	/**
	* Adds a Uri to the service with attributes
	*/
	public void addURI(string sURI, int? priority, string append)
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

		uris.Add(uri);
		int? priority = uri.getPriority();
		prioritizedURIs.addObject((priority == null)? PrioritizedList.PRIORITY_NULL : priority.ToString(), uri);
	}


	/**
	* Get a Servie Path
	*/
	public SEPPath getPathAt(int n)
	{
		return (n < getNumPaths()) ? (SEPPath) paths.get(n) : null;
	}


	/**
	* Adds a Uri to the service
	*/
	public void addPath(string sPath)
	{
		addPath(sPath,null,null);
	}


	/**
	* Adds a Uri to the service with attributes
	*/
	public void addPath(string sPath, string match, Boolean select)
	{
		try {
			paths.Add(new SEPPath(sPath,match,select));
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
	public string getProviderId()
	{
		return (providerID != null) ? providerID.getValue(): null;
	}


	/**
	* Sets the authority id element value
	*/
	public void setProviderId(string val)
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
	* Here we are converting the string obj that is being passed into XML
	* XmlElement before storing it into otherChildrenVectorMap ArrayList. The reason
	* we are doing this is, we need to preserve NameSpaces, and also support a scenario
	* where a Child XmlElement under Service XmlElement, can have Sub Elements. With this
	* it will preserve all the Text Nodes under the Sub XmlElement.
	*
	* @param sTag - The tag name. Needs to be the Fully Qualified Name of the XML XmlElement.
	*
	*                    For Example "usrns1:info1"  or "info1" (If not using NameSpaces)
	*
	* @param sTagValue - The tag values. Needs to be valid XML string like --
	*
	*            "<usrns1:info1 xmlns:usrns1=\"xri://$user1*schema/localinfo\" >Newton</usrns1:info1>"

	* @return -- Boolean - -True if the string could be Successfully Parsed and Stored, Else it will return false
	*
	*/
	public bool setOtherTagValues(string sTag, string sTagValue)
	{
		string xmlStr =
			"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sTagValue;
		bool returnValue = false;

		try
		{
			InputStream oIn = new ByteArrayInputStream(xmlStr.getBytes());
			DOMParser oDOMParser = DOMUtils.getDOMParser();
			oDOMParser.parse(new InputSource(oIn));
			XmlDocument oDOMDoc = oDOMParser.getDocument();
			XmlElement oElement = oDOMDoc.getDocumentElement();

			ArrayList oVector = (ArrayList) otherChildrenVectorMap.get(sTag);

			if (oVector == null)
			{
				oVector = new ArrayList();
				otherChildrenVectorMap.put(sTag, oVector);
			}

			oVector.Add(oElement.CloneNode(true));

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
	public ArrayList getOtherTagValues(string sTag)
	{
		return (ArrayList) otherChildrenVectorMap.get(sTag);
	}

	public void setExtension(string extension) //throws URISyntaxException, ParseException {

		string xmlStr = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
		"<xrd xmlns=\"xri://$xrd*($v*2.0)\"><Service>" +
		extension
		+ "</Service></xrd>";
		
		Service tempService = XRD.parseXRD(xmlStr, false).getServiceAt(0);
		
		this.otherChildrenVectorMap = tempService.otherChildrenVectorMap;
	}

	public string getExtension() {

		StringBuilder extension = new StringBuilder();
		
		Iterator oCustomTags = otherChildrenVectorMap.keySet().iterator();
		while (oCustomTags.hasNext())
		{
			string sTag = (string) oCustomTags.next();
			ArrayList oValues = (ArrayList) otherChildrenVectorMap.get(sTag);
			for (int i = 0; i < oValues.size(); i++)
			{
				Node oChild = (Node) oValues.get(i);
				
				extension.append(DOMUtils.ToString((XmlElement) oChild, true, true));
			}
		}

		return extension.ToString();
	}


	/**
	* This method will make DOM using the specified document.  If any DOM state
	* has been stored with the obj, it will not be used in this method.
	* This method generates a reference-free copy of new DOM.
	* 
	* @param oDoc - The document to use for generating DOM
	*/
	public Node toDOM(XmlDocument oDoc)
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
	public Node toDOM(XmlDocument doc, bool wantFiltered)
	{
		XmlElement elem =
			//name space tag is not required any more
			doc.createElementNS(Tags.NS_XRD_V2, Tags.TAG_SERVICE);

		if (getPriority() != null) {
			elem.SetAttribute(Tags.ATTR_PRIORITY, getPriority().ToString());
		}

		if (providerID != null && providerID.getValue() != null) {
			elem.AppendChild(this.providerID.toXML(doc));
		}
		for (int i = 0; i < getNumTypes(); i++)
		{
			SEPElement type = (SEPElement)getTypeAt(i);
			elem.AppendChild(type.toXML(doc, Tags.TAG_TYPE));
		}
		for (int i = 0; i < getNumPaths(); i++)
		{
			SEPElement path = (SEPElement)getPathAt(i);
			elem.AppendChild(path.toXML(doc, Tags.TAG_PATH));
		}
		for (int i = 0; i < getNumMediaTypes(); i++)
		{
			SEPElement mtype = (SEPElement)getMediaTypeAt(i);
			elem.AppendChild(mtype.toXML(doc, Tags.TAG_MEDIATYPE));
		}

		if (wantFiltered) {
			ArrayList uris = getPrioritizedURIs();
			for (int i = 0; i < uris.size(); i++) {
				SEPUri u = (SEPUri)uris.get(i);
				elem.AppendChild(u.toXML(doc, Tags.TAG_URI));
			}
		}
		else {
			for (int i = 0; i < getNumURIs(); i++)
			{
				SEPUri uri = getURIAt(i);
				elem.AppendChild(uri.toXML(doc, Tags.TAG_URI));
			}
		}

		for (int i = 0; i < getNumRedirects(); i++)
		{
			Redirect redir = getRedirectAt(i);
			elem.AppendChild(redir.toXML(doc, Tags.TAG_REDIRECT));
		}

		for (int i = 0; i < getNumRefs(); i++)
		{
			Ref _ref = getRefAt(i);
			elem.AppendChild(_ref.toXML(doc, Tags.TAG_REF));
		}


		for (int i = 0; i < getNumLocalIDs(); i++)
		{
			XmlElement localID = (XmlElement) getLocalIDAt(i).toXML(doc);
			elem.AppendChild(localID);
		}
		
		if (getKeyInfo() != null)
		{
			Node oChild = doc.importNode(getKeyInfo().getElement(), true);
			elem.AppendChild(oChild);
		}
		
		// this does not preserve the order and only works for text elements
		// TBD: Add namespace support for these
		Iterator oCustomTags = otherChildrenVectorMap.keySet().iterator();
		while (oCustomTags.hasNext())
		{
			string sTag = (string) oCustomTags.next();
			ArrayList oValues = (ArrayList) otherChildrenVectorMap.get(sTag);
			for (int i = 0; i < oValues.size(); i++)
			{
				// Importing the Child Node into New XmlDocument and also adding it to the
				// Service XmlElement as a Child Node
				Node oChild = (Node) oValues.get(i);
				Node oChild2 = doc.importNode(oChild, true);
				elem.AppendChild(oChild2);
			}
		}

		return elem;
	}


	/**
	* Returns formatted obj.  Do not use if signature needs to be preserved.
	*/
	public override string ToString()
	{
		return dump();
	}


	/**
	* Returns obj as a formatted XML string.
	* @param sTab - The characters to prepend before each new line
	*/
	public string dump()
	{
		XmlDocument doc = new XmlDocument();
		Node elm = this.toDOM(doc);
		doc.AppendChild(elm);
		return DOMUtils.ToString(doc);

	}


	/**
	* @return Returns the priority.
	*/
	public int? getPriority() {
		return priority;
	}


	/**
	* @param priority The priority to set.
	*/
	public void setPriority(int? priority) {
		this.priority = priority;
	}


	/**
	* @param priority The priority to set.
	*/
	public void setPriority(string priority) {
		this.priority = priority;
	}


	/**
	* @return Returns the mediaTypes.
	*/
	public ArrayList getMediaTypes() {
		return mediaTypes;
	}


	/**
	* @return Returns the otherChildrenVectorMap.
	*/
	public Hashtable getOtherChildrenVectorMap() {
		return otherChildrenVectorMap;
	}


	/**
	* @return Returns the paths.
	*/
	public ArrayList getPaths() {
		return paths;
	}


	/**
	* @return Returns the types.
	*/
	public ArrayList getTypes() {
		return types;
	}


	public void addType(SEPType type){
		if(type == null) return;
		types.Add(type);
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


	public Object clone() //throws CloneNotSupportedException{
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
		ArrayList elements = null;
		if(types != null){
			elements = new ArrayList();
			for(int i =0; i< types.size(); i++){
				SEPElement element = (SEPElement) types.get(i);
				elements.add(element.clone());
			}
			srvc.types = elements;
		}

		if(mediaTypes != null){
			elements = new ArrayList();
			for(int i =0; i< mediaTypes.size(); i++){
				SEPElement element = (SEPElement) mediaTypes.get(i);
				elements.add(element.clone());
			}
			srvc.mediaTypes = elements;
		}

		if(paths != null){
			elements = new ArrayList();
			for(int i =0; i< paths.size(); i++){
				SEPElement element = (SEPElement) paths.get(i);
				elements.add(element.clone());
			}
			srvc.paths = elements;
		}

		if(uris != null){
			elements = new ArrayList();
			for(int i =0; i< uris.size(); i++){
				SEPUri element = (SEPUri) uris.get(i);
				elements.Add(element.clone());
			}
			srvc.uris = elements;
		}

		return srvc;
	}


	/**
	* @param uris The uris to set.
	*/
	public void setURIs(ArrayList uris) {
		this.uris = uris;
	}


	/**
	* @param mediaTypes The mediaTypes to set.
	*/
	public void setMediaTypes(ArrayList mediaTypes) {
		this.mediaTypes = mediaTypes;
	}


	/**
	* @param paths The paths to set.
	*/
	public void setPaths(ArrayList paths) {
		this.paths = paths;
	}


	/**
	* @param types The types to set.
	*/
	public void setTypes(ArrayList types) {
		this.types = types;
	}

	
	/**
	* @return Returns a copy of the collection of Refs in the order as it appears in the original XRD
	*/
	public ArrayList getRefs() {
		return (ArrayList)refs.clone();
	}

	
	public Ref getRefAt(int n){
		return (Ref)refs.get(n);
	}
	
	
	public int getNumRefs(){
		return refs.size();
	}
	
	
	public void addRef(Ref _ref) {
		if (prioritizedRefs == null)
			prioritizedRefs = new PrioritizedList();
		
		int? priority = _ref.getPriority();		
		refs.add(_ref);
		prioritizedRefs.addObject((priority == null)? "null" : priority.ToString(), _ref);
	}

	public ArrayList getPrioritizedRefs() {
		return prioritizedRefs.getList();
	}
	

	/**
	* @return Returns a copy of the collection of Redirects in the order as it appears in the original XRD
	*/
	public ArrayList getRedirects() {
		return (ArrayList)redirects.clone();
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
		
		int? priority = redirect.getPriority();		
		redirects.add(redirect);
		prioritizedRedirects.addObject((priority == null)? "null" : priority.ToString(), redirect);
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
	
	


	public bool Equals(Object o) {

		if (! (o is Service)) return(false);
		
		Service other = (Service) o;

		if (other == null) return(false);
		if (other == this) return(true);

		if (this.providerID == null && other.providerID != null) return(false);
		if (this.providerID != null && ! (this.providerID.Equals(other.providerID))) return(false);

		if (this.priority == null && other.priority != null) return(false);
		if (this.priority != null && ! (this.priority.Equals(other.priority))) return(false);

		if (this.types == null && other.types != null) return(false);
		if (this.types != null && ! (this.types.Equals(other.types))) return(false);

		if (this.paths == null && other.paths != null) return(false); 
		if (this.paths != null && ! (this.paths.Equals(other.paths))) return(false);

		if (this.mediaTypes == null && other.mediaTypes != null) return(false); 
		if (this.mediaTypes != null &&! (this.mediaTypes.Equals(other.mediaTypes))) return(false);

		if (this.uris == null && other.uris != null) return(false); 
		if (this.uris != null && ! (this.uris.Equals(other.uris))) return(false);

		if (this.otherChildrenVectorMap == null && other.otherChildrenVectorMap != null) return(false); 
		if (this.otherChildrenVectorMap != null && ! (this.otherChildrenVectorMap.Equals(other.otherChildrenVectorMap))) return(false);

		if (this.prioritizedURIs == null && other.prioritizedURIs != null) return(false); 
		if (this.prioritizedURIs != null && ! (this.prioritizedURIs.Equals(other.prioritizedURIs))) return(false);

		// TODO: should we compare the KeyInfo too ?

		return(true);
	}
}
}