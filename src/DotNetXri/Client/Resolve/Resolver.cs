/**
 * 
 */

using System.Text;

namespace DotNetXri.Client.Resolve {

using java.io.ByteArrayInputStream;
using java.io.IOException;
using java.io.InputStream;
using java.io.UnsupportedEncodingException;
using java.net.HttpURLConnection;
using java.net.Uri;
using java.net.URISyntaxException;
using java.net.URLEncoder;
using java.text.ParseException;
using java.util.ArrayList;
using java.util.Hashtable;
using java.util.Iterator;
using java.util.LinkedHashMap;
using java.util.List;
using java.util.Properties;
using java.util.ArrayList;

using org.apache.xerces.parsers.DOMParser;
using org.apache.xml.security.exceptions.XMLSecurityException;
using org.apache.xml.security.keys.KeyInfo;
using org.openxri.AuthorityPath;
using org.openxri.IRIAuthority;
using org.openxri.XRI;
using org.openxri.XRIAbsolutePath;
using org.openxri.XRIAuthority;
using org.openxri.XRIParseException;
using org.openxri.XRIPath;
using org.openxri.XRIQuery;
using org.openxri.XRISegment;
using org.openxri.XRISubSegment;
using org.openxri.http.BaseFetcher;
using org.openxri.resolve.exception.InvalidAuthorityURIException;
using org.openxri.resolve.exception.PartialResolutionException;
using org.openxri.resolve.exception.XRIResolutionException;
using org.openxri.saml.Assertion;
using org.openxri.saml.Attribute;
using org.openxri.saml.AttributeStatement;
using org.openxri.saml.NameID;
using org.openxri.saml.Subject;
using org.openxri.util.DOMUtils;
using org.openxri.util.IO;
using org.openxri.util.PrioritizedList;
using org.openxri.xml.CanonicalEquivID;
using org.openxri.xml.CanonicalID;
using org.openxri.xml.EquivID;
using org.openxri.xml.LocalID;
using org.openxri.xml.Ref;
using org.openxri.xml.SEPUri;
using org.openxri.xml.ServerStatus;
using org.openxri.xml.Service;
using org.openxri.xml.SimpleXMLElement;
using org.openxri.xml.Status;
using org.openxri.xml.Tags;
using org.openxri.xml.XRD;
using org.openxri.xml.XRDS;
using org.openxri.xml.Redirect;
using org.w3c.dom.Document;
using org.w3c.dom.Element;
using org.xml.sax.InputSource;
using org.xml.sax.SAXException;
using DotNetXri.Loggers;
using System.Collections;
using DotNetXri.Client.Xml;

/**
 * @author wtan
 *
 */
public class Resolver :BaseFetcher {

	private static ILog log = Logger.Create(typeof(Resolver));

	protected Hashtable root = null;

	protected int maxFollowRefs = -1;

	protected int maxRequests = -1;

	protected Uri proxyURI = null;

	private int maxTotalBytes = -1;

	private int maxBytesPerRequest = -1;
	
	private Hashtable needNoHttps = null;
	

	/**
	 * Creates a Resolver obj.
	 */
	public Resolver() {
		root = new Hashtable();
		needNoHttps = new Hashtable();
	}

	/**
	 * This is the recommended API to use for performing authority resolution an an XRI
	 * @param qxri
	 * @param flags - ResolverFlag
	 * @param state - ResolverState
	 * @return
	 * @throws PartialResolutionException
	 */
	public XRDS resolveAuthToXRDS(XRI qxri, ResolverFlags flags,
			ResolverState state) /*throws PartialResolutionException */{
		log.Info("resolveAuthToXRDS(s'" + qxri + "', flags: " + flags + ")");
		if (proxyURI != null)
			return resolveViaProxy(qxri, null, null, false, flags, state);

		XRDS xrds = resolveAuthority(qxri, flags, state);
		if (flags.isUric()) {
			constructURIinXRD(xrds.getFinalXRD(), qxri);
		}
		return xrds;
	}


	/**
	 * 
	 * @param qxri
	 * @param flags ResolverFlags
	 * @param state
	 * @return
	 * @throws PartialResolutionException
	 */
	public XRD resolveAuthToXRD(XRI qxri, ResolverFlags flags,
			ResolverState state) /*throws PartialResolutionException */{
		log.Info("resolveAuthToXRD(s'" + qxri + "', flags: " + flags + ")");

		if (proxyURI != null) {
			XRDS xrds = resolveViaProxy(qxri, null, null, false, flags, state);
			return xrds.getFinalXRD();
		}

		XRDS xrds = resolveAuthority(qxri, flags, state);
		XRD finalXRD = xrds.getFinalXRD();
		if (flags.isUric()) {
			constructURIinXRD(finalXRD, qxri);
		}
		return finalXRD;
	}

	
	public XRDS resolveSEPToXRDS(XRI qxri, string sepType, string sepMediaType,
			ResolverFlags flags, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveSEPToXRDS('" + qxri + "', sepType=" + sepType
				+ ", sepMediaType=" + sepMediaType + ", flags:" + flags + ")");

		if (proxyURI != null)
			return resolveViaProxy(qxri, sepType, sepMediaType, true, flags, state);

		XRDS xrds = resolveAuthority(qxri, flags, state);
		XRD finalXRD = xrds.getFinalXRD();

		selectServiceFromXRD(xrds, finalXRD, qxri, sepType, sepMediaType, flags, state);
		if (flags.isUric()) {
			constructURIinXRD(xrds.getFinalXRD(), qxri);			
		}
		return xrds;
	}

	
	public XRD resolveSEPToXRD(XRI qxri, string sepType, string sepMediaType,
			ResolverFlags flags, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveSEPToXRD('" + qxri + "', sepType=" + sepType
				+ ", sepMediaType=" + sepMediaType + ", flags: " + flags + ")");

		XRDS xrds = resolveSEPToXRDS(qxri, sepType, sepMediaType, flags, state);
		return xrds.getFinalXRD();
	}

	
	public ArrayList resolveSEPToURIList(XRI qxri, string sepType,
			string sepMediaType, ResolverFlags flags, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveSEPToURIList('" + qxri + "', sepType=" + sepType
				+ ", sepMediaType=" + sepMediaType + ", flags: " + flags + ")");

		// no need to do uric
		flags.setUric(false);
		XRD xrd = resolveSEPToXRD(qxri, sepType, sepMediaType, flags, state);
		if (xrd == null)
			return new ArrayList();

		if (xrd.getSelectedServices().getList().size() < 1) {
			log.error("SEP Selection succeeded but no Service found!?");
			return new ArrayList();
		}

		Service topService = (Service) xrd.getSelectedServices().getList().get(0);

		ArrayList urisOut = new ArrayList();
		ArrayList uris = topService.getPrioritizedURIs();
		for (int i = 0; uris != null && i < uris.size(); i++) {
			SEPUri uri = (SEPUri) uris.get(i);
			string append = uri.getAppend();
			urisOut.add(constructURI(uri.getURI(), append, qxri));
		}

		return urisOut;
	}
	

	public string resolveSEPToTextURIList(XRI qxri, string sepType,
			string sepMediaType, ResolverFlags flags, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveSEPToTextURIList('" + qxri + ", sepType=" + sepType
				+ ", sepMediaType=" + sepMediaType + ", flags: " + flags + ")");
		ArrayList uris = resolveSEPToURIList(qxri, sepType, sepMediaType,
				flags, state);
		StringBuilder buf = new StringBuilder();
		for (int i = 0; uris != null && i < uris.size(); i++) {
			buf.append(uris.get(i).ToString());
			buf.append("\n");
		}
		return buf.ToString();
	}
	
	/////////////////////////////////////////////////////////////////////////
	// BEGIN DEPRECATED APIS                                               //
	/////////////////////////////////////////////////////////////////////////
	
