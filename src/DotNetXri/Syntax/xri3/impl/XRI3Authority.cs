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
using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl
{
	public class XRI3Authority : XRI3SyntaxComponent, XRIAuthority
	{
		private const long serialVersionUID = -3281614016180358848L;

		private Rule rule;

		private IList<XRI3SubSegment> subSegments;

		public XRI3Authority(string value)
		{
			this.rule = XRI3Util.getParser().parse("xri-authority", value);
			this.read();
		}

		public XRI3Authority(XRIAuthority xriAuthority, XRISubSegment xriSubSegment)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(xriAuthority.ToString());
			buffer.Append(xriSubSegment.ToString());

			this.rule = XRI3Util.getParser().parse("xri-authority", buffer.ToString());
			this.read();
		}

		XRI3Authority(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.subSegments = new List<XRI3SubSegment>();
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// xri_authority

			// read global_subseg from xri_authority

			IList<Rule> list_xri_authority = ((xri_authority)obj).rules;
			if (list_xri_authority.Count < 1)
				return;
			obj = list_xri_authority[0];	// global_subseg
			this.subSegments.Add(new XRI3SubSegment((global_subseg)obj));

			// read subsegs from xri_authority

			if (list_xri_authority.Count < 2)
				return;
			for (int i = 1; i < list_xri_authority.Count; i++)
			{

				obj = list_xri_authority[i];	// subseg
				this.subSegments.Add(new XRI3SubSegment((subseg)obj));
			}
		}

		public Rule getParserObject()
		{
			return (this.rule);
		}

		public IList<XRI3SubSegment> SubSegments
		{
			get
			{
				return (this.subSegments);
			}
		}

		public int NumSubSegments
		{
			get
			{
				return (this.subSegments.Count);
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
					return (null);

				return this.subSegments[0];
			}
		}

		public XRISubSegment LastSubSegment
		{
			get
			{
				if (this.subSegments.Count < 1)
					return (null);

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