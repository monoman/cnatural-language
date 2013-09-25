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

namespace stab.lang {

    /// <summary>
    /// Provides methods to manipulate a collection of ints without boxing / unboxing.
    /// </summary>
	public interface IntCollection : Collection<Integer>, IntIterable {
		bool add(int i);
		bool addAll(IntCollection c);
		bool contains(int i);
		bool containsAll(IntCollection c);
		IntIterator iterator();
		bool removeItem(int i);
		bool removeAll(IntCollection c);
		bool retainAll(IntCollection c);
		int[] toArray(int[] a);
	}

    /// <summary>
    /// Provides methods to manipulate a collection of longs without boxing / unboxing.
    /// </summary>
	public interface LongCollection : Collection<Long>, LongIterable {
		bool add(long l);
		bool addAll(LongCollection c);
		bool contains(long l);
		bool containsAll(LongCollection c);
		LongIterator iterator();
		bool removeItem(long l);
		bool removeAll(LongCollection c);
		bool retainAll(LongCollection c);
		long[] toArray(long[] a);
	}

    /// <summary>
    /// Provides methods to manipulate a collection of floats without boxing / unboxing.
    /// </summary>
	public interface FloatCollection : Collection<Float>, FloatIterable {
		bool add(float f);
		bool addAll(FloatCollection c);
		bool contains(float f);
		bool containsAll(FloatCollection c);
		FloatIterator iterator();
		bool removeItem(float f);
		bool removeAll(FloatCollection c);
		bool retainAll(FloatCollection c);
		float[] toArray(float[] a);
	}

    /// <summary>
    /// Provides methods to manipulate a collection of doubles without boxing / unboxing.
    /// </summary>
	public interface DoubleCollection : Collection<Double>, DoubleIterable {
		bool add(double d);
		bool addAll(DoubleCollection c);
		bool contains(double d);
		bool containsAll(DoubleCollection c);
		DoubleIterator iterator();
		bool removeItem(double d);
		bool removeAll(DoubleCollection c);
		bool retainAll(DoubleCollection c);
		double[] toArray(double[] a);
	}
}