	/**
	 * @deprecated
	 */
	public XRDS resolveAuthToXRDS(string qxri, TrustType trustType, bool followRefs) 
			/*throws PartialResolutionException*/
	{
		return resolveAuthToXRDS(qxri, trustType, followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveAuthToXRDS(string qxri, TrustType trustType, bool followRefs, ResolverState state)
			/*throws PartialResolutionException*/
	{
		XRI xri = parseAbsoluteQXRIOrError(qxri);
		return resolveAuthToXRDS(xri, trustType, followRefs, state);
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveAuthToXRDS(XRI qxri, TrustType trustType, bool followRefs)
			/*throws PartialResolutionException*/
	{
		return resolveAuthToXRDS(qxri, trustType, followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveAuthToXRDS(XRI qxri, TrustType trustType,
			bool followRefs, ResolverState state)
			/*throws PartialResolutionException */{
		ResolverFlags f = new ResolverFlags(trustType, followRefs);
		return resolveAuthToXRDS(qxri, f, state);
	}

	/**
	 * @deprecated
	 */
	public XRD resolveAuthToXRD(string qxri, TrustType trustType,
			bool followRefs) /*throws PartialResolutionException */{
		return resolveAuthToXRD(qxri, trustType, followRefs,
				new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRD resolveAuthToXRD(string qxri, TrustType trustType,
			bool followRefs, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveAuthToXRD(s'" + qxri + "', trustType=" + trustType
				+ ", followRefs=" + followRefs + ")");
		XRI xri = parseAbsoluteQXRIOrError(qxri);
		return resolveAuthToXRD(xri, trustType, followRefs, state);
	}

	/**
	 * @deprecated
	 */
	public XRD resolveAuthToXRD(XRI qxri, TrustType trustType,
			bool followRefs) /*throws PartialResolutionException */{
		return resolveAuthToXRD(qxri, trustType, followRefs,
				new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRD resolveAuthToXRD(XRI qxri, TrustType trustType,
			bool followRefs, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveAuthToXRD('" + qxri + "', trustType=" + trustType
				+ ", followRefs=" + followRefs + ")");

		ResolverFlags f = new ResolverFlags(trustType, followRefs);
		return resolveAuthToXRD(qxri, f, state);
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveSEPToXRDS(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToXRDS(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveSEPToXRDS(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs,
			ResolverState state) /*throws PartialResolutionException */{
		log.Info("resolveSEPToXRDS(s'" + qxri + "', trustType=" + trustType
				+ ", sepType=" + sepType + ", sepMediaType=" + sepMediaType
				+ ", followRefs=" + followRefs + ")");

		XRI xri = parseAbsoluteQXRIOrError(qxri);
		return resolveSEPToXRDS(xri, trustType, sepType, sepMediaType,
				followRefs, state);
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveSEPToXRDS(XRI qxri, TrustType trustType, string sepType,
			string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToXRDS(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRDS resolveSEPToXRDS(XRI qxri, TrustType trustType, string sepType,
			string sepMediaType, bool followRefs, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveSEPToXRDS('" + qxri + "', trustType=" + trustType
				+ ", sepType=" + sepType + ", sepMediaType=" + sepMediaType
				+ ", followRefs=" + followRefs + ")");

		ResolverFlags f = new ResolverFlags(trustType, followRefs);
		return resolveSEPToXRDS(qxri, sepType, sepMediaType, f, state);
	}

	/**
	 * @deprecated
	 */
	public XRD resolveSEPToXRD(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToXRD(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRD resolveSEPToXRD(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs,
			ResolverState state) /*throws PartialResolutionException */{
		log.Info("resolveSEPToXRD(s'" + qxri + "', trustType=" + trustType
				+ ", sepType=" + sepType + ", sepMediaType=" + sepMediaType
				+ ", followRefs=" + followRefs + ")");

		XRI xri = parseAbsoluteQXRIOrError(qxri);
		return resolveSEPToXRD(xri, trustType, sepType, sepMediaType,
				followRefs, state);
	}

	/**
	 * @deprecated
	 */
	public XRD resolveSEPToXRD(XRI qxri, TrustType trustType, string sepType,
			string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToXRD(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public XRD resolveSEPToXRD(XRI qxri, TrustType trustType, string sepType,
			string sepMediaType, bool followRefs, ResolverState state)
			/*throws PartialResolutionException */{
		log.Info("resolveSEPToXRD('" + qxri + "', trustType=" + trustType
				+ ", sepType=" + sepType + ", sepMediaType=" + sepMediaType
				+ ", followRefs=" + followRefs + ")");

		XRDS xrds = resolveSEPToXRDS(qxri, trustType, sepType, sepMediaType,
				followRefs, state);
		return xrds.getFinalXRD();
	}

	/**
	 * @deprecated
	 */
	public ArrayList resolveSEPToURIList(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToURIList(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public ArrayList resolveSEPToURIList(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs,
			ResolverState state) /*throws PartialResolutionException */{
		log.Info("resolveSEPToURIList(s'" + qxri + "', trustType=" + trustType
				+ ", sepType=" + sepType + ", sepMediaType=" + sepMediaType
				+ ", followRefs=" + followRefs + ")");

		XRI xri = parseAbsoluteQXRIOrError(qxri);
		return resolveSEPToURIList(xri, trustType, sepType, sepMediaType,
				followRefs, state);
	}

	/**
	 * @deprecated
	 */
	public ArrayList resolveSEPToURIList(XRI qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToURIList(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}

	/**
	 * @deprecated
	 */
	public ArrayList resolveSEPToURIList(XRI qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs,
			ResolverState state) /*throws PartialResolutionException */{
		ResolverFlags f = new ResolverFlags(trustType, followRefs);
		return resolveSEPToURIList(qxri, sepType, sepMediaType, f, state);
	}

	/**
	 * @deprecated
	 */
	public string resolveSEPToTextURIList(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs)
			/*throws PartialResolutionException */{
		return resolveSEPToTextURIList(qxri, trustType, sepType, sepMediaType,
				followRefs, new ResolverState());
	}
	
	/**
	 * @deprecated
	 */
	public string resolveSEPToTextURIList(string qxri, TrustType trustType,
			string sepType, string sepMediaType, bool followRefs,
			ResolverState state) /*throws PartialResolutionException */{
		ResolverFlags f = new ResolverFlags(trustType, followRefs);
		XRI xri = parseAbsoluteQXRIOrError(qxri);
		return resolveSEPToTextURIList(xri, sepType, sepMediaType, f, state);
	}

	/////////////////////////////////////////////////////////////////////////
	// END DEPRECATED APIS                                                 //
	/////////////////////////////////////////////////////////////////////////

	
	
	
	

	/**
	 * Resolve the given <code>qxri</code> according to its type.
	 * If it is an XRIAuthority, get its first authority XRD and resolve the rest of the segment.
	 * If it is an IRIAuthority, just resolve it directly.
	 * @param xrdsOut
	 * @param qxri - QXRI with IRIAuthority or XRIAuthority (GCS or XRef)
	 * @param flags - ResolverFlags
	 * @return
	 */
	public XRDS resolveAuthority(XRI qxri, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		log.Info("resolveAuthority(s'" + qxri + "', flags: " + flags + ")");

		XRDS xrdsOut = new XRDS();
		xrdsOut.setRef("xri://" + qxri.getAuthorityPath().ToString());

		// determine the authority type
		AuthorityPath ap = qxri.getAuthorityPath();
		try {
			XRDS newXRDS;
			if (ap is XRIAuthority)
				newXRDS = resolveXRIAuth(qxri, (XRIAuthority)ap, flags, state);
			else if (ap is IRIAuthority)
				newXRDS = resolveIRIAuth((IRIAuthority) ap, flags, state);
			else
				throw new RuntimeException("Unknown authority type");
			xrdsOut.addAll(newXRDS);
			return xrdsOut;
		} catch (PartialResolutionException e) {
			xrdsOut.addAll(e.getPartialXRDS());
			throw new PartialResolutionException(xrdsOut);
		}
	}

	
	protected XRDS resolveXRIAuth(XRI origQXRI, XRIAuthority xriAuth, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{

		string rootAuth = xriAuth.getRootAuthority();
		if (rootAuth == null) {
			throw new RuntimeException("First subsegment of '" + xriAuth + "' is null");
		}

		XRD rootXRD = getAuthority(rootAuth);
		if (rootXRD == null) {
			XRDS xrdsOut = new XRDS();
			// unknown root
			XRD err = createErrorXRD(
				xriAuth.toURINormalForm(),
				Status.UNKNOWN_ROOT,
				"Authority '" + rootAuth + "' is not configured"
			);
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		XRISegment segment = xriAuth.getXRISegment();

		// resolve segment (list of subsegments not including root) against root XRD
		return resolveAuthSegment(origQXRI, rootXRD, segment, flags, state);
	}

	
	
	
	protected XRDS resolveIRIAuth(IRIAuthority iriAuth, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		log.Info("resolveIRIAuth('" + iriAuth + "', flags: " + flags + ")");

		XRDS xrdsOut = new XRDS();

		if (flags.isSaml()) {
			XRD err = createErrorXRD(iriAuth.toURINormalForm(),
					Status.NOT_IMPLEMENTED,
					"SAML is not supported for an IRI authority");
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		// only use http for insecure and https for secure
		string scheme = flags.isHttps() ? "https" : "http";

		Uri uri = null;
		try {
			uri = new Uri(scheme, iriAuth.getIUserInfo(), iriAuth.getIHost(),
					iriAuth.getPort(), null, null, null);
		} catch (java.net.URISyntaxException e) {
			XRD err = createErrorXRD(iriAuth.toURINormalForm(),
					Status.INVALID_INPUT,
					"Unable to construct Uri to resolve IRI authority: "
							+ e.getMessage());
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		// now that we've constructed the new Uri, try to return the stream from it
		InputStream in = null;
		try {
			in = getDataFromURI(uri, uri.ToString(), flags, state);
		} catch (Exception e) {
			XRD err = createErrorXRD(iriAuth.toURINormalForm(),
					Status.NETWORK_ERROR,
					"Network error occurred while resolving IRI authority: "
							+ e.getMessage());
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		if (in == null) {
			throw new RuntimeException(
					"resolveIRIAuth - getDataFromURI returned null");
		}

		// read the descriptors
		try {
			xrdsOut = readXRDS(in);
			if (xrdsOut.getNumChildren() != 1) {
				XRD err = createErrorXRD(iriAuth.toURINormalForm(),
						Status.UNEXPECTED_RESPONSE,
						"Expected 1 XRD from IRI authority, got "
								+ xrdsOut.getNumChildren() + " instead");
				xrdsOut.add(err);
				throw new PartialResolutionException(xrdsOut);
			}
		} catch (XRIResolutionException e) {
			XRD err = createErrorXRD(iriAuth.toURINormalForm(),
					Status.UNEXPECTED_RESPONSE,
					"Error reading XRDS from server: " + e.getMessage());
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		// add the descriptor, but only if is is valid
		XRD xrd = xrdsOut.getDescriptorAt(0);
		if (!xrd.isValid()) {
			XRD err = createErrorXRD(iriAuth.toURINormalForm(),
					Status.UNEXPECTED_XRD, "XRD is invalid");
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		return xrdsOut;
	}
	

	protected XRDS resolveViaProxy(XRI qxri, string serviceType,
			string serviceMediaType, bool sepSelect, ResolverFlags flags,
			ResolverState state) /*throws PartialResolutionException */{
		log.Info("resolveViaProxy('" + qxri + ", serviceType=" + serviceType
				+ ", serviceMediaType=" + serviceMediaType + ", flags: "
				+ flags + ")");

		XRDS xrdsOut = new XRDS();

		// build the new Uri for the proxy
		Uri newURI = null;
		try {
			StringBuilder query = new StringBuilder();
			if (serviceType != null) {
				query.append("_xrd_t=");
				query.append(URLEncoder.encode(serviceType, "UTF-8"));
				query.append('&');
			}

			if (serviceMediaType != null) {
				query.append("_xrd_m=");
				query.append(URLEncoder.encode(serviceMediaType, "UTF-8"));
				query.append('&');
			}

			query.append("_xrd_r=");
			query.append(Tags.CONTENT_TYPE_XRDS);
			query.append(';');
			query.append(flags.getTrustParameters());
			query.append(";sep=");
			query.append(sepSelect);
			query.append(";ref=");
			query.append(flags.isRefs());
			query.append('&');

			if (proxyURI.getQuery() != null) {
				query.append(proxyURI.getQuery());
			}

			StringBuilder uriBuf = new StringBuilder();
			uriBuf.append(proxyURI.getScheme());
			uriBuf.append("://");
			uriBuf.append(proxyURI.getAuthority());
			uriBuf.append(proxyURI.getPath());
			if (uriBuf.charAt(uriBuf.length() - 1) != '/')
				uriBuf.append('/');

			StringBuilder qxriNoQuery = new StringBuilder(qxri.getAuthorityPath().toIRINormalForm());
			if (sepSelect) {
				qxriNoQuery.append(qxri.getXRIPath().toURINormalForm());
			}
			
			uriBuf.append(URLEncoder.encode(qxriNoQuery.ToString(), "UTF-8"));
			uriBuf.append('?');
			uriBuf.append(query);

			log.Info("resolveViaProxy - constructed proxy query Uri '"
					+ uriBuf + "'");

			newURI = new Uri(uriBuf.ToString());
		} catch (java.net.URISyntaxException oEx) {
			XRD err = createErrorXRD(qxri.getAuthorityPath().toURINormalForm(),
					Status.INVALID_INPUT,
					"Unable to construct Uri to access proxy resolution service");
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		} catch (UnsupportedEncodingException e) {
			// thrown from URLEncoder.encode() - this should never happen since the 
			// US-ASCII encoding should be supported on every computer or so we hope :)
			XRD err = createErrorXRD(qxri.getAuthorityPath().toURINormalForm(),
					Status.INVALID_INPUT, "Charset not supported");
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		} catch (Exception e) {
			XRD err = createErrorXRD(qxri.getAuthorityPath().toURINormalForm(),
					Status.PERM_FAIL,
					"Unexpected error while constructing proxy Uri: "
							+ e.getMessage());
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}

		InputStream in = null;
		try {
			// try to get the data from it
			in = getDataFromURI(newURI, qxri.toURINormalForm(), flags, state);
			XRDS xrds = readXRDS(in);
			XRD finalXRD = xrds.getFinalXRD();

			string code = finalXRD.getStatusCode();
			if ((flags.isRefs() && !code.equals(Status.SUCCESS) && !code
					.equals(Status.REF_NOT_FOLLOWED))
					|| !code.equals(Status.SUCCESS)) {
				// got either a non-SUCCESS code or
				// followRefs is on but we got non-SUCCESS and non-REF_NOT_FOLLOWED
				throw new PartialResolutionException(xrds);
			}
			return xrds;
		} catch (PartialResolutionException e) {
			// re-throw
			throw e;
		} catch (Exception e) {
			XRD err = createErrorXRD(qxri.getAuthorityPath().toURINormalForm(),
					Status.PERM_FAIL, "Error fetching XRDS from proxy: "
							+ e.getMessage());
			xrdsOut.add(err);
			throw new PartialResolutionException(xrdsOut);
		}
	}

	
	protected XRDS processRedirects(XRI qxri, XRD parent, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		log.Info("processRedirects (qxri=" + qxri + ")");
		
		XRDS xrdsOut = new XRDS();
		XRDS tmpXRDS;
		List redirects = parent.getPrioritizedRedirects();
		Iterator it = redirects.iterator();
		
		// there must be some redirects!
		if (!it.hasNext())
			throw new RuntimeException("processRedirects: No redirect to process!");

		// do a depth-first following of redirects
		while (it.hasNext()) {
			Redirect r = (Redirect)it.next();
			log.debug("processRedirects - Got redirect " + r);

			Uri uri;
			try {
				uri = new Uri(r.getValue());
				if (r.getAppend() != null) {
					// construct Uri
					string constructedURI = constructURI(uri, r.getAppend(), qxri);
					uri = new Uri(constructedURI);
				}
			} catch (URISyntaxException e) {
				// skip invalid URIs
				log.warn("processRedirects - Encountered invalid Uri while constructing Redirect Uri"); 
				continue;
			}
			
			try {
				log.info("processRedirects - Fetching Uri (" + uri + ")");
				tmpXRDS = fetchRedirectXRDS(uri, parent, qxri, flags, state);
				xrdsOut.add(tmpXRDS);

				/*
				XRD finalXRD = tmpXRDS.getFinalXRD();
				if (finalXRD.getNumRedirects() > 0) {
					tmpXRDS = processRedirects(qxri, finalXRD, flags, state);
				}
				*/
				
				return xrdsOut; // we're done!
			}
			catch (PartialResolutionException e) {
				xrdsOut.add(e.getPartialXRDS());
				// fall through to continue to the next 
			}
		}
		
		log.info("processRedirects - exhausted list of Redirects. Throwing PRE");
		throw new PartialResolutionException(xrdsOut);
	}
	

	
	protected string verifyCID(XRD prevXRD, XRD xrd) {
		CanonicalID parentCID = prevXRD.getCanonicalID();
		CanonicalID thisCID = xrd.getCanonicalID();
		
		if (parentCID == null) {
			log.warn("verifyCID: no parent CID!");
			return Status.CID_FAILED;
		}
		
		if (thisCID == null)
			return Status.CID_ABSENT;

		try {
			XRI parentCIDXRI = new XRI(parentCID.getValue());
			XRI thisCIDXRI   = new XRI(thisCID.getValue());
			if (parentCIDXRI.isRelative())
				return Status.CID_FAILED;
			
			if (thisCIDXRI.isRelative())
				return Status.CID_FAILED;
			
			AuthorityPath parentAuthorityAP = parentCIDXRI.getAuthorityPath();
			if (parentAuthorityAP == null)
				return Status.CID_FAILED;
			
			AuthorityPath thisAuthorityAP = thisCIDXRI.getAuthorityPath();
			if (thisAuthorityAP == null)
				return Status.CID_FAILED;
			
			if (!(thisAuthorityAP is XRIAuthority))
				return Status.CID_FAILED;
			
			XRI thisAuthorityParentXRI = ((XRIAuthority)thisAuthorityAP).getParentAsXRI();
			if (thisAuthorityParentXRI == null)
				return Status.CID_FAILED;
			
			string parentXRIStr = parentAuthorityAP.toIRINormalForm();
			string thisParentXRIStr = thisAuthorityParentXRI.getAuthorityPath().toIRINormalForm();
			if (parentXRIStr.equalsIgnoreCase(thisParentXRIStr))
				return Status.CID_VERIFIED;
			else
				return Status.CID_FAILED;
		}
		catch (Exception e) {
			log.warn("verifyCID - exception caught: " + e);
		}
		
		return Status.CID_FAILED;
	}

	
	/**
	 * check that each synonym element in the Redirect xrd is present in the parent
	 * @param xrd
	 * @param parent
	 * @return
	 */
	protected string verifyRedirectXRD(XRD xrd, XRD parent)
	{
		CanonicalID cid = xrd.getCanonicalID();
		CanonicalID cid2 = parent.getCanonicalID();
		if (cid != null && cid2 == null)
			return "CanonicalID element is not present in parent XRD";

		if (cid != null) {
			if (!verifySynonymElement(cid, cid2))
				return "CanonicalID element is different from parent's";
		}


		CanonicalEquivID ceid = xrd.getCanonicalEquivID();
		CanonicalEquivID ceid2 = parent.getCanonicalEquivID();
		if (ceid != null && ceid2 == null)
			return "CanonicalEquivID element is not present in parent XRD";

		if (ceid != null) {
			if (!verifySynonymElement(ceid, ceid2))
				return "CanonicalEquivID element is different from parent's";
		}


		for (int i = 0; i < xrd.getNumLocalIDs(); i++) {
			LocalID l = xrd.getLocalIDAt(i);
			
			// search for it in parent
			bool found = false;
			for (int j = 0; j < parent.getNumLocalIDs(); j++) {
				if (verifySynonymElement(l, parent.getLocalIDAt(j))) {
					found = true;
					break;
				}
			}
			
			if (!found) {
				return "LocalID element " + l.getValue() + " is not present in parent XRD";
			}
		}
		
		for (int i = 0; i < xrd.getNumEquivIDs(); i++) {
			EquivID e = xrd.getEquivIDAt(i);
			
			// search for it in parent
			bool found = false;
			for (int j = 0; j < parent.getNumEquivIDs(); j++) {
				if (verifySynonymElement(e, parent.getEquivIDAt(j))) {
					found = true;
					break;
				}
			}
			
			if (!found) {
				return "EquivID element " + e.getValue() + " is not present in parent XRD";
			}
		}
		
		return null;
	}
	
	
	private bool verifySynonymElement(SimpleXMLElement e1, SimpleXMLElement e2)
	{
		try {
			XRI x1 = XRI.fromURINormalForm(e1.getValue());				
			XRI x2 = XRI.fromURINormalForm(e2.getValue());
			return x1.equals(x2);
		}
		catch (XRIParseException e) {
			// CanonicalID not an XRI, we ignore here
		}
		
		try {
			Uri u1 = new Uri(e1.getValue());
			Uri u2 = new Uri(e2.getValue());
			if (!u1.equals(u2))
				return false;
		} catch (URISyntaxException e) {
			return false;
		}
		
		return true;
	}
	
	
	protected string verifyCEID(XRI qxri, XRD xrd, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		CanonicalEquivID ceid = xrd.getCanonicalEquivID();
		if (ceid == null) {
			log.debug("verifyCEID - No CEID in the XRD");
			return Status.CID_ABSENT;
		}

		log.debug("verifyCEID - Verifying CEID (" + ceid.getValue() + ")");
		
		CanonicalID cid = xrd.getCanonicalID();
		if (cid == null) {
			log.debug("verifyCEID - No CID in the XRD");
			return Status.CID_FAILED;
		}
		
		XRI cidX;
		try {
			cidX = XRI.fromURINormalForm(cid.getValue());
		}
		catch (Exception e) {
			log.debug("verifyCEID - Unable to parse CID");
			return Status.CID_FAILED;
		}
		
		if (ceid.getValue().equals(cid.getValue())) {
			log.debug("verifyCEID - CID is character-for-character equivalent to CEID.");
			return Status.CID_VERIFIED;
		}
		
		try {
			XRI ceidX = XRI.fromURINormalForm(ceid.getValue());

			log.info("verifyCEID - resolving CEID");
			XRD ceidXRD = resolveAuthToXRD(ceidX, flags, state);
			Status s = ceidXRD.getStatus();
			if (!s.getCID().equals(Status.CID_VERIFIED)) {
				log.debug("verifyCEID - XRD for CEID is not CID verified");
				return Status.CID_FAILED;
			}
			
			CanonicalID cid2 = ceidXRD.getCanonicalID();
			if (cid2 == null) {
				log.debug("verifyCEID - no CID in the XRD in CEID");
				return Status.CID_FAILED;				
			}
			
			XRI cidX2 = XRI.fromURINormalForm(cid2.getValue());
			if (!cidX2.toURINormalForm().equals(cidX.toURINormalForm())) {
				log.debug("verifyCEID - CEID's XRD's CID is not equivalent to the original CID");
				return Status.CID_FAILED;
			}
			
			/// look for an EquivID or CanonicalEquivID backpointer to the original CID
			CanonicalEquivID ceid2 = ceidXRD.getCanonicalEquivID();
			XRI ceidX2 = XRI.fromURINormalForm(ceid2.getValue());
			if (ceidX2.toURINormalForm().equals(cidX.toURINormalForm())) {
				log.debug("verifyCEID - CEID backpointer found");
				return Status.CID_VERIFIED;
			}

			for (int i = 0; i < ceidXRD.getNumEquivIDs(); i++) {
				EquivID e = ceidXRD.getEquivIDAt(i);
				XRI eX = XRI.fromURINormalForm(e.getValue());
				if (eX.toURINormalForm().equals(cidX.toURINormalForm())) {
					log.debug("verifyCEID - EquivID backpointer found");
					return Status.CID_VERIFIED;					
				}
			}
		}
		catch (Exception e) {
			log.debug("verifyCEID - unknown error: " + e);
		}
		return Status.CID_FAILED;
	}


	
	/**
	 * 
	 * @param uri 
	 * @param parent
	 * @param parentService
	 * @param segment
	 * @param flags
	 * @param state
	 * @return
	 * @throws PartialResolutionException
	 */
	protected XRDS fetchAuthXRDSHelper(XRI qxri, Uri uri, XRD parent, Service parentService, XRISegment segment, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		XRDS xrdsOut = new XRDS();
		string query = segment.getSubSegmentAt(0).toURINormalForm(true);

		Uri newURI;
		try {
			newURI = constructAuthResURI(uri.ToString(), segment.toURINormalForm(true));
			log.Info("fetchAuthXRDS - newURI = " + newURI);
		}
		catch (java.net.URISyntaxException oEx) {
			throw makeResolutionException(xrdsOut, query, Status.AUTH_RES_ERROR, "Invalid Uri for authority resolution service");
		}

		XRDS tmpXRDS = null;
		// now that we've constructed the new Uri, try to return the stream from it
		try {
			InputStream in = getDataFromURI(newURI, segment.ToString(), flags, state);
			tmpXRDS = readXRDS(in);
			log.debug("fetchAuthXRDS - got XRDS = " + tmpXRDS.ToString());
		} catch (IOException e) {
			log.Info("fetchAuthXRDS - got IOException from Uri " + newURI);
			throw makeResolutionException(xrdsOut, query, Status.NETWORK_ERROR, "Networking error encountered");
		} catch (Exception e) {
			log.Info("fetchAuthXRDS - " + e);
			throw makeResolutionException(xrdsOut, query, Status.AUTH_RES_ERROR, e.getMessage());
		}
		
		//// sanity checks
		
		// there should be at least one child element
		if (tmpXRDS.getNumChildren() < 1) {
			throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Invalid XRDS document");
		}
		
		if (tmpXRDS.getNumChildren() > segment.getNumSubSegments()) {
			throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Invalid XRDS document: too many XRD elements returned");			
		}
		

		XRD prevXRD = parent;

		// check each child
		for (int d = 0; d < tmpXRDS.getNumChildren(); d++) {
			if (!tmpXRDS.isXRDAt(d))
				throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Authority XRDS document should not contain XRDS element");

			XRD xrd = tmpXRDS.getDescriptorAt(d);
			xrdsOut.add(xrd);

			// status is not success
			ServerStatus sstat = xrd.getServerStatus();
			Status stat;
			if (sstat == null) {
				// compatibility: if no ServerStatus, look for Status
				stat = xrd.getStatus();
				if (stat != null) {
					xrd.setServerStatus(new ServerStatus(stat.getCode(), stat.getText()));
				}
			}
			else {
				stat = new Status(sstat.getCode(), sstat.getText());
				xrd.setStatus(stat);
			}
			
			if (stat == null)
				throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Missing ServerStatus or Status element in XRD");


			if (!stat.getCode().equals(Status.SUCCESS)) {
				throw new PartialResolutionException(xrdsOut);
			}

			// check the basic properties of the descriptor
			if (!xrd.isValid()) {				
				xrd.setStatus(new Status(Status.UNEXPECTED_RESPONSE, "XRD is not valid (stale?)"));
				throw new PartialResolutionException(xrdsOut);
			}

			if (flags.isCid()) {
				Status parentStatus = parent.getStatus();
				Status s = xrd.getStatus();
				
				string parentCIDStat = parentStatus.getCID(); 
				if (parentCIDStat.equals(Status.CID_FAILED)) {
					s.setCID(Status.CID_FAILED);
				}
				else {
					s.setCID(verifyCID(prevXRD, xrd));
				}
			}
			
			// if we need to do trusted resolution checking
			if (flags.isSaml())
			{
			    // Each descriptor must be validated independently as well as
			    // against the one that preceded (described) it in the
			    // descriptor chain.
				// TODO: there could be more than one Authority Resolution Service
				//       in the final XRD
				
				/*
			    bool valid = isTrustedDescriptor(segment.getSubSegmentAt(d), xrd, parentService);

			    // bail if the descriptor is not valid
			    if (!valid) {
			    	xrd.setStatus(new Status(Status.UNVERIFIED_SIGNATURE, "Signature verification failed."));
			    	throw new PartialResolutionException(xrdsOut);
			    }
			    */
			}

			prevXRD = xrd;
		}
		
		return xrdsOut;

	}
	

	/**
	 * Fetches the XRDS document given the authResServices 
	 * @param qxri
	 * @param parent
	 * @param authResServices
	 * @param segment
	 * @param flags
	 * @param state
	 * @return
	 * @throws PartialResolutionException 
	 */
	protected XRDS fetchAuthXRDS(XRI qxri, XRD parent, List authResServices, XRISegment segment, ResolverFlags flags, ResolverState state) /*throws PartialResolutionException*/
	{
		XRDS xrdsOut = null;
		XRD errXRD = null;
		string query = segment.getSubSegmentAt(0).toURINormalForm(true);
		
		//// TODO verify synonyms

		///// Try each Uri in each selected service in turn
		Exception savedException = null;
		Iterator srvIterator = authResServices.iterator();
		while (srvIterator.hasNext()) {			
			Service srv = (Service) srvIterator.next();
			Iterator uriIterator = srv.getPrioritizedURIs().iterator();
			
			while (uriIterator.hasNext()) {
				SEPUri sepURI = (SEPUri) uriIterator.next();
				Uri uri = sepURI.getURI();

				log.Info("fetchAuthXRDS - trying Uri='" + uri + "'");

				// skip non-HTTPS URIs if HTTPS was requested
				if (flags.isHttps() && !uri.getScheme().equals(HTTPS)) {
					log.Info("fetchAuthXRDS - skipping non HTTPS Uri");
					continue;
				}

				try {
					xrdsOut = fetchAuthXRDSHelper(qxri, uri, parent, srv, segment, flags, state);
					// if no error, return immediately
					return xrdsOut;
				}
				catch (PartialResolutionException e) {
					xrdsOut = e.getPartialXRDS();
				}
			}
		}

		if (xrdsOut == null) { // no appropriate Uri found
			xrdsOut = new XRDS();
			string code = flags.isHttps()? Status.TRUSTED_RES_ERROR : Status.AUTH_RES_ERROR;
			xrdsOut.add(createErrorXRD(query, code, "No Uri found for authority resolution"));
		}
		throw new PartialResolutionException(xrdsOut);
	}

	
	protected void checkMaxRequests(XRDS xrdsOut, string query, ResolverState state)
		/*throws PartialResolutionException*/
	{
		if (maxRequests >= 0 && state.getNumRequests() >= maxRequests) {
			XRD finalXRD = xrdsOut.getFinalXRD();
			if (finalXRD == null) {
				finalXRD = createErrorXRD(query, Status.LIMIT_EXCEEDED, "Maximum of authority resolution requests exceeded");
				xrdsOut.add(finalXRD);
			} else {
				finalXRD.setStatus(new Status(Status.LIMIT_EXCEEDED,
						"Maximum of authority resolution requests exceeded"));
			}
			throw new PartialResolutionException(xrdsOut);
		}
		
	}
	

	private PartialResolutionException
	makeResolutionException(XRDS targetXRDS, string query, string status, string message)
	{
		XRD x = createErrorXRD(query, status, message);
		targetXRDS.add(x);
		return new PartialResolutionException(targetXRDS); 
	}

	
	/**
	 * Resolve the given authority segment (without root) from the
	 * <code>parent</code> (root-authority) XRD. It will follow references if told to do so.
	 * @param xrdsOut - the resulting XRDS document (nested if need be)
	 * @param parent
	 * @param segment
	 * @param flags
	 * @return bool <code>true</code> if the given segment is completely resolved.
	 */
	public XRDS resolveAuthSegment(XRI qxri, XRD parent, XRISegment segment,
			ResolverFlags flags, ResolverState state)
			/*throws PartialResolutionException*/
	{
		log.Info("resolveAuthSegment - segment='" + segment + "'");

		XRDS xrdsOut = new XRDS();
		XRDS tmpXRDS = null;
		CanonicalID parentCID = null;
		bool authResComplete = false;
		ResolverFlags currentFlags = null; // this is only for overriding by HttpsBypassAuthority settings

		string parentXRI = ((XRIAuthority)qxri.getAuthorityPath()).getRootAuthority();
		XRISegment remainingSegment = segment;
		
		while (remainingSegment != null && remainingSegment.getNumSubSegments() > 0) {
			// clone flags
			currentFlags = new ResolverFlags(flags);
			
			// more subsegments to resolve
			string query = remainingSegment.getSubSegmentAt(0).toURINormalForm(true);

			log.debug("resolveAuthSegment - resolving subsegment '" + query + "'");
			
			checkMaxRequests(xrdsOut, query, state);
			
			// if HTTPS is requested and what we are resolving is allowed to bypass HTTPS, we turn off the HTTPS flag
			// for auth-res service selection
			if (currentFlags.isHttps() && isHttpsBypassAuthority(parentXRI)) {
				log.debug("Bypassing HTTPS for " + parentXRI);
				currentFlags.setHttps(false);
			}
			
			//// perform service selection
			string authResMediaType = Tags.CONTENT_TYPE_XRDS + ";" + currentFlags.getTrustParameters();
			List authResServices = selectServices(parent.getServices(), Tags.SERVICE_AUTH_RES, null, authResMediaType, currentFlags);
			if (authResServices.size() < 1) {
				log.debug("resolveAuthSegment - no authority resolution service found!");
				throw makeResolutionException(
					xrdsOut,
					query,
					Status.AUTH_RES_NOT_FOUND,
					"Authority Resolution Service Not Found"
				);
			}

			try {
				// retrieve XRDS documents for the given subsegment
				log.Info("resolveAuthSegment - fetching XRDS");
				tmpXRDS = fetchAuthXRDS(qxri, parent, authResServices, remainingSegment, currentFlags, state);
			}
			catch (PartialResolutionException e) {
				log.Info("got PRE: " + e.getPartialXRDS());
				log.Info("xrdsOut.n = " + xrdsOut.getNumChildren() + ", partialXRDS.n=" + e.getPartialXRDS().getNumChildren());
				xrdsOut.add(e.getPartialXRDS());
				throw new PartialResolutionException(xrdsOut);				
			}
			
			//// add the subsegments
			xrdsOut.addAll(tmpXRDS);
			
			//// replace parent XRD
			parent = tmpXRDS.getFinalXRD();
			
			for (int k = 0; k < tmpXRDS.getNumChildren(); k++) {
				XRISubSegment subseg = remainingSegment.getSubSegmentAt(k);
				parentXRI = parentXRI + subseg;
			}
			
			remainingSegment = remainingSegment.getRemainder(tmpXRDS.getNumChildren());

			tmpXRDS = null;			
			try {
				if (parent.getNumRedirects() > 0) {
					log.debug("resolveAuthSegment - processing Redirect(s)");
					tmpXRDS = processRedirects(qxri, parent, currentFlags, state);
					xrdsOut.addAll(tmpXRDS);
					// replace parent
					parent = tmpXRDS.getFinalXRD();
				}
				else if (parent.getNumRefs() > 0) {
					if (!currentFlags.isRefs()) {
						throw makeResolutionException(
							xrdsOut,
							query,
							Status.REF_NOT_FOLLOWED,
							"Ref not followed");
					}
					log.debug("resolveAuthSegment - processing Ref(s)");
					tmpXRDS = processRefs(parent, currentFlags, state);
					xrdsOut.addAll(tmpXRDS);
					// replace parent
					parent = tmpXRDS.getFinalXRD();
				}				
			}
			catch (PartialResolutionException e) {
				xrdsOut.addAll(e.getPartialXRDS());
				log.debug("resolveAuthSegment - got PRE while processing Ref or Redirect");
				throw new PartialResolutionException(xrdsOut);
			}
		}
		
		log.debug("resolveAuthSegment - successfully resolved all subsegments");
		if (parent.getStatus().getCID().equals(Status.CID_VERIFIED)
				&& parent.getCanonicalEquivID() != null)
		{
			log.debug("resolveAuthSegment - final XRD contains a CanonicalEquivID. Verifying...");
			string vStat = verifyCEID(qxri, parent, currentFlags, state);
			parent.getStatus().setCEID(vStat);
		}
		return xrdsOut;
	}

	
	
	protected XRDS processRefs(XRD parent, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		XRDS xrdsOut = new XRDS();
		//// get all the Refs in the parent XRD
		List refs = parent.getPrioritizedRefs();
		Iterator it = refs.iterator();
		
		//// try each one in turn
		while (it.hasNext()) {
			Ref _ref = (Ref)it.next();

			checkMaxRefs(xrdsOut, _ref.getValue(), state);

			XRI refXRI;
			try {
				refXRI = parseAbsoluteQXRIOrError(_ref.getValue());
			}
			catch (PartialResolutionException e) {
				xrdsOut.add(e.getPartialXRDS());
				continue;
			}
			
			// record that we are following a ref
			state.pushFollowingRef(refXRI);
			
			try {
				XRDS tmpXRDS = resolveAuthority(refXRI, flags, state);
				xrdsOut.add(tmpXRDS);
				break;
			}
			catch (PartialResolutionException e) {
				xrdsOut.add(e.getPartialXRDS());
				if (!it.hasNext()) // last ref, throw the exception (otherwise, continue)
					throw new PartialResolutionException(xrdsOut);
			}
		}
		return xrdsOut;
	}

	
	protected void checkMaxRefs(XRDS xrdsOut, string query, ResolverState state) {
	}

	
	
    protected bool isTrustedDescriptor(XRISubSegment subseg, XRD xrd, Service parentService) {
		// Descriptor must provide an XML ID for SAML assertion validation
		string sXMLID = xrd.getXmlID();
		if ((sXMLID == null) || (sXMLID.length() == 0)) {
			return false;
		}

		// Must contain an authority id
		string sAuthorityID = xrd.getProviderID();
		if ((sAuthorityID == null) || (sAuthorityID.length() == 0)) {
			return false;
		}

		// The resolved element must match the subsegment we were looking for
		string sSubsegment = subseg.ToString();
		if ((xrd.getQuery() == null)
				|| (!xrd.getQuery().equals(sSubsegment))) {
			return false;
		}

		// Must contain a SAML assertion
		Assertion oAssertion = xrd.getSAMLAssertion();
		if (oAssertion == null) {
			return false;
		}

		// Assertion must also be valid
		if (!oAssertion.isValid()) {
			return false;
		}

		// Subject must have correct info
		Subject oSubject = oAssertion.getSubject();
		if (oSubject == null) {
			return false;
		}

		NameID oNameID = oSubject.getNameID();
		if (oNameID == null) {
			return false;
		}

		if (!sSubsegment.equals(oNameID.getValue())) {
			return false;
		}

		if (!sAuthorityID.equals(oNameID.getNameQualifier())) {
			return false;
		}

		// must contain a SAML Attribute statement
		AttributeStatement oAttrStmt = oAssertion.getAttributeStatement();
		if (oAttrStmt == null) {
			return false;
		}

		// Attribute must describe the XML ID reference
		Attribute oAttr = oAttrStmt.GetAttribute();
		if ((oAttr.getName() == null)
				|| (!oAttr.getName().equals(Tags.NS_XRD_V2))) {
			return false;
		}

		string sValue = oAttr.getValue();
		if ((sValue == null) || (!sValue.equals("#" + sXMLID))) {
			return false;
		}

		// The describing descriptor must contain an authority
		if (parentService == null) {
			return false;
		}

		// Describing authority id must match this descriptor's authority id
		if ((parentService.getProviderId() == null)
				|| (!parentService.getProviderId().equals(sAuthorityID))) {
			return false;
		}

		// Describing authority must provide KeyInfo
		KeyInfo oKeyInfo = parentService.getKeyInfo();
		if (oKeyInfo == null) {
			log.debug("KeyInfo is missing from describing Authority.");
			return false;
		}

		// Verify assertion
		try {
			xrd.verifySignature(oKeyInfo.getPublicKey());
		} catch (XMLSecurityException oEx) {
			log.debug("Assertion verification failed.", oEx);
			return false;
		}

		return true;
	}
	
	/**
	 * @param xrd
	 * @param qxri
	 * @param sepType
	 * @param sepMediaType
	 * @param flags
	 * @param state
	 * @return
	 * @throws PartialResolutionException
	 */
	public List selectServiceFromXRD(XRDS xrdsOut, XRD xrd, XRI qxri, string sepType,
			string sepMediaType, ResolverFlags flags, ResolverState state)
			/*throws PartialResolutionException */{
		// get the QXRI path
		string path = null;
		XRIAbsolutePath absPath = qxri.getXRIAbsolutePath();
		if (absPath != null)
			path = absPath.toURINormalForm();

		// find services
		List selectedSvcs = selectServices(xrd.getServices(), sepType, path, sepMediaType, flags);
		xrd.setSelectedServices(selectedSvcs);
		if (selectedSvcs.size() == 0) {
			Status s = xrd.getStatus();
			s.setCode(Status.SEP_NOT_FOUND);
			s.setText("Requested service endpoint not found");
			throw new PartialResolutionException(xrdsOut);
		}

		// check if first Service has Redirect of Ref
		Service srv = (Service)selectedSvcs.get(0);
		if (srv.getNumRedirects() > 0) {
			// try to follow each redirect (which will recurse back into this method if the XRDS was fetched successfully.)
			selectedSvcs = processServiceRedirects(xrdsOut, srv, xrd, qxri, sepType, sepMediaType, flags, state);
		}
		else if (srv.getNumRefs() > 0) {
			selectedSvcs = processServiceRefs(xrdsOut, srv, xrd, qxri, sepType, sepMediaType, flags, state);
		}

		return selectedSvcs;
	}

	
	
	/**
	 * 
	 * @param xrdsOut Each 
	 * @param srv
	 * @param parent
	 * @param qxri
	 * @param sepType
	 * @param sepMediaType
	 * @param flags
	 * @param state
	 * @return
	 * @throws PartialResolutionException
	 */
	private List processServiceRedirects(XRDS xrdsOut, Service srv, XRD parent, XRI qxri,
			string sepType, string sepMediaType, ResolverFlags flags,
			ResolverState state) /*throws PartialResolutionException*/
	{
		List redirects = srv.getPrioritizedRedirects();
		Iterator it = redirects.iterator();
		
		// there must be some redirects!
		if (!it.hasNext())
			throw new RuntimeException("processServiceRedirects: No redirect to process!");

		// do a depth-first following of redirects
		while (it.hasNext()) {
			Redirect r = (Redirect)it.next();
			Uri uri;
			try {
				uri = new Uri(r.getValue());
				if (r.getAppend() != null) {
					// construct Uri
					string constructedURI = constructURI(uri, r.getAppend(), qxri);
					uri = new Uri(constructedURI);
				}
			} catch (URISyntaxException e) {
				XRDS tmpXRDS = new XRDS();
				XRD err = createErrorXRD(r.getValue(), Status.INVALID_REDIRECT, "Invalid Redirect Uri");
				tmpXRDS.add(err);
				xrdsOut.add(tmpXRDS);
				continue;
			}
			
			try {
				XRDS tmpXRDS = fetchRedirectXRDS(uri, parent, qxri, flags, state);
				xrdsOut.add(tmpXRDS);
				
				XRD finalXRD = tmpXRDS.getFinalXRD();
				tmpXRDS = new XRDS();
				List services = selectServiceFromXRD(tmpXRDS, finalXRD, qxri, sepType, sepMediaType, flags, state);
				xrdsOut.addAll(tmpXRDS);
				return services; // we're done!
			} catch (XRIResolutionException e) {
				XRDS tmpXRDS = new XRDS();
				XRD err = createErrorXRD(uri.ToString(), Status.REDIRECT_ERROR, "Error fetching XRDS: " + e.getMessage());
				tmpXRDS.add(err);
				xrdsOut.add(tmpXRDS);
				
				// fall through to continue to the next 
			}
		}
		throw new PartialResolutionException(xrdsOut);
	}
	
	protected List processServiceRefs(XRDS xrdsOut, Service srv, XRD parent, XRI qxri, string sepType, string sepMediaType, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
		//// get all the Refs in the parent XRD
		List refs = srv.getPrioritizedRefs();
		Iterator it = refs.iterator();
		
		//// try each one in turn
		while (it.hasNext()) {
			Ref _ref = (Ref)it.next();
	
			checkMaxRefs(xrdsOut, _ref.getValue(), state);
	
			XRI refXRI;
			try {
				refXRI = parseAbsoluteQXRIOrError(_ref.getValue());
			}
			catch (PartialResolutionException e) {
				xrdsOut.add(e.getPartialXRDS());
				continue;
			}
			
			// record that we are following a ref
			state.pushFollowingRef(refXRI);
			
			try {
				XRDS tmpXRDS = resolveSEPToXRDS(refXRI, sepType, sepMediaType, flags, state);
				xrdsOut.add(tmpXRDS);
				return tmpXRDS.getFinalXRD().getSelectedServices().getList();
			}
			catch (PartialResolutionException e) {
				xrdsOut.add(e.getPartialXRDS());
				// fall through to continue to the next
			}
		}
		throw new PartialResolutionException(xrdsOut);
	}



	/**
	 * Filter <code>services</code> based on the given <code>type</code>, <code>path</code>
	 * and <code>mediaType</code> criteria and return a prioritized list of services.
	 * @param services
	 * @param type
	 * @param path
	 * @param mediaType
	 * @return ArrayList containing Services in priority order
	 */
	protected ArrayList selectServices(
			ArrayList services,
			string type,
			string path,
			string mediaType,
			ResolverFlags flags)
	{
		if (services == null || services.size() == 0)
			return new ArrayList();

		SEPSelector selector = new SEPSelector(new ArrayList(services));
		//		ArrayList selectedServices = selector.getSelectedSEPs(type, mediaType, path);
		List selectedServices = SEPSelector.select(services, type, mediaType,
				path, flags);
		if (selectedServices == null || selectedServices.size() == 0)
			return new ArrayList();

		PrioritizedList list = new PrioritizedList();
		for (int i = 0; i < selectedServices.size(); i++) {
			Service s = (Service) selectedServices.get(i);
			string priority = (s.getPriority() == null) ? PrioritizedList.PRIORITY_NULL
					: s.getPriority().ToString();
			list.addObject(priority, s);
		}

		return list.getList();
	}

	
	/**
	 * Gets an InputStream from the Uri according the XRI HTTP Bindings.
	 * 
	 * @param uri -
	 *            The Uri to get data from
	 * @param flags -
	 *            ResolverFlags
	 * @return InputStream if HTTP OK is received, null if HTTP Not found
	 * @throws XRIResolutionException
	 *             if HTTP response is not OK or Not Found
	 */
	protected InputStream getDataFromURI(Uri uri, string query, ResolverFlags flags, ResolverState state)
		/*throws XRIResolutionException, IOException*/
	{
		// Post request
		HttpURLConnection conn = null;
		InputStream in = null;

		try {
			// add the appropriate accept header
			LinkedHashMap requestProp = new LinkedHashMap();
			string sContentType = Tags.CONTENT_TYPE_XRDS + ";"
					+ flags.getTrustParameters();
			requestProp.put(Tags.HEADER_ACCEPT, sContentType);

			conn = IO.getConnectionToURI(uri, "GET", requestProp,
					moSocketFactory, maxHttpRedirects, false);

			if (conn.getResponseCode() != HttpURLConnection.HTTP_OK) {
				conn.disconnect();
				throw new XRIResolutionException(
						"Got bad response code from Uri: " + uri.ToString()
								+ ", code = " + conn.getResponseCode());
			}

			// read the bytes
			int bufSize = 4096;
			byte[] buf = new byte[bufSize];
			int bufIndex = 0;
			in = conn.getInputStream();

			while (true) {
				int n = in.read();
				if (n == -1) // EOF
					break;

				if (maxBytesPerRequest >= 0 && bufIndex >= maxBytesPerRequest) {
					throw new XRIResolutionException(Status.LIMIT_EXCEEDED,
							"Maximum response size exceeded");
				}

				if (maxTotalBytes >= 0
						&& state.getNumBytesReceived() + bufIndex >= maxTotalBytes) {
					throw new XRIResolutionException(Status.LIMIT_EXCEEDED,
							"Maximum total received bytes exceeded");
				}

				if (bufIndex >= bufSize) {
					byte[] newBuf = new byte[bufSize * 2];
					System.arraycopy(buf, 0, newBuf, 0, bufSize);
					buf = newBuf;
					bufSize *= 2;
				}
				buf[bufIndex++] = (byte) n;
			}

			// using Latin1 allows the length() method to return the exact bytes
			string bufString = new string(buf, 0, bufIndex, "iso-8859-1");
			state.pushResolved(query, flags.ToString(), bufString, uri);
			return new ByteArrayInputStream(buf, 0, bufIndex);
		} catch (java.io.IOException e) {
			// There was a communication error
			log.warn("Failed XRI lookup from " + uri.ToString()
					+ ".  IOException " + e);
			throw e;
		} finally {
			if (in != null) {
				try {
					in.close();
				} catch (IOException e) {
				}
			}
		}

	}

	
	
	protected XRDS fetchRedirectXRDS(Uri uri, XRD parent, XRI qxri, ResolverFlags flags, ResolverState state)
		/*throws PartialResolutionException*/
	{
			XRDS xrdsOut = new XRDS();
			string query = qxri.toURINormalForm();
			xrdsOut.setRedirect(uri.ToString());
			
			XRDS tmpXRDS = null;
			try {
				log.info("fetchRedirectXRDS - fetching from Uri(" + uri + ")");
				InputStream in = getDataFromURI(uri, query, flags, state);
				
				log.info("fetchRedirectXRDS - reading content from Uri(" + uri + ")");
				tmpXRDS = readXRDS(in);

				log.debug("fetchRedirectXRDS - got XRDS = " + tmpXRDS.ToString());
			} catch (IOException e) {
				log.error("fetchRedirectXRDS - got IOException from Uri " + uri);
				throw makeResolutionException(xrdsOut, query, Status.NETWORK_ERROR, "Networking error encountered");
			} catch (Exception e) {
				log.error("fetchRedirectXRDS - unexpected error: " + e);
				e.printStackTrace();
				throw makeResolutionException(xrdsOut, query, Status.AUTH_RES_ERROR, e.getMessage());
			}

			//// sanity checks
			
			// there should be exactly one child element
			if (tmpXRDS.getNumChildren() != 1 || !tmpXRDS.isXRDAt(0)) {
				throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Invalid XRDS document: single XRD element expected");
			}
			
			if (!tmpXRDS.isXRDAt(0))
				throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Authority XRDS document should not contain XRDS element");

			XRD xrd = tmpXRDS.getDescriptorAt(0);
			xrdsOut.add(xrd);


			ServerStatus sstat = xrd.getServerStatus();
			Status stat;
			if (sstat == null) {
				// compatibility: if no ServerStatus, look for Status
				stat = xrd.getStatus();
			}
			else {
				stat = new Status(sstat.getCode(), sstat.getText());
				xrd.setStatus(stat);
			}
			
			if (stat == null)
				throw makeResolutionException(xrdsOut, query, Status.INVALID_XRDS, "Missing ServerStatus or Status element in Redirect XRD");

			if (!stat.getCode().equals(Status.SUCCESS)) {
				throw new PartialResolutionException(xrdsOut);
			}

			// check the basic properties of the descriptor
			if (!xrd.isValid()) {				
				xrd.setStatus(new Status(Status.UNEXPECTED_RESPONSE, "XRD is not valid (stale?)"));
				throw new PartialResolutionException(xrdsOut);
			}
			
			string err = verifyRedirectXRD(xrd, parent);
			if (err != null) {
				xrd.setStatus(new Status(Status.REDIRECT_VERIFY_FAILED, err));
				throw new PartialResolutionException(xrdsOut);
			}
			
			// copy parent Status cid attribute if present
			stat.setCID(xrd.getCanonicalID() == null? Status.CID_ABSENT : parent.getStatus().getCID());
			
			// copy parent Status ceid attribute (without checking if present since it is supposed to be "off" for
			// non-final XRD, and we don't know whether this is a final XRD.)
			stat.setCEID(parent.getStatus().getCEID());
			
			try {
				if (xrd.getNumRedirects() > 0) {
					log.info("fetchRedirectXRDS - XRD at Uri(" + uri + ") contains Redirect(s), following..");
					tmpXRDS = processRedirects(qxri, xrd, flags, state);
					xrdsOut.addAll(tmpXRDS);
				}
				else if (xrd.getNumRefs() > 0) {
					if (!flags.isRefs()) {
						throw makeResolutionException(
							xrdsOut,
							query,
							Status.REF_NOT_FOLLOWED,
							"Ref not followed");
					}
					log.info("fetchRedirectXRDS - XRD at Uri(" + uri + ") contains Ref(s), following..");
					tmpXRDS = processRefs(xrd, flags, state);
					xrdsOut.addAll(tmpXRDS);
				}
			}
			catch (PartialResolutionException e) {
				xrdsOut.addAll(e.getPartialXRDS());
				throw new PartialResolutionException(xrdsOut);
			}

			return xrdsOut;
	}
	

	protected XRDS readXRDS(InputStream in) /*throws XRIResolutionException */{
		XRDS xrds = null;

		if (in == null) {
			return xrds;
		}

		// Read response into DOM structure
		try {
			log.debug("readXRDS - parsing input stream");
			DOMParser domParser = DOMUtils.getDOMParser();
			domParser.parse(new InputSource(in));
			Document doc = domParser.getDocument();
			Element element = doc.getDocumentElement();
			log.debug("readXRDS - successfully read XML document into DOM");
			xrds = new XRDS(element, true);
			log.debug("readXRDS - successfully parsed XRDS document");
		} catch (IOException e) {
			throw new XRIResolutionException("I/O error while reading XRDS document: " + e, e);
		} catch (SAXException e) {
			throw new XRIResolutionException("Invalid XRDS document: " + e, e);
		} catch (URISyntaxException e) {
			throw new XRIResolutionException("Error parsing XRDS document (URISyntaxException): " + e, e);
		} catch (ParseException e) {
			throw new XRIResolutionException("Error parsing XRDS document (ParseException)", e);
		} finally {
			try {
				in.close();
			} catch (IOException e) {
			}
		}

		return xrds;
	}


	/**
	 * Creates a Resolver obj configured by properties.
	 * @throws ParseException 
	 * @throws URISyntaxException 
	 */
	public Resolver(Properties properties) throws URISyntaxException,
			ParseException {
	
		this();
	
		int maxFollowRedirects = Integer.parseInt(properties.getProperty(
				"MaxHttpRedirectAllowed", "10"));
		int maxFollowRefs = Integer.parseInt(properties.getProperty(
				"MaxFollowRefs", "10"));
		int maxRequests = Integer.parseInt(properties.getProperty(
				"MaxRequests", "10"));
		int maxTotalBytes = Integer.parseInt(properties.getProperty(
				"MaxTotalBytes", "1048576")); // 1Mb
		int maxBytesPerRequest = Integer.parseInt(properties.getProperty(
				"MaxBytesPerRequest", "512000")); // 500Kb
		XRD equalsAuthority = XRD.parseXRD(properties
				.getProperty("EqualsAuthority"), false);
		XRD atAuthority = XRD.parseXRD(properties.getProperty("AtAuthority"),
				false);
		XRD bangAuthority = XRD.parseXRD(properties
				.getProperty("BangAuthority"), false);
		string supports = properties.getProperty("SupportedResMediaTypes");
	
		this.setMaxHttpRedirects(maxFollowRedirects);
		this.setMaxFollowRefs(maxFollowRefs);
		this.setMaxRequests(maxRequests);
		this.setMaxTotalBytes(maxTotalBytes);
		this.setMaxBytesPerRequest(maxBytesPerRequest);
	
		this.setAuthority("=", equalsAuthority);
		this.setAuthority("@", atAuthority);
		this.setAuthority("!", bangAuthority);
	}

	public XRD createErrorXRD(string query, string code, string msg) {
		XRD err = new XRD();
		err.setQuery(query);
		Status stat = new Status(code, msg);
		err.setStatus(stat);
		return err;
	}

	
	
	public Uri constructAuthResURI(string sepURI, string segment)
			/*throws URISyntaxException*/
	{
		
		if (false) { // old construction rules
			Uri uri = new Uri(sepURI);
	
			StringBuilder sepURIStr = new StringBuilder(uri.getScheme());
			sepURIStr.append("://");
			sepURIStr.append(uri.getAuthority());
			sepURIStr.append(uri.getPath());
	
			if (sepURIStr.length() < 0
					|| sepURIStr.charAt(sepURIStr.length() - 1) != '/')
				sepURIStr.append('/');
			sepURIStr.append(segment);

			if (uri.getQuery() != null) {
				sepURIStr.append("?");
				sepURIStr.append(uri.getQuery());
			}
	
			Uri newURI = new Uri(sepURIStr.ToString());
			return newURI;
		}
		
		StringBuilder sb = new StringBuilder(sepURI.ToString());
		if (sb.length() == 0 ||
				sb.charAt(sb.length() - 1) != '/')
			sb.append('/');
		sb.append(segment);
		return new Uri(sb.ToString());
	}

	public string constructURI(Uri sepURI, string append, XRI qxri) {
		log.Info("constructURI - sepURI=" + sepURI + ", append=" + append
				+ ", qxri=" + qxri);

		if (append == null)
			return sepURI.ToString();
		
		StringBuilder result = new StringBuilder(sepURI.ToString());

		if (append.equals(SEPUri.APPEND_NONE)) {
		} else if (append.equals(SEPUri.APPEND_LOCAL)) {
			XRIPath path = qxri.getXRIPath();
			if (path != null) {
				result.append(path.toURINormalForm());
			}

			XRIQuery query = qxri.getQuery();
			if (query != null) {
				result.append('?');
				result.append(query);
			}
		} else if (append.equals(SEPUri.APPEND_AUTHORITY)) {
			AuthorityPath a = qxri.getAuthorityPath();
			if (a != null)
				result.append(a.toURINormalForm());
		} else if (append.equals(SEPUri.APPEND_PATH)) {
			XRIAbsolutePath p = qxri.getXRIAbsolutePath();
			if (p != null)
				result.append(p.toURINormalForm());
		} else if (append.equals(SEPUri.APPEND_QUERY)) {
			XRIQuery q = qxri.getQuery();
			if (q != null && q.ToString().length() > 0)
				result.append("?" + q.ToString());
		} else if (append.equals(SEPUri.APPEND_QXRI)) {
			string qxriParam = qxri.toURINormalForm();
			if (qxriParam.startsWith("xri://"))
				qxriParam = qxriParam.substring(6);
			result.append(qxriParam);
		}

		log.Info("constructURI - returning '" + result + "'");
		return result.ToString();
	}

	
	private void constructURIinXRD(XRD finalXRD, XRI qxri) {
		List seps = finalXRD.getSelectedServices().getList(); // does not clone
		Iterator it = seps.iterator();
		
		log.debug("constructURIinXRD()");
		
		while (it.hasNext()) {
			log.debug("constructURIinXRD() got selected service");
			Service sep = (Service)it.next();
			List uris = sep.getURIs();
			Iterator itURI = uris.iterator();
			while (itURI.hasNext()) {
				SEPUri uri = (SEPUri)itURI.next();
				string append = uri.getAppend();
				if (append != null) {
					string r = constructURI(uri.getURI(), uri.getAppend(), qxri);
					try {
						uri.setURI(new Uri(r));
						uri.setAppend(null);
					} catch (URISyntaxException e) {}
				}
			}
		}

		for (int i = 0; i < finalXRD.getNumServices(); i++) {
			log.debug("constructURIinXRD() got service");
			Service sep = (Service)finalXRD.getServiceAt(i);
			List uris = sep.getURIs();
			Iterator itURI = uris.iterator();
			while (itURI.hasNext()) {
				SEPUri uri = (SEPUri)itURI.next();
				string append = uri.getAppend();
				if (append != null) {
					string r = constructURI(uri.getURI(), uri.getAppend(), qxri);
					try {
						uri.setURI(new Uri(r));
						uri.setAppend(null);
					} catch (URISyntaxException e) {}
				}
			}			
		}
	}


	/**
	 * Parse the given absolute <code>qxri</code>.
	 * @param qxri QXRI to parse
	 * @return XRI
	 */
	private XRI parseAbsoluteQXRIOrError(string qxri)
			/*throws PartialResolutionException */{
		try {
			XRI xri = new XRI(qxri);
			if (xri.isRelative()) {
				XRD err = createErrorXRD(qxri, Status.INVALID_QXRI,
						"QXRI is not absolute.");
				throw new PartialResolutionException(err);
			}
			return xri;
		} catch (XRIParseException e) {
			XRD err = createErrorXRD(qxri, Status.INVALID_QXRI,
					"QXRI parse error: " + e.getMessage());
			throw new PartialResolutionException(err);
		} catch (Exception e) {
			XRD err = createErrorXRD(qxri, Status.PERM_FAIL,
					"Unexpected error while parsing input: " + e.getMessage());
			throw new PartialResolutionException(err);
		}
	}

	protected XRI parseAbsoluteXRI(string qxri) {
		try {
			XRI xri = new XRI(qxri);
			if (xri.isRelative()) {
				log.warn("parseAbsoluteXRI - '" + qxri + "' is not absolute!");
				return null;
			}
			return xri;
		} catch (XRIParseException e) {
			log.warn("parseAbsoluteXRI - failed to parse '" + qxri + "' - "
					+ e.getMessage());
			return null;
		}
	}

	/**
	 * @param maxFollowRefs The maxFollowRefs to set.
	 */
	public void setMaxFollowRefs(int maxFollowRefs) {
		this.maxFollowRefs = maxFollowRefs;
	}

	/**
	 * @param maxRequests The maxRequests to set.
	 */
	public void setMaxRequests(int maxRequests) {
		this.maxRequests = maxRequests;
	}

	/**
	 * @param maxTotalBytes The maxTotalBytes to set.
	 */
	public void setMaxTotalBytes(int maxTotalBytes) {
		this.maxTotalBytes = maxTotalBytes;
	}

	/**
	 * @param maxBytesPerRequest The maxBytesPerRequest to set.
	 */
	public void setMaxBytesPerRequest(int maxBytesPerRequest) {
		this.maxBytesPerRequest = maxBytesPerRequest;
	}

	/**
	 * Queries the resolver to see if the given authority has been configured.
	 * @param auth - authority to query
	 * @return true if authority is configured, false otherwise.
	 */
	public bool hasAuthority(string auth) {
		return root.containsKey(auth);
	}

	/**
	 * Returns the <code>XRD</code> representing the given authority.
	 * @param auth - authority to query
	 * @return XRD representing the authority
	 */
	public XRD getAuthority(string auth) {
		Object xrd = root.get(auth);
		return (xrd == null) ? null : (XRD) xrd;
	}

	/**
	 * Configures the Resolver to use the given <code>XRD</code> for the authority.
	 * @param auth - GCS or Cross Reference authority to configure
	 * @param authXRD - XRD representing the authority
	 */
	public void setAuthority(string auth, XRD authXRD) {
		root.put(auth, authXRD);
	}

	
	/**
	 * Configures the Resolver to bypass HTTPS for the given authorities, while satisfying the https=true requirement.
	 */
	public void addHttpsBypassAuthority(string auth) {
		needNoHttps.put(auth.toLowerCase(), Boolean.TRUE);
	}
	
	
	/**
	 * Tests to see if resolving names at the given authority can bypass HTTPS while satisfying the https=true requirement.
	 */
	public bool isHttpsBypassAuthority(string auth) {
		if (needNoHttps.containsKey(auth.toLowerCase())) {
			return true;
		}
		return false;
	}
	
	
	/**
	 * @return Returns the proxyURI.
	 */
	public Uri getProxyURI() {
		return proxyURI;
	}

	/**
	 * @param proxyURI The proxyURI to set.
	 */
	public void setProxyURI(Uri proxyURI) {
		this.proxyURI = proxyURI;
	}

}
}