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


namespace DotNetXri.Client.Xml {

	//using java.io.ByteArrayInputStream;
	//using java.io.IOException;
	//using java.io.InputStream;
	//using java.io.Serializable;
	//using java.net.UriFormatException;
	//using java.security.PrivateKey;
	//using java.security.PublicKey;
	//using java.text.ParseException;
	//using java.util.ArrayList;
	//using java.util.Collection;
	//using java.util.Date;
	//using java.util.Hashtable;
	//using java.util.IEnumerator;
	//using java.util.ArrayList;

	//using org.apache.xerces.dom.XmlDocument;
	//using org.apache.xerces.parsers.DOMParser;
	//using org.apache.xml.security.c14n.Canonicalizer;
	//using org.apache.xml.security.exceptions.XMLSecurityException;
	//using org.apache.xml.security.signature.Reference;
	//using org.apache.xml.security.signature.SignedInfo;
	//using org.apache.xml.security.signature.XMLSignature;
	//using org.apache.xml.security.transforms.Transforms;
	//using org.openxri.XRIParseException;
	//using org.openxri.resolve.exception.XRIResolutionException;
	//using org.openxri.saml.Assertion;
	//using org.openxri.util.DOM3Utils;
	//using org.openxri.util.DOMUtils;
	//using org.openxri.util.PrioritizedList;
	//using org.w3c.dom.XmlDocument;
	//using org.w3c.dom.XmlElement;
	//using org.w3c.dom.Node;
	//using org.xml.sax.InputSource;
	//using org.xml.sax.SAXException;
	using DotNetXri.Loggers;
	using System;
	using System.Collections;
	using System.Text;
	using DotNetXri.Client.Util;
	using System.Collections.Generic;
	using System.Xml;
	using DotNetXri.Client.Saml;
	using DotNetXri.Syntax;
	using System.IO;


	/**
	* This class describes the XRD XML element used for XRI Authority
	* resolution.
	*
	* @author =chetan
	* @author =wil
	*/
	public class XRD : Cloneable, Serializable {

		public const string CURRENT_VERSION = "2.0";

		private static ILog soLog = Logger.Create(typeof(XRD));

		// optional attributes
		private string xmlID;
		private string idRef;
		private string version;

		// other elements
		private ArrayList types;
		private Query query;
		private Status status;
		private ServerStatus serverStatus;
		private Expires expires;
		private ProviderID providerID;

		private ArrayList localIDs;
		private ArrayList equivIDs;
		private ArrayList canonicalIDs;
		private CanonicalEquivID canonicalEquivID;

		private ArrayList redirects;
		private PrioritizedList prioritizedRedirects;

		private ArrayList refs;
		private PrioritizedList prioritizedRefs;

		private ArrayList services;
		private PrioritizedList prioritizedServices;
		private PrioritizedList selectedServices;

		private Assertion samlAssertion;

		private XmlElement moElem;
		private Dictionary<object, object> moOtherChildrenVectorsMap;


		static XRD() {/*
			try {
				org.apache.xml.security.Init.init();
			} catch (Exception oEx) {
				soLog.Warn("Failed to initialize XML Sec library", oEx);
			}*/
		}


		/**
		* Constructs an empty XRD
		*/
		public XRD() {
			reset();
		}


		/**
		* This method resets the state of the XRD.
		*/
		public void reset() {
			xmlID = "";
			idRef = null;
			version = null;
			types = new ArrayList();
			query = null;
			status = null;
			serverStatus = null;
			expires = null;
			providerID = null;

			localIDs = new ArrayList();
			equivIDs = new ArrayList();
			canonicalIDs = new ArrayList(); // CanonicalID (TODO: Should be 0 or 1)
			canonicalEquivID = null;

			refs = new ArrayList();
			prioritizedRefs = new PrioritizedList();

			redirects = new ArrayList();
			prioritizedRedirects = new PrioritizedList();

			services = new ArrayList();
			prioritizedServices = new PrioritizedList();
			selectedServices = new PrioritizedList();

			samlAssertion = null;

			moElem = null;
			moOtherChildrenVectorsMap = new Dictionary<object, object>();
		}


