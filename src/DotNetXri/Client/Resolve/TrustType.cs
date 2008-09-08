namespace DotNetXri.Client.Resolve {

	using DotNetXri.Client.Resolve.Exception;
	/**
	 * @author wtan
	 *
	 */
	public class TrustType : Serializable {
		public const string TRUST_NONE = "none";
		public const string TRUST_SAML = "saml";
		public const string TRUST_HTTPS = "https";
		public const string TRUST_SAML_HTTPS = "saml+https";
		public const string TRUST = "trust";

		protected string type = TRUST_NONE;

		/**
		 * Constructor. Creates a TrustType
		 * @param type
		 */
		public TrustType() {
		}

		/**
		 * Constructor. Creates a TrustType
		 * @param type
		 */
		public TrustType(string type)
			//throws IllegalTrustTypeException
		{
			setType(type);
		}

		/**
		 * @return Returns the type as a string.
		 */
		public string getType() {
			return type;
		}

		/**
		 * @return Returns a string representation of the parameter name value pair.
		 */
		public string getParameterPair() {
			string https = (type.Equals(TRUST_HTTPS) || type.Equals(TRUST_SAML_HTTPS)) ? "true" : "false";
			string saml = (type.Equals(TRUST_SAML) || type.Equals(TRUST_SAML_HTTPS)) ? "true" : "false";
			return MimeType.PARAM_HTTPS + "=" + https + ";" + MimeType.PARAM_SAML + "=" + saml;
		}

		public void setParameterPair(bool isHttps, bool isSaml) {
			if (isHttps) {
				if (isSaml)
					type = TRUST_SAML_HTTPS;
				else
					type = TRUST_HTTPS;
			} else {
				if (isSaml)
					type = TRUST_SAML;
				else
					type = TRUST_NONE;
			}
		}

		/**
		 * @param type The type to set.
		 */
		public void setType(string type)
			//throws IllegalTrustTypeException
		{
			type = type.ToLowerInvariant();
			if ((!type.Equals(TRUST_NONE))
					&& !type.Equals(TRUST_SAML)
					&& !type.Equals(TRUST_HTTPS)
					&& !type.Equals(TRUST_SAML_HTTPS)) {
				throw new IllegalTrustTypeException(type);
			}

			this.type = type;
		}

		/**
		 * @return Returns <code>true</code> if the type is <code>https</code> or <code>saml+https</code>, <code>false</code> otherwise. 
		 */
		public bool isHTTPS() {
			return (type.Equals(TRUST_HTTPS) || type.Equals(TRUST_SAML_HTTPS));
		}

		/**
		 * @return Returns <code>true</code> if the type is <code>saml</code> or <code>saml+https</code>, <code>false</code> otherwise.
		 */
		public bool isSAML() {
			return (type.Equals(TRUST_SAML) || type.Equals(TRUST_SAML_HTTPS));
		}

		/**
		 * Compares with a string representation of the trust type.
		 * @param trustType
		 */
		public bool Equals(string trustType) {
			return this.type.Equals(trustType, System.StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString() {
			return getType();
		}
	}
}