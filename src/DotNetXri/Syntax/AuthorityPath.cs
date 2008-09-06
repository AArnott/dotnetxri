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

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This class provides a base class for all types of AuthorityPath elements
	/// </summary>
	public abstract class AuthorityPath : Parsable
	{
		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		protected AuthorityPath()
		{ }

		/// <summary>
		/// Constructs AuthorityPath from a String
		/// </summary>
		/// <param name="sPath"></param>
		protected AuthorityPath(string sPath)
			: base(sPath)
		{ }

		/// <summary>
		/// Static method to build the correct AuthorityPath obj from a String
		/// </summary>
		/// <param name="sPath"></param>
		/// <returns></returns>
		public static AuthorityPath buildAuthorityPath(string sPath)
		{
			ParseStream oStream = new ParseStream(sPath);
			AuthorityPath oPath = scanAuthority(oStream);

			// only return the path if the entire stream was consumed
			return (oStream.getData().Length == 0) ? oPath : null;
		}

		internal static AuthorityPath scanAuthority(ParseStream oParseStream)
		{
			GCSAuthority oGCSAuthority = new GCSAuthority();
			if (oGCSAuthority.scan(oParseStream))
			{
				return oGCSAuthority;
			}

			XRefAuthority oXRefAuthority = new XRefAuthority();
			if (oXRefAuthority.scan(oParseStream))
			{
				return oXRefAuthority;
			}

			IRIAuthority oIRIAuthority = new IRIAuthority();
			if (oIRIAuthority.scan(oParseStream))
			{
				return oIRIAuthority;
			}

			return null;
		}

		/// <summary>
		/// Serializes the authority into IRI-normal form
		/// </summary>
		/// <returns></returns>
		public abstract string toIRINormalForm();

		/// <summary>
		/// Serializes the authority into URI-normal form
		/// </summary>
		/// <returns></returns>
		public abstract string toURINormalForm();
	}
}