//
// - PropertyTreeNodeReader.cs -
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
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    class PropertyTreeNodeReader : PropertyTreeReader {

        private PropertyNode node;
        private ReadState state;

        public PropertyTreeNodeReader(PropertyNode node) {
            this.node = node;
            this.state = ReadState.Initial;
        }

        public override object Value {
            get {
                Moved();
                return node.Value;
            }
        }

        public override bool Read() {
            switch (this.state) {
                case ReadState.Initial:
                    this.state = ReadState.Interactive;
                    return true;

                case ReadState.Interactive:
                    PropertyNode next = this.node;

                    while (next != null) {
                        if (next.NextSibling != null) {
                            this.node = next.NextSibling;
                            return true;
                        }

                        next = next.Parent;
                    }

                    return false;

                case ReadState.Error:
                case ReadState.EndOfFile:
                case ReadState.Closed:
                default:
                    return false;
            }
        }

        public override PropertyNodeType NodeType {
            get {
                Moved();
                return node.NodeType;
            }
        }

        public override string Namespace {
            get {
                Moved();
                return node.Namespace;
            }
        }

        public override string Name {
            get {
                Moved();
                return node.Name;
            }
        }

        public override int Depth {
            get {
                Moved();
                return node.Depth;
            }
        }

        public override PropertyNodeDefinition Definition {
            get {
                Moved();
                return node.Definition;
            }
        }

        public override ReadState ReadState {
            get {
                return state;
            }
        }

        public override int Position {
            get {
                Moved();
                return node.Position;
            }
        }

        public override bool HasChildren {
            get {
                Moved();
                return node.HasChildren;
            }
        }

        void Moved() {
            if (this.ReadState == ReadState.Initial)
                throw PropertyTreesFailure.ReaderNotMoved();
        }
    }
}
