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
package org.openxri.resolve;

import java.net.URI;
import java.util.ArrayList;

import org.openxri.GCSAuthority;
import org.openxri.XRI;
import org.openxri.XRIAuthority;
import org.openxri.xml.XRD;


/**
 * This class <strong>used to provide</strong> encapsulation of the cache state of a Resolver obj.
 * Now, it is used to store information about the references processed, URI traversed, etc. during
 * a resolution request.
 * The caching functionality may be revived at a later date.
 *
 * @author =chetan
 * @author =wil
 */
public class ResolverState
{

	private long timeStarted;
	private ArrayList steps;
	private int numRefsFollowed;	// successful or not
	private int numRequests;		// # of requests
	private int numBytesReceived;	// just the XRDS content size
	
    /**
     * Constructor
     */
    public ResolverState()
    {
    	timeStarted = System.currentTimeMillis();
    	steps = new ArrayList();
    	numRefsFollowed = 0; // successful or not
    	numRequests = 0;
    	numBytesReceived = 0;
    }

    /**
     * @return Returns the time that this obj was constructed.
     */
    public long getTimeStarted()
    {
    	return timeStarted;
    }
	
	/**
	 * @return Returns the number of Refs followed.
	 */
	public int getNumRefsFollowed() {
		return numRefsFollowed;
	}

	/**
	 * @return Returns the number of resolution requests attempted
	 */
	public int getNumRequests() {
		return numRequests;
	}

	/**
	 * @return Returns the total size of XRDS's received.
	 */
	public int getNumBytesReceived() {
		return numBytesReceived;
	}


	public ResolverStep getStepAt(int i)
	{
		return (ResolverStep)steps.get(i);
	}

	
	public int getNumSteps()
	{
		return steps.size();
	}
	

    /**
     * 
     * @param qxri QXRI that was resolved
     * @param xrds XRDS document received
     * @param uri  URI queried to resolve the QXRI
     */
	public void pushResolved(String qxri, String trustType, String xrds, URI uri)
	{
		ResolverStep step = new ResolverStep(qxri, trustType, xrds, null, uri);
		steps.add(step);
		numRequests++;
		numBytesReceived += xrds.length();
	}
	
	
	public void pushFollowingRef(XRI ref)
	{
		ResolverStep step = new ResolverStep(null, null, null, ref, null);
		steps.add(step);
		numRefsFollowed++;
	}
	
	
	public String toString()
	{
		StringBuffer sb = new StringBuffer();
		
		sb.append("NumRequests=" + numRequests + ", numRefsFollowed=" + numRefsFollowed + ", numBytesReceived=" + numBytesReceived + "\n");
		for (int i = 0; i < getNumSteps(); i++) {
			ResolverStep step = getStepAt(i);
			sb.append(step.toString());
			sb.append("\n");
		}
		return sb.toString();
	}
	
	
	public class ResolverStep
	{
		public final String qxri;
		public final String trust; 
		public final String xrds;
		public final URI    uri;
		public final XRI    ref;
		public final long   timeCompleted;
		
		public ResolverStep(String qxri, String trust, String xrds, XRI ref, URI uri)
		{
			this.qxri  = qxri;
			this.trust = trust;
			this.xrds  = xrds;
			this.ref   = ref;
			this.uri   = uri;
			this.timeCompleted = System.currentTimeMillis();
		}
		
		public String toString()
		{
			StringBuffer sb = new StringBuffer();
			sb.append("QXRI=");
			sb.append(qxri);
			sb.append(", trust=");
			sb.append(trust);
			sb.append(", uri=");
			sb.append((uri == null)? "null" : uri.toASCIIString());
			sb.append(", ref=");
			sb.append(ref);
			sb.append(", elapsed=");
			sb.append(timeCompleted - timeStarted);
			sb.append("ms\n\nXRDS = \n");
			sb.append(xrds);
			return sb.toString();
		}
	}
	
	
	
	
///////////// Deprecated stuff below.
	
	
	
	/**
	 * @deprecated
	 */
    private Cache moBasicCache = new Cache(1000);

    /**
     * @deprecated
     */
    private Cache moTrustedCache = new Cache(1000);


    /**
     * Returns the Cache obj for the security model
     * @param bTrusted if true, returns the trusted cache, if false, returns
     *        the regular cache.
	 * @deprecated
     */
    public Cache getCache(bool bTrusted)
    {
        return (bTrusted == true) ? moBasicCache : moTrustedCache;

    }


    /**
     * Sets the XRD for the final subsegment of the XRIAuthority.
     * The authority may contain multiple subgments.  Each internal subsegment
     * will not be set.
     * @param oAuth The XRIAuthority element to set
     * @param oDesc The descriptor for the final subsegment in the oAuth
	 * @deprecated
     */
    public void set(XRIAuthority oAuth, XRD oDesc)
    {
        moBasicCache.stuff(oAuth, oDesc);
        moTrustedCache.stuff(oAuth, oDesc);

    }

    /**
     * Resets the internal XRD cache for this XRIAuthority
     * @param oAuth The XRIAuthority element to prune
     */
    public void reset(XRIAuthority oAuth)
    {
        moBasicCache.prune(oAuth);
        moTrustedCache.prune(oAuth);
    }

    /**
     * Sets the XRD for the "@" Authority.
     * @param oDesc The descriptor for the "@" Authority
     * @deprecated
     */
    public void setAtAuthority(XRD oDesc)
    {
    	GCSAuthority oAuth = new GCSAuthority("@");
    	set(oAuth, oDesc);
    	
    }
    
    /**
     * Sets the XRD for the "=" Authority.
     * @param oDesc The descriptor for the "=" Authority
     */
    public void setEqualsAuthority(XRD oDesc)
    {
    	GCSAuthority oAuth = new GCSAuthority("=");
    	set(oAuth, oDesc);
    	
    }

}
