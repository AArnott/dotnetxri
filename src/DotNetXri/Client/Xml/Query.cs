package org.openxri.xml;

public class Query :SimpleXMLElement{
	
  public Query(String queryString): base(Tags.TAG_QUERY) {
	  setValue(queryString);
  }
  
  public Query(): base(Tags.TAG_QUERY) {
  }
  
  public Query(Query q) { super(q); }
}
