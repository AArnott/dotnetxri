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
	public class XRI3SubSegment : XRI3SyntaxComponent, XRISubSegment
	{
		private Rule rule;

		private char gcs;
		private char lcs;
		private XRI3Literal literal;
		private XRI3XRef xref;

		public XRI3SubSegment(string value)
		{
			this.rule = XRI3Util.getParser().parse("subseg", value);
			this.read();
		}

		public XRI3SubSegment(char gcs, XRISubSegment localSubSegment)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(gcs);
			buffer.Append(localSubSegment.ToString());

			this.rule = XRI3Util.getParser().parse("subseg", buffer.ToString());
			this.read();
		}

		public XRI3SubSegment(char cs, string uri)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(cs.ToString());
			buffer.Append(XRI3Constants.XREF_START);
			buffer.Append(uri);
			buffer.Append(XRI3Constants.XREF_END);

			this.rule = XRI3Util.getParser().parse("subseg", buffer.ToString());
			this.read();
		}

		XRI3SubSegment(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.gcs = null;
			this.lcs = null;
			this.literal = null;
			this.xref = null;
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// subseg or global_subseg or local_subseg or rel_subseg or rel_subseg_nc

			// subseg?

			if (obj is subseg)
			{
				// read global_subseg or local_subseg from subseg

				IList<Rule> list_subseg = ((subseg)obj).rules;
				if (list_subseg.Count < 1)
					return;
				obj = list_subseg[0];	// global_subseg or local_subseg
			}

			// global_subseg?

			if (obj is global_subseg)
			{
				// read gcs_char from global_subseg;

				IList<Rule> list_global_subseg = ((global_subseg)obj).rules;
				if (list_global_subseg.Count < 1)
					return;
				obj = list_global_subseg[0];	// gcs_char
				this.gcs = new char(((gcs_char)obj).spelling.charAt(0));

				// read rel_subseg or local_subseg from global_subseg

				if (list_global_subseg.Count < 2)
					return;
				obj = list_global_subseg[1];	// rel_subseg or local_subseg
			}

			// local_subseg?

			if (obj is local_subseg)
			{
				// read lcs_char from local_subseg;

				IList<Rule> list_local_subseg = ((local_subseg)obj).rules;
				if (list_local_subseg.Count < 1)
					return;
				obj = list_local_subseg[0];	// lcs_char
				this.lcs = new char(((lcs_char)obj).spelling.charAt(0));

				// read rel_subseg from local_subseg

				if (list_local_subseg.Count < 2)
					return;
				obj = list_local_subseg[1];	// rel_subseg
			}

			// rel_subseg or rel_subseg_nc?

			if (obj is rel_subseg)
			{
				// read literal or xref from rel_subseg

				IList<Rule> list_rel_subseg = ((rel_subseg)obj).rules;
				if (list_rel_subseg.Count < 1)
					return;
				obj = list_rel_subseg[0];	// literal or xref
			}
			else if (obj is rel_subseg_nc)
			{
				// read literal_nc or xref from rel_subseg_nc

				IList<Rule> list_rel_subseg_nc = ((rel_subseg_nc)obj).rules;
				if (list_rel_subseg_nc.Count < 1)
					return;
				obj = list_rel_subseg_nc[0];	// literal_nc or xref
			}
			else
			{
				return;
			}

			// literal or literal_nc or xref?

			if (obj is literal)
			{
				this.literal = new XRI3Literal((literal)obj);
			}
			else if (obj is literal_nc)
			{
				this.literal = new XRI3Literal((literal_nc)obj);
			}
			else if (obj is xref)
			{
				this.xref = new XRI3XRef((xref)obj);
			}
			else
			{
				return;
			}
		}

		public Rule ParserObject
		{
			get
			{
				return this.rule;
			}
		}

		public bool hasGCS()
		{
			return (this.gcs != null);
		}

		public bool hasLCS()
		{
			return (this.lcs != null);
		}

		public bool hasLiteral()
		{
			return (this.literal != null);
		}

		public bool hasXRef()
		{
			return (this.xref != null);
		}

		public char GCS
		{
			get
			{
				return this.gcs;
			}
		}

		public char LCS
		{
			get
			{
				return this.lcs;
			}
		}

		public XRILiteral Literal
		{
			get
			{
				return this.literal;
			}
		}

		public XRIXRef XRef
		{
			get
			{
				return this.xref;
			}
		}

		public bool isGlobal()
		{
			return (this.hasGCS());
		}

		public bool isLocal()
		{
			return (this.hasLCS() && !this.hasGCS());
		}

		public bool isPersistent()
		{
			return (this.hasLCS() && this.LCS.Equals(XRI3Constants.LCS_BANG));
		}

		public bool isReassignable()
		{
			return ((this.hasGCS() && !this.hasLCS()) || (this.hasLCS() && this.LCS.Equals(XRI3Constants.LCS_STAR)));
		}
	}
}