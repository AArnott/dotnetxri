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

namespace DotNetXri.Syntax.Xri3.Impl
{
	/// <summary>
	/// Utility class that provides XRI-IRI-URI transformations
	/// </summary>
	public class IRIUtils
	{
		private static readonly int[] HEXCHARS = {
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // 00-0F
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // 10-1F
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // 20-2F
			 0,  1,  2,  3,  4,  5,  6,  7,  8,  9, -1, -1, -1, -1, -1, -1, // 30-3F
			-1, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1  // 40-4F
		};

		private const string RFC3986_GEN_DELIMS = ":/?#[]@";
		private const string RFC3986_SUB_DELIMS = "!$&'()*+,;=";
		private const string RFC3986_RESERVED = RFC3986_GEN_DELIMS + RFC3986_SUB_DELIMS;
		private const string ALPHA_LOWER = "abcdefghijklmnopqrstuvwxyz";
		private static readonly string ALPHA = ALPHA_LOWER + ALPHA_LOWER.ToUpper();

		// utf8 byte types
		private const int end = 0;
		private const int ill = 1;
		private const int asc = 2;
		private const int trl = 3;
		private const int by2 = 4;
		private const int e0 = 5;
		private const int by3 = 6;
		private const int ed = 7;
		private const int p13 = 8;
		private const int by4 = 9;
		private const int p16 = 10;

		private const int notal = 0;
		private const int unres = 1;
		private const int gendl = 2;
		private const int subdl = 3;
		private const int slash = 4;

		// private const String URICHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~/%:[]@?#!$&'()*,;=+";
		private static readonly int[] URICHARS = {
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 00-0F
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 10-1F
		0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 20-2F
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, // 30-3F
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 40-4F
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, // 50-5F
		0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 60-6F
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0  // 70-7F
	};

		private static readonly int[] UTF8lead = {
		/* 0x00 */ end, ill, ill, ill, ill, ill, ill, ill,
		/* 0x08 */ ill, asc, asc, ill, ill, asc, ill, ill,
		/* 0x10 */ ill, ill, ill, ill, ill, ill, ill, ill,
		/* 0x18 */ ill, ill, ill, ill, ill, ill, ill, ill,
		/* 0x20 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x28 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x30 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x38 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x40 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x48 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x50 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x58 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x60 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x68 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x70 */ asc, asc, asc, asc, asc, asc, asc, asc,
		/* 0x78 */ asc, asc, asc, asc, asc, asc, asc, ill,
		/* 0x80 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0x88 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0x90 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0x98 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0xA0 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0xA8 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0xB0 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0xB8 */ trl, trl, trl, trl, trl, trl, trl, trl,
		/* 0xC0 */ ill, ill, by2, by2, by2, by2, by2, by2,
		/* 0xC8 */ by2, by2, by2, by2, by2, by2, by2, by2,
		/* 0xD0 */ by2, by2, by2, by2, by2, by2, by2, by2,
		/* 0xD8 */ by2, by2, by2, by2, by2, by2, by2, by2,
		/* 0xE0 */  e0, by3, by3, by3, by3, by3, by3, by3,
		/* 0xE8 */ by3, by3, by3, by3, by3,  ed, by3, by3,
		/* 0xF0 */ p13, by4, by4, by4, p16, ill, ill, ill,
		/* 0xF8 */ ill, ill, ill, ill, ill, ill, ill, ill
	};

