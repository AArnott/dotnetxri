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

import org.apache.xml.security.utils.Constants;


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
	public static final String SERVICE_AUTH_RES = "xri://$res*auth*($v*2.0)";
	
	public static final String NS_XML = "http://www.w3.org/XML/1998/namespace";
    public static final String NS_XMLNS = "http://www.w3.org/2000/xmlns/";
    
    /* XRI resoultion tag & attrribute constants */
    public static final String TAG_QUERY = "Query";
    public static final String TAG_STATUS = "Status";
    public static final String TAG_SERVERSTATUS = "ServerStatus";
    public static final String ATTR_CODE = "code";
    public static final String TAG_LOCALID ="LocalID";
    public static final String TAG_CANONICALID = "CanonicalID";
    public static final String TAG_EQUIVID = "EquivID";
    public static final String TAG_CANONICALEQUIVID = "CanonicalEquivID";
    public static final String TAG_REF = "Ref";
    public static final String TAG_REDIRECT = "Redirect";
    public static final String TAG_EXPIRES = "Expires";
    public static final String TAG_SERVICE = "Service";
//    public static final String TAG_AUTHORITY = "Authority";
//    public static final String TAG_INTERNAL = "Internal";
//    public static final String TAG_EXTERNAL = "External";
    public static final String TAG_URI = "URI";
    public static final String ATTR_PRIORITY = "priority";
    public static final String ATTR_APPEND = "append";
    public static final String TAG_MEDIATYPE = "MediaType";
    public static final String TAG_PATH = "Path";
    public static final String TAG_TYPE = "Type";
    public static final String TAG_PROVIDERID = "ProviderID";
    public static final String TAG_XRD = "XRD";
    //public static final String ATTR_XMLID = "xml:id";
    public static final String ATTR_ID_LOW = "id";
    public static final String ATTR_ID_CAP = "ID";
    public static final String ATTR_IDREF = "idRef";
    public static final String ATTR_XRD_VERSION = "version";
    public static final String TAG_XRDS = "XRDS";

    public static final String ATTR_CID  = "cid";
    public static final String ATTR_CEID  = "ceid";
    
    public static final String ATTR_XMLNS = "xmlns";
    public static final String NS_XRD_V2 = "xri://$xrd*($v*2.0)";
    public static final String NS_XRDS ="xri://$xrds";
    public static final String ATTR_REF="ref";
    public static final String ATTR_REDIRECT="redirect";
    public static final String XMLNS_XRDS = "xmlns:xrds";

    public static final String NS_SAML = "urn:oasis:names:tc:SAML:2.0:assertion";
    public static final String XMLNS_NS_SAML = "xmlns=\"" + NS_SAML + "\"";
    public static final String TAG_ASSERTION = "Assertion";
    public static final String TAG_ISSUER = "Issuer";
    public static final String TAG_SUBJECT = "Subject";
    public static final String TAG_CONDITIONS = "Conditions";
    public static final String TAG_ATTRIBUTESTATEMENT = "AttributeStatement";
    public static final String TAG_ATTRIBUTE = "Attribute";
    public static final String TAG_ATTRIBUTEVALUE = "AttributeValue";
    public static final String TAG_NAMEID = "NameID";
    public static final String ATTR_NAME = "Name";
    public static final String ATTR_NAMEQUALIFIER = "NameQualifier";
    public static final String ATTR_NOTBEFORE = "NotBefore";
    public static final String ATTR_NOTONORAFTER = "NotOnOrAfter";
    public static final String ATTR_ISSUEINSTANT = "IssueInstant";
    public static final String ATTR_VERSION = "Version";

    public static final String NS_XMLDSIG = Constants.SignatureSpecNS;

    public static final String TAG_KEYINFO = Constants._TAG_KEYINFO;
    public static final String TAG_SIGNATURE = Constants._TAG_SIGNATURE;

    public static final String CONTENT_TYPE_XRDS = "application/xrds+xml";
    public static final String CONTENT_TYPE_XRD ="application/xrd+xml";
    public static final String CONTENT_TYPE_URILIST = "text/uri-list";
    
    public static final String HEADER_ACCEPT = "Accept";
    public static final String HEADER_UA = "User-Agent";

} // Class: Tags
