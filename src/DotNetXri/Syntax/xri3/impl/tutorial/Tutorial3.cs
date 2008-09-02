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

namespace DotNetXri.Syntax.Xri3.Impl.Tutorial
{
	public class Tutorial3
	{
		public static void Main(string[] args)
		{
			// Something like this may happen when working with XDI.
			// We use the following XRI address: +name+first/$is/+!3
			// We want to know the following
			// - XDI subject
			// - XDI predicate
			// - XDI reference

			XRI xri = new XRI3("+name+first/$is/+!3");
			XRIAuthority xriAuthority = xri.Authority;
			XRIPath xriPath = xri.Path;

			Logger.Info("Checking XDI address " + xri.ToString());

			Logger.Info("XDI Subject: " + xriAuthority.ToString());
			Logger.Info("XDI Predicate: " + xriPath.getSegment(0).ToString());
			Logger.Info("XDI Reference: " + xriPath.getSegment(1).ToString());
		}
	}
}