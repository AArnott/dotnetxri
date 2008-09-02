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
	/// This class is a utility class for parsing a String into Parsable objects
	/// </summary>
	public class ParseStream
	{
		string msData = null;
		int mnConsumed = 0;

		/// <summary>
		/// Constructs and input stream from a String
		/// </summary>
		/// <param name="sString"></param>
		public ParseStream(string sString)
		{
			msData = sString;
		}

		/// <summary>
		/// Constructs a new ParseStream obj for consuming data based on the
		/// current obj
		/// </summary>
		/// <returns></returns>
		public ParseStream begin()
		{
			return new ParseStream(msData);
		}

		/// <summary>
		/// Consumes all or part of the current string based on the passed in
		/// stream.  The passed in stream MUST have been created as a result of
		/// a call to begin()
		/// </summary>
		/// <param name="oRef">The stream containing the data that has yet to be consumed</param>
		public void end(ParseStream oRef)
		{
			msData = oRef.msData;
			mnConsumed += oRef.mnConsumed;
		}

		/// <summary>
		/// Returns the String that has been consumed by the passed in stream.  The
		/// passed in stream MUST have been created as a result of a call to begin()
		/// </summary>
		/// <param name="oRef">The stream containing the data that has yet to be consumed</param>
		/// <returns>The string that was consumed by the passed in stream.</returns>
		public string getConsumed(ParseStream oRef)
		{
			return (oRef.mnConsumed > 0) ? msData.Substring(0, oRef.mnConsumed) : null;
		}

		/// <summary>
		/// Returns whether or not the stream is empty
		/// </summary>
		/// <returns>Returns true if stream is empty</returns>
		public bool empty()
		{
			return msData.Length == 0;
		}

		/// <summary>
		/// Consumes a given number of characters
		/// </summary>
		/// <param name="nSize">The amount of characters to consume</param>
		public void consume(int nSize)
		{
			if (nSize > 0)
			{
				mnConsumed += nSize;
				msData = msData.Substring(nSize);
			}
		}

		/// <summary>
		/// Returns the characters yet to be consumed
		/// </summary>
		/// <returns>The String representation of the characters yet to be
		/// consumed</returns>
		public override string ToString()
		{
			return msData;
		}

		/// <summary>
		/// Returns the characters yet to be consumed
		/// </summary>
		/// <returns>The String representation of the characters yet to be
		/// consumed</returns>
		public string getData()
		{
			return msData;
		}
	}
}