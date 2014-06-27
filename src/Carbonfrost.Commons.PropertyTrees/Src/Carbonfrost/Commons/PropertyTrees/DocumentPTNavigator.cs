//
// - DocumentPTNavigator.cs -
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
using System.ComponentModel;
using System.Xml;

using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class DocumentPTNavigator : PropertyTreeNavigator, IXmlNamespaceResolver {

        private PropertyNode current;

        public DocumentPTNavigator(PropertyNode current) {
            this.current = current;
        }

        public override int Position {
            get { return current.Position; } }

        public override int LineNumber {
            get { return current.LineNumber; }
        }

        public override int LinePosition {
            get { return current.LinePosition; }
        }

        public override bool MoveTo(PropertyTreeNavigator other) {
            if (other == null)
                throw new ArgumentNullException("other");

            DocumentPTNavigator otherNav = other as DocumentPTNavigator;
            if (otherNav != null && this.current.Root == otherNav.current.Root) {
                this.current = otherNav.current;
                return true;
            }
            return false;
        }

        public override bool MoveToChild(int position) {
            if (position < 0 && current.Parent != null) {
                position = position % current.Parent.Children.Count;
            }
            return MoveToChild(node => node.Position == position);
        }

        public override bool MoveToChild(PropertyNodeType nodeType) {
            return MoveToChild(node => node.NodeType == nodeType);
        }

        public override bool MoveToChild(string ns, string name) {
            Require.NotNullOrEmptyString("name", name);
            ns = ns ?? string.Empty;

            return MoveToChild(node => node.Name == name && node.Namespace == ns);
        }

        public override bool MoveToFirst() {
            return MoveToGeneric(this.current.FirstSibling);
        }

        public override void MoveToRoot() {
            this.current = this.current.Root;
        }

        public override bool MoveToSibling(int index) {
            return MoveToSibling(node => node.Position == index);
        }

        public override bool MoveToSibling(string name) {
            Require.NotNullOrEmptyString("name", name);
            return MoveToSibling(node => node.Name == name);
        }

        public override bool MoveToSibling(string ns, string name) {
            Require.NotNullOrEmptyString("name", name);
            ns = ns ?? string.Empty;

            return MoveToSibling(node => node.Name == name && node.Namespace == ns);
        }

        public override bool MoveToNext() {
            return MoveToGeneric(this.current.NextSibling);
        }

        public override bool MoveToPrevious() {
            return MoveToGeneric(this.current.PreviousSibling);
        }

        public override bool MoveToParent() {
            return MoveToGeneric(this.current.Parent);
        }

        public override PropertyTreeNavigator Clone() {
            return new DocumentPTNavigator(this.current);
        }

        public override object Value {
            get { return this.current.Value; }
            set { this.current.Value = value; }
        }

        public override int Depth {
            get { return current.Depth; } }

        public override PropertyNodeDefinition Definition {
            get { return current.Definition; } }

        public override PropertyNodeType NodeType {
            get { return this.current.NodeType; } }

        public override string Namespace {
            get { return this.current.Namespace; } }

        public override string Name {
            get { return this.current.Name; } }

        internal override bool IsExpressNamespace {
            get {
                return this.current.IsExpressNamespace;
            }
        }

        public override PropertyTreeWriter AppendChild() {
            if (this.current.IsPropertyTree) {
                var tree = (PropertyTree) this.current;
                return new PropertyTreeNodeWriter(tree);
            } else {
                // TODO Could there be a schema tree here?
                throw new NotImplementedException();
            }
        }

        public override void AppendProperty(string localName, string ns, object value) {
            this.current.AppendProperty(localName, ns, value);
        }

        // ---
        private bool MoveToChild(Func<PropertyNode, bool> predicate) {
            return MoveToSibling(this.current.FirstChild, predicate);
        }

        private bool MoveToSibling(Func<PropertyNode, bool> predicate) {
            return MoveToSibling(this.current.FirstSibling, predicate);
        }

        private bool MoveToSibling(PropertyNode start,
                                   Func<PropertyNode, bool> predicate) {
            PropertyNode node = start;
            if (node != null) {
                while (!(predicate(node))) {
                    node = node.NextSibling;
                    if (node == null)
                        return false;
                }

                this.current = node;
                return true;
            }
            return false;
        }

        private bool MoveToGeneric(PropertyNode node) {
            if (node == null)
                return false;

            this.current = node;
            return true;
        }

        // `IXmlNamespaceResolver'
        public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope) {
            return null;
        }

        public string LookupNamespace(string prefix) {
            return this.current.LookupNamespace(prefix);
        }

        public string LookupPrefix(string namespaceName) {
            return null;
        }
    }
}
