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
package org.openxri.xml;

using org.openxri.xml.Service;
using org.openxri.xml.XRD;
using junit.framework.Test;
using junit.framework.TestCase;
using junit.framework.TestSuite;
using junit.textui.TestRunner;


/*
********************************************************************************
* Class: ParseTest
********************************************************************************
*/ /**
* @author =chetan
*
* To change the template for this generated type comment go to
* Window>Preferences>Java>Code Generation>Code and Comments
*/
public class ParseTest
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
        return new TestSuite(ParseTest.class);

    } // suite()

    // suite()    

    /*
    ****************************************************************************
    * testSerialization()
    ****************************************************************************
    */ /**
    *
    */
    public void testSerialization()
    {
        //J-
        String sVal =
            "<XRD\n" +
            " xml:id=\"1f81b6e0-b64b-1026-f1bc-c0a80b9d3f5b\">\n" +
            "  <Query>.foo</Query>\n" +
            "  <Status code=\"0\"/>\n" +
            "  <ProviderID>\n" +
            "    urn:uuid:D5CFA9CB-F43B-228C-3CEE-C5E9F2D3CB15\n" +
            "  </ProviderID>\n" +
            "  <Service>\n" +
            "    <ProviderID>\n" +
            "      urn:uuid:C5C9EECF-A3BC-4883-8852-8EECB54CE1D5\n" +
            "    </ProviderID>\n" +
            "    <Type>xri://$res*auth*($v*2.0)</Type>\n" +
            "    <MediaType>application/xrds+xml;trust=none</MediaType>\n" +
            "    <MediaType>application/xrds+xml;trust=saml</MediaType>\n" +
            "    <MediaType>application/xrds+xml;trust=https</MediaType>\n" +
            "    <MediaType>application/xrds+xml;trust=saml+https</MediaType>\n" +
            "    <URI>http://test.epok.net/foo/bar</URI>\n" +
            "    <URI>https://test.epok.net/foo/bar</URI>\n" +
            "    <URI>http://test.epok.net/foo/bar</URI>\n" +
            "    <ds:KeyInfo\n" +
            "     xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\">\n" +
            "    </ds:KeyInfo>\n" +
            "  </Service>\n" +
            "  <Service>\n" +
            "    <Type>xri:@epok/foo</Type>\n" +
            "    <MediaType>application/vnd.epok.foo</MediaType>\n" +
            "  </Service>\n" +
            "  <Service>\n" +
            "    <Type>xri:@epok/foo2</Type>\n" +
            "    <MediaType>application/vnd.epok.foo2</MediaType>\n" +
            "    <URI>http://foo2.epok.net/foo/bar</URI>\n" +
            "    <Custom>Custom Data</Custom>\n" +
            "    <Custom2>Custom Data2</Custom2>\n" +
            "  </Service>\n" +
            "  <LocalID>xri://@!1!3/!5/*internal</LocalID>" +
            "  <Ref>xri://@!1!3/!5/*external</Ref>" +
            "</XRD>\n";
        //J+
        try
        {
            XRD oDesc =
                XRD.parseXRD(sVal, false);

            // Resolved
            assertTrue(
                "Resolved component failed.  Expected .foo, got " +
                oDesc.getQuery(), oDesc.getQuery().equals(".foo"));

            // AuthorityID
            assertEquals(
                "urn:uuid:D5CFA9CB-F43B-228C-3CEE-C5E9F2D3CB15",
                oDesc.getProviderID().trim());

            Service authService = oDesc.getServiceAt(0);
            // Authority
            assertNotNull(authService);

            // Authority/AuthorityID
            assertEquals(
                "urn:uuid:C5C9EECF-A3BC-4883-8852-8EECB54CE1D5",
                authService.getProviderId().trim());

            // Authority/URI(s)
            assertEquals(3, authService.getNumURIs());
            assertEquals(
                "http://test.epok.net/foo/bar",
                authService.getURIForScheme("http").getURI().toString());
            assertEquals(
                "https://test.epok.net/foo/bar",
                authService.getURIForScheme("https").getURI().toString());

            // Authority/URI(s)
            /*
            assertEquals(3, oDesc.getServiceForType(Tags.SERVICE_AUTH_TRUSTED).getNumURIs());
            assertEquals(
                "http://test.epok.net/foo/bar",
                oDesc.getXRIAuthority().getTrustedURIForScheme("http") + "");
                */
            
            // Authority/KeyInfo
            assertNotNull(authService.getKeyInfo());

            // Services
            Service oLA = oDesc.getServiceAt(1);
            assertTrue("Incorrect type", oLA.getTypeAt(0).getType().equals("xri:@epok/foo"));
            assertTrue("Incorrect URI for Local Access", oLA.getNumURIs() == 0);

            oLA = oDesc.getServiceAt(2);
            assertTrue("Incorrect type", oLA.getTypeAt(0).getType().equals("xri:@epok/foo2"));
            assertTrue(
                "Incorrect URI for Local Access2", oLA.getNumURIs() == 1);
            assertTrue(
                "Incorrect Value for Custom Data",
                oLA.getOtherTagValues("Custom") != null);
            assertTrue(
                "Incorrect Value for Custom Data2",
                oLA.getOtherTagValues("Custom2") != null);

            // Synonyms
            assertTrue(
                "Incorrect # for Internal", oDesc.getNumLocalIDs() == 1);
            assertTrue(
                "Incorrect # for External", oDesc.getNumRefs() == 1);

            System.Console.WriteLine(oDesc.toString());
        }
        catch (Exception e)
        {
            e.printStackTrace();
            assertTrue("Caught unexpected exception " + e.toString(), false);
        }

    } // testSerialization()

    /*
    ****************************************************************************
    * testSignedDescriptor()
    ****************************************************************************
    */ /**
    *
    */
    public void testSignedDescriptor()
    {
        //J-
        String sVal = ""
            + "<XRD\n"
            + " xmlns=\"\"\n"
            + " DescriptorID=\"a93853082806b81c173c1434c984fe20\"\n"
            + " id=\"#foo\">\n"
            + "  <Authority xmlns=\"\">\n"
            + "    <URI xmlns=\"\">http://xri.epok.com</URI>\n"
            + "  </Authority>\n"
            + "  <Assertion\n"
            + "   xmlns=\"urn:oasis:names:tc:SAML:2.0:assertion\"\n"
            + "   xmlns:saml=\"urn:oasis:names:tc:SAML:2.0:assertion\"\n"
            + "   xmlns:samlp=\"urn:oasis:names:tc:SAML:2.0:protocol\"\n"
            + "   ID=\"ff83e3a7a1ac06392653a1b2147cc535\"\n"
            + "   IssueInstant=\"2005-01-27T21:29:10.384Z\"\n"
            + "   MajorVersion=\"2\"\n"
            + "   MinorVersion=\"0\">\n"
            + "      <Issuer>World's Best</Issuer>\n"
            + "      <Subject>\n"
            + "        <NameID>\n"
            + "          .epok\n"
            + "        </NameID>\n"
            + "      </Subject>\n"
            + "    <ds:Signature\n"
            + "       xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\">\n"
            + "      <ds:SignedInfo>\n"
            + "        <ds:CanonicalizationMethod\n"
            + "         Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\"/>\n"
            + "        <ds:SignatureMethod\n"
            + "         Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\"/>\n"
            + "        <ds:Reference\n"
            + "         URI=\"#a93853082806b81c173c1434c984fe20\">\n"
            + "          <ds:Transforms>\n"
            + "            <ds:Transform\n"
            + "             Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\"/>\n"
            + "            <ds:Transform\n"
            + "             Algorithm=\"http://www.w3.org/2001/10/xml-exc-c14n#\">\n"
            + "              <ec:InclusiveNamespaces\n"
            + "               xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\"\n"
            + "               xmlns:ec=\"http://www.w3.org/2001/10/xml-exc-c14n#\"\n"
            + "               PrefixList=\"#default code ds kind rw saml samlp typens\"/>\n"
            + "            </ds:Transform>\n"
            + "          </ds:Transforms>\n"
            + "          <ds:DigestMethod\n"
            + "           Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\"/>\n"
            + "          <ds:DigestValue>\n"
            + "            p6QbBu6uKTfM6pQ73mgBlyzJOYE=\n"
            + "          </ds:DigestValue>\n"
            + "        </ds:Reference>\n"
            + "      </ds:SignedInfo>\n"
            + "      <ds:SignatureValue>\n"
            + "l94Yfpu5RIexeBywCH1mPyVlOgotqwdEhqdXsmxyDeLyw2RjkT671rkVv102YEMyLghudpC8GE75\n"
            + "tvcIc3Nm7q+7vX8ygdPxoyHlSnQda7yblgcW1EtPQWKD5sor5ue9hGHVukvtgXS8WRcIa4AYhnyM\n"
            + "onchjRVZwx7+AjnHyFs=\n"
            + "      </ds:SignatureValue>\n"
            + "    </ds:Signature>\n"
            + "    <AttributeStatement>\n"
            + "      <Attribute\n"
            + "       Name=\"XRD\">\n"
            + "        <AttributeValue>\n"
            + "          #foo\n"
            + "        </AttributeValue>\n"
            + "      </Attribute>\n"
            + "    </AttributeStatement>\n"
            + "  </Assertion>\n"
            + "</XRD>\n";
        //J+
        try
        {
            XRD oDesc =
                XRD.parseXRD(sVal, false);

            assertNotNull(oDesc.getSAMLAssertion());

            System.Console.WriteLine(oDesc.toString());
        }
        catch (Exception e)
        {
            e.printStackTrace();
            assertTrue("Caught unexpected exception " + e.toString(), false);
        }

    } // testSignedDescriptor()

} // Class: ParseTest
