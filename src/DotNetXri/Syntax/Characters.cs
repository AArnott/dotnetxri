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
	/// This class provides utility methods for parsing XRIs
	/// </summary>
	public class Characters
	{
		public static readonly UnicodeSet UCSCHAR;
		public static readonly UnicodeSet IUNRESERVED;
		public static readonly UnicodeSet IPCHAR;
		public static readonly UnicodeSet XRI_PCHAR;

		public const string XRI_SUB_DELIMS = "&;,'";
		public const string SUB_DELIMS = "!$&'()*+,;=";

		static Characters()
		{
			UCSCHAR = new UnicodeSet();
			UCSCHAR.add(0xA0, 0xD7FF);
			UCSCHAR.add(0xF900, 0xFDCF);
			UCSCHAR.add(0xFDF0, 0xFFEF);
			UCSCHAR.add(0x1000, 0x1FFFD);
			UCSCHAR.add(0x2000, 0x2FFFD);
			UCSCHAR.add(0x3000, 0x3FFFD);
			UCSCHAR.add(0x4000, 0x4FFFD);
			UCSCHAR.add(0x5000, 0x5FFFD);
			UCSCHAR.add(0x6000, 0x6FFFD);
			UCSCHAR.add(0x7000, 0x7FFFD);
			UCSCHAR.add(0x8000, 0x8FFFD);
			UCSCHAR.add(0x9000, 0x9FFFD);
			UCSCHAR.add(0xA000, 0xAFFFD);
			UCSCHAR.add(0xB000, 0xBFFFD);
			UCSCHAR.add(0xC000, 0xCFFFD);
			UCSCHAR.add(0xD000, 0xDFFFD);
			UCSCHAR.add(0xE000, 0xEFFFD);

			IUNRESERVED = new UnicodeSet();
			IUNRESERVED.addAll("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~");
			IUNRESERVED.addAll(UCSCHAR);

			// ipchar = iunreserved / pct-encoded / sub-delims / ":" / "@"
			IPCHAR = new UnicodeSet();
			IPCHAR.addAll(IUNRESERVED);
			IPCHAR.add(0x25); // '%'
			IPCHAR.addAll(SUB_DELIMS); // sub-delims
			IPCHAR.add(0x3A); // ':'
			IPCHAR.add(0x40); // '@'

			// xri-pchar = iunreserved / pct-encoded / xri-sub-delims / ":"
			XRI_PCHAR = new UnicodeSet();
			XRI_PCHAR.addAll(IUNRESERVED);
			XRI_PCHAR.add(0x25); // '%'
			XRI_PCHAR.addAll(XRI_SUB_DELIMS);
			XRI_PCHAR.add(0x3A); // ':'
		}

		/// <summary>
		/// Determines if this character is a valid xri-pchar
		/// </summary>
		/// <param name="c">The char to examine</param>
		/// <returns></returns>
		public static bool isPChar(int c)
		{
			return XRI_PCHAR.contains(c);
		}

		/// <summary>
		/// Determines if this character is a valid ipchar (iunreserved/pct-encoded/sub-delims/":"/"@").
		/// </summary>
		/// <param name="c">The char to examine</param>
		/// <returns></returns>
		/// <remarks>
		/// This does not check for pct-encoded characters
		/// </remarks>
		public static bool isIPChar(int c)
		{
			return IPCHAR.contains(c);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="c">the character just scanned</param>
		/// <param name="s">the String containing char c</param>
		/// <param name="i">the index of char c in String s</param>
		/// <returns>true if c is escaped</returns>
		internal static bool isEscaped(int c, string s, int i)
		{
			if (c == '%')
			{
				if ((i + 2) < s.Length)
				{
					char hex1 = s[i + 1];
					char hex2 = s[i + 2];

					if (
						(char.digit(hex1, 16) != -1) &&
						(char.digit(hex2, 16) != -1))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}