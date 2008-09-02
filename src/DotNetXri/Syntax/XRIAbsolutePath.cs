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
	/// This class provides a strong typing for a XRI Absolute Path.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRI Absolute Path.
	/// </summary>
	public class XRIAbsolutePath : XRIPath
	{
		/// <summary>
		/// Constructs LocalPath from a String
		/// </summary>
		/// <param name="sPath"></param>
		public XRIAbsolutePath(string sPath)
			: base(sPath)
		{
			mbAllowColon = true;
			parse();
		}

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		internal XRIAbsolutePath()
		{
			mbAllowColon = true;
		}

		internal XRIAbsolutePath(XRINoSchemePath oPath)
		{
			moSegments = oPath.moSegments;
			mbAllowColon = true;
		}

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oStream)
		{
			// make sure we start with a slash
			if (oStream.empty() || (oStream.getData()[0] != '/'))
			{
				return false;
			}

			// consume the slash
			oStream.consume(1);

			// now scan the XRI segments as we are supposed to
			base.scanXRISegments(oStream);

			// return true no matter what, we got the slash
			return true;
		}

		/// <summary>
		/// Serializes Local Path into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the Local Path</returns>
		public string toIRINormalForm()
		{
			return "/" + base.toIRINormalForm();
		}

		/// <summary>
		/// Tests to see if this absolute path is a subsegment-wise prefix of
		/// the given <code>path</code>.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>true if <code>path</code> is prefixed with or exactly the same as this XRIAbsolutePath obj.</returns>
		public bool isPrefixOf(XRIAbsolutePath path)
		{
			int n = this.getNumSegments();
			// first, if we have more segments than the given path, return false
			if (n > path.getNumSegments())
				return false;

			// compare segment by segment
			for (int i = 0; i < n; i++)
			{
				XRISegment seg = getSegmentAt(i);
				if (i == n - 1)
				{
					// final segment, can be a suffix
					if (!seg.isPrefixOf(path.getSegmentAt(i)))
						return false;
				}
				else if (!seg.EqualsIgnoreCase(path.getSegmentAt(i)))
					// not final segment, must be equal
					return false;
			}
			return true;
		}
	}
}