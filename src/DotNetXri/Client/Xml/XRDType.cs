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
package org.openxri.xml;


/**
 * 
 * @author =wil
 * @see org.openxri.xml.XRDType
 */
public class XRDType :SimpleXMLElement
{
	public XRDType(XRDType s) : base(s) {
	}
	
	public XRDType() : base(Tags.TAG_TYPE) {
	}

	public XRDType(String type) : base(Tags.TAG_TYPE) {
		setType(type);
	}
	
	public String getType() {
		return getValue();
	}
	
	public void setType(String type) {
		setValue(type);
	}
}
