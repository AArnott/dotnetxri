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
namespace DotNetXri.Client.Resolve {

import java.net.URI;
import javax.net.ssl.SSLSocketFactory;
import org.openxri.XRIParseException;
import org.openxri.resolve.exception.XRIResolutionException;
import org.openxri.xml.XRD;


/*
********************************************************************************
* Interface: ResolveInterface
********************************************************************************
*/ /**
* This class defines the public interfaces for the Resolver class
*
* @author =chetan
*/
public interface ResolveInterface
{
    /*
    ****************************************************************************
    * resolveAuthToXRD()
    ****************************************************************************
    */ /**
    * Resolve XRI.
    * Returns null upon pure not found.
    * Returns the final XRD if found.
    * Throws exceptions upon errors while talking to servers.
    */
    public XRD resolveAuthToXRD(
        String qxri, String trustType, bool followRefs)
        throws XRIParseException, XRIResolutionException;


    /*
    ****************************************************************************
    * getSSLSocketFactory()
    ****************************************************************************
    */ /**
    * Returns the SSL Socket Factory being used for SSL connections.
    * Returns null if default has not been changed.
    */
    public SSLSocketFactory getSSLSocketFactory();

    /*
    ****************************************************************************
    * setSSLSocketFactory()
    ****************************************************************************
    */ /**
    * Sets the Socket Factory to use for SSL connections
    */
    public void setSSLSocketFactory(SSLSocketFactory oSocketFactory);

    /*
    ****************************************************************************
    * getFollowRedirects()
    ****************************************************************************
    */ /**
    * Gets maximum number of redirects to follow for a single resolution
    */
    public int getFollowRedirects();

    /*
    ****************************************************************************
    * setFollowRedirects()
    ****************************************************************************
    */ /**
    * Sets how many redirects to follow in a single resolution
    */
    public void setFollowRedirects(int nFollowRedirects);

    /*
    ****************************************************************************
    * setProxyResolver()
    ****************************************************************************
    */ /**
    * Sets the proxy resolver to use for XRI resolution
    * @param oProxyURI - The URI of the proxy resolver to use for resolution.
    * Set to null to disable Proxy resolution
    */
    public void setProxyResolver(URI oProxyURI);

    /*
    ****************************************************************************
    * getProxyResolver()
    ****************************************************************************
    */ /**
    * Returns the URI of the proxy resolver being used.  If null, root resolvers
    * may be contacted directly.
    */
    public URI getProxyResolver();

    /*
    ****************************************************************************
    * isLookaheadMode()
    ****************************************************************************
    */ /**
    * Returns true if lookahead mode is enabled
    */
    public bool isLookaheadMode();

    /*
    ****************************************************************************
    * setLookaheadMode()
    ****************************************************************************
    */ /**
    * Sets lookahead mode
    */
    public void setLookaheadMode(bool bVal);

} // Interface: ResolveInterface
}