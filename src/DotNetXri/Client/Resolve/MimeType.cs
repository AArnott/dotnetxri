/**
 * 
 */
namespace DotNetXri.Client.Resolve {

import java.util.Comparator;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Set;

import org.openxri.resolve.exception.IllegalTrustTypeException;

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

	public const String PARAM_SEP          = "sep";
	public const String PARAM_REFS         = "refs";
	public const String PARAM_HTTPS        = "https";
	public const String PARAM_SAML         = "saml";
	public const String PARAM_CID          = "cid";
	public const String PARAM_URIC         = "uric";
	public const String PARAM_NO_DEFAULT_T = "nodefault_t";
	public const String PARAM_NO_DEFAULT_P = "nodefault_p";
	public const String PARAM_NO_DEFAULT_M = "nodefault_m";

	/**
	 * @deprecated
	 */
	public const String PARAM_TRUST        = "trust";

	public const String XRDS_XML = "application/xrds+xml";
	public const String XRD_XML  = "application/xrd+xml";
	public const String URI_LIST = "text/uri-list";
	
	
	// mime type
	protected String type = null;
	
	// media params
	protected HashMap params = null;
	
	// whatever was given to the parse method is saved here
	protected String original = null;

	/**
	 * Creates a MimeType obj
	 */
	protected MimeType(String type, HashMap params, String original) {
		this.type = type;
		this.params = params;
		this.original = original;
	}

	/**
	 * Creates a MimeType obj with no params.
	 * @param type all-lowercase string with no leading or trailing whitespace.
	 */
	public MimeType(String type) {
		this(type, new HashMap(), type);
	}
	
	/**
	 * Retrieves the MIME parameter value for the given key
	 * @param key
	 * @return String the value if present (could be an empty String), or <code>null</code> if not present
	 */
	public String getParam(String key) {
		Object val = params.get(key);
		if (val == null)
			return null;
		return (String)val;
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
				String myTrust = (String)this.params.get(PARAM_TRUST);
				TrustType myTT = null;
				if (myTrust != null)
					myTT = new TrustType(myTrust);
				String myHTTPS = (String)this.params.get(PARAM_HTTPS);
				String mySAML  = (String)this.params.get(PARAM_SAML);
				
				if (myHTTPS == null && myTT != null) {
					myHTTPS = myTT.isHTTPS()? "true" : "false";
				}
				if (mySAML == null && myTT != null) {
					mySAML = myTT.isSAML()? "true" : "false";
				}
				
				
				String itsTrust = (String)m.params.get(PARAM_TRUST);
				TrustType itsTT = null;
				if (itsTrust != null)
					itsTT = new TrustType(itsTrust);
				String itsHTTPS = (String)m.params.get(PARAM_HTTPS);
				String itsSAML  = (String)m.params.get(PARAM_SAML);
				
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
			String k = (String)i.next();
			String v1 = (String)params.get(k);
			String v2 = (String)m.params.get(k);
			
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
	public bool isType(String mtype) {
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
			String k = (String)i.next();
			String v1 = (String)params.get(k);
			String v2 = (String)m.params.get(k);
			
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
	public static MimeType parse(String typeStr) {
		if (typeStr == null)
			return null;

		String[] parts = typeStr.split("\\s*;\\s*");
		String typeVal = parts[0].trim().toLowerCase();
		HashMap params = new HashMap();
		for (int i = 1; i < parts.length; i++) {
			if (parts[i].length() == 0)
				continue; // ignore blank parts
			String[] kvpair = parts[i].split("\\s*=\\s*");
			String val = (kvpair.length > 1)? kvpair[1] : "";
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
	public String getType() {
		return type;
	}

	/**
	 * @return Returns the original type string (as given to <code>parse</code>.)
	 */
	public String toString() {
		return original;
	}

	/**
	 * @return Returns the normalized string suitable for use in Accept and Content-Type headers.
	 */
	public String toNormalizedString() {
		StringBuffer sb = new StringBuffer(type);
		Iterator ki = params.keySet().iterator();
		while (ki.hasNext()) {
			String key = (String)ki.next();
			String val = (String)params.get(key);
			sb.append(';');
			sb.append(key);
			sb.append('=');
			sb.append(val);
		}
		return sb.toString();
	}
	
	
	private static bool isXriResMediaType(String v)
	{
		return (v.equals(XRDS_XML)
				|| v.equals(XRD_XML)
				|| v.equals(URI_LIST));
		
	}
	
	private static void setXriResDefaultParams(HashMap params)
	{
		String val = null;

		if (params.get(PARAM_TRUST) == null && params.get(PARAM_HTTPS) == null && params.get(PARAM_SAML) == null) {
			params.put(PARAM_HTTPS, "false");
			params.put(PARAM_SAML, "false");
		}
	}
	
	
	public static void main(String args[]) {
		MimeType m1 = MimeType.parse("application/xrdS+xml");
		MimeType m2 = MimeType.parse("application/xrds+xml;trust=none");
		MimeType m3 = MimeType.parse("application/xrds+xml;sep=true");
		MimeType m4 = MimeType.parse("application/xRds+xml;sep=true;refs=true;trust=none");
		MimeType m5 = MimeType.parse("application/Xrds+xml;trust=https");
		MimeType m6 = MimeType.parse("application/xrds+xml;trust=https;refs=true;refs=false");
		MimeType m7 = MimeType.parse(          "text/plain;trust=https;refs=true;refs=false");

		if (!m1.equals(m2)) {
			System.Console.WriteLine("m1.equals(m2) = " + m1.equals(m2));
			System.Console.WriteLine("m1 = " + m1.toNormalizedString());
			System.Console.WriteLine("m2 = " + m2.toNormalizedString());			
		}
		
		if (!m1.isSuperSetOf(m2)) {
			System.Console.WriteLine("m1.isSuperSetOf(m2) = " + m1.isSuperSetOf(m2));
			System.Console.WriteLine("m1 = " + m1.toNormalizedString());
			System.Console.WriteLine("m2 = " + m2.toNormalizedString());			
		}

		if (!m4.isSuperSetOf(m3)) {
			System.Console.WriteLine("m4.isSuperSetOf(m3) = " + m4.isSuperSetOf(m3));
			System.Console.WriteLine("m4 = " + m4.toNormalizedString());
			System.Console.WriteLine("m3 = " + m3.toNormalizedString());			
		}

		if (m2.equals(m5)) {
			System.Console.WriteLine("m2.isSuperSetOf(m5) = " + m2.isSuperSetOf(m5));
			System.Console.WriteLine("m2 = " + m2.toNormalizedString());
			System.Console.WriteLine("m5 = " + m5.toNormalizedString());			
		}

		if (m1.isSuperSetOf(m5)) {
			System.Console.WriteLine("m1.isSuperSetOf(m5) = " + m1.isSuperSetOf(m5));
			System.Console.WriteLine("m1 = " + m1.toNormalizedString());
			System.Console.WriteLine("m5 = " + m5.toNormalizedString());			
		}

		if (!m6.isSuperSetOf(m5)) {
			System.Console.WriteLine("m6.isSuperSetOf(m5) = " + m6.isSuperSetOf(m5));
			System.Console.WriteLine("m6 = " + m6.toNormalizedString());
			System.Console.WriteLine("m5 = " + m5.toNormalizedString());			
		}

		if (m6.isSuperSetOf(m7)) {
			System.Console.WriteLine("m6.isSuperSetOf(m7) = " + m6.isSuperSetOf(m7));
			System.Console.WriteLine("m6 = " + m6.toNormalizedString());
			System.Console.WriteLine("m7 = " + m7.toNormalizedString());			
		}
	}

}
}