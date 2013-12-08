//
// - PropertyTreeXmlWriter.cs -
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

    public partial class PropertyTreeXmlWriter : PropertyTreeWriter {

        private WriteState thisState;
        private readonly XmlWriter xmlWriter;
        private readonly PropertyTreeWriterSettings settings;
        private readonly Stack<PTXWriterState> states = new Stack<PTXWriterState>();

        internal PropertyTreeXmlWriter(XmlWriter xmlWriter, PropertyTreeWriterSettings settings) {
            this.xmlWriter = xmlWriter;
            this.settings = settings ?? new PropertyTreeWriterSettings();
        }

        public static PropertyTreeXmlWriter Create(string outputFileName, PropertyTreeWriterSettings settings = null) {
            return PropertyTreeWriter.CreateXml(outputFileName, settings);
        }

        public static PropertyTreeXmlWriter Create(Stream outputStream, Encoding encoding = null, PropertyTreeWriterSettings settings = null) {
            return PropertyTreeWriter.CreateXml(outputStream, encoding, settings);
        }

        public static PropertyTreeXmlWriter Create(TextWriter outputWriter, PropertyTreeWriterSettings settings = null) {
            return PropertyTreeWriter.CreateXml(outputWriter, settings);
        }

        public static PropertyTreeXmlWriter Create(XmlWriter outputWriter, PropertyTreeWriterSettings settings = null) {
            return PropertyTreeWriter.CreateXml(outputWriter, settings);
        }

        // PropertyTreeWriter overrides
        public override PropertyTreeWriterSettings Settings {
            get {
                return this.settings;
            }
        }

        public override WriteState WriteState {
            get { return thisState; } }

        public override void Flush() {
            this.xmlWriter.Flush();
        }

        public override void WriteStartTree(string localName, string ns) {
            Require.NotNullOrEmptyString("localName", localName);
            Guard();

            if (this.states.Count == 0)
                WriteStartDocument();

            PushState(this.states.Peek().CreateTreeChild(ns, localName));
            this.thisState = WriteState.Tree;
        }

        public override void WriteStartProperty(string localName, string ns) {
            Require.NotNullOrEmptyString("localName", localName);
            Guard();
            PushState(this.states.Peek().CreatePropertyChild(ns, localName));
            this.thisState = WriteState.Property;
        }

        public override void WritePropertyValue(string value) {
            Guard();
            PropertyState state = (PropertyState) RequirePeek(PTXWriterStateType.Property);
            state.AppendPropertyValue(value);
            this.thisState = WriteState.PropertyContent;
        }

        public override void WriteEndProperty() {
            Guard();
            RequirePop(PTXWriterStateType.Property);

            // TODO Is there a difference here (couldn't we be positioned after the root node??)
            this.thisState = this.states.Peek().State == PTXWriterStateType.Root ? WriteState.Tree : WriteState.Tree;
        }

        public override void WriteEndTree() {
            Guard();
            RequirePop(PTXWriterStateType.Tree);

            if (this.states.Count == 0)
                this.thisState = WriteState.Error; // FIXME USe the correct value
            else
                this.thisState = WriteState.Tree;
        }

        public override void WriteComment(string comment) {
            Guard();
            // Comments are pushed ahead of properties if they are written within them
            // TODO Ensure that this behavior is tested
            this.xmlWriter.WriteComment(comment);
        }

        public override void WriteStartDocument() {
            Guard();
            PushState(new RootState());
        }

        public override void WriteEndDocument() {
            Guard();
            while (this.states.Count > 0)
                PopState();
        }

        protected override void Dispose(bool manualDispose) {
            if (manualDispose) {
                this.xmlWriter.Close();
            }

            this.thisState = WriteState.Closed;
            base.Dispose(manualDispose);
        }


        PTXWriterState RequirePeek(PTXWriterStateType requiredType) {
            if (this.states.Peek().State == requiredType) {
                return this.states.Peek();
            } else
                throw PropertyTreesFailure.WriterIncorrectState(this.WriteState);
        }

        PTXWriterState RequirePop(PTXWriterStateType requiredType) {
            if (this.states.Peek().State == requiredType)
                return PopState();
            else
                throw PropertyTreesFailure.WouldCreateMalformedDocument();

        }

        PTXWriterState PopState() {
            var result = this.states.Pop();
            result.Exit(this);
            return result;
        }

        void PushState(PTXWriterState state) {
            this.states.Push(state);
            state.Enter(this);
        }

        void Guard() {
            // TODO Is this how XML handles it?
            // "Writer is in the error state."
            ThrowIfDisposed();
        }

    }
}
