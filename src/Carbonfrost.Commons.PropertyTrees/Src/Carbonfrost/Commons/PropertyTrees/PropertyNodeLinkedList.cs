//
// - PropertyNodeLinkedList.cs -
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
using System.Collections.Generic;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    [Serializable]
    sealed class PropertyNodeLinkedList : PropertyNodeCollection {

        private PropertyNode head;
        private PropertyNode tail;
        private int version;
        private readonly PropertyTree parent;

        public PropertyNode Tail { get { return tail; } }
        public PropertyNode Head { get { return head; } }

        internal PropertyNodeLinkedList(PropertyTree parent) {
            this.parent = parent;
        }

        internal void Clear() {
            foreach (var node in this) {
                Exit(node);
            }

            this.head = null;
            this.tail = null;
        }

        private void Exit(PropertyNode node) {
            node.NextSibling = null;
            node.PreviousSibling = null;
            node.Position = -1;
            node.parent = null;
        }

        internal void Remove(PropertyNode node) {
            if (head == node) {
                this.head = node.NextSibling;
            }
            if (tail == node) {
                this.tail = node.PreviousSibling ?? head;
            }

            var prev = node.PreviousSibling;
            var next = node.NextSibling;
            if (prev != null) prev.NextSibling = next;
            if (next != null) {
                next.PreviousSibling = prev;
                ApplyPositions(next, node.Position);
            }

            node.parent = null;
            node.Position = -1;
            node.NextSibling = null;
            node.PreviousSibling = null;
        }

        internal void InsertInternal(int index, PropertyNode node) {
            if (index == 0) {
                AddFirst(node);

            } else if (index == Count) {
                AddLast(node);

            } else {
                var before = this[index];
                InsertBefore(node, before);
            }
        }

        internal void InsertBefore(PropertyNode node, PropertyNode before) {
            before.PreviousSibling.NextSibling = node;
            node.PreviousSibling = before.PreviousSibling;
            before.PreviousSibling = node;
            node.NextSibling = before;
            node.parent = this.parent;
            version++;

            ApplyPositions(node, before.Position);
        }

        private void ApplyPositions(PropertyNode node, int position) {
            while (node != null) {
                node.Position = position++;
                node = node.NextSibling;
            }
        }

        private void AddFirst(PropertyNode node) {
            if (head == null) {
                this.head = this.tail = node;
                this.head.Position = 0;

            } else {
                this.head.PreviousSibling = node;
                node.NextSibling = this.head;
                node.PreviousSibling = null;
                this.head = node;
                ApplyPositions(this.head, 0);
            }

            version++;
            node.parent = this.parent;
        }

        private void AddLast(PropertyNode node) {
            if (head == null) {
                this.head = this.tail = node;
                this.head.Position = 0;

            } else {
                this.tail.NextSibling = node;
                node.Position = tail.Position + 1;
                node.PreviousSibling = tail;
                tail = node;
            }

            version++;
            node.parent = this.parent;
        }

        public override int Count {
            get {
                if (tail == null)
                    return 0;
                else
                    return tail.Position + 1;
            }
        }

        public override IEnumerator<PropertyNode> GetEnumerator() {
            PropertyNode current = this.head;
            int startVersion = this.version;
            while (current != null) {
                if (startVersion != this.version)
                    throw Failure.CollectionModified();

                yield return current;
                current = current.NextSibling;
            }
        }
    }
}

