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

namespace DotNetXri.Client.Resolve.Exception
{
	/// <summary>
	/// Base class for exceptions thrown during XRI Resolution
	/// </summary>
	public class XRIResolutionException : System.Exception
	{
		private System.Exception moEx = null;
		private string status = null;

		/// <summary>
		/// Constructs an exception with the given message.
		/// </summary>
		/// <param name="message"></param>
		public XRIResolutionException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Constructs an exception with the given message and underlying exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public XRIResolutionException(string message, System.Exception exception)
			: base(message)
		{
			moEx = exception;

		}

		public XRIResolutionException(string statusCode, string message)
			: base(message)
		{
			status = statusCode;
		}

		/// <summary>
		/// Returns the status.
		/// </summary>
		/// <returns></returns>
		public string getStatus()
		{
			return status;
		}

	}
}