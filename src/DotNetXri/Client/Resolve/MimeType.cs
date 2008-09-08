/**
 * 
 */

using System.Text;

namespace DotNetXri.Client.Resolve {

using java.util.Comparator;
using java.util.Hashtable;
using java.util.Iterator;
using java.util.Set;

using org.openxri.resolve.exception.IllegalTrustTypeException;

/**
 * Encapsulates a media type in XRI Resolution.
 * Contains minimal business rules to recognize the 3 'built-in' types
 * used in XRI resolution: <code>application/xrds+xml</code>,
 * <code>application/xrd+xml</code> and <code>text/uri-list</code>.
 * 
 * @author wtan
 *
 */
public class MimeType : Comparable {

	public const string PARAM_SEP          = "sep";
	public const string PARAM_REFS         = "refs";
	public const string PARAM_HTTPS        = "https";
	public const string PARAM_SAML         = "saml";
	public const string PARAM_CID          = "cid";
	public const string PARAM_URIC         = "uric";
	public const string PARAM_NO_DEFAULT_T = "nodefault_t";
	public const string PARAM_NO_DEFAULT_P = "nodefault_p";
	public const string PARAM_NO_DEFAULT_M = "nodefault_m";

	/**
	 * @deprecated
	 */
	public const string PARAM_TRUST        = "trust";

	public const string XRDS_XML = "application/xrds+xml";
	public const string XRD_XML  = "application/xrd+xml";
	public const string URI_LIST = "text/uri-list";
	
	
	// mime type
	protected string type = null;
	
	// media params
	protected Hashtable params = null;
	
	// whatever was given to the parse method is saved here
	protected string original = null;

	/**
	 * Creates a MimeType obj
	 */
	protected MimeType(string type, Hashtable params, string original) {
		this.type = type;
		this.params = params;
		this.original = original;
	}

	/**
	 * Creates a MimeType obj with no params.
	 * @param type all-lowercase string with no leading or trailing whitespace.
	 */
	public MimeType(string type) {
		this(type, new Hashtable(), type);
	}
	
	/**
	 * Retrieves the MIME parameter value for the given key
	 * @param key
	 * @return string the value if present (could be an empty string), or <code>null</code> if not present
	 */
	public string getParam(string key) {
		Object val = params.get(key);
		if (val == null)
			return null;
		return (string)val;
	}

	/**
	 * Gets the set of parameter keys
	 * @return
	 */
	public Set paramKeys() {
		return params.keySet();
	}
	
	/**
	 * Compares the content of this obj with the candidate. Both must have the same type and same parameter values.
	 * @param m
	 * @return
	 */
	public bool equals(MimeType m) {
		if (!type.equals(m.type))
			return false;

		// if the main type is the special type (XRDS document), we use different rules to match
		if (type.equals(XRDS_XML)) {
			try {
				// look for all three parameters: https, saml, and trust
				// but the latter is only used if either https or saml is absent 
				string myTrust = (string)this.params.get(PARAM_TRUST);
				TrustType myTT = null;
				if (myTrust != null)
					myTT = new TrustType(myTrust);
				string myHTTPS = (string)this.params.get(PARAM_HTTPS);
				string mySAML  = (string)this.params.get(PARAM_SAML);
				
				if (myHTTPS == null && myTT != null) {
					myHTTPS = myTT.isHTTPS()? "true" : "false";
				}
				if (mySAML == null && myTT != null) {
					mySAML = myTT.isSAML()? "true" : "false";
				}
				
				
				string itsTrust = (string)m.params.get(PARAM_TRUST);
				TrustType itsTT = null;
				if (itsTrust != null)
					itsTT = new TrustType(itsTrust);
				string itsHTTPS = (string)m.params.get(PARAM_HTTPS);
				string itsSAML  = (string)m.params.get(PARAM_SAML);
				
				if (itsHTTPS == null && itsTT != null) {
					itsHTTPS = itsTT.isHTTPS()? "true" : "false";
				}
				if (itsSAML == null && itsTT != null) {
					itsSAML = itsTT.isSAML()? "true" : "false";
				}
				
				
				if (myHTTPS.equals(itsHTTPS) && mySAML.equals(itsSAML))
					return true;
				// else, fall through
			}
			catch (IllegalTrustTypeException e) {
				// ignore
			}
		}
		
		
		// must have the same number of parameters
		if (params.size() != m.params.size())
			return false;
		
		// check each param
		Set keys = params.keySet();
		Iterator i = keys.iterator();
		while (i.hasNext()) {
			string k = (string)i.next();
			string v1 = (string)params.get(k);
			string v2 = (string)m.params.get(k);
			
			if (v2 == null) {
				// key does not exist in m.params
				return false;
			}
			
			if (!v1.equals(v2))
				return false;
		}
		return true;
	}


	public bool equals(Object o)
	{
		return this.equals((MimeType)o);
	}

	
	/**
	 * This compares two objects and return their weigted order based on the q parameter
	 */
	public int compareTo(Object o)
	{
		MimeType m2 = (MimeType)o;
		
		float q1 = 1.0f;
		float q2 = 1.0f;
		
		try {
			q1 = Float.parseFloat(this.getParam("q"));
		}
		catch (Exception e) {}
		
		try {
			q2 = Float.parseFloat(m2.getParam("q"));
		}
		catch (Exception e) {}
		
		return (int) ((q2 - q1) * 1000);
	}
	
	/**
	 * Tests to see if this <code>MimeType</code> has the same type as the simple type
	 * <code>mtype</code>. If this <code>MimeType</code> has sub-parameters, they are ignored.
	 */
	public bool isType(string mtype) {
		return type.equals(mtype.toLowerCase());
	}
	
