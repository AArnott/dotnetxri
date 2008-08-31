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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetXri.Client.Http {
	/*
	********************************************************************************
	* Class: BaseFetcher
	********************************************************************************
	*/
	/**
 * This class provides an abstract base class for X2RFetcher and Resolver.  It
 * provides members for storing SSL and redirect information.
 * @author =chetan
 */
	public abstract class BaseFetcher {
		public const String HTTP = "http";
		public const String HTTPS = "https";

		/**
		 * the maximum number of HTTP redirects to follow. Zero means not following any HTTP redirect.
		 */
		protected int maxHttpRedirects = 5;
		/**
		 * the SSL socket factory to use
		 */
		protected SSLSocketFactory moSocketFactory = null;

		/*
		****************************************************************************
		* Constructor()
		****************************************************************************
		*/
		/**
	 * Default constructor
	 */
		protected BaseFetcher() { } // Constructor()

		/*
		****************************************************************************
		* getSSLSocketFactory()
		****************************************************************************
		*/
		/**
	 * Gets the SSL SocketFactory being used
	 */
		public SSLSocketFactory getSSLSocketFactory() {
			return moSocketFactory;

		} // getSSLSocketFactory()

		/*
		****************************************************************************
		* setSSLSocketFactory()
		****************************************************************************
		*/
		/**
	 * Sets the SSL SocketFactory to use
	 */
		public void setSSLSocketFactory(SSLSocketFactory oSocketFactory) {
			moSocketFactory = oSocketFactory;

		} // setSSLSocketFactory()

		/*
		****************************************************************************
		* getFollowRedirects()
		****************************************************************************
		*/
		/**
	 * Returns the number of redirects to follow. -breakiterator
	 * 0 or less indicates no redirects.
	 */
		public int getFollowRedirects() {
			return maxHttpRedirects;

		} // getFollowRedirects()

		/*
		****************************************************************************
		* setFollowRedirects()
		****************************************************************************
		*/
		/**
	 * Sets the number of redirects to follow.  -breakiterator
	 * 0 or less indicates no redirects.
	 */
		public void setMaxHttpRedirects(int nFollowRedirects) {
			maxHttpRedirects = nFollowRedirects;

		} // setFollowRedirects()

	} // Class: BaseFetcher
}
