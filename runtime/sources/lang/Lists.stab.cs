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
    /// Provides methods to manipulate a list of ints without boxing / unboxing.
    /// </summary>
	public interface IntList : List<Integer>, IntCollection {
		void add(int index, int i);
		bool addAll(int index, IntCollection c);
		int indexOf(int i);
		int lastIndexOf(int i);
		int removeAt(int index);
		IntList subList(int fromIndex, int toIndex);
		int getItem(int index);
		int setItem(int index, int value);
	}

	/// <summary>
    /// Provides methods to manipulate a list of longs without boxing / unboxing.
    /// </summary>
	public interface LongList : List<Long>, LongCollection {
		void add(int index, long l);
		bool addAll(int index, LongCollection c);
		int indexOf(long l);
		int lastIndexOf(long l);
		long removeAt(int index);
		LongList subList(int fromIndex, int toIndex);
		long getItem(int index);
		long setItem(int index, long value);
	}

	/// <summary>
    /// Provides methods to manipulate a list of floats without boxing / unboxing.
    /// </summary>
	public interface FloatList : List<Float>, FloatCollection {
		void add(int index, float f);
		bool addAll(int index, FloatCollection c);
		int indexOf(float f);
		int lastIndexOf(float f);
		float removeAt(int index);
		FloatList subList(int fromIndex, int toIndex);
		float getItem(int index);
		float setItem(int index, float value);
	}

	/// <summary>
    /// Provides methods to manipulate a list of doubles without boxing / unboxing.
    /// </summary>
	public interface DoubleList : List<Double>, DoubleCollection {
		void add(int index, double d);
		bool addAll(int index, DoubleCollection c);
		int indexOf(double d);
		int lastIndexOf(double d);
		double removeAt(int index);
		DoubleList subList(int fromIndex, int toIndex);
		double getItem(int index);
		double setItem(int index, double value);
	}
}
