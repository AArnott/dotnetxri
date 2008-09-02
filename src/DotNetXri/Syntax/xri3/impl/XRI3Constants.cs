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

namespace DotNetXri.Syntax.Xri3.Impl
{
	public class XRI3Constants
	{
		public const string XRI_SCHEME = "xri:";

		public const string AUTHORITY_PREFIX = "";
		public const string PATH_PREFIX = "/";
		public const string QUERY_PREFIX = "?";
		public const string FRAGMENT_PREFIX = "#";

		public const string XREF_START = "(";
		public const string XREF_END = ")";

		public static readonly XRI3 XRI_NULL = new XRI3("$null");

		public const char GCS_EQUALS = '=';
		public const char GCS_AT = '@';
		public const char GCS_PLUS = '+';
		public const char GCS_DOLLAR = '$';

		public const char LCS_STAR = '*';
		public const char LCS_BANG = '!';

		public static readonly char[] GCS_ARRAY = new char[] {
			GCS_EQUALS,
			GCS_AT,
			GCS_PLUS,
			GCS_DOLLAR
		};

		public static readonly char[] LCS_ARRAY = new char[] {
			LCS_STAR,
			LCS_BANG
		};
	}
}