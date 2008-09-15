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
package org.openxri;

using junit.framework.Test;
using junit.framework.TestCase;
using junit.framework.TestSuite;
using junit.textui.TestRunner;
using java.util.Iterator;
using java.util.Vector;
using org.openxri.RelativeXRI;
using org.openxri.XRI;
using org.openxri.XRIAuthority;
using org.openxri.XRIParseException;
using org.openxri.XRIReference;
using org.openxri.XRISegment;


/*
********************************************************************************
* Class: XRITest
********************************************************************************
*/ /**
* @author =chetan
*
* To change the template for this generated type comment go to
* Window>Preferences>Java>Code Generation>Code and Comments
*/
public class XRITest
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
        return new TestSuite(typeof(XRITest));

    } // suite()

    // suite()    

    /*
    ****************************************************************************
    * Class: XriTestCase
    ****************************************************************************
    */ /**
    *
    */
    static class XriTestCase
    {
        public String msVal;
        public int mnNumAuthSubs;
        public int mnNumRelativeSegments;
        public bool mbAbsolute;

        /*
        ************************************************************************
        * Constructor()
        ************************************************************************
        */ /**
        *
        */
        public XriTestCase(
            String sVal, int nNumAuthSubs, int nNumRelativeSegments,
            bool bAbsolute)
        {
            msVal = sVal;
            mnNumAuthSubs = nNumAuthSubs;
            mnNumRelativeSegments = nNumRelativeSegments;
            mbAbsolute = bAbsolute;

        } // Constructor()

    } // Class: XriTestCase

    /*
    ****************************************************************************
    * testRelativeXRIs()
    ****************************************************************************
    */ /**
    *
    */
    public void testRelativeXRIs()
    {
        Vector oVector = new Vector();

        oVector.add(new XriTestCase("", 0, 0, false));
        oVector.add(new XriTestCase("x", 0, 1, false));
        oVector.add(new XriTestCase("*x", 0, 1, false));
        oVector.add(new XriTestCase("!x", 0, 1, false));
        oVector.add(new XriTestCase("/!x", 0, 1, false));
        oVector.add(new XriTestCase("/x", 0, 1, false));
        oVector.add(new XriTestCase("/.x", 0, 1, false));
        oVector.add(new XriTestCase("./x", 0, 2, false));
        oVector.add(new XriTestCase("../x", 0, 2, false));
        oVector.add(new XriTestCase("../!x", 0, 2, false));
        oVector.add(new XriTestCase("../*x", 0, 2, false));
        oVector.add(new XriTestCase("./moo!", 0, 2, false));
        oVector.add(new XriTestCase("./moo*", 0, 2, false));
        oVector.add(new XriTestCase("./moo**", 0, 2, false));
        oVector.add(new XriTestCase("./moo*!", 0, 2, false));
        oVector.add(new XriTestCase("./moo!!", 0, 2, false));
        oVector.add(new XriTestCase("./moo://:/:!!", 0, 5, false));

        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            XriTestCase oCase = (XriTestCase) oIt.Current;
            String sXRI = (String) oCase.msVal;
            try
            {
                checkXRI(
                    sXRI, oCase.mnNumAuthSubs, oCase.mnNumRelativeSegments,
                    oCase.mbAbsolute);
            }
            catch (XRIParseException oEx)
            {
                oEx.dump();
                assertTrue(
                    "Caught XRIException for valid XRI: \"" + sXRI + "\": " +
                    oEx, false);
            }
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Exception for valid XRI: \"" + sXRI + "\": " + oEx,
                    false);
            }
        }

        Logger.Info("Valid Relative XRIs tested successfully");

    } // testRelativeXRIs()

    /*
    ****************************************************************************
    * testAbsoluteXRIs()
    ****************************************************************************
    */ /**
    *
    */
    public void testAbsoluteXRIs()
    {
        Vector oVector = new Vector();

        oVector.add(new XriTestCase("xri://www", 0, 0, true)); // iregname doesn't have to be DNS valid
        oVector.add(new XriTestCase("xri://www.epok.net", 0, 0, true));
        oVector.add(new XriTestCase("xri://www.epok.net/", 0, 0, true));
        oVector.add(new XriTestCase("xri://10.1.1.1/", 0, 0, true));
        oVector.add(new XriTestCase("xri://255.255.255.255/", 0, 0, true));
        oVector.add(
            new XriTestCase("xri://[fe80::207:e9ff:fe50:abcd]/", 0, 0, true));
        oVector.add(new XriTestCase("xri://www.epok.net:80/", 0, 0, true));
        oVector.add(new XriTestCase("xri://bob@www.epok.net:80/", 0, 0, true));
        oVector.add(new XriTestCase("xri://www.epok.net/!foo!bar", 0, 1, true));
        oVector.add(new XriTestCase("xri://@", 0, 0, true));
        oVector.add(new XriTestCase("xri://+", 0, 0, true));
        oVector.add(new XriTestCase("xri://!", 0, 0, true));
        oVector.add(new XriTestCase("xri://!!", 1, 0, true));
        oVector.add(new XriTestCase("xri://@*", 1, 0, true));
        oVector.add(new XriTestCase("xri://!!foo", 1, 0, true));
        oVector.add(new XriTestCase("xri://@!foo!bar*bix*moo!foo", 5, 0, true));
        oVector.add(new XriTestCase("xri://@*foo!bar*bix*moo!foo", 5, 0, true));
        oVector.add(new XriTestCase("xri://@foo!bar*bix*moo!foo", 5, 0, true));
        oVector.add(
            new XriTestCase("xri://@!foo!bar*bix*moo!foo/", 5, 0, true));
        oVector.add(
            new XriTestCase("xri://@!foo!bar*bix*moo!foo/x", 5, 1, true));
        oVector.add(
            new XriTestCase("xri://@!foo!bar*bix*moo!foo/!x", 5, 1, true));
        oVector.add(
            new XriTestCase("xri://@!foo!bar*bix*moo!foo/*x", 5, 1, true));
        oVector.add(
            new XriTestCase(
                "xri://@!foo!bar*bix*moo!foo/foo!bar/a*b/./*asd/../asd", 5, 6,
                true));
        oVector.add(
            new XriTestCase("xri://@email*com*foo*gates*john.doe&", 5, 0, true));
        oVector.add(new XriTestCase("xri://@email*com*bob.jones", 3, 0, true));
        oVector.add(
            new XriTestCase(
                "xri://@email*com*(*bob!jones/foo/asd)", 3, 0, true));
        oVector.add(
            new XriTestCase("xri://@email*com*(;as;d!bob*jones)", 3, 0, true));
        oVector.add(
            new XriTestCase(
                "xri://@email*com*(=bob.jones/jones*hoo/asd!moo)", 3, 0, true));
        oVector.add(
            new XriTestCase(
                "xri://@email*com*(=bob.jones/jones*hoo/asd!moo)/(foo!boo)!moo!loo/a*b!(boo!doo)",
                3, 2, true));
        oVector.add(
            new XriTestCase(
                "xri://@email*com*(=bob.jones/jones*hoo/asd!moo)/((foo!boo)!(@!foo!boo))!moo!loo/a*b!(boo!doo)",
                3, 2, true));
        oVector.add(
            new XriTestCase(
                "xri://@email*com*(=bob.jones/jones*hoo/asd!moo)/()", 3, 1, true));
        oVector.add(new XriTestCase("xri://(foo!boo*a)", 0, 0, true));
        oVector.add(new XriTestCase("xri://(foo!boo*a)!foo", 1, 0, true));
        oVector.add(new XriTestCase("xri://@!6!7", 2, 0, true));
        oVector.add(new XriTestCase("xri://@!6!7!!!!!4", 7, 0, true));
        oVector.add(
            new XriTestCase("xri://@!foo!bar*bix*moo!foo/**x", 5, 1, true)); // two stars

        // TODO: dirty-harry
        oVector.add(
                 new XriTestCase("xri://@example/(http://www.openxri.org/)", 1, 1, true));
        oVector.add(new XriTestCase("xri://@a*b*c*d/", 4, 0, true));
        oVector.add(new XriTestCase("xri://@a*b*c*d/f/", 4, 1, true));
        oVector.add(new XriTestCase("xri://@a*b*c*d/f////", 4, 4, true));
        oVector.add(new XriTestCase("xri://@a*b*c*d/f//x//", 4, 4, true));

        // TODO: gwachob
        // XRef Authority with IRI that has a "host" inside
        oVector.add(
                new XriTestCase("(mailto:gabe@foo.com)*foo", 1, 0, true));
        
        // XRef authority with IRI that has no "host"
        oVector.add(
                new XriTestCase("(uuid:bc1a39a4-ca76-4088-9e52-0d5829622870)*foo", 1, 0, true));

        oVector.add(
            new XriTestCase("xri://@a*b*c*d/:f:::/:/::x//", 4, 4, true));

        // IRI Authorities as Xrefs
        oVector.add(
            new XriTestCase(
                "xri://@a*b*c*d/!(xri://www.epok.net/moo/foo)/!x", 4, 2, true));
        oVector.add(
            new XriTestCase(
                "xri://@a*b*c*d/!(xri://www.epok.net)!y!z/!x", 4, 2, true));

        oVector.add(
                new XriTestCase(
                    "xri://@a*b*c*d/e*f/g?h/i", 4, 2, true));

        oVector.add(
                new XriTestCase(
                    "xri://@!no!path?some-query&blah#funny/frag", 2, 0, true));

        oVector.add(
                new XriTestCase(
                    "xri://!!some*auth/and/path/but-no-query#only-frag?", 2, 3, true));

        // test case contributed by Steve Churchill
        oVector.add(
                new XriTestCase(
                    "xri://!!1234/(+login)", 1, 1, true));
        
        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            XriTestCase oCase = (XriTestCase) oIt.Current;
            String sXRI = (String) oCase.msVal;
            try
            {
                checkXRI(
                    sXRI, oCase.mnNumAuthSubs, oCase.mnNumRelativeSegments,
                    oCase.mbAbsolute);
            }
            catch (XRIParseException oEx)
            {
                oEx.dump();
                assertTrue(
                    "Caught XRIException for valid XRI: \"" + sXRI + "\": " +
                    oEx, false);
            }
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Exception for valid XRI: \"" + sXRI + "\": " + oEx,
                    false);
            }
        }

        Logger.Info("Valid Absolute XRIs tested successfully");

    } // testAbsoluteXRIs()

    /*
    ****************************************************************************
    * testInvalidRelativeXRIs()
    ****************************************************************************
    */ /**
    *
    */
    public void testInvalidRelativeXRIs()
    {
        Vector oVector = new Vector();
        oVector.add("moo://"); // has colon at beginning
        oVector.add("//x"); // has empty first segment
        oVector.add("*a:b"); // has colon in first xrisegment

        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            String sXRI = (String) oIt.Current;
            try
            {
                checkXRI(sXRI, false);
                assertTrue(
                    "No Exception for invalid relative XRI: \"" + sXRI + "\"",
                    false);
            }
            catch (XRIParseException oEx) {}
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Incorrect Exception for invalid relative XRI: \"" +
                    sXRI + "\": " + oEx, false);
            }
        }

        Logger.Info("Invalid Relative XRIs tested successfully");

    } // testInvalidRelativeXRIs()

    /*
    ****************************************************************************
    * testInvalidAbsoluteXRIs()
    ****************************************************************************
    */ /**
    *
    */
    public void testInvalidAbsoluteXRIs()
    {
        Vector oVector = new Vector();
        oVector.add(""); // empty
        oVector.add("../!x"); // relative
        oVector.add("xri://a*b*c*d"); // no GCS root
        oVector.add("xri://*foo!bar*bix*moo!foo"); // no GCS root
        oVector.add("xri://@!foo!bar*bix*moo!foo/x)"); // unopened x-ref
        oVector.add("xri://@!foo!bar*bix*moo(!foo/x)"); // non-delimited x-ref
        oVector.add("xri://@!foo!bar*bix*moo!foo/x()"); // non-delimited x-ref
        oVector.add("xri://@!foo!bar*bix*moo!foo()/x"); // non-delimited x-ref
        oVector.add("xri://@!foo!bar*bix*moo!foo!(@=)/x"); // invalid x-ref        
        oVector.add("xri://(foo!boo*a)a"); // x-ref authority with bad segment
        oVector.add("xri://@a*b*c*d//"); // ends in "//"
        oVector.add("xri://1.2.3.256/"); // bad IP address
        oVector.add("xri://1.2..5/"); // bad IP address
        oVector.add("xri://1.2.5/"); // bad IP address
        oVector.add("xri://[ah:45]/"); // bad IPv6 address
        oVector.add("xri://!*foo"); // ! community with non-persistent segment
        oVector.add("xri://!foo"); // ! community with non-persistent segment
        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            String sXRI = (String) oIt.Current;
            try
            {
                checkXRI(sXRI, true);
                assertTrue(
                    "No Exception for invalid XRI: \"" + sXRI + "\"", false);
            }
            catch (XRIParseException oEx) {}
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Incorrect Exception for invalid XRI: \"" + sXRI +
                    "\": " + oEx, false);
            }
        }

        Logger.Info("Invalid Absolute XRIs tested successfully");

    } // testInvalidAbsoluteXRIs()

    /*
    ****************************************************************************
    * testXRISegments()
    ****************************************************************************
    */ /**
    *
    */
    public void testXRISegments()
    {
        Vector oVector = new Vector();

        oVector.add("moo!&");
        oVector.add("moo!;");
        oVector.add("moo!&!foo!bar*bix*moo!foo");
        oVector.add("moo!'*foo!bar*bix*moo!foo");
        oVector.add("moo!,foo!bar*bix*moo!foo");
        oVector.add("*x");
        oVector.add("!x");
        oVector.add("x");
        oVector.add("moo!a*b*c*d");

        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            String sSeg = (String) oIt.Current;
            try
            {
                checkXRISegment(sSeg);
            }
            catch (XRIParseException oEx)
            {
                oEx.dump();
                assertTrue(
                    "Caught XRIException for valid XRISegment: \"" + sSeg +
                    "\": " + oEx, false);
            }
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Exception for valid XRISegment: \"" + sSeg +
                    "\": " + oEx, false);
            }
        }
        Logger.Info("Valid XRI Segments tested successfully");

    } // testXRISegments()

    /*
    ****************************************************************************
    * testInvalidXRISegments()
    ****************************************************************************
    */ /**
    *
    */
    public void testInvalidXRISegments()
    {
        Vector oVector = new Vector();

        oVector.add("");
        oVector.add("xri://@!foo!bar*bix*moo!foo/");
        oVector.add("xri://@!foo!bar*bix*moo!foo/x");
        oVector.add("xri://@!foo!bar*bix*moo!foo/!x");
        oVector.add("xri://@!foo!bar*bix*moo!foo/*x");
        oVector.add("xri://@!foo!bar*bix*moo!foo/foo!bar/a*b/./*asd/../asd");
        oVector.add("/!x");
        oVector.add("/x");
        oVector.add("/*x");
        oVector.add("../x");
        oVector.add("../!x");
        oVector.add("../*x");
        oVector.add("./x");
        oVector.add("./moo!");
        oVector.add("./moo*");
        oVector.add("./moo**");
        oVector.add("./moo!*");
        oVector.add("./moo*!");
        oVector.add("./moo!!");
        oVector.add("xri://!foo!bar*bix*moo!foo");
        oVector.add("xri://@!foo!bar*bix*moo!foo//");
        oVector.add("xri://@!foo!bar*bix*moo!foo/**x");

        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            String sSeg = (String) oIt.Current;
            try
            {
                checkXRISegment(sSeg);
                assertTrue(
                    "No Exception for invalid XRISegment: \"" + sSeg + "\"",
                    false);
            }
            catch (XRIParseException oEx) {}
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Incorrect Exception for invalid XRISegment: \"" +
                    sSeg + "\": " + oEx, false);
            }
        }

        Logger.Info("Invalid XRI Segments tested successfully");

    } // testInvalidXRISegments()

    /*
    ****************************************************************************
    * checkXRI()
    ****************************************************************************
    */ /**
    *
    */
    private void checkXRI(String sXRI, bool bAbsolute)
    {
        XRIReference oXRI = null;
        if (bAbsolute)
        {
            oXRI = new XRI(sXRI);
        }
        else
        {
            oXRI = new RelativeXRI(sXRI);
        }

        if (!oXRI.toString().Equals(sXRI))
        {
            throw new RuntimeException(
                "Parsed xri \"" + oXRI.toString() +
                "\" did not match expected serialization \"" + sXRI);
        }

    } // checkXRI()

    /*
    ****************************************************************************
    * checkXRIAsURI()
    ****************************************************************************
    */ /**
    *
    */
    private void checkXRIAsURI(String sXRI, String sURI)
    {
        XRI oXRI = new XRI(sXRI);
        String sGenURI = oXRI.toURINormalForm();
        if (!sGenURI.Equals(sURI))
        {
            throw new RuntimeException(
                "Parsed xri \"" + oXRI.toString() +
                "\" did not match expected URI serialization \"" + sURI + "\"" +
                ", got \"" + sGenURI + "\"");
        }

    }

    /*
    ****************************************************************************
    * checkXRI()
    ****************************************************************************
    */
    private void checkXRI(
        String sXRI, int nNumAuthSub, int nNumRelativeSegments,
        bool bAbsolute)
    {
        XRIReference oXRI = null;
        if (bAbsolute)
        {
            oXRI = new XRI(sXRI);
        }
        else
        {
            oXRI = new RelativeXRI(sXRI);
        }

        if (!oXRI.toString().Equals(sXRI))
        {
            throw new RuntimeException(
                "Parsed xri \"" + oXRI.toString() +
                "\" did not match original xri \"" + sXRI);
        }

        int nNum = 0;
        if (oXRI.getAuthorityPath() is XRIAuthority)
        {
            nNum = ((XRIAuthority) oXRI.getAuthorityPath()).getNumSubSegments();
        }

        if (nNum != nNumAuthSub)
        {
            throw new RuntimeException(
                "Parsed xri \"" + oXRI.toString() +
                "\" did not parse into correct number of subsegments.  " +
                "Expected " + nNumAuthSub + ", Got " + nNum);
        }

        nNum = 0;
        if (oXRI.getXRIPath() != null)
        {
            nNum = oXRI.getXRIPath().getNumSegments();
        }

        if (nNum != nNumRelativeSegments)
        {
            throw new RuntimeException(
                "Parsed xri \"" + oXRI.toString() +
                "\" did not parse into correct number of xrisegments.  " +
                "Expected " + nNumRelativeSegments + ", Got " + nNum);
        }

    } // checkXRI()

    /*
    ****************************************************************************
    * checkXRISegment()
    ****************************************************************************
    */ /**
    *
    */
    private void checkXRISegment(String sXRISegment)
    {
        XRISegment oXRISegment = new XRISegment(sXRISegment);
        String sTostring = oXRISegment.toString();

        if (
            (!sTostring.Equals(sXRISegment)) &&
            (!sTostring.Equals(XRI.RDELIM_S + sXRISegment)))
        {
            throw new RuntimeException(
                "Parsed xrisegment \"" + oXRISegment.toString() +
                "\" did not match original xrisegment \"" + sXRISegment);
        }

    } // checkXRISegment()

    /*
    ****************************************************************************
    * Class: XriUriTestCase
    ****************************************************************************
    */ /**
    *
    */
    static class XriUriTestCase
    {
        public String msXRI;
        public String msURI;

        /*
        ************************************************************************
        * Constructor()
        ************************************************************************
        */ /**
        *
        */
        public XriUriTestCase(String sXRI, String sURI)
        {
            msXRI = sXRI;
            msURI = sURI;

        } // Constructor()

    } // Class: XriUriTestCase

    /*
    ****************************************************************************
    * testURIConversion()
    ****************************************************************************
    */ /**
    *
    */
    public void testURIConversion()
    {
        Vector oVector = new Vector();

        // no changes
        oVector.add(
            new XriUriTestCase(
                "xri://@email*com*(bob.jones)",
                "xri://@email*com*(bob.jones)"));
        oVector.add(
                new XriUriTestCase(
                    "xri://@*email*com*(bob.jones)",
                    "xri://@*email*com*(bob.jones)"));
        oVector.add(
            new XriUriTestCase(
                "xri://(foo!boo*a)!foo",
                "xri://(foo!boo*a)!foo"));
        oVector.add(
            new XriUriTestCase(
                "xri://@*email*com*(@!bob.jones)",
                "xri://@*email*com*(@!bob.jones)"));
        oVector.add(
                new XriUriTestCase(
                    "xri://@example/!12345",
                    "xri://@example/!12345"));
        
        /* TODO empty path segment
        oVector.add(
                new XriUriTestCase(
                    "xri://@example/!12345/",
                    "xri://@example/!12345/"));
                    */
        
        oVector.add(
                new XriUriTestCase(
                    "xri://@example/!1234!5678*90",
                    "xri://@example/!1234!5678*90"));
        oVector.add(
                new XriUriTestCase(
                    "xri://!!1234.5678.90AB.CDEF",
                    "xri://!!1234.5678.90AB.CDEF"));
        oVector.add(
                new XriUriTestCase(
                    "xri://@!1234.5678.90AB.CDEF",
                    "xri://@!1234.5678.90AB.CDEF"));

        // do not escape [#?/] outside of cross reference
        oVector.add(
            new XriUriTestCase(
                "xri://@mango%20sticky-rice*(@rice/good%20food?cat=desert#mangostickyrice)/",
                "xri://@mango%2520sticky-rice*(@rice%2Fgood%2520food%3Fcat=desert%23mangostickyrice)/"));
        
        // escape slashes
        oVector.add(
            new XriUriTestCase(
                "xri://@email*com*(*bob!jones/foo/asd)",
                "xri://@email*com*(*bob!jones%2Ffoo%2Fasd)"));
        
        // escape slashes and percent signs
        oVector.add(
                new XriUriTestCase(
                    "xri://@email*com*(=bob.jones/jones%2Ehoo/asd!moo)",
                    "xri://@email*com*(=bob.jones%2Fjones%252Ehoo%2Fasd!moo)"));
        oVector.add(
            new XriUriTestCase(
                "xri://@email*com*(=bob.jones/((%2Ejones*hoo))/asd!moo)",
                "xri://@email*com*(=bob.jones%2F((%252Ejones*hoo))%2Fasd!moo)"));
        oVector.add(
            new XriUriTestCase(
                "xri://@email*com*(*bob!jones/foo/asd)/bob%2E/(foo%2E/bar)",
                "xri://@email*com*(*bob!jones%2Ffoo%2Fasd)/bob%252E/(foo%252E%2Fbar)"));

        // query and fragment
        oVector.add(
                new XriUriTestCase(
                    "xri://@example/path?q=foo#bar",
                    "xri://@example/path?q=foo#bar"));
        oVector.add(
                new XriUriTestCase(
                    "xri://@example*nopath?q=foo#bar",
                    "xri://@example*nopath?q=foo#bar"));

        IEnumerator oIt = oVector.GetEnumerator();
        while (oIt.MoveNext())
        {
            XriUriTestCase oCase = (XriUriTestCase) oIt.Current;
            String sXRI = (String) oCase.msXRI;
            try
            {
                checkXRIAsURI(sXRI, oCase.msURI);
            }
            catch (XRIParseException oEx)
            {
                oEx.dump();
                assertTrue(
                    "Caught XRIException for valid XRI: \"" + sXRI + "\": " +
                    oEx, false);
            }
            catch (Exception oEx)
            {
                assertTrue(
                    "Caught Exception for valid XRI: \"" + sXRI + "\": " + oEx,
                    false);
            }
        }

        Logger.Info("Valid URIs conversion tested successfully");

    }

    
    public void testEquivalence()
    {
    	try {
    		XRI x1 = XRI.fromURINormalForm("xri://@foo*(=bar%2Fbaz)");
    		XRI x2 = new XRI("@foo*(=bar/baz)");
    		assertTrue("x1 and x2 should be equivalent", x1.Equals(x2));
    	}
    	catch (Exception e) {
    		fail("Not expecting exception here: " + e);
    	}
    }
    
    
    /*
    ****************************************************************************
    * testUtilMethods()
    ****************************************************************************
    */ /**
    *
    */
    public void testUtilMethods()
    {
        XRISegment oSegment = new XRISegment("!a!b!c!d");
        assertTrue(
            "Failed XRISegment::getRemainder",
            oSegment.getRemainder(0).toString().Equals("!a!b!c!d"));
        assertTrue(
            "Failed XRISegment::getRemainder",
            oSegment.getRemainder(1).toString().Equals("!b!c!d"));
        assertTrue(
            "Failed XRISegment::getRemainder",
            oSegment.getRemainder(2).toString().Equals("!c!d"));
        assertTrue(
            "Failed XRISegment::getRemainder",
            oSegment.getRemainder(3).toString().Equals("!d"));
        assertTrue(
            "Failed XRISegment::getRemainder", oSegment.getRemainder(4) == null);

        Logger.Info("Utility methods tested successfully");

    } // testUtilMethods()

} // Class: XRITest
