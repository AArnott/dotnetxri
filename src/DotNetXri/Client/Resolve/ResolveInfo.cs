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

import java.util.Iterator;
import java.util.Vector;
import org.openxri.xml.XRD;


/*
********************************************************************************
* Class: ResolveInfo
********************************************************************************
*/ /**
* Holds information for a XRI resolve request.
* @author =chetan
*/
public class ResolveInfo
{
    // ordered vector of descriptors used/obtained during resolve
    private Vector moResolveChains = new Vector();

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructor - initializes as unresolved.
    */
    public ResolveInfo(String sXRI)
    {
        ResolveChain oChain = new ResolveChain(sXRI);
        moResolveChains.add(oChain);

    } // Constructor()

    /*
    ****************************************************************************
    * getXRI()
    ****************************************************************************
    */ /**
    * Get the XRI attempted to be resolved
    */
    public String getXRI()
    {
        return ((ResolveChain) moResolveChains.get(0)).getXRI();

    } // getXRI()

    /*
    ****************************************************************************
    * resolvedAll()
    ****************************************************************************
    */ /**
    * Was the XRI resolved to its completion?
    */
    public bool resolvedAll()
    {
        return getLastChain().resolvedAll();

    } // resolvedAll()

    /*
    ****************************************************************************
    * addChain()
    ****************************************************************************
    */ /**
    * Add the ResolveChain to the ResolvedInfo
    */
    public void addChain(ResolveChain oChain)
    {
        moResolveChains.add(oChain);

    } // addChain()

    /*
    ****************************************************************************
    * getFinalXRIDescriptor()
    ****************************************************************************
    */ /**
    * Get the last descriptor encountered during resolve.
    *
    * For a complete resolve, that is when resolvedAll() is true, the
    * final descriptor belongs to the complete XRI.
    *
    * For an incomplete resolve, this descriptor, if non-null, gives
    * the descriptor that was queried last for a resolve
    * getUnresolved() gives the remaining part of XRI that is still
    * left to be resolved.
    */
    public XRD getFinalXRIDescriptor()
    {
        return getLastChain().getFinalXRIDescriptor();

    } // getFinalXRIDescriptor()

    /*
    ****************************************************************************
    * getUnresolved()
    ****************************************************************************
    */ /**
    * Get the unresolved portion.
    * This portion does not start with "xri".
    * Example: Could be ".yahoo.john" for "xri:@email.com.yahoo.john".
    * Null is returned for complete resolutions.
    */
    public String getUnresolved()
    {
        return getLastChain().getUnresolved();

    } // getUnresolved()

    /*
    ****************************************************************************
    * getFirstChain()
    ****************************************************************************
    */ /**
    * Get the first chain that was being resolved
    */
    public ResolveChain getFirstChain()
    {
        return (ResolveChain) moResolveChains.get(0);

    } // getFirstChain()

    /*
    ****************************************************************************
    * getLastChain()
    ****************************************************************************
    */ /**
    * Get the last chain that was being resolved
    */
    public ResolveChain getLastChain()
    {
        return (ResolveChain) moResolveChains.get(moResolveChains.size() - 1);

    } // getLastChain()

    /*
    ****************************************************************************
    * getNumChains()
    ****************************************************************************
    */ /**
    * Get the number of chains during resolution
    */
    public int getNumChains()
    {
        return moResolveChains.size();

    } // getNumChains()

    /*
    ****************************************************************************
    * getChainIterator()
    ****************************************************************************
    */ /**
    * Get the number of chains during resolution
    */
    public Iterator getChainIterator()
    {
        return moResolveChains.iterator();

    } // getChainIterator()

} // Class: ResolveInfo
