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

using System.Collections;
using System.Collections.Generic;

namespace DotNetXri.Syntax.Xri3.Impl.Parser
{
	public class Displayer : Visitor
	{
		public void visit(Rule rule)
		{
			rule.visit(this);
		}

		public object visit_xri(Parser.xri rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_scheme(Parser.xri_scheme rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_noscheme(Parser.xri_noscheme rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_reference(Parser.xri_reference rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_relative_xri_ref(Parser.relative_xri_ref rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_relative_xri_part(Parser.relative_xri_part rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_hier_part(Parser.xri_hier_part rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_authority(Parser.xri_authority rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_subseg(Parser.subseg rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_global_subseg(Parser.global_subseg rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_local_subseg(Parser.local_subseg rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_gcs_char(Parser.gcs_char rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_lcs_char(Parser.lcs_char rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_rel_subseg(Parser.rel_subseg rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_rel_subseg_nc(Parser.rel_subseg_nc rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_literal(Parser.literal rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_literal_nc(Parser.literal_nc rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xref(Parser.xref rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xref_empty(Parser.xref_empty rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xref_xri_reference(Parser.xref_xri_reference rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xref_IRI(Parser.xref_IRI rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xref_value(Parser.xref_value rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_path(Parser.xri_path rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_path_abempty(Parser.xri_path_abempty rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_path_abs(Parser.xri_path_abs rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_path_noscheme(Parser.xri_path_noscheme rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_segment(Parser.xri_segment rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_segment_nz(Parser.xri_segment_nz rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_segment_nc(Parser.xri_segment_nc rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_pchar(Parser.xri_pchar rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_pchar_nc(Parser.xri_pchar_nc rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_reserved(Parser.xri_reserved rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_gen_delims(Parser.xri_gen_delims rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_xri_sub_delims(Parser.xri_sub_delims rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_IRI(Parser.IRI rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_scheme(Parser.scheme rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ihier_part(Parser.ihier_part rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_iauthority(Parser.iauthority rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_iuserinfo(Parser.iuserinfo rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ihost(Parser.ihost rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_IP_literal(Parser.IP_literal rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_IPvFuture(Parser.IPvFuture rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_IPv6address(Parser.IPv6address rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ls32(Parser.ls32 rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_h16(Parser.h16 rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_IPv4address(Parser.IPv4address rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_dec_octet(Parser.dec_octet rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ireg_name(Parser.ireg_name rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_port(Parser.port rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ipath_abempty(Parser.ipath_abempty rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ipath_abs(Parser.ipath_abs rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ipath_rootless(Parser.ipath_rootless rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ipath_empty(Parser.ipath_empty rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_isegment(Parser.isegment rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_isegment_nz(Parser.isegment_nz rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_iquery(Parser.iquery rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_iprivate(Parser.iprivate rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ifragment(Parser.ifragment rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ipchar(Parser.ipchar rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_iunreserved(Parser.iunreserved rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_pct_encoded(Parser.pct_encoded rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ucschar(Parser.ucschar rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_reserved(Parser.reserved rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_gen_delims(Parser.gen_delims rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_sub_delims(Parser.sub_delims rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_unreserved(Parser.unreserved rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_ALPHA(Parser.ALPHA rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_BIT(Parser.BIT rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_CHAR(Parser.CHAR rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_CR(Parser.CR rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_CRLF(Parser.CRLF rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_CTL(Parser.CTL rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_DIGIT(Parser.DIGIT rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_DQUOTE(Parser.DQUOTE rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_HEXDIG(Parser.HEXDIG rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_HTAB(Parser.HTAB rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_LF(Parser.LF rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_LWSP(Parser.LWSP rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_OCTET(Parser.OCTET rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_SP(Parser.SP rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_VCHAR(Parser.VCHAR rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_WSP(Parser.WSP rule)
		{
			return visitRules(rule.rules);
		}

		public object visit_StringValue(Parser.StringValue value)
		{
			Logger.Info(value.spelling);
			return null;
		}

		public object visit_NumericValue(Parser.NumericValue value)
		{
			Logger.Info(value.spelling);
			return null;
		}

		private object visitRules(IList<Rule> rules)
		{
			for (IEnumerator<Rule> i = rules.GetEnumerator(); i.MoveNext(); )
				i.Current.visit(this);
			return null;
		}
	}
}