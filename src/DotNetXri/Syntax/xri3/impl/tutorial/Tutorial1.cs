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
	public class Tutorial1
	{
		public static void Main(string[] args)
		{
			// Let's assume we are a resolver that got a simple XRI to resolve.
			// The XRI is: =parity*markus/+contact
			// The resolver needs to know the following:
			// - list of subsegments
			// - path

			XRI xri = new XRI3("=parity*markus/+contact");
			XRIAuthority xriAuthority = xri.Authority;
			XRIPath xriPath = xri.Path;

			Logger.Info("Resolving XRI " + xri.ToString());
			Logger.Info("Listing " + xriAuthority.getNumSubSegments() + " subsegments...");

			for (int i = 0; i < xriAuthority.getNumSubSegments(); i++)
			{
				XRISubSegment subSegment = xriAuthority.getSubSegment(i);
				Logger.Info("Subsegment #" + i + ": " + subSegment.ToString());
				Logger.Info("  Global: " + subSegment.isGlobal());
				Logger.Info("  Local: " + subSegment.isLocal());
			}

			Logger.Info("Path: " + xriPath.ToString());
		}
	}
}