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

using System.Collections.Generic;

namespace DotNetXri.Syntax.Xri3.Impl.Parser
{
	public class XmlDisplayer : Visitor
	{
		public void visit(Rule rule)
		{
			rule.visit(this);
		}

		public object visit_xri(Parser.xri rule)
		{
			Logger.Info("<xri>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri>");

			return false;
		}

		public object visit_xri_scheme(Parser.xri_scheme rule)
		{
			Logger.Info("<xri-scheme>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-scheme>");

			return false;
		}

		public object visit_xri_noscheme(Parser.xri_noscheme rule)
		{
			Logger.Info("<xri-noscheme>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-noscheme>");

			return false;
		}

		public object visit_xri_reference(Parser.xri_reference rule)
		{
			Logger.Info("<xri-reference>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-reference>");

			return false;
		}

		public object visit_relative_xri_ref(Parser.relative_xri_ref rule)
		{
			Logger.Info("<relative-xri-ref>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</relative-xri-ref>");

			return false;
		}

		public object visit_relative_xri_part(Parser.relative_xri_part rule)
		{
			Logger.Info("<relative-xri-part>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</relative-xri-part>");

			return false;
		}

		public object visit_xri_hier_part(Parser.xri_hier_part rule)
		{
			Logger.Info("<xri-hier-part>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-hier-part>");

			return false;
		}

		public object visit_xri_authority(Parser.xri_authority rule)
		{
			Logger.Info("<xri-authority>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-authority>");

			return false;
		}

		public object visit_subseg(Parser.subseg rule)
		{
			Logger.Info("<subseg>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</subseg>");

			return false;
		}

		public object visit_global_subseg(Parser.global_subseg rule)
		{
			Logger.Info("<global-subseg>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</global-subseg>");

			return false;
		}

		public object visit_local_subseg(Parser.local_subseg rule)
		{
			Logger.Info("<local-subseg>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</local-subseg>");

			return false;
		}

		public object visit_gcs_char(Parser.gcs_char rule)
		{
			Logger.Info("<gcs-char>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</gcs-char>");

			return false;
		}

		public object visit_lcs_char(Parser.lcs_char rule)
		{
			Logger.Info("<lcs-char>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</lcs-char>");

			return false;
		}

		public object visit_rel_subseg(Parser.rel_subseg rule)
		{
			Logger.Info("<rel-subseg>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</rel-subseg>");

			return false;
		}

		public object visit_rel_subseg_nc(Parser.rel_subseg_nc rule)
		{
			Logger.Info("<rel-subseg-nc>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</rel-subseg-nc>");

			return false;
		}

		public object visit_literal(Parser.literal rule)
		{
			Logger.Info("<literal>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</literal>");

			return false;
		}

		public object visit_literal_nc(Parser.literal_nc rule)
		{
			Logger.Info("<literal-nc>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</literal-nc>");

			return false;
		}

		public object visit_xref(Parser.xref rule)
		{
			Logger.Info("<xref>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xref>");

			return false;
		}

		public object visit_xref_empty(Parser.xref_empty rule)
		{
			Logger.Info("<xref-empty>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xref-empty>");

			return false;
		}

		public object visit_xref_xri_reference(Parser.xref_xri_reference rule)
		{
			Logger.Info("<xref-xri-reference>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xref-xri-reference>");

			return false;
		}

		public object visit_xref_IRI(Parser.xref_IRI rule)
		{
			Logger.Info("<xref-IRI>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xref-IRI>");

			return false;
		}

		public object visit_xref_value(Parser.xref_value rule)
		{
			Logger.Info("<xref-value>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xref-value>");

			return false;
		}

		public object visit_xri_path(Parser.xri_path rule)
		{
			Logger.Info("<xri-path>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-path>");

			return false;
		}

		public object visit_xri_path_abempty(Parser.xri_path_abempty rule)
		{
			Logger.Info("<xri-path-abempty>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-path-abempty>");

			return false;
		}

		public object visit_xri_path_abs(Parser.xri_path_abs rule)
		{
			Logger.Info("<xri-path-abs>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-path-abs>");

			return false;
		}

		public object visit_xri_path_noscheme(Parser.xri_path_noscheme rule)
		{
			Logger.Info("<xri-path-noscheme>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-path-noscheme>");

			return false;
		}

		public object visit_xri_segment(Parser.xri_segment rule)
		{
			Logger.Info("<xri-segment>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-segment>");

