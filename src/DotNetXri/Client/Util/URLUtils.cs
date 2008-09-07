namespace DotNetXri.Client.Util {
	using System.Text;
	using DotNetXri.Syntax;

	public class URLUtils {
		/// <summary>
		/// This is the same as java.net.URLDecode(s, "UTF-8") except
		/// that '+' is not decoded to ' ' (space).
		/// </summary>
		/// <author>=wil</author>
		public static string decode(string s) {
			if (s == null)
				return null;

			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < s.Length; i++) {
				char c = s[i];
				if (c != '%') {
					builder.Append(c);
					continue;
				}

				builder.Append(IRIUtils.decodeHex(s, i));
				i += 2;
			}

			return builder.ToString();
		}
	}
}