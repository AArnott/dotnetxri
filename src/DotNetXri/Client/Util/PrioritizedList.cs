package org.openxri.util;
using java.io.Serializable;
using java.util.ArrayList;
using java.util.Collections;
using java.util.Comparator;
using java.util.Iterator;
using java.util.TreeMap;

using org.openxri.xml.Service;


public class PrioritizedList : Serializable {

	public static final int POLICY_RANDOMIZE=1;
	public const String PRIORITY_NULL = "null";

	//contains priority values int the same numerical order
	private TreeMap sortedList = new TreeMap(new Item()); 


	public PrioritizedList() {
		
	}
	
	public Object clone() {
		PrioritizedList pl = new PrioritizedList();
		pl.sortedList = sortedList;
		return pl;
	}

	/**
	 * Add the given obj based unsigned priroity string
	 * if multiple objects are stored with same priority, they
	 * are grouped together and later applies random shuffling
	 * @param pPriority
	 * @param o
	 */
	public void addObject(String pPriority, Object o)
	{	

		String priority = pPriority;
		if(priority == null || o == null) return;

		if(priority.equalsIgnoreCase(PrioritizedList.PRIORITY_NULL)){
			priority=priority.toLowerCase();
		}else {
			//priority must be always +ve integer
			long l = Long.parseLong(priority); 		
			if (l < 0 ) return;
		}


		Item dbitem = (Item)sortedList.get(priority);

		if(dbitem != null){
			dbitem.objects.add(o);
		}else{
			dbitem = new Item();
			dbitem.priority = priority;
			dbitem.objects.add(o);
			sortedList.put(priority,dbitem);
		}
	}

	public ArrayList getList(){	
		ArrayList list = new ArrayList();

		for(Iterator i = sortedList.keySet().iterator(); i.hasNext();){		
			String priority = (String)i.next();
			Item item = (Item) sortedList.get(priority);
			applyPolicy(item);
			list.addAll(item.objects);
		}
		return list;
	}

	/* method used internally */
	private void applyPolicy(Item item){
		if(!item.policyExecuted){
			if(item.objects.size() > 0){
				Collections.shuffle(item.objects);	
				item.policyExecuted = true;
			}
		}
	}

	/* internal data structure & comparator for sorting */
	private class Item : Comparator, Serializable {

		String priority = PrioritizedList.PRIORITY_NULL; // lowest priority (infinite value)
		bool policyExecuted = false;
		ArrayList objects = new ArrayList();

		public int compare(Object a, Object b){
			if(a == null && b == null){
				throw new IllegalArgumentException ("arguments cannot be null");
			}
			if(!(a is String || b is String)){
				throw new IllegalArgumentException ("arguments must be of type 'String'");		    		 
			}

			String aitem = (String)a;
			String bitem = (String)b;
			if(aitem.equalsIgnoreCase(bitem) ){ // both null & integer values holds good
				return 0;
			}

			if(aitem.equals(PrioritizedList.PRIORITY_NULL)) return  1;
			if(bitem.equals(PrioritizedList.PRIORITY_NULL)) return -1;

			long aPriority = Long.parseLong(aitem);
			long bPriority = Long.parseLong(bitem);

			return (aPriority < bPriority) ? -1: 1;

		}
	}

	public bool equals(Object o) {

		PrioritizedList other = (PrioritizedList) o;

		if (other == null) return(false);
		if (other == this) return(true);

		return(this.sortedList == null ? other.sortedList == null : this.sortedList.equals(other.sortedList));
	}
}
