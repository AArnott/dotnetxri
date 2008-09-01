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

using DotNetXri.Client.Xml;

namespace DotNetXri.Client.Resolve.Exception
{
	/// <summary>
	/// This exception is thrown by the top level XRI resolution APIs to indicate
	/// that resolution was not completed successfully.
	/// </summary>
	public class PartialResolutionException : XRIResolutionException
	{
		/// <summary>
		/// XRDS of the partial resolution result.
		/// </summary>
		public XRDS PartialXRDS
		{
			get;
			private set;
		}

		/**
		 * Constructs an obj of this class. 
		 * @param msg Error message
		 * @param xrds The partial resolution results
		 */
		public PartialResolutionException(XRDS xrds)
			: base("Resolution did not complete successfully.")
		{
			PartialXRDS = xrds;
		}

		public PartialResolutionException(XRD xrd)
			: base(xrd.getStatus().getText())
		{
			PartialXRDS = new XRDS();
			PartialXRDS.add(xrd);
		}
	}
}