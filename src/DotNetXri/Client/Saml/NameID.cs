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
namespace DotNetXri.Client.Saml {
using org.apache.xerces.dom.DocumentImpl;
using org.openxri.util.DOMUtils;
using org.openxri.xml.Tags;
using org.w3c.dom.Document;
using org.w3c.dom.Element;
using org.w3c.dom.Node;


/*
********************************************************************************
* Class: NameID
********************************************************************************
*/ /**
* This class provides encapsulation of a SAML 2.0 NameID element type
* @author =chetan
*/
public class NameID
{
    private String msTag = "";
    private String msNQ = "";
    private String msValue = "";

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public NameID(Element oElem)
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
        msTag = "";
        msNQ = "";
        msValue = "";

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

        msTag = oElem.getLocalName();

        // get the id attribute
        if (oElem.hasAttributeNS(null, Tags.ATTR_NAMEQUALIFIER))
        {
            msNQ = oElem.getAttributeNS(null, Tags.ATTR_NAMEQUALIFIER);
        }

        Node oChild = oElem.getFirstChild();
        if ((oChild != null) && (oChild.getNodeType() == Node.TEXT_NODE))
        {
            msValue = oChild.getNodeValue();
        }

    } // fromDOM()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructs an empty SAML NameID obj.
    * @param sTag - The tag name for the element during serialization
    */
    public NameID(String sTag)
    {
        msTag = sTag;

    } // Constructor()

    /*
    ****************************************************************************
    * setNameQualifier()
    ****************************************************************************
    */ /**
    * Sets the nameQualifier attribute
    */
    public void setNameQualifier(String sVal)
    {
        msNQ = sVal;

    } // setNameQualifier()

    /*
    ****************************************************************************
    * getNameQualifier()
    ****************************************************************************
    */ /**
    * Returns the nameQualifier attribute
    */
    public String getNameQualifier()
    {
        return msNQ;

    } // getNameQualifier()

    /*
    ****************************************************************************
    * setValue()
    ****************************************************************************
    */ /**
    * Sets the text value
    */
    public void setValue(String sVal)
    {
        msValue = sVal;

    } // setValue()

    /*
    ****************************************************************************
    * getValue()
    ****************************************************************************
    */ /**
    * Returns the text value
    */
    public String getValue()
    {
        return msValue;

    } // getValue()

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

        Element oElem = oDoc.createElementNS(Tags.NS_SAML, msTag);

        if (!msNQ.equals(""))
        {
            oElem.setAttributeNS(null, Tags.ATTR_NAMEQUALIFIER, msNQ);
        }

        oElem.appendChild(oDoc.createTextNode(msValue));

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

} // Class: NameID
}