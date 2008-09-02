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

namespace DotNetXri.Syntax.Xri3.Impl.Parser
{
	public interface Visitor
	{
		void visit(Rule rule);

		object visit_xri(Parser.xri rule);
		object visit_xri_scheme(Parser.xri_scheme rule);
		object visit_xri_noscheme(Parser.xri_noscheme rule);
		object visit_xri_reference(Parser.xri_reference rule);
		object visit_relative_xri_ref(Parser.relative_xri_ref rule);
		object visit_relative_xri_part(Parser.relative_xri_part rule);
		object visit_xri_hier_part(Parser.xri_hier_part rule);
		object visit_xri_authority(Parser.xri_authority rule);
		object visit_subseg(Parser.subseg rule);
		object visit_global_subseg(Parser.global_subseg rule);
		object visit_local_subseg(Parser.local_subseg rule);
		object visit_gcs_char(Parser.gcs_char rule);
		object visit_lcs_char(Parser.lcs_char rule);
		object visit_rel_subseg(Parser.rel_subseg rule);
		object visit_rel_subseg_nc(Parser.rel_subseg_nc rule);
		object visit_literal(Parser.literal rule);
		object visit_literal_nc(Parser.literal_nc rule);
		object visit_xref(Parser.xref rule);
		object visit_xref_empty(Parser.xref_empty rule);
		object visit_xref_xri_reference(Parser.xref_xri_reference rule);
		object visit_xref_IRI(Parser.xref_IRI rule);
		object visit_xref_value(Parser.xref_value rule);
		object visit_xri_path(Parser.xri_path rule);
		object visit_xri_path_abempty(Parser.xri_path_abempty rule);
		object visit_xri_path_abs(Parser.xri_path_abs rule);
		object visit_xri_path_noscheme(Parser.xri_path_noscheme rule);
		object visit_xri_segment(Parser.xri_segment rule);
		object visit_xri_segment_nz(Parser.xri_segment_nz rule);
		object visit_xri_segment_nc(Parser.xri_segment_nc rule);
		object visit_xri_pchar(Parser.xri_pchar rule);
		object visit_xri_pchar_nc(Parser.xri_pchar_nc rule);
		object visit_xri_reserved(Parser.xri_reserved rule);
		object visit_xri_gen_delims(Parser.xri_gen_delims rule);
		object visit_xri_sub_delims(Parser.xri_sub_delims rule);
		object visit_IRI(Parser.IRI rule);
		object visit_scheme(Parser.scheme rule);
		object visit_ihier_part(Parser.ihier_part rule);
		object visit_iauthority(Parser.iauthority rule);
		object visit_iuserinfo(Parser.iuserinfo rule);
		object visit_ihost(Parser.ihost rule);
		object visit_IP_literal(Parser.IP_literal rule);
		object visit_IPvFuture(Parser.IPvFuture rule);
		object visit_IPv6address(Parser.IPv6address rule);
		object visit_ls32(Parser.ls32 rule);
		object visit_h16(Parser.h16 rule);
		object visit_IPv4address(Parser.IPv4address rule);
		object visit_dec_octet(Parser.dec_octet rule);
		object visit_ireg_name(Parser.ireg_name rule);
		object visit_port(Parser.port rule);
		object visit_ipath_abempty(Parser.ipath_abempty rule);
		object visit_ipath_abs(Parser.ipath_abs rule);
		object visit_ipath_rootless(Parser.ipath_rootless rule);
		object visit_ipath_empty(Parser.ipath_empty rule);
		object visit_isegment(Parser.isegment rule);
		object visit_isegment_nz(Parser.isegment_nz rule);
		object visit_iquery(Parser.iquery rule);
		object visit_iprivate(Parser.iprivate rule);
		object visit_ifragment(Parser.ifragment rule);
		object visit_ipchar(Parser.ipchar rule);
		object visit_iunreserved(Parser.iunreserved rule);
		object visit_pct_encoded(Parser.pct_encoded rule);
		object visit_ucschar(Parser.ucschar rule);
		object visit_reserved(Parser.reserved rule);
		object visit_gen_delims(Parser.gen_delims rule);
		object visit_sub_delims(Parser.sub_delims rule);
		object visit_unreserved(Parser.unreserved rule);
		object visit_ALPHA(Parser.ALPHA rule);
		object visit_BIT(Parser.BIT rule);
		object visit_CHAR(Parser.CHAR rule);
		object visit_CR(Parser.CR rule);
		object visit_CRLF(Parser.CRLF rule);
		object visit_CTL(Parser.CTL rule);
		object visit_DIGIT(Parser.DIGIT rule);
		object visit_DQUOTE(Parser.DQUOTE rule);
		object visit_HEXDIG(Parser.HEXDIG rule);
		object visit_HTAB(Parser.HTAB rule);
		object visit_LF(Parser.LF rule);
		object visit_LWSP(Parser.LWSP rule);
		object visit_OCTET(Parser.OCTET rule);
		object visit_SP(Parser.SP rule);
		object visit_VCHAR(Parser.VCHAR rule);
		object visit_WSP(Parser.WSP rule);
		object visit_StringValue(Parser.StringValue value);
		object visit_NumericValue(Parser.NumericValue value);
	}
}