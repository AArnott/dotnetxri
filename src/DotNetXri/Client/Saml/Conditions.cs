/*
 * Copyright 2005 OpenXRI Foundation
 * Subsequently ported and altered by Andrew Arnott
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
namespace DotNetXri.Client.Saml {
	//using java.text.ParseException;
	//using java.util.Date;

	//using org.openxri.util.DOMUtils;
	//using org.openxri.xml.Tags;
	//using org.w3c.dom.XmlDocument;
	//using org.w3c.dom.XmlElement;
	using System;
	using DotNetXri.Loggers;


	/*
	********************************************************************************
	* Class: Conditions
	********************************************************************************
	*/
	/**
 * This class provides encapsulation of a SAML 2.0 Conditions element
 * @author =chetan
 */
	public class Conditions {
		private static ILog soLog = Logger.Create(typeof(Conditions));
		private DateTime? moNotBefore = null;
		private DateTime? moNotAfter = null;

		/*
		****************************************************************************
		* Constructor()
		****************************************************************************
		*/
		/**
	 * Creates an empty SAML conditions element
	 */
		public Conditions() { } // Constructor()

		/*
		****************************************************************************
		* Constructor()
		****************************************************************************
		*/
		/**
	 *  This method populates the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
		public Conditions(XmlElement oElem) {
			fromDOM(oElem);

		} // Constructor()

		/*
		****************************************************************************
		* ToString()
		****************************************************************************
		*/
		/**
	 * Returns formatted obj.  Do not use if signature needs to be preserved.
	 */
		public override string ToString() {
			return dump("");

		} // ToString()

		/*
		****************************************************************************
		* dump()
		****************************************************************************
		*/
		/**
	 * Returns obj as a formatted XML string.
	 * @param sTab - The characters to prepend before each new line
	 */
		public string dump(string sTab) {
			return "";

			// TODO Auto-generated

		} // dump()

		/*
		****************************************************************************
		* reset()
		****************************************************************************
		*/
		/**
	 * Resets the internal state of this obj
	 */
		public void reset() {
			moNotBefore = null;
			moNotAfter = null;

		} // reset()

		/*
		****************************************************************************
		* fromDOM()
		****************************************************************************
		*/
		/**
	 *  This method populates the obj from DOM.  It does not keep a
	 * copy of the DOM around.  Whitespace information is lost in this process.
	 */
		public void fromDOM(XmlElement oElem) {
			reset();

			// get the notbefore attribute
			if (oElem.hasAttributeNS(null, Tags.ATTR_NOTBEFORE)) {
				string sVal = oElem.getAttributeNS(null, Tags.ATTR_NOTBEFORE);
				try {
					moNotBefore = DOMUtils.fromXMLDateTime(sVal);
				} catch (ParseException oEx) {
					soLog.warn("Caught exception on notBefore time", oEx);
				}
			}

			// get the notAfter attribute
			if (oElem.hasAttributeNS(null, Tags.ATTR_NOTONORAFTER)) {
				string sVal = oElem.getAttributeNS(null, Tags.ATTR_NOTONORAFTER);
				try {
					moNotAfter = DOMUtils.fromXMLDateTime(sVal);
				} catch (ParseException oEx) {
					soLog.warn("Caught exception on notAfter time", oEx);
				}
			}

		} // fromDOM()

		/*
		****************************************************************************
		* isValid()
		****************************************************************************
		*/
		/**
	 * Returns true if the current time is within the notBefore and NotOnOrAfter
	 * attributes.
	 */
		public bool isValid() {
			DateTime oNow = DateTime.Now;
			if ((moNotBefore != null) && (oNow < moNotBefore)) {
				return false;
			}

			if ((moNotAfter != null) && (oNow > moNotAfter)) {
				return false;
			}

			return true;

		} // isValid()

		/*
		****************************************************************************
		* toDOM()
		****************************************************************************
		*/
		/**
	 *  This method will make DOM using the specified document.  If any DOM state
	 * has been stored with the obj, it will not be used in this method.
	 * This method generates a reference-free copy of new DOM.
	 * @param oDoc - The document to use for generating DOM
	 */
		public XmlElement toDOM(XmlDocument oDoc) {
			// for this particular toDOM implementation, oDoc must not be null
			if (oDoc == null) {
				return null;
			}

			XmlElement oElem = oDoc.createElementNS(Tags.NS_SAML, Tags.TAG_CONDITIONS);

			return oElem;

		} // toDOM()

		/*
		****************************************************************************
		* getNotAfter()
		****************************************************************************
		*/
		/**
	 * Returns the notOnOrAfter attribute
	 */
		public DateTime? getNotAfter() {
			return moNotAfter;

		} // getNotAfter()

		/*
		****************************************************************************
		* setNotAfter()
		****************************************************************************
		*/
		/**
	 * Sets the notOnOrAfter attribute
	 */
		public void setNotAfter(DateTime? oVal) {
			moNotAfter = oVal;

		} // setNotAfter()

		/*
		****************************************************************************
		* getNotBefore()
		****************************************************************************
		*/
		/**
	 * Returns the notBefore attribute
	 */
		public DateTime? getNotBefore() {
			return moNotBefore;

		} // getNotBefore()

		/*
		****************************************************************************
		* setNotBefore()
		****************************************************************************
		*/
		/**
	 * Sets the notBefore attribute
	 */
		public void setNotBefore(DateTime? oVal) {
			moNotBefore = oVal;

		} // setNotBefore()

	} // Class: Conditions
}