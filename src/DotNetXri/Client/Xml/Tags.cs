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

using org.apache.xml.security.utils.Constants;


/*
********************************************************************************
* Class: Tags
********************************************************************************
*/ /**
* This class contains constants found in the XML during XRI resolution
* @author =chetan
*/
public class Tags
{
    
	/** 
	 * name space constants & meta for xri service resolution
	 * section 2.1.2 of 18th March2006 V2.0 wd10,XRI resolution document.
	*/
	public const String SERVICE_AUTH_RES = "xri://$res*auth*($v*2.0)";
	
	public const String NS_XML = "http://www.w3.org/XML/1998/namespace";
    public const String NS_XMLNS = "http://www.w3.org/2000/xmlns/";
    
    /* XRI resoultion tag & attrribute constants */
    public const String TAG_QUERY = "Query";
    public const String TAG_STATUS = "Status";
    public const String TAG_SERVERSTATUS = "ServerStatus";
    public const String ATTR_CODE = "code";
    public const String TAG_LOCALID ="LocalID";
    public const String TAG_CANONICALID = "CanonicalID";
    public const String TAG_EQUIVID = "EquivID";
    public const String TAG_CANONICALEQUIVID = "CanonicalEquivID";
    public const String TAG_REF = "Ref";
    public const String TAG_REDIRECT = "Redirect";
    public const String TAG_EXPIRES = "Expires";
    public const String TAG_SERVICE = "Service";
//    public const String TAG_AUTHORITY = "Authority";
//    public const String TAG_INTERNAL = "Internal";
//    public const String TAG_EXTERNAL = "External";
    public const String TAG_URI = "URI";
    public const String ATTR_PRIORITY = "priority";
    public const String ATTR_APPEND = "append";
    public const String TAG_MEDIATYPE = "MediaType";
    public const String TAG_PATH = "Path";
    public const String TAG_TYPE = "Type";
    public const String TAG_PROVIDERID = "ProviderID";
    public const String TAG_XRD = "XRD";
    //public const String ATTR_XMLID = "xml:id";
    public const String ATTR_ID_LOW = "id";
    public const String ATTR_ID_CAP = "ID";
    public const String ATTR_IDREF = "idRef";
    public const String ATTR_XRD_VERSION = "version";
    public const String TAG_XRDS = "XRDS";

    public const String ATTR_CID  = "cid";
    public const String ATTR_CEID  = "ceid";
    
    public const String ATTR_XMLNS = "xmlns";
    public const String NS_XRD_V2 = "xri://$xrd*($v*2.0)";
    public const String NS_XRDS ="xri://$xrds";
    public const String ATTR_REF="ref";
    public const String ATTR_REDIRECT="redirect";
    public const String XMLNS_XRDS = "xmlns:xrds";

    public const String NS_SAML = "urn:oasis:names:tc:SAML:2.0:assertion";
    public const String XMLNS_NS_SAML = "xmlns=\"" + NS_SAML + "\"";
    public const String TAG_ASSERTION = "Assertion";
    public const String TAG_ISSUER = "Issuer";
    public const String TAG_SUBJECT = "Subject";
    public const String TAG_CONDITIONS = "Conditions";
    public const String TAG_ATTRIBUTESTATEMENT = "AttributeStatement";
    public const String TAG_ATTRIBUTE = "Attribute";
    public const String TAG_ATTRIBUTEVALUE = "AttributeValue";
    public const String TAG_NAMEID = "NameID";
    public const String ATTR_NAME = "Name";
    public const String ATTR_NAMEQUALIFIER = "NameQualifier";
    public const String ATTR_NOTBEFORE = "NotBefore";
    public const String ATTR_NOTONORAFTER = "NotOnOrAfter";
    public const String ATTR_ISSUEINSTANT = "IssueInstant";
    public const String ATTR_VERSION = "Version";

    public const String NS_XMLDSIG = Constants.SignatureSpecNS;

    public const String TAG_KEYINFO = Constants._TAG_KEYINFO;
    public const String TAG_SIGNATURE = Constants._TAG_SIGNATURE;

    public const String CONTENT_TYPE_XRDS = "application/xrds+xml";
    public const String CONTENT_TYPE_XRD ="application/xrd+xml";
    public const String CONTENT_TYPE_URILIST = "text/uri-list";
    
    public const String HEADER_ACCEPT = "Accept";
    public const String HEADER_UA = "User-Agent";

} // Class: Tags
