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
package org.openxri.xml;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.Serializable;
import java.net.URISyntaxException;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Vector;

import org.apache.xerces.dom.DocumentImpl;
import org.apache.xerces.parsers.DOMParser;
import org.apache.xml.security.c14n.Canonicalizer;
import org.apache.xml.security.exceptions.XMLSecurityException;
import org.apache.xml.security.signature.Reference;
import org.apache.xml.security.signature.SignedInfo;
import org.apache.xml.security.signature.XMLSignature;
import org.apache.xml.security.transforms.Transforms;
import org.openxri.XRIParseException;
import org.openxri.resolve.exception.XRIResolutionException;
import org.openxri.saml.Assertion;
import org.openxri.util.DOM3Utils;
import org.openxri.util.DOMUtils;
import org.openxri.util.PrioritizedList;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;


/**
 * This class describes the XRD XML element used for XRI Authority
 * resolution.
 *
 * @author =chetan
 * @author =wil
 */
public class XRD : Cloneable, Serializable
{
	
	public const String CURRENT_VERSION = "2.0";
	
	private static org.apache.commons.logging.Log soLog =
		org.apache.commons.logging.LogFactory.getLog(
				XRD.class.getName());
	
	// optional attributes
	private String xmlID;
	private String idRef;
	private String version;

	// other elements
	private Vector           types;
	private Query            query;
	private Status           status;
	private ServerStatus     serverStatus;
	private Expires          expires;
	private ProviderID       providerID;

	private Vector           localIDs;
	private Vector           equivIDs;
	private Vector           canonicalIDs;
	private CanonicalEquivID canonicalEquivID;
	
	private Vector           redirects;
	private PrioritizedList  prioritizedRedirects;
	
	private Vector           refs;
	private PrioritizedList  prioritizedRefs;
	
	private Vector           services;
	private PrioritizedList  prioritizedServices;
	private PrioritizedList  selectedServices;

	private Assertion        samlAssertion;

	private Element          moElem;
	private HashMap          moOtherChildrenVectorsMap;

	
	static
	{
		try {
			org.apache.xml.security.Init.init();
		}
		catch (Exception oEx) {
			soLog.warn("Failed to initialize XML Sec library", oEx);
		}
	}
	
	
	/**
	 * Constructs an empty XRD
	 */
	public XRD()
	{
		reset();
	}

	
	/**
	 * This method resets the state of the XRD.
	 */
	public void reset()
	{
		xmlID        = "";
		idRef        = null;
		version	     = null;
		types        = new Vector();
		query        = null;
		status       = null;
		serverStatus = null;
		expires      = null;
		providerID   = null;

		localIDs             = new Vector();    
		equivIDs             = new Vector();
		canonicalIDs         = new Vector(); // CanonicalID (TODO: Should be 0 or 1)
		canonicalEquivID     = null;
		
		refs                 = new Vector();
		prioritizedRefs      = new PrioritizedList();
		
		redirects            = new Vector();
		prioritizedRedirects = new PrioritizedList();
		
		services             = new Vector();
		prioritizedServices  = new PrioritizedList();
		selectedServices     = new PrioritizedList();

		samlAssertion        = null;

		moElem               = null;
		moOtherChildrenVectorsMap = new HashMap();
	}