		/**
		* Clone this obj
		*/
		//public Object clone() {
		//    XRD x = new XRD();

		//    x.xmlID = xmlID;
		//    x.idRef = idRef;
		//    x.version = version;
		//    if (types != null)
		//        x.types = (ArrayList)types.clone();
		//    if (query != null)
		//        x.query = new Query(query);
		//    if (status != null)
		//        x.status = new Status(status);
		//    if (serverStatus != null)
		//        x.serverStatus = new ServerStatus(serverStatus);
		//    if (expires != null)
		//        x.expires = new Expires(expires);
		//    if (providerID != null)
		//        x.providerID = new ProviderID(providerID);

		//    if (localIDs != null)
		//        x.localIDs = (ArrayList)localIDs.clone();
		//    if (equivIDs != null)
		//        x.equivIDs = (ArrayList)equivIDs.clone();
		//    if (canonicalIDs != null)
		//        x.canonicalIDs = (ArrayList)canonicalIDs.clone();
		//    if (canonicalEquivID != null)
		//        x.canonicalEquivID = new CanonicalEquivID(canonicalEquivID);

		//    if (refs != null)
		//        x.refs = (ArrayList)refs.clone();
		//    if (prioritizedRefs != null)
		//        x.prioritizedRefs = (PrioritizedList)prioritizedRefs.clone();

		//    if (redirects != null)
		//        x.redirects = (ArrayList)redirects.clone();
		//    if (prioritizedRedirects != null)
		//        x.prioritizedRedirects = (PrioritizedList)prioritizedRedirects.clone();

		//    if (services != null)
		//        x.services = (ArrayList)services.clone();
		//    if (prioritizedServices != null)
		//        x.prioritizedServices = (PrioritizedList)prioritizedServices.clone();
		//    if (selectedServices != null)
		//        x.selectedServices = (PrioritizedList)selectedServices.clone();

		//    // XXX shallow copy here!
		//    x.samlAssertion = samlAssertion;
		//    x.moElem = moElem;

		//    if (moOtherChildrenVectorsMap != null)
		//        x.moOtherChildrenVectorsMap = (Dictionary<object, object>)moOtherChildrenVectorsMap.clone();

		//    return x;
		//}



		/**
		*  This method creates the XRD from DOM.  It optionally keeps a
		* pointer to the DOM.
		* @param oElem - The DOM to create the obj from
		* @param bKeepDOM - If true, will keep a copy of the DOM with the obj
		*/
		public XRD(XmlElement oElem, bool bKeepDOM) //throws UriFormatException, ParseException
		{
			if (bKeepDOM) {
				setDOM(oElem);
			} else {
				fromDOM(oElem);
			}
		}


		/**
		* Constructs XRD from a string
		*/
		public static XRD parseXRD(string xmlStr, bool bKeepDOM)
			//throws UriFormatException, ParseException
		{
			XRD oXriD = null;

			try {
				XmlDocument oDOMDoc = new XmlDocument();
				oDOMDoc.LoadXml(xmlStr);

				// XRDS
				XmlElement oElement = oDOMDoc.DocumentElement;

				// Populate the cache and store the Descriptors from the response
				oXriD = new XRD(oElement, bKeepDOM);
			} catch (IOException oEx) {
				soLog.Error("IOException", oEx);
				throw new XRIParseException("IOException", oEx);
			} catch (XmlException oEx) {
				soLog.Error("XmlException", oEx);
				throw new XRIParseException("XmlException", oEx);
			}

			return oXriD;
		}


		/**
		* This method populates the obj from DOM.  It does not keep a
		* copy of the DOM around.  Whitespace information is lost in this process.
		*/
		public void fromDOM(XmlElement oElem) //throws UriFormatException, ParseException
		{
			reset();

			// get the id attribute
			if (oElem.hasAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW))
				xmlID = oElem.getAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW);

			if (oElem.hasAttributeNS(Tags.NS_XML, Tags.ATTR_IDREF))
				idRef = oElem.getAttributeNS(Tags.NS_XML, Tags.ATTR_IDREF);

