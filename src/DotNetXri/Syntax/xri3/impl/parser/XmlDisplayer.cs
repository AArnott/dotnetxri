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
    System.Console.WriteLine("<xri>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri>");

    return Boolean.FALSE;
  }

  public Object visit_xri_scheme(Parser.xri_scheme rule)
  {
    System.Console.WriteLine("<xri-scheme>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-scheme>");

    return Boolean.FALSE;
  }

  public Object visit_xri_noscheme(Parser.xri_noscheme rule)
  {
    System.Console.WriteLine("<xri-noscheme>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-noscheme>");

    return Boolean.FALSE;
  }

  public Object visit_xri_reference(Parser.xri_reference rule)
  {
    System.Console.WriteLine("<xri-reference>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-reference>");

    return Boolean.FALSE;
  }

  public Object visit_relative_xri_ref(Parser.relative_xri_ref rule)
  {
    System.Console.WriteLine("<relative-xri-ref>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</relative-xri-ref>");

    return Boolean.FALSE;
  }

  public Object visit_relative_xri_part(Parser.relative_xri_part rule)
  {
    System.Console.WriteLine("<relative-xri-part>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</relative-xri-part>");

    return Boolean.FALSE;
  }

  public Object visit_xri_hier_part(Parser.xri_hier_part rule)
  {
    System.Console.WriteLine("<xri-hier-part>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-hier-part>");

    return Boolean.FALSE;
  }

  public Object visit_xri_authority(Parser.xri_authority rule)
  {
    System.Console.WriteLine("<xri-authority>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-authority>");

    return Boolean.FALSE;
  }

  public Object visit_subseg(Parser.subseg rule)
  {
    System.Console.WriteLine("<subseg>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</subseg>");

    return Boolean.FALSE;
  }

  public Object visit_global_subseg(Parser.global_subseg rule)
  {
    System.Console.WriteLine("<global-subseg>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</global-subseg>");

    return Boolean.FALSE;
  }

  public Object visit_local_subseg(Parser.local_subseg rule)
  {
    System.Console.WriteLine("<local-subseg>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</local-subseg>");

    return Boolean.FALSE;
  }

  public Object visit_gcs_char(Parser.gcs_char rule)
  {
    System.Console.WriteLine("<gcs-char>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</gcs-char>");

    return Boolean.FALSE;
  }

  public Object visit_lcs_char(Parser.lcs_char rule)
  {
    System.Console.WriteLine("<lcs-char>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</lcs-char>");

    return Boolean.FALSE;
  }

  public Object visit_rel_subseg(Parser.rel_subseg rule)
  {
    System.Console.WriteLine("<rel-subseg>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</rel-subseg>");

    return Boolean.FALSE;
  }

  public Object visit_rel_subseg_nc(Parser.rel_subseg_nc rule)
  {
    System.Console.WriteLine("<rel-subseg-nc>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</rel-subseg-nc>");

    return Boolean.FALSE;
  }

  public Object visit_literal(Parser.literal rule)
  {
    System.Console.WriteLine("<literal>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</literal>");

    return Boolean.FALSE;
  }

  public Object visit_literal_nc(Parser.literal_nc rule)
  {
    System.Console.WriteLine("<literal-nc>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</literal-nc>");

    return Boolean.FALSE;
  }

  public Object visit_xref(Parser.xref rule)
  {
    System.Console.WriteLine("<xref>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xref>");

    return Boolean.FALSE;
  }

  public Object visit_xref_empty(Parser.xref_empty rule)
  {
    System.Console.WriteLine("<xref-empty>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xref-empty>");

    return Boolean.FALSE;
  }

  public Object visit_xref_xri_reference(Parser.xref_xri_reference rule)
  {
    System.Console.WriteLine("<xref-xri-reference>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xref-xri-reference>");

    return Boolean.FALSE;
  }

  public Object visit_xref_IRI(Parser.xref_IRI rule)
  {
    System.Console.WriteLine("<xref-IRI>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xref-IRI>");

    return Boolean.FALSE;
  }

  public Object visit_xref_value(Parser.xref_value rule)
  {
    System.Console.WriteLine("<xref-value>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xref-value>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path(Parser.xri_path rule)
  {
    System.Console.WriteLine("<xri-path>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-path>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path_abempty(Parser.xri_path_abempty rule)
  {
    System.Console.WriteLine("<xri-path-abempty>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-path-abempty>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path_abs(Parser.xri_path_abs rule)
  {
    System.Console.WriteLine("<xri-path-abs>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-path-abs>");

    return Boolean.FALSE;
  }

  public Object visit_xri_path_noscheme(Parser.xri_path_noscheme rule)
  {
    System.Console.WriteLine("<xri-path-noscheme>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-path-noscheme>");

    return Boolean.FALSE;
  }

  public Object visit_xri_segment(Parser.xri_segment rule)
  {
    System.Console.WriteLine("<xri-segment>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-segment>");

    return Boolean.FALSE;
  }

  public Object visit_xri_segment_nz(Parser.xri_segment_nz rule)
  {
    System.Console.WriteLine("<xri-segment-nz>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-segment-nz>");

    return Boolean.FALSE;
  }

  public Object visit_xri_segment_nc(Parser.xri_segment_nc rule)
  {
    System.Console.WriteLine("<xri-segment-nc>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-segment-nc>");

    return Boolean.FALSE;
  }

  public Object visit_xri_pchar(Parser.xri_pchar rule)
  {
    System.Console.WriteLine("<xri-pchar>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-pchar>");

    return Boolean.FALSE;
  }

  public Object visit_xri_pchar_nc(Parser.xri_pchar_nc rule)
  {
    System.Console.WriteLine("<xri-pchar-nc>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-pchar-nc>");

    return Boolean.FALSE;
  }

  public Object visit_xri_reserved(Parser.xri_reserved rule)
  {
    System.Console.WriteLine("<xri-reserved>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-reserved>");

    return Boolean.FALSE;
  }

  public Object visit_xri_gen_delims(Parser.xri_gen_delims rule)
  {
    System.Console.WriteLine("<xri-gen-delims>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-gen-delims>");

    return Boolean.FALSE;
  }

  public Object visit_xri_sub_delims(Parser.xri_sub_delims rule)
  {
    System.Console.WriteLine("<xri-sub-delims>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</xri-sub-delims>");

    return Boolean.FALSE;
  }

  public Object visit_IRI(Parser.IRI rule)
  {
    System.Console.WriteLine("<IRI>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</IRI>");

    return Boolean.FALSE;
  }

  public Object visit_scheme(Parser.scheme rule)
  {
    System.Console.WriteLine("<scheme>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</scheme>");

    return Boolean.FALSE;
  }

  public Object visit_ihier_part(Parser.ihier_part rule)
  {
    System.Console.WriteLine("<ihier-part>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ihier-part>");

    return Boolean.FALSE;
  }

  public Object visit_iauthority(Parser.iauthority rule)
  {
    System.Console.WriteLine("<iauthority>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</iauthority>");

    return Boolean.FALSE;
  }

  public Object visit_iuserinfo(Parser.iuserinfo rule)
  {
    System.Console.WriteLine("<iuserinfo>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</iuserinfo>");

    return Boolean.FALSE;
  }

  public Object visit_ihost(Parser.ihost rule)
  {
    System.Console.WriteLine("<ihost>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ihost>");

    return Boolean.FALSE;
  }

  public Object visit_IP_literal(Parser.IP_literal rule)
  {
    System.Console.WriteLine("<IP-literal>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</IP-literal>");

    return Boolean.FALSE;
  }

  public Object visit_IPvFuture(Parser.IPvFuture rule)
  {
    System.Console.WriteLine("<IPvFuture>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</IPvFuture>");

    return Boolean.FALSE;
  }

  public Object visit_IPv6address(Parser.IPv6address rule)
  {
    System.Console.WriteLine("<IPv6address>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</IPv6address>");

    return Boolean.FALSE;
  }

  public Object visit_ls32(Parser.ls32 rule)
  {
    System.Console.WriteLine("<ls32>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ls32>");

    return Boolean.FALSE;
  }

  public Object visit_h16(Parser.h16 rule)
  {
    System.Console.WriteLine("<h16>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</h16>");

    return Boolean.FALSE;
  }

  public Object visit_IPv4address(Parser.IPv4address rule)
  {
    System.Console.WriteLine("<IPv4address>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</IPv4address>");

    return Boolean.FALSE;
  }

  public Object visit_dec_octet(Parser.dec_octet rule)
  {
    System.Console.WriteLine("<dec-octet>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</dec-octet>");

    return Boolean.FALSE;
  }

  public Object visit_ireg_name(Parser.ireg_name rule)
  {
    System.Console.WriteLine("<ireg-name>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ireg-name>");

    return Boolean.FALSE;
  }

  public Object visit_port(Parser.port rule)
  {
    System.Console.WriteLine("<port>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</port>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_abempty(Parser.ipath_abempty rule)
  {
    System.Console.WriteLine("<ipath-abempty>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ipath-abempty>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_abs(Parser.ipath_abs rule)
  {
    System.Console.WriteLine("<ipath-abs>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ipath-abs>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_rootless(Parser.ipath_rootless rule)
  {
    System.Console.WriteLine("<ipath-rootless>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ipath-rootless>");

    return Boolean.FALSE;
  }

  public Object visit_ipath_empty(Parser.ipath_empty rule)
  {
    System.Console.WriteLine("<ipath-empty>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ipath-empty>");

    return Boolean.FALSE;
  }

  public Object visit_isegment(Parser.isegment rule)
  {
    System.Console.WriteLine("<isegment>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</isegment>");

    return Boolean.FALSE;
  }

  public Object visit_isegment_nz(Parser.isegment_nz rule)
  {
    System.Console.WriteLine("<isegment-nz>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</isegment-nz>");

    return Boolean.FALSE;
  }

  public Object visit_iquery(Parser.iquery rule)
  {
    System.Console.WriteLine("<iquery>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</iquery>");

    return Boolean.FALSE;
  }

  public Object visit_iprivate(Parser.iprivate rule)
  {
    System.Console.WriteLine("<iprivate>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</iprivate>");

    return Boolean.FALSE;
  }

  public Object visit_ifragment(Parser.ifragment rule)
  {
    System.Console.WriteLine("<ifragment>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ifragment>");

    return Boolean.FALSE;
  }

  public Object visit_ipchar(Parser.ipchar rule)
  {
    System.Console.WriteLine("<ipchar>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ipchar>");

    return Boolean.FALSE;
  }

  public Object visit_iunreserved(Parser.iunreserved rule)
  {
    System.Console.WriteLine("<iunreserved>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</iunreserved>");

    return Boolean.FALSE;
  }

  public Object visit_pct_encoded(Parser.pct_encoded rule)
  {
    System.Console.WriteLine("<pct-encoded>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</pct-encoded>");

    return Boolean.FALSE;
  }

  public Object visit_ucschar(Parser.ucschar rule)
  {
    System.Console.WriteLine("<ucschar>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ucschar>");

    return Boolean.FALSE;
  }

  public Object visit_reserved(Parser.reserved rule)
  {
    System.Console.WriteLine("<reserved>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</reserved>");

    return Boolean.FALSE;
  }

  public Object visit_gen_delims(Parser.gen_delims rule)
  {
    System.Console.WriteLine("<gen-delims>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</gen-delims>");

    return Boolean.FALSE;
  }

  public Object visit_sub_delims(Parser.sub_delims rule)
  {
    System.Console.WriteLine("<sub-delims>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</sub-delims>");

    return Boolean.FALSE;
  }

  public Object visit_unreserved(Parser.unreserved rule)
  {
    System.Console.WriteLine("<unreserved>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</unreserved>");

    return Boolean.FALSE;
  }

  public Object visit_ALPHA(Parser.ALPHA rule)
  {
    System.Console.WriteLine("<ALPHA>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</ALPHA>");

    return Boolean.FALSE;
  }

  public Object visit_BIT(Parser.BIT rule)
  {
    System.Console.WriteLine("<BIT>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</BIT>");

    return Boolean.FALSE;
  }

  public Object visit_CHAR(Parser.CHAR rule)
  {
    System.Console.WriteLine("<CHAR>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</CHAR>");

    return Boolean.FALSE;
  }

  public Object visit_CR(Parser.CR rule)
  {
    System.Console.WriteLine("<CR>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</CR>");

    return Boolean.FALSE;
  }

  public Object visit_CRLF(Parser.CRLF rule)
  {
    System.Console.WriteLine("<CRLF>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</CRLF>");

    return Boolean.FALSE;
  }

  public Object visit_CTL(Parser.CTL rule)
  {
    System.Console.WriteLine("<CTL>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</CTL>");

    return Boolean.FALSE;
  }

  public Object visit_DIGIT(Parser.DIGIT rule)
  {
    System.Console.WriteLine("<DIGIT>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</DIGIT>");

    return Boolean.FALSE;
  }

  public Object visit_DQUOTE(Parser.DQUOTE rule)
  {
    System.Console.WriteLine("<DQUOTE>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</DQUOTE>");

    return Boolean.FALSE;
  }

  public Object visit_HEXDIG(Parser.HEXDIG rule)
  {
    System.Console.WriteLine("<HEXDIG>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</HEXDIG>");

    return Boolean.FALSE;
  }

  public Object visit_HTAB(Parser.HTAB rule)
  {
    System.Console.WriteLine("<HTAB>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</HTAB>");

    return Boolean.FALSE;
  }

  public Object visit_LF(Parser.LF rule)
  {
    System.Console.WriteLine("<LF>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</LF>");

    return Boolean.FALSE;
  }

  public Object visit_LWSP(Parser.LWSP rule)
  {
    System.Console.WriteLine("<LWSP>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</LWSP>");

    return Boolean.FALSE;
  }

  public Object visit_OCTET(Parser.OCTET rule)
  {
    System.Console.WriteLine("<OCTET>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</OCTET>");

    return Boolean.FALSE;
  }

  public Object visit_SP(Parser.SP rule)
  {
    System.Console.WriteLine("<SP>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</SP>");

    return Boolean.FALSE;
  }

  public Object visit_VCHAR(Parser.VCHAR rule)
  {
    System.Console.WriteLine("<VCHAR>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</VCHAR>");

    return Boolean.FALSE;
  }

  public Object visit_WSP(Parser.WSP rule)
  {
    System.Console.WriteLine("<WSP>");
    if (visitRules(rule.rules).booleanValue()) System.Console.WriteLine("");
    System.Console.WriteLine("</WSP>");

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
