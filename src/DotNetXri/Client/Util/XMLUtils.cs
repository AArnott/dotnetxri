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
package org.openxri.util;

import org.doomdark.uuid.UUIDGenerator;


/*
********************************************************************************
* Class: XMLUtils
********************************************************************************
*/ /**
* Provides XML Utilities.
* @author =chetan
*/
public class XMLUtils
{
    private static UUIDGenerator soGenerator = UUIDGenerator.getInstance();

    /*
    ****************************************************************************
    * genXmlID()
    ****************************************************************************
    */ /**
    * Generates a new Time-based UUID that can be used in an XML id attribute.
    */
    public static String genXmlID()
    {
        return soGenerator.generateTimeBasedUUID().toString();

    } // genXmlID()

} // Class: XMLUtils
