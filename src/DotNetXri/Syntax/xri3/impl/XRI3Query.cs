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

using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl
{
	public class XRI3Query : XRI3SyntaxComponent, XRIQuery
	{
		private const long serialVersionUID = 8838957773108506171L;

		private Rule rule;

		private string value;

		public XRI3Query(string value)
		{
			this.rule = XRI3Util.getParser().parse("iquery", value);
			this.read();
		}

		XRI3Query(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.value = null;
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// iquery

			this.value = ((iquery)obj).spelling;
		}

		public Rule ParserObject
		{
			get
			{
				return this.rule;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}
	}
}