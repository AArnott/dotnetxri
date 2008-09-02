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
	/// This class provides a strong typing for a Relative XRI.  Any
	/// obj of this class that appears outside of the package is a valid
	/// Relative XRI.
	/// </summary>
	public class RelativeXRI : Parsable, XRIReference
	{
		private XRIPath moXRIPath;

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		internal RelativeXRI()
		{ }

		/// <summary>
		/// Constructs XRI from String
		/// </summary>
		/// <param name="sXRI"></param>
		public RelativeXRI(string sXRI)
			: base(sXRI)
		{
			parse();
		}

		/// <summary>
		/// Constructs an XRI from the provided RelativePath
		/// </summary>
		/// <param name="oPath"></param>
		public RelativeXRI(XRIPath oPath)
		{
			moXRIPath = oPath;
			setParsedValue(oPath.ToString());
		}

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oStream)
		{
			moXRIPath = scanXRIPath(oStream);
			return true;
		}

		static XRIPath scanXRIPath(ParseStream oStream)
		{
			// check for a local path regardless of scanAuthority outcome
			XRIAbsolutePath oPath = new XRIAbsolutePath();
			if (oPath.scan(oStream))
			{
				return oPath;
			}
			else
			{
				XRINoSchemePath oRelativePath = new XRINoSchemePath();
				if (oRelativePath.scan(oStream))
				{
					return oRelativePath;
				}
			}

			return null;
		}

		public AuthorityPath AuthorityPath
		{
			get
			{
				return null;
			}
		}

		public XRIPath XRIPath
		{
			get
			{
				return moXRIPath;
			}
		}
	}
}