			return false;
		}

		public object visit_xri_segment_nz(Parser.xri_segment_nz rule)
		{
			Logger.Info("<xri-segment-nz>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-segment-nz>");

			return false;
		}

		public object visit_xri_segment_nc(Parser.xri_segment_nc rule)
		{
			Logger.Info("<xri-segment-nc>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-segment-nc>");

			return false;
		}

		public object visit_xri_pchar(Parser.xri_pchar rule)
		{
			Logger.Info("<xri-pchar>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-pchar>");

			return false;
		}

		public object visit_xri_pchar_nc(Parser.xri_pchar_nc rule)
		{
			Logger.Info("<xri-pchar-nc>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-pchar-nc>");

			return false;
		}

		public object visit_xri_reserved(Parser.xri_reserved rule)
		{
			Logger.Info("<xri-reserved>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-reserved>");

			return false;
		}

		public object visit_xri_gen_delims(Parser.xri_gen_delims rule)
		{
			Logger.Info("<xri-gen-delims>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-gen-delims>");

			return false;
		}

		public object visit_xri_sub_delims(Parser.xri_sub_delims rule)
		{
			Logger.Info("<xri-sub-delims>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</xri-sub-delims>");

			return false;
		}

		public object visit_IRI(Parser.IRI rule)
		{
			Logger.Info("<IRI>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</IRI>");

			return false;
		}

		public object visit_scheme(Parser.scheme rule)
		{
			Logger.Info("<scheme>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</scheme>");

			return false;
		}

		public object visit_ihier_part(Parser.ihier_part rule)
		{
			Logger.Info("<ihier-part>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ihier-part>");

			return false;
		}

		public object visit_iauthority(Parser.iauthority rule)
		{
			Logger.Info("<iauthority>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</iauthority>");

			return false;
		}

		public object visit_iuserinfo(Parser.iuserinfo rule)
		{
			Logger.Info("<iuserinfo>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</iuserinfo>");

			return false;
		}

		public object visit_ihost(Parser.ihost rule)
		{
			Logger.Info("<ihost>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ihost>");

			return false;
		}

		public object visit_IP_literal(Parser.IP_literal rule)
		{
			Logger.Info("<IP-literal>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</IP-literal>");

			return false;
		}

		public object visit_IPvFuture(Parser.IPvFuture rule)
		{
			Logger.Info("<IPvFuture>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</IPvFuture>");

			return false;
		}

		public object visit_IPv6address(Parser.IPv6address rule)
		{
			Logger.Info("<IPv6address>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</IPv6address>");

			return false;
		}

		public object visit_ls32(Parser.ls32 rule)
		{
			Logger.Info("<ls32>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ls32>");

			return false;
		}

		public object visit_h16(Parser.h16 rule)
		{
			Logger.Info("<h16>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</h16>");

			return false;
		}

		public object visit_IPv4address(Parser.IPv4address rule)
		{
			Logger.Info("<IPv4address>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</IPv4address>");

			return false;
		}

		public object visit_dec_octet(Parser.dec_octet rule)
		{
			Logger.Info("<dec-octet>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</dec-octet>");

			return false;
		}

		public object visit_ireg_name(Parser.ireg_name rule)
		{
			Logger.Info("<ireg-name>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ireg-name>");

			return false;
		}

		public object visit_port(Parser.port rule)
		{
			Logger.Info("<port>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</port>");

			return false;
		}

		public object visit_ipath_abempty(Parser.ipath_abempty rule)
		{
			Logger.Info("<ipath-abempty>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ipath-abempty>");

			return false;
		}

		public object visit_ipath_abs(Parser.ipath_abs rule)
		{
			Logger.Info("<ipath-abs>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ipath-abs>");

			return false;
		}

		public object visit_ipath_rootless(Parser.ipath_rootless rule)
		{
			Logger.Info("<ipath-rootless>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ipath-rootless>");

			return false;
		}

		public object visit_ipath_empty(Parser.ipath_empty rule)
		{
			Logger.Info("<ipath-empty>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ipath-empty>");

			return false;
		}

		public object visit_isegment(Parser.isegment rule)
		{
			Logger.Info("<isegment>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</isegment>");

			return false;
		}

		public object visit_isegment_nz(Parser.isegment_nz rule)
		{
			Logger.Info("<isegment-nz>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</isegment-nz>");

			return false;
		}

		public object visit_iquery(Parser.iquery rule)
		{
			Logger.Info("<iquery>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</iquery>");

