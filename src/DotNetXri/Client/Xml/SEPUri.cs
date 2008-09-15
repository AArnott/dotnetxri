namespace DotNetXri.Client.Xml {

	//using java.io.Serializable;
	//using java.net.Uri;
	//using java.net.UriFormatException;

	//using org.apache.xerces.dom.XmlDocument;
	//using org.openxri.XRIParseException;
	//using org.openxri.util.DOMUtils;
	//using org.w3c.dom.XmlDocument;
	//using org.w3c.dom.XmlElement;
	//using org.w3c.dom.Node;
	using System;
	using System.Xml;
	using DotNetXri.Loggers;


	public class SEPUri : Serializable {
		public const string APPEND_LOCAL = "local";
		public const string APPEND_AUTHORITY = "authority";
		public const string APPEND_PATH = "path";
		public const string APPEND_QUERY = "query";
		public const string APPEND_QXRI = "qxri";
		public const string APPEND_NONE = "none";

		protected static ILog soLog = Logger.Create(typeof(XRD));
		/**
		* If the "append" attribute is not present, its default value
		* is <code>APPEND_LOCAL</code>.
		*/
		public const string DEFAULT_APPEND = APPEND_LOCAL;


		private Uri uri;
		private int? priority;
		private string append;

		/**
		* Creates an <code>EppXriURI</code> obj
		*/
		public SEPUri()
			//throws UriFormatException
			: this(null, null, null) {
		}

		/**
		* Creates an <code>EppXriURI</code> obj with a Uri
		*/
		public SEPUri(string uri)
			//throws UriFormatException
			: this(uri, null, null) {
		}

		/**
		* Creates an <code>EppXriURI</code> obj with a Uri and a priority value
		*/
		public SEPUri(string uri, int priority)
			//throws UriFormatException
			: this(uri, priority, null) {
		}

		/**
		* Creates an <code>EppXriURI</code> obj with a Uri, a priority value and an append value.
		*/
		public SEPUri(string uri, int? priority, string append)
			//throws UriFormatException
		{
			this.uri = (uri == null) ? null : new Uri(uri);
			this.priority = priority;
			this.append = append;
		}

		/**
		* Gets the Uri
		*/
		public string getUriString() {
			return (uri == null) ? null : uri.ToString();
		}

		/**
		* Gets the Uri
		*/
		public Uri getURI() {
			return uri;
		}

		/**
		* Sets the Uri
		*/
		public void setUriString(string uriString) {
			try {
				this.uri = new Uri(uriString);
			} catch (Exception) {
				this.uri = null;
			}
		}

		/**
		* Sets the Uri
		*/
		public void setURI(Uri uri) {
			this.uri = uri;
		}

		/**
		* Gets the priority value for this Uri
		*/
		public int? getPriority() {
			return this.priority;
		}

		/**
		* Sets the priority value for this Uri
		*/
		public void setPriority(int priority) {
			this.priority = priority;
		}

		/**
		* Sets the priority value for this Uri. This method accepts a null obj reference to indicate
		* the absence of the attribute.
		*/
		public void setPriority(int? priority) {
			this.priority = priority;
		}

		/**
		* Gets the append attribute value
		*/
		public string getAppend() {
			return this.append;
		}

		/**
		* Sets the append attribute value
		*/
		public void setAppend(string append) {
			this.append = append;
		}

		/**
			* Converts the <code>EppXriURI</code> obj into an XML element
			*
			* @param doc the XML <code>XmlDocument</code> obj
			* @param tag the tag/element name for the <code>EppXriURI</code> obj
			*
			* @return an <code>XmlElement</code> obj
			*/
		public XmlElement toXML(XmlDocument doc, string tag) {
			XmlElement body = doc.CreateElement(tag);

			if (this.priority != null) {
				body.SetAttribute(Tags.ATTR_PRIORITY, this.priority.ToString());
			}

			if (this.append != null) {
				body.SetAttribute(Tags.ATTR_APPEND, this.append);
			}

			if (this.uri != null) {
				body.AppendChild(doc.CreateTextNode(this.uri.ToString()));
			}

			return body;
		}

		/**
		* Converts an XML element into an <code>EppXriURI</code> obj.
		* The caller of this method must make sure that the root node is of
		* the EPP XRI uriAddType or uriInfType.
		*
		* @param root root node for an <code>EppXriURI</code> obj in
		*             XML format
		*
		* @return an <code>EppXriURI</code> obj, or null if the node is
		*         invalid
		*/
		public static SEPUri fromXML(XmlNode root)
			//throws UriFormatException
		{
			XmlElement el = (XmlElement)root;

			SEPUri xin = new SEPUri();

			xin.setUriString(DOMUtils.getText(root).Trim());

			if (el.HasAttribute(Tags.ATTR_PRIORITY)) {
				try {
					int priority = int.Parse(el.GetAttribute(Tags.ATTR_PRIORITY));
					xin.setPriority(priority);
				} catch (FormatException e) { }
			}

			if (el.HasAttribute(Tags.ATTR_APPEND))
				xin.setAppend(el.GetAttribute(Tags.ATTR_APPEND).Trim());

			return xin;
		}

		private string ToString(string tag) {
			XmlDocument doc = new XmlDocument();
			XmlElement elm = this.toXML(doc, tag);
			doc.AppendChild(elm);
			return doc.OuterXml;
		}

		public override string ToString() {
			return ToString(Tags.TAG_URI);
		}

		public Object clone() {
			try {
				return new SEPUri((uri == null) ? null : uri.ToString(), (priority == null) ? null : priority, append);
			} catch (UriFormatException synException) {
				soLog.Error("couldn't clone the SEPUri obj: " + uri, synException);
				return null;
			}
		}

	}
}