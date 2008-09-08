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
namespace DotNetXri.Client.Util {
	using System.Net;
	using System;
	using System.Net.Cache;
	using System.Collections.Specialized;

	/*
	********************************************************************************
	* Class: IO
	********************************************************************************
	*/
	/**
 * Provides helper methods to create a HttpURLConnection from a Uri and properties
 * associated with how to access the Uri.
 * @author =chetan
 */
	public class IO {
		protected static int DefaultMaximumRedirections = 10;
		/*
		****************************************************************************
		* getConnectionToURI()
		****************************************************************************
		*/
		/**
	 *Gets the connection to a Uri.
	 *@param oURI The Uri to access
	 *@param sMethod - The HTTP Method (Defaults to "GET")
	 *@param oReqProperties The request properties to set
	 *@param oSSLFactory The SSL Socket Factory to use if necessary
	 *@param bFollowRedirects Whether or not to follow unlimited redirects
	 *@param bUseCaches Whether or not to use the HTTP cache
	 *@return the HttpURLConnection The connection obj after calling connect()
	 */
		public static HttpWebRequest getConnectionToURI(
			Uri oURI, string sMethod, NameValueCollection oReqProperties,
			object/*SSLSocketFactory*/ oSSLFactory, bool bFollowRedirects,
			bool bUseCaches)
			//throws IOException
		{
			return getConnectionToURI(oURI, sMethod, oReqProperties, oSSLFactory,
				DefaultMaximumRedirections, bUseCaches);
		} // getConnectionToURI()

		/*
		****************************************************************************
		* getConnectionToURI()
		****************************************************************************
		*/
		/**
	 *Gets the connection to a Uri.
	 *@param oURI The Uri to access
	 *@param sMethod - The HTTP Method (Defaults to "GET")
	 *@param oReqProperties The request properties to set
	 *@param oSSLFactory The SSL Socket Factory to use if necessary
	 *@param nFollowRedirects The maximum number of redirects to follow
	 *@param bUseCaches Whether or not to use the HTTP cache
	 *@return the HttpURLConnection The connection obj after calling connect()
	 */
		public static HttpWebRequest getConnectionToURI(
			Uri oURI, string sMethod, NameValueCollection oReqProperties,
			object/*SSLSocketFactory*/ oSSLFactory, int nFollowRedirects, bool bUseCaches)
			//throws IOException
		{
			if (string.IsNullOrEmpty(sMethod)) {
				sMethod = "GET";
			}
			HttpWebRequest oConnection = null;
			try {
				// create the connection obj
				oConnection = (HttpWebRequest)WebRequest.Create(oURI);
				oConnection.Method = sMethod;
				// TODO: re-evaluate this when we know whether this method
				//       is called in scenarios where SSL is required.
				oConnection.AllowAutoRedirect = nFollowRedirects > 0;
				oConnection.MaximumAutomaticRedirections = nFollowRedirects;
				oConnection.CachePolicy = new RequestCachePolicy(bUseCaches ? RequestCacheLevel.CacheIfAvailable : RequestCacheLevel.BypassCache);
				oConnection.Headers[HttpRequestHeader.UserAgent] = "DotNetXri";
				//oConnection.setAllowUserInteraction(false); // TODO: what's this do?
				//oConnection.setDoInput(true);
				//oConnection.setDoOutput(sMethod.Equals("POST"));

				// setup request properties
				if (oReqProperties != null) {
					oConnection.Headers.Add(oReqProperties);
				}

				//// setup the SSL Socket factory
				//if (oConnection is HttpsURLConnection)
				//{
				//    // only set the socket factory if it has been overriden
				//    if (oSSLFactory != null)
				//    {
				//        ((HttpsURLConnection) oConnection).setSSLSocketFactory(
				//            oSSLFactory);
				//    }
				//}

				// connect and return
				//oConnection.connect();
				return oConnection;
			} catch (WebException oEx) {
				if (oEx.Response != null) {
					oEx.Response.Close();
				}
				throw;
			}
		} // getConnectionToURI()

	} // Class: IO
}