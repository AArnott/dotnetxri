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

namespace DotNetXri.Syntax.Xri3.Impl.Parser
{
	public abstract class Rule
	{
		public readonly string spelling;
		public readonly IList<Rule> rules;

		protected Rule(string spelling, IList<Rule> rules)
		{
			this.spelling = spelling;
			this.rules = rules;
		}

		public Rule(Rule rule)
			: this(rule.spelling, rule.rules)
		{ }

		public override string ToString()
		{
			return spelling;
		}

		public override bool Equals(object obj)
		{
			return obj is Rule && spelling.Equals(((Rule)obj).spelling);
		}

		public override int GetHashCode()
		{
			return spelling.GetHashCode();
		}

		public int CompareTo(Rule rule)
		{
			return spelling.CompareTo(rule.spelling);
		}

		public abstract object visit(Visitor visitor);
	}
}