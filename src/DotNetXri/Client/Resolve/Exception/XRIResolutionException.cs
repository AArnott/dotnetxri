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
// This package
package org.openxri.resolve.exception;


/*
********************************************************************************
* Class: XRIResolutionException
********************************************************************************
*/ /**
* Base class for exceptions thrown during XRI Resolution
* @author chandra
*/
public class XRIResolutionException
    :java.lang.Exception
{
    private Exception moEx = null;
    private String status = null;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructs an exception with the given message
    */
    public XRIResolutionException(String sMsg)
    : base(sMsg) {

    } // Constructor()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructs an exception with the given message and underlying exception
    */
    public XRIResolutionException(String sMsg, Exception oEx)
    : base(sMsg) {
        moEx = oEx;

    } // Constructor()

    public XRIResolutionException(String statusCode, String sMsg)
    : base(sMsg) {
    	status = statusCode;
    }
    /*
    ****************************************************************************
    * printUnderlyingStackTrace()
    ****************************************************************************
    */ /**
    * Prints out the underlying exception
    */
    public void printUnderlyingStackTrace()
    {
        if (moEx != null)
        {
            moEx.printStackTrace();
        }

    } // printUnderlyingStackTrace()

	/**
	 * @return Returns the status.
	 */
	public String getStatus() {
		return status;
	}

} // Class: XRIResolutionException
