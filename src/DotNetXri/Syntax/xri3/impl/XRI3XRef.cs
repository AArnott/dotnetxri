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
using System.Collections;
using System.Collections.Generic;
using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl
{
	public class XRI3XRef : XRI3SyntaxComponent, XRIXRef
	{
		private const long serialVersionUID = 5499307555025868602L;

		private Rule rule;

		private XRI3Reference xriReference;
		private String iri;

		public XRI3XRef(string value)
		{
			this.rule = XRI3Util.getParser().parse("xref", value);
			this.read();
		}

		XRI3XRef(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.xriReference = null;
			this.iri = null;
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// xref or xref_empty or xref_xri_reference or xref_IRI

			// xref or xref_empty or xref_xri_reference or xref_IRI ?

			if (obj is xref)
			{
				IList<Rule> list_xref = ((xref)obj).rules;
				if (list_xref.Count < 1)
					return;
				obj = list_xref[0];	// xref_empty or xref_xri_reference or xref_IRI
			}
			else if (obj is xref_empty)
			{

			}
			else if (obj is xref_xri_reference)
			{

			}
			else if (obj is xref_IRI)
			{

			}
			else
			{
				throw new InvalidCastException(obj.GetType().Name);
			}

			// xref_empty or xref_xri_reference or xref_IRI ?
			
			if (obj is xref_empty)
			{

			}
			else if (obj is xref_xri_reference)
			{
				// read xri_reference from xref_xri_reference

				IList<Rule> list_xref_xri_reference = ((xref_xri_reference)obj).rules;
				if (list_xref_xri_reference.Count < 2)
					return;
				obj = list_xref_xri_reference[1];	// xri_reference
				this.xriReference = new XRI3Reference((xri_reference)obj);
			}
			else if (obj is xref_IRI)
			{
				// read IRI from xref_IRI

				IList<Rule> list_xref_IRI = ((xref_IRI)obj).rules;
				if (list_xref_IRI.Count < 2)
					return;
				obj = list_xref_IRI[1];	// IRI
				this.iri = ((IRI)obj).spelling;
			}
			else
			{
				throw new InvalidCastException(obj.GetType().Name);
			}
		}

		public Rule ParserObject
		{
			get
			{
				return this.rule;
			}
		}

		public bool hasXRIReference()
		{
			return (this.xriReference != null);
		}

		public bool hasIRI()
		{
			return (this.iri != null);
		}

		public XRIReference XRIReference
		{
			get
			{
				return this.xriReference;
			}
		}

		public string IRI
		{
			get
			{
				return this.iri;
			}
		}
	}
}