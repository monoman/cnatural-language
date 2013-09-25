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
using java.lang.reflect;
using java.util;
using stab.lang;
 
namespace stab.query {

	abstract class CollectionBase<E> : Collection<E> {
		public bool addAll(Collection<E> c) {
			bool result = false;
			foreach (var d in c) {
				result = true;
				add(d);
			}
			return result;
		}
		
		public bool containsAll(Collection<?> c) {
			foreach (var o in c) {
				if (!contains(o)) {
					return false;
				}
			}
			return true;
		}
		
		public bool removeAll(Collection<?> c) {
			var result = false;
			foreach (var o in c) {
				if (remove(o)) {
					result = true;
				}
			}
			return result;
		}
		
		public Object[] toArray() {
			var result = new Object[size()];
			int index = 0;
			foreach (var i in this) {
				result[index++] = i;
			}
			return result;
		}
		
		public T[] toArray<T>(T[] a) {
			int len = size();
			T[] result;
			if (sizeof(a) < len) {
				result = (T[])Array.newInstance(a.getClass().getComponentType(), len);
			} else {
				result = a;
			}
			int index = 0;
			foreach (var i in this) {
				Array.set(result, index++, i);
			}
			if (sizeof(result) > len) {
				result[len] = null;
			}
			return result;
		}
	}

	// ========================================================================
	// Lists
	// ========================================================================

	abstract class ListBase<E> : CollectionBase<E>, List<E> {
		protected int size*;

		public void clear() {
			size = 0;
		}
		
		public bool isEmpty() {
			return size == 0;
		}
		
		public ListIterator<E> listIterator() {
			throw new UnsupportedOperationException();
		}
		
		public ListIterator<E> listIterator(int index) {
			throw new UnsupportedOperationException();
		}

		public int size() {
			return size;
		}
	}

	// ========================================================================

	class InternalIntList : ListBase<Integer>, IntList {
		private final static int[] Empty = new int[0];
	
		private int[] items;
		
		InternalIntList(IntIterable source) {
			items = Empty;
			foreach (var i in source) {
				add(i);
			}
		}
		
		// Collection
		
		public bool add(Integer i) {
			return add(i.intValue());
		}
		
		public bool contains(Object o) {
			var i = o as Integer;
			if (i == null) {
				return false;
			}
			return contains(i.intValue());
		}
		
		public bool remove(Object o) {
			var i = o as Integer;
			if (i == null) {
				return false;
			}
			return removeItem(i.intValue());
		}

		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new IntHashSet(7);
			foreach (var o in c) {
				var i = o as Integer;
				if (i != null) {
					set.add(i.intValue());
				}
			}
			return retainAll(set);
		}
		
		private bool retainAll(IntHashSet set) {
			var t = new int[Math.min(size, set.count())];
			int len = 0;
			for (int i = 0; i < size; i++) {
				var item = items[i];
				if (set.contains(item)) {
					t[len++] = item;
				}
			}
			if (size != len) {
				items = t;
				size = len;
				return true;
			}
			return false;
		}

		// IntCollection
		
		public bool add(int i) {
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			items[size++] = i;
			return true;
		}
		
		public bool addAll(IntCollection c) {
			var result = false;
			foreach (var i in c) {
				result = true;
				add(i);
			}
			return result;
		}
		
		public bool contains(int i) {
			for (int index = 0; index < size; index++) {
				if (items[index] == i) {
					return true;
				}
			}
			return false;
		}
		
		public bool containsAll(IntCollection c) {
			foreach (var i in c) {
				if (!contains(i)) {
					return false;
				}
			}
			return true;
		}
		
		public IntIterator iterator() {
			for (int i = 0; i < size; i++) {
				yield return items[i];
			}
		}
		
		public bool removeItem(int i) {
			for (int index = 0; index < size; index++) {
				if (items[index] == i) {
					removeAt(index);
					return true;
				}
			}
			return false;
		}
		
