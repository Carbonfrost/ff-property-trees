//
// - PropertyTreeNavigator.cs -
//
// Copyright 2010, 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Diagnostics;
using System.Xml;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees {

    [DebuggerDisplay("{debuggerDisplayProxy}")]
    public abstract class PropertyTreeNavigator : ICloneable, IPropertyNode, IXmlLineInfo {

        // TODO Probably need additional APIs similar to XPath

        public virtual bool CanEdit { get { return false; } }
        public virtual bool HasChildren { get { return false; } }
        public virtual string Path { get { throw new NotImplementedException(); } }

        private string debuggerDisplayProxy {
            get {
                return this.QualifiedName.ToString();
            }
        }

        public bool IsProperty {
            get { return this.NodeType == PropertyNodeType.Property; }
        }

        public bool IsPropertyTree {
            get { return this.NodeType == PropertyNodeType.PropertyTree; }
        }

        protected PropertyTreeNavigator() {}

        public virtual PropertyTreeWriter AppendChild() {
            throw new NotSupportedException();
        }

        public void AppendChild(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            AppendChild().CopyFrom(node);
        }

        public virtual void AppendChild(PropertyTreeReader newChild) {
            if (newChild == null)
                throw new ArgumentNullException("newChild");

            AppendChild().ReadToEnd(newChild);
        }

        public virtual void AppendChild(PropertyTreeNavigator newChild) {
            if (newChild == null)
                throw new ArgumentNullException("newChild");

            PropertyTreeReader reader = CreateReader();
            AppendChild(reader);
        }

        public virtual void AppendPropertyTree(string localName, string ns) {
            if (localName == null)
                throw new ArgumentNullException("localName");
            if (localName.Length == 0)
                throw Failure.EmptyString("localName");

            var w = AppendChild();
            w.WriteStartTree(localName, ns);
            w.WriteEndTree();
        }

        public virtual void AppendPropertyTree(string localName) {
            if (localName == null)
                throw new ArgumentNullException("localName");
            if (localName.Length == 0)
                throw Failure.EmptyString("localName");

            var w = AppendChild();
            w.WriteStartTree(localName);
            w.WriteEndTree();
        }

        public void AppendProperty(string localName, object value) {
            AppendProperty(localName, null, value);
        }

        public void AppendProperty(string localName) {
            AppendProperty(localName, null);
        }

        public virtual void AppendProperty(string localName, string ns, object value) {
            if (localName == null)
                throw new ArgumentNullException("localName");
            if (localName.Length == 0)
                throw Failure.EmptyString("localName");

            // TODO Handle value types that can't be supported here
            AppendChild().WriteProperty(localName, ns, Convert.ToString(value));
        }

        public virtual PropertyTreeNavigator CreateNavigator() {
            return Clone();
        }

        public virtual void RemoveChildren(PropertyTreeNavigator lastSiblingToDelete) {
            throw new NotSupportedException();
        }

        public virtual void RemoveSelf() {
            throw new NotSupportedException();
        }

        public virtual string GetString(string name) {
            throw new NotImplementedException();
        }

        public virtual PropertyTreeWriter InsertAfter() {
            throw new NotSupportedException();
        }

        public virtual PropertyTreeWriter InsertBefore() {
            throw new NotSupportedException();
        }

        public virtual void InsertBefore(PropertyTreeNavigator newSibling) {
            throw new NotSupportedException();
        }

        public virtual bool MoveTo(PropertyTreeNavigator other) {
            throw new NotSupportedException();
        }

        public virtual bool MoveToChild(string ns, string name) {
            throw new NotSupportedException();
        }

        public abstract bool MoveToChild(PropertyNodeType nodeType);
        public abstract bool MoveToChild(int position);
        public abstract bool MoveToFirst();

        public virtual bool MoveToFirstChild() {
            return MoveToChild(0);
        }

        public virtual bool MoveToSibling(int index) {
            throw new NotImplementedException();
        }

        public abstract bool MoveToSibling(string name);
        public abstract bool MoveToSibling(string ns, string name);
        public abstract bool MoveToNext();
        public abstract bool MoveToParent();
        public abstract bool MoveToPrevious();
        public abstract void MoveToRoot();

        // ICloneable implementation
        object ICloneable.Clone() {
            return this.Clone();
        }

        public abstract PropertyTreeNavigator Clone();

        public abstract int Position { get; }
        public abstract int Depth { get; }
        public abstract string Name { get; }
        public abstract string Namespace { get; }

        public abstract PropertyNodeDefinition Definition { get; }

        public Type ValueType {
            get {
                throw new NotImplementedException();
            }
        }

        public abstract object Value { get; set; }
        public abstract PropertyNodeType NodeType { get; }

        public virtual FileLocation FileLocation {
            get {
                return new FileLocation(LineNumber, LinePosition, null);
            }
        }

        // IXmlLineInfo implementation
        public virtual int LineNumber { get { return -1; } }
        public virtual int LinePosition { get { return -1; } }

        bool IXmlLineInfo.HasLineInfo() {
            return true;
        }

        public QualifiedName QualifiedName {
            get {
                return QualifiedName.Create(this.Namespace, this.Name);
            }
        }

        private PropertyTreeReader CreateReader() {
            return new PropertyTreeNavigatorReader(this);
        }

        // `IPropertyTreeReader' implemenation
        public virtual object Bind(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType"); // $NON-NLS-1

            var obj = PropertyTreeMetaObject.Create(componentType);
            return TopLevelBind(obj, null).Component;
        }

        public virtual T Bind<T>() {
            return (T) Bind(typeof(T));
        }

        public virtual T Bind<T>(T model) {
            PropertyTreeMetaObject obj;
            if (ReferenceEquals(model, null))
                obj = PropertyTreeMetaObject.Create(typeof(T));
            else
                obj = PropertyTreeMetaObject.Create(model);

            return (T) TopLevelBind(obj, null).Component;
        }

        public void CopyContentsTo(PropertyTree tree) {
            throw new NotImplementedException();
        }

        public void WriteContentsTo(PropertyTreeWriter writer) {
            throw new NotImplementedException();
        }

        public void WriteTo(PropertyTreeWriter writer) {
            throw new NotImplementedException();
        }

        public void CopyTo(PropertyNode node) {
            throw new NotImplementedException();
        }

        internal PropertyTreeMetaObject TopLevelBind(PropertyTreeMetaObject obj, IServiceProvider serviceProvider) {
            return PropertyTreeMetaObjectBinder.Create().Bind(obj, this, serviceProvider);
        }
    }

}
