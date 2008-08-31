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
package org.openxri.tools.xrilookup;

import java.net.URL;
import java.net.URLConnection;
import java.io.FileReader;
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.IOException;
import org.openxri.resolve.MimeType;
import org.openxri.resolve.Resolver;
import org.openxri.resolve.ResolverState;
import org.openxri.resolve.TrustType;
import org.openxri.resolve.exception.IllegalTrustTypeException;
import org.openxri.resolve.exception.PartialResolutionException;
import org.openxri.resolve.exception.XRIResolutionException;
import org.openxri.xml.*;

/*
********************************************************************************
* Class: XRILookup
********************************************************************************
*/ /**
* Provides tracert-like output for XRI resolution.
* See outputUsage for command line usage and program output.
*
* @author =steven.churchill (ooTao.com)
*/

public class XRILookup {
    
    // program results
    final private static int SUCCESS                        = 0;
    final private static int FAILURE                        = 1;
    
    // program commands as command line args
    final private static String CMD_HELP                    = "help";
    
    // options as command line args
    final private static String OPT_ROOT_EQUALS_URI         = "-root_equals_auth";
    final private static String OPT_ROOT_AT_URI             = "-root_at_auth";
    final private static String OPT_ROOT_BANG_URI           = "-root_bang_auth";
    final private static String OPT_PROXY_URI               = "-proxy";
    final private static String OPT_RES_MEDIA               = "-xrd-r";
    final private static String OPT_RES_TYPE                = "-xrd-t";
    final private static String OPT_MEDIA_TYPE              = "-xrd-m";
    final private static String OPT_TRUST_TYPE              = "-trust";
    final private static String OPT_FOLLOW_REFS             = "-refs";
    final private static String OPT_SEP                     = "-sep";
    final private static String OPT_VERBOSE                 = "-verbose";
    final private static String OPT_CHECK_ROOTS             = "-check_roots";
    final private static String OPT_ROOT_FILE               = "-roots";
    
    final private static String ROOT_DEF_EQ_URI   = "http://equal.xri.net";
    final private static String ROOT_DEF_AT_URI   = "http://at.xri.net";
    final private static String ROOT_DEF_BANG_URI = "http://bang.xri.net";
    final private static String PROXY_URI         = "http://xri.net";
    final private static String RES_MEDIA         = "application/xrds+xml";
    final private static String TRUST_TYPE        = "none";
    final private static bool FOLLOW_REFS       = true;
    final private static bool SEP_SELECT        = false;
    
    // data argument variable
    private String msTargetXRI;
    
    // option variables
    private bool mbIsVerbose;
    private String msRootEqualsURI;
    private String msRootAtURI;
    private String msRootBangURI;
    private String msProxyURI;
    private bool mbCheckRoots;
    private String msRootFile;
    private String xrdR = RES_MEDIA;
    private String xrdT = null;
    private String xrdM = null;
    private TrustType trustType = new TrustType();
    private bool followRefs = FOLLOW_REFS;
    private bool doSEP = SEP_SELECT;
    
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
        XRILookup oTraceRt = new XRILookup();

