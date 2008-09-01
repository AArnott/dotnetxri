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
package org.openxri.util;

using java.lang.reflect.InvocationTargetException;

using org.apache.xerces.dom.DocumentImpl;
using org.w3c.dom.Attr;
using org.w3c.dom.Document;
using org.w3c.dom.Element;
using org.w3c.dom.Node;


/*
********************************************************************************
* Class: DOM3Utils
********************************************************************************
*/ /**
* This class provides utilities such that DOM3 is not needed at runtime if
* a Xerces DOM2 compliant DOM is used.  DOM3 is required to build this class.
* @author =chetan
*/
public class DOM3Utils
{
    private static bool sbHasDOM3Support = false;

    static
    {
        try
        {
            Document.class.getDeclaredMethod(
                "adoptNode", new Class[] { Node.class });
            Element.class.getDeclaredMethod(
            		"setIdAttributeNode", new Class[] { Node.class });
            sbHasDOM3Support = true;
        }
        catch (NoSuchMethodException e) {}
    }

    /*
    ****************************************************************************
    * hasDOM3Support()
    ****************************************************************************
    */ /**
    * Returns true iF DOM3 is available at runtime
    */
    public static bool hasDOM3Support()
    {
        return sbHasDOM3Support;

    } // hasDOM3Support()
    
    /*
     ****************************************************************************
     * isXercesDocument()
     ****************************************************************************
     */ /**
     * Returns true if specified Document is backed by Xerces
     */
    public static bool isXercesDocument(Document oDoc)
    {
        return oDoc.getClass().getName().startsWith("org.apache.xerces.dom");
    }

    /*
    ****************************************************************************
    * bestEffortAdopt()
    ****************************************************************************
    */ /**
    *  Attempts to adopt the node into the document
    */
    public static void bestEffortAdopt(Document oDoc, Node oNode)
    {
        // do nothing if there is nothing to do
        if (
            (oDoc == null) || (oNode == null) ||
            (oNode.getOwnerDocument() == oDoc))
        {
            return;
        }

        // if we are using DOM3, use the adoptNode API
        if (hasDOM3Support())
        {
        	// oDoc.adoptNode(oNode);
        	try {
				oDoc.getClass().getMethod("adoptNode", new Class[] { oNode.getClass() })
					.invoke(oDoc, new Object[] { oNode });
			}
			catch (IllegalArgumentException e) { }
			catch (SecurityException e) { }
			catch (IllegalAccessException e) { }
			catch (InvocationTargetException e) { }
			catch (NoSuchMethodException e) { }
        	
            return;
        }

        // if it is an DocumentImpl, steal its adoption
        if (isXercesDocument(oDoc))
        {
            ((DocumentImpl) oDoc).adoptNode(oNode);
            return;
        }

        // otherwise just import and live with the copying
        oDoc.importNode(oNode, true);

    } // bestEffortAdopt()

    /*
    ****************************************************************************
    * bestEffortSetIDAttr()
    ****************************************************************************
    */ /**
    * Makes a best effort at setting the ID attribute so that it can be looked
    * up using Document::getElementsById
    */
    public static void bestEffortSetIDAttr(
        Element oElem, String sNS, String sAttr)
    {
        // do nothing if there is nothing to do
        if ((oElem == null) || (sAttr == null))
        {
            return;
        }

        // if we are using DOM3, use the setIdAttributeNode API
        if (hasDOM3Support())
        {
            Attr oAttr = oElem.getAttributeNodeNS(sNS, sAttr);
            
            // compiler-friendly way of doing setId as follows:
            // oElem.setIdAttributeNode(oAttr, true);
        	try {
        		oElem.getClass().getMethod("setIdAttributeNode", new Class[] { oAttr.getClass(), Boolean.TYPE })
					.invoke(oElem, new Object[] { oAttr, Boolean.TRUE });
			}
        	/*
			catch (IllegalArgumentException e) { }
			catch (SecurityException e) { }
			catch (IllegalAccessException e) { }
			catch (InvocationTargetException e) { }
			catch (NoSuchMethodException e) { }
			*/
        	catch (Exception e) {
        		throw new RuntimeException("Exception caught while calling setIdAttributeNode: " + e);
        	}
        	
        	if (!oAttr.isId()) {
        		throw new RuntimeException("attribute node is not of type Id even after calling setIdAttributeNode!");
        	}
			return;
        }

        // if it is an DocumentImpl, use the putIdentifier method
        if (isXercesDocument(oElem.getOwnerDocument()))
        {
            String sAttrVal = oElem.getAttributeNS(sNS, sAttr);
            ((DocumentImpl) oElem.getOwnerDocument()).putIdentifier(
                sAttrVal, oElem);
            return;
        }

        throw new RuntimeException(
            "No known method to set a signable ID attribute. " +
            "Try using a DOM3-compliant Parser or a DOM2 Xerces Document.");

    } // bestEffortSetIDAttr()

} // Class: DOM3Utils
