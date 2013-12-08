//
// - PropertyTreeXmlReader.PTXReaderState.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.PropertyTrees.Schema;
using XReadState = System.Xml.ReadState;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class PropertyTreeXmlReader {

        internal abstract class PTXReaderState {

            internal static readonly PTXReaderState Element = new ElementState();
            internal static readonly PTXReaderState Initial = new InitialState();
            internal static readonly PTXReaderState EndElement = new EndElementState();
            internal static readonly PTXReaderState Attribute = new AttributeState();
            internal static readonly PTXReaderState EndEmptyElement = new EndElementState();
            internal static readonly PTXReaderState EndStartElement = new EndStartElementState();
            internal static readonly PTXReaderState EOF = new EOFState();

            public bool IsEOF { get { return this is EOFState; } }

            public abstract PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader);

            internal static PTXReaderState AnyState(PropertyTreeXmlReader r, XmlReader reader) {
                while (reader.Read()) {
                    return ChooseState(reader);
                }

                return EOF;
            }

            internal static PTXReaderState ChooseState(XmlReader reader) {
                switch (reader.MoveToContent()) {
                    case XmlNodeType.Element:
                        return Element;

                    case XmlNodeType.EndElement:
                        return EndElement;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:


                    case XmlNodeType.Comment:
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.Notation:
                    default:
                        return EOF;
                }
            }

            internal bool MoveToNextAttribute(PropertyTreeXmlReader r, XmlReader reader) {
                while (reader.MoveToNextAttribute()) {
                    if (IsXmlnsDefinition(reader)) {
                        r.ExportNamespace();
                    } else {
                        return true;
                    }
                }

                return false;
            }

            internal bool MoveToFirstAttribute(PropertyTreeXmlReader r, XmlReader reader) {
                if (reader.MoveToFirstAttribute()) {
                    if (IsXmlnsDefinition(reader)) {
                        r.ExportNamespace();
                        return MoveToNextAttribute(r, reader);

                    } else {
                        return true;
                    }
                }

                return false;
            }

            private static bool IsXmlnsDefinition(XmlReader reader) {
                return reader.Prefix == "xmlns" || reader.Name == "xmlns";
            }
        }

        class EndStartElementState : PTXReaderState {

            // On element, after attributes
            public override PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader) {
                if (reader.IsEmptyElement) {
                    r.PushState(PropertyNodeType.EndPropertyTree);
                    return AnyState(r, reader);
                }

                return AnyState(r, reader).Accept(r, reader);
            }
        }

        class EndElementState : PTXReaderState {

            public override PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader) {
                r.PushState(PropertyNodeType.EndPropertyTree);
                return AnyState(r, reader);
            }
        }

        class InitialState : PTXReaderState {

            public override PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader) {
                return AnyState(r, reader).Accept(r, reader);
            }
        }

        class ElementState : PTXReaderState {

            public override PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader) {
                r.PushState(PropertyNodeType.PropertyTree);

                if (MoveToFirstAttribute(r, reader))
                    return Attribute;

                else if (reader.IsEmptyElement)
                    return EndEmptyElement;

                else
                    return EndStartElement;
            }

        }

        class EOFState : PTXReaderState {

            public override PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader) {
                throw new NotImplementedException();
            }
        }

        class AttributeState : PTXReaderState {

            public override PTXReaderState Accept(PropertyTreeXmlReader r, XmlReader reader) {
                r.PushState(PropertyNodeType.Property);

                if (MoveToNextAttribute(r, reader)) {
                    return this;

                } else {
                    reader.MoveToElement();
                    return PTXReaderState.EndStartElement;
                }

            }
        }

    }
}