	/**
	 * Clone this obj
	 */
	public Object clone()
	{
		XRD x = new XRD();
		
		x.xmlID            = xmlID;
		x.idRef            = idRef;
		x.version          = version;
		if (types != null) 
			x.types = (Vector)types.clone();		
		if (query != null) 
			x.query = new Query(query);
		if (status != null) 
			x.status = new Status(status);
		if (serverStatus != null) 
			x.serverStatus = new ServerStatus(serverStatus);
		if (expires != null) 
			x.expires = new Expires(expires);
		if (providerID != null) 
			x.providerID = new ProviderID(providerID);

		if (localIDs != null)
			x.localIDs = (Vector)localIDs.clone();
		if (equivIDs != null) 
			x.equivIDs = (Vector)equivIDs.clone();
		if (canonicalIDs != null) 
			x.canonicalIDs = (Vector)canonicalIDs.clone();
		if (canonicalEquivID != null) 
			x.canonicalEquivID = new CanonicalEquivID(canonicalEquivID);
		
		if (refs != null)
			x.refs = (Vector)refs.clone();
		if (prioritizedRefs != null)
			x.prioritizedRefs = (PrioritizedList)prioritizedRefs.clone();
		
		if (redirects != null)
			x.redirects = (Vector)redirects.clone();
		if (prioritizedRedirects != null)
			x.prioritizedRedirects = (PrioritizedList)prioritizedRedirects.clone();

		if (services != null)
			x.services = (Vector)services.clone();
		if (prioritizedServices != null)
			x.prioritizedServices = (PrioritizedList)prioritizedServices.clone();
		if (selectedServices != null)
			x.selectedServices = (PrioritizedList)selectedServices.clone();

		// XXX shallow copy here!
		x.samlAssertion = samlAssertion;
		x.moElem        = moElem;
		
		if (moOtherChildrenVectorsMap != null)
			x.moOtherChildrenVectorsMap = (HashMap)moOtherChildrenVectorsMap.clone();
		
		return x;
	}
	
	
	
	/**
	 *  This method creates the XRD from DOM.  It optionally keeps a
	 * pointer to the DOM.
	 * @param oElem - The DOM to create the obj from
	 * @param bKeepDOM - If true, will keep a copy of the DOM with the obj
	 */
	public XRD(Element oElem, bool bKeepDOM) throws URISyntaxException, ParseException
	{
		if (bKeepDOM) {
			setDOM(oElem);
		}
		else {
			fromDOM(oElem);
		}
	}


	/**
	 * Constructs XRD from a String
	 */
	public static XRD parseXRD (String xmlStr, bool bKeepDOM)
	throws URISyntaxException, ParseException
	{
		InputStream oIn = new ByteArrayInputStream(xmlStr.getBytes());
		
		XRD oXriD = null;
		
		try {
			DOMParser oDOMParser = DOMUtils.getDOMParser();
			oDOMParser.parse(new InputSource(oIn));
			Document oDOMDoc = oDOMParser.getDocument();
			
			// XRDS
			Element oElement = oDOMDoc.getDocumentElement();
			
			// Populate the cache and store the Descriptors from the response
			oXriD = new XRD(oElement, bKeepDOM);
		}
		catch (IOException oEx) {
			oEx.printStackTrace();
			throw new XRIParseException("IOException", oEx);
		}
		catch (SAXException oEx) {
			oEx.printStackTrace();
			throw new XRIParseException("SAXException", oEx);
		}
		finally {
			try {
				oIn.close();
			}
			catch (IOException e) {}
		}
		
		return oXriD;
	}


