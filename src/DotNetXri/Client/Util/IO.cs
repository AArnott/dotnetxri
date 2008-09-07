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


using java.io.IOException;
using java.net.HttpURLConnection;
using java.net.URI;
using java.net.URISyntaxException;
using java.util.Iterator;
using java.util.Map;
using java.util.Set;
using javax.net.ssl.HttpsURLConnection;
using javax.net.ssl.SSLSocketFactory;


/*
********************************************************************************
* Class: IO
********************************************************************************
*/ /**
* Provides helper methods to create a HttpURLConnection from a URI and properties
* associated with how to access the URI.
* @author =chetan
*/
public class IO
{

    /*
    ****************************************************************************
    * getConnectionToURI()
    ****************************************************************************
    */ /**
    *Gets the connection to a URI.
    *@param oURI The URI to access
    *@param sMethod - The HTTP Method (Defaults to "GET")
    *@param oReqProperties The request properties to set
    *@param oSSLFactory The SSL Socket Factory to use if necessary
    *@param bFollowRedirects Whether or not to follow unlimited redirects
    *@param bUseCaches Whether or not to use the HTTP cache
    *@return the HttpURLConnection The connection obj after calling connect()
    */
    public static HttpURLConnection getConnectionToURI(
        URI oURI, String sMethod, Map oReqProperties,
        SSLSocketFactory oSSLFactory, bool bFollowRedirects,
        bool bUseCaches)
        throws IOException
    {
        if ((sMethod == null) || (sMethod.equals("")))
        {
            sMethod = "GET";
        }
        HttpURLConnection oConnection = null;
        try
        {
            // create the connection obj
            oConnection = (HttpURLConnection) oURI.toURL().openConnection();
            oConnection.setRequestMethod(sMethod);
            oConnection.setInstanceFollowRedirects(bFollowRedirects);
            oConnection.setAllowUserInteraction(false);
            oConnection.setUseCaches(bUseCaches);
            oConnection.setDoInput(true);
            oConnection.setRequestProperty("user-agent", "OpenXRI");
            oConnection.setDoOutput(sMethod.equals("POST"));

            // setup request properties
            if (oReqProperties != null)
            {
                Set oSet = oReqProperties.entrySet();
                for (Iterator oIt = oSet.iterator(); oIt.hasNext();)
                {
                    Map.Entry oProperty = (Map.Entry) oIt.next();
                    oConnection.addRequestProperty(
                        oProperty.getKey().toString(),
                        oProperty.getValue().toString());
                }
            }

            // setup the SSL Socket factory
            if (oConnection is HttpsURLConnection)
            {
                // only set the socket factory if it has been overriden
                if (oSSLFactory != null)
                {
                    ((HttpsURLConnection) oConnection).setSSLSocketFactory(
                        oSSLFactory);
                }
            }

            // connect and return
            oConnection.connect();
            return oConnection;
        }
        catch (IOException oEx)
        {
            // When the server sends a 404, we can get an exception and
            // end up here.  The problem is that the socket connection
            // to the server is not closed, and won't be closed until
            // the GC has a chance to cleanup the InputStream.  A really
            // fast client box can fill the descriptor table.  Since
            // we never get the InputStream, we have no way of closing
            // it.
            if (oConnection != null)
            {
                oConnection.disconnect();
            }
            throw oEx;
        }

    } // getConnectionToURI()

    /*
    ****************************************************************************
    * getConnectionToURI()
    ****************************************************************************
    */ /**
    *Gets the connection to a URI.
    *@param oURI The URI to access
    *@param sMethod - The HTTP Method (Defaults to "GET")
    *@param oReqProperties The request properties to set
    *@param oSSLFactory The SSL Socket Factory to use if necessary
    *@param nFollowRedirects The maximum number of redirects to follow
    *@param bUseCaches Whether or not to use the HTTP cache
    *@return the HttpURLConnection The connection obj after calling connect()
    */
    public static HttpURLConnection getConnectionToURI(
        URI oURI, String sMethod, Map oReqProperties,
        SSLSocketFactory oSSLFactory, int nFollowRedirects, bool bUseCaches)
        throws IOException
    {
        URI oNextURI = oURI;
        for (int i = 0; i <= nFollowRedirects; i++)
        {
            HttpURLConnection oConnection =
                getConnectionToURI(
                    oNextURI, sMethod, oReqProperties, oSSLFactory, false,
                    bUseCaches);
            int nCode = oConnection.getResponseCode();
            switch (nCode)
            {
                // if there is a redirect, return 
                case HttpURLConnection.HTTP_MOVED_PERM:
                case HttpURLConnection.HTTP_MOVED_TEMP:
                case HttpURLConnection.HTTP_SEE_OTHER:
                case HttpURLConnection.HTTP_USE_PROXY:
                    String sURI = oConnection.getHeaderField("Location");
                    try
                    {
                        oNextURI = new URI(sURI);
                    }
                    catch (URISyntaxException e)
                    {
                        throw new IOException(
                            "Got invalid URI via http redirect from : " +
                            oNextURI.toString() + ", location header = " +
                            sURI);
                    }
                    break;

                // anything else, good or bad, should be returned to the caller
                default:
                    return oConnection;
            }
        }
        throw new IOException(
            "Got too many redirects accessing " + oURI.toString());

    } // getConnectionToURI()

} // Class: IO
}