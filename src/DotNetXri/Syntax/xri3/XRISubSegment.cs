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

namespace DotNetXri.Syntax.Xri3
{
	public interface XRISubSegment : XRISyntaxComponent
	{
		public bool hasGCS();
		public bool hasLCS();
		public bool hasLiteral();
		public bool hasXRef();

		public char getGCS();
		public char getLCS();
		public XRILiteral getLiteral();
		public XRIXRef getXRef();

		public bool isGlobal();
		public bool isLocal();
		public bool isPersistent();
		public bool isReassignable();
	}
}