//
// - PropertyTreeNavigatorWrapper.cs -
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
using System.Collections.Generic;
using System.Xml;

using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract class PropertyTreeNavigatorWrapper : PropertyTreeNavigator, IXmlNamespaceResolver, IXmlLineInfo {

        private readonly PropertyTreeNavigator navigator;

        private IXmlNamespaceResolver Resolver {
            get {
                return Navigator as IXmlNamespaceResolver ?? Utility.NullResolver;
            }
        }

        protected PropertyTreeNavigator Navigator {
            get { return navigator; }
        }

        protected PropertyTreeNavigatorWrapper(PropertyTreeNavigator navigator) {
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            this.navigator = navigator;
        }

        public override int LineNumber {
            get { return Navigator.LineNumber; }
        }

        public override int LinePosition {
            get { return Navigator.LinePosition; }
        }

        public override object Value {
            get {
                return Navigator.Value;
            }
            set {
                Navigator.Value = value;
            }
        }

        public override int Position {
            get {
                return Navigator.Position;
            }
        }

        public override PropertyNodeType NodeType {
            get {
                return Navigator.NodeType;
            }
        }

        public override string Namespace {
            get {
                return Navigator.Namespace;
            }
        }

        public override string Name {
            get {
                return Navigator.Name;
            }
        }

        public override bool MoveToSibling(string ns, string name) {
            return Navigator.MoveToSibling(ns, name);
        }

        public override bool MoveToSibling(string name) {
            return Navigator.MoveToSibling(name);
        }

        public override void MoveToRoot() {
            Navigator.MoveToRoot();
        }

        public override bool MoveToPrevious() {
            return Navigator.MoveToPrevious();
        }

        public override bool MoveToParent() {
            return Navigator.MoveToParent();
        }

        public override bool MoveToNext() {
            return Navigator.MoveToNext();
        }

        public override bool MoveToFirst() {
            return Navigator.MoveToFirst();
        }

        public override bool MoveToChild(int position) {
            return Navigator.MoveToChild(position);
        }

        public override bool MoveToChild(PropertyNodeType nodeType) {
            return Navigator.MoveToChild(nodeType);
        }

        public override int Depth {
            get {
                return Navigator.Depth;
            }
        }

        public override PropertyNodeDefinition Definition {
            get {
                return Navigator.Definition;
            }
        }

        public override PropertyTreeNavigator Clone() {
            // Clients should override this, but we use reflection to help
            try {
                return (PropertyTreeNavigator) Activator.CreateInstance(GetType(), new[] { Navigator.Clone() });

            } catch (Exception) {
                throw new NotImplementedException();
            }
        }

        public virtual IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope) {
            return Resolver.GetNamespacesInScope(scope);
        }

        public virtual string LookupNamespace(string prefix) {
            return Resolver.LookupNamespace(prefix);
        }

        public virtual string LookupPrefix(string namespaceName) {
            return Resolver.LookupPrefix(namespaceName);
        }

    }
}
