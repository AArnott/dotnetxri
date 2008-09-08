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
using org.w3c.dom.XmlNode;
	using System.Xml;
	using DotNetXri.Client.Xml;


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
    private string msTag = "";
    private string msNQ = "";
    private string msValue = "";

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public NameID(XmlElement oElem)
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
    public void fromDOM(XmlElement oElem)
    {
        reset();

        msTag = oElem.LocalName;

        // get the id attribute
        if (oElem.hasAttributeNS(null, Tags.ATTR_NAMEQUALIFIER))
        {
            msNQ = oElem.getAttributeNS(null, Tags.ATTR_NAMEQUALIFIER);
        }

        XmlNode oChild = oElem.getFirstChild();
        if ((oChild != null) && (oChild.getNodeType() == XmlNode.TEXT_NODE))
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
    public NameID(string sTag)
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
    public void setNameQualifier(string sVal)
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
    public string getNameQualifier()
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
    public void setValue(string sVal)
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
    public string getValue()
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
    public XmlElement toDOM(XmlDocument oDoc)
    {
        // for this particular toDOM implementation, oDoc must not be null
        if (oDoc == null)
        {
            return null;
        }

        XmlElement oElem = oDoc.createElementNS(Tags.NS_SAML, msTag);

        if (!msNQ.Equals(""))
        {
            oElem.setAttributeNS(null, Tags.ATTR_NAMEQUALIFIER, msNQ);
        }

        oElem.AppendChild(oDoc.CreateTextNode(msValue));

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
		return doc.OuterXml;

    } // dump()

} // Class: NameID
}