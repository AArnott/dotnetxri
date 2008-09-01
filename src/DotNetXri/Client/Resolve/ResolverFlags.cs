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
namespace DotNetXri.Client.Resolve {

using java.net.URI;
using java.util.ArrayList;

using org.openxri.GCSAuthority;
using org.openxri.XRI;
using org.openxri.XRIAuthority;
using org.openxri.xml.XRD;


/**
 * This class encapsulates the various flags used as input to the Resolver methods.
 *
 * @author =wil
 */
public class ResolverFlags
{
	private bool https; // HTTPS trusted resolution?
	private bool saml;  // SAML trusted resolution? 
	private bool refs;  // follow refs?
	private bool noDefaultT; // default SEP type matching?
	private bool noDefaultP; // default SEP path matching?
	private bool noDefaultM; // default SEP mediatype matching?
	private bool uric;  // construct URI on XRD output?
	private bool cid;   // do CID verification?

    /**
     * Constructor
     */
    public ResolverFlags()
    {
    	https = false;
    	saml  = false;
    	refs  = true;
    	noDefaultT = false;
    	noDefaultP = false;
    	noDefaultM = false;
    	uric  = false;
    	cid   = true;
    }
    
    /**
     * Copy Constructor
     */
    public ResolverFlags(ResolverFlags other)
    {
    	https = other.https;
    	saml  = other.saml;
    	refs  = other.refs;
    	noDefaultT = other.noDefaultT;
    	noDefaultP = other.noDefaultP;
    	noDefaultM = other.noDefaultM;
    	uric  = other.uric;
    	cid   = other.cid;
    }

    /**
     * Constructor that provides compatibility with old trustType and followRefs interface
     */
    public ResolverFlags(TrustType trustType, bool followRefs)
    {
    	this();
		setHttps(trustType.isHTTPS());
		setSaml(trustType.isSAML());
		setRefs(followRefs);
    }

	public String toString()
	{
		StringBuffer sb = new StringBuffer();
		sb.append("ResolverFlags (");
		sb.append(toURIQuery());
		sb.append(")");

		return sb.toString();
	}
	
	public String toURIQuery()
	{
		StringBuffer sb = new StringBuffer();
		sb.append("https=");
		sb.append(https);
		sb.append("&saml=");
		sb.append(saml);
		sb.append("&refs=");
		sb.append(refs);
		sb.append("&no_default_t=");
		sb.append(noDefaultT);
		sb.append("&no_default_p=");
		sb.append(noDefaultP);
		sb.append("&no_default_m=");
		sb.append(noDefaultM);
		sb.append("&uric=");
		sb.append(uric);
		sb.append("&cid=");
		sb.append(cid);

		return sb.toString();		
	}

	/**
	 * @return the https
	 */
	public bool isHttps() {
		return https;
	}

	/**
	 * @param https the https to set
	 */
	public void setHttps(bool https) {
		this.https = https;
	}

	/**
	 * @return the saml
	 */
	public bool isSaml() {
		return saml;
	}

	/**
	 * @param saml the saml to set
	 */
	public void setSaml(bool saml) {
		this.saml = saml;
	}

	/**
	 * @return the refs
	 */
	public bool isRefs() {
		return refs;
	}

	/**
	 * @param refs the refs to set
	 */
	public void setRefs(bool refs) {
		this.refs = refs;
	}

	/**
	 * @return the noDefaultT
	 */
	public bool isNoDefaultT() {
		return noDefaultT;
	}

	/**
	 * @param noDefaultT the noDefaultT to set
	 */
	public void setNoDefaultT(bool noDefaultT) {
		this.noDefaultT = noDefaultT;
	}

	/**
	 * @return the noDefaultP
	 */
	public bool isNoDefaultP() {
		return noDefaultP;
	}

	/**
	 * @param noDefaultP the noDefaultP to set
	 */
	public void setNoDefaultP(bool noDefaultP) {
		this.noDefaultP = noDefaultP;
	}

	/**
	 * @return the noDefaultM
	 */
	public bool isNoDefaultM() {
		return noDefaultM;
	}

	/**
	 * @param noDefaultM the noDefaultM to set
	 */
	public void setNoDefaultM(bool noDefaultM) {
		this.noDefaultM = noDefaultM;
	}

	/**
	 * @return the uric
	 */
	public bool isUric() {
		return uric;
	}

	/**
	 * @param uric the uric to set
	 */
	public void setUric(bool uric) {
		this.uric = uric;
	}

	/**
	 * @return the cid
	 */
	public bool isCid() {
		return cid;
	}

	/**
	 * @param cid the cid to set
	 */
	public void setCid(bool cid) {
		this.cid = cid;
	}
	
	
	public String getTrustParameters() {
		StringBuffer sb = new StringBuffer();
		sb.append(MimeType.PARAM_HTTPS);
		sb.append("=");
		sb.append(isHttps()? "true" : "false");
		sb.append(";");
		sb.append(MimeType.PARAM_SAML);
		sb.append("=");
		sb.append(isSaml()? "true" : "false");
		return sb.toString();
	}
}
}