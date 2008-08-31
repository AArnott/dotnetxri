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
* Class: NoInitialDescriptorException
********************************************************************************
*/ /**
* Exception thrown when an initial descriptor could not be found for the
* community.
* @author chandra
*/
public class NoInitialDescriptorException
    :XRIResolutionException
{
    private String msXRI = null;

    /*
    ****************************************************************************
    * Constructor()
    ****************************************************************************
    */ /**
    * Constructor
    * @param sXRI - The XRI that had no community configured
    */
    public NoInitialDescriptorException(String sXRI)
    {
        super("No global authority server found for XRI " + sXRI);

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

} // Class: NoInitialDescriptorException