		public static string IRItoXRI(String iri)
		{
			StringBuilder sb = new StringBuilder();

			int cp;
			for (int i = 0; i < iri.Length; i += UTF16.getCharCount(cp))
			{
				cp = UTF16.charAt(iri, i);

				if (cp == '%')
				{
					// check percent encoded characters
					int percentEnc = decodeHex(iri, i);
					if (percentEnc == -1)
						throw new Exception("Invalid percent encoding encountered in IRI");

					// the percent-encoded sequence is valid and we know that i+1 and i+2 are ASCII
					switch (percentEnc)
					{
						case '/':
							sb.Append('/');
							break;
						case '?':
							sb.Append('?');
							break;
						case '#':
							sb.Append('#');
							break;
						case '%':
							sb.Append('%');
							break;
						default:
							sb.Append(iri.Substring(i, 3));
					}
					i += 2;
				}
				else
				{
					// just append the current codepoint to the buffer
					sb.Append(iri.Substring(i, UTF16.getCharCount(cp)));
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Transform the given URI to IRI according to the rules in RFC3987 section 3.2
		/// </summary>
		/// <param name="uri">this MUST be a valid URI string. Use the <code>java.net.URI</code> class
		/// to check before using this method.</param>
		/// <returns></returns>
		public static string URItoIRI(string uri)
		{
			int uriLength = uri.Length;
			byte[] bb = new byte[uriLength]; // Holds the transformed IRI encoded in UTF-8 (should never be greater than uriLength if URI is in ASCII)
			int bbIndex = 0;

			// Step 1 - Represent the URI as a sequence of octets in US-ASCII.
			//          Not checked because we assume that the URI is valid.
			//          It follows from that that the String is all ASCII. 

			byte[] u8buf = new byte[4]; // max number of bytes for UTF-8 encoded code point
			int i = 0; // uri index
			while (i < uriLength)
			{
				char cp = uri[i];

				if (cp > 0x7F)
					throw new Exception("Non ASCII character encountered in URI");

				if (cp != '%')
				{
					// non-percent encoded character, append to byte buffer
					bb[bbIndex++] = (byte)cp;
					i++;
					continue;
				}

				// check that the percent-encoded sequence is valid
				int cpVal;
				try
				{
					cpVal = decodeHex(uri, i);
				}
				catch (ArgumentException e)
				{
					throw new Exception("Invalid percent encoding encountered in URI: " + e.Message);
				}

				// TODO should we allow cpVal == 0? Not checking for now because we are not URI component aware
				if (cpVal <= 0x7F)
				{ // isascii(cpVal)
					// Step 2 - decode everything but '%' or reserved or illegal characters in US-ASCII
					int flg = URICHARS[cpVal];
					if (cpVal == '%' || flg == gendl || flg == subdl || flg == notal)
					{
						bb[bbIndex++] = (byte)uri[i++];
						bb[bbIndex++] = (byte)uri[i++];
						bb[bbIndex++] = (byte)uri[i++];
					}
					else
					{
						// append decoded
						bb[bbIndex++] = (byte)cpVal;
						i += 3;
					}
					continue;
				}

				// test to see if we have a valid UTF-8 sequence
				int n = countUTF8Sequence(uri, i, u8buf);
				if (n > 0)
				{
					// valid UTF-8 sequence of n-bytes long
					if (hasBiDiChar(new string(u8buf, 0, n, "UTF-8")))
					{
						// do not unescape bi-di character (must not be in IRI)
						bb[bbIndex++] = (byte)uri[i++];
						bb[bbIndex++] = (byte)uri[i++];
						bb[bbIndex++] = (byte)uri[i++];
					}
					else
					{
						for (int j = 0; j < n; j++)
						{
							bb[bbIndex++] = u8buf[j];
						}
						i += n * 3; // skip n * (%XX sequences)
					}
				}
				else
				{
					// not a valid UTF-8 sequence, do not unescape it
					bb[bbIndex++] = (byte)uri[i++];
					bb[bbIndex++] = (byte)uri[i++];
					bb[bbIndex++] = (byte)uri[i++];
				}
			}
			return new string(bb, 0, bbIndex, "UTF-8");
		}

		/// <summary>
		/// Transforms the given IRI to URI.
		/// </summary>
		/// <param name="iri"></param>
		/// <returns></returns>
		public static string IRItoURI(string iri)
		{
			int iriLen = iri.Length;
			StringBuilder sb = new StringBuilder(iriLen * 2);

			int cp;
			for (int i = 0; i < iriLen; i += UTF16.getCharCount(cp))
			{
				cp = UTF16.charAt(iri, i);
				if (isUCSCharOrIPrivate(cp))
					sb.Append(toUTF8PercentEncoded(cp));
				else
					UTF16.Append(sb, cp);
			}
			return sb.ToString();
		}

		protected static String toUTF8PercentEncoded(int cp)
		{
			string s = null;
			try
			{
				s = URLEncoder.encode(UCharacter.toString(cp), "UTF-8");
			}
			catch (UnsupportedEncodingException e)
			{
			}
			return s;
		}

		protected static bool isUCSCharOrIPrivate(int cp)
		{
			return (isUCSChar(cp) || isIPrivate(cp));
		}

		protected static bool isUCSChar(int cp)
		{
			if ((cp >= 0xA0 && cp <= 0xD7FF) ||
					(cp >= 0xF900 && cp <= 0xFDCF) ||
					(cp >= 0xFDF0 && cp <= 0xFFEF) ||
					(cp >= 0x10000 && cp <= 0x1FFFD) ||
					(cp >= 0x20000 && cp <= 0x2FFFD) ||
					(cp >= 0x30000 && cp <= 0x3FFFD) ||
					(cp >= 0x40000 && cp <= 0x4FFFD) ||
					(cp >= 0x50000 && cp <= 0x5FFFD) ||
					(cp >= 0x60000 && cp <= 0x6FFFD) ||
					(cp >= 0x70000 && cp <= 0x7FFFD) ||
					(cp >= 0x80000 && cp <= 0x8FFFD) ||
					(cp >= 0x90000 && cp <= 0x9FFFD) ||
					(cp >= 0xA0000 && cp <= 0xAFFFD) ||
					(cp >= 0xB0000 && cp <= 0xBFFFD) ||
					(cp >= 0xC0000 && cp <= 0xCFFFD) ||
					(cp >= 0xD0000 && cp <= 0xDFFFD) ||
					(cp >= 0xE0000 && cp <= 0xEFFFD))
				return true;
			return false;
		}

		protected static bool isIPrivate(int cp)
		{
			if ((cp >= 0xE000 && cp <= 0xF8FF) ||
					(cp >= 0xF0000 && cp <= 0xFFFFD) ||
					(cp >= 0x100000 && cp <= 0x10FFFD))
				return true;
			return false;
		}

		/// <summary>
		/// Transforms the given XRI part to IRI-normal form. This method does not parse the given String;
		/// it simply converts all '%' to '%25', and if <code>inXref</code> is true, also percent encodes
		/// '#', '?' and '/'.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="inXref"></param>
		/// <returns></returns>
		public static string XRItoIRI(string s, bool inXref)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c == '%')
					sb.Append("%25");
				else if (inXref)
				{
					if (c == '#')
						sb.Append("%23");
					else if (c == '?')
						sb.Append("%3F");
					else if (c == '/')
						sb.Append("%2F");
					else
						sb.Append(c);
				}
				else
					sb.Append(c);
			}
			return sb.ToString();
		}

