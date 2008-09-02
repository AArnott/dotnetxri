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

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This class provides a strong typing for a XRef Authority.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRef Authority.
	/// </summary>
	public class XRefAuthority : XRIAuthority
	{
		XRef moXRoot;

		/// <summary>
		/// Constructs a cross-reference authority from a string
		/// </summary>
		/// <param name="sPath"></param>
		public XRefAuthority(string sPath)
			: base(sPath)
		{
			parse();
		}

		internal XRefAuthority()
		{ }

		bool doScan(ParseStream oStream)
		{
			if (oStream.empty())
			{
				return false;
			}

			ParseStream oTempStream = oStream.begin();

			// make sure we have a valid XRI Value
			XRef oXRef = new XRef();
			if (!oXRef.scan(oTempStream))
			{
				return false;
			}

			// at this point, we know we have enough for a valid xref
			oStream.end(oTempStream);
			moXRoot = oXRef;

			// the cross-reference MAY be followed by an XRI Segment
			// where the star cannot be assumed
			XRISegment oSegment = new XRISegment(false, true, true);
			if (oSegment.scan(oStream))
			{
				moSegment = oSegment;
			}

			return true;
		}

		/// <summary>
		/// The Cross Reference Root Authority
		/// </summary>
		public XRef XRoot
		{
			get
			{
				parse();
				return moXRoot;
			}
		}

		/// <summary>
		/// Serializes the XRIAuthority into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the XRIAuthority</returns>
		public string toIRINormalForm()
		{
			string sValue = XRoot.toIRINormalForm();
			if (moSegment != null)
			{
				sValue += moSegment.toIRINormalForm();
			}

			return sValue;
		}

		/// <summary>
		/// Serializes the XRefAuthority into URI normal from
		/// </summary>
		/// <returns>The URI normal form of the XRefAuthority</returns>
		public string toURINormalForm()
		{
			return IRIUtils.IRItoURI(toIRINormalForm());
		}

		/// <summary>
		/// The Root XRI Authority
		/// </summary>
		/// <returns></returns>
		public string RootAuthority
		{
			get
			{
				return XRoot.ToString();
			}
		}

		/// <summary>
		/// The parent XRIAuthority for this obj.  Equivalent to all but
		/// the last SubSegment.
		/// </summary>
		public XRIAuthority Parent
		{
			get
			{
				parse();

				// return null if there is no XRISegment
				if (this.moSegment == null)
				{
					return null;
				}

				// otherwise, we are good to go
				XRefAuthority oParent = new XRefAuthority();
				oParent.moXRoot = this.moXRoot;
				oParent.moSegment = this.moSegment.getParent();
				oParent.msValue = moXRoot.ToString() + oParent.moSegment.ToString();
				oParent.mbParsed = true;
				oParent.mbParseResult = this.mbParseResult;

				return oParent;

			}
		}
	}
}