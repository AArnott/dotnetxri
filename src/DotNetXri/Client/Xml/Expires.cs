package org.openxri.xml;

import java.util.Date;

import org.openxri.util.DOMUtils;

public class Expires extends SimpleXMLElement{
	private Date expires = null;
	
	public Expires (Expires e) {
		super(e);
	}
	
	public Expires(){
		super(Tags.TAG_EXPIRES);
	}
	
	public Expires(Date dateValue){
		super(Tags.TAG_EXPIRES);
		setDate(dateValue);
	}
	
	public Date getDate(){
		return expires;
	}
	
	public String getDateString(){
		if (expires == null) return "";	
		return DOMUtils.toXMLDateTime(expires);
	}
	
	public void setDate(Date dateValue){
		expires = dateValue;
		setValue(getDateString());
		
	}
}
