//
// - PropertyNodeCollection.cs -
//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract class PropertyNodeCollection : IList<PropertyNode>, IList {

        public virtual PropertyNode this[QualifiedName name] {
            get {
                if (name == null)
                    throw new ArgumentNullException("name");

                foreach (var t in this) {
                    if (name == t.QualifiedName)
                        return t;
                }
                return null;
            }
        }

        public virtual PropertyNode this[string name, string ns] {
            get {
                if (name == null)
                    throw new ArgumentNullException("name"); // $NON-NLS-1
                if (name.Length == 0)
                    throw Failure.EmptyString("name"); // $NON-NLS-1
                ns = ns ?? string.Empty;

                return this[QualifiedName.Create(ns, name)];
            }
        }

        public virtual PropertyNode this[string name] {
            get {
                return this[name, null];
            }
        }

        public virtual PropertyNode this[int index] {
            get {
                if (index < 0 || index > this.Count)
                    throw Failure.IndexOutOfRange("index", index, 0, this.Count - 1);

                foreach (var t in this) {
                    if (index-- == 0)
                        return t;
                }
                return null;
            }
        }

        // IList<PropertyNode> implementation
        PropertyNode IList<PropertyNode>.this[int index] {
            get { return this[index]; }
            set { throw Failure.ReadOnlyCollection(); } }

        public abstract int Count { get; }

        public virtual bool IsReadOnly { get { return false; } }

        public virtual int IndexOf(PropertyNode item) {
            if (item == null)
                throw new ArgumentNullException("item"); // $NON-NLS-1

            int index = 0;
            foreach (var pn in this) {
                if (object.Equals(pn, item))
                    return index;
                index++;
            }

            return -1;
        }

        public virtual int IndexOf(QualifiedName name) {
            if (name == null)
                throw new ArgumentNullException("name");

            int index = 0;
            foreach (var pn in this) {
                if (pn.QualifiedName == name)
                    return index;
                index++;
            }

            return -1;
        }

        void IList<PropertyNode>.Insert(int index, PropertyNode item) {
            throw Failure.ReadOnlyCollection();
        }

        void IList<PropertyNode>.RemoveAt(int index) {
            throw Failure.ReadOnlyCollection();
        }

        void ICollection<PropertyNode>.Add(PropertyNode item) {
            throw Failure.ReadOnlyCollection();
        }

        void ICollection<PropertyNode>.Clear() {
            throw Failure.ReadOnlyCollection();
        }

        public virtual bool Contains(PropertyNode item) {
            return IndexOf(item) >= 0;
        }

        public bool Contains(string ns, string name) {
            return IndexOf(QualifiedName.Create(ns, name)) >= 0;
        }

        public bool Contains(string name) {
            return IndexOf(NamespaceUri.Default + name) >= 0;
        }

        public virtual bool Contains(QualifiedName name) {
            return IndexOf(name) >= 0;
        }

        public virtual void CopyTo(PropertyNode[] array, int arrayIndex) {
            if (array == null)
                throw new ArgumentNullException("array"); // $NON-NLS-1
            if (array.Rank != 1)
                throw Failure.RankNotOne("array"); // $NON-NLS-1
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw Failure.IndexOutOfRange("arrayIndex", arrayIndex, 0, array.Length - 1); // $NON-NLS-1
            if (arrayIndex + this.Count > array.Length)
                throw Failure.NotEnoughSpaceInArray("arrayIndex", arrayIndex); // $NON-NLS-1

            foreach (var t in this)
                array[arrayIndex++] = t;
        }

        bool ICollection<PropertyNode>.Remove(PropertyNode item) {
            throw Failure.ReadOnlyCollection();
        }

        public abstract IEnumerator<PropertyNode> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        object IList.this[int index] {
            get {
                return this[index];
            }
            set {
                throw Failure.ReadOnlyCollection();
            }
        }

        bool IList.IsFixedSize {
            get { return false; } }

        object ICollection.SyncRoot {
            get { return null; } }

        bool ICollection.IsSynchronized {
            get { return false; } }

        int IList.Add(object value) {
            throw Failure.ReadOnlyCollection();
        }

        bool IList.Contains(object value) {
            throw Failure.ReadOnlyCollection();
        }

        int IList.IndexOf(object value) {
            if (value == null)
                throw new ArgumentNullException("value");
            PropertyNode node = value as PropertyNode;
            if (node == null)
                return -1;

            int index = 0;
            foreach (var current in this) {
                if (node == current)
                    return index;
                index++;
            }

            return -1;
        }

        void IList.Insert(int index, object value) {
            throw Failure.ReadOnlyCollection();
        }

        void IList.Remove(object value) {
            throw Failure.ReadOnlyCollection();
        }

        void ICollection.CopyTo(Array array, int index) {
            this.ToArray().CopyTo(array, index);
        }

        void IList.RemoveAt(int index) {
            throw Failure.ReadOnlyCollection();
        }

        void IList.Clear() {
            throw Failure.ReadOnlyCollection();
        }
    }
}
