//
// - ReflectedPropertyTreeSchema.cs -
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
using System.Reflection;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime.Components;
using System.Linq;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    class ReflectedPropertyTreeSchema : PropertyTreeSchema {

        private readonly Assembly assembly;
        private readonly PropertyTreeDefinitionCollection types;

        public ReflectedPropertyTreeSchema(Assembly assembly) {
            this.assembly = assembly;
            // TODO Lazy load these in a derived class of PropertyTreeDefinitionCollection
        }

        // `PropertyTreeSchema' implementation
        public override PropertyTreeDefinitionCollection Types {
            get {
                return types;
            }
        }

        public override Assembly SourceAssembly {
            get {
                return this.assembly;
            }
        }

        public override ComponentName SchemaName {
            get {
                throw new NotImplementedException();
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override NamespaceUri DefaultNamespace {
            get {
                throw new NotImplementedException();
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

    }
}
