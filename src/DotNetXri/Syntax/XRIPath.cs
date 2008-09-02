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

using System.Collections.Generic;
using System.Text;

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This base class provides a strong typing for a XRI Path.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRI Path.
	/// </summary>
	public abstract class XRIPath : Parsable
	{
		public IList<XRISegment> moSegments = new List<XRISegment>();
		protected bool mbAllowColon = true;

		protected XRIPath()
		{ }

		protected XRIPath(string sVal)
			: base(sVal)
		{ }

		/// <summary>
		/// 
		/// </summary>
		/// <returns>The number of XRISegmentVals for this relative path</returns>
		public int getNumSegments()
		{
			parse();
			return (moSegments == null) ? 0 : moSegments.Count;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>Iterator for the XRISegmentVals for this relative path</returns>
		public IEnumerator<XRISegment> getSegmentIterator()
		{
			parse();
			return (moSegments == null) ? null : moSegments.GetEnumerator();
		}

		/// <summary>
		/// Returns the XRISegmentVal at the given index
		/// </summary>
		/// <param name="nIndex">The index of the XRISegmentVal to return</param>
		/// <returns>The XRISegmentVal at the specified index</returns>
		public XRISegment getSegmentAt(int nIndex)
		{
			parse();
			if ((moSegments == null) || (nIndex >= moSegments.Count))
			{
				return null;
			}

			return (XRISegment)moSegments[nIndex];
		}

		/// <summary>
		/// Parses the input stream into XRISegmentVals
		/// </summary>
		/// <param name="oPathStream">The input stream to scan from</param>
		protected void scanXRISegments(ParseStream oPathStream)
		{
			// sets whether colons are allowed
			bool bAllowColon = mbAllowColon;

			// loop through the XRI segments as long as we are consuming something
			bool bConsumed = true;
			while (!oPathStream.empty() && bConsumed)
			{
				bConsumed = false;
				ParseStream oStream = oPathStream.begin();
				bool bStartsWithSlash = (oStream.getData()[0] == '/');

				// if this is the first segment, it must not start with slash
				if ((bStartsWithSlash) && (moSegments.Count == 0))
				{
					break;
				}

				// if this is not the first segment, we expect a slash
				if ((!bStartsWithSlash) && (moSegments.Count > 0))
				{
					break;
				}

				// consume the slash if necessary
				if (bStartsWithSlash)
				{
					bConsumed = true;
					oStream.consume(1);
				}

				// if there is actually a segment, add it to the list
				XRISegment oSegment = new XRISegment(true, bAllowColon, true);
				if (oSegment.scan(oStream))
				{
					bConsumed = true;
					moSegments.Add(oSegment);
				}

				// consume whatever we used (even if the segment was empty)
				oPathStream.end(oStream);

				// after the first segment, colons are allowed
				bAllowColon = true;
			}
		}

		/// <summary>
		/// Serializes Relative Path into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the Relative Path</returns>
		public string toIRINormalForm()
		{
			StringBuilder sValue = new StringBuilder();

			IEnumerator<XRISegment> oIt = moSegments.GetEnumerator();
			if (oIt.MoveNext())
			{
				sValue.Append(oIt.Current.toIRINormalForm());
			}

			while (oIt.MoveNext())
			{
				sValue.Append("/");
				sValue.Append(oIt.Current.toIRINormalForm());
			}

			return sValue.ToString();
		}

		/// <summary>
		/// Serializes Relative Path into URI normal from
		/// </summary>
		/// <returns>The URI normal form of the Relative Path</returns>
		public string toURINormalForm()
		{
			return IRIUtils.IRItoURI(toIRINormalForm());
		}
	}
}