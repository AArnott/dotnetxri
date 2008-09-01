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

using java.util.Vector;
using org.openxri.xml.XRD;
using org.openxri.xml.XRDS;


/*
********************************************************************************
* Class: ResolveChain
********************************************************************************
*/ /**
* Holds information for a XRI resolution chain (no redirects)
* @author chandra
* @author =chetan
*/
public class ResolveChain
{
    // xri attempted to resolve
    private String msXRI = null;

    // were all segments resolved?
    private bool mbResolvedAll = false;

    // ordered vector of descriptors used/obtained during resolve
    private XRDS moXRIDescriptors = new XRDS();

    // unresolved portion for an incomplete resolve
    private String msUnresolved = "";

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructor - initializes as unresolved.
    */
    public ResolveChain(String sXRI)
    {
        msXRI = sXRI;

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
        return msXRI;

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
        return mbResolvedAll;

    } // resolvedAll()

    /*
    ****************************************************************************
    * getXRIDescriptors()
    ****************************************************************************
    */ /**
    * Returns the XRDS element for this chain
    */
    public XRDS getXRIDescriptors()
    {
        return moXRIDescriptors;

    } // getXRIDescriptors()

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
        int nSize = moXRIDescriptors.getNumChildren();
        return (nSize > 0) ? moXRIDescriptors.getDescriptorAt(nSize - 1) : null;

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
        return msUnresolved;

    } // getUnresolved()

    /*
    ****************************************************************************
    * setResolvedAll()
    ****************************************************************************
    */ /**
    * Mark that the XRI resolved completely.
    */
    public void setResolvedAll()
    {
        mbResolvedAll = true;

    } // setResolvedAll()

    /*
    ****************************************************************************
    * setUnresolved()
    ****************************************************************************
    */ /**
    * Set unresolved portion.
    */
    public void setUnresolved(String sUnresolved)
    {
        msUnresolved = (sUnresolved == null) ? "" : sUnresolved;

    } // setUnresolved()

    /*
    ****************************************************************************
    * addXRIDescriptor()
    ****************************************************************************
    */ /**
    * Adds the specified XRI Descriptor to the chain
    */
    public void addXRIDescriptor(XRD oXRID)
    {
        moXRIDescriptors.add(oXRID);

    } // addXRIDescriptor()

    /*
    ****************************************************************************
    * addXRIDescriptors()
    ****************************************************************************
    */ /**
    * Adds the specified XRI Descriptors to the chain
    */
    public void addXRIDescriptors(XRDS oDescriptors)
    {
        for (int i = 0; i < oDescriptors.getNumChildren(); i++)
        {
            addXRIDescriptor(oDescriptors.getDescriptorAt(i));
        }

    } // addXRIDescriptors()

    /*
    ****************************************************************************
    * addXRIDescriptors()
    ****************************************************************************
    */ /**
    * Adds the specified XRI Descriptors to the chain
    */
    public void addXRIDescriptors(Vector oDescriptors)
    {
        for (int i = 0; i < oDescriptors.size(); i++)
        {
            addXRIDescriptor((XRD) oDescriptors.get(i));
        }

    } // addXRIDescriptors()

} // Class: ResolveChain
}