	/**
	 * This method populates the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
	public void fromDOM(Element oElem) throws URISyntaxException, ParseException
	{
		reset();
		
		// get the id attribute
		if (oElem.hasAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW))
			xmlID = oElem.getAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW);
		
		if (oElem.hasAttributeNS(Tags.NS_XML, Tags.ATTR_IDREF))
			idRef = oElem.getAttributeNS(Tags.NS_XML, Tags.ATTR_IDREF);
		
		if (oElem.hasAttributeNS(null, Tags.ATTR_XRD_VERSION))
			version = oElem.getAttributeNS(null, Tags.ATTR_XRD_VERSION);
	
		for (Element oChild = DOMUtils.getFirstChildElement(oElem); oChild != null; oChild = DOMUtils.getNextSiblingElement(oChild)) {
			
			String sChildName = oChild.getLocalName();
			if (sChildName == null) sChildName = oChild.getNodeName();
			
			if (sChildName.equals(Tags.TAG_TYPE))
			{
				XRDType t = new XRDType();
				t.fromXML(oChild);
				types.add(t);
			}
			else if (sChildName.equals(Tags.TAG_QUERY))
			{
				Query q = new Query();
				q.fromXML(oChild);
				this.query = q;
			}
			else if (sChildName.equals(Tags.TAG_STATUS))
			{
				Status s = new Status();
				s.fromXML(oChild);
				this.status = s;
			}
			else if (sChildName.equals(Tags.TAG_SERVERSTATUS))
			{
				ServerStatus s = new ServerStatus();
				s.fromXML(oChild);
				this.serverStatus = s;
			}
			else if (sChildName.equals(Tags.TAG_EXPIRES))
			{
				// only accept the first Expires element and make sure it
				expires = new Expires(
						DOMUtils.fromXMLDateTime(oChild.getFirstChild().getNodeValue())
				);	
			}
			else if (sChildName.equals(Tags.TAG_PROVIDERID))
			{
				ProviderID p = new ProviderID();
				p.fromXML(oChild);
				this.providerID = p;
			}
			else if (sChildName.equals(Tags.TAG_LOCALID))
			{
				addLocalID(new LocalID(oChild));
			}
			else if (sChildName.equals(Tags.TAG_EQUIVID))
			{
				equivIDs.add(new EquivID(oChild));
			}
			else if (sChildName.equals(Tags.TAG_CANONICALID))
			{
				canonicalIDs.add(new CanonicalID(oChild));
			}
			else if (sChildName.equals(Tags.TAG_CANONICALEQUIVID))
			{
				canonicalEquivID = new CanonicalEquivID();
				canonicalEquivID.fromXML(oChild);
			}
			else if (sChildName.equals(Tags.TAG_REDIRECT))
			{
				Redirect ref = new Redirect(oChild);
				addRedirect(ref);
			}
			else if (sChildName.equals(Tags.TAG_REF))
			{
				Ref ref = new Ref(oChild);
				addRef(ref);
			}
			else if (sChildName.equals(Tags.TAG_SERVICE))
			{
				addService(new Service(oChild));
			}
			else if (
					(oChild.getNamespaceURI() != null) &&
					oChild.getNamespaceURI().equals(Tags.NS_SAML) &&
					(oChild.getLocalName() != null) &&
					oChild.getLocalName().equals(Tags.TAG_ASSERTION))
			{
				samlAssertion = new Assertion(oChild);
			}
			// Added this code to support extensions in Authority Element
			else
			{
				Vector oVector =
					(Vector) moOtherChildrenVectorsMap.get(sChildName);
				
				if (oVector == null)
				{
					oVector = new Vector();
					moOtherChildrenVectorsMap.put(sChildName, oVector);
				}
				
				oVector.add(oChild.cloneNode(true));
			}
		}
	}


	/**
	 * Serializes the DOM associated with this XRD.  Will create DOM
	 * if no DOM is already stored.
	 * @param bIndent - whether or not to indent the XML.  If true will break
	 * signature.
	 * @param bOmitXMLDeclaration - whether or not to omit the XML preamble
	 */
	public String serializeDOM(bool bIndent, bool bOmitXMLDeclaration)
	{
		getDOM();
		return DOMUtils.toString(moElem, bIndent, bOmitXMLDeclaration);
	}


	/**
	 * Returns obj as a formatted XML string.
	 * @param sTab - The characters to prepend before each new line
	 */
	public String toString()
	{
		Document doc = new DocumentImpl();
		Element elm = this.toDOM(doc);
		doc.appendChild(elm);
		return DOMUtils.toString(doc); 
	}


	/**
	 * Returns this XRD with only the selected services (filtered and sorted) as XML.
	 * @return
	 */
	public String toResultString()
	{
		Document doc = new DocumentImpl();
		Element elm = this.toDOM(doc, true); // filtered
		doc.appendChild(elm);
		return DOMUtils.toString(doc);
	}



	/**
	 * This method returns DOM stored with this obj.  It may be cached and
	 * there is no guarantee as to which document it was created from
	 */
	public Element getDOM()
	{
		if (moElem == null)
		{
			moElem = toDOM(new DocumentImpl());
			moElem.getOwnerDocument().appendChild(moElem);
		}
		
		return moElem;
	}


	/**
	 *  This method will import an XRD from DOM, and hold on to it, as
	 * retrievable by getDOM.  The fromDOM method, on the otherhand, will not keep
	 * a copy of the DOM.
	 */
	public void setDOM(Element oElem)
	throws URISyntaxException, ParseException
	{
		fromDOM(oElem);
		moElem = oElem;
	}


