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
using System.Collections.Generic;
using System.Text;

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This class provides a strong typing for a XRI Segment.  Any
	/// obj of this class that appears outside of the package is a valid
	/// XRI Segment with at least one subsegment.
	/// </summary>
	public class XRISegment : Parsable
	{
		private IList<XRISubSegment> moSubSegments = null;
		private bool mbAllowImpliedDelimiter = true;
		private bool mbAllowColon = true;
		private bool mbAllowReassignable = true;

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		/// <param name="bAllowImpliedDelimiter"></param>
		/// <param name="bAllowColon"></param>
		/// <param name="bAllowReassignable"></param>
		internal XRISegment(bool bAllowImpliedDelimiter, bool bAllowColon, bool bAllowReassignable)
		{
			mbAllowImpliedDelimiter = bAllowImpliedDelimiter;
			mbAllowColon = bAllowColon;
			mbAllowReassignable = bAllowReassignable;
		}

		/// <summary>
		/// Constructs XRISegment from String
		/// </summary>
		/// <param name="sVal"></param>
		public XRISegment(string sVal)
			: base(sVal)
		{
			parse();
		}

		/// <summary>
		/// Constructs XRISegment from String
		/// </summary>
		/// <param name="sVal"></param>
		/// <param name="bAllowImpliedDelimiter"></param>
		/// <param name="bAllowColon"></param>
		public XRISegment(string sVal, bool bAllowImpliedDelimiter, bool bAllowColon)
			: base(sVal)
		{
			mbAllowImpliedDelimiter = bAllowImpliedDelimiter;
			mbAllowColon = bAllowColon;
			parse();
		}

		/// <summary>
		/// Returns the number of subsegments in the XRI segment
		/// </summary>
		/// <returns>number of subsegments</returns>
		public int getNumSubSegments()
		{
			parse();
			return (moSubSegments == null) ? 0 : moSubSegments.Count;

		}

		/// <summary>
		/// provides an Iterator for the subsegments
		/// </summary>
		/// <returns>provides an Iterator for the subsegments</returns>
		public IEnumerator<XRISubSegment> getSubSegmentIterator()
		{
			parse();
			return (moSubSegments == null) ? null : moSubSegments.GetEnumerator();
		}

		/// <summary>
		/// Returns the subsegment at the given index
		/// </summary>
		/// <param name="nIndex">The index of the subsegment to return</param>
		/// <returns>The subsegment at the specified location</returns>
		public XRISubSegment getSubSegmentAt(int nIndex)
		{
			parse();
			if ((moSubSegments == null) || (nIndex >= moSubSegments.Count))
			{
				return null;
			}

			return (XRISubSegment)moSubSegments[nIndex];
		}

		/// <summary>
		/// Returns the parent XRISegment for this obj.  Equivalent to all but
		/// the last SubSegment.
		/// </summary>
		/// <returns>The parent XRISegment of this obj</returns>
		public XRISegment getParent()
		{
			parse();

			// return null if there isn't atleast 2 subsegments
			if ((this.moSubSegments == null) || (this.moSubSegments.Count <= 1))
			{
				return null;
			}

			// return a XRISegment with the first n-1 subsegments
			XRISegment oParent = new XRISegment(mbAllowImpliedDelimiter, mbAllowColon, mbAllowReassignable);
			oParent.msValue = "";
			oParent.moSubSegments = new List<XRISubSegment>();
			oParent.mbParsed = true;
			oParent.mbParseResult = this.mbParseResult;
			for (int i = 0; i < (this.moSubSegments.Count - 1); i++)
			{
				XRISubSegment oSubseg = (XRISubSegment)this.moSubSegments[i];
				oParent.moSubSegments.Add(oSubseg);
				oParent.msValue += oSubseg.ToString();
			}

			return oParent;
		}

		/// <summary>
		/// Returns the last part of the XRI segment.  Skips over the specified number
		/// of subsegments and returns the remainder as a XRISegment.
		/// For example: This XRI Segment is "!a!b!c!d"
		///              getSegmentRemaider(0) => !a!b!c!d
		///	             getSegmentRemaider(1) => !b!c!d
		///	             getSegmentRemaider(2) => !c!d
		///	             getSegmentRemaider(3) => !d
		///	             getSegmentRemaider(4) => null
		/// </summary>
		/// <param name="nSkip"></param>
		/// <returns>The number of subsegments to skip.</returns>
		public XRISegment getRemainder(int nSkip)
		{
			parse();

			// return null if there isn't atleast nSkip subsegments
			if ((this.moSubSegments == null) ||
				(this.moSubSegments.Count <= nSkip))
			{
				return null;
			}

			// return a XRISegment without the first few subsegments
			XRISegment oRemainder = new XRISegment(false, mbAllowColon, mbAllowReassignable);
			oRemainder.msValue = "";
			oRemainder.moSubSegments = new List<XRISubSegment>();
			oRemainder.mbParsed = true;
			oRemainder.mbParseResult = this.mbParseResult;
			for (int i = nSkip; i < this.moSubSegments.Count; i++)
			{
				XRISubSegment oSubseg = this.moSubSegments[i];
				oRemainder.moSubSegments.Add(oSubseg);
				oRemainder.msValue += oSubseg.ToString();
			}

			return oRemainder;
		}

		/// <summary>
		/// String representation of the obj.
		/// </summary>
		/// <returns>the String form of the XRI with its persistent indicator.</returns>
		public override string ToString()
		{
			parse();

			StringBuilder sRetval = new StringBuilder();
			for (int i = 0; i < moSubSegments.Count; i++)
			{
				// don't output the star at the beginning; hope that's correct; =peacekeeper
				sRetval.Append(moSubSegments[i].ToString(i > 0));
			}

			return sRetval.ToString();
		}

		/// <summary>
		/// Parses the input stream into the obj
		/// </summary>
		/// <param name="oXRISegStream">The input stream to scan from</param>
		/// <returns>True if part of the Stream was consumed into the obj</returns>
		bool doScan(ParseStream oXRISegStream)
		{
			moSubSegments = new List<XRISubSegment>();
			bool bAllowImpliedDelimiter = mbAllowImpliedDelimiter;
			bool bAllowReassignable = mbAllowReassignable;

			// loop through the stream, but don't consume the real string unless
			// we are successful
			while (!oXRISegStream.empty())
			{
				// determine if we have a delimiter for the next subsegment
				char c = oXRISegStream.getData()[0];

				// break out if the first character has to be persistent and isn't
				if ((!bAllowReassignable) && (c != XRI.PDELIM))
				{
					break;
				}

				// check if we have a valid non-null subsegment
				XRISubSegment oSubSegment = new XRISubSegment(bAllowImpliedDelimiter, mbAllowColon);
				if (oSubSegment.scan(oXRISegStream))
				{
					// if we had a valid sub-segment, consume it and add it to the list
					moSubSegments.Add(oSubSegment);
				}
				else
				{
					break;
				}

				bAllowImpliedDelimiter = false;
				bAllowReassignable = true;
			}

			// if we have subsegments, we are good.  Otherwise, it is an error
			if (moSubSegments.Count > 0)
			{
				return true;
			}

			moSubSegments = null;
			return false;

		}

		/// <summary>
		/// Serializes XRISegment into IRI normal from
		/// </summary>
		/// <param name="wantOptionalDelim"></param>
		/// <returns>The IRI normal form of the XRISegment</returns>
		public string toIRINormalForm(bool wantOptionalDelim)
		{
			string sValue = "";
			bool first = true;

			IEnumerator<XRISubSegment> oIt = moSubSegments.GetEnumerator();
			while (oIt.MoveNext())
			{
				sValue += oIt.Current.toIRINormalForm(wantOptionalDelim || !first);
				first = false;
			}

			return sValue;
		}

		/// <summary>
		/// Serializes XRISegment into IRI normal from
		/// </summary>
		/// <returns>The IRI normal form of the XRISegment</returns>
		public string toIRINormalForm()
		{
			return toIRINormalForm(false);
		}

		/// <summary>
		/// Serializes XRISegment into URI normal from
		/// </summary>
		/// <param name="wantOptionalDelim"></param>
		/// <returns>The URI normal form of the XRISegment</returns>
		public string toURINormalForm(bool wantOptionalDelim)
		{
			return IRIUtils.IRItoURI(toIRINormalForm(wantOptionalDelim));
		}

		/// <summary>
		/// Serializes XRISegment into URI normal from
		/// </summary>
		/// <returns>The URI normal form of the XRISegment</returns>
		public string toURINormalForm()
		{
			return toURINormalForm(false);
		}

		public bool Equals(XRISegment segment)
		{
			return ToString().Equals(segment.ToString());
		}

		public bool EqualsIgnoreCase(XRISegment segment)
		{
			return ToString().Equals(segment.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public bool isPrefixOf(XRISegment segment)
		{
			int n = this.getNumSubSegments();

			// first, return false if this segment has more subsegments than the given segment
			if (n > segment.getNumSubSegments())
				return false;

			for (int i = 0; i < n; i++)
			{
				XRISubSegment subseg = this.getSubSegmentAt(i);
				if (!subseg.EqualsIgnoreCase(segment.getSubSegmentAt(i)))
					return false;
			}
			return true;
		}
	}
}