		private static int countUTF8Sequence(string inValue, int inIndex, byte[] u8buf)
		{
			int[] c = new int[4];

			if (!isPercentEncoded(inValue, inIndex))
				return 0;

			c[0] = decodeHex(inValue, inIndex);
			switch (UTF8lead[c[0]])
			{
				case end: // truncated sequence
				case ill: // illegal in UTF-8
				case trl: // trailer - illegal
				case asc: // this function is not meant to get ASCII
					return 0;

				case by2:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					if (UTF8lead[c[1]] != trl)
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					return 2;

				case e0:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex) || !isPercentEncoded(inValue, inIndex + 3))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					inIndex += 3;
					c[2] = decodeHex(inValue, inIndex);

					if (((c[1] & 0xE0) != 0xA0) || (UTF8lead[c[2]] != trl))
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					u8buf[2] = (byte)c[2];
					return 3;

				case by3:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex) || !isPercentEncoded(inValue, inIndex + 3))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					inIndex += 3;
					c[2] = decodeHex(inValue, inIndex);

					if ((UTF8lead[c[1]] != trl) || (UTF8lead[c[2]] != trl))
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					u8buf[2] = (byte)c[2];
					return 3;

				case ed:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex) || !isPercentEncoded(inValue, inIndex + 3))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					inIndex += 3;
					c[2] = decodeHex(inValue, inIndex);

					if (((c[1] & 0xE0) != 0x80) || (UTF8lead[c[2]] != trl))
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					u8buf[2] = (byte)c[2];
					return 3;

				case p13:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[2] = decodeHex(inValue, inIndex);
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[3] = decodeHex(inValue, inIndex);

					if ((c[1] < 0x90 || 0xBF < c[1])
							|| (UTF8lead[c[2]] != trl)
							|| (UTF8lead[c[3]] != trl))
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					u8buf[2] = (byte)c[2];
					u8buf[3] = (byte)c[3];
					return 4;

				case by4:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[2] = decodeHex(inValue, inIndex);
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[3] = decodeHex(inValue, inIndex);

					if ((UTF8lead[c[1]] != trl)
							|| (UTF8lead[c[2]] != trl)
							|| (UTF8lead[c[3]] != trl))
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					u8buf[2] = (byte)c[2];
					u8buf[3] = (byte)c[3];
					return 4;

				case p16:
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[1] = decodeHex(inValue, inIndex);
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[2] = decodeHex(inValue, inIndex);
					inIndex += 3;
					if (!isPercentEncoded(inValue, inIndex))
						return 0;
					c[3] = decodeHex(inValue, inIndex);

					if (((c[1] & 0xF0) != 0x80)
							|| (UTF8lead[c[2]] != trl)
							|| (UTF8lead[c[3]] != trl))
						return 0;
					u8buf[0] = (byte)c[0];
					u8buf[1] = (byte)c[1];
					u8buf[2] = (byte)c[2];
					u8buf[3] = (byte)c[3];
					return 4;

				default:
					// should never reach here
					return 0;
			}
		}

		/// <summary>
		/// Attempt to read a percent encoded sequence from the given string <code>s</code> at <code>index</code> position.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="index"></param>
		/// <returns>The percent sequence String of length 3, or <code>null</code> if a valid sequence cannot be read.</returns>
		private static string getHex(string s, int index)
		{
			// make sure the string is long enough
			if (s.Length < index + 3)
				return null;

			if (s[index] != '%')
				return null;

			int c1 = (int)char.ToUpper(s[index + 1]);
			int c2 = (int)char.ToUpper(s[index + 2]);

			if (c1 >= HEXCHARS.Length || HEXCHARS[c1] == -1 ||
					c2 >= HEXCHARS.Length || HEXCHARS[c2] == -1)
				return null; // invalid hex chars

			return s.Substring(index, 3);
		}

		/// <summary>
		/// Attempts to decode a 3-character percent-encoded sequence to the numeric value.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="index"></param>
		/// <returns>the numeric value of the %XX sequence, or -1 if the sequence is invalid.</returns>
		public static int decodeHex(string s, int index)
		{
			// make sure the string is long enough
			if (s.Length < index + 3)
				throw new ArgumentException("Incomplete percent escape");

			if (s[index] != '%')
				return -1;

			int c1 = (int)char.ToUpper(s[index + 1]);
			int c2 = (int)char.ToUpper(s[index + 2]);
			int c1val, c2val;

			if (c1 >= HEXCHARS.Length || c2 >= HEXCHARS.Length)
				throw new ArgumentException("Illegal hex characters");

			c1val = HEXCHARS[c1];
			c2val = HEXCHARS[c2];

			if (c1val == -1 || c2val == -1)
				throw new ArgumentException("Illegal hex characters");

			return c1val * 16 + c2val;
		}

		private static bool isPercentEncoded(string s, int index)
		{
			// make sure the string is long enough
			if (s.Length < index + 3)
				return false;

			if (s[index] != '%')
				return false;

			int c1 = (int)char.ToUpper(s[index + 1]);
			int c2 = (int)char.ToUpper(s[index + 2]);

			if (c1 >= HEXCHARS.Length || c2 >= HEXCHARS.Length)
				return false; // invalid hex chars

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <returns>true if any of (LRM, RLM, LRE, RLE, LRO, RLO, or PDF) exists in <code>s</code></returns>
		private static bool hasBiDiChar(string s)
		{
			int cp;
			for (int i = 0; i < s.Length; i += UTF16.getCharCount(cp))
			{
				cp = UTF16.charAt(s, i);
				if (cp == 0x200E // LRM
						|| cp == 0x200F // RLM
						|| cp == 0x202A // LRE
						|| cp == 0x202B // RLE
						|| cp == 0x202D // LRO
						|| cp == 0x202E // RLO
						|| cp == 0x202C // PDF
						)
					return true;
			}
			return false;
		}
	}
}