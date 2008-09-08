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


/**
	* 
	* @author =wil
	* @see org.openxri.xml.ServerStatus
	*/
public class Status : SimpleXMLElement {

	public const string SUCCESS = "100";
	public const string PERM_FAIL = "200";
	public const string NOT_IMPLEMENTED = "201";
	public const string LIMIT_EXCEEDED = "202";
	public const string INVALID_INPUT = "210";
	public const string INVALID_QXRI = "211";
	public const string INVALID_OUTPUT_FORMAT = "212";
	public const string INVALID_SEP_TYPE = "213";
	public const string INVALID_SEP_MEDIA_TYPE = "214";
	public const string UNKNOWN_ROOT = "215";
	public const string AUTH_RES_ERROR = "220";
	public const string AUTH_RES_NOT_FOUND = "221";
	public const string QUERY_NOT_FOUND = "222";
	public const string UNEXPECTED_XRD = "223";
	public const string INACTIVE = "224";
	public const string TRUSTED_RES_ERROR = "230";
	public const string HTTPS_RES_NOT_FOUND = "231";
	public const string SAML_RES_NOT_FOUND = "232";
	public const string HTTPS_SAML_RES_NOT_FOUND = "233";
	public const string UNVERIFIED_SIGNATURE = "234";
	public const string SEP_SELECTION_ERROR = "240";
	public const string SEP_NOT_FOUND = "241";
	public const string REDIRECT_ERROR = "250";
	public const string INVALID_REDIRECT = "251";
	public const string INVALID_HTTPS_REDIRECT = "252";
	public const string REDIRECT_VERIFY_FAILED = "253";
	public const string REF_ERROR = "260";
	public const string INVALID_REF = "261";
	public const string REF_NOT_FOLLOWED = "262"; // used to be 101
	public const string TEMPORARY_FAIL = "300";
	public const string TIMEOUT_ERROR = "301";
	public const string NETWORK_ERROR = "320";
	public const string UNEXPECTED_RESPONSE = "321";
	public const string INVALID_XRDS = "322";

	// aliases
	public const string INVALID_RESPONSE = INVALID_XRDS;
	public const string INVALID_RES_MEDIA_TYPE = INVALID_OUTPUT_FORMAT;


	public const string CID_ABSENT = "absent";
	public const string CID_OFF = "off";
	public const string CID_VERIFIED = "verified";
	public const string CID_FAILED = "failed";


	public Status(Status s)
		: base(s) {
	}

	public Status() {
		this(Tags.TAG_STATUS);
	}

	public Status(string code)
		: base(Tags.TAG_STATUS) {
		setCode(code);
		setCID(CID_OFF);
		setCEID(CID_OFF);
	}

	public Status(string code, string text)
		: base(Tags.TAG_STATUS, text) {
		setCode(code);
	}

	public string getCode() {
		return getAttributeValue(Tags.ATTR_CODE);
	}

	public string getText() {
		return getValue();
	}

	public void setCode(string code) {
		addAttribute(Tags.ATTR_CODE, code);
	}

	public void setText(string text) {
		setValue(text);
	}

	public string getCID() {
		return getAttributeValue(Tags.ATTR_CID);
	}

	public void setCID(string status) {
		addAttribute(Tags.ATTR_CID, status);
	}

	public string getCEID() {
		return getAttributeValue(Tags.ATTR_CEID);
	}

	public void setCEID(string status) {
		addAttribute(Tags.ATTR_CEID, status);
	}

}
}