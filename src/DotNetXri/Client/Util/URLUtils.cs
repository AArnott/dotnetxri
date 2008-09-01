/**
 * 
 */
package org.openxri.util;

using java.io.ByteArrayOutputStream;
using java.io.UnsupportedEncodingException;

using org.openxri.IRIUtils;

/**
 * @author =wil
 *
 */
public class URLUtils {


	/**
	 * This is the same as java.net.URLDecode(s, "UTF-8") except
	 * that '+' is not decoded to ' ' (space).
	 * 
	 * @see java.net.URLDecoder
	 * @param s
	 */
	public static String decode(String s)
	{
		if (s == null)
			return null;

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		for (int i = 0; i < s.length(); i++) {
			char c = s.charAt(i);
			if (c != '%') {
				out.write(c);
				continue;
			}
			
			out.write(IRIUtils.decodeHex(s, i));
			i += 2;
		}
		
		try {
			return out.toString("UTF-8");			
		}
		catch (UnsupportedEncodingException e) {
			// should not happen
			return null;
		}
	}
}