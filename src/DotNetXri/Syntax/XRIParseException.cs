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
	/// This class is used to indicate a parsing failure of an XRI syntax element.
	/// </summary>
	public class XRIParseException : Exception
	{
		private Exception moEx = null;

		/// <summary>
		/// Constructs a XRIParseException with a default message
		/// </summary>
		public XRIParseException()
			: base("Invalid XRI")
		{ }

		/// <summary>
		/// Constructs a XRIParseException with the provided message
		/// </summary>
		/// <param name="message"></param>
		public XRIParseException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Constructs a XRIParseException with the provided message and
		/// based off of the provided Exception
		/// </summary>
		/// <param name="sMsg"></param>
		/// <param name="oEx"></param>
		public XRIParseException(string message, Exception exception)
			: base(message)
		{
			moEx = exception;
		}
	}
}