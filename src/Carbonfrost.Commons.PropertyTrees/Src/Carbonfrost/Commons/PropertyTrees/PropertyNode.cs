//
// - PropertyNode.cs -
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract partial class PropertyNode
        : IHierarchyObject, ICloneable, INotifyPropertyChanged, IHierarchyNavigable, IPropertyNode {

        internal PropertyTree parent;
        private IDictionary<string, string> prefixMap;
        private bool isExpressNamespace;

        public abstract PropertyNodeCollection Children { get; }

        public PropertyNode this[int index] {
            get { return Children[index]; }
        }

        public PropertyNode this[QualifiedName name] {
            get { return Children[name]; }
        }

        public PropertyNode this[string name] {
            get { return Children[name]; }
        }

        public PropertyNode this[string name, string ns] {
            get { return Children[name, ns]; }
        }

        public PropertyNode PreviousSibling { get; internal set; }
        public PropertyNode NextSibling { get; internal set; }

        public Uri BaseUri {
            get;
            set;
        }

        public PropertyNode FirstSibling {
            get {
                return (this.Parent == null) ? null : this.Parent.FirstChild;
            }
        }

        public PropertyNode LastSibling {
            get {
                return (this.Parent == null) ? null : this.Parent.LastChild;
            }
        }

        public virtual PropertyNode LastChild { get { return null; } }
        public virtual PropertyNode FirstChild { get { return null; } }

        public QualifiedName QualifiedName {
            get {
                if (string.IsNullOrEmpty(this.Name))
                    return null;

                return QualifiedName.Create(this.Namespace ?? string.Empty, this.Name);
            }
            set {
                if (value == null) {
                    this.Name = null;
                    this.Namespace = null;
                } else {
                    this.Name = value.LocalName;
                    this.Namespace = value.Namespace.NamespaceName;
                }
            }
        }

        public int LineNumber { get; set; }
        public int LinePosition { get; set; }

        bool IPropertyTreeNavigator.IsExpressNamespace {
            get { return IsExpressNamespace; }
        }

        internal virtual bool IsExpressNamespace {
            get {
                return this.isExpressNamespace;
            }
        }

        protected internal abstract void AcceptVisitor(PropertyTreeVisitor visitor);
        protected internal abstract TResult AcceptVisitor<TArgument, TResult>(PropertyTreeVisitor<TArgument, TResult> visitor, TArgument argument);

        public IEnumerable<PropertyNode> SelectNodes(string path) {
            throw new NotImplementedException();
        }

        public virtual PropertyNode SelectNode(string path) {
            // UNDONE Actual implementation that uses paths
            return SelectChild(path);
        }

        public PropertyTreeNavigator CreateNavigator() {
            return new DocumentPTNavigator(this);
        }

        public NameValueCollection Flatten() {
            return new PropertyTreeFlattener().DoVisit(this);
        }

        public void RemoveSelf() {
            if (Parent == null)
                throw PropertyTreesFailure.CannotDeleteRoot();

            Parent.RemoveChild(this);
        }

        public PropertyTreeReader ReadNode() {
            return ReadNode(false);
        }

        private PropertyTreeReader ReadNode(bool move) {
            var result = new PropertyTreeNodeReader(this);
            if (move)
                result.Read();

            return result;
        }

        public virtual void Rename(string newName) {
            // TODO Name behavior -- arbitrary changes
            // Enforce schema logic (trees can rename and aggregate)
            throw new NotImplementedException();
        }

        public void ReplaceWith(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (Parent != null) {
                int index = this.Position;
                var parent = this.Parent;
                Parent.RemoveChild(this);
                parent.InsertChildAt(index, node);
            }
        }

        public void WriteTo(PropertyTreeWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteNode(this);
        }

        public void WriteContentsTo(PropertyTreeWriter writer) {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (var s in this.Children)
                writer.WriteNode(s);
        }

        public PropertyTree Root {
            get {
                PropertyNode current = this;
                while (current.Parent != null) {
                    current = current.Parent;
                }

                return (PropertyTree) current;
            }
        }

        public PropertyTree Parent {
            get { return parent; }
            set {
                if (value == null) {
                    if (this.Parent != null)
                        this.Parent.RemoveChild(this);

                } else {
                    value.AppendChild(this);
                }
            }
        }

        internal PropertyNode() {}

        public void AppendChild(PropertyNode propertyNode) {
            InsertChildAt(Children.Count, propertyNode);
        }

        public void PrependChild(PropertyNode propertyNode) {
            InsertChildAt(0, propertyNode);
        }

        public virtual PropertyTree AppendTree(string name) {
            var result = new PropertyTree { Name = name };
            this.AppendChild(result);
            return result;
        }

        public virtual void InsertChildAt(int index, PropertyNode node) {
            throw PropertyTreesFailure.CannotAppendChild();
        }

        public void RemoveChildAt(int index) {
            RemoveChild(this.Children[index]);
        }

        public virtual bool RemoveChild(PropertyNode node) {
            return false;
        }

        public virtual bool RemoveChildren() {
            return false;
        }

        public virtual int IndexOfChild(PropertyNode propertyNode) {
            return -1;
        }

        public virtual IEnumerable<PropertyNode> GetDescendants(string name, Type blessedType) {
            throw new NotImplementedException();
        }

        public virtual void Bless(Type type) {
        }

        public void Unbless() {
            Bless(null);
        }

        public void AppendTo(PropertyTree other) {
            other.AppendChild(this);
        }

        // `IPropertyTreeReader`
        public T Bind<T>() {
            return CreateNavigator().Bind<T>();
        }

        public T Bind<T>(T model) {
            return CreateNavigator().Bind<T>(model);
        }

        public object Bind(Type instanceType) {
            return CreateNavigator().Bind(instanceType);
        }

        // IHierarchyObject implementation
        IHierarchyObject IHierarchyObject.ParentObject {
            get {
                return Parent;
            }
            set {
                if (value == null)
                    this.Parent = null;
                else {
                    PropertyTree newParent = value as PropertyTree;
                    if (newParent == null)
                        throw Failure.NotInstanceOf("value", value, typeof(PropertyNode));

                    this.Parent = newParent;
                }
            }
        }

        IEnumerable<IHierarchyObject> IHierarchyObject.ChildrenObjects {
            get {
                return this.Children;
            }
        }

        // IHierarchyNavigable implementation
        public PropertyNode SelectChild(string name) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw Failure.EmptyString("name");

            return this.Children[name];
        }

        public PropertyNode SelectChild(int index) {
            return this.Children[index];
        }

        IHierarchyNavigable IHierarchyNavigable.SelectChild(string childId) {
            return SelectChild(childId);
        }

        IHierarchyNavigable IHierarchyNavigable.SelectChild(int index) {
            return SelectChild(index);
        }

        public object SelectAttribute(string attribute) {
            if (attribute == null)
                throw new ArgumentNullException("attribute"); // $NON-NLS-1
            attribute = attribute.Trim();

            if (attribute.Length == 0)
                throw Failure.AllWhitespace("attribute"); // $NON-NLS-1

            switch (attribute) {
                case "position":
                    return this.Position;
                case "empty":
                    return !HasChildren;
                case "root":
                    return IsRoot;
                case "tree":
                    return IsPropertyTree;
                case "property":
                    return IsProperty;
                default:
                    return SelectAttributeCore(attribute);
            }
        }

        protected abstract object SelectAttributeCore(string attribute);

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        // 'ICloneable' implementation.
        object ICloneable.Clone() {
            return Clone();
        }

        protected abstract PropertyNode CloneCore();

        public PropertyNode Clone() {
            return CloneCore();
        }

        // IPropertyTreeNavigator implementation
        public string Namespace { get; set; }
        public string Name { get; set; }
        public virtual int Depth { get { return 0; } }
        public abstract PropertyNodeDefinition Definition { get; }

        public int Position { get; internal set; }

        public virtual string Path { get { return string.Empty; } }
        public bool IsPropertyTree { get { return this.NodeType == PropertyNodeType.PropertyTree; } }
        public bool IsProperty { get { return this.NodeType == PropertyNodeType.Property; } }
        public bool IsRoot { get { return this.Parent == null; } }
        public abstract PropertyNodeType NodeType { get; }

        public abstract object Value { get; set; }

        public Type ValueType {
            get { return TypeHelper.TypeOf(this.Value); } }

        public bool HasChildren {
            get {
                return this.Children.Count > 0;
            }
        }

        public PropertyTreeWriter AppendChild() {
            if (this.IsPropertyTree)
                return new PropertyTreeNodeWriter((PropertyTree) this);
            else
                throw PropertyTreesFailure.CannotAppendChild();
        }

        public void AppendChild(PropertyTreeReader newChild) {
            if (newChild == null)
                throw new ArgumentNullException("newChild");

            AppendChild().CopyFrom(newChild);
        }

        public void AppendChild(PropertyTreeNavigator newChild) {
            throw new NotImplementedException();
        }

        public void AppendPropertyTree(string localName, string ns) {
            if (localName == null)
                throw new ArgumentNullException("localName");
            if (localName.Length == 0)
                throw Failure.EmptyString("localName");

            AppendChild(new PropertyTree(localName, ns));
        }

        public void AppendPropertyTree(string localName) {
            AppendPropertyTree(localName, string.Empty);
        }

        public void AppendProperty(string localName, string ns, object value) {
            AppendChild(new Property(localName, ns) { Value = value });
        }

        public void AppendProperty(string localName, object value) {
            AppendProperty(localName, string.Empty, value);
        }

        public void AppendProperty(string localName) {
            AppendProperty(localName, null);
        }

        public void RemoveChildren(PropertyTreeNavigator lastSiblingToDelete) {
            throw new NotImplementedException();
        }

        public PropertyTreeWriter InsertAfter() {
            throw new NotImplementedException();
        }

        public PropertyTreeWriter InsertBefore() {
            throw new NotImplementedException();
        }

        public void InsertBefore(PropertyTreeNavigator newSibling) {
            throw new NotImplementedException();
        }

        internal string LookupNamespace(string prefix) {
            string result;
            if (prefixMap != null && prefixMap.TryGetValue(prefix, out result))
                return result;
            else if (this.Parent != null)
                return this.Parent.LookupNamespace(prefix);
            else
                return null;
        }

        public void CopyContentsTo(PropertyTree tree) {
            if (tree == null)
                throw new ArgumentNullException("tree");

            foreach (var child in this.Children) {
                tree.AppendChild(child.Clone());
            }
        }

        public abstract void CopyTo(PropertyNode node);

        internal void InitFrom(IXmlLineInfo lineInfo,
                               IUriContext uriContext,
                               IDictionary<string, string> prefixMap,
                               bool isExpressNamespace)
        {
            if (lineInfo != null) {
                LinePosition = lineInfo.LinePosition;
                LineNumber = lineInfo.LineNumber;
            }
            if (uriContext != null) {
                BaseUri = uriContext.BaseUri;
            }
            this.prefixMap = prefixMap;
            this.isExpressNamespace = isExpressNamespace;
        }
    }
}
