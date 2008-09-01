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

import java.util.Collections;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Vector;
import org.openxri.XRISubSegment;
import org.openxri.XRIAuthority;
import org.openxri.xml.XRD;
import org.openxri.xml.XRDS;


/*
********************************************************************************
* Class: Cache
********************************************************************************
*/ /**
* This class provides a cache for XRI resolutions
*
* @author steveg
* @author =chetan
*/
public class Cache
{
    // points to the CacheNode for this Cache obj
    CacheNode moRootNode = null;
    private int mnNodes = 0;
    private int mnMaxSize = 1000;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Creates a Cache structure with the specified maximum size
    */
    Cache(int nSize)
    {
        moRootNode = new CacheNode(this);
        mnMaxSize = nSize;

    } // Constructor()

    /*
    ****************************************************************************
    * increment()
    ****************************************************************************
    */ /**
    * Increments the node count
    */
    synchronized int increment()
    {
        mnNodes++;
        return mnNodes;

    } // increment()

    /*
    ****************************************************************************
    * decrement()
    ****************************************************************************
    */ /**
    * Decrements the node count
    */
    synchronized int decrement()
    {
        mnNodes--;
        return mnNodes;

    } // decrement()

    /*
    ****************************************************************************
    * getNumNodes()
    ****************************************************************************
    */ /**
    * Returns the number of nodes in the cache
    */
    public synchronized int getNumNodes()
    {
        return mnNodes;

    } // getNumNodes()

    /*
    ****************************************************************************
    * setMaxSize()
    ****************************************************************************
    */ /**
    * Sets the maximum size of the cache
    */
    public synchronized void setMaxSize(int nVal)
    {
        mnMaxSize = nVal;

    } // setMaxSize()

    /*
    ****************************************************************************
    * getMaxSize()
    ****************************************************************************
    */ /**
    * Gets the maximum size of the cache
    */
    public synchronized int getMaxSize()
    {
        return mnMaxSize;

    } // getMaxSize()

    // use this methods to get a CacheValue based on the GCS Authority

    /*
    ****************************************************************************
    * find()
    ****************************************************************************
    */ /**
    * Finds entries in the cache
    * @param oAuth the XRI Authority to look up
    * @param bPartial specifies whether or not to return partial hits
    * found for the authority to the end of the vector
    */
    CachedValue find(XRIAuthority oAuth, bool bPartial)
    {
        return find(oAuth, bPartial, false, null);

    } // find()

    /*
    ****************************************************************************
    * find()
    ****************************************************************************
    */ /**
    * Finds entries in the cache
    * @param oAuth the XRI Authority to look up
    * @param bPartial specifies whether or not to return partial hits
    * @param bCompleteChain specifies whether or not intermediate values must have a descriptor
    * @param oCachedDescriptors if not null, adds each descriptor that was
    * found for the authority to the end of the vector
    */
    CachedValue find(
        XRIAuthority oAuth, bool bPartial, bool bCompleteChain,
        Vector oCachedDescriptors)
    {
        CacheNode oNode =
            findNode(oAuth, bPartial, bCompleteChain, oCachedDescriptors);
        return (oNode != null) ? oNode.moCacheValue : null;

    } // find()

    /*
    ****************************************************************************
    * findNode()
    ****************************************************************************
    */ /**
    *
    */
    private CacheNode findNode(
        XRIAuthority oAuth, bool bPartial, bool bCompleteChain,
        Vector oCachedDescriptors)
    {
        // get the Node for the community root
        CacheNode oCommunityNode = moRootNode.find(oAuth.getRootAuthority());
        if (oCommunityNode == null)
        {
            return null;
        }

        // if the found node doesn't have a cached value, potentially bail
        if (
            (oCommunityNode.moCacheValue == null) ||
            (oCommunityNode.moCacheValue.getDescriptor() == null))
        {
            if (bCompleteChain)
            {
                return null;
            }
        }
        else if (oCachedDescriptors != null)
        {
            oCachedDescriptors.add(oCommunityNode.moCacheValue.getDescriptor());
        }

        // find the deepest node that fits the bill
        CacheResult oDeepestNode =
            oCommunityNode.find(oAuth, 0, bCompleteChain, oCachedDescriptors);

        // return the node we found if we got everything, or we are in partial mode
        if (bPartial || (oDeepestNode.mnNumFound == oAuth.getNumSubSegments()))
        {
            return oDeepestNode.moLastCacheNode;
        }
        else
        {
            return null;
        }

    } // findNode()

