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
	/// <summary>
	/// This class provides a strong typing for a XRI subsegment.  Any
	/// obj of this class that appears outside of the package is a valid
	/// subsegment.
	/// </summary>
	public class XRISubSegment : Parsable
	{
		bool mbPersistant = false;
		XRef moXRef = null;
		bool mbAllowColon = false;
		bool mbAllowImpliedDelimiter = false;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bAllowImpliedDelimiter"></param>
		/// <param name="bAllowColon"></param>
		/// <remarks>
		/// Protected Constructor used by package only
		/// </remarks>
		internal XRISubSegment(bool bAllowImpliedDelimiter, bool bAllowColon)
		{
			mbAllowImpliedDelimiter = bAllowImpliedDelimiter;
			mbAllowColon = bAllowColon;
		}

		/// <summary>
		/// Constructs SubSegment from String
		/// </summary>
		/// <param name="sXRI"></param>
		public XRISubSegment(string sXRI)
			: base(sXRI)
		{
			parse();
		}

		/// <summary>
		/// Constructs SubSegment from String
		/// </summary>
		/// <param name="sXRI"></param>
		/// <param name="bAllowColon"></param>
		public XRISubSegment(string sXRI, bool bAllowColon)
			: base(sXRI)
		{
			mbAllowColon = bAllowColon;
			parse();
		}

		/// <summary>
		/// Indicates whether Subsegment is persistent or not
		/// </summary>
		/// <returns>Indicates whether Subsegment is persistent or not</returns>
		public bool isPersistant()
		{
			return mbPersistant;
		}

		/// <summary>
		/// Returns the cross-reference of this obj
		/// </summary>
		/// <returns>XRef the cross-reference of this obj</returns>
		public XRef getXRef()
		{
			parse();
			return moXRef;
		}

		/// <summary>
		/// String representation of the obj.
		/// </summary>
		/// <returns>the String form of the SubSegment with its persistent indicator.</returns>
		public override string ToString()
		{
			return ToString(true);
		}

		public string ToString(bool wantOptionalDelim)
		{
			parse();

			// add the dot if necessary
			string sRetval = base.ToString();
			if ((sRetval.Length > 0) &&
				(sRetval[0] != XRI.RDELIM) &&
				(sRetval[0] != XRI.PDELIM) &&
				wantOptionalDelim)
			{
				sRetval = XRI.RDELIM_S + sRetval;
			}

			return sRetval;
		}

		public bool Equals(XRISubSegment subseg)
		{
			return ToString(true).Equals(subseg.ToString(true));
		}

		public bool EqualsIgnoreCase(XRISubSegment subseg)
		{
			return ToString(true).Equals(subseg.ToString(true), StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oStream)
		{
			if (oStream.getData()[0] == XRI.PDELIM)
			{
				this.mbPersistant = true;
				oStream.consume(1);
			}
			else if (oStream.getData()[0] == XRI.RDELIM)
			{
				oStream.consume(1);
			}
			else if (!mbAllowImpliedDelimiter)
			{
				return false;
			}

			// if there is a cross-reference, it has priority in scanning
			XRef oXRef = new XRef();
			if (oXRef.scan(oStream))
			{
				moXRef = oXRef;
				return true;
			}

			// read the characters, it is ok if they are empty
			int n = scanPChars(oStream.getData());
			oStream.consume(n);

			return true;
		}

		/// <summary>
		/// Reads xri-pchars from the String
		/// </summary>
		/// <param name="s">The String to scan from</param>
		/// <returns>The number of characters read in</returns>
		private int scanPChars(string s)
		{
			int c;
			for (int i = 0; i < s.Length; i += UTF16.getCharCount(c))
			{
				c = UTF16.charAt(s, i);

				if (Characters.isPChar(c))
				{
					// pchar includes colon, but our configuration might not allow it
					if (c != ':')
						continue;

					// it's a colon, only let through if our configuration allows it
					if (mbAllowColon)
						continue;
				}

				//
				// escaped
				//
				if (Characters.isEscaped(c, s, i))
				{
					i += 2;
					continue;
				}

				return i;
			}

			return s.Length;
		}

		/// <summary>
		/// Serializes SubSegment into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the SubSegment</returns>
		public string toIRINormalForm()
		{
			return toIRINormalForm(true);
		}

		public string toIRINormalForm(bool wantOptionalDelim)
		{
			if (moXRef != null)
			{
				string value;
				if (isPersistant())
					value = XRI.PDELIM_S;
				else
					value = wantOptionalDelim ? XRI.RDELIM_S : "";
				return value + moXRef.toIRINormalForm();
			}

			return IRIUtils.XRItoIRI(ToString(wantOptionalDelim), false);
		}

		/// <summary>
		/// Serializes SubSegment into URI normal from
		/// </summary>
		/// <returns>The URI normal form of the SubSegment</returns>
		public string toURINormalForm()
		{
			return toURINormalForm(true);
		}

		public string toURINormalForm(bool wantOptionalDelim)
		{
			return IRIUtils.IRItoURI(toIRINormalForm(wantOptionalDelim));
		}
	}
}