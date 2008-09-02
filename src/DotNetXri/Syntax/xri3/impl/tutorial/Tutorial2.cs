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
	public class Tutorial2
	{
		public static void Main(string[] args)
		{
			// The library can also construct new XRIs or XRI components.
			// For example, if we have an XRI +name, and a relative XRI reference +first,
			// we can construct a new XRI +name+first

			XRI xri = new XRI3("+name");
			XRIReference xriReference = new XRI3Reference("+first");

			Logger.Info("Got XRI " + xri.ToString());
			Logger.Info("Got XRI reference " + xriReference.ToString());

			XRI xriNew = new XRI3(xri, xriReference);

			Logger.Info("Constructed new XRI " + xriNew.ToString());
		}
	}
}