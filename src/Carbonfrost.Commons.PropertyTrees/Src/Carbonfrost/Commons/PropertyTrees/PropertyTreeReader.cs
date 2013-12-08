//
// - PropertyTreeReader.cs -
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
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract partial class PropertyTreeReader : DisposableObject, IPropertyTreeNavigator {

        public abstract PropertyNodeDefinition Definition { get; }

        public PropertyDefinition PropertyDefinition {
            get { return Definition as PropertyDefinition; } }

        public PropertyTreeDefinition PropertyTreeDefinition {
            get { return Definition as PropertyTreeDefinition; } }

        internal virtual IDictionary<string, string> PrefixMap { get { return null; } }

        public abstract ReadState ReadState { get; }
        public abstract bool Read();

        public virtual PropertyTreeNavigator CreateNavigator() {
            return this.ReadPropertyTree().CreateNavigator();
        }

        public bool MoveToContent() {
        start:
            switch (this.NodeType) {
                case PropertyNodeType.Property:
                case PropertyNodeType.PropertyTree:
                case PropertyNodeType.EndPropertyTree:
                    return true;

                default:
                    if (Read())
                        goto start;
                    else
                        return false;
            }
        }

        public PropertyTree ReadPropertyTree() {
            PropertyTreeNodeWriter nodeWriter = new PropertyTreeNodeWriter();
            this.CopyTo(nodeWriter);
            return nodeWriter.Root;
        }

        public virtual object Bind(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType"); // $NON-NLS-1

            PropertyTreeBinder binder =
                PropertyTreeBinder.GetPropertyTreeBinder(componentType, null);
            return binder.Bind(componentType, this);
        }

        public virtual T Bind<T>() {
            return (T) Bind(typeof(T));
        }

        public virtual T Bind<T>(T model) {
            PropertyTreeBinder binder =
                PropertyTreeBinder.GetPropertyTreeBinder(typeof(T), null);
            return (T) binder.Bind(model, this);
        }

        public void CopySubtreeTo(PropertyTreeWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer"); // $NON-NLS-1

            writer.CopyCurrent(this);
            writer.ReadSubtree(this);
        }

        public void CopyTo(PropertyTreeWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer"); // $NON-NLS-1

            // Move past initial implicitly
            StartImplicitly();
            writer.CopyCurrent(this);
            writer.ReadToEnd(this);
        }

        public void CopyTo(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.IsProperty) {
                throw new NotImplementedException();
            } else {
                CopyTo(new PropertyTreeNodeWriter((PropertyTree) node));
            }
        }

        // IPropertyTreeNavigator implementation
        public abstract PropertyNodeType NodeType { get; }
        public abstract string Namespace { get; }
        public abstract string Name { get; }
        public abstract object Value { get; }
        public abstract int Depth { get; }
        public abstract int Position { get; }
        public virtual string Path { get { throw new NotImplementedException(); } }

        public Type ValueType {
            get { return TypeHelper.TypeOf(this.Value); } }

        public QualifiedName QualifiedName {
            get {
                return QualifiedName.Create(this.Namespace, this.Name);
            }
        }

        public abstract bool HasChildren { get; }

        public bool IsProperty {
            get {
                return this.NodeType == PropertyNodeType.Property;
            }
        }

        public bool IsPropertyTree {
            get {
                return this.NodeType == PropertyNodeType.PropertyTree;
            }
        }

        public virtual int LineNumber { get { return -1; } }
        public virtual int LinePosition { get { return -1; } }

        private void StartImplicitly() {
            if (this.ReadState == ReadState.Initial)
                this.Read();
        }
    }
}