    /*
    ****************************************************************************
    * stuff()
    ****************************************************************************
    */ /**
    *
    */
    synchronized void stuff(XRIAuthority oAuth, XRD oDescriptor)
    {
        stuff(oAuth, oDescriptor, oAuth.getNumSubSegments());

    } // stuff()

    /*
    ****************************************************************************
    * stuff()
    ****************************************************************************
    */ /**
    * Adds the descriptors associated with an authority to the cache
    * @param oAuth - The Authority whose descriptors to store
    * @param oDescriptors - The descriptors of the subsegments in oAuth
    */
    public void stuff(XRIAuthority oAuth, XRDS oDescriptors)
    {
        for (int i = 0; i < oDescriptors.getNumChildren(); i++)
        {
            stuff(oAuth, oDescriptors.getDescriptorAt(i), i);
        }

    } // stuff()

    /*
    ****************************************************************************
    * stuff()
    ****************************************************************************
    */ /**
    * Adds a descriptors associated with a subsegment to the cache.
    * @param oAuth - The Authority containing a subsegment whose descriptor is
    * provided
    * @param oDescriptor - The descriptor of the subsegment
    * @param nDepth - The index of the subsegment in oAuth
    */
    synchronized void stuff(
        XRIAuthority oAuth, XRD oDescriptor, int nDepth)
    {
        // get the community node
        CacheNode oCommunityNode = moRootNode.mkdir(oAuth.getRootAuthority());

        // if necessary, create a node for this authority path
        CacheNode oNode = oCommunityNode.mkdir(oAuth, 0, nDepth);

        // set the correct descriptor for the node
        oNode.moCacheValue = new CachedValue(oDescriptor, nDepth);

        trim();

    } // stuff()

    /*
    ****************************************************************************
    * trim()
    ****************************************************************************
    */ /**
    * Reduces the size of the cache to be within the maximum size
    */
    synchronized void trim()
    {
        while (mnNodes > mnMaxSize)
        {
            CacheNode oNode = moRootNode.moPrev;

            // try to find a non top-level node
            while ((oNode.moParent == moRootNode))
            {
                oNode = oNode.moPrev;
            }

            // if all we got was the root, bail
            if (oNode == moRootNode)
            {
                return;
            }

            oNode.removeSelf(true);
        }

    } // trim()

    // unstuff node and prune all the children of the node

    /*
    ****************************************************************************
    * prune()
    ****************************************************************************
    */ /**
    * Removes the Authority and all subsegments registered underneath it from
    * the cache.
    */
    synchronized bool prune(XRIAuthority oAuth)
    {
        CacheNode oNode = findNode(oAuth, false, false, null);
        return (oNode == null) ? false : oNode.removeSelf(true);

    } // prune()

    /*
    ****************************************************************************
    * dump()
    ****************************************************************************
    */ /**
    * Prints out a description of the cache contents
    */
    synchronized void dump()
    {
        System.Console.WriteLine("Num nodes = " + mnNodes);
        CacheNode oNode = moRootNode.moNext;
        int i = 1;
        while (oNode != moRootNode)
        {
            System.Console.WriteLine("#" + i + "\t" + oNode.msSubsegment);
            oNode = oNode.moNext;
            i++;
        }

    } // dump()

    /*
    ****************************************************************************
    * Class: CachedValue
    ****************************************************************************
    */ /**
    * This class provides an externally visible representation of the result of
    * searching the cache.  It provides the descriptor for resulting cache
    * entry as well as the depth of that entry.
    */
    public class CachedValue
    {
        private XRD moDescriptor;
        private int mnDepth = 0;

        /*
        ************************************************************************
        * Constructor()
        ************************************************************************
        */ /**
        * Constructs a value with the corresponding parameters
        */
        CachedValue(XRD oDesc, int nDepth)
        {
            moDescriptor = oDesc;
            mnDepth = nDepth;

        } // Constructor()

