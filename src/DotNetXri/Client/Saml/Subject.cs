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
package org.openxri.saml;

import org.apache.xerces.dom.DocumentImpl;
import org.openxri.util.DOMUtils;
import org.openxri.xml.Tags;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;


/*
********************************************************************************
* Class: Subject
********************************************************************************
*/ /**
* This class provides encapsulation of a SAML 2.0 Subject element
* @author =chetan
*/
public class Subject
{
    private NameID moNameID;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public Subject(Element oElem)
    {
        fromDOM(oElem);

    } // Constructor()

    /*
    ****************************************************************************
    * reset()
    ****************************************************************************
    */ /**
    * Resets the internal state of this obj
    */
    public void reset()
    {
        moNameID = null;

    } // reset()

    /*
    ****************************************************************************
    * fromDOM()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public void fromDOM(Element oElem)
    {
        reset();

        for (
            Node oChild = DOMUtils.getFirstChildElement(oElem); oChild != null;
            oChild = DOMUtils.getNextSiblingElement(oChild))
        {
            if (oChild.getLocalName().equals(Tags.TAG_NAMEID))
            {
                // only accept the first XRIAuthority
                if (moNameID == null)
                {
                    moNameID = new NameID((Element) oChild);
                }
            }
        }

    } // fromDOM()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  Creates an empty SAML Subject element
    */
    public Subject() {} // Constructor()

    /*
    ****************************************************************************
    * getNameID()
    ****************************************************************************
    */ /**
    * Returns the nameid element
    */
    public NameID getNameID()
    {
        return moNameID;

    } // getNameID()

    /*
    ****************************************************************************
    * setNameID()
    ****************************************************************************
    */ /**
    * Sets the nameid element
    */
    public void setNameID(NameID oVal)
    {
        moNameID = oVal;

    } // setNameID()

    /*
    ****************************************************************************
    * toDOM()
    ****************************************************************************
    */ /**
    *  This method will make DOM using the specified document.  If any DOM state
    * has been stored with the obj, it will not be used in this method.
    * This method generates a reference-free copy of new DOM.
    * @param oDoc - The document to use for generating DOM
    */
    public Element toDOM(Document oDoc)
    {
        // for this particular toDOM implementation, oDoc must not be null
        if (oDoc == null)
        {
            return null;
        }

        Element oElem = oDoc.createElementNS(Tags.NS_SAML, Tags.TAG_SUBJECT);

        if (moNameID != null)
        {
            Element oChildElem = (Element) moNameID.toDOM(oDoc);
            oElem.appendChild(oChildElem);
        }

        return oElem;

    } // toDOM()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    * Returns formatted obj.  Do not use if signature needs to be preserved.
    */
    public String toString()
    {
        return dump();

    } // toString()

    /*
    ****************************************************************************
    * dump()
    ****************************************************************************
    */ /**
    * Returns obj as a formatted XML string.
    * @param sTab - The characters to prepend before each new line
    */
    public String dump()
    {
        Document doc = new DocumentImpl();
        Element elm = this.toDOM(doc);
        doc.appendChild(elm);
        return DOMUtils.toString(doc);

    } // dump()

} // Class: Subject
