namespace DotNetXri.Client.Util {

	using java.io.Serializable;
	using java.util.ArrayList;
	using java.util.Collections;
	using java.util.Comparator;
	using java.util.Iterator;
	using java.util.TreeMap;

	using DotNetXri.Client.Xml;
	using System;
	using System.Collections;

	[Serializable]
	public class PrioritizedList /*: Serializable*/ {

		public const int POLICY_RANDOMIZE = 1;
		public const string PRIORITY_NULL = "null";

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
		public void addObject(string pPriority, Object o) {

			string priority = pPriority;
			if (priority == null || o == null) return;

			if (priority.Equals(PrioritizedList.PRIORITY_NULL, StringComparison.OrdinalIgnoreCase)) {
				priority = priority.ToLowerInvariant();
			} else {
				//priority must be always +ve integer
				long l = long.Parse(priority);
				if (l < 0) return;
			}


			Item dbitem = (Item)sortedList.get(priority);

			if (dbitem != null) {
				dbitem.objects.Add(o);
			} else {
				dbitem = new Item();
				dbitem.priority = priority;
				dbitem.objects.Add(o);
				sortedList[priority] = dbitem;
			}
		}

		public ArrayList getList() {
			ArrayList list = new ArrayList();

			foreach( string priority in sortedList.Keys) {
				Item item = (Item)sortedList.get(priority);
				applyPolicy(item);
				list.AddRange(item.objects);
			}
			return list;
		}

		/* method used internally */
		private void applyPolicy(Item item) {
			if (!item.policyExecuted) {
				if (item.objects.Count > 0) {
					Collections.shuffle(item.objects);
					item.policyExecuted = true;
				}
			}
		}

		/* internal data structure & comparator for sorting */
		private class Item : Comparator, Serializable {

			internal string priority = PrioritizedList.PRIORITY_NULL; // lowest priority (infinite value)
			internal bool policyExecuted = false;
			internal ArrayList objects = new ArrayList();

			public int compare(Object a, Object b) {
				if (a == null && b == null) {
					throw new ArgumentException("arguments cannot be null");
				}
				if (!(a is string || b is string)) {
					throw new ArgumentException("arguments must be of type 'string'");
				}

				string aitem = (string)a;
				string bitem = (string)b;
				if (aitem.Equals(bitem, StringComparison.OrdinalIgnoreCase)) { // both null & integer values holds good
					return 0;
				}

				if (aitem.Equals(PrioritizedList.PRIORITY_NULL)) return 1;
				if (bitem.Equals(PrioritizedList.PRIORITY_NULL)) return -1;

				long aPriority = long.Parse(aitem);
				long bPriority = long.Parse(bitem);

				return (aPriority < bPriority) ? -1 : 1;

			}
		}

		public bool Equals(Object o) {

			PrioritizedList other = (PrioritizedList)o;

			if (other == null) return (false);
			if (other == this) return (true);

			return (this.sortedList == null ? other.sortedList == null : this.sortedList.Equals(other.sortedList));
		}
	}
}