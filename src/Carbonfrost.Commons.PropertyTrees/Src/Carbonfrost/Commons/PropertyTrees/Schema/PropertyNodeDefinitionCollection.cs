//
// - PropertyNodeDefinitionCollection.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class PropertyNodeDefinitionCollection<T> : IMakeReadOnly, ICollection<T>, IEnumerable<T>
        where T : PropertyNodeDefinition
    {

        private bool isReadOnly;
        private readonly Dictionary<QualifiedName, T> items = new Dictionary<QualifiedName, T>(Utility.OrdinalIgnoreCaseQualifiedName);

        protected bool IsReadOnly { get { return isReadOnly; } }

        internal PropertyNodeDefinitionCollection() {}

        protected void ThrowIfReadOnly() {
            if (IsReadOnly)
                throw Failure.ReadOnlyCollection();
        }

        public bool ContainsValue(T value) {
            return items.ContainsValue(value);
        }

        internal virtual void AddInternal(T item) {
            lock (this.items) {
                this.items.Add(item.QualifiedName, item);
            }
        }

        public void Add(T item) {
            if (item == null)
                throw new ArgumentNullException("item");
            ThrowIfReadOnly();
            AddInternal(item);
        }

        public T this[string name, string ns] {
            get {
                if (name == null)
                    throw new ArgumentNullException("name"); // $NON-NLS-1
                if (name.Length == 0)
                    throw Failure.EmptyString("name"); // $NON-NLS-1
                ns = ns ?? string.Empty;

                return this[QualifiedName.Create(ns, name)];
            }
        }

        public IEnumerable<T> GetByLocalName(string name) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            return this.items.Values.Where(t => t.Name == name);
        }

        public IEnumerator<T> GetEnumerator() {
            return items.Values.GetEnumerator();
        }

        public bool Contains(QualifiedName key) {
            return ContainsKey(key);
        }

        public bool Contains(string name) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            return this.items.Keys.Any(t => string.Equals(t.LocalName, name, StringComparison.OrdinalIgnoreCase));
        }

        public bool Contains(string name, string ns) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            return Contains(QualifiedName.Create(ns ?? string.Empty, name));
        }

        // IDictionary implementation
        public T this[QualifiedName name] {
            get {
                T result;
                if (this.items.TryGetValue(name, out result))
                    return result;

                return null;
            }
            set {
                ThrowIfReadOnly();
                items[name] = value;
            }
        }

        public int Count {
            get { return items.Count; } }

        bool IMakeReadOnly.IsReadOnly {
            get { return isReadOnly; } }

        bool ICollection<T>.IsReadOnly {
            get { return isReadOnly; } }

        public bool ContainsKey(QualifiedName key) {
            return items.ContainsKey(key);
        }

        public bool Remove(QualifiedName key) {
            ThrowIfReadOnly();
            return items.Remove(key);
        }

        public bool TryGetValue(QualifiedName key, out T value) {
            return items.TryGetValue(key, out value);
        }

        public void Clear() {
            ThrowIfReadOnly();
            items.Clear();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void MakeReadOnly() {
            this.isReadOnly = true;
        }

        public bool Contains(T item) {
            if (item == null)
                throw new ArgumentNullException("item");

            return this.ContainsKey(item.QualifiedName);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            items.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            if (item == null)
                throw new ArgumentNullException("item");

            if (object.Equals(item, this[item.QualifiedName]))
                return Remove(item.QualifiedName);

            return false;
        }
    }
}