		public bool removeAll(IntCollection c) {
			var result = false;
			foreach (var i in c) {
				if (removeItem(i)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(IntCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new IntHashSet(7);
			foreach (var i in c) {
				set.add(i);
			}
			return retainAll(set);
		}
		
		public int[] toArray(int[] a) {
			int[] result = (sizeof(a) < size) ? new int[size] : a;
			System.arraycopy(items, 0, result, 0, size);
			return result;
		}

		// List
		
		public void add(int index, Integer i) {
			if (i == null) {
				throw new NullPointerException("i");
			}
			add(index, i.intValue());
		}

		public bool addAll(int index, Collection<Integer> c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var i in c) {
				result = true;
				items[index++] = i;
			}
			size += len;
			return result;
		}

		public Integer get(int index) {
			return getItem(index);
		}

		public int indexOf(Object o) {
			var i = o as Integer;
			if (i == null) {
				return -1;
			}
			return indexOf(i.intValue());
		}
		
		public int lastIndexOf(Object o) {
			var i = o as Integer;
			if (i == null) {
				return -1;
			}
			return lastIndexOf(i.intValue());
		}

		public Integer remove(int index) {
			return removeAt(index);
		}

		public Integer set(int index, Integer i) {
			if (i == null) {
				throw new NullPointerException("i");
			}
			return setItem(index, i.intValue());
		}
		
		// IntList
		
		public void add(int index, int i) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + 1, size - index);
			}
			items[index] = i;
			size++;
		}
		
		public bool addAll(int index, IntCollection c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var i in c) {
				result = true;
				items[index++] = i;
			}
			size += len;
			return result;
		}
		
		public int indexOf(int i) {
			for (int index = 0; index < size; index++) {
				if (items[index] == i) {
					return index;
				}
			}
			return -1;
		}
		
		public int lastIndexOf(int i) {
			for (int index = size - 1; index >= 0; index++) {
				if (items[index] == i) {
					return index;
				}
			}
			return -1;
		}
		
		public int removeAt(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			size--;
			int result = items[index];
			if (index < size) {
				System.arraycopy(items, index + 1, items, index, size - index);
			}
			return result;
		}
		
		public IntList subList(int fromIndex, int toIndex) {
			throw new UnsupportedOperationException();
		}

		public int getItem(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			return items[index];
		}
		
		public int setItem(int index, int value) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int result = items[index];
			items[index] = value;
			return result;
		}
		
		private void resize(int newSize) {
			var length = sizeof(items);
			if (length == 0) {
				items = new int[(newSize == 0) ? 4 : newSize];
			} else {
				var t = new int[newSize];
				System.arraycopy(items, 0, t, 0, length);
				items = t;
			}
		}
	}
	
	// ========================================================================

	class InternalLongList : ListBase<Long>, LongList {
		private final static long[] Empty = new long[0];
	
		private long[] items;
		
		InternalLongList(LongIterable source) {
			items = Empty;
			foreach (var d in source) {
				add(d);
			}
		}
		
		// Collection
		
		public bool add(Long l) {
			return add(l.longValue());
		}
		
		public bool contains(Object o) {
			var l = o as Long;
			if (l == null) {
				return false;
			}
			return contains(l.longValue());
		}
		
		public bool remove(Object o) {
			var l = o as Long;
			if (l == null) {
				return false;
			}
			return removeItem(l.longValue());
		}

		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new LongHashSet(7);
			foreach (var o in c) {
				var l = o as Long;
				if (l != null) {
					set.add(l.longValue());
				}
			}
			return retainAll(set);
		}
		
		// LongCollection
		
		public bool add(long l) {
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			items[size++] = l;
			return true;
		}
		
		public bool addAll(LongCollection c) {
			var result = false;
			foreach (var i in c) {
				result = true;
				add(i);
			}
			return result;
		}
		
		public bool contains(long l) {
			for (int index = 0; index < size; index++) {
				if (items[index] == l) {
					return true;
				}
			}
			return false;
		}
		
		public bool containsAll(LongCollection c) {
			foreach (var l in c) {
				if (!contains(l)) {
					return false;
				}
			}
			return true;
		}
		
		public LongIterator iterator() {
			for (int i = 0; i < size; i++) {
				yield return items[i];
			}
		}
		
		public bool removeItem(long l) {
			for (int index = 0; index < size; index++) {
				if (items[index] == l) {
					removeAt(index);
					return true;
				}
			}
			return false;
		}
		
		public bool removeAll(LongCollection c) {
			var result = false;
			foreach (var l in c) {
				if (removeItem(l)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(LongCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new LongHashSet(7);
			foreach (var l in c) {
				set.add(l);
			}
			return retainAll(set);
		}

		private bool retainAll(LongHashSet set) {
			var t = new long[Math.min(size, set.count())];
			int len = 0;
			for (int i = 0; i < size; i++) {
				var item = items[i];
				if (set.contains(item)) {
					t[len++] = item;
				}
			}
			if (size != len) {
				items = t;
				size = len;
				return true;
			}
			return false;
		}
		
		public long[] toArray(long[] a) {
			var result = (sizeof(a) < size) ? new long[size] : a;
			System.arraycopy(items, 0, result, 0, size);
			return result;
		}

		// List
		
		public void add(int index, Long l) {
			if (l == null) {
				throw new NullPointerException("l");
			}
			add(index, l.longValue());
		}

		public bool addAll(int index, Collection<Long> c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var l in c) {
				result = true;
				items[index++] = l;
			}
			size += len;
			return result;
		}

		public Long get(int index) {
			return getItem(index);
		}

		public int indexOf(Object o) {
			var l = o as Long;
			if (l == null) {
				return -1;
			}
			return indexOf(l.longValue());
		}
		
		public int lastIndexOf(Object o) {
			var l = o as Long;
			if (l == null) {
				return -1;
			}
			return lastIndexOf(l.longValue());
		}
		
		public Long remove(int index) {
			return removeAt(index);
		}

		public Long set(int index, Long l) {
			if (l == null) {
				throw new NullPointerException("l");
			}
			return setItem(index, l.longValue());
		}
		
		// LongList
		
		public void add(int index, long l) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + 1, size - index);
			}
			items[index] = l;
			size++;
		}
		
		public bool addAll(int index, LongCollection c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var l in c) {
				result = true;
				items[index++] = l;
			}
			size += len;
			return result;
		}
		
		public int indexOf(long l) {
			for (int index = 0; index < size; index++) {
				if (items[index] == l) {
					return index;
				}
			}
			return -1;
		}
		
		public int lastIndexOf(long l) {
			for (int index = size - 1; index >= 0; index++) {
				if (items[index] == l) {
					return index;
				}
			}
			return -1;
		}
		
		public long removeAt(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			size--;
			var result = items[index];
			if (index < size) {
				System.arraycopy(items, index + 1, items, index, size - index);
			}
			return result;
		}
		
		public LongList subList(int fromIndex, int toIndex) {
			throw new UnsupportedOperationException();
		}

		public long getItem(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			return items[index];
		}
		
		public long setItem(int index, long value) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			var result = items[index];
			items[index] = value;
			return result;
		}
		
		private void resize(int newSize) {
			var length = sizeof(items);
			if (length == 0) {
				items = new long[(newSize == 0) ? 4 : newSize];
			} else {
				var t = new long[newSize];
				System.arraycopy(items, 0, t, 0, length);
				items = t;
			}
		}
	}

	// ========================================================================

	class InternalFloatList : ListBase<Float>, FloatList {
		private final static float[] Empty = new float[0];
	
		private float[] items;
		
		InternalFloatList(FloatIterable source) {
			items = Empty;
			foreach (var d in source) {
				add(d);
			}
		}
		
		// Collection
		
		public bool add(Float f) {
			return add(f.floatValue());
		}
		
		public bool contains(Object o) {
			var f = o as Float;
			if (f == null) {
				return false;
			}
			return contains(f.floatValue());
		}
		
		public bool remove(Object o) {
			var f = o as Float;
			if (f == null) {
				return false;
			}
			return removeItem(f.floatValue());
		}

		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new FloatHashSet(7);
			foreach (var o in c) {
				var f = o as Float;
				if (f != null) {
					set.add(f.floatValue());
				}
			}
			return retainAll(set);
		}
		
		// FloatCollection
		
		public bool add(float f) {
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			items[size++] = f;
			return true;
		}
		
		public bool addAll(FloatCollection c) {
			var result = false;
			foreach (var i in c) {
				result = true;
				add(i);
			}
			return result;
		}
		
		public bool contains(float f) {
			for (int index = 0; index < size; index++) {
				if (items[index] == f) {
					return true;
				}
			}
			return false;
		}
		
		public bool containsAll(FloatCollection c) {
			foreach (var f in c) {
				if (!contains(f)) {
					return false;
				}
			}
			return true;
		}
		
		public FloatIterator iterator() {
			for (int i = 0; i < size; i++) {
				yield return items[i];
			}
		}
		
		public bool removeItem(float f) {
			for (int index = 0; index < size; index++) {
				if (items[index] == f) {
					removeAt(index);
					return true;
				}
			}
			return false;
		}
		
		public bool removeAll(FloatCollection c) {
			var result = false;
			foreach (var f in c) {
				if (removeItem(f)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(FloatCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new FloatHashSet(7);
			foreach (var f in c) {
				set.add(f);
			}
			return retainAll(set);
		}

		private bool retainAll(FloatHashSet set) {
			var t = new float[Math.min(size, set.count())];
			int len = 0;
			for (int i = 0; i < size; i++) {
				var item = items[i];
				if (set.contains(item)) {
					t[len++] = item;
				}
			}
			if (size != len) {
				items = t;
				size = len;
				return true;
			}
			return false;
		}
		
		public float[] toArray(float[] a) {
			var result = (sizeof(a) < size) ? new float[size] : a;
			System.arraycopy(items, 0, result, 0, size);
			return result;
		}

		// List
		
		public void add(int index, Float f) {
			if (f == null) {
				throw new NullPointerException("f");
			}
			add(index, f.floatValue());
		}

		public bool addAll(int index, Collection<Float> c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var f in c) {
				result = true;
				items[index++] = f;
			}
			size += len;
			return result;
		}

		public Float get(int index) {
			return getItem(index);
		}

		public int indexOf(Object o) {
			var f = o as Float;
			if (f == null) {
				return -1;
			}
			return indexOf(f.floatValue());
		}
		
		public int lastIndexOf(Object o) {
			var f = o as Float;
			if (f == null) {
				return -1;
			}
			return lastIndexOf(f.floatValue());
		}
		
		public Float remove(int index) {
			return removeAt(index);
		}

		public Float set(int index, Float f) {
			if (f == null) {
				throw new NullPointerException("f");
			}
			return setItem(index, f.floatValue());
		}
		
		// FloatList
		
		public void add(int index, float f) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + 1, size - index);
			}
			items[index] = f;
			size++;
		}
		
		public bool addAll(int index, FloatCollection c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var f in c) {
				result = true;
				items[index++] = f;
			}
			size += len;
			return result;
		}
		
		public int indexOf(float f) {
			for (int index = 0; index < size; index++) {
				if (items[index] == f) {
					return index;
				}
			}
			return -1;
		}
		
		public int lastIndexOf(float f) {
			for (int index = size - 1; index >= 0; index++) {
				if (items[index] == f) {
					return index;
				}
			}
			return -1;
		}
		
		public float removeAt(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			size--;
			var result = items[index];
			if (index < size) {
				System.arraycopy(items, index + 1, items, index, size - index);
			}
			return result;
		}
		
		public FloatList subList(int fromIndex, int toIndex) {
			throw new UnsupportedOperationException();
		}

		public float getItem(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			return items[index];
		}
		
		public float setItem(int index, float value) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			var result = items[index];
			items[index] = value;
			return result;
		}
		
		private void resize(int newSize) {
			var length = sizeof(items);
			if (length == 0) {
				items = new float[(newSize == 0) ? 4 : newSize];
			} else {
				var t = new float[newSize];
				System.arraycopy(items, 0, t, 0, length);
				items = t;
			}
		}
	}

	// ========================================================================

	class InternalDoubleList : ListBase<Double>, DoubleList {
		private final static double[] Empty = new double[0];
	
		private double[] items;
		
		InternalDoubleList(DoubleIterable source) {
			items = Empty;
			foreach (var d in source) {
				add(d);
			}
		}
		
		// Collection
		
		public bool add(Double d) {
			return add(d.doubleValue());
		}
		
		public bool contains(Object o) {
			var d = o as Double;
			if (d == null) {
				return false;
			}
			return contains(d.doubleValue());
		}
		
		public bool remove(Object o) {
			var d = o as Double;
			if (d == null) {
				return false;
			}
			return removeItem(d.doubleValue());
		}

		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new DoubleHashSet(7);
			foreach (var o in c) {
				var d = o as Double;
				if (d != null) {
					set.add(d.doubleValue());
				}
			}
			return retainAll(set);
		}
		
		// DoubleCollection
		
		public bool add(double d) {
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			items[size++] = d;
			return true;
		}
		
		public bool addAll(DoubleCollection c) {
			var result = false;
			foreach (var i in c) {
				result = true;
				add(i);
			}
			return result;
		}
		
		public bool contains(double d) {
			for (int index = 0; index < size; index++) {
				if (items[index] == d) {
					return true;
				}
			}
			return false;
		}
		
		public bool containsAll(DoubleCollection c) {
			foreach (var f in c) {
				if (!contains(f)) {
					return false;
				}
			}
			return true;
		}
		
		public DoubleIterator iterator() {
			for (int i = 0; i < size; i++) {
				yield return items[i];
			}
		}
		
		public bool removeItem(double d) {
			for (int index = 0; index < size; index++) {
				if (items[index] == d) {
					removeAt(index);
					return true;
				}
			}
			return false;
		}
		
		public bool removeAll(DoubleCollection c) {
			var result = false;
			foreach (var d in c) {
				if (removeItem(d)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(DoubleCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var set = new DoubleHashSet(7);
			foreach (var d in c) {
				set.add(d);
			}
			return retainAll(set);
		}

		private bool retainAll(DoubleHashSet set) {
			var t = new double[Math.min(size, set.count())];
			int len = 0;
			for (int i = 0; i < size; i++) {
				var item = items[i];
				if (set.contains(item)) {
					t[len++] = item;
				}
			}
			if (size != len) {
				items = t;
				size = len;
				return true;
			}
			return false;
		}
		
		public double[] toArray(double[] a) {
			var result = (sizeof(a) < size) ? new double[size] : a;
			System.arraycopy(items, 0, result, 0, size);
			return result;
		}

		// List
		
		public void add(int index, Double d) {
			if (d == null) {
				throw new NullPointerException("d");
			}
			add(index, d.doubleValue());
		}

		public bool addAll(int index, Collection<Double> c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var d in c) {
				result = true;
				items[index++] = d;
			}
			size += len;
			return result;
		}

		public Double get(int index) {
			return getItem(index);
		}

		public int indexOf(Object o) {
			var d = o as Double;
			if (d == null) {
				return -1;
			}
			return indexOf(d.doubleValue());
		}
		
		public int lastIndexOf(Object o) {
			var d = o as Double;
			if (d == null) {
				return -1;
			}
			return lastIndexOf(d.doubleValue());
		}
		
		public Double remove(int index) {
			return removeAt(index);
		}

		public Double set(int index, Double d) {
			if (d == null) {
				throw new NullPointerException("d");
			}
			return setItem(index, d.doubleValue());
		}
		
		// DoubleList
		
		public void add(int index, double d) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			if (size == sizeof(items)) {
				resize(size * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + 1, size - index);
			}
			items[index] = d;
			size++;
		}
		
		public bool addAll(int index, DoubleCollection c) {
			if (index < 0 || index > size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			int len = c.size();
			if (sizeof(items) < size + len) {
				resize((size + len) * 2);
			}
			if (index < size) {
				System.arraycopy(items, index, items, index + len, size - index);
			}
			var result = false;
			foreach (var d in c) {
				result = true;
				items[index++] = d;
			}
			size += len;
			return result;
		}
		
		public int indexOf(double d) {
			for (int index = 0; index < size; index++) {
				if (items[index] == d) {
					return index;
				}
			}
			return -1;
		}
		
		public int lastIndexOf(double d) {
			for (int index = size - 1; index >= 0; index++) {
				if (items[index] == d) {
					return index;
				}
			}
			return -1;
		}
		
		public double removeAt(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			size--;
			var result = items[index];
			if (index < size) {
				System.arraycopy(items, index + 1, items, index, size - index);
			}
			return result;
		}
		
		public DoubleList subList(int fromIndex, int toIndex) {
			throw new UnsupportedOperationException();
		}

		public double getItem(int index) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			return items[index];
		}
		
		public double setItem(int index, double value) {
			if (index < 0 || index >= size) {
				throw new IndexOutOfBoundsException("index = " + index);
			}
			var result = items[index];
			items[index] = value;
			return result;
		}
		
		private void resize(int newSize) {
			var length = sizeof(items);
			if (length == 0) {
				items = new double[(newSize == 0) ? 4 : newSize];
			} else {
				var t = new double[newSize];
				System.arraycopy(items, 0, t, 0, length);
				items = t;
			}
		}
	}
	
	// ========================================================================
	// Sets
	// ========================================================================

	class InternalSet<E> : CollectionBase<E>, Set<E> {
		private HashSet<E> set;
		
		InternalSet(EqualityTester<E> tester) {
			this.set = new HashSet<E>(tester, 7);
		}
		
		public bool add(E e) {
			return set.add(e);
		}
		
		public void clear() {
			set = new HashSet<E>(set.equalityTester(), 7);
		}
		
		public bool contains(Object o) {
			#pragma warning disable 270
			return set.contains((E)o);
			#pragma warning restore
		}
		
		public bool isEmpty() {
			return set.count() == 0;
		}
		
		public Iterator<E> iterator() {
			return set.iterator();
		}
		
		public bool remove(Object o) {
			#pragma warning disable 270
			return set.remove((E)o);
			#pragma warning restore
		}
		
		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new HashSet<E>(set.equalityTester(), 7);
			foreach (var o in c) {
				if (contains(o)) {
					#pragma warning disable 270
					newSet.add((E)o);
					#pragma warning restore
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public int size() {
			return set.count();
		}
	}

	// ========================================================================
	
	class InternalIntSet : CollectionBase<Integer>, IntSet {
		private IntHashSet set;
		
		InternalIntSet() {
			this.set = new IntHashSet(7);
		}

		// Collection
		
		public bool add(Integer e) {
			return add(e.intValue());
		}
		
		public void clear() {
			set = new IntHashSet(7);
		}
		
		public bool contains(Object o) {
			var i = o as Integer;
			if (i == null) {
				return false;
			}
			return contains(i.intValue());
		}
		
		public bool isEmpty() {
			return set.count() == 0;
		}
		
		public bool remove(Object o) {
			var i = o as Integer;
			if (i == null) {
				return false;
			}
			return removeItem(i.intValue());
		}
		
		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new IntHashSet(7);
			foreach (var o in c) {
				var i = o as Integer;
				if (i != null && set.contains(i)) {
					newSet.add(i);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public int size() {
			return set.count();
		}
		
		// IntCollection
		
		public bool add(int i) {
			return set.add(i);
		}
		
		public bool addAll(IntCollection c) {
			var result = false;
			foreach (var i in c) {
				result = true;
				add(i);
			}
			return result;
		}
		
		public bool contains(int i) {
			return set.contains(i);
		}
		
		public bool containsAll(IntCollection c) {
			foreach (var i in c) {
				if (!contains(i)) {
					return false;
				}
			}
			return true;
		}
		
		public IntIterator iterator() {
			return set.iterator();
		}
		
		public bool removeItem(int i) {
			return set.remove(i);
		}
		
		public bool removeAll(IntCollection c) {
			var result = false;
			foreach (var i in c) {
				if (removeItem(i)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(IntCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new IntHashSet(7);
			foreach (var i in c) {
				if (set.contains(i)) {
					newSet.add(i);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public int[] toArray(int[] a) {
			int len = size();
			int[] result = (sizeof(a) < len) ? new int[len] : a;
			int index = 0;
			foreach (var i in this) {
				result[index++] = i;
			}
			return result;
		}
	}

	// ========================================================================
	
	class InternalLongSet : CollectionBase<Long>, LongSet {
		private LongHashSet set;
		
		InternalLongSet() {
			this.set = new LongHashSet(7);
		}

		// Collection
		
		public bool add(Long e) {
			return add(e.longValue());
		}
		
		public void clear() {
			set = new LongHashSet(7);
		}
		
		public bool contains(Object o) {
			var l = o as Long;
			if (l == null) {
				return false;
			}
			return contains(l.longValue());
		}
		
		public bool isEmpty() {
			return set.count() == 0;
		}
		
		public bool remove(Object o) {
			var l = o as Long;
			if (l == null) {
				return false;
			}
			return removeItem(l.longValue());
		}
		
		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new LongHashSet(7);
			foreach (var o in c) {
				var l = o as Long;
				if (l != null && set.contains(l)) {
					newSet.add(l);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public int size() {
			return set.count();
		}
		
		// LongCollection
		
		public bool add(long l) {
			return set.add(l);
		}
		
		public bool addAll(LongCollection c) {
			var result = false;
			foreach (var i in c) {
				result = true;
				add(i);
			}
			return result;
		}
		
		public bool contains(long l) {
			return set.contains(l);
		}
		
		public bool containsAll(LongCollection c) {
			foreach (var l in c) {
				if (!contains(l)) {
					return false;
				}
			}
			return true;
		}
		
		public LongIterator iterator() {
			return set.iterator();
		}
		
		public bool removeItem(long l) {
			return set.remove(l);
		}
		
		public bool removeAll(LongCollection c) {
			var result = false;
			foreach (var l in c) {
				if (removeItem(l)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(LongCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new LongHashSet(7);
			foreach (var l in c) {
				if (set.contains(l)) {
					newSet.add(l);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public long[] toArray(long[] a) {
			int len = size();
			long[] result = (sizeof(a) < len) ? new long[len] : a;
			int index = 0;
			foreach (var i in this) {
				result[index++] = i;
			}
			return result;
		}
	}

	// ========================================================================
	
	class InternalFloatSet : CollectionBase<Float>, FloatSet {
		private FloatHashSet set;
		
		InternalFloatSet() {
			this.set = new FloatHashSet(7);
		}

		// Collection
		
		public bool add(Float e) {
			return add(e.floatValue());
		}
		
		public void clear() {
			set = new FloatHashSet(7);
		}
		
		public bool contains(Object o) {
			var f = o as Float;
			if (f == null) {
				return false;
			}
			return contains(f.floatValue());
		}
		
		public bool isEmpty() {
			return set.count() == 0;
		}
		
		public bool remove(Object o) {
			var f = o as Float;
			if (f == null) {
				return false;
			}
			return removeItem(f.floatValue());
		}
		
		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new FloatHashSet(7);
			foreach (var o in c) {
				var f = o as Float;
				if (f != null && set.contains(f)) {
					newSet.add(f);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public int size() {
			return set.count();
		}
		
		// FloatCollection
		
		public bool add(float f) {
			return set.add(f);
		}
		
		public bool addAll(FloatCollection c) {
			var result = false;
			foreach (var f in c) {
				result = true;
				add(f);
			}
			return result;
		}
		
		public bool contains(float f) {
			return set.contains(f);
		}
		
		public bool containsAll(FloatCollection c) {
			foreach (var f in c) {
				if (!contains(f)) {
					return false;
				}
			}
			return true;
		}
		
		public FloatIterator iterator() {
			return set.iterator();
		}
		
		public bool removeItem(float f) {
			return set.remove(f);
		}
		
		public bool removeAll(FloatCollection c) {
			var result = false;
			foreach (var f in c) {
				if (removeItem(f)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(FloatCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new FloatHashSet(7);
			foreach (var f in c) {
				if (set.contains(f)) {
					newSet.add(f);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public float[] toArray(float[] a) {
			int len = size();
			float[] result = (sizeof(a) < len) ? new float[len] : a;
			int index = 0;
			foreach (var f in this) {
				result[index++] = f;
			}
			return result;
		}
	}

	// ========================================================================
	
	class InternalDoubleSet : CollectionBase<Double>, DoubleSet {
		private DoubleHashSet set;
		
		InternalDoubleSet() {
			this.set = new DoubleHashSet(7);
		}

		// Collection
		
		public bool add(Double e) {
			return add(e.doubleValue());
		}
		
		public void clear() {
			set = new DoubleHashSet(7);
		}
		
		public bool contains(Object o) {
			var d = o as Double;
			if (d == null) {
				return false;
			}
			return contains(d.doubleValue());
		}
		
		public bool isEmpty() {
			return set.count() == 0;
		}
		
		public bool remove(Object o) {
			var d = o as Double;
			if (d == null) {
				return false;
			}
			return removeItem(d.doubleValue());
		}
		
		public bool retainAll(Collection<?> c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new DoubleHashSet(7);
			foreach (var o in c) {
				var d = o as Double;
				if (d != null && set.contains(d)) {
					newSet.add(d);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public int size() {
			return set.count();
		}
		
		// DoubleCollection
		
		public bool add(double d) {
			return set.add(d);
		}
		
		public bool addAll(DoubleCollection c) {
			var result = false;
			foreach (var d in c) {
				result = true;
				add(d);
			}
			return result;
		}
		
		public bool contains(double d) {
			return set.contains(d);
		}
		
		public bool containsAll(DoubleCollection c) {
			foreach (var d in c) {
				if (!contains(d)) {
					return false;
				}
			}
			return true;
		}
		
		public DoubleIterator iterator() {
			return set.iterator();
		}
		
		public bool removeItem(double d) {
			return set.remove(d);
		}
		
		public bool removeAll(DoubleCollection c) {
			var result = false;
			foreach (var d in c) {
				if (removeItem(d)) {
					result = true;
				}
			}
			return result;
		}
		
		public bool retainAll(DoubleCollection c) {
			if (this.isEmpty() || c.isEmpty()) {
				return false;
			}
			var newSet = new DoubleHashSet(7);
			foreach (var d in c) {
				if (set.contains(d)) {
					newSet.add(d);
				}
			}
			var result = set.count() != newSet.count();
			set = newSet;
			return result;
		}
		
		public double[] toArray(double[] a) {
			int len = size();
			double[] result = (sizeof(a) < len) ? new double[len] : a;
			int index = 0;
			foreach (var i in this) {
				result[index++] = i;
			}
			return result;
		}
	}
}
