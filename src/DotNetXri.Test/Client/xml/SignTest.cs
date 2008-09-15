/*
 * Copyright 2005 OpenXRI Foundation
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
namespace DotNetXri.Client.Xml {


using java.security.KeyPair;
using java.security.KeyPairGenerator;
using junit.framework.Test;
using junit.framework.TestCase;
using junit.framework.TestSuite;
using junit.textui.TestRunner;
using org.openxri.saml.Assertion;
using org.openxri.saml.Attribute;
using org.openxri.saml.AttributeStatement;
using org.openxri.saml.NameID;
using org.openxri.saml.Subject;


/*
********************************************************************************
* Class: SignTest
********************************************************************************
*/ /**
* @author =chetan
*
* Tests signing and verifying the XRI Descriptor
*/
public class SignTest
    :TestCase
{
    /*
    ****************************************************************************
    * main()
    ****************************************************************************
    */ /**
    *
    */
    public static void main(String[] args)
    {
        // Pass control to the non-graphical test runner
        TestRunner.run(suite());

    } // main()

    /*
    ****************************************************************************
    * suite()
    ****************************************************************************
    */ /**
    *
    */
    public static Test suite()
    {
        return new TestSuite(SignTest.class);

    } // suite()

    /*
    ****************************************************************************
    * testSignVerify()
    ****************************************************************************
    */ /**
    * Test sign/verify an XRD
    */
    public void testSignVerify()
    {
        try
        {
            KeyPairGenerator oKPG = KeyPairGenerator.getInstance("RSA");
            KeyPair oKP = oKPG.genKeyPair();

            XRD oXRID = new XRD();
            oXRID.genXmlID();
            Service authSrv = new Service();
            authSrv.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=saml");
            authSrv.addType(Tags.SERVICE_AUTH_RES);
            authSrv.addURI("http://xri.epok.com");
            oXRID.addService(authSrv);

            String sParentID = "xyz";

            // create the SAMLSubject
            Subject oSubject = new Subject();
            NameID oName = new NameID(Tags.TAG_NAMEID);
            oName.setNameQualifier(sParentID);
            oName.setValue(".epok");
            oSubject.setNameID(oName);

            // create the SAML Attribute Statement
            AttributeStatement oAttrStatement = new AttributeStatement();
            Attribute oAttr = new Attribute();
            oAttr.setValue("#" + oXRID.getXmlID());
            oAttr.setName(Tags.TAG_XRD);
            oAttrStatement.setAttribute(oAttr);

            // now we can create the true SAML Assertion
            Assertion oAssertion = new Assertion();
            NameID oIssuer = new NameID(Tags.TAG_ISSUER);
            oIssuer.setValue("World's Best");
            oAssertion.setIssuer(oIssuer);
            oAssertion.setSubject(oSubject);
            oAssertion.setAttrStatement(oAttrStatement);
            oXRID.setSAMLAssertion(oAssertion);

            oXRID.sign(oKP.getPrivate());
            oXRID.verifySignature(oKP.getPublic());
            String sXRID = oXRID.serializeDOM(false, true);
            Logger.Info("OLD XRD: " + sXRID);

            XRD oNewXRID =
                XRD.parseXRD(sXRID, true);
            String sNewXRID = oNewXRID.serializeDOM(false, true);
            Logger.Info("NEW XRD: " + sNewXRID);
            assertTrue(
                "Unable to reconstruct DOM correctly", sXRID.Equals(sNewXRID));
            oNewXRID.verifySignature(oKP.getPublic());
        }
        catch (Exception e)
        {
            e.printStackTrace();
            assertTrue(
                "Unexpected exception using XRD assertion", false);
        }

    } // testSignVerify()

} // Class: SignTest
}