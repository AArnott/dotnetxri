package org.openxri.xri3.impl.parser;


using java.util.ArrayList;
using java.util.Iterator;

public class XmlDisplayer : Visitor
{
  public void visit(Rule rule)
  {
    rule.visit(this);
  }

  public Object visit_xri(Parser.xri rule)
  {
    Logger.Info("<xri>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri>");

    return Boolean.FALSE;
  }

  public Object visit_xri_scheme(Parser.xri_scheme rule)
  {
    Logger.Info("<xri-scheme>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-scheme>");

    return Boolean.FALSE;
  }

  public Object visit_xri_noscheme(Parser.xri_noscheme rule)
  {
    Logger.Info("<xri-noscheme>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-noscheme>");

    return Boolean.FALSE;
  }

  public Object visit_xri_reference(Parser.xri_reference rule)
  {
    Logger.Info("<xri-reference>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-reference>");

    return Boolean.FALSE;
  }

  public Object visit_relative_xri_ref(Parser.relative_xri_ref rule)
  {
    Logger.Info("<relative-xri-ref>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</relative-xri-ref>");

    return Boolean.FALSE;
  }

  public Object visit_relative_xri_part(Parser.relative_xri_part rule)
  {
    Logger.Info("<relative-xri-part>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</relative-xri-part>");

    return Boolean.FALSE;
  }

  public Object visit_xri_hier_part(Parser.xri_hier_part rule)
  {
    Logger.Info("<xri-hier-part>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-hier-part>");

    return Boolean.FALSE;
  }

  public Object visit_xri_authority(Parser.xri_authority rule)
  {
    Logger.Info("<xri-authority>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-authority>");

    return Boolean.FALSE;
  }

  public Object visit_subseg(Parser.subseg rule)
  {
    Logger.Info("<subseg>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</subseg>");

    return Boolean.FALSE;
  }

  public Object visit_global_subseg(Parser.global_subseg rule)
  {
    Logger.Info("<global-subseg>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</global-subseg>");

    return Boolean.FALSE;
  }

  public Object visit_local_subseg(Parser.local_subseg rule)
  {
    Logger.Info("<local-subseg>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</local-subseg>");

    return Boolean.FALSE;
  }

  public Object visit_gcs_char(Parser.gcs_char rule)
  {
    Logger.Info("<gcs-char>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</gcs-char>");

    return Boolean.FALSE;
  }

  public Object visit_lcs_char(Parser.lcs_char rule)
  {
    Logger.Info("<lcs-char>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</lcs-char>");

    return Boolean.FALSE;
  }

  public Object visit_rel_subseg(Parser.rel_subseg rule)
  {
    Logger.Info("<rel-subseg>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</rel-subseg>");

    return Boolean.FALSE;
  }

  public Object visit_rel_subseg_nc(Parser.rel_subseg_nc rule)
  {
    Logger.Info("<rel-subseg-nc>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</rel-subseg-nc>");

    return Boolean.FALSE;
  }

  public Object visit_literal(Parser.literal rule)
  {
    Logger.Info("<literal>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</literal>");

    return Boolean.FALSE;
  }

  public Object visit_literal_nc(Parser.literal_nc rule)
  {
    Logger.Info("<literal-nc>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</literal-nc>");

    return Boolean.FALSE;
  }

  public Object visit_xref(Parser.xref rule)
  {
    Logger.Info("<xref>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xref>");

    return Boolean.FALSE;
  }

  public Object visit_xref_empty(Parser.xref_empty rule)
  {
    Logger.Info("<xref-empty>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xref-empty>");

    return Boolean.FALSE;
  }

  public Object visit_xref_xri_reference(Parser.xref_xri_reference rule)
  {
    Logger.Info("<xref-xri-reference>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xref-xri-reference>");

    return Boolean.FALSE;
  }

  public Object visit_xref_IRI(Parser.xref_IRI rule)
  {
    Logger.Info("<xref-IRI>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xref-IRI>");

    return Boolean.FALSE;
  }

