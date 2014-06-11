//
// - BuilderMetaObject.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class BuilderMetaObject : PropertyTreeMetaObject {

        private readonly PropertyTreeMetaObject inner;

        public BuilderMetaObject(PropertyTreeMetaObject inner) {
            this.inner = inner;
        }

        public override Type ComponentType {
            get {
                return inner.ComponentType;
            }
        }

        public override object Component {
            get {
                return inner.Component;
            }
        }

        public override PropertyTreeMetaObject BindEndObject(IServiceProvider serviceProvider) {
            return PropertyTreeMetaObject.Create(inner.Component.InvokeBuilder(serviceProvider));
        }

        public override PropertyTreeMetaObject BindConstructor(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            return new BuilderMetaObject(inner.BindConstructor(definition, arguments));
        }

        public override void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, IServiceProvider serviceProvider) {
            inner.BindSetMember(property, name, value, serviceProvider);
        }
    }
}