        /*
        ************************************************************************
        * getDepth()
        ************************************************************************
        */ /**
        * Returns the depth at which the descriptor was found
        */
        public int getDepth()
        {
            return mnDepth;

        } // getDepth()

        /*
        ************************************************************************
        * getDescriptor()
        ************************************************************************
        */ /**
        * Returns the descriptor found for some subsegment in the cache
        */
        public XRD getDescriptor()
        {
            return ((moDescriptor != null) && (moDescriptor.isValid()))
            ? moDescriptor : null;

        } // getDescriptor()

        /*
        ************************************************************************
        * toString()
        ************************************************************************
        */ /**
        * Serializes the descriptor member.
        */
        public String toString()
        {
            return moDescriptor.toString();

        } // toString()

    } // Class: CachedValue

} // Class: Cache



/*
********************************************************************************
* Class: CacheNode
********************************************************************************
*/ /**
*  This class encapsulates the nodes stored in the cache data structure.
*/
class CacheNode
{
    Cache.CachedValue moCacheValue = null;
    Map moCacheHash = Collections.synchronizedMap(new HashMap());
    CacheNode moParent = null;
    String msSubsegment = null;
    CacheNode moPrev = null;
    CacheNode moNext = null;
    Cache moCache = null;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * This constructor is used only for the root node, makes a circular
    * doubly linked list
    */
    CacheNode(Cache oCache)
    {
        moCache = oCache;
        moNext = this;
        moPrev = this;

    } // Constructor()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  This is used to add a new CacheNode to the cache
    */
    CacheNode(CacheNode oParent, String sSubsegment)
    {
        // we need to keep track of these for the removeSelf method
        moParent = oParent;
        msSubsegment = sSubsegment;
        moCache = oParent.moCache;
        moCache.increment();

        addToFront();

    } // Constructor()

    /*
    ****************************************************************************
    * touch()
    ****************************************************************************
    */ /**
    * Used to freshen the node in the cache so that it is not removed due to
    * size limits
    */
    void touch()
    {
        // detach yourself from your current spot first
        detach();

        // now add to the front
        addToFront();

    } // touch()

    /*
    ****************************************************************************
    * removeSelf()
    ****************************************************************************
    */ /**
    * This removes the node from the cache along with its children.
    * @param bRemoveFromParent - Whether or not to remove the node from its parent
    * Only used in recursion as an optimization.
    */
    bool removeSelf(bool bRemoveFromParent)
    {
        if (moParent == null)
        {
            return false;
        }

        // remove yourself from the linked list
        detach();

        // lower the counter
        moCache.decrement();

        // tell your children to remove themselves
        Iterator oChild = moCacheHash.values().iterator();
        while (oChild.hasNext())
        {
            ((CacheNode) oChild.next()).removeSelf(false);
        }

        // if we don't have to remove ourselves from our parent, we are done
        if (!bRemoveFromParent)
        {
            return true;
        }

        // finally, remove yourself from your parent
        return (moParent.moCacheHash.remove(msSubsegment) != null);

    } // removeSelf()

    /*
    ****************************************************************************
    * find()
    ****************************************************************************
    */ /**
    * Finds subsegments under this CacheNode.  Will popluate oCachedDescriptors
    * along the way.
    * @param oAuth - The authority to search for
    * @param nNextSubsegment - The index of the next subsegment to search for
    * @param bCompleteChain - Whether or not a descriptor is necessary for all
    * subsegments in oAuth
    * @param oCachedDescriptors - If not null, stores descriptors found in the
    * cache, in the order of the subsegments.
    */
    CacheResult find(
        XRIAuthority oAuth, int nNextSubsegment, bool bCompleteChain,
        Vector oCachedDescriptors)
    {
        // if there are no new subsegments to get, just return "this", we are done
        XRISubSegment oSubSegment = oAuth.getSubSegmentAt(nNextSubsegment);
        if (oSubSegment == null)
        {
            return new CacheResult(this, nNextSubsegment);
        }

        // also return if we can't find the next subsegment
        CacheNode oNode = find(oSubSegment.toString());
        if (oNode == null)
        {
            return new CacheResult(this, nNextSubsegment);
        }

        // if the found node doesn't have a cached value, potentially bail
        if (
            (oNode.moCacheValue == null) ||
            (oNode.moCacheValue.getDescriptor() == null))
        {
            if (bCompleteChain)
            {
                return new CacheResult(this, nNextSubsegment);
            }
        }
        else if (oCachedDescriptors != null)
        {
            oCachedDescriptors.add(oNode.moCacheValue.getDescriptor());
        }

        // N O T E: The direcory metaphore used here allows for directories
        //           to be "empty" (null moCacheValue) and still have subdirs.
        //	     
        // As we recurse up, if the returned CacheNode has an empty
        // moCacheValue, we'll return "this" unless they caller does not
        // allow partials.
        return oNode.find(
            oAuth, nNextSubsegment + 1, bCompleteChain, oCachedDescriptors);

    } // find()

