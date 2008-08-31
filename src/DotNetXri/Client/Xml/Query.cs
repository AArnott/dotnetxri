package org.openxri.xml;

public class Query extends SimpleXMLElement{
	
  public Query(String queryString){
	  super(Tags.TAG_QUERY);
	  setValue(queryString);
  }
  
  public Query(){
	  super(Tags.TAG_QUERY);
  }
  
  public Query(Query q) { super(q); }
}
