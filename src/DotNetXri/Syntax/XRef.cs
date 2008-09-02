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

namespace DotNetXri.Syntax
{
	// TODO: support URI in xref
	/// <summary>
	/// This class provides a strong typing for a XRI cross reference.  Any
	/// obj of this class that appears outside of the package is a valid
	/// cross reference.
	/// </summary>
	public class XRef : Parsable
	{
		XRIReference moXRIRef;
		string msIRI;

		/// <summary>
		/// Constructs a cross-reference from a string
		/// </summary>
		/// <param name="sPath"></param>
		public XRef(string sPath)
			: base(sPath)
		{
			parse();
		}

		internal XRef()
		{ }

		bool doScan(ParseStream oStream)
		{
			if (oStream.empty())
			{
				return false;
			}

			if (oStream.getData()[0] != '(')
			{
				return false;
			}

			ParseStream oTempStream = oStream.begin();
			oTempStream.consume(1);

			String sIRI = null;
			// make sure we have a valid XRI reference
			XRIReference oRef = scanXRIReference(oTempStream);
			if (oRef == null || oTempStream.empty() || (oTempStream.getData()[0] != ')'))
			{
				// if we got a reference, but the resulting temp stream is empty or does not begin with ')' 
				// it got parsed wrongly (happens if the XRef is an IRI). Retry parsing with an IRI
				if (oRef != null)
				{
					oTempStream = oStream.begin();
					oTempStream.consume(1);
				}
				// if there is no XRI Reference, see if it is an IRI
				sIRI = scanIRI(oTempStream);
				if (sIRI == null)
				{
					return false;
				}
			}

			// make sure we have the trailing ')'
			if (oTempStream.empty() || (oTempStream.getData()[0] != ')'))
			{
				return false;
			}

			// at this point, complete consumption and return true
			oTempStream.consume(1);
			oStream.end(oTempStream);
			moXRIRef = oRef;
			msIRI = sIRI;

			return true;
		}

		/// <summary>
		/// Returns a non-null XRIReference if an XRIReference is consumed from the 
		/// stream
		/// </summary>
		/// <param name="oStream"></param>
		/// <returns></returns>
		static XRIReference scanXRIReference(ParseStream oStream)
		{
			// make sure we have a valid XRI Value
			XRI oXRI = new XRI();
			if (oXRI.scan(oStream))
			{
				return oXRI;
			}

			// try parsing it as a relative XRI
			RelativeXRI oRelXRI = new RelativeXRI();
			if (oRelXRI.scan(oStream))
			{
				return oRelXRI;
			}

			return null;
		}

		/// <summary>
		/// Returns a non-null String if an IRI is consumed from the stream
		/// </summary>
		/// <param name="oStream"></param>
		/// <returns></returns>
		static string scanIRI(ParseStream oStream)
		{
			int n = scanIRIChars(oStream.getData());
			string data = oStream.getData().Substring(0, n);

			try
			{
				// try parsing to check validity, Java's URI parser may not be IRI compliant so
				// this is a TODO
				Uri u = new Uri(data);
				if (!u.IsAbsoluteUri)
				{
					return null;
				}
			}
			catch (UriFormatException)
			{
				return null;
			}

			oStream.consume(n);
			return data;
		}

		static int scanIRIChars(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];

				// assume that ')' has been escaped out
				if (c == ')')
					return i;
			}
			return s.Length;
		}

		public XRIReference getXRIReference()
		{
			parse();
			return moXRIRef;
		}

		public string getIRI()
		{
			parse();
			return msIRI;
		}

		/// <summary>
		/// Serializes XRef into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the XRef</returns>
		public string toIRINormalForm()
		{
			return IRIUtils.XRItoIRI(ToString(), true);
		}

		public string toURINormalForm()
		{
			return IRIUtils.IRItoURI(toIRINormalForm());
		}
	}
}