	/**
	 * This method resets any DOM state in the XRI Descriptor.  It is useful for
	 * keeping memory consumption low.
	 */
	public void clearDOM()
	{
		moElem = null;
	}


	/**
	 *  This method create a DOM using the specified document.
	 *  If the <code>wantFiltered</code> argument is true, the returned
	 *  document will have all prioritized elements sorted and
	 *  only selected services are returned.
	 *  
	 * @param doc - The document to use for generating DOM
	 * @param wantFiltered - Get sorted+filtered Refs and Services
	 */
	public Element toDOM(Document doc, bool wantFiltered)
	{
		if (doc == null)
			return null;
		
		Element elem = doc.createElementNS(Tags.NS_XRD_V2, Tags.TAG_XRD);
		
		if (!xmlID.equals(""))
			elem.setAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW, xmlID);

		if (idRef != null)
			elem.setAttributeNS(Tags.NS_XML, Tags.ATTR_IDREF, idRef);

		if (version != null)
			elem.setAttributeNS(null, Tags.ATTR_XRD_VERSION, version);

		for (int i = 0; i < getNumTypes(); i++)
		{
			Element t = (Element) getTypeAt(i).toXML(doc);
			elem.appendChild(t);
		}
		
		if (query != null) {
			Element oResolved = query.toXML(doc);
			elem.appendChild(oResolved);
		}
		
		if (status != null) {
			Element oStatus = status.toXML(doc);
			elem.appendChild(oStatus);
		}
		
		if (serverStatus != null) {
			Element oServerStatus = serverStatus.toXML(doc);
			elem.appendChild(oServerStatus);
		}
		
		if (expires != null ) {
			Element oExpires = expires.toXML(doc);
			elem.appendChild(oExpires);
		}
		
		if (providerID != null ) {
			Element oProviderid = providerID.toXML(doc);
			elem.appendChild(oProviderid);
		}
		
		for (int i = 0; i < getNumLocalIDs(); i++)
		{
			Element oLocal = (Element) getLocalIDAt(i).toXML(doc);
			elem.appendChild(oLocal);
		}
		
		for (int i = 0; i < getNumEquivIDs(); i++)
		{
			Element equivID = (Element) getEquivIDAt(i).toXML(doc);
			elem.appendChild(equivID);
		}
		
		for (int i = 0; i < getNumCanonicalids(); i++)
		{
			Element oLocal = (Element) getCanonicalidAt(i).toXML(doc);
			elem.appendChild(oLocal);
		}
		
		if (canonicalEquivID != null)
			elem.appendChild(canonicalEquivID.toXML(doc));

		if (wantFiltered) {
			ArrayList redirects = getPrioritizedRedirects();
			for (int i =0; i < redirects.size(); i++){
				Redirect r = (Redirect)redirects.get(i);
				Element e = (Element) r.toXML(doc);
				elem.appendChild(e);
			}
			
			ArrayList refs = getPrioritizedRefs();
			for (int i =0; i < refs.size(); i++){
				Ref r = (Ref)refs.get(i);
				Element e = (Element) r.toXML(doc);
				elem.appendChild(e);
			}
			
			ArrayList svcs = getSelectedServices().getList();
			for (int i = 0; i < svcs.size(); i++)
			{
				Service s = (Service)svcs.get(i);
				Element e = (Element) s.toDOM(doc, true);
				elem.appendChild(e);
			}
		}
		else {
			for (int i =0; i < getNumRedirects(); i++){
				Element e = (Element) getRedirectAt(i).toXML(doc);
				elem.appendChild(e);
			}
			
			for (int i =0; i < getNumRefs(); i++){
				Element e = (Element) getRefAt(i).toXML(doc);
				elem.appendChild(e);
			}
			
			for (int i = 0; i < getNumServices(); i++)
			{
				Element e = (Element) getServiceAt(i).toDOM(doc);
				elem.appendChild(e);
			}
			
			if (samlAssertion != null)
			{
				Element oSAMLElement = (Element) samlAssertion.toDOM(doc);
				elem.appendChild(oSAMLElement);
			}
			
			Iterator oCustomTags = moOtherChildrenVectorsMap.keySet().iterator();
			while (oCustomTags.hasNext())
			{
				String sTag = (String) oCustomTags.next();
				Vector oValues = (Vector) moOtherChildrenVectorsMap.get(sTag);
				for (int i = 0; i < oValues.size(); i++)
				{
					Element oCustom = doc.createElement(sTag);
					
					// Importing the Child Node into New Document and also adding it to the
					// Service Element as a Child Node
					Node oChild = (Node) oValues.get(i);
					Node oChild2 = doc.importNode(oChild, true);
					elem.appendChild(oChild2);
				}
			}
		}
		
