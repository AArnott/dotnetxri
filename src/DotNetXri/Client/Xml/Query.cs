namespace DotNetXri.Client.Xml {

public class Query : SimpleXMLElement {

	public Query(string queryString)
		: base(Tags.TAG_QUERY) {
		setValue(queryString);
	}

	public Query()
		: base(Tags.TAG_QUERY) {
	}

	public Query(Query q) { super(q); }
}
}