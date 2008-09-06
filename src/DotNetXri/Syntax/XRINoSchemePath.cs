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
	/// This class provides a strong typing for a XRINoSchemePath.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRINoSchemePath.
	/// </summary>
	public class XRINoSchemePath : XRIPath
	{
		/// <summary>
		/// Constructs RelativePath from a String
		/// </summary>
		/// <param name="sXRI"></param>
		public XRINoSchemePath(string sXRI)
			: base(sXRI)
		{
			mbAllowColon = false;
			parse();
		}

		internal XRINoSchemePath()
		{
			mbAllowColon = false;
		}

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oStream)
		{
			// NOTE: A RelativePath can be empty
			if (oStream.empty())
			{
				return true;
			}

			// doesn't matter if this works or not, will consume what it needs to
			scanXRISegments(oStream);

			return true;
		}
	}
}