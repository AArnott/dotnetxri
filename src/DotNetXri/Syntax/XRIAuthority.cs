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
	/// This class provides a base class for all types of XRIAuthority elements.
	/// </summary>
	public abstract class XRIAuthority : AuthorityPath
	{
		protected XRISegment moSegment;

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		protected XRIAuthority()
		{ }

		/// <summary>
		/// Constructs XRIAuthority from a String
		/// </summary>
		/// <param name="sPath"></param>
		protected XRIAuthority(string sPath)
			: base(sPath)
		{
			parse();
		}

		/// <summary>
		/// Returns the XRI Segment for this Authority Path
		/// </summary>
		/// <returns>The XRI Segment</returns>
		public XRISegment XRISegment
		{
			get
			{
				parse();
				return moSegment;
			}
		}

		/// <summary>
		/// The number of subsegments in the XRI segment
		/// </summary>
		public int NumSubSegments
		{
			get
			{
				parse();
				if (moSegment != null)
				{
					return moSegment.getNumSubSegments();
				}

				return 0;
			}
		}
	 
		/// <summary>
		/// Returns the subsegment at the given index
		/// </summary>
		/// <param name="nIndex">The index of the subsegment to return</param>
		/// <returns>The subsegment at the specified location</returns>
		public XRISubSegment getSubSegmentAt(int nIndex)
		{
			parse();
			if (moSegment != null)
			{
				return moSegment.getSubSegmentAt(nIndex);
			}

			return null;
		}

		/// <summary>
		/// Returns the last subsegment in the XRI segment
		/// </summary>
		/// <returns>The last subsegment</returns>
		public XRISubSegment getLastSubSegment()
		{
			parse();

			if (moSegment != null)
			{
				int nSize = moSegment.getNumSubSegments();
				if (nSize >= 1)
				{
					return moSegment.getSubSegmentAt(nSize - 1);
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the root XRI Authority as a String
		/// </summary>
		/// <returns>The Root XRI Authority</returns>
		public abstract string RootAuthority
		{
			get;
		}

		/// <summary>
		/// Returns the parent XRIAuthority for this obj.  Equivalent to all but
		/// the last SubSegment.
		/// </summary>
		/// <returns>The parent XRIAuthority of this obj</returns>
		public abstract XRIAuthority Parent
		{
			get;
		}

		/// <summary>
		/// Returns the parent XRefAuthority for this obj.  Equivalent to all but
		/// the last SubSegment.
		/// </summary>
		public XRI ParentAsXRI
		{
			get
			{
				AuthorityPath oParent = Parent;
				return (oParent == null) ? null : new XRI(oParent);
			}
		}
	}
}