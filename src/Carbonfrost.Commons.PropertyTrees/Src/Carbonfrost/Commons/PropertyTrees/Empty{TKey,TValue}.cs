//
// - Empty{TKey,TValue}.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections;
using System.Collections.Generic;

using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    internal static class Empty<TKey, TValue> {

        public static readonly IDictionary<TKey, TValue> Dictionary = new NullDictionary();
        public static readonly IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary = (IReadOnlyDictionary<TKey, TValue>) Dictionary;

        private sealed class NullDictionary : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> {

            IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys {
                get {
                    return Empty<TKey>.Array;
                }
            }

            IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values {
                get {
                    return Empty<TValue>.Array;
                }
            }

            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
                throw Failure.ReadOnlyCollection();
            }

            void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
                throw Failure.ReadOnlyCollection();
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
                return false;
            }

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
                throw Failure.ReadOnlyCollection();
            }

            void IDictionary<TKey, TValue>.Add(TKey key, TValue value) {
                throw Failure.ReadOnlyCollection();
            }

            public bool ContainsKey(TKey key) {
                return false;
            }

            bool IDictionary<TKey, TValue>.Remove(TKey key) {
                return false;
            }

            public bool TryGetValue(TKey key, out TValue value) {
                value = default(TValue);
                return false;
            }

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
                return Empty<KeyValuePair<TKey, TValue>>.Enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return Empty<KeyValuePair<TKey, TValue>>.Enumerator;
            }

            public int Count {
                get {
                    return 0;
                }
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
                get {
                    return true;
                }
            }

            public TValue this[TKey key] {
                get {
                    throw new KeyNotFoundException();
                }
                set
                {
                    throw Failure.ReadOnlyCollection();
                }
            }

            ICollection<TKey> IDictionary<TKey, TValue>.Keys {
                get {
                    return Empty<TKey>.Array;
                }
            }

            ICollection<TValue> IDictionary<TKey, TValue>.Values {
                get {
                    return Empty<TValue>.Array;
                }
            }
        }
    }

}
