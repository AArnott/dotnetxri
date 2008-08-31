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


/**
 * 
 * @author =wil
 * @see org.openxri.xml.ServerStatus
 */
public class Status :SimpleXMLElement{

	public const String SUCCESS					= "100";
	public const String PERM_FAIL				= "200";
	public const String NOT_IMPLEMENTED			= "201"; 
	public const String LIMIT_EXCEEDED			= "202"; 
	public const String INVALID_INPUT			= "210"; 
	public const String INVALID_QXRI				= "211"; 
	public const String INVALID_OUTPUT_FORMAT	= "212";
	public const String INVALID_SEP_TYPE			= "213";
	public const String INVALID_SEP_MEDIA_TYPE	= "214"; 
	public const String UNKNOWN_ROOT				= "215"; 
	public const String AUTH_RES_ERROR			= "220"; 
	public const String AUTH_RES_NOT_FOUND		= "221"; 
	public const String QUERY_NOT_FOUND			= "222";
	public const String UNEXPECTED_XRD			= "223";
	public const String INACTIVE					= "224";
	public const String TRUSTED_RES_ERROR		= "230"; 
	public const String HTTPS_RES_NOT_FOUND		= "231"; 
	public const String SAML_RES_NOT_FOUND		= "232"; 
	public const String HTTPS_SAML_RES_NOT_FOUND	= "233"; 
	public const String UNVERIFIED_SIGNATURE		= "234";
	public const String SEP_SELECTION_ERROR		= "240"; 
	public const String SEP_NOT_FOUND			= "241";
	public const String REDIRECT_ERROR			= "250";
	public const String INVALID_REDIRECT			= "251";
	public const String INVALID_HTTPS_REDIRECT	= "252";
	public const String REDIRECT_VERIFY_FAILED	= "253";
	public const String REF_ERROR				= "260";
	public const String INVALID_REF				= "261";
	public const String REF_NOT_FOLLOWED			= "262"; // used to be 101
	public const String TEMPORARY_FAIL			= "300"; 
	public const String TIMEOUT_ERROR			= "301";
	public const String NETWORK_ERROR			= "320";
	public const String UNEXPECTED_RESPONSE		= "321"; 
	public const String INVALID_XRDS				= "322";

	// aliases
	public const String INVALID_RESPONSE = INVALID_XRDS;
	public const String INVALID_RES_MEDIA_TYPE = INVALID_OUTPUT_FORMAT;

	
	public const String CID_ABSENT   = "absent";
	public const String CID_OFF      = "off";
	public const String CID_VERIFIED = "verified";
	public const String CID_FAILED   = "failed";
	
	
	public Status(Status s) : base(s) {
	}
	
	public Status() {
		this(Tags.TAG_STATUS);
	}
	
	public Status(String code) : base(Tags.TAG_STATUS) {
		setCode(code);
		setCID(CID_OFF);
		setCEID(CID_OFF);
	}	
	
	public Status(String code, String text) : base(Tags.TAG_STATUS, text) {
		setCode(code);
	}
	
	public String getCode() {
		return getAttributeValue(Tags.ATTR_CODE);
	}
	
	public String getText() {
		return getValue();
	}
	
	public void setCode(String code) {
		addAttribute(Tags.ATTR_CODE, code);
	}
	
	public void setText(String text) {
		setValue(text);
	}
	
	public String getCID() {
		return getAttributeValue(Tags.ATTR_CID);
	}

	public void setCID(String status) {
		addAttribute(Tags.ATTR_CID, status);
	}

	public String getCEID() {
		return getAttributeValue(Tags.ATTR_CEID);
	}

	public void setCEID(String status) {
		addAttribute(Tags.ATTR_CEID, status);
	}

}
