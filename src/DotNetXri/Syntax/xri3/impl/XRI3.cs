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
	public class XRI3 : XRI3SyntaxComponent, XRI
	{
		private static readonly ICollection<string> reserved = new HashSet<string>(new string[] {
			"user",
			"users",
			"individual",
			"individuals",
			"person",
			"persons",
			"personal",
			"personal.name",
			"personal.names",
			"organization",
			"organizations",
			"organizational",
			"organizational.name",
			"organizational.names",
			"name",
			"names",
			"iname",
			"inames",
			"i-name",
			"i-names",
			"i.name",
			"i.names",
			"number",
			"numbers",
			"inumber",
			"inumbers",
			"i-number",
			"i-numbers",
			"i.number",
			"i.numbers",
			"broker",
			"brokers",
			"i-broker",
			"i-brokers",
			"i.broker",
			"i.brokers",
			"gsp",
			"grsp",
			"global.service",
			"global.services",
			"global.service.provider",
			"global.service.providers",
			"public",
			"trust",
			"federation",
			"federations",
			"global",
			"service",
			"services",
			"provider",
			"providers",
			"registry",
			"registries",
			"registrant",
			"registrants",
			"aero",
			"biz",
			"cat",
			"com",
			"coop",
			"info",
			"jobs",
			"mobi",
			"museum",
			"net",
			"org",
			"pro",
			"travel",
			"gov",
			"edu",
			"mil",
			"int",
			"www",
			"ftp",
			"mail",
			"xdi",
			"xdiorg",
			"xdi-org",
			"xdi.org",
			"xri",
			"xriorg",
			"xri-org",
			"xri.org",
			"xri.xdi",
			"xdi.xri",
			"xri-xdi",
			"xdi-xri",
			"itrust",
			"i-trust",
			"i.trust",
			"cordance",
			"cordance.corp",
			"cordance.corporation",
			"cordance.net"
		});

		private Rule rule;

		private string scheme;
		private XRI3Authority authority;
		private XRI3Path path;
		private XRI3Query query;
		private XRI3Fragment fragment;

		public XRI3(string value)
		{
			this.rule = XRI3Util.getParser().parse("xri", value);
			this.read();
		}

		public XRI3(XRI xri, XRISyntaxComponent xriPart)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(xri.ToString());
			buffer.Append(xriPart.ToString());

			this.rule = XRI3Util.getParser().parse("xri", buffer.ToString());
			this.read();
		}

		public XRI3(XRI xri, string xriPart)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(xri.ToString());
			buffer.Append(xriPart);

			this.rule = XRI3Util.getParser().parse("xri", buffer.ToString());
			this.read();
		}

		public XRI3(char gcs, string uri)
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append(gcs.ToString());
			buffer.Append(XRI3Constants.XREF_START);
			buffer.Append(uri);
			buffer.Append(XRI3Constants.XREF_END);

			this.rule = XRI3Util.getParser().parse("xri", buffer.ToString());
			this.read();
		}

		XRI3(Rule rule)
		{
			this.rule = rule;
			this.read();
		}

		private void reset()
		{
			this.scheme = null;
			this.authority = null;
			this.path = null;
			this.query = null;
			this.fragment = null;
		}

		private void read()
		{
			this.reset();

			object obj = this.rule;	// xri

			// read xri_scheme or xri_noscheme from xri

			IList<Rule> list_xri = ((xri)obj).rules;
			if (list_xri.Count < 1)
				return;
			obj = list_xri[0];	// xri_scheme or xri_noscheme

			// xri_scheme or xri_noscheme ?

			if (obj is xri_scheme)
			{

				// read "xri:" from xri_scheme

				IList<Rule> list_xri_scheme = ((xri_scheme)obj).rules;
				if (list_xri_scheme.Count < 1)
					return;
				obj = list_xri_scheme[0];	// "xri:"
				this.scheme = ((StringValue)obj).spelling;

				// read xri_noscheme from xri_scheme

				if (list_xri_scheme.Count < 2)
					return;
				obj = list_xri_scheme[1];	// xri_noscheme
			}
			else if (obj is xri_noscheme)
			{

			}
			else
			{
				throw new InvalidCastException(obj.GetType().Name);
			}

			// read xri_hier_part from xri_noscheme

			IList<Rule> list_xri_noscheme = ((xri_noscheme)obj).rules;
			if (list_xri_noscheme.Count < 1)
				return;
			obj = list_xri_noscheme[0];	// xri_hier_part

			// read xri_authority from xri_hier_part

			IList<Rule> list_xri_hier_part = ((xri_hier_part)obj).rules;
			if (list_xri_hier_part.Count < 1)
				return;
			obj = list_xri_hier_part[0];	// xri_authority
			this.authority = new XRI3Authority((xri_authority)obj);
			if (this.authority.getParserObject().spelling.Length < 1)
				this.authority = null;

			// read xri_path_abempty from xri_hier_part

			if (list_xri_hier_part.Count < 2)
				return;
			obj = list_xri_hier_part[1];	// xri_path_abempty
			this.path = new XRI3Path((xri_path_abempty)obj);
			if (this.path.getParserObject().spelling.Length < 1)
				this.path = null;

			// read iquery or ifragment from xri_noscheme

			if (list_xri_noscheme.Count < 3)
				return;
			obj = list_xri_noscheme[2];	// iquery or ifragment

			// iquery or ifragment ?

			if (obj is iquery)
			{
				this.query = new XRI3Query((iquery)obj);
				if (this.query.getParserObject().spelling.length() < 1)
					this.query = null;

				// read ifragment from xri_noscheme

				if (list_xri_noscheme.Count < 5)
					return;
				obj = list_xri_noscheme[4];	// ifragment
				this.fragment = new XRI3Fragment((ifragment)obj);
				if (this.fragment.getParserObject().spelling.length() < 1)
					this.fragment = null;
			}
			else if (obj is ifragment)
			{
				this.fragment = new XRI3Fragment((ifragment)obj);
				if (this.fragment.getParserObject().spelling.length() < 1)
					this.fragment = null;
			}
			else
			{
				throw new InvalidCastException(obj.GetType().Name);
			}
		}

		public Rule getParserObject()
		{
			return (this.rule);
		}

		public bool hasScheme()
		{
			return (this.scheme != null);
		}

		public bool hasAuthority()
		{
			return (this.authority != null);
		}

		public bool hasPath()
		{
			return (this.path != null);
		}

		public bool hasQuery()
		{
			return (this.query != null);
		}

		public bool hasFragment()
		{
			return (this.fragment != null);
		}

		public string Scheme
		{
			get
			{
				return (this.scheme);
			}
		}

		public XRIAuthority Authority
		{
			get
			{
				return (this.authority);
			}
		}

		public XRIPath Path
		{
			get
			{
				return (this.path);
			}
		}

		public XRIQuery Query
		{
			get
			{
				return (this.query);
			}
		}

		public XRIFragment Fragment
		{
			get
			{
				return (this.fragment);
			}
		}

		public bool isIName()
		{
			IList<XRI3SubSegment> subSegments = this.authority.SubSegments;

			// all subsegments must be reassignable

			for (int i = 0; i < subSegments.Count; i++)
			{
				XRI3SubSegment subSegment = subSegments[i];
				if (!subSegment.isReassignable())
					return (false);
			}

			// some additional rules for i-names

			string spelling = this.authority.ToString();

			if (spelling.StartsWith("."))
				return (false);
			if (spelling.EndsWith("."))
				return (false);
			if (spelling.StartsWith("-"))
				return (false);
			if (spelling.EndsWith("-"))
				return (false);
			if (spelling.IndexOf("..") >= 0)
				return (false);
			if (spelling.IndexOf("--") >= 0)
				return (false);
			if (spelling.IndexOf(".-") >= 0)
				return (false);
			if (spelling.IndexOf("-.") >= 0)
				return (false);
			if (spelling.IndexOf('%') >= 0)
				return (false);
			if (spelling.IndexOf('_') >= 0)
				return (false);
			if (spelling.Length > 254)
				return (false);

			return (true);
		}

		public bool isINumber()
		{
			IList<XRI3SubSegment> subSegments = this.authority.SubSegments;

			// all subsegments must be persistent

			for (int i = 0; i < subSegments.Count; i++)
			{
				XRI3SubSegment subSegment = subSegments[i];
				if (!subSegment.isPersistent())
					return (false);
			}

			return (true);
		}

		public bool isReserved()
		{
			string spelling = this.authority.ToString();

			return (reserved.Contains(spelling.Substring(1)) | reserved.Contains(spelling.Substring(1)));
		}

		public String toIRINormalForm()
		{
			StringBuilder iri = new StringBuilder();

			// authority

			if (this.authority != null)
			{
				iri.Append(XRI3Constants.XRI_SCHEME);
				iri.Append(XRI3Constants.AUTHORITY_PREFIX).Append(this.authority.toIRINormalForm());
			}

			// path

			if (this.path != null)
			{
				iri.Append(XRI3Constants.PATH_PREFIX).Append(this.path.toIRINormalForm());
			}

			// query

			if (this.query != null)
			{
				iri.Append(XRI3Constants.QUERY_PREFIX).Append(this.query.toIRINormalForm());
			}

			// fragment

			if (this.fragment != null)
			{
				iri.Append(XRI3Constants.FRAGMENT_PREFIX).Append(this.fragment.toIRINormalForm());
			}

			// done

			return (iri.ToString());
		}

		public bool isValidXRIReference()
		{
			XRIReference xriReference;

			try
			{
				xriReference = this.toXRIReference();
			}
			catch (Exception)
			{
				return (false);
			}

			return (xriReference != null);
		}

		public XRIReference toXRIReference()
		{
			return (new XRI3Reference(this.ToString()));
		}

		public XRI3Reference toXRI3Reference()
		{
			return (new XRI3Reference(this.ToString()));
		}

		public bool StartsWith(XRI xri)
		{
			if (xri.Authority == null)
				return (true);
			if (xri.Authority != null && this.Authority == null)
				return (false);

			if (!this.Authority.Equals(xri.Authority))
				return (false);

			if (xri.Path == null)
				return (true);
			if (xri.Path != null && this.Path == null)
				return (false);

			IList<XRISegment> thisSegments = this.Path.getSegments();
			IList<XRISegment> xriSegments = xri.Path.getSegments();

			if (thisSegments.Count < xriSegments.Count)
				return (false);

			for (int i = 0; i < xriSegments.Count; i++)
			{
				if (!(thisSegments[i].Equals(xriSegments[i])))
					return (false);
			}

			return (true);
		}
	}
}