    /*
    ****************************************************************************
    * mkdir()
    ****************************************************************************
    */ /**
    *  Creates an entry under this CacheNode for the given subsegments
    * @param oAuth - The authority to add
    * @param n - The index of the subsegment to start with
    * @param iTargetDepth - The index of the subsegment to stop at.
    * @return The final CacheNode created by this method
    */
    CacheNode mkdir(XRIAuthority oAuth, int n, int iTargetDepth)
    {
        XRISubSegment oSubSegment = oAuth.getSubSegmentAt(n);
        if (oSubSegment == null)
        {
            return this;
        }

        CacheNode oNode = mkdir(oSubSegment.toString());

        return ((n + 1) < iTargetDepth)
        ? oNode.mkdir(oAuth, n + 1, iTargetDepth) : oNode;

    } // mkdir()

    /*
    ****************************************************************************
    * mkdir()
    ****************************************************************************
    */ /**
    * Creates a CacheNode under this CacheNode for the specified subsegment
    */
    CacheNode mkdir(String sSubSegment)
    {
        CacheNode oNode = find(sSubSegment);
        if (oNode == null)
        {
            oNode = new CacheNode(this, sSubSegment);
            moCacheHash.put(sSubSegment, oNode);
        }

        return oNode;

    } // mkdir()

    /*
    ****************************************************************************
    * find()
    ****************************************************************************
    */ /**
    * Tries to find the specified subsegment under the current CacheNode
    */
    CacheNode find(String sSubSegment)
    {
        CacheNode oNode = (CacheNode) moCacheHash.get(sSubSegment);
        if (oNode != null)
        {
            synchronized (moCache)
            {
                // While we were blocked, the node might have been pruned
                if (oNode.moNext == null)
                {
                    return null;
                }

                oNode.touch();
            }
        }

        return oNode;

    } // find()

    /*
    ****************************************************************************
    * addToFront()
    ****************************************************************************
    */ /**
    * Places this CacheNode in the front of the queue.  Synchronized externally.
    */
    private void addToFront()
    {
        // insert the new node right after the root node
        moPrev = moCache.moRootNode;
        moNext = moCache.moRootNode.moNext;
        moCache.moRootNode.moNext = this;
        moNext.moPrev = this;

    } // addToFront()

    /*
    ****************************************************************************
    * detach()
    ****************************************************************************
    */ /**
    * Detaches this CacheNode from the queue.  Synchronized externally.
    */
    private void detach()
    {
        // remove yourself from the current spot in the list
        moPrev.moNext = moNext;
        moNext.moPrev = moPrev;
        moNext = null;
        moPrev = null;

    } // detach()

} // Class: CacheNode



/*
********************************************************************************
* Class: CacheResult
********************************************************************************
*/ /**
*  This class is used to provide an encapsulated result of searching the
*  CacheNodes.  Along with the last CachNode found, it returns the depth of the
*  CacheNode
*/
class CacheResult
{
    CacheNode moLastCacheNode = null;
    int mnNumFound = 0;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *
    */
    CacheResult(CacheNode oNode, int nNumFound)
    {
        moLastCacheNode = oNode;
        mnNumFound = nNumFound;

    } // Constructor()

} // Class: CacheResult
}