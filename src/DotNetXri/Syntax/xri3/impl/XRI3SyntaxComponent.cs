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
using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl
{
	public abstract class XRI3SyntaxComponent : XRISyntaxComponent
	{
		public abstract Rule getParserObject();

		public string toIRINormalForm()
		{
			string this_spelling = this.getParserObject().spelling;
			if (this_spelling == null)
				throw new NullReferenceException();

			return (IRIUtils.XRItoIRI(this_spelling, false));
		}

		public string toURINormalForm()
		{
			return (IRIUtils.IRItoURI(this.toIRINormalForm()));
		}

		public int CompareTo(object obj)
		{
			if (obj == this)
				return (0);
			if (obj == null)
				throw new NullReferenceException();
			if (obj is String)
				return (this.ToString().CompareTo((string)obj));
			if (!(obj is XRI3SyntaxComponent))
				return (0);

			String this_spelling = this.getParserObject().spelling;
			String obj_spelling = ((XRI3SyntaxComponent)obj).getParserObject().spelling;
			if (this_spelling == null || obj_spelling == null)
				throw new NullReferenceException();

			return (this_spelling.CompareTo(obj_spelling));
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return (true);
			if (obj == null)
				return (false);
			if (obj is String)
				return (this.ToString().Equals(obj));
			if (!(obj is XRI3SyntaxComponent))
				return (false);

			String this_spelling = this.getParserObject().spelling;
			String obj_spelling = ((XRI3SyntaxComponent)obj).getParserObject().spelling;
			if (this_spelling == null || obj_spelling == null)
				return (false);

			return (this_spelling.Equals(obj_spelling));
		}

		public override int GetHashCode()
		{
			string this_spelling = this.getParserObject().spelling;
			if (this_spelling == null)
				return (0);

			return (this_spelling.GetHashCode());
		}

		public override string ToString()
		{
			return (this.getParserObject().spelling);
		}
	}
}