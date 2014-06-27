//
// - PropertyTreeNodeWriter.cs -
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
using System.Text;
using System.Xml;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeNodeWriter : PropertyTreeWriter {

        // TODO Additional type blessing plus metadata
        // TODO support all metadata extensions
        // TODO Parsing handling should deal with invalid values based
        //   on policy (either throw or handle on an error callback)
        // TODO Could have nested metadata (which implies this ought
        // to work like a PropertyTree then bind)

        private PropertyNode currentParent;
        private PropertyTree root;
        private PropertyBuilder property;
        private bool started;
        private IXmlLineInfo lineInfo;
        private IDictionary<string, string> prefixMap;
        private bool isExpressNamespace;

        public PropertyTree Root {
            get {
                return this.root;
            }
        }

        public PropertyTreeNodeWriter() {}

        public PropertyTreeNodeWriter(PropertyTree tree) {
            if (tree == null)
                throw new ArgumentNullException("tree");

            this.currentParent = tree;
            this.root = tree;
        }

        // PropertyTreeWriter
        public override PropertyTreeWriterSettings Settings {
            get {
                throw new NotImplementedException();
            }
        }

        public override WriteState WriteState {
            get {
                if (this.IsDisposed)
                    return WriteState.Closed;

                if (currentParent == null)
                    return started ? WriteState.Start : WriteState.Prolog;

                else if (property == null)
                    return WriteState.Tree;

                else if (property.TextLength > 0)
                    return WriteState.Property;

                else
                    return WriteState.PropertyContent;
            }
        }

        public override void WriteStartTree(string localName, string ns) {
            Require.NotNullOrEmptyString("localName", localName);
            StartImplicitly();
            PropertyTree newTree = new PropertyTree();
            newTree.Name = localName;
            newTree.Namespace = ns;
            CopyLineInfo(newTree);

            if (this.root == null) {
                this.root = newTree;
                this.currentParent = newTree;
            } else
                PushParent(newTree);
        }

        public override void WriteStartProperty(string localName, string ns) {
            Require.NotNullOrEmptyString("localName", localName);
            if (currentParent == null)
                throw PropertyTreesFailure.WouldCreateMalformedDocumentRootRequired();

            this.property = new PropertyBuilder(ns, localName);
            CopyLineInfo(property.Property);
            PushParent(this.property.Property);
        }

        public override void WriteStartDocument() {
            this.started = true;
        }

        public override void WritePropertyValue(string value) {
            if (this.property == null)
                throw PropertyTreesFailure.WouldCreateMalformedDocument();

            this.property.AppendValue(value);
        }

        public override void WriteEndTree() {
            if (currentParent == null)
                throw PropertyTreesFailure.WouldCreateMalformedDocument();

            PopParent();
        }

        public override void WriteEndProperty() {
            if (property == null || currentParent == null)
                throw PropertyTreesFailure.WouldCreateMalformedDocument();

            this.property.End();
            this.property = null;
            PopParent();
        }

        public override void WriteEndDocument() {
            this.currentParent = null;
        }

        public override void WriteComment(string comment) {}

        public override void Flush() {
        }

        internal override void SetExpressNamespace(bool isExpressNamespace) {
            this.isExpressNamespace = isExpressNamespace;
        }

        internal override void SetLineInfo(IXmlLineInfo lineInfo, IDictionary<string, string> prefixMap) {
            this.lineInfo = lineInfo;
            this.prefixMap = prefixMap;
        }

        private void StartImplicitly() {
            if (this.started)
                return;
            WriteStartDocument();
        }

        private void PushParent(PropertyNode newParent) {
            if (this.currentParent == null)
                throw PropertyTreesFailure.WouldCreateMalformedDocument();

            this.currentParent.AppendChild(newParent);
            this.currentParent = newParent;
        }

        private void PopParent() {
            this.currentParent = this.currentParent.Parent;
        }

        private void CopyLineInfo(PropertyNode node) {
            if (lineInfo != null) {
                node.LinePosition = lineInfo.LinePosition;
                node.LineNumber = lineInfo.LineNumber;
            }
            node.prefixMap = this.prefixMap;
            node.isExpressNamespace = this.isExpressNamespace;
        }
    }
}
