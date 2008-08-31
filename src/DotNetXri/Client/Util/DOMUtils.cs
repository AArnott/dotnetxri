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

import java.io.StringWriter;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

import org.apache.xerces.dom.TextImpl;
import org.apache.xerces.parsers.DOMParser;
import org.apache.xml.serialize.OutputFormat;
import org.apache.xml.serialize.XMLSerializer;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.w3c.dom.Document;


/*
********************************************************************************
* Class: DOMUtils
********************************************************************************
*/ /**
* Provides simple, standrard utilities for dealing with DOM.  Parts of this code
* are based on code available from OpenSAML.  See www.opensaml.org.
*
*@author =chetan
*/
public class DOMUtils
{
    private static ThreadLocal soDOMParser =
        new ThreadLocal()
        {
            /*
            ********************************************************************
            * initialValue()
            ********************************************************************
            */ /**
            *
            */
            protected synchronized Object initialValue()
            {
                return new DOMParser();

            } // initialValue()
        };

    /*
    ****************************************************************************
    * getDOMParser()
    ****************************************************************************
    */ /**
    * Returns a DOMParser object that can safely be used in the current
    * thread.  This is done because newing a Xerces DOMParser object is
    * expensive and parser instances are not thread safe.
    * @return a reset DOMParser object
    */
    public static DOMParser getDOMParser()
    {
        DOMParser oParser = (DOMParser) soDOMParser.get();
        oParser.reset();
        return oParser;

    } // getDOMParser()

    /*
    ****************************************************************************
    * getFirstChildElement()
    ****************************************************************************
    */ /**
    *  Gets the first child Element matching the NS and tag from the given node    *
    *
    * @param oNode     The node to look under
    * @param sNS       The namespace URI of the element
    * @param sTag      The tag name of the element to locate
    * @return          The first child Element of oNode with the given ns and tag
    */
    public static Element getFirstChildElement(
        Node oNode, String sNS, String sTag)
    {
        Element oElem = getFirstChildElement(oNode);
        while ((oElem != null) && !isElementNamed(oElem, sNS, sTag))
        {
            oElem = getNextSiblingElement(oElem);
        }
        return oElem;

    } // getFirstChildElement()

    /*
    ****************************************************************************
    * getNextSiblingElement()
    ****************************************************************************
    */ /**
    *  Gets the next sibling Element of the node.  Skips over whitespace nodes;
    *
    * @param oNode  The sibling to start searching from
    * @return      The next sibling Element of oNode
    */
    public static Element getNextSiblingElement(Node oNode)
    {
        Node oNext = oNode.getNextSibling();
        while ((oNext != null) && (oNext.getNodeType() != Node.ELEMENT_NODE))
        {
            oNext = oNext.getNextSibling();
        }

        return (Element) oNext;

    } // getNextSiblingElement()

    /*
    ****************************************************************************
    * isElementNamed()
    ****************************************************************************
    */ /**
    *  Checks if an element matches the given namespace and tag
    *
    */
    public static bool isElementNamed(
        Element oElem, String sNS, String sTag)
    {
        // bail if no elem
        if (oElem == null)
        {
            return false;
        }

        // match the namespace
        bool bMatch =
            (sNS == null) ? (oElem.getNamespaceURI() == null)
                          : sNS.equals(oElem.getNamespaceURI());
        if (bMatch)
        {
            bMatch =
                (sTag == null) ? (oElem.getLocalName() == null)
                               : sTag.equals(oElem.getLocalName());
        }

        return bMatch;

    } // isElementNamed()

