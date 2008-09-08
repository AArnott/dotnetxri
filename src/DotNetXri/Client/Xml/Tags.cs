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

using org.apache.xml.security.utils.Constants;


/*
********************************************************************************
* Class: Tags
********************************************************************************
*/
/**
* This class contains constants found in the XML during XRI resolution
* @author =chetan
*/
public class Tags {

	/** 
		* name space constants & meta for xri service resolution
		* section 2.1.2 of 18th March2006 V2.0 wd10,XRI resolution document.
	*/
	public const string SERVICE_AUTH_RES = "xri://$res*auth*($v*2.0)";

	public const string NS_XML = "http://www.w3.org/XML/1998/namespace";
	public const string NS_XMLNS = "http://www.w3.org/2000/xmlns/";

	/* XRI resoultion tag & attrribute constants */
	public const string TAG_QUERY = "Query";
	public const string TAG_STATUS = "Status";
	public const string TAG_SERVERSTATUS = "ServerStatus";
	public const string ATTR_CODE = "code";
	public const string TAG_LOCALID = "LocalID";
	public const string TAG_CANONICALID = "CanonicalID";
	public const string TAG_EQUIVID = "EquivID";
	public const string TAG_CANONICALEQUIVID = "CanonicalEquivID";
	public const string TAG_REF = "Ref";
	public const string TAG_REDIRECT = "Redirect";
	public const string TAG_EXPIRES = "Expires";
	public const string TAG_SERVICE = "Service";
	//    public const string TAG_AUTHORITY = "Authority";
	//    public const string TAG_INTERNAL = "Internal";
	//    public const string TAG_EXTERNAL = "External";
	public const string TAG_URI = "Uri";
	public const string ATTR_PRIORITY = "priority";
	public const string ATTR_APPEND = "append";
	public const string TAG_MEDIATYPE = "MediaType";
	public const string TAG_PATH = "Path";
	public const string TAG_TYPE = "Type";
	public const string TAG_PROVIDERID = "ProviderID";
	public const string TAG_XRD = "XRD";
	//public const string ATTR_XMLID = "xml:id";
	public const string ATTR_ID_LOW = "id";
	public const string ATTR_ID_CAP = "ID";
	public const string ATTR_IDREF = "idRef";
	public const string ATTR_XRD_VERSION = "version";
	public const string TAG_XRDS = "XRDS";

	public const string ATTR_CID = "cid";
	public const string ATTR_CEID = "ceid";

	public const string ATTR_XMLNS = "xmlns";
	public const string NS_XRD_V2 = "xri://$xrd*($v*2.0)";
	public const string NS_XRDS = "xri://$xrds";
	public const string ATTR_REF = "ref";
	public const string ATTR_REDIRECT = "redirect";
	public const string XMLNS_XRDS = "xmlns:xrds";

	public const string NS_SAML = "urn:oasis:names:tc:SAML:2.0:assertion";
	public const string XMLNS_NS_SAML = "xmlns=\"" + NS_SAML + "\"";
	public const string TAG_ASSERTION = "Assertion";
	public const string TAG_ISSUER = "Issuer";
	public const string TAG_SUBJECT = "Subject";
	public const string TAG_CONDITIONS = "Conditions";
	public const string TAG_ATTRIBUTESTATEMENT = "AttributeStatement";
	public const string TAG_ATTRIBUTE = "Attribute";
	public const string TAG_ATTRIBUTEVALUE = "AttributeValue";
	public const string TAG_NAMEID = "NameID";
	public const string ATTR_NAME = "Name";
	public const string ATTR_NAMEQUALIFIER = "NameQualifier";
	public const string ATTR_NOTBEFORE = "NotBefore";
	public const string ATTR_NOTONORAFTER = "NotOnOrAfter";
	public const string ATTR_ISSUEINSTANT = "IssueInstant";
	public const string ATTR_VERSION = "Version";

	public const string NS_XMLDSIG = Constants.SignatureSpecNS;

	public const string TAG_KEYINFO = Constants._TAG_KEYINFO;
	public const string TAG_SIGNATURE = Constants._TAG_SIGNATURE;

	public const string CONTENT_TYPE_XRDS = "application/xrds+xml";
	public const string CONTENT_TYPE_XRD = "application/xrd+xml";
	public const string CONTENT_TYPE_URILIST = "text/uri-list";

	public const string HEADER_ACCEPT = "Accept";
	public const string HEADER_UA = "User-Agent";

} // Class: Tags
}