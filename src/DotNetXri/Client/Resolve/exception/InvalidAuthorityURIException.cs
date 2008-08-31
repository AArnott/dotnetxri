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
* Class: InvalidAuthorityURIException
********************************************************************************
*/ /**
* Exception thrown when a URI obtained through XRIAuthority is invalid
* or when the URL built to send a request us invalid.
* @author chandra
*/
public class InvalidAuthorityURIException
    :XRIResolutionException
{
    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructor
    */
    public InvalidAuthorityURIException(String sMsg)
    : base(sMsg) {

    } // Constructor()

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructor
    */
    public InvalidAuthorityURIException(String sMsg, Exception oEx)
    : base(sMsg, oEx) {

    } // Constructor()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    *
    */
    public String toString()
    {
        return base.toString();

    } // toString()

} // Class: InvalidAuthorityURIException
