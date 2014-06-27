//
// - NodeList.cs -
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

    abstract class NodeList : IEnumerable<PropertyTreeNavigator> {

        public static NodeList Create(IEnumerable<PropertyTreeNavigator> nodes) {
            // TODO Consider optimizations based on size of list
            return new DefaultNodeList(nodes.ToList());
        }

        public IEnumerable<PropertyTreeNavigator> FindAndRemove(string localName) {
            return FindAndRemove(t => t.Name == localName);
        }

        public bool Remove(PropertyTreeNavigator nav) {
            return FindAndRemove(t => t == nav).Any();
        }

        public abstract IEnumerable<PropertyTreeNavigator> FindAndRemove(Predicate<PropertyTreeNavigator> predicate);

        public abstract void Clear();
        public abstract IEnumerator<PropertyTreeNavigator> GetEnumerator();

        public abstract IEnumerable<PropertyTreeNavigator> Rest();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        class DefaultNodeList : NodeList {

            readonly List<PropertyTreeNavigator> _list;
            readonly BitArray _deleted;
            bool _cleared;

            public DefaultNodeList(List<PropertyTreeNavigator> list) {
                this._list = list;
                this._deleted = new BitArray(list.Count);
            }

            public override void Clear() {
                _cleared = true;
            }

            public override IEnumerable<PropertyTreeNavigator> Rest() {
                if (_cleared)
                    yield break;

                int index = 0;
                foreach (var m in _list) {
                    if (!_deleted[index]) {
                        yield return m;
                    }
                    index++;
                }
            }

            public override IEnumerable<PropertyTreeNavigator> FindAndRemove(Predicate<PropertyTreeNavigator> predicate) {
                if (_cleared)
                    yield break;

                int index = 0;
                foreach (var m in _list) {
                    if (!_deleted[index] && predicate(m)) {
                        _deleted[index] = true;
                        yield return m;
                    }
                    index++;
                }
            }

            public override IEnumerator<PropertyTreeNavigator> GetEnumerator() {
                if (_cleared)
                    return Enumerable.Empty<PropertyTreeNavigator>().GetEnumerator();

                return _list.GetEnumerator();
            }
        }
    }

}