			if (oElem.hasAttributeNS(null, Tags.ATTR_XRD_VERSION))
				version = oElem.getAttributeNS(null, Tags.ATTR_XRD_VERSION);

			for (XmlElement oChild = oElem.FirstChild; oChild != null; oChild = oChild.NextSibling) {

				string sChildName = oChild.LocalName;
				if (sChildName == null) sChildName = oChild.getNodeName();

				if (sChildName.Equals(Tags.TAG_TYPE)) {
					XRDType t = new XRDType();
					t.fromXML(oChild);
					types.Add(t);
				} else if (sChildName.Equals(Tags.TAG_QUERY)) {
					Query q = new Query();
					q.fromXML(oChild);
					this.query = q;
				} else if (sChildName.Equals(Tags.TAG_STATUS)) {
					Status s = new Status();
					s.fromXML(oChild);
					this.status = s;
				} else if (sChildName.Equals(Tags.TAG_SERVERSTATUS)) {
					ServerStatus s = new ServerStatus();
					s.fromXML(oChild);
					this.serverStatus = s;
				} else if (sChildName.Equals(Tags.TAG_EXPIRES)) {
					// only accept the first Expires element and make sure it
					expires = new Expires(XmlConvert.ToDateTime(oChild.FirstChild.Value));
				} else if (sChildName.Equals(Tags.TAG_PROVIDERID)) {
					ProviderID p = new ProviderID();
					p.fromXML(oChild);
					this.providerID = p;
				} else if (sChildName.Equals(Tags.TAG_LOCALID)) {
					addLocalID(new LocalID(oChild));
				} else if (sChildName.Equals(Tags.TAG_EQUIVID)) {
					equivIDs.Add(new EquivID(oChild));
				} else if (sChildName.Equals(Tags.TAG_CANONICALID)) {
					canonicalIDs.Add(new CanonicalID(oChild));
				} else if (sChildName.Equals(Tags.TAG_CANONICALEQUIVID)) {
					canonicalEquivID = new CanonicalEquivID();
					canonicalEquivID.fromXML(oChild);
				} else if (sChildName.Equals(Tags.TAG_REDIRECT)) {
					Redirect _ref = new Redirect(oChild);
					addRedirect(_ref);
				} else if (sChildName.Equals(Tags.TAG_REF)) {
					Ref _ref = new Ref(oChild);
					addRef(_ref);
				} else if (sChildName.Equals(Tags.TAG_SERVICE)) {
					addService(new Service(oChild));
				} else if (
						  (oChild.NamespaceURI != null) &&
						  oChild.NamespaceURI.Equals(Tags.NS_SAML) &&
						  (oChild.LocalName != null) &&
						  oChild.LocalName.Equals(Tags.TAG_ASSERTION)) {
					samlAssertion = new Assertion(oChild);
				}
					// Added this code to support extensions in Authority XmlElement
				  else {
					ArrayList oVector =
						(ArrayList)moOtherChildrenVectorsMap[sChildName];

					if (oVector == null) {
						oVector = new ArrayList();
						moOtherChildrenVectorsMap[sChildName] = oVector;
					}

					oVector.Add(oChild.CloneNode(true));
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
		public string serializeDOM(bool bIndent, bool bOmitXMLDeclaration) {
			getDOM();
			return DOMUtils.ToString(moElem, bIndent, bOmitXMLDeclaration);
		}


		/**
		* Returns obj as a formatted XML string.
		* @param sTab - The characters to prepend before each new line
		*/
		public override string ToString() {
			XmlDocument doc = new XmlDocument();
			XmlElement elm = this.toDOM(doc);
			doc.AppendChild(elm);
			return doc.OuterXml;
		}


		/**
		* Returns this XRD with only the selected services (filtered and sorted) as XML.
		* @return
		*/
		public string toResultString() {
			XmlDocument doc = new XmlDocument();
			XmlElement elm = this.toDOM(doc, true); // filtered
			doc.AppendChild(elm);
			return doc.OuterXml;
		}



		/**
		* This method returns DOM stored with this obj.  It may be cached and
		* there is no guarantee as to which document it was created from
		*/
		public XmlElement getDOM() {
			if (moElem == null) {
				moElem = toDOM(new XmlDocument());
				moElem.OwnerDocument.AppendChild(moElem);
			}

			return moElem;
		}


		/**
		*  This method will import an XRD from DOM, and hold on to it, as
		* retrievable by getDOM.  The fromDOM method, on the otherhand, will not keep
		* a copy of the DOM.
		*/
		public void setDOM(XmlElement oElem)
			//throws UriFormatException, ParseException
		{
			fromDOM(oElem);
			moElem = oElem;
		}


		/**
		* This method resets any DOM state in the XRI Descriptor.  It is useful for
		* keeping memory consumption low.
		*/
		public void clearDOM() {
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
		public XmlElement toDOM(XmlDocument doc, bool wantFiltered) {
			if (doc == null)
				return null;

			XmlElement elem = doc.CreateElement(Tags.TAG_XRD, Tags.NS_XRD_V2);

			if (!xmlID.Equals(""))
				elem.SetAttribute(Tags.ATTR_ID_LOW, Tags.NS_XML, xmlID);

			if (idRef != null)
				elem.SetAttribute(Tags.ATTR_IDREF, Tags.NS_XML, idRef);

			if (version != null)
				elem.SetAttribute(Tags.ATTR_XRD_VERSION, null, version);

			for (int i = 0; i < getNumTypes(); i++) {
				XmlElement t = (XmlElement)getTypeAt(i).toXML(doc);
				elem.AppendChild(t);
			}

			if (query != null) {
				XmlElement oResolved = query.toXML(doc);
				elem.AppendChild(oResolved);
			}

			if (status != null) {
				XmlElement oStatus = status.toXML(doc);
				elem.AppendChild(oStatus);
			}

			if (serverStatus != null) {
				XmlElement oServerStatus = serverStatus.toXML(doc);
				elem.AppendChild(oServerStatus);
			}

			if (expires != null) {
				XmlElement oExpires = expires.toXML(doc);
				elem.AppendChild(oExpires);
			}

			if (providerID != null) {
				XmlElement oProviderid = providerID.toXML(doc);
				elem.AppendChild(oProviderid);
			}

			for (int i = 0; i < getNumLocalIDs(); i++) {
				XmlElement oLocal = (XmlElement)getLocalIDAt(i).toXML(doc);
				elem.AppendChild(oLocal);
			}

			for (int i = 0; i < getNumEquivIDs(); i++) {
				XmlElement equivID = (XmlElement)getEquivIDAt(i).toXML(doc);
				elem.AppendChild(equivID);
			}

			for (int i = 0; i < getNumCanonicalids(); i++) {
				XmlElement oLocal = (XmlElement)getCanonicalidAt(i).toXML(doc);
				elem.AppendChild(oLocal);
			}

			if (canonicalEquivID != null)
				elem.AppendChild(canonicalEquivID.toXML(doc));

			if (wantFiltered) {
				ArrayList redirects = getPrioritizedRedirects();
				for (int i = 0; i < redirects.Count; i++) {
					Redirect r = (Redirect)redirects[i];
					XmlElement e = (XmlElement)r.toXML(doc);
					elem.AppendChild(e);
				}

				ArrayList refs = getPrioritizedRefs();
				for (int i = 0; i < refs.Count; i++) {
					Ref r = (Ref)refs[i];
					XmlElement e = (XmlElement)r.toXML(doc);
					elem.AppendChild(e);
				}

				ArrayList svcs = getSelectedServices().getList();
				for (int i = 0; i < svcs.Count; i++) {
					Service s = (Service)svcs[i];
					XmlElement e = (XmlElement)s.toDOM(doc, true);
					elem.AppendChild(e);
				}
			} else {
				for (int i = 0; i < getNumRedirects(); i++) {
					XmlElement e = (XmlElement)getRedirectAt(i).toXML(doc);
					elem.AppendChild(e);
				}

				for (int i = 0; i < getNumRefs(); i++) {
					XmlElement e = (XmlElement)getRefAt(i).toXML(doc);
					elem.AppendChild(e);
				}

				for (int i = 0; i < getNumServices(); i++) {
					XmlElement e = (XmlElement)getServiceAt(i).toDOM(doc);
					elem.AppendChild(e);
				}

				if (samlAssertion != null) {
					XmlElement oSAMLElement = (XmlElement)samlAssertion.toDOM(doc);
					elem.AppendChild(oSAMLElement);
				}

				IEnumerator oCustomTags = moOtherChildrenVectorsMap.Keys.GetEnumerator();
				while (oCustomTags.MoveNext()) {
					string sTag = (string)oCustomTags.Current;
					ArrayList oValues = (ArrayList)moOtherChildrenVectorsMap[sTag];
					for (int i = 0; i < oValues.Count; i++) {
						XmlElement oCustom = doc.CreateElement(sTag);

						// Importing the Child Node into New XmlDocument and also adding it to the
						// Service XmlElement as a Child Node
						XmlNode oChild = (XmlNode)oValues[i];
						XmlNode oChild2 = doc.importNode(oChild, true);
						elem.AppendChild(oChild2);
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
		public XmlElement toDOM(XmlDocument doc) {
			return toDOM(doc, false);
		}


		/**
		* Stores simple elements in the Service by Tag
		*
		* Here we are converting the string obj that is being passed into XML
		* XmlElement before storing it into moOtherChildrenVectorsMap ArrayList. The reason
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
		public bool setOtherTagValues(string sTag, string sTagValue) {
			string xmlStr =
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + sTagValue;
			bool returnValue = false;

			try {
				XmlDocument oDOMDoc = new XmlDocument();
				oDOMDoc.LoadXml(xmlStr);
				XmlElement oElement = oDOMDoc.DocumentElement;

				ArrayList oVector = (ArrayList)moOtherChildrenVectorsMap[sTag];

				if (oVector == null) {
					oVector = new ArrayList();
					moOtherChildrenVectorsMap[sTag] = oVector;
				}

				oVector.Add(oElement.CloneNode(true));

				returnValue = true;
			} catch (Exception exp) {
				soLog.Error("Exception", exp);
				returnValue = false;
			}

			return returnValue;
		}


		/**
		* Returns unspecified simple elements in the Service by Tag
		* @param sTag - The tag name to get values for
		* @return a vector of text values whose element tag names match sTag
		*/
		public ArrayList getOtherTagValues(string sTag) {
			return (ArrayList)moOtherChildrenVectorsMap[sTag];
		}

		public void setExtension(string extension) //throws UriFormatException, ParseException {
		{
			string xmlStr = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
			"<xrd xmlns=\"xri://$xrd*($v*2.0)\">" +
			extension
			+ "</xrd>";

			XRD tempXrd = parseXRD(xmlStr, false);

			this.moOtherChildrenVectorsMap = tempXrd.moOtherChildrenVectorsMap;
		}

		public string getExtension() {

			StringBuilder extension = new StringBuilder();

			IEnumerator oCustomTags = moOtherChildrenVectorsMap.Keys.GetEnumerator();
			while (oCustomTags.MoveNext()) {
				string sTag = (string)oCustomTags.Current;
				ArrayList oValues = (ArrayList)moOtherChildrenVectorsMap[sTag];
				for (int i = 0; i < oValues.Count; i++) {
					XmlNode oChild = (XmlNode)oValues[i];

					extension.Append(DOMUtils.ToString((XmlElement)oChild, true, true));
				}
			}

			return extension.ToString();
		}

		/**
		* Returns the id attribute
		* @return string The authority id element
		*/
		public string getXmlID() {
			return xmlID;

		}


		/**
		* Sets the id attribute
		*/
		public void setXmlID(string sVal) {
			xmlID = sVal;
		}


		/**
		* Generates a new id attribute and sets it
		*/
		public void genXmlID() {
			xmlID = org.openxri.util.XMLUtils.genXmlID();
		}


		/**
		* Returns the provider id element value
		*/
		public string getProviderID() {
			return (providerID != null) ? providerID.getValue() : null;
		}


		/**
		* Sets the provider id element value
		*/
		public void setProviderID(string sVal) {
			providerID = new ProviderID(sVal);
		}


		/**
		* Returns the query element value
		*/
		public string getQuery() {
			return (query != null) ? query.getValue() : null;
		}


		/**
		* Sets the query element value
		*/
		public void setQuery(string sVal) {
			if (query != null)
				query.setValue(sVal);
			else
				query = new Query(sVal);
		}


		/**
		* Returns the expires element value
		*/
		public DateTime? getExpires() {
			return (expires != null) ? (DateTime?)expires.getDate() : null;
		}


		/**
		* Sets the expires element value
		*/
		public void setExpires(DateTime? d) {
			if (expires != null)
				expires.setDate(d);
			else
				expires = new Expires(d);
		}


		public int getNumLocalIDs() {
			return localIDs.Count;
		}

		public LocalID getLocalIDAt(int n) {
			return (LocalID)localIDs[n];
		}

		public void addLocalID(LocalID localId) {
			localIDs.Add(localId);
		}


		public int getNumEquivIDs() {
			return equivIDs.Count;
		}

		public EquivID getEquivIDAt(int n) {
			return (EquivID)equivIDs[n];
		}

		public void addEquivID(EquivID equivID) {
			equivIDs.Add(equivID);
		}


		/**
		* @deprecated
		*/
		public int getNumCanonicalids() {
			return canonicalIDs.Count;
		}

		/**
		* @deprecated
		*/
		public CanonicalID getCanonicalidAt(int n) {
			return (CanonicalID)canonicalIDs[n];
		}

		/**
		* @deprecated
		*/
		public void addCanonicalID(CanonicalID canonicalId) {
			canonicalIDs.Add(canonicalId);
		}

		/**
		* Sets the CanonicalID
		*/
		public void setCanonicalID(CanonicalID cid) {
			canonicalIDs.Clear();
			if (cid != null)
				canonicalIDs.Add(cid);
		}

		/**
		* Gets the CanonicalID
		*/
		public CanonicalID getCanonicalID() {
			if (canonicalIDs.Count > 0)
				return (CanonicalID)canonicalIDs[0];
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
		public ArrayList getRefs() {
			return (ArrayList)refs.clone();
		}


		public Ref getRefAt(int n) {
			return (Ref)refs[n];
		}


		public int getNumRefs() {
			return refs.Count;
		}


		public void addRef(Ref _ref) {
			int? priority = _ref.getPriority();
			refs.Add(_ref);
			prioritizedRefs.addObject((priority == null) ? "null" : priority.ToString(), _ref);
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


		public Redirect getRedirectAt(int n) {
			return (Redirect)redirects[n];
		}


		public int getNumRedirects() {
			return redirects.Count;
		}


		public void addRedirect(Redirect redirect) {
			int? priority = redirect.getPriority();
			redirects.Add(redirect);
			prioritizedRedirects.addObject((priority == null) ? "null" : priority.ToString(), redirect);
		}

		public ArrayList getPrioritizedRedirects() {
			return prioritizedRedirects.getList();
		}


		/**
		* Returns the number of services elements
		*/
		public int getNumServices() {
			return services.Count;
		}


		/**
		* Returns the service element at the specified index
		*/
		public Service getServiceAt(int n) {
			return (Service)services[n];
		}


		/**
		* Returns a vector of all the service elements
		*/
		public ArrayList getPrioritizedServices() {
			return prioritizedServices.getList();
		}


		/**
		* Adds a service element
		*/
		public void addService(Service service) {
			services.Add(service);
			int? priority = service.getPriority();
			prioritizedServices.addObject((priority == null) ? "null" : priority.ToString(), service);
		}


		/**
		* Returns the number of services elements
		*/
		public int getNumTypes() {
			return types.Count;
		}


		/**
		* Returns the Type element at the specified index
		*/
		public XRDType getTypeAt(int n) {
			return (XRDType)types[n];
		}


		/**
		* Returns the SAML assertion
		*/
		public Assertion getSAMLAssertion() {
			return samlAssertion;
		}


		/**
		* Sets the SAML assertion
		*/
		public void setSAMLAssertion(Assertion oSAMLAssertion) {
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
			//throws XMLSecurityException
		{
			// build up the DOM (stored in moElem for future use)
			getDOM();

			// before signing, make sure that the document is properly normalized
			// this is separate from the XMLDSig canonicalization and is more for attributes, namespaces, etc.
			moElem.getOwnerDocument().normalizeDocument();

			XmlElement oAssertionElem =
				DOMUtils.getFirstChildElement(
						moElem, Tags.NS_SAML, Tags.TAG_ASSERTION);
			if (oAssertionElem == null) {
				throw new XMLSecurityException(
				"Cannot create signature. No SAML Assertion attached to descriptor.");
			}

			XmlElement oSubjectElem =
				DOMUtils.getFirstChildElement(
						oAssertionElem, Tags.NS_SAML, Tags.TAG_SUBJECT);
			if (oSubjectElem == null) {
				throw new XMLSecurityException(
				"Cannot create signature. SAML Assertion has no subject.");
			}

			// make sure the id attribute is present
			string sID = moElem.getAttributeNS(Tags.NS_XML, Tags.ATTR_ID_LOW);
			if ((sID == null) || (sID.Equals(""))) {
				throw new XMLSecurityException(
						"Cannot create signature. ID is missing for " +
						moElem.LocalName);
			}

			// Set the DOM so that it can be signed
			DOM3Utils.bestEffortSetIDAttr(moElem, Tags.NS_XML, Tags.ATTR_ID_LOW);

			// Build the empty signature.
			XmlDocument oDoc = moElem.getOwnerDocument();
			XMLSignature oSig =
				new XMLSignature(
						oDoc, null, XMLSignature.ALGO_ID_SIGNATURE_RSA_SHA1,
						Canonicalizer.ALGO_ID_C14N_EXCL_OMIT_COMMENTS);

			// add all the transforms to the signature
			string[] oTransforms =
				new string[] { Transforms.TRANSFORM_ENVELOPED_SIGNATURE, Transforms.TRANSFORM_C14N_EXCL_OMIT_COMMENTS };
			Transforms oTrans = new Transforms(oSig.getDocument());
			for (int i = 0; i < oTransforms.Length; i++) {
				oTrans.addTransform(oTransforms[i]);
			}
			oSig.addDocument("#" + sID, oTrans);

			// now finally sign the thing
			oSig.sign(oKey);

			// now sub in this element
			XmlElement oSigElem = oSig.getElement();

			// insert the signature in the right place
			oAssertionElem.InsertBefore(oSigElem, oSubjectElem);

		}


		/**
		* This will verify the XRD against the given public key.  DOM
		* must already be associated with this descriptor.
		* @param oPubKey
		* @throws XMLSecurityException
		*/
		public void verifySignature(PublicKey oPubKey)
			//throws XMLSecurityException
		{
			if (moElem == null) {
				throw new XMLSecurityException(
				"Cannot verify the signature. No DOM stored for XRD");
			}

			// make sure the ID attribute is present
			string sIDAttr = Tags.ATTR_ID_LOW;
			string sIDAttrNS = Tags.NS_XML;
			string sID = moElem.getAttributeNS(sIDAttrNS, sIDAttr);
			if ((sID == null) || (sID.Equals(""))) {
				throw new XMLSecurityException(
						"Cannot verify the signature. ID is missing for " +
						moElem.LocalName);
			}
			string sRef = "#" + sID;

			// Set the DOM so that it can be verified
			DOM3Utils.bestEffortSetIDAttr(moElem, sIDAttrNS, sIDAttr);

			XmlElement oAssertionElem =
				DOMUtils.getFirstChildElement(
						moElem, Tags.NS_SAML, Tags.TAG_ASSERTION);

			if (oAssertionElem == null) {
				throw new XMLSecurityException(
				"Cannot verify the signature. No Assertion in XRD");
			}

			XmlElement oSigElem =
				DOMUtils.getFirstChildElement(
						oAssertionElem, Tags.NS_XMLDSIG, Tags.TAG_SIGNATURE);

			if (oSigElem == null) {
				throw new XMLSecurityException(
				"Cannot verify the signature. No signature in Assertion");
			}

			// create the signature element to verify
			XMLSignature oSig = null;
			oSig = new XMLSignature(oSigElem, null);

			// Validate the signature content by checking the references
			string sFailedRef = null;
			SignedInfo oSignedInfo = oSig.getSignedInfo();
			if (oSignedInfo.getLength() != 1) {
				throw new XMLSecurityException(
						"Cannot verify the signature. Expected 1 reference, got " +
						oSignedInfo.getLength());
			}

			// make sure it references the correct element
			Reference oRef = oSignedInfo.item(0);
			string sURI = oRef.getURI();
			if (!sRef.Equals(sURI)) {
				throw new XMLSecurityException(
				"Cannot verify the signature. Reference Uri did not match ID");
			}

			// check that the transforms are ok
			bool bEnvelopedFound = false;
			Transforms oTransforms = oRef.getTransforms();
			for (int i = 0; i < oTransforms.getLength(); i++) {
				string sTransform = oTransforms.item(i).getURI();
				if (Transforms.TRANSFORM_ENVELOPED_SIGNATURE.Equals(sTransform)) {
					// mark that we got the required transform
					bEnvelopedFound = true;
				} else if (
						!Transforms.TRANSFORM_C14N_EXCL_OMIT_COMMENTS.Equals(
								sTransform)) {
					// bonk if we don't have one of the two acceptable transforms
					throw new XMLSecurityException(
					"Unexpected transform in signature");
				}
			}

			if (!bEnvelopedFound) {
				throw new XMLSecurityException(
						"Could not find expected " +
						Transforms.TRANSFORM_ENVELOPED_SIGNATURE +
				" transform in signature");
			}

			// finally check the signature
			if (!oSig.checkSignatureValue(oPubKey)) {
				throw new RuntimeException("Signature failed to verify.");
			}

		}


		/**
		* Checks if this XRD is valid based on the optional Expires element
		*/
		public bool isValid() {
			// check to make sure the descriptor is not expired
			if ((expires != null && expires.getDate() != null) && (expires.getDate().before(new DateTime?()))) {
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
		public string getServerStatusCode() {
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
		public string getStatusCode() {
			if (status == null)
				return null;
			return status.getCode();
		}



		/**
		* @return Returns a copy of the collection of services as it appears in the original XRD
		*/
		public ArrayList getServices() {
			return (ArrayList)services.clone();
		}


		public void setServices(Collection col) {
			services = new ArrayList();
			prioritizedServices = new PrioritizedList();

			IEnumerator i = col.GetEnumerator();
			while (i.MoveNext()) {
				Service s = (Service)i.Current;
				addService(s);
			}
		}


		public void setSelectedServices(Collection svcs) {
			selectedServices = new PrioritizedList();
			IEnumerator i = svcs.GetEnumerator();
			while (i.MoveNext()) {
				Service s = (Service)i.Current;
				int? priority = s.getPriority();
				string priStr = (priority == null) ? "null" : priority.ToString();
				selectedServices.addObject(priStr, s);
			}
		}

		public void setEquivIDs(Collection col) {
			equivIDs = new ArrayList();

			IEnumerator i = col.GetEnumerator();
			while (i.MoveNext()) {
				EquivID e = (EquivID)i.Current;
				addEquivID(e);
			}
		}

		public void setRefs(Collection col) {
			refs = new ArrayList();

			IEnumerator i = col.GetEnumerator();
			while (i.MoveNext()) {
				Ref r = (Ref)i.Current;
				addRef(r);
			}
		}

		public void setRedirects(Collection col) {
			redirects = new ArrayList();

			IEnumerator i = col.GetEnumerator();
			while (i.MoveNext()) {
				Redirect r = (Redirect)i.Current;
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
}