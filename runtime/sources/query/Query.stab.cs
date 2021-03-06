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
using java.util;
using stab.lang;

namespace stab.query {

	public static class Query {

		///////////////////////////////////////////////////////////////////////////////////////////
		// aggregate
		//
		
		public static TSource aggregate<TSource>(this Iterable<TSource> source, FunctionTTT<TSource, TSource, TSource> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var it = source.iterator();
			var result = it.next();
			while (it.hasNext()) {
				result = accumulator.invoke(result, it.next());
			}
			return result;
		}
		
		public static int aggregate(this IntIterable source, FunctionIntIntInt accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var it = source.iterator();
			var result = it.nextInt();
			while (it.hasNext()) {
				result = accumulator.invoke(result, it.nextInt());
			}
			return result;
		}
		
		public static long aggregate(this LongIterable source, FunctionLongLongLong accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var it = source.iterator();
			var result = it.nextLong();
			while (it.hasNext()) {
				result = accumulator.invoke(result, it.nextLong());
			}
			return result;
		}
		
		public static float aggregate(this FloatIterable source, FunctionFloatFloatFloat accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var it = source.iterator();
			var result = it.nextFloat();
			while (it.hasNext()) {
				result = accumulator.invoke(result, it.nextFloat());
			}
			return result;
		}
		
		public static double aggregate(this DoubleIterable source, FunctionDoubleDoubleDouble accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var it = source.iterator();
			var result = it.nextDouble();
			while (it.hasNext()) {
				result = accumulator.invoke(result, it.nextDouble());
			}
			return result;
		}

		public static TAccumulate aggregate<TSource, TAccumulate>(this Iterable<TSource> source, TAccumulate seed,
				FunctionTTT<TAccumulate, TSource, TAccumulate> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static int aggregate<TSource>(this Iterable<TSource> source, int seed, FunctionIntTInt<TSource> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static TAccumulate aggregate<TAccumulate>(this IntIterable source, TAccumulate seed,
				FunctionTIntT<TAccumulate, TAccumulate> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static long aggregate<TSource>(this Iterable<TSource> source, long seed, FunctionLongTLong<TSource> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static TAccumulate aggregate<TAccumulate>(this LongIterable source, TAccumulate seed,
				FunctionTLongT<TAccumulate, TAccumulate> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static float aggregate<TSource>(this Iterable<TSource> source, float seed, FunctionFloatTFloat<TSource> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static TAccumulate aggregate<TAccumulate>(this FloatIterable source, TAccumulate seed,
				FunctionTFloatT<TAccumulate, TAccumulate> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static double aggregate<TSource>(this Iterable<TSource> source, double seed, FunctionDoubleTDouble<TSource> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		public static TAccumulate aggregate<TAccumulate>(this DoubleIterable source, TAccumulate seed,
				FunctionTDoubleT<TAccumulate, TAccumulate> accumulator) {
			if (source == null) throw new NullPointerException("source");
			if (accumulator == null) throw new NullPointerException("accumulator");

			var result = seed;
			foreach (var s in source) {
				result = accumulator.invoke(result, s);
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// all
		//
		
		public static bool all<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (!predicate.invoke(s)) {
					return false;
				}
			}
			return true;
		}
		
		public static bool all(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (!predicate.invoke(s)) {
					return false;
				}
			}
			return true;
		}
		
		public static bool all(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (!predicate.invoke(s)) {
					return false;
				}
			}
			return true;
		}
		
		public static bool all(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (!predicate.invoke(s)) {
					return false;
				}
			}
			return true;
		}
		
		public static bool all(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (!predicate.invoke(s)) {
					return false;
				}
			}
			return true;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// any
		//
		
		public static bool any<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof Collection<?>) {
				return !((Collection<TSource>)source).isEmpty();
			}
			return source.iterator().hasNext();
		}

		public static bool any<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return true;
				}
			}
			return false;
		}

