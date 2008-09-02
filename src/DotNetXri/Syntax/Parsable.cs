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

namespace DotNetXri.Syntax
{
	/// <summary>
	/// This class provides a base class for all classes that are parsed according
	/// to the XRI Syntax definition.
	/// </summary>
	public abstract class Parsable : IComparable
	{
		protected string msValue = null;
		protected bool mbParsed = false;
		protected bool mbParseResult = false;

		/// <summary>
		/// Protected Constructor used by package only
		/// </summary>
		protected Parsable()
		{
			setValue(null);
		}

		/// <summary>
		/// Constructs Parsable obj from a String
		/// </summary>
		/// <param name="value"></param>
		protected Parsable(string value)
		{
			setValue(value);
		}

		private void setValue(string value)
		{
			msValue = value;
		}

		/// <summary>
		/// Outputs the obj according to the XRI Syntax defined for this obj
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return msValue;
		}

		/// <summary>
		/// Parses the set value.
		/// </summary>
		/// <remarks>
		/// Throws XRIParseException if entire value could not be parsed into the
		/// obj
		/// </remarks>
		protected void parse()
		{
			string value = msValue;

			// only do work if the value isn't already parsed
			if (!mbParsed)
			{
				ParseStream oStream = new ParseStream(msValue);

				if (scan(oStream))
				{
					// Did we consume the entire string?
					mbParseResult = oStream.getData().Length == 0;
				}

				// Set to true even if we fail, no need to fail over and over again.
				mbParsed = true;
			}

			// throw an exception if things failed
			if (!mbParseResult)
			{
				throw new XRIParseException(
						"Not a valid " + this.GetType().Name +
						" class: \"" + value + "\"");
			}
		}

		/// <summary>
		/// Scans the stream for parts that can be parsed into the obj
		/// </summary>
		/// <param name="oParseStream">The input stream to read from</param>
		/// <returns>Returns true if all or part of the stream could be
		/// parsed into the obj</returns>
		public bool scan(ParseStream oParseStream)
		{
			if (oParseStream == null)
			{
				return false;
			}

			ParseStream oStream = oParseStream.begin();

			if (doScan(oStream))
			{
				setParsedValue(oParseStream.getConsumed(oStream));
				oParseStream.end(oStream);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Scans the stream for parts that can be parsed into the obj
		/// </summary>
		/// <param name="oParseStream">The input stream to read from</param>
		/// <returns>Returns true if all or part of the stream could be
		/// parsed into the obj</returns>
		protected abstract bool doScan(ParseStream oParseStream);

		/// <summary>
		/// Sets the parsed value for the obj
		/// </summary>
		/// <param name="value">The value to set the obj to</param>
		protected void setParsedValue(string value)
		{
			msValue = value ?? "";

			mbParsed = true;
			mbParseResult = true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Parsable))
				return (false);

			Parsable other = (Parsable)obj;

			if (this.msValue == null && other.msValue != null)
				return (false);
			if (this.msValue != null && other.msValue == null)
				return (false);
			if (this.msValue != null && other.msValue != null && !this.msValue.Equals(other.msValue))
				return (false);

			return (true);
		}

		public override int GetHashCode()
		{
			return (this.msValue == null ? 0 : this.msValue.GetHashCode());
		}

		public int compareTo(Object obj)
		{
			Parsable other = (Parsable)obj;

			if (obj == null || this.msValue == null || other.msValue == null)
				throw new NullReferenceException();

			return (this.msValue.CompareTo(other.msValue));
		}
	}
}