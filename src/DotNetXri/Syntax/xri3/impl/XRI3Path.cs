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
	public class XRI3Path : XRI3SyntaxComponent, XRIPath
	{
		private Rule rule;

		private IList<XRISegment> segments;

		public XRI3Path(string value)
		{
			this.rule = XRI3Util.getParser().parse("xri-path", value);
			this.read();
		}

		XRI3Path(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.segments = new List<XRISegment>();
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// xri_path or xri_path_abempty or xri_path_abs or xri_path_noscheme

			// xri_path ?

			if (obj is xri_path)
			{
				// read xri_path_abempty or xri_path_abs or xri_path_noscheme from xri_path

				IList<Rule> list_xri_path = ((xri_path)obj).rules;
				if (list_xri_path.Count < 1)
					return;
				obj = list_xri_path[0];	// xri_path_abempty or xri_path_abs or xri_path_noscheme
			}

			// xri_path_abempty or xri_path_abs or xri_path_noscheme ?

			if (obj is xri_path_abempty)
			{
				// read xri_segments from xri_path_abempty

				IList<Rule> list_xri_path_abempty = ((xri_path_abempty)obj).rules;
				if (list_xri_path_abempty.Count < 2)
					return;
				for (int i = 0; i + 1 < list_xri_path_abempty.Count; i += 2)
				{

					obj = list_xri_path_abempty[i + 1];	// xri_segment
					this.segments.Add(new XRI3Segment((xri_segment)obj));
				}
			}
			else if (obj is xri_path_abs)
			{
				// read xri_segment_nz from xri_path_abs

				IList<Rule> list_xri_path_abs = ((xri_path_abs)obj).rules;
				if (list_xri_path_abs.Count < 2)
					return;
				obj = list_xri_path_abs[1];	// xri_segment_nz
				this.segments.Add(new XRI3Segment((xri_segment_nz)obj));

				// read xri_segments from xri_path_abs

				if (list_xri_path_abs.Count < 4)
					return;
				for (int i = 2; i + 1 < list_xri_path_abs.Count; i += 2)
				{
					obj = list_xri_path_abs[i + 1];	// xri_segment
					this.segments.Add(new XRI3Segment((xri_segment)obj));
				}
			}
			else if (obj is xri_path_noscheme)
			{
				// read xri_segment_nc from xri_path_noscheme

				IList<Rule> list_xri_path_noscheme = ((xri_path_noscheme)obj).rules;
				if (list_xri_path_noscheme.Count < 1)
					return;
				obj = list_xri_path_noscheme[0];	// xri_segment_nc
				this.segments.Add(new XRI3Segment((xri_segment_nc)obj));

				// read xri_segments from xri_path_noscheme

				if (list_xri_path_noscheme.Count < 3)
					return;
				for (int i = 1; i + 1 < list_xri_path_noscheme.Count; i += 2)
				{
					obj = list_xri_path_noscheme[i + 1];	// xri_segment
					this.segments.Add(new XRI3Segment((xri_segment)obj));
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

		public IList<XRISegment> Segments
		{
			get
			{
				return this.segments;
			}
		}

		public int NumSegments
		{
			get
			{
				return this.segments.Count;
			}
		}

		public XRISegment getSegment(int i)
		{
			return this.segments[i];
		}

		public XRISegment FirstSegment
		{
			get
			{
				if (this.segments.Count < 1)
					return (null);

				return this.segments[0];
			}
		}

		public XRISegment LastSegment
		{
			get
			{
				if (this.segments.Count < 1)
					return (null);

				return this.segments[this.segments.Count - 1];
			}
		}

		public bool StartsWith(XRISegment[] segments)
		{

			if (this.segments.Count < segments.Length)
				return (false);

			for (int i = 0; i < segments.Length; i++)
			{

				if (!(this.segments[i].Equals(segments[i])))
					return (false);
			}

			return (true);
		}
	}
}