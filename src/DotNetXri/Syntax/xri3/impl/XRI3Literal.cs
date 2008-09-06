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
	public class XRI3Literal : XRI3SyntaxComponent, XRILiteral
	{
		private Rule rule;

		private string value;

		public XRI3Literal(string value)
		{
			this.rule = XRI3Util.getParser().parse("literal", value);
			this.read();
		}

		internal XRI3Literal(Rule rule)
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

			object obj = this.rule;	// literal or literal_nc

			// literal of literal_nc

			if (obj is Parser.Parser.literal)
			{
				this.value = ((Parser.Parser.literal)obj).spelling;
			}
			else if (obj is Parser.Parser.literal_nc)
			{
				this.value = ((Parser.Parser.literal_nc)obj).spelling;
			}
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