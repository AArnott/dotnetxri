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
* Class: BadAuthorityException
********************************************************************************
*/ /**
* Exception thrown when a XRI being resolved does not have an authority path
* or has an unsupported authority path type
* @author chandra
*/
public class BadAuthorityException
    extends XRIResolutionException
{
    private String msXRI = null;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    *
    */
    public BadAuthorityException(String sXRI)
    {
        super("Missing global authority symbol in XRI " + sXRI);

        msXRI = sXRI;

    } // Constructor()

    /*
    ****************************************************************************
    * getXRI()
    ****************************************************************************
    */ /**
    *
    */
    public String getXRI()
    {
        return msXRI;

    } // getXRI()

    /*
    ****************************************************************************
    * toString()
    ****************************************************************************
    */ /**
    *
    */
    public String toString()
    {
        return super.toString();

    } // toString()

} // Class: BadAuthorityException