	/**
	 * Tests to see if this <code>MimeType</code> has the same type as <code>m</code> and
	 * that every parameter of <code>m</code> must be present and has the same value in this
	 * <code>MimeType</code>.
	 * @param m
	 * @return
	 */
	public bool isSuperSetOf(MimeType m) {
		if (!type.equals(m.type))
			return false;

		// must have equal or more parameters than candidate
		if (params.size() < m.params.size())
			return false;

		// check each param
		Set keys = m.params.keySet();
		Iterator i = keys.iterator();
		while (i.hasNext()) {
			string k = (string)i.next();
			string v1 = (string)params.get(k);
			string v2 = (string)m.params.get(k);
			
			if (v1 == null) {
				// key does not exist in this obj's params
				return false;
			}
			
			if (!v1.equals(v2))
				return false;
		}
		return true;
	}
	
	
	/**
	 * Parses a HTTP Accept or Content-Type header value into the type and params components
	 * @param typeStr
	 * @return Returns a new <code>MimeType</code> obj.
	 */
	public static MimeType parse(string typeStr) {
		if (typeStr == null)
			return null;

		string[] parts = typeStr.split("\\s*;\\s*");
		string typeVal = parts[0].trim().toLowerCase();
		Hashtable params = new Hashtable();
		for (int i = 1; i < parts.length; i++) {
			if (parts[i].length() == 0)
				continue; // ignore blank parts
			string[] kvpair = parts[i].split("\\s*=\\s*");
			string val = (kvpair.length > 1)? kvpair[1] : "";
			params.put(kvpair[0].toLowerCase(), val.trim());
		}
		
		if (isXriResMediaType(typeVal)) {
			setXriResDefaultParams(params);
		}

		return new MimeType(typeVal, params, typeStr);
	}

	public bool isValidXriResMediaType()
	{
		return isXriResMediaType(type);
	}

	/**
	 * @return Returns the type.
	 */
	public string getType() {
		return type;
	}

	/**
	 * @return Returns the original type string (as given to <code>parse</code>.)
	 */
	public override string ToString() {
		return original;
	}

	/**
	 * @return Returns the normalized string suitable for use in Accept and Content-Type headers.
	 */
	public string toNormalizedString() {
		StringBuilder sb = new StringBuilder(type);
		Iterator ki = params.keySet().iterator();
		while (ki.hasNext()) {
			string key = (string)ki.next();
			string val = (string)params.get(key);
			sb.append(';');
			sb.append(key);
			sb.append('=');
			sb.append(val);
		}
		return sb.ToString();
	}
	
	
	private static bool isXriResMediaType(string v)
	{
		return (v.equals(XRDS_XML)
				|| v.equals(XRD_XML)
				|| v.equals(URI_LIST));
		
	}
	
	private static void setXriResDefaultParams(Hashtable params)
	{
		string val = null;

		if (params.get(PARAM_TRUST) == null && params.get(PARAM_HTTPS) == null && params.get(PARAM_SAML) == null) {
			params.put(PARAM_HTTPS, "false");
			params.put(PARAM_SAML, "false");
		}
	}
	
	
	public static void main(string args[]) {
		MimeType m1 = MimeType.parse("application/xrdS+xml");
		MimeType m2 = MimeType.parse("application/xrds+xml;trust=none");
		MimeType m3 = MimeType.parse("application/xrds+xml;sep=true");
		MimeType m4 = MimeType.parse("application/xRds+xml;sep=true;refs=true;trust=none");
		MimeType m5 = MimeType.parse("application/Xrds+xml;trust=https");
		MimeType m6 = MimeType.parse("application/xrds+xml;trust=https;refs=true;refs=false");
		MimeType m7 = MimeType.parse(          "text/plain;trust=https;refs=true;refs=false");

		if (!m1.equals(m2)) {
			Logger.Info("m1.equals(m2) = " + m1.equals(m2));
			Logger.Info("m1 = " + m1.toNormalizedString());
			Logger.Info("m2 = " + m2.toNormalizedString());			
		}
		
		if (!m1.isSuperSetOf(m2)) {
			Logger.Info("m1.isSuperSetOf(m2) = " + m1.isSuperSetOf(m2));
			Logger.Info("m1 = " + m1.toNormalizedString());
			Logger.Info("m2 = " + m2.toNormalizedString());			
		}

		if (!m4.isSuperSetOf(m3)) {
			Logger.Info("m4.isSuperSetOf(m3) = " + m4.isSuperSetOf(m3));
			Logger.Info("m4 = " + m4.toNormalizedString());
			Logger.Info("m3 = " + m3.toNormalizedString());			
		}

		if (m2.equals(m5)) {
			Logger.Info("m2.isSuperSetOf(m5) = " + m2.isSuperSetOf(m5));
			Logger.Info("m2 = " + m2.toNormalizedString());
			Logger.Info("m5 = " + m5.toNormalizedString());			
		}

		if (m1.isSuperSetOf(m5)) {
			Logger.Info("m1.isSuperSetOf(m5) = " + m1.isSuperSetOf(m5));
			Logger.Info("m1 = " + m1.toNormalizedString());
			Logger.Info("m5 = " + m5.toNormalizedString());			
		}

		if (!m6.isSuperSetOf(m5)) {
			Logger.Info("m6.isSuperSetOf(m5) = " + m6.isSuperSetOf(m5));
			Logger.Info("m6 = " + m6.toNormalizedString());
			Logger.Info("m5 = " + m5.toNormalizedString());			
		}

		if (m6.isSuperSetOf(m7)) {
			Logger.Info("m6.isSuperSetOf(m7) = " + m6.isSuperSetOf(m7));
			Logger.Info("m6 = " + m6.toNormalizedString());
			Logger.Info("m7 = " + m7.toNormalizedString());			
		}
	}

}
}