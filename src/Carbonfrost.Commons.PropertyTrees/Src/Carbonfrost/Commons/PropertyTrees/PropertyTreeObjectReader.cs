//
// - PropertyTreeObjectReader.cs -
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
using System.Runtime.Serialization;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeObjectReader : PropertyTreeReader {

        private readonly ObjectIDGenerator generator;
        private readonly object graph;

        public PropertyTreeObjectReader(object graph) {
            if (graph == null)
                throw new ArgumentNullException("graph");
            
            this.generator = new ObjectIDGenerator();
            this.graph = graph;
        }

        // TODO Support object reading graphness

        public override bool Read() {
            throw new NotImplementedException();
        }

        public override object Value {
            get {
                throw new NotImplementedException();
            }
        }

        public override PropertyNodeType NodeType {
            get {
                throw new NotImplementedException();
            }
        }

        public override string Namespace {
            get {
                throw new NotImplementedException();
            }
        }

        public override string Name {
            get {
                throw new NotImplementedException();
            }
        }

        public override int Depth {
            get {
                throw new NotImplementedException();
            }
        }

        public override PropertyNodeDefinition Definition {
            get {
                throw new NotImplementedException();
            }
        }

        public override ReadState ReadState {
            get {
                throw new NotImplementedException();
            }
        }

        public override int Position {
            get {
                throw new NotImplementedException();
            }
        }

        public override bool HasChildren {
            get {
                throw new NotImplementedException();
            }
        }
        
    }
    
}