		return elem;
	}


	/**
	 *  This method will make DOM using the specified document.  If any DOM state
	 * has been stored with the obj, it will not be used in this method.
	 * This method generates a reference-free copy of new DOM.
	 * @param doc - The document to use for generating DOM
	 */
	public Element toDOM(Document doc)
	{
		return toDOM(doc, false);
	}


	/**
	 * Stores simple elements in the Service by Tag
	 *
	 * Here we are converting the String obj that is being passed into XML
	 * Element before storing it into moOtherChildrenVectorsMap Vector. The reason
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
			"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + sTagValue;
		bool returnValue = false;
		
		try {
			InputStream oIn = new ByteArrayInputStream(xmlStr.getBytes());
			DOMParser oDOMParser = DOMUtils.getDOMParser();
			oDOMParser.parse(new InputSource(oIn));
			Document oDOMDoc = oDOMParser.getDocument();
			Element oElement = oDOMDoc.getDocumentElement();
			
			Vector oVector = (Vector) moOtherChildrenVectorsMap.get(sTag);
			
			if (oVector == null) {
				oVector = new Vector();
				moOtherChildrenVectorsMap.put(sTag, oVector);
			}
			
			oVector.add(oElement.cloneNode(true));
			
			returnValue = true;
		}
		catch (Exception exp) {
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
		return (Vector) moOtherChildrenVectorsMap.get(sTag);
	}

	public void setExtension(String extension) throws URISyntaxException, ParseException {

		String xmlStr = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + 
		"<xrd xmlns=\"xri://$xrd*($v*2.0)\">" +
		extension
		+ "</xrd>";
		
		XRD tempXrd = parseXRD(xmlStr, false);
		
		this.moOtherChildrenVectorsMap = tempXrd.moOtherChildrenVectorsMap;
	}

	public String getExtension() {

		StringBuffer extension = new StringBuffer();
		
		Iterator oCustomTags = moOtherChildrenVectorsMap.keySet().iterator();
		while (oCustomTags.hasNext())
		{
			String sTag = (String) oCustomTags.next();
			Vector oValues = (Vector) moOtherChildrenVectorsMap.get(sTag);
			for (int i = 0; i < oValues.size(); i++)
			{
				Node oChild = (Node) oValues.get(i);
				
				extension.append(DOMUtils.toString((Element) oChild, true, true));
			}
		}

		return extension.toString();
	}

	/**
	 * Returns the id attribute
	 * @return String The authority id element
	 */
	public String getXmlID()
	{
		return xmlID;
		
	}


	/**
	 * Sets the id attribute
	 */
	public void setXmlID(String sVal)
	{
		xmlID = sVal;
	}


	/**
	 * Generates a new id attribute and sets it
	 */
	public void genXmlID()
	{
		xmlID = org.openxri.util.XMLUtils.genXmlID();
	}


	/**
	 * Returns the provider id element value
	 */
	public String getProviderID()
	{
		return (providerID != null) ? providerID.getValue(): null;
	}


	/**
	 * Sets the provider id element value
	 */
	public void setProviderID(String sVal)
	{
		providerID = new ProviderID(sVal);
	}


	/**
	 * Returns the query element value
	 */
	public String getQuery()
	{
		return (query != null)? query.getValue(): null;
	}
	
	
	/**
	 * Sets the query element value
	 */
	public void setQuery(String sVal)
	{
		if (query != null)
			query.setValue(sVal);
		else
			query = new Query(sVal);
	}
	
	
	/**
	 * Returns the expires element value
	 */
	public Date getExpires()
	{
		return (expires !=null)? expires.getDate(): null;
	}
	
	
	/**
	 * Sets the expires element value
	 */
	public void setExpires(Date d)
	{
		if (expires != null)
			expires.setDate(d);
		else
			expires = new Expires(d);
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
	
	
	public int getNumEquivIDs(){
		return equivIDs.size();
	}
	
	public EquivID getEquivIDAt(int n) {
		return (EquivID) equivIDs.get(n);
	}
	
	public void addEquivID(EquivID equivID) {
		equivIDs.add(equivID);
	}
	
	
	/**
	 * @deprecated
	 */
	public int getNumCanonicalids(){
		return canonicalIDs.size();
	}

	/**
	 * @deprecated
	 */
	public CanonicalID getCanonicalidAt(int n){
		return (CanonicalID) canonicalIDs.get(n);
	}

	/**
	 * @deprecated
	 */
	public void addCanonicalID(CanonicalID canonicalId) {		
		canonicalIDs.add(canonicalId);
	}

	/**
	 * Sets the CanonicalID
	 */
	public void setCanonicalID(CanonicalID cid) {
		canonicalIDs.clear();
		if (cid != null)
			canonicalIDs.add(cid);
	}

	/**
	 * Gets the CanonicalID
	 */
	public CanonicalID getCanonicalID() {
		if (canonicalIDs.size() > 0)
			return (CanonicalID) canonicalIDs.get(0);
		else
			return null;
	}


	/**
	 * Sets the CanonicalEquivID
	 */
	public void setCanonicalEquivID(CanonicalEquivID ceid) {
		canonicalEquivID = ceid;
	}

	/**
	 * Gets the CanonicalEquivID
	 */
	public CanonicalEquivID getCanonicalEquivID() {
		return canonicalEquivID;
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
		Integer priority = redirect.getPriority();		
		redirects.add(redirect);
		prioritizedRedirects.addObject((priority == null)? "null" : priority.toString(), redirect);
	}

	public ArrayList getPrioritizedRedirects() {
		return prioritizedRedirects.getList();
	}

	
	/**
	 * Returns the number of services elements
	 */
	public int getNumServices()
	{
		return services.size();
	}
	
	
	/**
	 * Returns the service element at the specified index
	 */
	public Service getServiceAt(int n)
	{
		return (Service) services.get(n);
	}

	
	/**
	 * Returns a vector of all the service elements
	 */
	public ArrayList getPrioritizedServices()
	{
		return prioritizedServices.getList();
	}
	
	
	/**
	 * Adds a service element
	 */
	public void addService(Service service)
	{
		services.add(service);
		Integer priority = service.getPriority();
		prioritizedServices.addObject((priority == null)? "null" : priority.toString(), service);
	}
	
	
	/**
	 * Returns the number of services elements
	 */
	public int getNumTypes()
	{
		return types.size();
	}
	
	
	/**
	 * Returns the Type element at the specified index
	 */
	public XRDType getTypeAt(int n)
	{
		return (XRDType) types.get(n);
	}

	
	/**
	 * Returns the SAML assertion
	 */
	public Assertion getSAMLAssertion()
	{
		return samlAssertion;
	}
	
	
	/**
	 * Sets the SAML assertion
	 */
	public void setSAMLAssertion(Assertion oSAMLAssertion)
	{
		samlAssertion = oSAMLAssertion;
	}
	
	
	/**
	 * This will sign the XRD using the provided Private Key.  The
	 * signature will be kept in DOM.  DOM will be created if it doesn't exist
	 * already.
	 * @param oKey - The private key to sign the descriptor with.
	 * @throws XMLSecurityException
	 */
	public void sign(PrivateKey oKey)
	throws XMLSecurityException
	{
		// build up the DOM (stored in moElem for future use)
		getDOM();
		
		// before signing, make sure that the document is properly normalized
		// this is separate from the XMLDSig canonicalization and is more for attributes, namespaces, etc.
		moElem.getOwnerDocument().normalizeDocument();
		
		Element oAssertionElem =
			DOMUtils.getFirstChildElement(
					moElem, Tags.NS_SAML, Tags.TAG_ASSERTION);
		if (oAssertionElem == null)
		{
			throw new XMLSecurityException(
			"Cannot create signature. No SAML Assertion attached to descriptor.");
		}
		
		Element oSubjectElem =
			DOMUtils.getFirstChildElement(
					oAssertionElem, Tags.NS_SAML, Tags.TAG_SUBJECT);
		if (oSubjectElem == null)
		{
			throw new XMLSecurityException(
			"Cannot create signature. SAML Assertion has no subject.");
		}
		
		// make sure the id attribute is present
		String sID = moElem.getAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW);
		if ((sID == null) || (sID.equals("")))
		{
			throw new XMLSecurityException(
					"Cannot create signature. ID is missing for " +
					moElem.getLocalName());
		}
		
		// Set the DOM so that it can be signed
		DOM3Utils.bestEffortSetIDAttr(moElem, Tags.NS_XML, Tags.ATTR_ID_LOW);
		
		// Build the empty signature.
		Document oDoc = moElem.getOwnerDocument();
		XMLSignature oSig =
			new XMLSignature(
					oDoc, null, XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA1,
					Canonicalizer.ALGO_ID_C14N_EXCL_OMIT_COMMENTS);
		
		// add all the transforms to the signature
		String[] oTransforms =
			new String[] { Transforms.TRANSFORM_ENVELOPED_SIGNATURE, Transforms.TRANSFORM_C14N_EXCL_OMIT_COMMENTS };
		Transforms oTrans = new Transforms(oSig.getDocument());
		for (int i = 0; i < oTransforms.length; i++)
		{
			oTrans.addTransform(oTransforms[i]);
		}
		oSig.addDocument("#" + sID, oTrans);
		
		// now finally sign the thing
		oSig.sign(oKey);
		
		// now sub in this element
		Element oSigElem = oSig.getElement();
		
		// insert the signature in the right place
		oAssertionElem.insertBefore(oSigElem, oSubjectElem);
		
	}
	
	
	/**
	 * This will verify the XRD against the given public key.  DOM
	 * must already be associated with this descriptor.
	 * @param oPubKey
	 * @throws XMLSecurityException
	 */
	public void verifySignature(PublicKey oPubKey)
	throws XMLSecurityException
	{
		if (moElem == null)
		{
			throw new XMLSecurityException(
			"Cannot verify the signature. No DOM stored for XRD");
		}
		
		// make sure the ID attribute is present
		String sIDAttr = Tags.ATTR_ID_LOW;
		String sIDAttrNS = Tags.NS_XML;
		String sID = moElem.getAttributeNS(sIDAttrNS, sIDAttr);
		if ((sID == null) || (sID.equals("")))
		{
			throw new XMLSecurityException(
					"Cannot verify the signature. ID is missing for " +
					moElem.getLocalName());
		}
		String sRef = "#" + sID;
		
		// Set the DOM so that it can be verified
		DOM3Utils.bestEffortSetIDAttr(moElem, sIDAttrNS, sIDAttr);
		
		Element oAssertionElem =
			DOMUtils.getFirstChildElement(
					moElem, Tags.NS_SAML, Tags.TAG_ASSERTION);
		
		if (oAssertionElem == null)
		{
			throw new XMLSecurityException(
			"Cannot verify the signature. No Assertion in XRD");
		}
		
		Element oSigElem =
			DOMUtils.getFirstChildElement(
					oAssertionElem, Tags.NS_XMLDSIG, Tags.TAG_SIGNATURE);
		
		if (oSigElem == null)
		{
			throw new XMLSecurityException(
			"Cannot verify the signature. No signature in Assertion");
		}
		
		// create the signature element to verify
		XMLSignature oSig = null;
		oSig = new XMLSignature(oSigElem, null);
		
		// Validate the signature content by checking the references
		String sFailedRef = null;
		SignedInfo oSignedInfo = oSig.getSignedInfo();
		if (oSignedInfo.getLength() != 1)
		{
			throw new XMLSecurityException(
					"Cannot verify the signature. Expected 1 reference, got " +
					oSignedInfo.getLength());
		}
		
		// make sure it references the correct element
		Reference oRef = oSignedInfo.item(0);
		String sURI = oRef.getURI();
		if (!sRef.equals(sURI))
		{
			throw new XMLSecurityException(
			"Cannot verify the signature. Reference URI did not match ID");
		}
		
		// check that the transforms are ok
		bool bEnvelopedFound = false;
		Transforms oTransforms = oRef.getTransforms();
		for (int i = 0; i < oTransforms.getLength(); i++)
		{
			String sTransform = oTransforms.item(i).getURI();
			if (Transforms.TRANSFORM_ENVELOPED_SIGNATURE.equals(sTransform))
			{
				// mark that we got the required transform
				bEnvelopedFound = true;
			}
			else if (
					!Transforms.TRANSFORM_C14N_EXCL_OMIT_COMMENTS.equals(
							sTransform))
			{
				// bonk if we don't have one of the two acceptable transforms
				throw new XMLSecurityException(
				"Unexpected transform in signature");
			}
		}
		
		if (!bEnvelopedFound)
		{
			throw new XMLSecurityException(
					"Could not find expected " +
					Transforms.TRANSFORM_ENVELOPED_SIGNATURE +
			" transform in signature");
		}
		
		// finally check the signature
		if (!oSig.checkSignatureValue(oPubKey))
		{
			throw new RuntimeException("Signature failed to verify.");
		}
		
	}
	
	
	/**
	 * Checks if this XRD is valid based on the optional Expires element
	 */
	public bool isValid()
	{
		// check to make sure the descriptor is not expired
		if ((expires !=null && expires.getDate() != null) && (expires.getDate().before(new Date())))
		{
			return false;
		}
		
		return true;
	}
	
	
	public void setStatus(Status status) {
		this.status = status;
	}
	
	/**
	 * @return Returns the status.
	 */
	public Status getStatus() {
		return status;
	}
	
	/**
	 * @return Returns the ServerStatus code.
	 */
	public String getServerStatusCode()
	{
		if (serverStatus == null)
			return null;
		return serverStatus.getCode();
	}
	
	
	/**
	 * Sets the ServerStatus
	 * @param serverStatus
	 */
	public void setServerStatus(ServerStatus serverStatus) {
		this.serverStatus = serverStatus;
	}
	
	/**
	 * @return Returns the ServerStatus.
	 */
	public ServerStatus getServerStatus() {
		return serverStatus;
	}
	
	
	/**
	 * @return Returns the status code.
	 */
	public String getStatusCode()
	{
		if (status == null)
			return null;
		return status.getCode();
	}
	
	
	
	/**
	 * @return Returns a copy of the collection of services as it appears in the original XRD
	 */
	public Vector getServices() {
		return (Vector)services.clone();
	}
	
	
	public void setServices(Collection col) {
		services = new Vector();
		prioritizedServices = new PrioritizedList();
		
		Iterator i = col.iterator();
		while (i.hasNext()) {
			Service s = (Service)i.next();
			addService(s);
		}
	}
	
	
	public void setSelectedServices (Collection svcs) {
		selectedServices = new PrioritizedList();
		Iterator i = svcs.iterator();
		while (i.hasNext()) {
			Service s = (Service)i.next();
			Integer priority = s.getPriority();
			String priStr = (priority == null)? "null" : priority.toString();
			selectedServices.addObject(priStr, s);
		}
	}
	
	public void setEquivIDs(Collection col) {
		equivIDs = new Vector();
		
		Iterator i = col.iterator();
		while (i.hasNext()) {
			EquivID e = (EquivID)i.next();
			addEquivID(e);
		}
	}
	
	public void setRefs(Collection col) {
		refs = new Vector();
		
		Iterator i = col.iterator();
		while (i.hasNext()) {
			Ref r = (Ref)i.next();
			addRef(r);
		}
	}
	
	public void setRedirects(Collection col) {
		redirects = new Vector();
		
		Iterator i = col.iterator();
		while (i.hasNext()) {
			Redirect r = (Redirect)i.next();
			addRedirect(r);
		}
	}
	
	/**
	 * @return Returns the selectedServices.
	 */
	public PrioritizedList getSelectedServices() {
		return selectedServices;
	}
	
}
