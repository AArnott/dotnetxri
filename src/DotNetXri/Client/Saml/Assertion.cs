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
using org.apache.xml.security.signature.XMLSignature;
using org.openxri.util.DOMUtils;
using org.openxri.xml.Tags;
using org.w3c.dom.Document;
using org.w3c.dom.Element;
using org.w3c.dom.Node;


/*
********************************************************************************
* Class: Assertion
********************************************************************************
*/ /**
* This class provides encapsulation of a SAML 2.0 Assertion
* @author =chetan
*/
public class Assertion
{
    private static org.apache.commons.logging.Log soLog =
        org.apache.commons.logging.LogFactory.getLog(Assertion.class.getName());
    private String msXmlID = "";
    private String msIssueInstant = "";
    private Element moElem;
    private NameID moIssuer;
    private XMLSignature moSignature;
    private Subject moSubject;
    private Conditions moConditions;
    private AttributeStatement moAttrStatement;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Contructs a SAML 2.0 assertion and generates an id for it
    */
    public Assertion()
    {
        genXmlID();

    } // Constructor()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Contructs a SAML 2.0 assertion from the specified DOM
    */
    public Assertion(Element oElem)
    {
        fromDOM(oElem);

    } // Constructor()

    /*
    ****************************************************************************
    * getIssueInstant()
    ****************************************************************************
    */ /**
    * Returns the issueInstant attribute
    */
    public String getIssueInstant()
    {
        return msIssueInstant;

    } // getIssueInstant()

    /*
    ****************************************************************************
    * setIssueInstant()
    ****************************************************************************
    */ /**
    * Sets the issueInstant attribute
    */
    public void setIssueInstant(String sVal)
    {
        msIssueInstant = sVal;

    } // setIssueInstant()

    /*
    ****************************************************************************
    * getXmlID()
    ****************************************************************************
    */ /**
    * Returns the id attribute
    */
    public String getXmlID()
    {
        return msXmlID;

    } // getXmlID()

    /*
    ****************************************************************************
    * setXmlID()
    ****************************************************************************
    */ /**
    * Sets the id attribute
    */
    public void setXmlID(String sVal)
    {
        msXmlID = sVal;

    } // setXmlID()

    /*
    ****************************************************************************
    * genXmlID()
    ****************************************************************************
    */ /**
    * Sets the id attribute with a newly generated id
    */
    public void genXmlID()
    {
        msXmlID = "_" + org.openxri.util.XMLUtils.genXmlID();

    } // genXmlID()

    /*
    ****************************************************************************
    * setDOM()
    ****************************************************************************
    */ /**
    *  This method will import from DOM, and hold on to it, as
    * retrievable by getDOM.  The fromDOM method, on the otherhand, will not keep
    * a copy of the DOM.
    */
    public void setDOM(Element oElem)
    {
        fromDOM(oElem);
        moElem = oElem;

    } // setDOM()

    /*
    ****************************************************************************
    * reset()
    ****************************************************************************
    */ /**
    * Resets the internal state of this obj
    */
    public void reset()
    {
        msXmlID = "";
        msIssueInstant = "";
        moElem = null;
        moIssuer = null;
        moSignature = null;
        moSubject = null;
        moConditions = null;
        moAttrStatement = null;

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

        // get the id attribute
        if (oElem.hasAttributeNS(null, Tags.ATTR_ID_CAP))
        {
            msXmlID = oElem.getAttributeNS(null, Tags.ATTR_ID_CAP);
        }

        if (oElem.hasAttributeNS(null, Tags.ATTR_ISSUEINSTANT))
        {
            msIssueInstant = oElem.getAttributeNS(null, Tags.ATTR_ISSUEINSTANT);
        }

        for (
            Node oChild = DOMUtils.getFirstChildElement(oElem); oChild != null;
            oChild = DOMUtils.getNextSiblingElement(oChild))
        {
            if (oChild.getLocalName().equals(Tags.TAG_ISSUER))
            {
                // only accept the first XRIAuthority
                if (moIssuer == null)
                {
                    moIssuer = new NameID((Element) oChild);
                }
            }
            else if (oChild.getLocalName().equals(Tags.TAG_SIGNATURE))
            {
                // only accept the first XRIAuthority
                if (moSignature == null)
                {
                    try
                    {
                        Document oDoc = new DocumentImpl();
                        Element oChildCopy =
                            (Element) oDoc.importNode(oChild, true);
                        moSignature = new XMLSignature(oChildCopy, null);
                    }
                    catch (Exception oEx)
                    {
                        soLog.warn(
                            "Caught exception while parsing Signature", oEx);
                    }
                }
            }
            else if (oChild.getLocalName().equals(Tags.TAG_SUBJECT))
            {
                // only accept the first XRIAuthority
                if (moSubject == null)
                {
                    moSubject = new Subject((Element) oChild);
                }
            }
            else if (oChild.getLocalName().equals(Tags.TAG_CONDITIONS))
            {
                // only accept the first XRIAuthority
                if (moConditions == null)
                {
                    moConditions = new Conditions((Element) oChild);
                }
            }
            else if (oChild.getLocalName().equals(Tags.TAG_ATTRIBUTESTATEMENT))
            {
                // only accept the first XRIAuthority
                if (moAttrStatement == null)
                {
                    moAttrStatement = new AttributeStatement((Element) oChild);
                }
            }
        }

    } // fromDOM()

