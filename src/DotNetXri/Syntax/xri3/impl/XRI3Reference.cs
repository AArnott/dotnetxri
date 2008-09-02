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
using System.Collections.Generic;
using System.Text;
using DotNetXri.Syntax.Xri3.Impl.Parser;

namespace DotNetXri.Syntax.Xri3.Impl
{
	public class XRI3Reference : XRI3SyntaxComponent, XRIReference
	{
		private Rule rule;

		private XRI3 xri;
		private XRI3Path path;
		private XRI3Query query;
		private XRI3Fragment fragment;

		public XRI3Reference(string value)
		{
			this.rule = XRI3Util.getParser().parse("xri-reference", value);
			this.read();
		}

		public XRI3Reference(XRIReference xriReference, XRISyntaxComponent xriPart)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(xriReference.ToString());
			buffer.Append(xriPart.ToString());

			this.rule = XRI3Util.getParser().parse("xri-reference", buffer.ToString());
			this.read();
		}

		public XRI3Reference(XRIReference xriReference, string xriPart)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(xriReference.ToString());
			buffer.Append(xriPart);

			this.rule = XRI3Util.getParser().parse("xri-reference", buffer.ToString());
			this.read();
		}

		XRI3Reference(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.xri = null;
			this.path = null;
			this.query = null;
			this.fragment = null;
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// xri_reference

			// read xri or relative_xri_ref from xri_reference

			IList<Rule> list_xri_reference = ((xri_reference)obj).rules;
			if (list_xri_reference.Count < 1)
				return;
			obj = list_xri_reference[0];	// xri or relative_xri_ref

			// xri or relative_xri_ref ?

			if (obj is xri)
			{
				this.xri = new XRI3((xri)obj);
			}
			else if (obj is relative_xri_ref)
			{

				// read relative_xri_part from relative_xri_ref

				IList<Rule> list_relative_xri_ref = ((relative_xri_ref)obj).rules;
				if (list_relative_xri_ref.Count < 1)
					return;
				obj = list_relative_xri_ref[0];	// relative_xri_part

				// read xri_path_abs or xri_path_noscheme or ipath_empty from relative_xri_part

				IList<Rule> list_relative_xri_part = ((relative_xri_part)obj).rules;
				if (list_relative_xri_part.Count < 1)
					return;
				obj = list_relative_xri_part[0];	// xri_path_abs or xri_path_noscheme or ipath_empty	

				// read xri_path_abs or xri_path_noscheme or ipath_emptry ?

				if (obj is xri_path_abs)
				{
					this.path = new XRI3Path((xri_path_abs)obj);
				}
				else if (obj is xri_path_noscheme)
				{
					this.path = new XRI3Path((xri_path_noscheme)obj);
				}
				else if (obj is ipath_empty)
				{
					this.path = new XRI3Path((ipath_empty)obj);
				}
				else
				{
					throw new InvalidCastException(obj.GetType().Name);
				}

				// read iquery from relative_xri_ref

				if (list_relative_xri_ref.Count < 3)
					return;
				obj = list_relative_xri_ref[2];	// iquery
				this.query = new XRI3Query((iquery)obj);

				// read ifragment from relative_xri_ref

				if (list_relative_xri_ref.Count < 5)
					return;
				obj = list_relative_xri_ref[4];	// ifragment
				this.fragment = new XRI3Fragment((ifragment)obj);
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

		public bool hasScheme()
		{
			if (this.xri != null)
				return (this.xri.hasScheme());

			return (false);
		}

		public bool hasAuthority()
		{
			if (this.xri != null)
				return (this.xri.hasAuthority());

			return (false);
		}

		public bool hasPath()
		{
			if (this.xri != null)
				return (this.xri.hasPath());

			return (this.path != null);
		}

		public bool hasQuery()
		{
			if (this.xri != null)
				return (this.xri.hasQuery());

			return (this.query != null);
		}

		public bool hasFragment()
		{
			if (this.xri != null)
				return (this.xri.hasFragment());

			return (this.fragment != null);
		}

		public string Scheme
		{
			get
			{
				if (this.xri != null)
					return (this.xri.Scheme);

				return (null);
			}
		}

		public XRIAuthority Authority
		{
			get
			{
				if (this.xri != null)
					return (this.xri.Authority);

				return (null);
			}
		}

		public XRIPath Path
		{
			get
			{
				if (this.xri != null)
					return (this.xri.Path);

				return (this.path);
			}
		}

		public XRIQuery Query
		{
			get
			{
				if (this.xri != null)
					return (this.xri.Query);

				return (this.query);
			}
		}

		public XRIFragment Fragment
		{
			get
			{
				if (this.xri != null)
					return (this.xri.Fragment);

				return (this.fragment);
			}
		}

		public string toIRINormalForm()
		{
			if (this.xri != null)
				return (this.xri.toIRINormalForm());

			return (base.toIRINormalForm());
		}

		public bool isValidXRI()
		{
			XRI xri;

			try
			{
				xri = this.toXRI();
			}
			catch (Exception)
			{
				return (false);
			}

			return (xri != null);
		}

		public XRI toXRI()
		{
			return (new XRI3(this.ToString()));
		}

		public XRI3 toXRI3()
		{
			return (new XRI3(this.ToString()));
		}
	}
}