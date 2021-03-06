/*
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using java.lang;
using stab.lang;

namespace stab.query {

	public interface Lookup<TKey, TElement> : Iterable<Grouping<TKey, TElement>> {
		bool contains(TKey key);
		int Count^;
		Iterable<TElement> this[TKey key]^;
	}

	public interface IntLookup<TElement> : Iterable<IntGrouping<TElement>> {
		bool contains(int key);
		int Count^;
		Iterable<TElement> this[int key]^;
	}

	public interface LongLookup<TElement> : Iterable<LongGrouping<TElement>> {
		bool contains(long key);
		int Count^;
		Iterable<TElement> this[long key]^;
	}

	public interface FloatLookup<TElement> : Iterable<FloatGrouping<TElement>> {
		bool contains(float key);
		int Count^;
		Iterable<TElement> this[float key]^;
	}

	public interface DoubleLookup<TElement> : Iterable<DoubleGrouping<TElement>> {
		bool contains(double key);
		int Count^;
		Iterable<TElement> this[double key]^;
	}
	
	class DefaultLookup<TKey, TElement> : Lookup<TKey, TElement> {
		private HashSet<TKey> keySet;
		private DefaultGrouping<TKey, TElement>[] groupings;
		private DefaultGrouping<TKey, TElement> lastGrouping;
		
		DefaultLookup(EqualityTester<TKey> comparator) {
			this.keySet = new HashSet<TKey>(comparator, 7);
			this.groupings = new DefaultGrouping<TKey, TElement>[7];
		}
		
		public bool contains(TKey key) {
			return keySet.find(key) != -1;
		}
		
		public int Count {
			get {
				return keySet.count();
			}
		}
		
		public Iterable<TElement> this[TKey key] {
			get {
				var grouping = getGrouping(key, false);
				if (grouping == null) {
					return Query.empty();
				} else {
					return grouping;
				}
			}
		}
		
		public java.util.Iterator<Grouping<TKey, TElement>> iterator() {
			var g = lastGrouping;
			if (g != null) {
				while ((g = g.Next) != lastGrouping) {
					yield return g;
				}
				yield return g;
			}
		}
		
		DefaultGrouping<TKey, TElement> getGrouping(TKey key, bool create) {
			int index = keySet.find(key);
			if (index == -1) {
				if (create) {
					index = keySet.addCore(key);
					if (index == sizeof(groupings)) {
						var t = new DefaultGrouping<TKey, TElement>[keySet.capacity()];
						System.arraycopy(groupings, 0, t, 0, sizeof(groupings));
						groupings = t;
					}
					var g = new DefaultGrouping<TKey, TElement>(key);
					groupings[index] = g;
					if (lastGrouping == null) {
						g.Next = g;
					} else {
						g.Next = lastGrouping.Next;
						lastGrouping.Next = g;
					}
					lastGrouping = g;
				} else {
					return null;
				}
			}
			return groupings[index];
		}
	}
	
	class DefaultIntLookup<TElement> : IntLookup<TElement> {
		private IntHashSet keySet;
		private DefaultIntGrouping<TElement>[] groupings;
		private DefaultIntGrouping<TElement> lastGrouping;
		
		DefaultIntLookup() {
			this.keySet = new IntHashSet(7);
			this.groupings = new DefaultIntGrouping<TElement>[7];
		}
		
		public bool contains(int key) {
			return keySet.find(key) != -1;
		}
		
		public int Count {
			get {
				return keySet.count();
			}
		}
		
		public Iterable<TElement> this[int key] {
			get {
				var grouping = getGrouping(key, false);
				if (grouping == null) {
					return Query.empty();
				} else {
					return grouping;
				}
			}
		}
		
		public java.util.Iterator<IntGrouping<TElement>> iterator() {
			var g = lastGrouping;
			if (g != null) {
				while ((g = g.Next) != lastGrouping) {
					yield return g;
				}
				yield return g;
			}
		}
		
		DefaultIntGrouping<TElement> getGrouping(int key, bool create) {
			int index = keySet.find(key);
			if (index == -1) {
				if (create) {
					index = keySet.addCore(key);
					if (index == sizeof(groupings)) {
						var t = new DefaultIntGrouping<TElement>[keySet.capacity()];
						System.arraycopy(groupings, 0, t, 0, sizeof(groupings));
						groupings = t;
					}
					var g = new DefaultIntGrouping<TElement>(key);
					groupings[index] = g;
					if (lastGrouping == null) {
						g.Next = g;
					} else {
						g.Next = lastGrouping.Next;
						lastGrouping.Next = g;
					}
					lastGrouping = g;
				} else {
					return null;
				}
			}
			return groupings[index];
		}
	}
	
	class DefaultLongLookup<TElement> : LongLookup<TElement> {
		private LongHashSet keySet;
		private DefaultLongGrouping<TElement>[] groupings;
		private DefaultLongGrouping<TElement> lastGrouping;
		
		DefaultLongLookup() {
			this.keySet = new LongHashSet(7);
			this.groupings = new DefaultLongGrouping<TElement>[7];
		}
		
		public bool contains(long key) {
			return keySet.find(key) != -1;
		}
		
		public int Count {
			get {
				return keySet.count();
			}
		}
		
		public Iterable<TElement> this[long key] {
			get {
				var grouping = getGrouping(key, false);
				if (grouping == null) {
					return Query.empty();
				} else {
					return grouping;
				}
			}
		}
		
		public java.util.Iterator<LongGrouping<TElement>> iterator() {
			var g = lastGrouping;
			if (g != null) {
				while ((g = g.Next) != lastGrouping) {
					yield return g;
				}
				yield return g;
			}
		}
		
		DefaultLongGrouping<TElement> getGrouping(long key, bool create) {
			int index = keySet.find(key);
			if (index == -1) {
				if (create) {
					index = keySet.addCore(key);
					if (index == sizeof(groupings)) {
						var t = new DefaultLongGrouping<TElement>[keySet.capacity()];
						System.arraycopy(groupings, 0, t, 0, sizeof(groupings));
						groupings = t;
					}
					var g = new DefaultLongGrouping<TElement>(key);
					groupings[index] = g;
					if (lastGrouping == null) {
						g.Next = g;
					} else {
						g.Next = lastGrouping.Next;
						lastGrouping.Next = g;
					}
					lastGrouping = g;
				} else {
					return null;
				}
			}
			return groupings[index];
		}
	}
	
	class DefaultFloatLookup<TElement> : FloatLookup<TElement> {
		private FloatHashSet keySet;
		private DefaultFloatGrouping<TElement>[] groupings;
		private DefaultFloatGrouping<TElement> lastGrouping;
		
		DefaultFloatLookup() {
			this.keySet = new FloatHashSet(7);
			this.groupings = new DefaultFloatGrouping<TElement>[7];
		}
		
		public bool contains(float key) {
			return keySet.find(key) != -1;
		}
		
		public int Count {
			get {
				return keySet.count();
			}
		}
		
		public Iterable<TElement> this[float key] {
			get {
				var grouping = getGrouping(key, false);
				if (grouping == null) {
					return Query.empty();
				} else {
					return grouping;
				}
			}
		}
		
		public java.util.Iterator<FloatGrouping<TElement>> iterator() {
			var g = lastGrouping;
			if (g != null) {
				while ((g = g.Next) != lastGrouping) {
					yield return g;
				}
				yield return g;
			}
		}
		
		DefaultFloatGrouping<TElement> getGrouping(float key, bool create) {
			int index = keySet.find(key);
			if (index == -1) {
				if (create) {
					index = keySet.addCore(key);
					if (index == sizeof(groupings)) {
						var t = new DefaultFloatGrouping<TElement>[keySet.capacity()];
						System.arraycopy(groupings, 0, t, 0, sizeof(groupings));
						groupings = t;
					}
					var g = new DefaultFloatGrouping<TElement>(key);
					groupings[index] = g;
					if (lastGrouping == null) {
						g.Next = g;
					} else {
						g.Next = lastGrouping.Next;
						lastGrouping.Next = g;
					}
					lastGrouping = g;
				} else {
					return null;
				}
			}
			return groupings[index];
		}
	}
	
	class DefaultDoubleLookup<TElement> : DoubleLookup<TElement> {
		private DoubleHashSet keySet;
		private DefaultDoubleGrouping<TElement>[] groupings;
		private DefaultDoubleGrouping<TElement> lastGrouping;
		
		DefaultDoubleLookup() {
			this.keySet = new DoubleHashSet(7);
			this.groupings = new DefaultDoubleGrouping<TElement>[7];
		}
		
		public bool contains(double key) {
			return keySet.find(key) != -1;
		}
		
		public int Count {
			get {
				return keySet.count();
			}
		}
		
		public Iterable<TElement> this[double key] {
			get {
				var grouping = getGrouping(key, false);
				if (grouping == null) {
					return Query.empty();
				} else {
					return grouping;
				}
			}
		}
		
		public java.util.Iterator<DoubleGrouping<TElement>> iterator() {
			var g = lastGrouping;
			if (g != null) {
				while ((g = g.Next) != lastGrouping) {
					yield return g;
				}
				yield return g;
			}
		}
		
		DefaultDoubleGrouping<TElement> getGrouping(double key, bool create) {
			int index = keySet.find(key);
			if (index == -1) {
				if (create) {
					index = keySet.addCore(key);
					if (index == sizeof(groupings)) {
						var t = new DefaultDoubleGrouping<TElement>[keySet.capacity()];
						System.arraycopy(groupings, 0, t, 0, sizeof(groupings));
						groupings = t;
					}
					var g = new DefaultDoubleGrouping<TElement>(key);
					groupings[index] = g;
					if (lastGrouping == null) {
						g.Next = g;
					} else {
						g.Next = lastGrouping.Next;
						lastGrouping.Next = g;
					}
					lastGrouping = g;
				} else {
					return null;
				}
			}
			return groupings[index];
		}
	}
}