    /*
    ****************************************************************************
    * getDOM()
    ****************************************************************************
    */ /**
    * This method returns DOM stored with this obj.  It may be cached and
    * there is no guarantee as to which document it was created from
    */
    public Element getDOM()
    {
        if (moElem == null)
        {
            moElem = toDOM(new DocumentImpl());
            moElem.getOwnerDocument().appendChild(moElem);
        }

        return moElem;

    } // getDOM()

    /*
    ****************************************************************************
    * isValid()
    ****************************************************************************
    */ /**
    * Returns true of the assertion is valid.  Checks validity period in
    * conditions and required fields and attributes
    */
    public bool isValid()
    {
        if (
            (msIssueInstant.equals("")) || (msXmlID.equals("")) ||
            (moIssuer == null))
        {
            return false;
        }

        if ((moConditions != null) && (!moConditions.isValid()))
        {
            return false;
        }

        return true;

    } // isValid()

    /*
    ****************************************************************************
    * getAttributeStatement()
    ****************************************************************************
    */ /**
    * Returns the first attribute statement in the assertion
    */
    public AttributeStatement getAttributeStatement()
    {
        return moAttrStatement;

    } // getAttributeStatement()

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

        Element oElem = oDoc.createElementNS(Tags.NS_SAML, Tags.TAG_ASSERTION);

        oElem.setAttributeNS(Tags.NS_XMLNS, "xmlns", Tags.NS_SAML);
        oElem.setAttributeNS(Tags.NS_XMLNS, "xmlns:saml", Tags.NS_SAML);
        oElem.setPrefix("saml");
        oElem.setAttributeNS(null, Tags.ATTR_VERSION, "2.0");
        oElem.setAttributeNS(null, Tags.ATTR_ID_CAP, msXmlID);
        oElem.setAttributeNS(null, Tags.ATTR_ISSUEINSTANT, msIssueInstant);

        if (moIssuer != null)
        {
            Element oChildElem = (Element) moIssuer.toDOM(oDoc);
            oElem.appendChild(oChildElem);
        }

        if (moSignature != null)
        {
            Element oChildElem = moSignature.getElement();

            // import the node, we want a copy
            oChildElem = (Element) oDoc.importNode(oChildElem, true);
            oElem.appendChild(oChildElem);
        }

        if (moSubject != null)
        {
            Element oChildElem = (Element) moSubject.toDOM(oDoc);
            oElem.appendChild(oChildElem);
        }

        if (moConditions != null)
        {
            Element oChildElem = (Element) moConditions.toDOM(oDoc);
            oElem.appendChild(oChildElem);
        }

        if (moAttrStatement != null)
        {
            Element oChildElem = (Element) moAttrStatement.toDOM(oDoc);
            oElem.appendChild(oChildElem);
        }

        return oElem;

    } // toDOM()

    /*
    ****************************************************************************
    * getSubject()
    ****************************************************************************
    */ /**
    * Returns the subject of this assertion
    */
    public Subject getSubject()
    {
        return moSubject;

    } // getSubject()

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

    /*
    ****************************************************************************
    * getIssuer()
    ****************************************************************************
    */ /**
    *Returns the issuer element of the assertion
    */
    public NameID getIssuer()
    {
        return moIssuer;

    } // getIssuer()

    /*
    ****************************************************************************
    * setIssuer()
    ****************************************************************************
    */ /**
    * Sets the issuer element of the assertion
    */
    public void setIssuer(NameID oIssuer)
    {
        moIssuer = oIssuer;

    } // setIssuer()

    /*
    ****************************************************************************
    * getConditions()
    ****************************************************************************
    */ /**
    * Returns the conditions element of the assertion
    */
    public Conditions getConditions()
    {
        return moConditions;

    } // getConditions()

    /*
    ****************************************************************************
    * setConditions()
    ****************************************************************************
    */ /**
    * Sets the conditions element of the assertion
    */
    public void setConditions(Conditions oVal)
    {
        moConditions = oVal;

    } // setConditions()

    /*
    ****************************************************************************
    * setAttrStatement()
    ****************************************************************************
    */ /**
    * Sets the attribute statement element of the assertion.  Only one
    * attribute statement is supported per assertion.
    */
    public void setAttrStatement(AttributeStatement oVal)
    {
        moAttrStatement = oVal;

    } // setAttrStatement()

    /*
    ****************************************************************************
    * setSubject()
    ****************************************************************************
    */ /**
    * Sets the subject element of the assertion
    */
    public void setSubject(Subject oVal)
    {
        moSubject = oVal;

    } // setSubject()

} // Class: Assertion
}