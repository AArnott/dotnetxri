/*
 * Copyright 2005 OpenXRI Foundation
 * Subsequently ported and altered by Andrew Arnott and Troels Thomsen
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

using System;
using System.Text;

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This class provides a strong typing for a XRI.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRI.  THERE ARE INTENTIONALLY NO SET METHODS.  Use this class like
	/// java.lang.String or java.net.URI
	/// </summary>
	public class XRI : Parsable, XRIReference
	{
		public const string PDELIM_S = "!";
		public const string RDELIM_S = "*";
		public const char PDELIM = '!';
		public const char RDELIM = '*';
		public const string XRI_SCHEME = "xri://";
		public static readonly int XRI_SCHEME_LENGTH = XRI_SCHEME.Length;

		AuthorityPath moAuthorityPath = null;
		XRIAbsolutePath moAbsolutePath = null;
		XRIQuery query = null;
		XRIFragment fragment = null;

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		internal XRI()
		{ }

		/// <summary>
		/// Constructs an XRI from the provided XRI
		/// </summary>
		/// <param name="oXRI"></param>
		public XRI(XRI oXRI)
		{
			moAuthorityPath = oXRI.AuthorityPath;
			moAbsolutePath = oXRI.XRIAbsolutePath;
			query = oXRI.Query;
			fragment = oXRI.Fragment;
			setParsedXRI();
		}

		/// <summary>
		/// absolute path
		/// </summary>
		public XRIAbsolutePath XRIAbsolutePath
		{
			get
			{
				return moAbsolutePath;
			}
		}

		/// <summary>
		/// Constructs XRI from String
		/// </summary>
		/// <param name="sXRI"></param>
		public XRI(string sXRI)
			: base(sXRI)
		{
			parse();
		}

		/// <summary>
		/// Constructs an XRI from the provided AuthorityPath
		/// </summary>
		/// <param name="oAuthority"></param>
		public XRI(AuthorityPath oAuthority)
		{
			moAuthorityPath = oAuthority;
			setParsedXRI();
		}

		/// <summary>
		/// Constructs an XRI from the provided AuthorityPath and LocalPath
		/// </summary>
		/// <param name="oAuthority"></param>
		/// <param name="oPath"></param>
		public XRI(AuthorityPath oAuthority, XRIPath oPath)
		{
			if (oAuthority == null)
			{
				throw new XRIParseException();
			}

			moAuthorityPath = oAuthority;
			if (oPath != null)
			{
				if (oPath is XRINoSchemePath)
				{
					moAbsolutePath = new XRIAbsolutePath((XRINoSchemePath)oPath);
				}
				else if (oPath is XRIAbsolutePath)
				{
					moAbsolutePath = (XRIAbsolutePath)oPath;
				}
			}
			setParsedXRI();
		}
		
		/// <summary>
		/// Constructs an XRI from the provided AuthorityPath, LocalPath, Query and Fragment
		/// </summary>
		/// <param name="oAuthority"></param>
		/// <param name="oPath"></param>
		/// <param name="query"></param>
		/// <param name="fragment"></param>
		public XRI(AuthorityPath oAuthority, XRIPath oPath, XRIQuery query, XRIFragment fragment)
		{
			if (oAuthority == null)
			{
				throw new XRIParseException();
			}

			moAuthorityPath = oAuthority;
			if (oPath != null)
			{
				if (oPath is XRINoSchemePath)
				{
					moAbsolutePath = new XRIAbsolutePath((XRINoSchemePath)oPath);
				}
				else if (oPath is XRIAbsolutePath)
				{
					moAbsolutePath = (XRIAbsolutePath)oPath;
				}
			}

			this.query = query;
			this.fragment = fragment;

			setParsedXRI();
		}

		/// <summary>
		/// Constructs an XRI from the provided XRI reference in IRI Normal Form
		/// </summary>
		/// <param name="iri"></param>
		/// <returns></returns>
		public static XRI fromIRINormalForm(string iri)
		{
			string xriNF = IRIUtils.IRItoXRI(iri);
			return new XRI(xriNF);
		}

		/// <summary>
		/// Constructs an XRI from the provided XRI reference in URI Normal Form
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static XRI fromURINormalForm(string uri)
		{
			string iriNF;
			try
			{
				iriNF = IRIUtils.URItoIRI(uri);
			}
			catch (UnsupportedEncodingException e)
			{
				// we're only using UTF-8 which should really be there in every JVM
				throw new XRIParseException("UTF-8 encoding not supported: " + e.getMessage());
			}

			string xriNF = IRIUtils.IRItoXRI(iriNF);
			return new XRI(xriNF);
		}

		/// <summary>
		/// This is used by constructors that need to set the parsed value
		/// without actually parsing the XRI.
		/// </summary>
		void setParsedXRI()
		{
			string sValue = XRI_SCHEME + moAuthorityPath.ToString();

			// add the local path and relative path as necessary
			if (moAbsolutePath != null)
			{
				sValue += moAbsolutePath.ToString();
			}

			if (query != null)
			{
				sValue += query.ToString();
			}

			if (fragment != null)
			{
				sValue += query.ToString();
			}

			setParsedValue(sValue);
		}

		/// <summary>
		/// returns true if the XRI is absolute
		/// </summary>
		/// <returns></returns>
		public bool isAbsolute()
		{
			parse();
			return (moAuthorityPath != null);
		}

		/// <summary>
		/// returns returns true if the XRI is relative
		/// </summary>
		/// <returns></returns>
		public bool isRelative()
		{
			return !isAbsolute();
		}

		/// <summary>
		/// Parses the input stream into an Authority Path
		/// </summary>
		/// <param name="oParseStream">The input stream to scan from</param>
		/// <returns></returns>
		static AuthorityPath scanSchemeAuthority(ParseStream oParseStream)
		{
			if (oParseStream.empty())
			{
				return null;
			}

			ParseStream oAuthStream = oParseStream.begin();

			// The xri:// is optional
			if ((oParseStream.getData().Length >= XRI_SCHEME_LENGTH))
			{
				string sScheme = oAuthStream.getData().Substring(0, XRI_SCHEME_LENGTH);
				if ((sScheme != null) && sScheme.Equals(XRI_SCHEME, StringComparison.InvariantCultureIgnoreCase))
				{
					oAuthStream.consume(XRI_SCHEME_LENGTH);
				}
			}

			// see if we get an authority
			AuthorityPath oAuthorityPath = AuthorityPath.scanAuthority(oAuthStream);

			// if we found one, consume the entire auth stream, including 
			// the scheme
			if (oAuthorityPath != null)
			{
				oParseStream.end(oAuthStream);
			}

			return oAuthorityPath;
		}


		public string ToString(bool wantScheme, bool caseFoldAuthority)
		{
			StringBuilder sb = new StringBuilder();

			if (moAuthorityPath != null)
			{
				if (wantScheme)
				{
					sb.Append(XRI_SCHEME);
				}

				string a = moAuthorityPath.ToString();
				if (caseFoldAuthority)
					a = a.ToLower();

				sb.Append(a);
			}

			if (moAbsolutePath != null)
				sb.Append(moAbsolutePath.ToString());

			if (query != null)
			{
				sb.Append("?");
				sb.Append(query.ToString());
			}

			if (fragment != null)
			{
				sb.Append("#");
				sb.Append(fragment.ToString());
			}

			return sb.ToString();
		}

		public bool Equals(XRI x)
		{
			return ToString(false, true).Equals(x.ToString(false, true));
		}

		public string toIRINormalForm()
		{
			string iri = "";

			// add the authority path if it is there
			if (moAuthorityPath != null)
			{
				iri = XRI_SCHEME + moAuthorityPath.toIRINormalForm();
			}

			// add the local path and relative path as necessary
			if (moAbsolutePath != null)
			{
				iri += moAbsolutePath.toIRINormalForm();
			}

			if (query != null)
				iri += "?" + query.toIRINormalForm();

			if (fragment != null)
				iri += "#" + fragment.toIRINormalForm();

			return iri;
		}

		/// <summary>
		/// Serializes the XRI into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the XRI</returns>
		public string toURINormalForm()
		{
			string iri = toIRINormalForm();
			return IRIUtils.IRItoURI(iri);
		}

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oStream)
		{
			moAuthorityPath = scanSchemeAuthority(oStream);
			if (moAuthorityPath == null)
			{
				return false;
			}

			XRIAbsolutePath oPath = new XRIAbsolutePath();
			if (oPath.scan(oStream))
			{
				moAbsolutePath = oPath;
			}

			XRIQuery query = new XRIQuery();
			if (query.scan(oStream))
			{
				this.query = query;
			}

			XRIFragment fragment = new XRIFragment();
			if (fragment.scan(oStream))
			{
				this.fragment = fragment;
			}

			return true;
		}
		
		public AuthorityPath AuthorityPath
		{
			get
			{
				return moAuthorityPath;
			}
		}


		public XRIPath XRIPath
		{
			get
			{
				return moAbsolutePath;
			}
		}

		/// <summary>
		/// Returns the query.
		/// </summary>
		public XRIQuery Query
		{
			get
			{
				return query;
			}
		}

		/// <summary>
		/// Returns the fragment.
		/// </summary>
		public XRIFragment Fragment
		{
			get
			{
				return fragment;
			}
		}
	}
}