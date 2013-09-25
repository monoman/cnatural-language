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

namespace stab.query {

    public interface Grouping<TKey, TElement> : Iterable<TElement> {
        TKey Key^;
    }

    public interface IntGrouping<TElement> : Iterable<TElement> {
        int Key^;
    }

    public interface LongGrouping<TElement> : Iterable<TElement> {
        long Key^;
    }

    public interface FloatGrouping<TElement> : Iterable<TElement> {
        float Key^;
    }

    public interface DoubleGrouping<TElement> : Iterable<TElement> {
        double Key^;
    }
    
    class DefaultGrouping<TKey, TElement> : Grouping<TKey, TElement> {
        private TElement[] array;
        private int count;
        
        DefaultGrouping(TKey key) {
            this.Key = key;
            #pragma warning disable 313
            this.array = new TElement[4];
            #pragma warning restore
        }

        public TKey Key^;

        public Iterator<TElement> iterator() {
            for (int i = 0; i < count; i++) {
                yield return array[i];
            }
        }
        
        public DefaultGrouping<TKey, TElement> Next;
        
        void add(TElement item) {
            if (count == sizeof(array)) {
                #pragma warning disable 313
                var t = new TElement[count * 2];
                #pragma warning restore
                System.arraycopy(array, 0, t, 0, count);
                array = t;
            }
            array[count++] = item;
        }
    }
    
    class DefaultIntGrouping<TElement> : IntGrouping<TElement> {
        private TElement[] array;
        private int count;
        
        DefaultIntGrouping(int key) {
            this.Key = key;
            #pragma warning disable 313
            this.array = new TElement[4];
            #pragma warning restore
        }

        public int Key^;

        public Iterator<TElement> iterator() {
            for (int i = 0; i < count; i++) {
                yield return array[i];
            }
        }
        
        public DefaultIntGrouping<TElement> Next;
        
        void add(TElement item) {
            if (count == sizeof(array)) {
                #pragma warning disable 313
                var t = new TElement[count * 2];
                #pragma warning restore
                System.arraycopy(array, 0, t, 0, count);
                array = t;
            }
            array[count++] = item;
        }
    }
    
    class DefaultLongGrouping<TElement> : LongGrouping<TElement> {
        private TElement[] array;
        private int count;
        
        DefaultLongGrouping(long key) {
            this.Key = key;
            #pragma warning disable 313
            this.array = new TElement[4];
            #pragma warning restore
        }

        public long Key^;

        public Iterator<TElement> iterator() {
            for (int i = 0; i < count; i++) {
                yield return array[i];
            }
        }
        
        public DefaultLongGrouping<TElement> Next;
        
        void add(TElement item) {
            if (count == sizeof(array)) {
                #pragma warning disable 313
                var t = new TElement[count * 2];
                #pragma warning restore
                System.arraycopy(array, 0, t, 0, count);
                array = t;
            }
            array[count++] = item;
        }
    }
    
    class DefaultFloatGrouping<TElement> : FloatGrouping<TElement> {
        private TElement[] array;
        private int count;
        
        DefaultFloatGrouping(float key) {
            this.Key = key;
            #pragma warning disable 313
            this.array = new TElement[4];
        }

        public float Key^;

        public Iterator<TElement> iterator() {
            for (int i = 0; i < count; i++) {
                yield return array[i];
            }
        }
        
        public DefaultFloatGrouping<TElement> Next;
        
        void add(TElement item) {
            if (count == sizeof(array)) {
                #pragma warning disable 313
                var t = new TElement[count * 2];
                #pragma warning restore
                System.arraycopy(array, 0, t, 0, count);
                array = t;
            }
            array[count++] = item;
        }
    }

    class DefaultDoubleGrouping<TElement> : DoubleGrouping<TElement> {
        private TElement[] array;
        private int count;
        
        DefaultDoubleGrouping(double key) {
            this.Key = key;
            #pragma warning disable 313
            this.array = new TElement[4];
            #pragma warning restore
        }

        public double Key^;

        public Iterator<TElement> iterator() {
            for (int i = 0; i < count; i++) {
                yield return array[i];
            }
        }
        
        public DefaultDoubleGrouping<TElement> Next;
        
        void add(TElement item) {
            if (count == sizeof(array)) {
                #pragma warning disable 313
                var t = new TElement[count * 2];
                #pragma warning restore
                System.arraycopy(array, 0, t, 0, count);
                array = t;
            }
            array[count++] = item;
        }
    }
}
