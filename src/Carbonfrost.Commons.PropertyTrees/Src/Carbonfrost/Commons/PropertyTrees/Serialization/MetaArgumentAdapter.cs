//
// - MetaArgumentAdapter.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class MetaArgumentAdapter : IReadOnlyDictionary<string, object> {

        readonly IReadOnlyDictionary<string, PropertyTreeMetaObject> _parameters;

        public MetaArgumentAdapter(IReadOnlyDictionary<string, PropertyTreeMetaObject> parameters) {
            this._parameters = parameters;
        }

        public bool ContainsKey(string key) {
            return _parameters.ContainsKey(key);
        }

        public bool TryGetValue(string key, out object value) {
            PropertyTreeMetaObject ptmo;
            if (_parameters.TryGetValue(key, out ptmo)) {
                value = ptmo.Component;
                return true;
            }

            value = null;
            return false;
        }

        public object this[string key] {
            get {
                object value;
                if (TryGetValue(key, out value))
                    return value;

                throw new KeyNotFoundException();
            }
        }

        public IEnumerable<string> Keys {
            get {
                return _parameters.Keys;
            }
        }

        public IEnumerable<object> Values {
            get {
                return _parameters.Values.Select(t => t.Component);
            }
        }

        public int Count {
            get {
                return _parameters.Count;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return new Enumerator(_parameters.GetEnumerator());
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        struct Enumerator : IEnumerator<KeyValuePair<string, object>> {

            readonly IEnumerator<KeyValuePair<string, PropertyTreeMetaObject>> _inner;

            public Enumerator(IEnumerator<KeyValuePair<string, PropertyTreeMetaObject>> inner) {
                this._inner = inner;
            }

            public bool MoveNext() {
                return _inner.MoveNext();
            }

            public void Reset() {
                _inner.Reset();
            }

            object System.Collections.IEnumerator.Current {
                get {
                    return Current;
                }
            }

            void IDisposable.Dispose() {}

            public KeyValuePair<string, object> Current {
                get {
                    var current = _inner.Current;
                    return new KeyValuePair<string, object>(current.Key, current.Value.Component);
                }
            }

        }

    }
}


