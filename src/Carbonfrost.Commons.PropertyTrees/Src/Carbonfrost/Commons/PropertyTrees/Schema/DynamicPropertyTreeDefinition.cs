//
// - DynamicPropertyTreeDefinition.cs -
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
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public class DynamicPropertyTreeDefinition : PropertyTreeDefinition {

        // TODO Implement dynamic schemata

        public override PropertyDefinition DefaultProperty {
            get {
                throw new NotImplementedException();
            }
        }

        public override PropertyTreeSchema Schema {
            get {
                throw new NotImplementedException();
            }
        }

        public override OperatorDefinitionCollection Operators {
            get {
                throw new NotImplementedException();
            }
        }

        public override Type SourceClrType {
            get {
                throw new NotImplementedException();
            }
        }

        public override PropertyTreeFactoryDefinition Constructor {
            get {
                throw new NotImplementedException();
            }
        }

        public override string Namespace {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override string Name {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override OperatorDefinition GetOperator(QualifiedName name, bool declaredOnly = false) {
            throw new NotImplementedException();
        }

        public override PropertyDefinitionCollection Properties {
            get {
                throw new NotImplementedException();
            }
        }

        public override PropertyDefinition GetProperty(QualifiedName name, bool declaredOnly = false) {
            throw new NotImplementedException();
        }

        public override IEnumerable<PropertyDefinition> EnumerateProperties(bool declaredOnly = false) {
            throw new NotImplementedException();
        }

        public override IEnumerable<OperatorDefinition> EnumerateOperators(bool declaredOnly = false) {
            throw new NotImplementedException();
        }

        public override PropertyDefinition GetProperty(string name, bool declaredOnly = false) {
            throw new NotImplementedException();
        }

        public override OperatorDefinition GetOperator(string name, bool declaredOnly = false) {
            throw new NotImplementedException();
        }
    }
}