		public static bool any(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return true;
				}
			}
			return false;
		}

		public static bool any(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return true;
				}
			}
			return false;
		}
		
		public static bool any(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return true;
				}
			}
			return false;
		}

		public static bool any(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return true;
				}
			}
			return false;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////
		// asIterable
		//

		public static Iterable<TSource> asIterable<TSource>(TSource[] array) {
			if (array == null) throw new NullPointerException("array");
		
			foreach (var s in array) {
				yield return s;
			}
		}

		public static BooleanIterable asIterable(bool[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static ByteIterable asIterable(byte[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static CharIterable asIterable(char[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static CharIterable asIterable(CharSequence s) {
			if (s == null) throw new NullPointerException("s");
			
			for (int i = 0; i < s.length(); i++) {
				yield return s[i];
			}
		}

		public static ShortIterable asIterable(short[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static IntIterable asIterable(int[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static LongIterable asIterable(long[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static FloatIterable asIterable(float[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		public static DoubleIterable asIterable(double[] array) {
			if (array == null) throw new NullPointerException("array");
			
			foreach (var s in array) {
				yield return s;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// average
		//
		
		public static double average(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var sum = 0L;
			var count = 0L;
			foreach (var s in source) {
				sum += s;
				count++;
			}
			if (count == 0L) {
				throw new NoSuchElementException();
			}
			return ((double)sum) / ((double)count);
		}
		
		public static double average<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> selector) {
			return source.select(selector).average();
		}
		
		public static double average(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var sum = 0L;
			var count = 0L;
			foreach (var s in source) {
				sum += s;
				count++;
			}
			if (count == 0L) {
				throw new NoSuchElementException();
			}
			return ((double)sum) / ((double)count);
		}
		
		public static double average<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> selector) {
			return source.select(selector).average();
		}
		
		public static double average(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var sum = 0d;
			var count = 0L;
			foreach (var s in source) {
				sum += s;
				count++;
			}
			if (count == 0L) {
				throw new NoSuchElementException();
			}
			return sum / ((double)count);
		}
		
		public static double average<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> selector) {
			return source.select(selector).average();
		}
		
		public static double average(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var sum = 0d;
			var count = 0L;
			foreach (var s in source) {
				sum += s;
				count++;
			}
			if (count == 0L) {
				throw new NoSuchElementException();
			}
			return sum / ((double)count);
		}
		
		public static double average<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> selector) {
			return source.select(selector).average();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// cast
		//
		
		public static Iterable<TResult> cast<TResult>(this Iterable<?> source, Class<TResult> targetClass) {
			if (source == null) throw new NullPointerException("source");
			if (targetClass == null) throw new NullPointerException("targetClass");
			
			var it = source.iterator();
			while (it.hasNext()) {
				yield return targetClass.cast(it.next());
			}
		}
		
		public static BooleanIterable castToBoolean(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as BooleanIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Boolean)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static ByteIterable castToByte(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as ByteIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Byte)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static CharIterable castToChar(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as CharIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Character)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static ShortIterable castToShort(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as ShortIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Short)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static IntIterable castToInt(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as IntIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Integer)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static LongIterable castToLong(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as LongIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Long)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static FloatIterable castToFloat(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as FloatIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Float)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		public static DoubleIterable castToDouble(this Iterable<?> source) {
			if (source == null) throw new NullPointerException("source");

			var src = source as DoubleIterable;
			if (src == null) {
				var it = source.iterator();
				while (it.hasNext()) {
					yield return (Double)it.next();
				}
			} else {
				foreach (var s in src) {
					yield return s;
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// concat
		//
		
		public static Iterable<TSource> concat<TSource>(this Iterable<TSource> first, Iterable<TSource> second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
			
			foreach (var s in first) {
				yield return s;
			}
			foreach (var s in second) {
				yield return s;
			}
		}
		
		public static IntIterable concat(this IntIterable first, IntIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
			
			foreach (var s in first) {
				yield return s;
			}
			foreach (var s in second) {
				yield return s;
			}
		}
		
		public static LongIterable concat(this LongIterable first, LongIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
			
			foreach (var s in first) {
				yield return s;
			}
			foreach (var s in second) {
				yield return s;
			}
		}
		
		public static FloatIterable concat(this FloatIterable first, FloatIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
			
			foreach (var s in first) {
				yield return s;
			}
			foreach (var s in second) {
				yield return s;
			}
		}
		
		public static DoubleIterable concat(this DoubleIterable first, DoubleIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
			
			foreach (var s in first) {
				yield return s;
			}
			foreach (var s in second) {
				yield return s;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// contains
		//

		public static bool contains<TSource>(this Iterable<TSource> source, TSource item) {
			return contains(source, item, null);
		}

		public static bool contains<TSource>(this Iterable<TSource> source, TSource item, EqualityTester<TSource> tester) {
			if (source == null) throw new NullPointerException("source");
			if (tester == null) {
				tester = Query.getDefaultEqualityTester();
			}
			
			foreach (var s in source) {
				if (tester.equals(s, item)) {
					return true;
				}
			}
			return false;
		}

		public static bool contains(this IntIterable source, int item) {
			if (source == null) throw new NullPointerException("source");

			foreach (var s in source) {
				if (item == s) {
					return true;
				}
			}
			return false;
		}

		public static bool contains(this LongIterable source, long item) {
			if (source == null) throw new NullPointerException("source");

			foreach (var s in source) {
				if (item == s) {
					return true;
				}
			}
			return false;
		}

		public static bool contains(this FloatIterable source, float item) {
			if (source == null) throw new NullPointerException("source");

			foreach (var s in source) {
				if (item == s) {
					return true;
				}
			}
			return false;
		}

		public static bool contains(this DoubleIterable source, double item) {
			if (source == null) throw new NullPointerException("source");

			foreach (var s in source) {
				if (item == s) {
					return true;
				}
			}
			return false;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// count
		//
		
		public static int count<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof Collection<?>) {
				return ((Collection<TSource>)source).size();
			}
			
			int result = 0;
			var it = source.iterator();
			while (it.hasNext()) {
				it.next();
				result++;
			}
			return result;
		}
		
		public static int count<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			int result = 0;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static int count(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			int result = 0;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static int count(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			int result = 0;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static int count(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			int result = 0;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static int count(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			int result = 0;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// defaultIfEmpty
		//
		
		public static Iterable<TSource> defaultIfEmpty<TSource>(this Iterable<TSource> source) {
			return source.defaultIfEmpty(null);
		}

		public static Iterable<TSource> defaultIfEmpty<TSource>(this Iterable<TSource> source, TSource defaultItem) {
			if (source == null) throw new NullPointerException("source");
			
			var it = source.iterator();
			if (!it.hasNext()) {
				yield return defaultItem;
			} else {
				do {
					yield return it.next();
				} while (it.hasNext());
			}
		}

		public static IntIterable defaultIfEmpty(this IntIterable source, int defaultItem) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				yield return defaultItem;
			} else {
				do {
					yield return it.nextInt();
				} while (it.hasNext());
			}
		}

		public static LongIterable defaultIfEmpty(this LongIterable source, long defaultItem) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				yield return defaultItem;
			} else {
				do {
					yield return it.nextLong();
				} while (it.hasNext());
			}
		}

		public static FloatIterable defaultIfEmpty(this FloatIterable source, float defaultItem) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				yield return defaultItem;
			} else {
				do {
					yield return it.nextFloat();
				} while (it.hasNext());
			}
		}

		public static DoubleIterable defaultIfEmpty(this DoubleIterable source, double defaultItem) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				yield return defaultItem;
			} else {
				do {
					yield return it.nextDouble();
				} while (it.hasNext());
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// distinct
		//
		
		public static Iterable<TSource> distinct<TSource>(this Iterable<TSource> source) {
			return distinct(source, null);
		}

		public static Iterable<TSource> distinct<TSource>(this Iterable<TSource> source, EqualityTester<TSource> tester) {
			if (source == null) throw new NullPointerException("source");
		
			var set = new HashSet<TSource>(tester, 7);
			foreach (var s in source) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static IntIterable distinct(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var set = new IntHashSet(7);
			foreach (var s in source) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static LongIterable distinct(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var set = new LongHashSet(7);
			foreach (var s in source) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static FloatIterable distinct(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var set = new FloatHashSet(7);
			foreach (var s in source) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static DoubleIterable distinct(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");
			
			var set = new DoubleHashSet(7);
			foreach (var s in source) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// elementAt
		
		public static TSource elementAt<TSource>(this Iterable<TSource> source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof  List<?>) {
				return ((List<TSource>)source)[index];
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
				throw new IndexOutOfBoundsException("index: " + i);
			}
		}

		public static int elementAt(this IntIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof IntList) {
				return ((IntList)source).getItem(index);
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
				throw new IndexOutOfBoundsException("index: " + i);
			}
		}

		public static long elementAt(this LongIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof LongList) {
				return ((LongList)source).getItem(index);
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
				throw new IndexOutOfBoundsException("index: " + i);
			}
		}

		public static float elementAt(this FloatIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof FloatList) {
				return ((FloatList)source).getItem(index);
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
				throw new IndexOutOfBoundsException("index: " + i);
			}
		}

		public static double elementAt(this DoubleIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof DoubleList) {
				return ((DoubleList)source).getItem(index);
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
				throw new IndexOutOfBoundsException("index: " + i);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// elementAtOrDefault

		public static TSource elementAtOrDefault<TSource>(this Iterable<TSource> source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof List<?>) {
				var list = (List<TSource>)source;
				if (index < list.size()) {
					return list[index];
				}
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
			}
			return null;
		}

		public static int elementAtOrDefault(this IntIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof IntList) {
				var list = (IntList)source;
				if (index < list.size()) {
					return list.getItem(index);
				}
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
			}
			return 0;
		}

		public static long elementAtOrDefault(this LongIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof LongList) {
				var list = (LongList)source;
				if (index < list.size()) {
					return list.getItem(index);
				}
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
			}
			return 0L;
		}

		public static float elementAtOrDefault(this FloatIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof FloatList) {
				var list = (FloatList)source;
				if (index < list.size()) {
					return list.getItem(index);
				}
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
			}
			return 0f;
		}

		public static double elementAtOrDefault(this DoubleIterable source, int index) {
			if (source == null) throw new NullPointerException("source");
			if (index < 0) throw new IllegalArgumentException("index");

			if (source instanceof DoubleList) {
				var list = (DoubleList)source;
				if (index < list.size()) {
					return list.getItem(index);
				}
			} else {
				int i = 0;
				foreach (var s in source) {
					if (i == index) {
						return s;
					}
					i++;
				}
			}
			return 0d;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// empty
	
		#pragma warning disable 252, 270 // Ignore warnings about raw generic types

		public static Iterable<TResult> empty<TResult>() {
			return (Iterable<TResult>)EmptyIterable.INSTANCE;
		}

		private class EmptyIterable : Iterable {
			static Iterable INSTANCE = new EmptyIterable();
			
			public Iterator iterator() {
				return EmptyIterator.INSTANCE;
			}			
		}
		
		private class EmptyIterator : Iterator {
			static Iterator INSTANCE = new EmptyIterator();
		
			public bool hasNext() {
				return false;
			}
			
			public Object next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		#pragma warning restore
		
		public static BooleanIterable emptyBoolean() {
			return EmptyBooleanIterable.INSTANCE;
		}
		
		private class EmptyBooleanIterable : BooleanIterable {
			static BooleanIterable INSTANCE = new EmptyBooleanIterable();
			
			public BooleanIterator iterator() {
				return EmptyBooleanIterator.INSTANCE;
			}
		}
		
		private class EmptyBooleanIterator : BooleanIterator {
			static BooleanIterator INSTANCE = new EmptyBooleanIterator();

			public bool nextBoolean() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Boolean next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static ByteIterable emptyByte() {
			return EmptyByteIterable.INSTANCE;
		}
		
		private class EmptyByteIterable : ByteIterable {
			static ByteIterable INSTANCE = new EmptyByteIterable();
			
			public ByteIterator iterator() {
				return EmptyByteIterator.INSTANCE;
			}
		}
		
		private class EmptyByteIterator : ByteIterator {
			static ByteIterator INSTANCE = new EmptyByteIterator();

			public byte nextByte() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Byte next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static CharIterable emptyChar() {
			return EmptyCharIterable.INSTANCE;
		}
		
		private class EmptyCharIterable : CharIterable {
			static CharIterable INSTANCE = new EmptyCharIterable();
			
			public CharIterator iterator() {
				return EmptyCharIterator.INSTANCE;
			}
		}
		
		private class EmptyCharIterator : CharIterator {
			static CharIterator INSTANCE = new EmptyCharIterator();

			public char nextChar() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Character next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static ShortIterable emptyShort() {
			return EmptyShortIterable.INSTANCE;
		}
		
		private class EmptyShortIterable : ShortIterable {
			static ShortIterable INSTANCE = new EmptyShortIterable();
			
			public ShortIterator iterator() {
				return EmptyShortIterator.INSTANCE;
			}
		}
		
		private class EmptyShortIterator : ShortIterator {
			static ShortIterator INSTANCE = new EmptyShortIterator();

			public short nextShort() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Short next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static IntIterable emptyInt() {
			return EmptyIntIterable.INSTANCE;
		}
		
		private class EmptyIntIterable : IntIterable {
			static IntIterable INSTANCE = new EmptyIntIterable();
			
			public IntIterator iterator() {
				return EmptyIntIterator.INSTANCE;
			}
		}
		
		private class EmptyIntIterator : IntIterator {
			static IntIterator INSTANCE = new EmptyIntIterator();

			public int nextInt() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Integer next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static LongIterable emptyLong() {
			return EmptyLongIterable.INSTANCE;
		}
		
		private class EmptyLongIterable : LongIterable {
			static LongIterable INSTANCE = new EmptyLongIterable();
			
			public LongIterator iterator() {
				return EmptyLongIterator.INSTANCE;
			}
		}
		
		private class EmptyLongIterator : LongIterator {
			static LongIterator INSTANCE = new EmptyLongIterator();

			public long nextLong() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Long next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static FloatIterable emptyFloat() {
			return EmptyFloatIterable.INSTANCE;
		}
		
		private class EmptyFloatIterable : FloatIterable {
			static FloatIterable INSTANCE = new EmptyFloatIterable();
			
			public FloatIterator iterator() {
				return EmptyFloatIterator.INSTANCE;
			}
		}
		
		private class EmptyFloatIterator : FloatIterator {
			static FloatIterator INSTANCE = new EmptyFloatIterator();

			public float nextFloat() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Float next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}
		
		public static DoubleIterable emptyDouble() {
			return EmptyDoubleIterable.INSTANCE;
		}
		
		private class EmptyDoubleIterable : DoubleIterable {
			static DoubleIterable INSTANCE = new EmptyDoubleIterable();
			
			public DoubleIterator iterator() {
				return EmptyDoubleIterator.INSTANCE;
			}
		}
		
		private class EmptyDoubleIterator : DoubleIterator {
			static DoubleIterator INSTANCE = new EmptyDoubleIterator();

			public double nextDouble() {
				throw new NoSuchElementException();
			}
			
			public bool hasNext() {
				return false;
			}
			
			public Double next() {
				throw new NoSuchElementException();
			}
			
			public void remove() {
				throw new UnsupportedOperationException();
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// except
		//

		public static Iterable<TSource> except<TSource>(this Iterable<TSource> first, Iterable<TSource> second) {
			return except(first, second, null);
		}
		
		public static Iterable<TSource> except<TSource>(this Iterable<TSource> first, Iterable<TSource> second, EqualityTester<TSource> tester) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new HashSet<TSource>(tester, 7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static IntIterable except(this IntIterable first, IntIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new IntHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static LongIterable except(this LongIterable first, LongIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new LongHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static FloatIterable except(this FloatIterable first, FloatIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new FloatHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static DoubleIterable except(this DoubleIterable first, DoubleIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new DoubleHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// first
		//
		
		public static TSource first<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");
		
			if (source instanceof List<?>) {
				var l = (List<TSource>)source;
				if (l.size() > 0) {
					return l[0];
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.next();
				}
			}
			throw new NoSuchElementException();
		}

		public static int first(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof IntList) {
				var list = (IntList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextInt();
				}
			}
			throw new NoSuchElementException();
		}

		public static long first(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof LongList) {
				var list = (LongList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextLong();
				}
			}
			throw new NoSuchElementException();
		}

		public static float first(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof FloatList) {
				var list = (FloatList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextFloat();
				}
			}
			throw new NoSuchElementException();
		}

		public static double first(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof DoubleList) {
				var list = (DoubleList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextDouble();
				}
			}
			throw new NoSuchElementException();
		}

		public static TSource first<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			throw new NoSuchElementException();
		}

		public static int first(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			throw new NoSuchElementException();
		}

		public static long first(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			throw new NoSuchElementException();
		}

		public static float first(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			throw new NoSuchElementException();
		}

		public static double first(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			throw new NoSuchElementException();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// firstOrDefault
		//
		
		public static TSource firstOrDefault<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");
		
			if (source instanceof List<?>) {
				var l = (List<TSource>)source;
				if (l.size() > 0) {
					return l[0];
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.next();
				}
			}
			return null;
		}

		public static int firstOrDefault(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof IntList) {
				var list = (IntList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextInt();
				}
			}
			return 0;
		}

		public static long firstOrDefault(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof LongList) {
				var list = (LongList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextLong();
				}
			}
			return 0L;
		}

		public static float firstOrDefault(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof FloatList) {
				var list = (FloatList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextFloat();
				}
			}
			return 0f;
		}

		public static double firstOrDefault(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof DoubleList) {
				var list = (DoubleList)source;
				if (list.size() > 0) {
					return list.getItem(0);
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					return it.nextDouble();
				}
			}
			return 0d;
		}

		public static TSource firstOrDefault<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			return null;
		}

		public static int firstOrDefault(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			return 0;
		}

		public static long firstOrDefault(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			return 0;
		}

		public static float firstOrDefault(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			return 0;
		}

		public static double firstOrDefault(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			foreach (var s in source) {
				if (predicate.invoke(s)) {
					return s;
				}
			}
			return 0;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// groupBy
		//
		
		public static Iterable<Grouping<TKey, TSource>> groupBy<TSource, TKey>(this Iterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector) {
			return groupBy(source, keySelector, null);
		}
		
		public static Iterable<Grouping<TKey, TSource>> groupBy<TSource, TKey>(this Iterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector, EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			
			var lookup = new DefaultLookup<TKey, TSource>(tester);
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(s);
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}

		public static Iterable<IntGrouping<TSource>> groupBy<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> keySelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			
			var lookup = new DefaultIntLookup<TSource>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(s);
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}

		public static Iterable<LongGrouping<TSource>> groupBy<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> keySelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			
			var lookup = new DefaultLongLookup<TSource>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(s);
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}

		public static Iterable<FloatGrouping<TSource>> groupBy<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> keySelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			
			var lookup = new DefaultFloatLookup<TSource>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(s);
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}

		public static Iterable<DoubleGrouping<TSource>> groupBy<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> keySelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			
			var lookup = new DefaultDoubleLookup<TSource>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(s);
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}

		public static Iterable<Grouping<TKey, TElement>> groupBy<TSource, TKey, TElement>(this Iterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector, FunctionTT<TSource, TElement> elementSelector) {
			return groupBy(source, keySelector, elementSelector, null);
		}
		
		public static Iterable<Grouping<TKey, TElement>> groupBy<TSource, TKey, TElement>(this Iterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector, FunctionTT<TSource, TElement> elementSelector, EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			if (elementSelector == null) throw new NullPointerException("elementSelector");
			
			var lookup = new DefaultLookup<TKey, TElement>(tester);
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(elementSelector.invoke(s));
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}
		
		public static Iterable<IntGrouping<TElement>> groupBy<TSource, TElement>(this Iterable<TSource> source,
				FunctionTInt<TSource> keySelector, FunctionTT<TSource, TElement> elementSelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			if (elementSelector == null) throw new NullPointerException("elementSelector");
			
			var lookup = new DefaultIntLookup<TElement>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(elementSelector.invoke(s));
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}
		
		public static Iterable<LongGrouping<TElement>> groupBy<TSource, TElement>(this Iterable<TSource> source,
				FunctionTLong<TSource> keySelector, FunctionTT<TSource, TElement> elementSelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			if (elementSelector == null) throw new NullPointerException("elementSelector");
			
			var lookup = new DefaultLongLookup<TElement>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(elementSelector.invoke(s));
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}
		
		public static Iterable<FloatGrouping<TElement>> groupBy<TSource, TElement>(this Iterable<TSource> source,
				FunctionTFloat<TSource> keySelector, FunctionTT<TSource, TElement> elementSelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			if (elementSelector == null) throw new NullPointerException("elementSelector");
			
			var lookup = new DefaultFloatLookup<TElement>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(elementSelector.invoke(s));
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}
		
		public static Iterable<DoubleGrouping<TElement>> groupBy<TSource, TElement>(this Iterable<TSource> source,
				FunctionTDouble<TSource> keySelector, FunctionTT<TSource, TElement> elementSelector) {
			if (source == null) throw new NullPointerException("source");
			if (keySelector == null) throw new NullPointerException("keySelector");
			if (elementSelector == null) throw new NullPointerException("elementSelector");
			
			var lookup = new DefaultDoubleLookup<TElement>();
			foreach (var s in source) {
				lookup.getGrouping(keySelector.invoke(s), true).add(elementSelector.invoke(s));
			}
			foreach (var g in lookup) {
				yield return g;
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// groupJoin
		//
		
		public static Iterable<TResult> groupJoin<TOuter, TInner, TKey, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTT<TOuter, TKey> outerKeySelector, FunctionTT<TInner, TKey> innerKeySelector,
				FunctionTTT<TOuter, Iterable<TInner>, TResult> resultSelector) {
			return groupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}
		
		public static Iterable<TResult> groupJoin<TOuter, TInner, TKey, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTT<TOuter, TKey> outerKeySelector, FunctionTT<TInner, TKey> innerKeySelector,
				FunctionTTT<TOuter, Iterable<TInner>, TResult> resultSelector, EqualityTester<TKey> tester) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");

			var lookup = new DefaultLookup<TKey, TInner>(tester);
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				yield return resultSelector.invoke(o, lookup[outerKeySelector.invoke(o)]);
			}
		}
		
		public static Iterable<TResult> groupJoin<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTInt<TOuter> outerKeySelector, FunctionTInt<TInner> innerKeySelector,
				FunctionTTT<TOuter, Iterable<TInner>, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");

			var lookup = new DefaultIntLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				yield return resultSelector.invoke(o, lookup[outerKeySelector.invoke(o)]);
			}
		}
		
		public static Iterable<TResult> groupJoin<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTLong<TOuter> outerKeySelector, FunctionTLong<TInner> innerKeySelector,
				FunctionTTT<TOuter, Iterable<TInner>, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");

			var lookup = new DefaultLongLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				yield return resultSelector.invoke(o, lookup[outerKeySelector.invoke(o)]);
			}
		}
		
		public static Iterable<TResult> groupJoin<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTFloat<TOuter> outerKeySelector, FunctionTFloat<TInner> innerKeySelector,
				FunctionTTT<TOuter, Iterable<TInner>, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");

			var lookup = new DefaultFloatLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				yield return resultSelector.invoke(o, lookup[outerKeySelector.invoke(o)]);
			}
		}
		
		public static Iterable<TResult> groupJoin<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTDouble<TOuter> outerKeySelector, FunctionTDouble<TInner> innerKeySelector,
				FunctionTTT<TOuter, Iterable<TInner>, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");

			var lookup = new DefaultDoubleLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				yield return resultSelector.invoke(o, lookup[outerKeySelector.invoke(o)]);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// intersect
		//

		public static Iterable<TSource> intersect<TSource>(this Iterable<TSource> first, Iterable<TSource> second) {
			return intersect(first, second, null);
		}
		
		public static Iterable<TSource> intersect<TSource>(this Iterable<TSource> first, Iterable<TSource> second,
				EqualityTester<TSource> tester) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new HashSet<TSource>(tester, 7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.remove(s)) {
					yield return s;
				}
			}
		}

		public static IntIterable intersect(this IntIterable first, IntIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new IntHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.remove(s)) {
					yield return s;
				}
			}
		}

		public static LongIterable intersect(this LongIterable first, LongIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new LongHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.remove(s)) {
					yield return s;
				}
			}
		}

		public static FloatIterable intersect(this FloatIterable first, FloatIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new FloatHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.remove(s)) {
					yield return s;
				}
			}
		}

		public static DoubleIterable intersect(this DoubleIterable first, DoubleIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new DoubleHashSet(7);
			foreach (var s in second) {
				set.add(s);
			}
			foreach (var s in first) {
				if (set.remove(s)) {
					yield return s;
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// join
		//
		
		public static Iterable<TResult> join<TOuter, TInner, TKey, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTT<TOuter, TKey> outerKeySelector, FunctionTT<TInner, TKey> innerKeySelector,
				FunctionTTT<TOuter, TInner, TResult> resultSelector) {
			return join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
		}
		
		public static Iterable<TResult> join<TOuter, TInner, TKey, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTT<TOuter, TKey> outerKeySelector, FunctionTT<TInner, TKey> innerKeySelector,
				FunctionTTT<TOuter, TInner, TResult> resultSelector, EqualityTester<TKey> tester) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");
			
			var lookup = new DefaultLookup<TKey, TInner>(tester);
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				var l = lookup.getGrouping(outerKeySelector.invoke(o), false);
				if (l != null) {
					foreach (var i in l) {
						yield return resultSelector.invoke(o, i);
					}
				}
			}
		}
		
		public static Iterable<TResult> join<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTInt<TOuter> outerKeySelector, FunctionTInt<TInner> innerKeySelector,
				FunctionTTT<TOuter, TInner, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");
			
			var lookup = new DefaultIntLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				var l = lookup.getGrouping(outerKeySelector.invoke(o), false);
				if (l != null) {
					foreach (var i in l) {
						yield return resultSelector.invoke(o, i);
					}
				}
			}
		}
		
		public static Iterable<TResult> join<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTLong<TOuter> outerKeySelector, FunctionTLong<TInner> innerKeySelector,
				FunctionTTT<TOuter, TInner, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");
			
			var lookup = new DefaultLongLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				var l = lookup.getGrouping(outerKeySelector.invoke(o), false);
				if (l != null) {
					foreach (var i in l) {
						yield return resultSelector.invoke(o, i);
					}
				}
			}
		}
		
		public static Iterable<TResult> join<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTFloat<TOuter> outerKeySelector, FunctionTFloat<TInner> innerKeySelector,
				FunctionTTT<TOuter, TInner, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");
			
			var lookup = new DefaultFloatLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				var l = lookup.getGrouping(outerKeySelector.invoke(o), false);
				if (l != null) {
					foreach (var i in l) {
						yield return resultSelector.invoke(o, i);
					}
				}
			}
		}
		
		public static Iterable<TResult> join<TOuter, TInner, TResult>(this Iterable<TOuter> outer, Iterable<TInner> inner,
				FunctionTDouble<TOuter> outerKeySelector, FunctionTDouble<TInner> innerKeySelector,
				FunctionTTT<TOuter, TInner, TResult> resultSelector) {
			if (outer == null) throw new NullPointerException("outer");
			if (inner == null) throw new NullPointerException("inner");
			if (outerKeySelector == null) throw new NullPointerException("outerKeySelector");
			if (innerKeySelector == null) throw new NullPointerException("innerKeySelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");
			
			var lookup = new DefaultDoubleLookup<TInner>();
			foreach (var i in inner) {
				lookup.getGrouping(innerKeySelector.invoke(i), true).add(i);
			}
			foreach (var o in outer) {
				var l = lookup.getGrouping(outerKeySelector.invoke(o), false);
				if (l != null) {
					foreach (var i in l) {
						yield return resultSelector.invoke(o, i);
					}
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// last
		//
		
		public static TSource last<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");
		
			if (source instanceof List<?>) {
				var l = (List<TSource>)source;
				int count = l.size();
				if (count > 0) {
					return l[count - 1];
				}
				throw new NoSuchElementException();
			} else {
				var it = source.iterator();
				if (!it.hasNext()) {
					throw new NoSuchElementException();
				}
				TSource result;
				do {
					result = it.next();
				} while (it.hasNext());
				return result;
			}
		}

		public static int last(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof IntList) {
				var list = (IntList)source;
				int count = list.size();
				if (count > 0) {
					return list.getItem(count - 1);
				}
				throw new NoSuchElementException();
			} else {
				var it = source.iterator();
				if (!it.hasNext()) {
					throw new NoSuchElementException();
				}
				int result;
				do {
					result = it.next();
				} while (it.hasNext());
				return result;
			}
		}

		public static long last(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof LongList) {
				var list = (LongList)source;
				int count = list.size();
				if (count > 0) {
					return list.getItem(count - 1);
				}
				throw new NoSuchElementException();
			} else {
				var it = source.iterator();
				if (!it.hasNext()) {
					throw new NoSuchElementException();
				}
				long result;
				do {
					result = it.next();
				} while (it.hasNext());
				return result;
			}
		}

		public static float last(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof FloatList) {
				var list = (FloatList)source;
				int count = list.size();
				if (count > 0) {
					return list.getItem(count - 1);
				}
				throw new NoSuchElementException();
			} else {
				var it = source.iterator();
				if (!it.hasNext()) {
					throw new NoSuchElementException();
				}
				float result;
				do {
					result = it.next();
				} while (it.hasNext());
				return result;
			}
		}

		public static double last(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof DoubleList) {
				var list = (DoubleList)source;
				int count = list.size();
				if (count > 0) {
					return list.getItem(count - 1);
				}
				throw new NoSuchElementException();
			} else {
				var it = source.iterator();
				if (!it.hasNext()) {
					throw new NoSuchElementException();
				}
				double result;
				do {
					result = it.next();
				} while (it.hasNext());
				return result;
			}
		}
		
		public static TSource last<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
		
			var it = source.iterator();
			TSource result = null;
			var hasItem = false;
			if (it.hasNext()) {
				do {
					var s = it.next();
					if (predicate.invoke(s)) {
						hasItem = true;
						result = s;
					}
				} while (it.hasNext());
				if (hasItem) {
					return result;
				}
			}
			throw new NoSuchElementException();
		}

		public static int last(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var result = 0;
			var hasItem = false;
			if (it.hasNext()) {
				do {
					var s = it.nextInt();
					if (predicate.invoke(s)) {
						hasItem = true;
						result = s;
					}
				} while (it.hasNext());
				if (hasItem) {
					return result;
				}
			}
			throw new NoSuchElementException();
		}

		public static long last(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var result = 0L;
			var hasItem = false;
			if (it.hasNext()) {
				do {
					var s = it.nextLong();
					if (predicate.invoke(s)) {
						hasItem = true;
						result = s;
					}
				} while (it.hasNext());
				if (hasItem) {
					return result;
				}
			}
			throw new NoSuchElementException();
		}

		public static float last(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var result = 0f;
			var hasItem = false;
			if (it.hasNext()) {
				do {
					var s = it.nextFloat();
					if (predicate.invoke(s)) {
						hasItem = true;
						result = s;
					}
				} while (it.hasNext());
				if (hasItem) {
					return result;
				}
			}
			throw new NoSuchElementException();
		}

		public static double last(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var result = 0d;
			var hasItem = false;
			if (it.hasNext()) {
				do {
					var s = it.nextDouble();
					if (predicate.invoke(s)) {
						hasItem = true;
						result = s;
					}
				} while (it.hasNext());
				if (hasItem) {
					return result;
				}
			}
			throw new NoSuchElementException();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// lastOrDefault
		//
		
		public static TSource lastOrDefault<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");
		
			if (source instanceof List<?>) {
				var l = (List<TSource>)source;
				int count = l.size();
				if (count > 0) {
					return l[count - 1];
				}
				return null;
			} else {
				TSource result = null;
				foreach (var s in source) {
					result = s;
				}
				return result;
			}
		}

		public static int lastOrDefault(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof IntList) {
				var list = (IntList)source;
				int count = list.size();
				if (count > 0) {
					return list[count - 1];
				}
				return 0;
			} else {
				var result = 0;
				foreach (var s in source) {
					result = s;
				}
				return result;
			}
		}

		public static long lastOrDefault(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof LongList) {
				var list = (LongList)source;
				int count = list.size();
				if (count > 0) {
					return list[count - 1];
				}
				return 0L;
			} else {
				var result = 0L;
				foreach (var s in source) {
					result = s;
				}
				return result;
			}
		}

		public static float lastOrDefault(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof FloatList) {
				var list = (FloatList)source;
				int count = list.size();
				if (count > 0) {
					return list[count - 1];
				}
				return 0f;
			} else {
				var result = 0f;
				foreach (var s in source) {
					result = s;
				}
				return result;
			}
		}

		public static double lastOrDefault(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof DoubleList) {
				var list = (DoubleList)source;
				int count = list.size();
				if (count > 0) {
					return list[count - 1];
				}
				return 0d;
			} else {
				var result = 0d;
				foreach (var s in source) {
					result = s;
				}
				return result;
			}
		}

		public static TSource lastOrDefault<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
		
			TSource result = null;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result = s;
				}
			}
			return result;
		}

		public static int lastOrDefault(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");

			var result = 0;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result = s;
				}
			}
			return result;
		}

		public static long lastOrDefault(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");

			var result = 0L;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result = s;
				}
			}
			return result;
		}

		public static float lastOrDefault(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");

			var result = 0f;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result = s;
				}
			}
			return result;
		}

		public static double lastOrDefault(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");

			var result = 0d;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result = s;
				}
			}
			return result;
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// longCount
		//
		
		public static long longCount<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");

			var result = 0L;
			var it = source.iterator();
			while (it.hasNext()) {
				it.next();
				result++;
			}
			return result;
		}
		
		public static long longCount<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var result = 0L;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static long longCount(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var result = 0L;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static long longCount(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var result = 0L;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static long longCount(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var result = 0L;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}
		
		public static long longCount(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var result = 0L;
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					result++;
				}
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// max
		//
		
		public static int max(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextInt();
			while (it.hasNext()) {
				var s = it.nextInt();
				if (s > result) {
					result = s;
				}
			}
			return result;
		}
		
		public static int max<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s > result) {
					result = s;
				}
			}
			return result;
		}

		public static long max(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextLong();
			while (it.hasNext()) {
				var s = it.nextLong();
				if (s > result) {
					result = s;
				}
			}
			return result;
		}
		
		public static long max<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s > result) {
					result = s;
				}
			}
			return result;
		}

		public static float max(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextFloat();
			while (it.hasNext()) {
				var s = it.nextFloat();
				if (s > result) {
					result = s;
				}
			}
			return result;
		}
		
		public static float max<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s > result) {
					result = s;
				}
			}
			return result;
		}

		public static double max(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextDouble();
			while (it.hasNext()) {
				var s = it.nextDouble();
				if (s > result) {
					result = s;
				}
			}
			return result;
		}
		
		public static double max<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s > result) {
					result = s;
				}
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// min
		//
		
		public static int min(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextInt();
			while (it.hasNext()) {
				var s = it.nextInt();
				if (s < result) {
					result = s;
				}
			}
			return result;
		}
		
		public static int min<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s < result) {
					result = s;
				}
			}
			return result;
		}

		public static long min(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextLong();
			while (it.hasNext()) {
				var s = it.nextLong();
				if (s < result) {
					result = s;
				}
			}
			return result;
		}
		
		public static long min<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s < result) {
					result = s;
				}
			}
			return result;
		}

		public static float min(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextFloat();
			while (it.hasNext()) {
				var s = it.nextFloat();
				if (s < result) {
					result = s;
				}
			}
			return result;
		}
		
		public static float min<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s < result) {
					result = s;
				}
			}
			return result;
		}

		public static double min(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = it.nextDouble();
			while (it.hasNext()) {
				var s = it.nextDouble();
				if (s < result) {
					result = s;
				}
			}
			return result;
		}
		
		public static double min<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var it = source.iterator();
			if (!it.hasNext()) {
				throw new NoSuchElementException();
			}
			var result = selector.invoke(it.next());
			while (it.hasNext()) {
				var s = selector.invoke(it.next());
				if (s < result) {
					result = s;
				}
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// ofType
		//
		
		public static Iterable<TResult> ofType<TResult>(this Iterable<?> source, Class<TResult> targetClass) {
			if (source == null) throw new NullPointerException("source");
			if (targetClass == null) throw new NullPointerException("targetClass");
			
			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.next();
				if (targetClass.isInstance(s)) {
					#pragma warning disable 270 // Disable unchecked cast warning
					yield return (TResult)s;
					#pragma warning restore
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// orderBy
		//
		
		public static OrderedIterable<TSource> orderBy<TSource, TKey>(this Iterable<TSource> source, FunctionTT<TSource, TKey> keySelector) {
			return new KeyOrderedIterable<TSource, TKey>(source, keySelector, null, false, null);
		}
		
		public static OrderedIterable<TSource> orderBy<TSource, TKey>(this Iterable<TSource> source, FunctionTT<TSource, TKey> keySelector,
				Comparator<TKey> comparator) {
			return new KeyOrderedIterable<TSource, TKey>(source, keySelector, comparator, false, null);
		}
		
		public static OrderedIterable<TSource> orderByDescending<TSource, TKey>(this Iterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector) {
			return new KeyOrderedIterable<TSource, TKey>(source, keySelector, null, true, null);
		}
		
		public static OrderedIterable<TSource> orderByDescending<TSource, TKey>(this Iterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector, Comparator<TKey> comparator) {
			return new KeyOrderedIterable<TSource, TKey>(source, keySelector, comparator, true, null);
		}

		public static OrderedIterable<TSource> orderBy<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> keySelector) {
			return new IntKeyOrderedIterable<TSource>(source, keySelector, false, null);
		}

		public static OrderedIterable<TSource> orderByDescending<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> keySelector) {
			return new IntKeyOrderedIterable<TSource>(source, keySelector, true, null);
		}

		public static OrderedIterable<TSource> orderBy<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> keySelector) {
			return new LongKeyOrderedIterable<TSource>(source, keySelector, false, null);
		}

		public static OrderedIterable<TSource> orderByDescending<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> keySelector) {
			return new LongKeyOrderedIterable<TSource>(source, keySelector, true, null);
		}

		public static OrderedIterable<TSource> orderBy<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> keySelector) {
			return new FloatKeyOrderedIterable<TSource>(source, keySelector, false, null);
		}

		public static OrderedIterable<TSource> orderByDescending<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> keySelector) {
			return new FloatKeyOrderedIterable<TSource>(source, keySelector, true, null);
		}

		public static OrderedIterable<TSource> orderBy<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> keySelector) {
			return new DoubleKeyOrderedIterable<TSource>(source, keySelector, false, null);
		}

		public static OrderedIterable<TSource> orderByDescending<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> keySelector) {
			return new DoubleKeyOrderedIterable<TSource>(source, keySelector, true, null);
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// thenBy
		//
		
		public static OrderedIterable<TSource> thenBy<TSource, TKey>(this OrderedIterable<TSource> source, FunctionTT<TSource, TKey> keySelector) {
			return source.createOrderedIterable(keySelector, null, false);
		}
		
		public static OrderedIterable<TSource> thenBy<TSource, TKey>(this OrderedIterable<TSource> source, FunctionTT<TSource, TKey> keySelector,
				Comparator<TKey> comparator) {
			return source.createOrderedIterable(keySelector, comparator, false);
		}
		
		public static OrderedIterable<TSource> thenByDescending<TSource, TKey>(this OrderedIterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector) {
			return source.createOrderedIterable(keySelector, null, true);
		}
		
		public static OrderedIterable<TSource> thenByDescending<TSource, TKey>(this OrderedIterable<TSource> source,
				FunctionTT<TSource, TKey> keySelector, Comparator<TKey> comparator) {
			return source.createOrderedIterable(keySelector, comparator, true);
		}

		public static OrderedIterable<TSource> thenBy<TSource>(this OrderedIterable<TSource> source, FunctionTInt<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, false);
		}

		public static OrderedIterable<TSource> thenByDescending<TSource>(this OrderedIterable<TSource> source,
				FunctionTInt<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, true);
		}

		public static OrderedIterable<TSource> thenBy<TSource>(this OrderedIterable<TSource> source, FunctionTLong<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, false);
		}

		public static OrderedIterable<TSource> thenByDescending<TSource>(this OrderedIterable<TSource> source,
				FunctionTLong<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, true);
		}

		public static OrderedIterable<TSource> thenBy<TSource>(this OrderedIterable<TSource> source, FunctionTFloat<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, false);
		}

		public static OrderedIterable<TSource> thenByDescending<TSource>(this OrderedIterable<TSource> source,
				FunctionTFloat<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, true);
		}

		public static OrderedIterable<TSource> thenBy<TSource>(this OrderedIterable<TSource> source, FunctionTDouble<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, false);
		}

		public static OrderedIterable<TSource> thenByDescending<TSource>(this OrderedIterable<TSource> source,
				FunctionTDouble<TSource> keySelector) {
			return source.createOrderedIterable(keySelector, true);
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// range
		//
		
		public static IntIterable range(int start, int count) {
			if (count < 0 || start > Integer.MAX_VALUE - count + 1) throw new IllegalArgumentException("count");
			
			while (count-- > 0) {
				yield return start++;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// repeat
		//
		
		public static Iterable<TSource> repeat<TSource>(TSource element, int count) {
			if (count < 0) throw new IllegalArgumentException("count");
			
			while (count-- > 0) {
				yield return element;
			}
		}
		
		public static IntIterable repeat(int element, int count) {
			if (count < 0) throw new IllegalArgumentException("count");
			
			while (count-- > 0) {
				yield return element;
			}
		}
		
		public static LongIterable repeat(long element, int count) {
			if (count < 0) throw new IllegalArgumentException("count");
			
			while (count-- > 0) {
				yield return element;
			}
		}
		
		public static FloatIterable repeat(float element, int count) {
			if (count < 0) throw new IllegalArgumentException("count");
			
			while (count-- > 0) {
				yield return element;
			}
		}
		
		public static DoubleIterable repeat(double element, int count) {
			if (count < 0) throw new IllegalArgumentException("count");
			
			while (count-- > 0) {
				yield return element;
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// reverse
		//
		
		public static Iterable<TSource> reverse<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");

			var list = new ArrayList<TSource>();
			foreach (var s in source) {
				list.add(s);
			}
			for (int i = list.size() - 1; i >= 0; --i) {
				yield return list[i];
			}
		}
		
		public static IntIterable reverse(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			var list = new InternalIntList(source);
			for (int i = list.size() - 1; i >= 0; --i) {
				yield return list.getItem(i);
			}
		}
		
		public static LongIterable reverse(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			var list = new InternalLongList(source);
			for (int i = list.size() - 1; i >= 0; --i) {
				yield return list.getItem(i);
			}
		}
		
		public static FloatIterable reverse(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			var list = new InternalFloatList(source);
			for (int i = list.size() - 1; i >= 0; --i) {
				yield return list.getItem(i);
			}
		}
		
		public static DoubleIterable reverse(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			var list = new InternalDoubleList(source);
			for (int i = list.size() - 1; i >= 0; --i) {
				yield return list.getItem(i);
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// select
		
		public static Iterable<TResult> select<TSource, TResult>(this Iterable<TSource> source, FunctionTT<TSource, TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static IntIterable select(this BooleanIterable source, FunctionBooleanInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static BooleanIterable select(this IntIterable source, FunctionIntBoolean selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static IntIterable select(this ByteIterable source, FunctionByteInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static ByteIterable select(this IntIterable source, FunctionIntByte selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static IntIterable select(this CharIterable source, FunctionCharInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static CharIterable select(this IntIterable source, FunctionIntChar selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static IntIterable select(this ShortIterable source, FunctionShortInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static ShortIterable select(this IntIterable source, FunctionIntShort selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		
		public static IntIterable select(this IntIterable source, FunctionIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static IntIterable select(this LongIterable source, FunctionLongInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static LongIterable select(this IntIterable source, FunctionIntLong selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static LongIterable select(this LongIterable source, FunctionLongLong selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static FloatIterable select(this IntIterable source, FunctionIntFloat selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static FloatIterable select(this FloatIterable source, FunctionFloatFloat selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static FloatIterable select(this LongIterable source, FunctionLongFloat selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static DoubleIterable select(this IntIterable source, FunctionIntDouble selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static DoubleIterable select(this LongIterable source, FunctionLongDouble selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static DoubleIterable select(this FloatIterable source, FunctionFloatDouble selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static IntIterable select<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this IntIterable source, FunctionIntT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static LongIterable select<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this LongIterable source, FunctionLongT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static FloatIterable select<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this FloatIterable source, FunctionFloatT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static DoubleIterable select<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this DoubleIterable source, FunctionDoubleT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				yield return selector.invoke(s);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// select indexed
		
		public static Iterable<TResult> select<TSource, TResult>(this Iterable<TSource> source, FunctionTIntT<TSource, TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}

		public static IntIterable select(this BooleanIterable source, FunctionBooleanIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static BooleanIterable select(this IntIterable source, FunctionIntIntBoolean selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static IntIterable select(this ByteIterable source, FunctionByteIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static ByteIterable select(this IntIterable source, FunctionIntIntByte selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static IntIterable select(this CharIterable source, FunctionCharIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static CharIterable select(this IntIterable source, FunctionIntIntChar selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static IntIterable select(this ShortIterable source, FunctionShortIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static ShortIterable select(this IntIterable source, FunctionIntIntShort selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		
		public static IntIterable select(this IntIterable source, FunctionIntIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static IntIterable select(this LongIterable source, FunctionLongIntInt selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static LongIterable select(this IntIterable source, FunctionIntIntLong selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static LongIterable select(this LongIterable source, FunctionLongIntLong selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static FloatIterable select(this IntIterable source, FunctionIntIntFloat selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static FloatIterable select(this FloatIterable source, FunctionFloatIntFloat selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static FloatIterable select(this LongIterable source, FunctionLongIntFloat selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static DoubleIterable select(this IntIterable source, FunctionIntIntDouble selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static DoubleIterable select(this LongIterable source, FunctionLongIntDouble selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static DoubleIterable select(this FloatIterable source, FunctionFloatIntDouble selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static IntIterable select<TSource>(this Iterable<TSource> source, FunctionTIntInt<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this IntIterable source, FunctionIntIntT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static LongIterable select<TSource>(this Iterable<TSource> source, FunctionTIntLong<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this LongIterable source, FunctionLongIntT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static FloatIterable select<TSource>(this Iterable<TSource> source, FunctionTIntFloat<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this FloatIterable source, FunctionFloatIntT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static DoubleIterable select<TSource>(this Iterable<TSource> source, FunctionTIntDouble<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}
		
		public static Iterable<TResult> select<TResult>(this DoubleIterable source, FunctionDoubleIntT<TResult> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			var index = 0;
			foreach (var s in source) {
				yield return selector.invoke(s, index++);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// selectMany
		
		public static Iterable<TResult> selectMany<TSource, TResult>(this Iterable<TSource> source,
				FunctionTT<TSource, Iterable<TResult>> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static Iterable<TResult> selectMany<TResult>(this IntIterable source, FunctionIntT<Iterable<TResult>> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static IntIterable selectMany(this IntIterable source, FunctionIntT<IntIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}

		public static IntIterable selectMany<TSource>(this Iterable<TSource> source, FunctionTT<TSource, IntIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static Iterable<TResult> selectMany<TResult>(this LongIterable source, FunctionLongT<Iterable<TResult>> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static LongIterable selectMany(this LongIterable source, FunctionLongT<LongIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}

		public static LongIterable selectMany<TSource>(this Iterable<TSource> source, FunctionTT<TSource, LongIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static Iterable<TResult> selectMany<TResult>(this FloatIterable source, FunctionFloatT<Iterable<TResult>> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static FloatIterable selectMany(this FloatIterable source, FunctionFloatT<FloatIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}

		public static FloatIterable selectMany<TSource>(this Iterable<TSource> source, FunctionTT<TSource, FloatIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static Iterable<TResult> selectMany<TResult>(this DoubleIterable source, FunctionDoubleT<Iterable<TResult>> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static DoubleIterable selectMany(this DoubleIterable source, FunctionDoubleT<DoubleIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}

		public static DoubleIterable selectMany<TSource>(this Iterable<TSource> source, FunctionTT<TSource, DoubleIterable> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");

			foreach (var s in source) {
				foreach (var r in selector.invoke(s)) {
					yield return r;
				}
			}
		}
		
		public static Iterable<TResult> selectMany<TSource, TCollection, TResult>(this Iterable<TSource> source,
				FunctionTT<TSource, Iterable<TCollection>> collectionSelector, FunctionTTT<TSource, TCollection, TResult> resultSelector) {
			if (source == null) throw new NullPointerException("source");
			if (collectionSelector == null) throw new NullPointerException("collectionSelector");
			if (resultSelector == null) throw new NullPointerException("resultSelector");

			foreach (var s in source) {
				foreach (var c in collectionSelector.invoke(s)) {
					yield return resultSelector.invoke(s, c);
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// sequenceEqual
		//
		
		public static bool sequenceEqual<TSource>(this Iterable<TSource> first, Iterable<TSource> second) {
			return sequenceEqual(first, second, null);
		}
		
		public static bool sequenceEqual<TSource>(this Iterable<TSource> first, Iterable<TSource> second, EqualityTester<TSource> tester) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
			if (tester == null) {
				tester = Query.getDefaultEqualityTester();
			}

			var it1 = first.iterator();
			var it2 = second.iterator();
			while (it1.hasNext()) {
				if (!it2.hasNext() || !tester.equals(it1.next(), it2.next())) {
					return false;
				}
			}
			return !it2.hasNext();
		}

		public static bool sequenceEqual(this IntIterable first, IntIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
		
			var it1 = first.iterator();
			var it2 = second.iterator();
			while (it1.hasNext()) {
				if (!it2.hasNext() || it1.nextInt() != it2.nextInt()) {
					return false;
				}
			}
			return !it2.hasNext();
		}

		public static bool sequenceEqual(this LongIterable first, LongIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
		
			var it1 = first.iterator();
			var it2 = second.iterator();
			while (it1.hasNext()) {
				if (!it2.hasNext() || it1.nextLong() != it2.nextLong()) {
					return false;
				}
			}
			return !it2.hasNext();
		}

		public static bool sequenceEqual(this FloatIterable first, FloatIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
		
			var it1 = first.iterator();
			var it2 = second.iterator();
			while (it1.hasNext()) {
				if (!it2.hasNext() || it1.nextFloat() != it2.nextFloat()) {
					return false;
				}
			}
			return !it2.hasNext();
		}

		public static bool sequenceEqual(this DoubleIterable first, DoubleIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");
		
			var it1 = first.iterator();
			var it2 = second.iterator();
			while (it1.hasNext()) {
				if (!it2.hasNext() || it1.nextDouble() != it2.nextDouble()) {
					return false;
				}
			}
			return !it2.hasNext();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// single
		//
		
		public static TSource single<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");
			
			if (source instanceof List<?>) {
				var l = (List<TSource>)source;
				switch (l.size()) {
				case 0:
					throw new NoSuchElementException();
				case 1:
					return l[0];
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result = it.next();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					throw new NoSuchElementException();
				}
			}
		}

		public static int single(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof IntList) {
				var l = (IntList)source;
				switch (l.size()) {
				case 0:
					throw new NoSuchElementException();
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextInt();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					throw new NoSuchElementException();
				}
			}
		}

		public static long single(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof LongList) {
				var l = (LongList)source;
				switch (l.size()) {
				case 0:
					throw new NoSuchElementException();
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextLong();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					throw new NoSuchElementException();
				}
			}
		}

		public static float single(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof FloatList) {
				var l = (FloatList)source;
				switch (l.size()) {
				case 0:
					throw new NoSuchElementException();
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextFloat();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					throw new NoSuchElementException();
				}
			}
		}

		public static double single(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof DoubleList) {
				var l = (DoubleList)source;
				switch (l.size()) {
				case 0:
					throw new NoSuchElementException();
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextDouble();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					throw new NoSuchElementException();
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// singleOrDefault
		//
		
		public static TSource singleOrDefault<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");
			
			if (source instanceof List<?>) {
				var l = (List<TSource>)source;
				switch (l.size()) {
				case 0:
					return null;
				case 1:
					return l[0];
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result = it.next();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					return null;
				}
			}
		}

		public static int singleOrDefault(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof IntList) {
				var l = (IntList)source;
				switch (l.size()) {
				case 0:
					return 0;
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextInt();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					return 0;
				}
			}
		}

		public static long singleOrDefault(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof LongList) {
				var l = (LongList)source;
				switch (l.size()) {
				case 0:
					return 0L;
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextLong();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					return 0L;
				}
			}
		}

		public static float singleOrDefault(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof FloatList) {
				var l = (FloatList)source;
				switch (l.size()) {
				case 0:
					return 0f;
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextFloat();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					return 0f;
				}
			}
		}

		public static double singleOrDefault(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			if (source instanceof DoubleList) {
				var l = (DoubleList)source;
				switch (l.size()) {
				case 0:
					return 0d;
				case 1:
					return l.getItem(0);
				default:
					throw new IllegalStateException();
				}
			} else {
				var it = source.iterator();
				if (it.hasNext()) {
					var result  = it.nextDouble();
					if (it.hasNext()) {
						throw new IllegalStateException();
					}
					return result;
				} else {
					return 0d;
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// skip
		//
		
		public static Iterable<TSource> skip<TSource>(this Iterable<TSource> source, int count) {
			if (source == null) throw new NullPointerException("source");
			
            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
				it.next();
			}
			while (it.hasNext()) {
				yield return it.next();
			}
		}

		public static IntIterable skip(this IntIterable source, int count) {
			if (source == null) throw new NullPointerException("source");
			
            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
				it.nextInt();
			}
			while (it.hasNext()) {
				yield return it.nextInt();
			}
		}

		public static LongIterable skip(this LongIterable source, int count) {
			if (source == null) throw new NullPointerException("source");
			
            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
				it.nextLong();
			}
			while (it.hasNext()) {
				yield return it.nextLong();
			}
		}

		public static FloatIterable skip(this FloatIterable source, int count) {
			if (source == null) throw new NullPointerException("source");
			
            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
				it.nextFloat();
			}
			while (it.hasNext()) {
				yield return it.nextFloat();
			}
		}


		public static DoubleIterable skip(this DoubleIterable source, int count) {
			if (source == null) throw new NullPointerException("source");
			
            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
				it.nextDouble();
			}
			while (it.hasNext()) {
				yield return it.nextDouble();
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// skipWhile
		//
        
        public static Iterable<TSource> skipWhile<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.next();
				 if (!predicate.invoke(s)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.next();
			}
        }

        public static IntIterable skipWhile(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextInt();
				 if (!predicate.invoke(s)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextInt();
			}
        }

        public static LongIterable skipWhile(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextLong();
				 if (!predicate.invoke(s)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextLong();
			}
        }

        public static FloatIterable skipWhile(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextFloat();
				 if (!predicate.invoke(s)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextFloat();
			}
        }

        public static DoubleIterable skipWhile(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextDouble();
				 if (!predicate.invoke(s)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextDouble();
			}
        }

		///////////////////////////////////////////////////////////////////////////////////////////
		// skipWhile indexed
		//
        
        public static Iterable<TSource> skipWhile<TSource>(this Iterable<TSource> source, FunctionTIntBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.next();
				 if (!predicate.invoke(s, index++)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.next();
			}
        }

        public static IntIterable skipWhile(this IntIterable source, FunctionIntIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextInt();
				 if (!predicate.invoke(s, index++)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextInt();
			}
        }

        public static LongIterable skipWhile(this LongIterable source, FunctionLongIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextLong();
				 if (!predicate.invoke(s, index++)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextLong();
			}
        }

        public static FloatIterable skipWhile(this FloatIterable source, FunctionFloatIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextFloat();
				 if (!predicate.invoke(s, index++)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextFloat();
			}
        }

        public static DoubleIterable skipWhile(this DoubleIterable source, FunctionDoubleIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextDouble();
				 if (!predicate.invoke(s, index++)) {
				 	yield return s;
				 	break;
				 }
			}
			while (it.hasNext()) {
				yield return it.nextDouble();
			}
        }

		///////////////////////////////////////////////////////////////////////////////////////////
		// sum
		//
        
        public static int sum(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = 0;
			foreach (var s in source) {
				result += s;
			}
			return result;
        }
        
        public static int sum<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
        
			var result = 0;
			foreach (var s in source) {
				result += selector.invoke(s);
			}
			return result;
        }
        
        public static long sum(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = 0L;
			foreach (var s in source) {
				result += s;
			}
			return result;
        }
        
        public static long sum<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
        
			var result = 0L;
			foreach (var s in source) {
				result += selector.invoke(s);
			}
			return result;
        }
        
        public static float sum(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = 0f;
			foreach (var s in source) {
				result += s;
			}
			return result;
        }
        
        public static float sum<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
        
			var result = 0f;
			foreach (var s in source) {
				result += selector.invoke(s);
			}
			return result;
        }
        
        public static double sum(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = 0d;
			foreach (var s in source) {
				result += s;
			}
			return result;
        }
        
        public static double sum<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
        
			var result = 0d;
			foreach (var s in source) {
				result += selector.invoke(s);
			}
			return result;
        }

		///////////////////////////////////////////////////////////////////////////////////////////
		// take
		//
        
        public static Iterable<TSource> take<TSource>(this Iterable<TSource> source, int count) {
			if (source == null) throw new NullPointerException("source");

            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
                yield return it.next();
            }
        }
        
        public static IntIterable take(this IntIterable source, int count) {
			if (source == null) throw new NullPointerException("source");

            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
                yield return it.nextInt();
            }
        }
        
        public static LongIterable take(this LongIterable source, int count) {
			if (source == null) throw new NullPointerException("source");

            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
                yield return it.nextLong();
            }
        }
        
        public static FloatIterable take(this FloatIterable source, int count) {
			if (source == null) throw new NullPointerException("source");

            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
                yield return it.nextFloat();
            }
        }
        
        public static DoubleIterable take(this DoubleIterable source, int count) {
			if (source == null) throw new NullPointerException("source");

            var it = source.iterator();
            while (count-- > 0 && it.hasNext()) {
                yield return it.nextDouble();
            }
        }

		///////////////////////////////////////////////////////////////////////////////////////////
		// takeWhile
		//
		
		public static Iterable<TSource> takeWhile<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.next();
				if (predicate.invoke(s)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static IntIterable takeWhile(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextInt();
				if (predicate.invoke(s)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static LongIterable takeWhile(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextLong();
				if (predicate.invoke(s)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static FloatIterable takeWhile(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextFloat();
				if (predicate.invoke(s)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static DoubleIterable takeWhile(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			while (it.hasNext()) {
				var s = it.nextDouble();
				if (predicate.invoke(s)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// takeWhile indexed
		//
		
		public static Iterable<TSource> takeWhile<TSource>(this Iterable<TSource> source, FunctionTIntBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.next();
				if (predicate.invoke(s, index++)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static IntIterable takeWhile(this IntIterable source, FunctionIntIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextInt();
				if (predicate.invoke(s, index++)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static LongIterable takeWhile(this LongIterable source, FunctionLongIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextLong();
				if (predicate.invoke(s, index++)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static FloatIterable takeWhile(this FloatIterable source, FunctionFloatIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextFloat();
				if (predicate.invoke(s, index++)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		public static DoubleIterable takeWhile(this DoubleIterable source, FunctionDoubleIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var it = source.iterator();
			var index = 0;
			while (it.hasNext()) {
				var s = it.nextDouble();
				if (predicate.invoke(s, index++)) {
					yield return s;
				} else {
					yield break;
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////
		// toList
		//

		public static List<TSource> toList<TSource>(this Iterable<TSource> source) {
			if (source == null) throw new NullPointerException("source");

			var result = new ArrayList<TSource>();
			foreach (var s in source) {
				result.add(s);
			}
			return result;
		}

		public static IntList toList(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			return new InternalIntList(source);
		}

		public static LongList toList(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			return new InternalLongList(source);
		}

		public static FloatList toList(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			return new InternalFloatList(source);
		}

		public static DoubleList toList(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			return new InternalDoubleList(source);
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		// toMap
		//

		public static Map<TKey, TSource> toMap<TSource, TKey>(this Iterable<TSource> source, FunctionTT<TSource, TKey> selector) {
			return toMap(source, selector, null);
		}

		public static Map<TKey, TSource> toMap<TSource, TKey>(this Iterable<TSource> source, FunctionTT<TSource, TKey> selector,
				EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalMap<TKey, TSource>(tester);
			foreach (var s in source) {
				result[selector.invoke(s)] = s;
			}
			return result;
		}

		public static IntTMap<TSource> toMap<TSource>(this Iterable<TSource> source, FunctionTInt<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalIntTMap<TSource>();
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static TIntMap<TKey> toMap<TKey>(this IntIterable source, FunctionIntT<TKey> selector) {
			return toMap(source, selector, null);
		}

		public static TIntMap<TKey> toMap<TKey>(this IntIterable source, FunctionIntT<TKey> selector, EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalTIntMap<TKey>(tester);
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static LongTMap<TSource> toMap<TSource>(this Iterable<TSource> source, FunctionTLong<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalLongTMap<TSource>();
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static TLongMap<TKey> toMap<TKey>(this LongIterable source, FunctionLongT<TKey> selector) {
			return toMap(source, selector, null);
		}

		public static TLongMap<TKey> toMap<TKey>(this LongIterable source, FunctionLongT<TKey> selector, EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalTLongMap<TKey>(tester);
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static FloatTMap<TSource> toMap<TSource>(this Iterable<TSource> source, FunctionTFloat<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalFloatTMap<TSource>();
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static TFloatMap<TKey> toMap<TKey>(this FloatIterable source, FunctionFloatT<TKey> selector) {
			return toMap(source, selector, null);
		}

		public static TFloatMap<TKey> toMap<TKey>(this FloatIterable source, FunctionFloatT<TKey> selector, EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalTFloatMap<TKey>(tester);
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static DoubleTMap<TSource> toMap<TSource>(this Iterable<TSource> source, FunctionTDouble<TSource> selector) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalDoubleTMap<TSource>();
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		public static TDoubleMap<TKey> toMap<TKey>(this DoubleIterable source, FunctionDoubleT<TKey> selector) {
			return toMap(source, selector, null);
		}

		public static TDoubleMap<TKey> toMap<TKey>(this DoubleIterable source, FunctionDoubleT<TKey> selector, EqualityTester<TKey> tester) {
			if (source == null) throw new NullPointerException("source");
			if (selector == null) throw new NullPointerException("selector");
		
			var result = new InternalTDoubleMap<TKey>(tester);
			foreach (var s in source) {
				result.put(selector.invoke(s), s);
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		// toSet
		//

		public static Set<TSource> toSet<TSource>(this Iterable<TSource> source) {
			return toSet(source, null);
		}
		
		public static Set<TSource> toSet<TSource>(this Iterable<TSource> source, EqualityTester<TSource> tester) {
			if (source == null) throw new NullPointerException("source");
		
			var result = new InternalSet<TSource>(tester);
			foreach (var s in source) {
				result.add(s);
			}
			return result;
		}
		
		public static IntSet toSet(this IntIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = new InternalIntSet();
			foreach (var s in source) {
				result.add(s);
			}
			return result;
		}
		
		public static LongSet toSet(this LongIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = new InternalLongSet();
			foreach (var s in source) {
				result.add(s);
			}
			return result;
		}
		
		public static FloatSet toSet(this FloatIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = new InternalFloatSet();
			foreach (var s in source) {
				result.add(s);
			}
			return result;
		}
		
		public static DoubleSet toSet(this DoubleIterable source) {
			if (source == null) throw new NullPointerException("source");

			var result = new InternalDoubleSet();
			foreach (var s in source) {
				result.add(s);
			}
			return result;
		}
		
		
		///////////////////////////////////////////////////////////////////////////////////////////
		// union
		//

		public static Iterable<TSource> union<TSource>(this Iterable<TSource> first, Iterable<TSource> second) {
			return union(first, second, null);
		}
		
		public static Iterable<TSource> union<TSource>(this Iterable<TSource> first, Iterable<TSource> second, EqualityTester<TSource> tester) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new HashSet<TSource>(tester, 7);
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
			foreach (var s in second) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static IntIterable union(this IntIterable first, IntIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new IntHashSet(7);
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
			foreach (var s in second) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static LongIterable union(this LongIterable first, LongIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new LongHashSet(7);
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
			foreach (var s in second) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static FloatIterable union(this FloatIterable first, FloatIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new FloatHashSet(7);
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
			foreach (var s in second) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		public static DoubleIterable union(this DoubleIterable first, DoubleIterable second) {
			if (first == null) throw new NullPointerException("first");
			if (second == null) throw new NullPointerException("second");

			var set = new DoubleHashSet(7);
			foreach (var s in first) {
				if (set.add(s)) {
					yield return s;
				}
			}
			foreach (var s in second) {
				if (set.add(s)) {
					yield return s;
				}
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		// where
		//
		
		public static Iterable<TSource> where<TSource>(this Iterable<TSource> source, FunctionTBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					yield return s;
				}
			}
		}
		
		public static IntIterable where(this IntIterable source, FunctionIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					yield return s;
				}
			}
		}
		
		public static LongIterable where(this LongIterable source, FunctionLongBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					yield return s;
				}
			}
		}
		
		public static FloatIterable where(this FloatIterable source, FunctionFloatBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					yield return s;
				}
			}
		}
		
		public static DoubleIterable where(this DoubleIterable source, FunctionDoubleBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			foreach (var s in source) {
				if (predicate.invoke(s)) {
					yield return s;
				}
			}
		}
        
		///////////////////////////////////////////////////////////////////////////////////////////
		// where indexed
		//
		
		public static Iterable<TSource> where<TSource>(this Iterable<TSource> source, FunctionTIntBoolean<TSource> predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");

			var index = 0;
			foreach (var s in source) {
				if (predicate.invoke(s, index++)) {
					yield return s;
				}
			}
		}
		
		public static IntIterable where(this IntIterable source, FunctionIntIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var index = 0;
			foreach (var s in source) {
				if (predicate.invoke(s, index++)) {
					yield return s;
				}
			}
		}
		
		public static LongIterable where(this LongIterable source, FunctionLongIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var index = 0;
			foreach (var s in source) {
				if (predicate.invoke(s, index++)) {
					yield return s;
				}
			}
		}
		
		public static FloatIterable where(this FloatIterable source, FunctionFloatIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var index = 0;
			foreach (var s in source) {
				if (predicate.invoke(s, index++)) {
					yield return s;
				}
			}
		}
		
		public static DoubleIterable where(this DoubleIterable source, FunctionDoubleIntBoolean predicate) {
			if (source == null) throw new NullPointerException("source");
			if (predicate == null) throw new NullPointerException("predicate");
			
			var index = 0;
			foreach (var s in source) {
				if (predicate.invoke(s, index++)) {
					yield return s;
				}
			}
		}
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Tuples
        
        public static Iterable<TSource> singleton<TSource>(TSource item) {
            yield return item;
        }
        
        public static Iterable<TSource> pair<TSource>(TSource item1, TSource item2) {
            yield return item1;
            yield return item2;
        }
        
        public static Iterable<TSource> triple<TSource>(TSource item1, TSource item2, TSource item3) {
            yield return item1;
            yield return item2;
            yield return item3;
        }
        
        public static Iterable<TSource> quadruple<TSource>(TSource item1, TSource item2, TSource item3, TSource item4) {
            yield return item1;
            yield return item2;
            yield return item3;
            yield return item4;
        }
		
		///////////////////////////////////////////////////////////////////////////////////////////
        //
		
		#pragma warning disable 270 // Ignore warning about raw generic types
		
		static EqualityTester<T> getDefaultEqualityTester<T>() {
			return (EqualityTester<T>)DefaultEqualityTester.INSTANCE;
		}
		
		private class DefaultEqualityTester<T> : EqualityTester<T> {
			static EqualityTester<?> INSTANCE = new DefaultEqualityTester<Object>();
			
			public bool equals(T item1, T item2) {
				return (item1 == null) ? item2 == null : item1.equals(item2);
			}
			
			public int getHashCode(T item) {
				return (item == null) ? 0 : item.hashCode();
			}
		}
		
		#pragma warning restore
	}
}