			return false;
		}

		public object visit_iprivate(Parser.iprivate rule)
		{
			Logger.Info("<iprivate>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</iprivate>");

			return false;
		}

		public object visit_ifragment(Parser.ifragment rule)
		{
			Logger.Info("<ifragment>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ifragment>");

			return false;
		}

		public object visit_ipchar(Parser.ipchar rule)
		{
			Logger.Info("<ipchar>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ipchar>");

			return false;
		}

		public object visit_iunreserved(Parser.iunreserved rule)
		{
			Logger.Info("<iunreserved>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</iunreserved>");

			return false;
		}

		public object visit_pct_encoded(Parser.pct_encoded rule)
		{
			Logger.Info("<pct-encoded>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</pct-encoded>");

			return false;
		}

		public object visit_ucschar(Parser.ucschar rule)
		{
			Logger.Info("<ucschar>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ucschar>");

			return false;
		}

		public object visit_reserved(Parser.reserved rule)
		{
			Logger.Info("<reserved>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</reserved>");

			return false;
		}

		public object visit_gen_delims(Parser.gen_delims rule)
		{
			Logger.Info("<gen-delims>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</gen-delims>");

			return false;
		}

		public object visit_sub_delims(Parser.sub_delims rule)
		{
			Logger.Info("<sub-delims>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</sub-delims>");

			return false;
		}

		public object visit_unreserved(Parser.unreserved rule)
		{
			Logger.Info("<unreserved>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</unreserved>");

			return false;
		}

		public object visit_ALPHA(Parser.ALPHA rule)
		{
			Logger.Info("<ALPHA>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</ALPHA>");

			return false;
		}

		public object visit_BIT(Parser.BIT rule)
		{
			Logger.Info("<BIT>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</BIT>");

			return false;
		}

		public object visit_CHAR(Parser.CHAR rule)
		{
			Logger.Info("<CHAR>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</CHAR>");

			return false;
		}

		public object visit_CR(Parser.CR rule)
		{
			Logger.Info("<CR>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</CR>");

			return false;
		}

		public object visit_CRLF(Parser.CRLF rule)
		{
			Logger.Info("<CRLF>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</CRLF>");

			return false;
		}

		public object visit_CTL(Parser.CTL rule)
		{
			Logger.Info("<CTL>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</CTL>");

			return false;
		}

		public object visit_DIGIT(Parser.DIGIT rule)
		{
			Logger.Info("<DIGIT>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</DIGIT>");

			return false;
		}

		public object visit_DQUOTE(Parser.DQUOTE rule)
		{
			Logger.Info("<DQUOTE>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</DQUOTE>");

			return false;
		}

		public object visit_HEXDIG(Parser.HEXDIG rule)
		{
			Logger.Info("<HEXDIG>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</HEXDIG>");

			return false;
		}

		public object visit_HTAB(Parser.HTAB rule)
		{
			Logger.Info("<HTAB>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</HTAB>");

			return false;
		}

		public object visit_LF(Parser.LF rule)
		{
			Logger.Info("<LF>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</LF>");

			return false;
		}

		public object visit_LWSP(Parser.LWSP rule)
		{
			Logger.Info("<LWSP>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</LWSP>");

			return false;
		}

		public object visit_OCTET(Parser.OCTET rule)
		{
			Logger.Info("<OCTET>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</OCTET>");

			return false;
		}

		public object visit_SP(Parser.SP rule)
		{
			Logger.Info("<SP>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</SP>");

			return false;
		}

		public object visit_VCHAR(Parser.VCHAR rule)
		{
			Logger.Info("<VCHAR>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</VCHAR>");

			return false;
		}

		public object visit_WSP(Parser.WSP rule)
		{
			Logger.Info("<WSP>");
			if (visitRules(rule.rules))
				Logger.Info("");
			Logger.Info("</WSP>");

			return false;
		}

		public object visit_StringValue(Parser.StringValue value)
		{
			Logger.Info(value.spelling);
			return true;
		}

		public object visit_NumericValue(Parser.NumericValue value)
		{
			Logger.Info(value.spelling);
			return true;
		}

		private bool visitRules(IList<Rule> rules)
		{
			bool terminal = false;
			for (IEnumerator<Rule> i = rules.GetEnumerator(); i.MoveNext(); )
				terminal = (bool)i.Current.visit(this);
			return terminal;
		}
	}
}