  public Object visit_xref_value(Parser.xref_value rule)
  {
    Logger.Info("<xref-value>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xref-value>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path(Parser.xri_path rule)
  {
    Logger.Info("<xri-path>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-path>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path_abempty(Parser.xri_path_abempty rule)
  {
    Logger.Info("<xri-path-abempty>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-path-abempty>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path_abs(Parser.xri_path_abs rule)
  {
    Logger.Info("<xri-path-abs>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-path-abs>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path_noscheme(Parser.xri_path_noscheme rule)
  {
    Logger.Info("<xri-path-noscheme>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-path-noscheme>");

    return Boolean.FALSE;
  }

  public Object visit_xri_segment(Parser.xri_segment rule)
  {
    Logger.Info("<xri-segment>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-segment>");

    return Boolean.FALSE;
  }

  public Object visit_xri_segment_nz(Parser.xri_segment_nz rule)
  {
    Logger.Info("<xri-segment-nz>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-segment-nz>");

    return Boolean.FALSE;
  }

  public Object visit_xri_segment_nc(Parser.xri_segment_nc rule)
  {
    Logger.Info("<xri-segment-nc>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-segment-nc>");

    return Boolean.FALSE;
  }

  public Object visit_xri_pchar(Parser.xri_pchar rule)
  {
    Logger.Info("<xri-pchar>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-pchar>");

    return Boolean.FALSE;
  }

  public Object visit_xri_pchar_nc(Parser.xri_pchar_nc rule)
  {
    Logger.Info("<xri-pchar-nc>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-pchar-nc>");

    return Boolean.FALSE;
  }

  public Object visit_xri_reserved(Parser.xri_reserved rule)
  {
    Logger.Info("<xri-reserved>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-reserved>");

    return Boolean.FALSE;
  }

  public Object visit_xri_gen_delims(Parser.xri_gen_delims rule)
  {
    Logger.Info("<xri-gen-delims>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-gen-delims>");

    return Boolean.FALSE;
  }

  public Object visit_xri_sub_delims(Parser.xri_sub_delims rule)
  {
    Logger.Info("<xri-sub-delims>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</xri-sub-delims>");

    return Boolean.FALSE;
  }

  public Object visit_IRI(Parser.IRI rule)
  {
    Logger.Info("<IRI>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</IRI>");

    return Boolean.FALSE;
  }

  public Object visit_scheme(Parser.scheme rule)
  {
    Logger.Info("<scheme>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</scheme>");

    return Boolean.FALSE;
  }

  public Object visit_ihier_part(Parser.ihier_part rule)
  {
    Logger.Info("<ihier-part>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ihier-part>");

    return Boolean.FALSE;
  }

  public Object visit_iauthority(Parser.iauthority rule)
  {
    Logger.Info("<iauthority>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</iauthority>");

    return Boolean.FALSE;
  }

  public Object visit_iuserinfo(Parser.iuserinfo rule)
  {
    Logger.Info("<iuserinfo>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</iuserinfo>");

    return Boolean.FALSE;
  }

  public Object visit_ihost(Parser.ihost rule)
  {
    Logger.Info("<ihost>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ihost>");

    return Boolean.FALSE;
  }

  public Object visit_IP_literal(Parser.IP_literal rule)
  {
    Logger.Info("<IP-literal>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</IP-literal>");

    return Boolean.FALSE;
  }

  public Object visit_IPvFuture(Parser.IPvFuture rule)
  {
    Logger.Info("<IPvFuture>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</IPvFuture>");

    return Boolean.FALSE;
  }

  public Object visit_IPv6address(Parser.IPv6address rule)
  {
    Logger.Info("<IPv6address>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</IPv6address>");

    return Boolean.FALSE;
  }

  public Object visit_ls32(Parser.ls32 rule)
  {
    Logger.Info("<ls32>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ls32>");

    return Boolean.FALSE;
  }

  public Object visit_h16(Parser.h16 rule)
  {
    Logger.Info("<h16>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</h16>");

    return Boolean.FALSE;
  }

  public Object visit_IPv4address(Parser.IPv4address rule)
  {
    Logger.Info("<IPv4address>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</IPv4address>");

    return Boolean.FALSE;
  }

  public Object visit_dec_octet(Parser.dec_octet rule)
  {
    Logger.Info("<dec-octet>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</dec-octet>");

    return Boolean.FALSE;
  }

  public Object visit_ireg_name(Parser.ireg_name rule)
  {
    Logger.Info("<ireg-name>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ireg-name>");

    return Boolean.FALSE;
  }

  public Object visit_port(Parser.port rule)
  {
    Logger.Info("<port>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</port>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_abempty(Parser.ipath_abempty rule)
  {
    Logger.Info("<ipath-abempty>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ipath-abempty>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_abs(Parser.ipath_abs rule)
  {
    Logger.Info("<ipath-abs>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ipath-abs>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_rootless(Parser.ipath_rootless rule)
  {
    Logger.Info("<ipath-rootless>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ipath-rootless>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_empty(Parser.ipath_empty rule)
  {
    Logger.Info("<ipath-empty>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ipath-empty>");

    return Boolean.FALSE;
  }

  public Object visit_isegment(Parser.isegment rule)
  {
    Logger.Info("<isegment>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</isegment>");

    return Boolean.FALSE;
  }

  public Object visit_isegment_nz(Parser.isegment_nz rule)
  {
    Logger.Info("<isegment-nz>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</isegment-nz>");

    return Boolean.FALSE;
  }

  public Object visit_iquery(Parser.iquery rule)
  {
    Logger.Info("<iquery>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</iquery>");

    return Boolean.FALSE;
  }

  public Object visit_iprivate(Parser.iprivate rule)
  {
    Logger.Info("<iprivate>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</iprivate>");

    return Boolean.FALSE;
  }

  public Object visit_ifragment(Parser.ifragment rule)
  {
    Logger.Info("<ifragment>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ifragment>");

    return Boolean.FALSE;
  }

  public Object visit_ipchar(Parser.ipchar rule)
  {
    Logger.Info("<ipchar>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ipchar>");

    return Boolean.FALSE;
  }

  public Object visit_iunreserved(Parser.iunreserved rule)
  {
    Logger.Info("<iunreserved>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</iunreserved>");

    return Boolean.FALSE;
  }

  public Object visit_pct_encoded(Parser.pct_encoded rule)
  {
    Logger.Info("<pct-encoded>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</pct-encoded>");

    return Boolean.FALSE;
  }

  public Object visit_ucschar(Parser.ucschar rule)
  {
    Logger.Info("<ucschar>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ucschar>");

    return Boolean.FALSE;
  }

  public Object visit_reserved(Parser.reserved rule)
  {
    Logger.Info("<reserved>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</reserved>");

    return Boolean.FALSE;
  }

  public Object visit_gen_delims(Parser.gen_delims rule)
  {
    Logger.Info("<gen-delims>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</gen-delims>");

    return Boolean.FALSE;
  }

  public Object visit_sub_delims(Parser.sub_delims rule)
  {
    Logger.Info("<sub-delims>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</sub-delims>");

    return Boolean.FALSE;
  }

  public Object visit_unreserved(Parser.unreserved rule)
  {
    Logger.Info("<unreserved>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</unreserved>");

    return Boolean.FALSE;
  }

  public Object visit_ALPHA(Parser.ALPHA rule)
  {
    Logger.Info("<ALPHA>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</ALPHA>");

    return Boolean.FALSE;
  }

  public Object visit_BIT(Parser.BIT rule)
  {
    Logger.Info("<BIT>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</BIT>");

    return Boolean.FALSE;
  }

  public Object visit_CHAR(Parser.CHAR rule)
  {
    Logger.Info("<CHAR>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</CHAR>");

    return Boolean.FALSE;
  }

  public Object visit_CR(Parser.CR rule)
  {
    Logger.Info("<CR>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</CR>");

    return Boolean.FALSE;
  }

  public Object visit_CRLF(Parser.CRLF rule)
  {
    Logger.Info("<CRLF>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</CRLF>");

    return Boolean.FALSE;
  }

  public Object visit_CTL(Parser.CTL rule)
  {
    Logger.Info("<CTL>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</CTL>");

    return Boolean.FALSE;
  }

  public Object visit_DIGIT(Parser.DIGIT rule)
  {
    Logger.Info("<DIGIT>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</DIGIT>");

    return Boolean.FALSE;
  }

  public Object visit_DQUOTE(Parser.DQUOTE rule)
  {
    Logger.Info("<DQUOTE>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</DQUOTE>");

    return Boolean.FALSE;
  }

  public Object visit_HEXDIG(Parser.HEXDIG rule)
  {
    Logger.Info("<HEXDIG>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</HEXDIG>");

    return Boolean.FALSE;
  }

  public Object visit_HTAB(Parser.HTAB rule)
  {
    Logger.Info("<HTAB>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</HTAB>");

    return Boolean.FALSE;
  }

  public Object visit_LF(Parser.LF rule)
  {
    Logger.Info("<LF>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</LF>");

    return Boolean.FALSE;
  }

  public Object visit_LWSP(Parser.LWSP rule)
  {
    Logger.Info("<LWSP>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</LWSP>");

    return Boolean.FALSE;
  }

  public Object visit_OCTET(Parser.OCTET rule)
  {
    Logger.Info("<OCTET>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</OCTET>");

    return Boolean.FALSE;
  }

  public Object visit_SP(Parser.SP rule)
  {
    Logger.Info("<SP>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</SP>");

    return Boolean.FALSE;
  }

  public Object visit_VCHAR(Parser.VCHAR rule)
  {
    Logger.Info("<VCHAR>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</VCHAR>");

    return Boolean.FALSE;
  }

  public Object visit_WSP(Parser.WSP rule)
  {
    Logger.Info("<WSP>");
    if (visitRules(rule.rules).booleanValue()) Logger.Info("");
    Logger.Info("</WSP>");

    return Boolean.FALSE;
  }

  public Object visit_StringValue(Parser.StringValue value)
  {
    Logger.Info(value.spelling);
    return Boolean.TRUE;
  }

  public Object visit_NumericValue(Parser.NumericValue value)
  {
    Logger.Info(value.spelling);
    return Boolean.TRUE;
  }

  private Boolean visitRules(ArrayList rules)
  {
    Boolean terminal = Boolean.FALSE;
    for (Iterator i = rules.iterator(); i.hasNext();)
      terminal = (Boolean)((Rule)i.next()).visit(this);
    return terminal;
  }
}

/* -----------------------------------------------------------------------------
 * eof
 * -----------------------------------------------------------------------------
 */
