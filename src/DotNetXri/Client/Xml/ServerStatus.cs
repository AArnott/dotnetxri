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
 * @see org.openxri.xml.Status
 */
public class ServerStatus :SimpleXMLElement
{
	public ServerStatus(ServerStatus s) : base(s) {
	}
	
	public ServerStatus() : base(Tags.TAG_SERVERSTATUS) {
	}

	public ServerStatus(String code) : base(Tags.TAG_SERVERSTATUS) {
		setCode(code);	
	}	
	
	public ServerStatus(String code, String text) : base(Tags.TAG_SERVERSTATUS, text) {
		setCode(code);
	}
	
	public String getCode() {
		return getAttributeValue(Tags.ATTR_CODE);
	}
	
	public String getText() {
		return getValue();
	}
	
	public void setCode(String code) {
		addAttribute(Tags.ATTR_CODE, code);
	}
	
	public void setText(String text) {
		setValue(text);
	}
}
