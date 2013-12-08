//
// - Property.cs -
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
using System.ComponentModel;
using System.Xml;

using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public class Property : PropertyNode {

        private object value;

        public event EventHandler ValueChanged;

        public Property() {}

        public Property(QualifiedName name) {
            this.QualifiedName = name;
        }

        public Property(string localName) {
            this.Name = localName;
        }

        public Property(string localName, string ns) {
            this.Name = localName;
            this.Namespace = ns;
        }

        // PropertyNode overrides
        public override object Value {
            get { return value; }
            set {
                // UNDONE Check the type
                if (this.value != value) {
                    this.value = value;
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnValueChanged(EventArgs e) {
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        // `PropertyNode' overrides.

        public override PropertyNodeCollection Children {
            get { return EmptyPropertyNodeCollection.Instance; } }

        // UNDONE Other attributes

        protected override object SelectAttributeCore(string attribute) {
            switch (attribute) {
                case "value":
                    return this.Value;
                case "position":
                    return (this.Parent == null) ? -1 : this.Parent.IndexOfChild(this);
                default:
                    return null;
            }
        }

        public override void CopyTo(PropertyNode node) {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.IsPropertyTree) {
                PropertyTreeWriter w = new PropertyTreeNodeWriter((PropertyTree) node);
                this.WriteTo(w);

            } else {
                // TODO Possible that Value isn't appropriate for Property
                Property p = (Property) node;
                p.QualifiedName = this.QualifiedName;
                p.Value = this.Value;
            }
        }

        public new Property Clone() {
            return new Property { QualifiedName = this.QualifiedName, Value = this.Value };
        }

        protected override PropertyNode CloneCore() {
            return Clone();
        }

        public override PropertyNodeType NodeType {
            get { return PropertyNodeType.Property; } }

        public override PropertyNodeDefinition Definition {
            get {
                throw new NotImplementedException();
            }
        }

        protected internal override void AcceptVisitor(PropertyTreeVisitor visitor) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            visitor.VisitProperty(this);
        }

        protected internal override TResult AcceptVisitor<TArgument, TResult>(PropertyTreeVisitor<TArgument, TResult> visitor, TArgument argument) {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            return visitor.VisitProperty(this, argument);
        }

    }
}
