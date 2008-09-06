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
using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl
{
	public class XRI3Segment : XRI3SyntaxComponent, XRISegment
	{
		private Rule rule;

		private IList<XRISubSegment> subSegments;

		public XRI3Segment(string value)
		{
			this.rule = XRI3Util.getParser().parse("xri-segment", value);
			this.read();
		}

		internal XRI3Segment(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.subSegments = new List<XRISubSegment>();
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// xri_segment or xri_segment_nz or xri_segment_nc

			// read rel_subsegs or subsegs or rel_subseg_ncs from xri_segment or xri_segment_nz or xri_segment_nc

			// xri_segment or xri_segment_nz or xri_segment_nc ?

			if (obj is Parser.Parser.xri_segment)
			{
				// read rel_subseg or subseg from xri_segment

				IList<Rule> list_xri_segment = ((Parser.Parser.xri_segment)obj).rules;
				if (list_xri_segment.Count < 1)
					return;
				obj = list_xri_segment[0];	// rel_subseg or subseg

				// rel_subseg or subseg?

				if (obj is Parser.Parser.rel_subseg)
				{
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.rel_subseg)obj));
				}
				else if (obj is Parser.Parser.subseg)
				{
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.subseg)obj));
				}
				else
				{
					throw new InvalidCastException(obj.GetType().Name);
				}

				// read subsegs from xri_segment

				if (list_xri_segment.Count < 2)
					return;
				for (int i = 1; i < list_xri_segment.Count; i++)
				{
					obj = list_xri_segment[i];	// subseg
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.subseg)obj));
				}
			}
			else if (obj is Parser.Parser.xri_segment_nz)
			{
				// read rel_subseg or subseg from xri_segment_nz

				IList<Rule> list_xri_segment_nz = ((Parser.Parser.xri_segment_nz)obj).rules;
				if (list_xri_segment_nz.Count < 1)
					return;
				obj = list_xri_segment_nz[0];	// rel_subseg or subseg

				// rel_subseg or subseg?

				if (obj is Parser.Parser.rel_subseg)
				{
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.rel_subseg)obj));
				}
				else if (obj is Parser.Parser.subseg)
				{
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.subseg)obj));
				}
				else
				{
					throw new InvalidCastException(obj.GetType().Name);
				}

				// read subsegs from xri_segment

				if (list_xri_segment_nz.Count < 2)
					return;
				for (int i = 1; i < list_xri_segment_nz.Count; i++)
				{
					obj = list_xri_segment_nz[i];	// subseg
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.subseg)obj));
				}
			}
			else if (obj is Parser.Parser.xri_segment_nc)
			{
				// read rel_subseg_nc or subseg from xri_segment_nc

				IList<Rule> list_xri_segment_nc = ((Parser.Parser.xri_segment_nc)obj).rules;
				if (list_xri_segment_nc.Count < 1)
					return;
				obj = list_xri_segment_nc[0];	// rel_subseg_nc or subseg

				// rel_subseg_nc or subseg?

				if (obj is Parser.Parser.rel_subseg_nc)
				{
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.rel_subseg_nc)obj));
				}
				else if (obj is Parser.Parser.subseg)
				{
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.subseg)obj));
				}
				else
				{
					throw new InvalidCastException(obj.GetType().Name);
				}

				// read subsegs from xri_segment

				if (list_xri_segment_nc.Count < 2)
					return;
				for (int i = 1; i < list_xri_segment_nc.Count; i++)
				{
					obj = list_xri_segment_nc[i];	// subseg
					this.subSegments.Add(new XRI3SubSegment((Parser.Parser.subseg)obj));
				}
			}
			else
			{
				throw new InvalidCastException(obj.GetType().Name);
			}
		}

		public Rule ParserObject
		{
			get
			{
				return this.rule;
			}
		}

		public IList<XRISubSegment> SubSegments
		{
			get
			{
				return this.subSegments;
			}
		}

		public int NumSubSegments
		{
			get
			{
				return this.subSegments.Count;
			}
		}

		public XRISubSegment getSubSegment(int i)
		{
			return this.subSegments[i];
		}

		public XRISubSegment FirstSubSegment
		{
			get
			{
				if (this.subSegments.Count < 1)
					return null;

				return this.subSegments[0];
			}
		}

		public XRISubSegment LastSubSegment
		{
			get
			{
				if (this.subSegments.Count < 1)
					return null;

				return this.subSegments[this.subSegments.Count - 1];
			}
		}

		public bool StartsWith(XRISubSegment[] subSegments)
		{

			if (this.subSegments.Count < subSegments.Length)
				return (false);

			for (int i = 0; i < subSegments.Length; i++)
			{

				if (!(this.subSegments[i].Equals(subSegments[i])))
					return (false);
			}

			return (true);
		}
	}
}