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
package org.openxri.tools.xritracert;

import java.net.URL;
import java.net.URLConnection;
import java.util.Iterator;

import org.openxri.XRI;
import org.openxri.resolve.ResolveChain;
import org.openxri.resolve.ResolveInfo;
import org.openxri.resolve.Resolver;
import org.openxri.resolve.TrustType;
import org.openxri.resolve.exception.XRIResolutionException;
import org.openxri.xml.Service;
import org.openxri.xml.Tags;
import org.openxri.xml.XRD;
import org.openxri.xml.XRDS;

/*
********************************************************************************
* Class: XRITraceRt
********************************************************************************
*/ /**
* Provides tracert-like output for XRI resolution.
* See outputUsage for command line usage and program output.
*
* @author =steven.churchill (ooTao.com)
*/

public class XRITraceRt {
    
    // program results
    private const int SUCCESS                        = 0;
    private const int FAILURE                        = 1;
    
    // program commands as command line args
    private const String CMD_HELP                    = "help";
    
    // options as command line args
    private const String OPT_ROOT_EQUALS_URI         = "-root_equals_auth";    
    private const String OPT_ROOT_AT_URI             = "-root_at_auth";    
    private const String OPT_VERBOSE                 = "-verbose";                
    private const String OPT_NO_HEADER               = "-no_header";    
    
    private const String ROOT_DEF_URI  = "http://localhost:8080/xri/resolve?ns=at";
    
    // data argument variable
    private String msTargetXRI;
    
    // option variables
    private bool mbIsVerbose;
    private bool mbDontOutputHeader;
    private String msRootEqualsURI;
    private String msRootAtURI;
    
    /*
    ****************************************************************************
    * main()
    ****************************************************************************
    */ /**
    * Invokes the process routine to execute the command specified by
    * the given arguments. See outputUsage for details about program 
    * arguments and output.
    */
    public static void main(String[] oArgs)
    {     
        StringBuffer sOutput = new StringBuffer();
        XRITraceRt oTraceRt = new XRITraceRt();

        int iResult = oTraceRt.process(sOutput, oArgs);
        exit(sOutput, iResult);

    } // main()
    
