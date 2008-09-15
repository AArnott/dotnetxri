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

using org.apache.xerces.dom.XmlDocument;
using org.openxri.util.DOMUtils;
using org.openxri.xml.Tags;
using org.w3c.dom.XmlDocument;
using org.w3c.dom.XmlElement;
using org.w3c.dom.Node;


/*
********************************************************************************
* Class: AttributeStatement
********************************************************************************
*/ /**
* This class provides encapsulation of a SAML 2.0 Attribute Statement element
* @author =chetan
*/
public class AttributeStatement
{
    private Attribute moAttr;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public AttributeStatement(XmlElement oElem)
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
        moAttr = null;

    } // reset()

    /*
    ****************************************************************************
    * fromDOM()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public void fromDOM(XmlElement oElem)
    {
        reset();

        for (
            Node oChild = oElem.FirstChild; oChild != null;
            oChild = oChild.NextSibling)
        {
            if (oChild.LocalName.Equals(Tags.TAG_ATTRIBUTE))
            {
                // only accept the first XRIAuthority
                if (moAttr == null)
                {
                    moAttr = new Attribute((XmlElement) oChild);
                }
            }
        }

    } // fromDOM()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructs an empty SAML attribute statement
    */
    public AttributeStatement() {} // Constructor()

    /*
    ****************************************************************************
    * GetAttribute()
    ****************************************************************************
    */ /**
    * Returns the first attribute element in the statement
    */
    public Attribute GetAttribute()
    {
        return moAttr;

    } // GetAttribute()

    /*
    ****************************************************************************
    * SetAttribute()
    ****************************************************************************
    */ /**
    * Sets the attribute for this statement.  Only 1 attribute at a time is
    * supported.
    */
    public void SetAttribute(Attribute oVal)
    {
        moAttr = oVal;

    } // SetAttribute()

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
    public XmlElement toDOM(XmlDocument oDoc)
    {
        // for this particular toDOM implementation, oDoc must not be null
        if (oDoc == null)
        {
            return null;
        }

        XmlElement oElem =
            oDoc.createElementNS(Tags.NS_SAML, Tags.TAG_ATTRIBUTESTATEMENT);

        if (moAttr != null)
        {
            XmlElement oChildElem = (XmlElement) moAttr.toDOM(oDoc);
            oElem.AppendChild(oChildElem);
        }

        return oElem;

    } // toDOM()

    /*
    ****************************************************************************
    * ToString()
    ****************************************************************************
    */ /**
    * Returns formatted obj.  Do not use if signature needs to be preserved.
    */
    public override string ToString()
    {
        return dump();

    } // ToString()

    /*
    ****************************************************************************
    * dump()
    ****************************************************************************
    */ /**
    * Returns obj as a formatted XML string.
    * @param sTab - The characters to prepend before each new line
    */
    public string dump()
    {
        XmlDocument doc = new XmlDocument();
        XmlElement elm = this.toDOM(doc);
        doc.AppendChild(elm);
        return DOMUtils.ToString(doc);

    } // dump()

} // Class: AttributeStatement
}