        int iResult = oTraceRt.process(sOutput, oArgs);
        exit(sOutput, iResult);

    } // main()
    
    /*
    ****************************************************************************
    * process()
    ****************************************************************************
    */ /**
    * Executes the xrilookup command as indicated by the input args. See 
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
            msRootEqualsURI = ROOT_DEF_EQ_URI;
            msRootAtURI     = ROOT_DEF_AT_URI;
            msRootBangURI   = ROOT_DEF_BANG_URI;
            msProxyURI      = null;
            mbCheckRoots    = true;
            msRootFile      = null;

            try {
            	setTrustType(TRUST_TYPE);
            }
            catch (Exception e) {
            	e.printStackTrace();
            }
            

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
            if (mbCheckRoots)
            {
                iResult = validateRootURIs(sOutput);
                if (iResult == FAILURE)
                {
                    return FAILURE;
                }
            }       
            
            
            // create and configure a resolver
            Resolver resolver = new Resolver();
            
            // populate the root with whatever trustType the user requested
            String trustParam = ";" + trustType.getParameterPair();
            
            XRD eqRoot = new XRD();
            Service eqAuthService = new Service();
            eqAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + trustParam, SEPElement.MATCH_ATTR_CONTENT, Boolean.FALSE);
            eqAuthService.addType(Tags.SERVICE_AUTH_RES);
            eqAuthService.addURI(msRootEqualsURI);
            eqRoot.addService(eqAuthService);
            
            XRD atRoot = new XRD();
            Service atAuthService = new Service();
            atAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + trustParam, SEPElement.MATCH_ATTR_CONTENT, Boolean.FALSE);
            atAuthService.addType(Tags.SERVICE_AUTH_RES);
            atAuthService.addURI(msRootAtURI);
            atRoot.addService(atAuthService);
            
            XRD bangRoot = new XRD();
            Service bangAuthService = new Service();
            bangAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + trustParam, SEPElement.MATCH_ATTR_CONTENT, Boolean.FALSE);
            bangAuthService.addType(Tags.SERVICE_AUTH_RES);
            bangAuthService.addURI(msRootBangURI);
            bangRoot.addService(bangAuthService);
            
            resolver.setAuthority("=", eqRoot);
            resolver.setAuthority("@", atRoot);
            resolver.setAuthority("!", bangRoot);
            
            if (msRootFile != null)
            {
                setRootsFromFile(resolver, msRootFile);
            }
              
            
            // invoke the tracert
            lookup(sOutput, resolver);
            
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
    * lookup()
    ****************************************************************************
    */ /**
    * For the XRI specified msTargetXRI, outputs the URIs for the registry 
    * servers used at each step of the resolution as per the command-line
    * options.
    * @param sOutput - string buffer for output
    * @param oResolver - resolver for top-most at/equals authority
    * 
    * @return SUCCESS or FAILURE
    */
    private void lookup(
        StringBuffer sOutput, Resolver resolver) throws XRIResolutionException 
    {
    	String mediaTypeStr = xrdR + ";" + trustType.getParameterPair() 
    		+ ";refs" + Boolean.toString(followRefs);
    	
    	if (msProxyURI != null) {
    		mediaTypeStr += (";sep" + Boolean.toString(doSEP));
    	}
    	MimeType resMediaType = MimeType.parse(mediaTypeStr);

    	ResolverState state = new ResolverState();
    	XRDS xrds = null;
        XRD xrd = null;
    	try {
    		if (doSEP) {
    			if (resMediaType.isType(MimeType.URI_LIST)) {
    				String  text = resolver.resolveSEPToTextURIList(msTargetXRI.toString(), trustType, xrdT, xrdM, followRefs, state);
    				if (text.length() <= 0)
    					sOutput.append("ERROR: SEP_NOT_FOUND(code=241): no url found\n");
    				else
    					sOutput.append(text);
    			}
    			else if (resMediaType.isType(MimeType.XRDS_XML)) {
    				xrds = resolver.resolveSEPToXRDS(msTargetXRI, trustType, xrdT, xrdM, followRefs, state);
    				sOutput.append(xrds.toString());
    			}
    			else if (resMediaType.isType(MimeType.XRD_XML)) {
    				xrd = resolver.resolveSEPToXRD(msTargetXRI, trustType, xrdT, xrdM, followRefs, state);
    				sOutput.append(xrd.toResultString());
    			}
    			else {
    				sOutput.append("Unknown resolution media type (sep=true)");
    			}
    		}
    		else {
    			if (resMediaType.isType(MimeType.XRDS_XML)) {
    				xrds = resolver.resolveAuthToXRDS(msTargetXRI, trustType, followRefs, state);
    				sOutput.append(xrds.toString());
    			}
    			else if (resMediaType.isType(MimeType.XRD_XML)) {
    				xrd = resolver.resolveAuthToXRD(msTargetXRI, trustType, followRefs, state);
    				sOutput.append(xrd.toString());
    			}
    			else if (resMediaType.isType(MimeType.URI_LIST)) {
    				// ignore (must do SEP when text/uri-list is specified)
    				sOutput.append("ERROR: text/uri-list must only be used with service selection (sep=true)\n");
    			}
    			else {
    				sOutput.append("ERROR: Unknown resolution media type (sep=false)");
    			}
    		}
    	}
        catch (PartialResolutionException pre) {

           	xrds = pre.getPartialXRDS();
            xrd = xrds.getFinalXRD();
           	
           	if (resMediaType == null || resMediaType.isType(MimeType.URI_LIST)) {
           		sOutput.append("ERROR: resolution failed. Partial XRDS:\n" + xrds.toString());
           	}
           	else if (resMediaType.isType(MimeType.XRDS_XML)) {
           		sOutput.append("ERROR: resolution failed. Partial XRDS:\n" + xrds.toString());
           	}
           	else if (resMediaType.isType(MimeType.XRD_XML)) {
           		sOutput.append("ERROR: resolution failed. Partial XRDS:\n" + xrds.toString());
           	}
        }
        finally {
        	if (mbIsVerbose)
        		sOutput.append("\n\nSTATISTICS:\n" + state.toString() + "\n");
        }
    }
    
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
            else if (sArg.equalsIgnoreCase(OPT_ROOT_BANG_URI)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_ROOT_BANG_URI);
                    return FAILURE;
                }
                msRootBangURI = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_PROXY_URI)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_PROXY_URI);
                    return FAILURE;
                }
                msProxyURI = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_RES_MEDIA)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_RES_MEDIA);
                    return FAILURE;
                }
                xrdR = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_RES_TYPE)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_RES_TYPE);
                    return FAILURE;
                }
                xrdT = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_MEDIA_TYPE)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_MEDIA_TYPE);
                    return FAILURE;
                }
                xrdM = sArgs[i];
            }
            else if (sArg.equalsIgnoreCase(OPT_TRUST_TYPE)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_TRUST_TYPE);
                    return FAILURE;
                }
                try {
                	setTrustType(sArgs[i]);
                }
                catch (IllegalTrustTypeException e) {
                	System.err.println("Trust type '" + sArgs[i] + "' is not valid: " + e);
                	return FAILURE;
                }
            }
            else if (sArg.equalsIgnoreCase(OPT_FOLLOW_REFS)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_FOLLOW_REFS);
                    return FAILURE;
                }
                followRefs = sArgs[i].toLowerCase().equals("true");
            }
            else if (sArg.equalsIgnoreCase(OPT_SEP)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_SEP);
                    return FAILURE;
                }
                doSEP = sArgs[i].toLowerCase().equals("true");
            }
            else if (sArg.equalsIgnoreCase(OPT_CHECK_ROOTS)) 
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_CHECK_ROOTS);
                    return FAILURE;
                }
                mbCheckRoots = sArgs[i].toLowerCase().equals("true");
            }   
            else if (sArg.equalsIgnoreCase(OPT_ROOT_FILE))
            {
                if (i == sArgs.length || isOption(sArgs[++i]))
                {
                    outputOptionRequiresArgument(sOutput, OPT_ROOT_FILE);
                    return FAILURE;
                }
                msRootFile = sArgs[i];
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

            sCurURI = msRootBangURI;
            oURL = new URL(sCurURI);
            oConn = oURL.openConnection();
            oConn.connect();
        }
        catch (Throwable oThrowable)
        {
            sOutput.append("Cannot connect to root authority URI: " + sCurURI);
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
             "usage: 1. xrilookup <XRI> [options] \n" + 
             "       2. xrilookup help \n" +
             "\n" +
             "The first form resolves the first segment of the given <XRI> and " +
             "outputs information about the final authority \n" +
             "\n" + 
             "Example usage: xrilookup xri://@community*member*family \n" +
             "\n" + 
             "Refer to the OASIS document \"Extensible Resource Identifier (XRI) \n" +
             "Resolution\" for information about XRI resolution. \n" +
             "\n" +
             "The second form generates this help message. \n" +
             "\n" +
             "Available options: \n" +
             "    -root_equals_auth <URI>: Root authority URI for '=' resolution. \n" +
             "    -root_at_auth <URI>:     Root authority URI for '@' resolution. \n" +
             "    -root_bang_auth <URI>:   Root authority URI for '!' resolution. \n" +
             "    -proxy <URI>:            Proxy URI (root authorities will not be used.\n" +
             "\n" +
             "    " + OPT_RES_MEDIA + " <return-media-type>: corresponds to _xrd_r\n" + 
             "    " + OPT_RES_TYPE + " <service-type>: corresponds to _xrd_t\n" +
             "    " + OPT_MEDIA_TYPE + " <service-media-type>: corresponds to _xrd_m\n" + 
             "    " + OPT_TRUST_TYPE + " (none|https|saml|saml+https)\n" +
             "    " + OPT_FOLLOW_REFS + " (true|false): Flag to indicate if Refs should be followed.\n" +
             "    " + OPT_SEP + " (true|false): Flag to indicate if SEP selection should be done.\n" +
             "    -verbose:        Print verbose output. \n" +
             "    -no_header:      Omit header from non-verbose output. \n" +
             "    " + OPT_CHECK_ROOTS + " (true|false): Flag to indicate if resolver should check for @,=,! roots first\n" +
             "    -roots <filename>: filename contains lines of <rootid><whitespace><authority URL> - can override options on command line\n" +
             
             "\n" +
             "The default value (used if unspecified) for the three '-root_' options are:\n" +
             "    -root_equals_auth " + ROOT_DEF_EQ_URI + ". \n" +
             "    -root_at_auth     " + ROOT_DEF_AT_URI + ". \n" +
             "    -root_bang_auth   " + ROOT_DEF_BANG_URI + ". \n" +
             "    -proxy            " + PROXY_URI + ".\n" +
             "    " + OPT_RES_MEDIA + " " + xrdR + "\n" +
             "    " + OPT_RES_TYPE + " " + xrdT + "\n" +
             "    " + OPT_MEDIA_TYPE + " " + xrdM + "\n" + 
             "    " + OPT_TRUST_TYPE + " " + trustType.toString() + "\n" +
             "    " + OPT_FOLLOW_REFS + " " + Boolean.toString(followRefs) + "\n" +
             "    " + OPT_SEP + " " + Boolean.toString(doSEP) + "\n" +
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
     }    
    
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
     * "xrilookup -help".
     */    
     private void outputPleaseTypeHelp(StringBuffer sOutput) 
     {
         sOutput.append("Type \"xrilookup help\". \n");
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
         
     }
     
     protected void setTrustType(String type)
     	throws IllegalTrustTypeException
     {
    	 trustType.setType(type);
     }
     
     /*
    ****************************************************************************
    * setRootsFromFile()
    ****************************************************************************
    */ /**
    * Scans list of command-line arguments, setting member variables for 
    * the data argument, and all options. 
    * @param oResolver- resolver object into which roots are set
    * @param sFilename- filename containing lines of <root auth><whitespace><auth service url> 
    */
    protected void setRootsFromFile(Resolver resolver, String filename) throws FileNotFoundException, IOException
    {
        BufferedReader bf=new BufferedReader(new FileReader(filename));
        String line;
        String trustParam = ";" + trustType.getParameterPair();
        while((line=bf.readLine()) != null)
        {
            if (line.startsWith("#"))
            {
                continue;
            }
            String [] parts=line.split("\\s");
            
            XRD newRoot = new XRD();
            Service eqAuthService = new Service();
            eqAuthService.addMediaType(Tags.CONTENT_TYPE_XRDS + trustParam, SEPElement.MATCH_ATTR_CONTENT, Boolean.FALSE);
            eqAuthService.addType(Tags.SERVICE_AUTH_RES);
            eqAuthService.addURI(parts[1].trim());
            newRoot.addService(eqAuthService);
            resolver.setAuthority(parts[0].trim(), newRoot);            
        }            
    }

} // Class: XRILookup