    /*
    ****************************************************************************
    * process()
    ****************************************************************************
    */ /**
    * Executes the xritracert command as indicated by the input args. See 
    * outputUsage for the forms of invocation and details about the program 
    * arguments.
    * 
    * @param sArgs - command line arguments (e.g., from main)
    * @param sOutput - program output (e.g., for stdout)
    * 
    * @return SUCCESS or FAILURE
    */
    public int process(StringBuffer sOutput, String[] sArgs) 
    {
        try 
        {
            // re-initialize variables so this may be called more than once
            sOutput.setLength(0);
            msTargetXRI = null;
            mbIsVerbose = false;
            mbDontOutputHeader = false;
            msRootEqualsURI = ROOT_DEF_URI;
            msRootAtURI = ROOT_DEF_URI;
                
            // exit with message if no arguments passed on the command line
            if (sArgs.length == 0) 
            {
                outputPleaseTypeHelp(sOutput); 
                return FAILURE;
            }
            
            // this is the "help" form of invocation (usage 2)
            if (sArgs[0].equalsIgnoreCase(CMD_HELP))
            {
                outputUsage(sOutput);
                return SUCCESS;
            }
            
            // from here on, we're dealing with the "normal" form of invocation
            // (usage 1).
            // scan the args, setting member variables for options, rootURI, 
            // and resolve XRI
            int iResult = scanArgs(sOutput, sArgs);
            if (iResult == FAILURE)
            {
                return FAILURE;
            }
            
            // validate that the root uris are ok
            iResult = validateRootURIs(sOutput);
            if (iResult == FAILURE)
            {
                return FAILURE;
            }       
            
            // create and configure a resolver
            Resolver resolver = new Resolver();
            
            XRD eqRoot = new XRD();
            Service eqAuthService = new Service();
            eqAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=none");
            eqAuthService.addType(Tags.SERVICE_AUTH_RES);
            eqAuthService.addURI(msRootEqualsURI);
            eqRoot.addService(eqAuthService);
            
            XRD atRoot = new XRD();
            Service atAuthService = new Service();
            atAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + ";trust=none");
            atAuthService.addType(Tags.SERVICE_AUTH_RES);
            atAuthService.addURI(msRootAtURI);
            atRoot.addService(atAuthService);

            resolver.setAuthority("=", eqRoot);
            resolver.setAuthority("@", atRoot);

            // invoke the tracert
            tracert(sOutput, resolver);
            
        }
        catch (Throwable oThrowable)
        {
            outputException(sOutput, oThrowable);
            return FAILURE;
        }
        return SUCCESS;
    }
    
    /*
    ****************************************************************************
    * tracert()
    ****************************************************************************
    */ /**
    * For the XRI specified msTargetXRI, outputs the URIs for the registry 
    * servers used at each step of the resolution as per the command-line
    * options.
    * @param sOutput - string buffer for output
    * @param resolver - resolver for top-most at/equals authority
    * 
    * @return SUCCESS or FAILURE
    */
    private void tracert(
        StringBuffer sOutput, Resolver resolver) throws XRIResolutionException 
    {
        
        // TODO: trusted resolution is currently not supported
        TrustType trustType = new TrustType(TrustType.TRUST_NONE);
        bool followRefs = true;
        
        // note: See also Resolver.resolve
        // resolver.setLookaheadMode(true);
        XRDS xrds = resolver.resolveAuthToXRDS(msTargetXRI, trustType, followRefs);
//        ResolveInfo oResolveInfo = resolver.resolveAuth(msTargetXRI, trustType, followRefs);
        
        // The non-verbose format is as follows:
        // 
        // xri://@community*member*family  
        // all subsegments resolved
        //
        //         resolved subsegment    resolved by  
        //    ------------------------    --------------------------------------------
        // 1.               *community    http://localhost:8080/xri/resolve?ns=at
        // 2.                  *member    http://127.0.0.1:8085/xri/resolve?ns=community
        // 3.                  *family    http://127.0.0.1:8080/xri/resolve?ns=member

        String sCol1Header = "     resolved subsegment";
        String sCol2Header = "resolved by";
        String sCol1Border = "------------------------";
        String sCol2Border = "--------------------------------------------";
        String sColGap = "  ";
        String sLeftHeaderPad = "   ";

        // output the trace hops into a separate buffer
        StringBuffer sTraceHops = new StringBuffer();     
        int iAuthorityHops = 0;
        
        /*
        Iterator i = oResolveInfo.getChainIterator();
        while (i.hasNext()) {
            ResolveChain oResolveChain = (ResolveChain) i.next();
            
            XRDS oDecriptors = oResolveChain.getXRIDescriptors();
            int length = oDecriptors.getNumChildren();
            for (int index = 0; index < length; index++) {
                
                XRD oDescriptor = oDecriptors.getDescriptorAt(index);
                
                if (!mbIsVerbose) 
                {
                    if (sLastResolvedByURI != null)
                    {        
                        String sResolved = oDescriptor.getQuery();
                        int iLeftPad = sCol1Border.length() - sResolved.length();
                        iLeftPad = (iLeftPad < 0)? 0 : iLeftPad;
                          
                        sTraceHops.append(new Integer(++iAuthorityHops).toString() + ". ");
                        outputChars(sTraceHops, ' ', iLeftPad);
                        sTraceHops.append(sResolved);
                        sTraceHops.append(sColGap);
                        sTraceHops.append(sLastResolvedByURI + "\n");
                    }
                    
                    sLastResolvedByURI = oDescriptor.getURI().toString();
                }
                else
                {
                    sTraceHops.append(oDescriptor.toString());
                }
            }
            
            // output the xri and the resolution status
            sOutput.append("\n");
            sOutput.append(msTargetXRI + "\n");
            if (oResolveInfo.resolvedAll()) 
            {
                sOutput.append("all subsegments resolved \n");
            }
            else 
            {
                // handle case where no subsegment is resolved
                if (iAuthorityHops == 0) 
                {
                    sOutput.append("no subsegments resolved \n\n");
                    return;
                }
                else 
                {
                    String sUnresolvedPart = oResolveInfo.getUnresolved();
                    sOutput.append("unresolved subsegments: " + sUnresolvedPart + "\n");
                }
            }
            
            // output the header
            sOutput.append("\n");
            if (!mbDontOutputHeader && !mbIsVerbose)
            {
                sOutput.append(
                    sLeftHeaderPad + sCol1Header + sColGap + sCol2Header + "\n" +
                    sLeftHeaderPad + sCol1Border + sColGap + sCol2Border + "\n"
                    );
            }
            
            // output the trace hops
            sOutput.append(sTraceHops);
        }
        sOutput.append("\n");
        */
        
    } // tracert()
    
    /*
    ****************************************************************************
    * scanArgs()
    ****************************************************************************
    */ /**
    * Scans list of command-line arguments, setting member variables for 
    * the data argument, and all options. 
    * @param sArgs - command lines args list
    * @param sOutput - string buffer for error message, if false returned
    * 
    * @return SUCCESS or FAILURE
    */
    private int scanArgs(StringBuffer sOutput, String[] sArgs)
    {        
        for (int i = 0; i < sArgs.length; i++)
        {
            String sArg = sArgs[i];
            if (!isOption(sArg)) 
            {
                // set the sole data argment
                if (msTargetXRI == null)
                {
                    msTargetXRI = sArg;
                }
                else
                {
                    outputPleaseTypeHelp(sOutput);
                    return FAILURE;
                }
            }
            else if (sArg.equalsIgnoreCase(OPT_VERBOSE)) 
            {
                mbIsVerbose = true;
            }           
            else if (sArg.equalsIgnoreCase(OPT_ROOT_AT_URI)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_ROOT_AT_URI);
                    return FAILURE;
                }
                msRootAtURI = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_ROOT_EQUALS_URI)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_ROOT_EQUALS_URI);
                    return FAILURE;
                }
                msRootEqualsURI = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_NO_HEADER)) 
            {
                mbDontOutputHeader = true;
            }
            else 
            {
                sOutput.append("Invalid option: " + sArg + "\n");
                outputPleaseTypeHelp(sOutput);
                return FAILURE;
            }
        }
        
        // error if we do not have at least xri
        if (msTargetXRI == null)
        {
            sOutput.append("Must specify target XRI. \n");
            outputPleaseTypeHelp(sOutput);
            return FAILURE;
        }
        return SUCCESS;
        
    } // scanArgs()

    
    /*
    ****************************************************************************
    * validateRootURIs()
    ****************************************************************************
    */ /**
    * Validates that connections can be made to the URIs for the at and equal 
    * root authorities.
    * 
    * @paran sOutput - string buffer containing error information if 
    * FAILURE is returned
    * 
    * @returns SUCCESS or FAILURE
    */
    private int validateRootURIs(StringBuffer sOutput) {
        
        String sCurURI = null;
        try
        {
            sCurURI = msRootEqualsURI;
            URL oURL = new URL(sCurURI);            
            URLConnection oConn = oURL.openConnection();
            oConn.connect();
            
            sCurURI = msRootAtURI;
            oURL = new URL(sCurURI);
            oConn = oURL.openConnection();
            oConn.connect();
        }
        catch (Throwable oThrowable)
        {
            sOutput.append("Cannot cannect to root authority URI: " + sCurURI);
            return FAILURE;
        }
        return SUCCESS;
        
    } // validateRootURIs()
    
    /*
     ****************************************************************************
     * outputUsage()
     ****************************************************************************
     */ /**
     * Outputs the program usage to the given string buffer.
     */
     private void outputUsage(StringBuffer sOutput) 
     {
         // output the overall program usage
         sOutput.append(
             "\n" +
             "usage: 1. xritracert <XRI> [options] \n" + 
             "       2. xritracert help \n" +
             "\n" +
             "The first form resolves the given <XRI> and outputs the URIs for \n" +
             "the registry servers used at each step of the resolution. \n" +
             "\n" + 
             "Example usage: xritracert xri://@community*member*family \n" +
             "\n" + 
             "Refer to the OASIS document \"Extensible Resource Identifier (XRI) \n" +
             "Resolution\" for information about XRI resolution. \n" +
             "\n" +
             "The second form generates this help message. \n" +
             "\n" +
             "Available options: \n" +
             "    -root_equals_auth <URI>: Root authority URI for '=' resolution. \n" +
             "    -root_at_auth <URI>:     Root authority URI for '@' resolution. \n" +
             "\n" + 
             "    -verbose:        Print verbose output. \n" +
             "    -no_header:      Omit header from non-verbose output. \n" +
             "\n" +
             "The default value (used if unspecified) for the two '-root_' options \n" +
             "is " + ROOT_DEF_URI + ". \n" +
             "\n" +
             "Program output: \n" +
             "    For successful invocation, 0 is returned with program \n" +
             "    output on stdout. Otherwise 1 is returned with error \n" +
             "    message output on stderr. \n" +
             "\n" + 
             "N.B.: \n" +
             "    The server script \"regexample\" can be used to register \n" +
             "    subsegments and create authorities. \n " +
             "\n" + 
             "\n" 
             );  

         
     } // outputUsage()    
    
     /*
     ****************************************************************************
     * isOption()
     ****************************************************************************
     */ /**
     * Returns true if the given argument string is an option. This is currently
     * defined as beginning with a dash character.
     */
     private bool isOption(String sArg)
     {
         return sArg.charAt(0) == '-';
         
     } // isOption()
     
     /*
     ****************************************************************************
     * outputPleaseTypeHelp()
     ****************************************************************************
     */ /**
     * Outputs text to the given buffer asking the end user to type 
     * "xriadmin -help".
     */    
     private void outputPleaseTypeHelp(StringBuffer sOutput) 
     {
         sOutput.append("Type \"xritracert help\". \n");
     }
     
     /*
     ****************************************************************************
     * outputOptionRequiresArgument()
     ****************************************************************************
     */ /**
     * Outputs text to the given buffer with text suitable for the given
     * option argument error.
     */ 
     private void outputOptionRequiresArgument(StringBuffer sOutput, String sOption)
     {
         sOutput.append("Option: " + sOption + " requires argument.\n");
         outputPleaseTypeHelp(sOutput);
         
     } // outputOptionRequiresArgument()
          
    /*
    ****************************************************************************
    * outputChars()
    ****************************************************************************
    */ /**
    * Outputs the given number of characters to the output buffer.
    */  
    void outputChars(StringBuffer sOutput, char c, int num) {
        char[] cArray = new char[num];
        for (int i = 0; i < num; i++) 
        {
            cArray[i] = c;
        }
        sOutput.append(cArray);
        
     } // outputChars()
     
     /*
     ****************************************************************************
     * outputException()
     ****************************************************************************
     */ /**
     * Formats the given throwable into the given output buffer.
     */     
     private void outputException(StringBuffer sOutput, Throwable oThrowable)
     {
         String message = oThrowable.getLocalizedMessage();
         sOutput.append(oThrowable.getClass().getName() + ": " + message + "\n");
         
         if (mbIsVerbose) 
         {
             oThrowable.printStackTrace();
         }
         
     } // outputException()
     
     /*
     ****************************************************************************
     * exit()
     ****************************************************************************
     */ /**
     * Prints the output of the given buffer and exits the program.
     *
     * @param sOutput - text to output
     */     
     static private void exit(StringBuffer sOutput, int iStatus)
     { 
         if (iStatus == FAILURE)
         {
             System.err.print(sOutput);
         }
         else
         {
             System.out.print(sOutput);
         }
         System.exit(iStatus);
         
     } // exit()

} // Class: XRITractRt
