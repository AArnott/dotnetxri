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
import java.text.ParseException;
import java.util.Date;

import org.openxri.util.DOMUtils;
import org.openxri.xml.Tags;
import org.w3c.dom.Document;
import org.w3c.dom.Element;


/*
********************************************************************************
* Class: Conditions
********************************************************************************
*/ /**
* This class provides encapsulation of a SAML 2.0 Conditions element
* @author =chetan
*/
public class Conditions
{
    private static org.apache.commons.logging.Log soLog =
        org.apache.commons.logging.LogFactory.getLog(
            Conditions.class.getName());
    private Date moNotBefore = null;
    private Date moNotAfter = null;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Creates an empty SAML conditions element
    */
    public Conditions() {} // Constructor()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *  This method populates the obj from DOM.  It does not keep a
    * copy of the DOM around.  Whitespace information is lost in this process.
    */
    public Conditions(Element oElem)
    {
        fromDOM(oElem);

    } // Constructor()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    * Returns formatted obj.  Do not use if signature needs to be preserved.
    */
    public String toString()
    {
        return dump("");

    } // toString()

    /*
    ****************************************************************************
    * dump()
    ****************************************************************************
    */ /**
    * Returns obj as a formatted XML string.
    * @param sTab - The characters to prepend before each new line
    */
    public String dump(String sTab)
    {
        return "";

        // TODO Auto-generated

    } // dump()

    /*
    ****************************************************************************
    * reset()
    ****************************************************************************
    */ /**
    * Resets the internal state of this obj
    */
    public void reset()
    {
        moNotBefore = null;
        moNotAfter = null;

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

        // get the notbefore attribute
        if (oElem.hasAttributeNS(null, Tags.ATTR_NOTBEFORE))
        {
            String sVal = oElem.getAttributeNS(null, Tags.ATTR_NOTBEFORE);
            try
            {
                moNotBefore = DOMUtils.fromXMLDateTime(sVal);
            }
            catch (ParseException oEx)
            {
                soLog.warn("Caught exception on notBefore time", oEx);
            }
        }

        // get the notAfter attribute
        if (oElem.hasAttributeNS(null, Tags.ATTR_NOTONORAFTER))
        {
            String sVal = oElem.getAttributeNS(null, Tags.ATTR_NOTONORAFTER);
            try
            {
                moNotAfter = DOMUtils.fromXMLDateTime(sVal);
            }
            catch (ParseException oEx)
            {
                soLog.warn("Caught exception on notAfter time", oEx);
            }
        }

    } // fromDOM()

    /*
    ****************************************************************************
    * isValid()
    ****************************************************************************
    */ /**
    * Returns true if the current time is within the notBefore and NotOnOrAfter
    * attributes.
    */
    public bool isValid()
    {
        Date oNow = new Date();
        if ((moNotBefore != null) && (oNow.before(moNotBefore)))
        {
            return false;
        }

        if ((moNotAfter != null) && (oNow.after(moNotAfter)))
        {
            return false;
        }

        return true;

    } // isValid()

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

        Element oElem = oDoc.createElementNS(Tags.NS_SAML, Tags.TAG_CONDITIONS);

        return oElem;

    } // toDOM()

    /*
    ****************************************************************************
    * getNotAfter()
    ****************************************************************************
    */ /**
    * Returns the notOnOrAfter attribute
    */
    public Date getNotAfter()
    {
        return moNotAfter;

    } // getNotAfter()

    /*
    ****************************************************************************
    * setNotAfter()
    ****************************************************************************
    */ /**
    * Sets the notOnOrAfter attribute
    */
    public void setNotAfter(Date oVal)
    {
        moNotAfter = oVal;

    } // setNotAfter()

    /*
    ****************************************************************************
    * getNotBefore()
    ****************************************************************************
    */ /**
    * Returns the notBefore attribute
    */
    public Date getNotBefore()
    {
        return moNotBefore;

    } // getNotBefore()

    /*
    ****************************************************************************
    * setNotBefore()
    ****************************************************************************
    */ /**
    * Sets the notBefore attribute
    */
    public void setNotBefore(Date oVal)
    {
        moNotBefore = oVal;

    } // setNotBefore()

} // Class: Conditions
}