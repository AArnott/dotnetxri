namespace DotNetXri.Syntax.Xri3.Impl {
public class XRI3Constants {

	public const String XRI_SCHEME = "xri:";

	public const String AUTHORITY_PREFIX = "";
	public const String PATH_PREFIX = "/";
	public const String QUERY_PREFIX = "?";
	public const String FRAGMENT_PREFIX = "#";

	public const String XREF_START = "(";
	public const String XREF_END = ")";

	public const XRI3 XRI_NULL = new XRI3("$null");
	
	public const char GCS_EQUALS = new char('='); 
	public const char GCS_AT = new char('@'); 
	public const char GCS_PLUS = new char('+'); 
	public const char GCS_DOLLAR = new char('$'); 

	public const char LCS_STAR = new char('*'); 
	public const char LCS_BANG = new char('!'); 

	public const char[] GCS_ARRAY = new char[] {
		GCS_EQUALS,
		GCS_AT,
		GCS_PLUS,
		GCS_DOLLAR
	};

	public const char[] LCS_ARRAY = new char[] {
		LCS_STAR,
		LCS_BANG
	};
}
}