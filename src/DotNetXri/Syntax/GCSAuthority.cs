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
	/// This class provides a strong typing for a GCS Authority.  Any
	/// obj of this class that appears outside of the package is a valid
	/// GCS Authority.
	/// </summary>
	public class GCSAuthority : XRIAuthority
	{
		private string msGCSRoot;

		/// <summary>
		/// Constructs GCSAuthority from a String
		/// </summary>
		/// <param name="sPath"></param>
		public GCSAuthority(string sPath)
			: base(sPath)
		{
			parse();
		}

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		public GCSAuthority()
		{ }

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oStream)
		{
			if (!scanGCSChar(oStream))
			{
				return false;
			}

			// read in a segment, but bail if it isn't persistent in the ! namespace
			XRISegment oSegment = new XRISegment(true, true, !msGCSRoot.Equals("!"));
			if (oSegment.scan(oStream))
			{
				moSegment = oSegment;
			}

			return true;
		}

		/// <summary>
		/// Parses the input stream into the GCS Character String
		/// </summary>
		/// <param name="oParseStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed</returns>
		private bool scanGCSChar(ParseStream oParseStream)
		{
			if (oParseStream.empty())
			{
				return false;
			}

			switch (oParseStream.getData()[0])
			{
				case '+':
				case '=':
				case '@':
				case '$':
				case '!':
					{
						// this way provides a clean copy, whereas substring does not                
						msGCSRoot = char.ToString(oParseStream.getData()[0]);
						oParseStream.consume(1);
						return true;
					}
			}

			return false;
		}

		/// <summary>
		/// The GCS Root Authority
		/// </summary>
		public char GCSRootAsChar
		{
			get
			{
				parse();
				return msGCSRoot[0];
			}
		}

		/// <summary>
		/// The GCS Root Authority
		/// </summary>
		public string GCSRoot
		{
			get
			{
				parse();
				return msGCSRoot;
			}
		}

		/// <summary>
		/// Serializes the XRIAuthority into IRI-normal from
		/// </summary>
		/// <returns>The IRI normal form of the XRIAuthority</returns>
		public string toIRINormalForm()
		{
			string sValue = msGCSRoot;
			if (moSegment != null)
			{
				sValue += moSegment.toIRINormalForm();
			}
			return sValue;
		}

		/// <summary>
		/// Serializes the XRIAuthority into URI normal from
		/// </summary>
		/// <returns>The URI normal form of the XRIAuthority</returns>
		public string toURINormalForm()
		{
			return IRIUtils.IRItoURI(toIRINormalForm());
		}
		
		/// <summary>
		/// The Root XRI Authority
		/// </summary>
		public string RootAuthority
		{
			get
			{
				return GCSRoot;
			}
		}

		/// <summary>
		/// The parent XRIAuthority of this obj.  Equivalent to all but
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
				GCSAuthority oParent = new GCSAuthority();
				oParent.msGCSRoot = this.msGCSRoot;
				oParent.moSegment = this.moSegment.getParent();
				oParent.msValue = msGCSRoot + (oParent.moSegment != null ? oParent.moSegment.ToString() : "");
				oParent.mbParsed = true;
				oParent.mbParseResult = this.mbParseResult;

				return oParent;
			}
		}
	}
}