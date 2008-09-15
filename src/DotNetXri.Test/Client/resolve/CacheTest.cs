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
namespace DotNetXri.Client.Resolve {


using java.util.Random;
using org.openxri.AuthorityPath;
using org.openxri.GCSAuthority;
using org.openxri.XRIAuthority;
using org.openxri.resolve.Cache;
using org.openxri.xml.Service;
using org.openxri.xml.Tags;
using org.openxri.xml.XRD;
using junit.framework.Test;
using junit.framework.TestCase;
using junit.framework.TestSuite;
using junit.textui.TestRunner;


/*
********************************************************************************
* Class: CacheTest
********************************************************************************
*/ /**
* @author =chetan
*
* To change the template for this generated type comment go to
* Window>Preferences>Java>Code Generation>Code and Comments
*/
public class CacheTest
    :TestCase
{
    /*
    ****************************************************************************
    * main()
    ****************************************************************************
    */ /**
    *
    */
    public static void main(String[] oArgs)
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
        return new TestSuite(typeof(CacheTest));

    } // suite()

    /*
    ****************************************************************************
    * testCache()
    ****************************************************************************
    */ /**
    *
    */
    public void testCache()
    {
        Cache oCache = new Cache(1000);
        assertTrue("Initial cache not empty", oCache.getNumNodes() == 0);

        XRD oDesc = new XRD();
        Service atAuthService = new Service();
        atAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=none");
        atAuthService.addType(Tags.SERVICE_AUTH_RES);
        atAuthService.addURI("http://gcs.epok.net/xri/resolve?ns=at");
        oDesc.addService(atAuthService);

        XRD oDummy = new XRD();
        Service dummyService = new Service();
        dummyService.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=none");
        dummyService.addType(Tags.SERVICE_AUTH_RES);
        dummyService.addURI("http://www.example.com/xri/resolve?id=1");
        oDummy.addService(dummyService);

        GCSAuthority oAuth = new GCSAuthority("@");
        oCache.stuff(oAuth, oDesc);
        assertTrue("Initial cache incorrect", oCache.getNumNodes() == 1);

        oCache.stuff(
            (XRIAuthority) AuthorityPath.buildAuthorityPath("@!a!b!foo"), oDummy);
        assertTrue("Cache size incorrect", oCache.getNumNodes() == 4);

        oCache.stuff(
            (XRIAuthority) AuthorityPath.buildAuthorityPath("@!a!c!moo"), oDummy);
        assertTrue("Cache size incorrect", oCache.getNumNodes() == 6);

        oCache.stuff(
            (XRIAuthority) AuthorityPath.buildAuthorityPath("@!a!c!woo"), oDummy);
        assertTrue("Cache size incorrect", oCache.getNumNodes() == 7);

        Cache.CachedValue oVal =
            oCache.find(
                (XRIAuthority) AuthorityPath.buildAuthorityPath("@!a!c!woo"),
                false);
        assertTrue("Cached value not found", oVal != null);

        oVal =
            oCache.find(
                (XRIAuthority) AuthorityPath.buildAuthorityPath("@!a!b!woo"),
                false);
        assertTrue("Cached value should not have been found", oVal == null);
        oCache.dump();

    } // testCache()

    /*
    ****************************************************************************
    * testConcurrent()
    ****************************************************************************
    */ /**
    *
    */
    public void testConcurrent()
    {
        Cache oCache = new Cache(1000);
        oCache.prune((XRIAuthority) AuthorityPath.buildAuthorityPath("@"));
        assertTrue("Initial cache not empty", oCache.getNumNodes() == 0);

        
        XRD oDesc = new XRD();
        Service atAuthService = new Service();
        atAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=none");
        atAuthService.addType(Tags.SERVICE_AUTH_RES);
        atAuthService.addURI("http://gcs.epok.net/xri/resolve?ns=at");
        oDesc.addService(atAuthService);

        GCSAuthority oAuth = new GCSAuthority("@");
        oCache.stuff(oAuth, oDesc);
        assertTrue("Initial cache incorrect", oCache.getNumNodes() == 1);

        oCache.setMaxSize(5);

        Random oRand = new Random();

        try
        {
            Thread[] oThreads = new StuffPruneThread[100];
            for (int i = 0; i < oThreads.length; i++)
            {
                oThreads[i] = new StuffPruneThread(oRand);
            }

            for (int i = 0; i < oThreads.length; i++)
            {
                oThreads[i].start();
            }

            for (int i = 0; i < oThreads.length; i++)
            {
                oThreads[i].join();
            }
        }
        catch (Exception e)
        {
            assertTrue("Unexpected exception" + e, false);
        }

        oCache.dump();

        assertTrue(
            "Max cache size not honored",
            oCache.getNumNodes() <= oCache.getMaxSize());

        Cache.CachedValue oVal =
            oCache.find(
                (XRIAuthority) AuthorityPath.buildAuthorityPath("@"), false);
        assertTrue("Cached value for @ not found", oVal != null);

    } // testConcurrent()

    /*
    ****************************************************************************
    * Class: StuffPruneThread
    ****************************************************************************
    */ /**
    *
    */
    class StuffPruneThread
        :Thread
    {
        private Random moRand = null;

        /*
        ************************************************************************
        * Constructor()
        ************************************************************************
        */ /**
        *
        */
        public StuffPruneThread(Random oRand)
        {
            moRand = oRand;

        } // Constructor()

        /*
        ************************************************************************
        * run()
        ************************************************************************
        */ /**
        *
        */
        public void run()
        {
            XRD oDummy = new XRD();
            Service dummyService = new Service();
            dummyService.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=none");
            dummyService.addType(Tags.SERVICE_AUTH_RES);
            dummyService.addURI("http://www.example.com/xri/resolve?id=1");
            oDummy.addService(dummyService);

            String[] oCases =
                { "@!a1!b2!c3!d4", "@!x1!y2!z3", "@!a1!b2!c3", "@!a1!b2", "@!a1!b2!m3", "@!a1!o2!p3", "@!a1!o2!q3", "@!a1!b2!c3!d4!e5", "@!x1!y2" };

            Cache oCache = new Cache(1000);

            for (int i = 0; i < 1000; i++)
            {
                int x = moRand.nextInt(oCases.length);
                bool bStuff = moRand.nextBoolean();
                XRIAuthority oAuth =
                    (XRIAuthority) AuthorityPath.buildAuthorityPath(oCases[x]);

                if (bStuff)
                {
                    oCache.stuff(oAuth, oDummy);
                }
                else
                {
                    oCache.prune(oAuth);
                }

                oCache.find(oAuth, true);
            }

        } // run()

    } // Class: StuffPruneThread

} // Class: CacheTest
}