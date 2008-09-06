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
	/// This class provides a strong typing for a XRI Query.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRI Query.
	/// </summary>
	public class XRIQuery : Parsable
	{
		protected string query = null;

		internal XRIQuery()
		{ }

		/// <summary>
		/// Constructs a XRIQuery from a string. Not implemented.
		/// </summary>
		/// <param name="sQuery"></param>
		public XRIQuery(string sQuery)
			: base(sQuery)
		{
			parse();
		}

		// this is called by Parsable.scan after calling our doScan with the consumed string.
		// we override this method to remove the leading '?'
		void setParsedValue(string sValue)
		{
			if (sValue != null)
			{
				if (sValue.Length > 0 && sValue[0] == '?')
					msValue = sValue.Substring(1);
				else
					msValue = sValue;
			}
			else
			{
				msValue = "";
			}

			mbParsed = true;
			mbParseResult = true;
		}

		bool doScan(ParseStream oStream)
		{
			if (oStream.empty() || oStream.getData()[0] != '?')
				return false;

			oStream.consume(1);

			// read the characters, it is ok if they are empty
			int n = scanIQueryChars(oStream.getData());
			oStream.consume(n);

			return true;
		}

		private int scanIQueryChars(string s)
		{
			int c;
			for (int i = 0; i < s.Length; i += UTF16.getCharCount(c))
			{
				c = UTF16.charAt(s, i);

				// pchar's
				if (Characters.isIPChar(c))
					continue;

				//
				// escaped
				//
				if (Characters.isEscaped(c, s, i))
				{
					i += 2;
					continue;
				}

				//
				// private
				//
				if (UCharacter.getType(c) == UCharacter.PRIVATE_USE)
					continue;

				//
				// "/" or "?"
				if (c == '/' || c == '?')
					continue;
				return i;
			}

			return s.Length;

		}

		public string toIRINormalForm()
		{
			return IRIUtils.XRItoIRI(ToString(), false);
		}
	}
}