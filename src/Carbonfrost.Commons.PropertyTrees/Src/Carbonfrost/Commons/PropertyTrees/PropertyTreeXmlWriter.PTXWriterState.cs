//
// - PropertyTreeXmlWriter.PTXWriterState.cs -
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
using System.IO;
using System.Text;
using System.Xml;

using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class PropertyTreeXmlWriter {

        abstract class PTXWriterState {
            public abstract PTXWriterStateType State { get; }
            public PTXWriterState Parent;

            protected PTXWriterState(PTXWriterState parent) {
                this.Parent = parent;
            }

            public abstract void Exit(PropertyTreeXmlWriter writer);
            public abstract void Enter(PropertyTreeXmlWriter writer);

            // TODO Schema conforming should also be tested here
            public abstract PTXWriterState CreateTreeChild(string ns, string name);
            public abstract PTXWriterState CreatePropertyChild(string ns, string name);
        }

        class RootState : PTXWriterState {

            public RootState() : base(null) {}

            public override PTXWriterStateType State {
                get { return PTXWriterStateType.Root; } }

            public override void Exit(PropertyTreeXmlWriter writer) {
                writer.xmlWriter.WriteEndDocument();
            }

            public override void Enter(PropertyTreeXmlWriter writer) {
                writer.xmlWriter.WriteStartDocument();
                writer.xmlWriter.WriteRaw(Environment.NewLine);
            }

            public override PTXWriterState CreateTreeChild(string ns, string name) {
                return new TreeState(this, ns, name);
            }

            public override PTXWriterState CreatePropertyChild(string ns, string name) {
                throw PropertyTreesFailure.WouldCreateMalformedDocumentRootRequired();
            }
        }

        class PropertyState : PTXWriterState {

            private readonly Dictionary<QualifiedName, string> pendingMetadata = new Dictionary<QualifiedName, string>();
            private readonly string ns;
            private readonly string name;

            public PropertyState(TreeState parent, string ns, string name) : base(parent) {
                this.ns = ns;
                this.name = name;
            }

            public override PropertyTreeXmlWriter.PTXWriterStateType State {
                get { return PTXWriterStateType.Property; } }

            public override PTXWriterState CreateTreeChild(string ns, string name) {
                QualifiedName qn = QualifiedName.Create(ns, name);
                if (ns != Xmlns.PropertyTrees2010)
                    throw PropertyTreesFailure.WriterOnlyPropertyTreesNamespace();

                if (pendingMetadata.ContainsKey(qn))
                    throw PropertyTreesFailure.WriterPropertyOrTreeExists(qn);

                // TODO Support schema trees (may have to store them as PTrees)
                throw new NotImplementedException();
            }

            public override PTXWriterState CreatePropertyChild(string ns, string name) {
                QualifiedName qn = QualifiedName.Create(ns, name);
                if (pendingMetadata.ContainsKey(qn))
                    throw PropertyTreesFailure.WriterPropertyOrTreeExists(qn);

                // TODO Support schema trees (may have to store them as PTrees)
                throw new NotImplementedException();
            }

            public override void Exit(PropertyTreeXmlWriter writer) {
                // TODO Ensure value is present
                if (!pendingMetadata.ContainsKey(PropertyAttribute.Value))
                    throw new NotImplementedException();

                if (pendingMetadata.Count > 1) {
                    // TODO Write composite properties
                    foreach (var kvp in this.pendingMetadata) {
                    }
                    throw new NotImplementedException();
                } else {
                    writer.xmlWriter.WriteAttributeString(this.name, this.ns, pendingMetadata[PropertyAttribute.Value]);
                }
            }

            public override void Enter(PropertyTreeXmlWriter writer) {
            }

            public void AppendPropertyValue(string value) {
                string existingValue;
                if (this.pendingMetadata.TryGetValue(PropertyAttribute.Value, out existingValue))
                    value = existingValue + value;

                this.pendingMetadata[PropertyAttribute.Value] = value;
            }
        }

        class TreeState : PTXWriterState {

            private readonly string ns;
            private readonly string name;

            public TreeState(PTXWriterState parent, string ns, string name) : base(parent) {
                this.ns = ns;
                this.name = name;
            }

            public override PropertyTreeXmlWriter.PTXWriterStateType State {
                get { return PTXWriterStateType.Tree; } }

            public override PTXWriterState CreateTreeChild(string ns, string name) {
                return new TreeState(this, ns, name);
            }

            public override PTXWriterState CreatePropertyChild(string ns, string name) {
                return new PropertyState(this, ns, name);
            }

            public override void Exit(PropertyTreeXmlWriter writer) {
                writer.xmlWriter.WriteEndElement();
            }

            public override void Enter(PropertyTreeXmlWriter writer) {
                // Can always start a tree
                writer.xmlWriter.WriteStartElement(name, ns);
            }
        }

        enum PTXWriterStateType {
            Root,
            Property,
            Tree,
        }

    }
}
