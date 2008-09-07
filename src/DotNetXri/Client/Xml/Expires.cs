namespace DotNetXri.Client.Xml {

	using java.util.Date;

	using org.openxri.util.DOMUtils;

	public class Expires : SimpleXMLElement {
		private Date expires = null;

		public Expires(Expires e)
			: base(e) {
		}

		public Expires()
			: base(Tags.TAG_EXPIRES) {
		}

		public Expires(Date dateValue)
			: base(Tags.TAG_EXPIRES) {
			setDate(dateValue);
		}

		public Date getDate() {
			return expires;
		}

		public String getDateString() {
			if (expires == null) return "";
			return DOMUtils.toXMLDateTime(expires);
		}

		public void setDate(Date dateValue) {
			expires = dateValue;
			setValue(getDateString());

		}
	}
}