    /*
    ****************************************************************************
    * getFirstChildElement()
    ****************************************************************************
    */ /**
    *  Gets the first child Element of the node, skipping any Text nodes such as whitespace.
    *
    * @param n     The parent in which to search for children
    * @return      The first child Element of n, or null if none
    */
    public static Element getFirstChildElement(Node n)
    {
        Node child = n.getFirstChild();
        while ((child != null) && (child.getNodeType() != Node.ELEMENT_NODE))
        {
            child = child.getNextSibling();
        }
        if (child != null)
        {
            return (Element) child;
        }
        else
        {
            return null;
        }

    } // getFirstChildElement()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    * This is a convenience method to pretty format an Element.
    * The XML delcaration is included.  If you want pretty printing,
    * but not the XML delcaration (or visa-versa), please do not create
    * another method here.  Use the most flexible version that already exists.
    */
    public static String toString(Element oElt)
    {
        return toString(oElt, true, false);

    } // toString()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    * Serializes the specified element.
    * @param oElt - The element to serialize
    * @param bIndent - Whether or not to pretty print
    * @param bOmitXMLDeclaration - Whether or not to omit the XML preamble
    */
    public static String toString(
        Element oElt, bool bIndent, bool bOmitXMLDeclaration)
    {
        OutputFormat oFormat = new OutputFormat("XML", "UTF-8", bIndent);
        oFormat.setOmitXMLDeclaration(bOmitXMLDeclaration);
        return toString(oElt, oFormat);

    } // toString()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    * Serializes the specified element.
    * @param oElt - The element to serialize
    * @param oFormat - The output format to use
    */
    public static String toString(Element oElt, OutputFormat oFormat)
    {
        StringWriter oWriter = new StringWriter();
        XMLSerializer oSerialize = new XMLSerializer(oWriter, oFormat);

        try
        {
        	oElt.getOwnerDocument().normalizeDocument();
            oSerialize.serialize(oElt);
        }
        catch (java.io.IOException ioe)
        {
            throw new RuntimeException(
                "Problem serializing the XML document:\n" + ioe.getMessage());
        }

        return oWriter.toString();

    } // toString()

    /*
    ****************************************************************************
    * fromXMLDateTime()
    ****************************************************************************
    */ /**
    * Converts the specified String in xsd:DateTime format (GMT) to a Java Date
    * object
    */
    public static Date fromXMLDateTime(String sTime)
        throws ParseException
    {
        int nDot = sTime.indexOf('.');
        SimpleDateFormat oDF =
            (nDot > 0) ? new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'")
                       : new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'");

        oDF.setTimeZone(TimeZone.getTimeZone("GMT"));
        return oDF.parse(sTime);

    } // fromXMLDateTime()

    /*
    ****************************************************************************
    * toXMLDateTime()
    ****************************************************************************
    */ /**
    *  Converts the Java Date object to a xsd:DateTime String in GMT.
    */
    public static String toXMLDateTime(Date oTime)
    {
        SimpleDateFormat oDF =
            new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
        oDF.setTimeZone(TimeZone.getTimeZone("GMT"));
        return oDF.format(oTime);

    } // toXMLDateTime()
    
    /**
     * Gets the text string associated with a node
     * @param root <code>Node</code> object from which text to be extracted.
     * @return a string associated with the node
     */
    public static String getText( Node root )
    {
            NodeList list = root.getChildNodes();
            if( (list == null) && (list.getLength() == 0) )
            {
                    String str = root.getNodeValue();
                    if( str == null )
                    {
                            str = "";
                    }
                    return str;
            }
            StringBuffer buf = new StringBuffer();
            for( int i = 0; i < list.getLength(); i++ )
            {
                    Node node = list.item(i);
                    if( node instanceof TextImpl )
                    {
                            String val = ((TextImpl) node).getNodeValue();
                            if( (val != null) && (val.length() > 0) )
                            {
                                    buf.append(val);
                            }
                    }
            }
            return buf.toString();
    }


    /**
     * Converts a <code>Document</code> into a formated XML string.
     * @param doc <code>Document</code> to be converted into a string.
     * @return a string representing the formated XML document
     */
    public static String toString( Document doc )
    {
            StringWriter str = new StringWriter();

            try
            {
                OutputFormat out = new OutputFormat(doc);
                out.setIndenting(true);
                out.setIndent(1);
                out.setStandalone(false);
                out.setEncoding("UTF-8");
                out.setLineWidth(0);

                XMLSerializer ser = new XMLSerializer(str, out);
                
                // normalizeDocument adds "xmlns" attributes, among other normalization
                doc.normalizeDocument(); // XXX: should we clone the document?
                ser.serialize(doc.getDocumentElement());  
            }
            catch( Exception e )
            {
                return null;
            }

            return str.toString();
    }

} // Class: DOMUtils
