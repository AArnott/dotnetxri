using System.Xml;
using System;
namespace DotNetXri.Client.Xml {

	//using java.util.Date;

	//using org.openxri.util.DOMUtils;

	public class Expires : SimpleXMLElement {
		private DateTime? expires = null;

		public Expires(Expires? e)
			: base(e) {
		}

		public Expires()
			: base(Tags.TAG_EXPIRES) {
		}

		public Expires(DateTime? dateValue)
			: base(Tags.TAG_EXPIRES) {
			setDate(dateValue);
		}

		public DateTime? getDate() {
			return expires;
		}

		public string getDateString() {
			if (expires == null) return "";
			return XmlConvert.ToString(expires.Value);
		}

		public void setDate(DateTime? dateValue) {
			expires = dateValue;
			setValue(getDateString());

		}
	}
}