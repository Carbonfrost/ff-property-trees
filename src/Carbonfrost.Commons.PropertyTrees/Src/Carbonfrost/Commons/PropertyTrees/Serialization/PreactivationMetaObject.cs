//
// - PreactivationMetaObject.cs -
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
using System.ComponentModel;
using System.Reflection;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class PreactivationMetaObject : PropertyTreeMetaObject {

        private Type componentType;
        private OperatorDefinition factoryDefinition;

        public PreactivationMetaObject(Type componentType) {
            this.componentType = componentType;
        }

        public override Type ComponentType {
            get {
                return this.componentType;
            }
        }

        public override object Component {
            get {
                return null;
            }
        }

        public OperatorDefinition FactoryOperator {
            get {
                return factoryDefinition
                    ?? PropertyTreeDefinition.FromType(this.ComponentType).Constructor;
            }
        }

        public override PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value;
            TryConvertFromText(text, serviceProvider, out value);
            return PropertyTreeMetaObject.Create(value, this.ComponentType);
        }

        public override PropertyTreeMetaObject BindConstructor(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            var op = definition;
            object component = null;
            object parent = null;

            var value = op.Apply(component, parent, arguments);
            if (this.Parent == null)
                return Create(value);
            else
                return this.Parent.CreateChild(value);
        }

        public override PropertyTreeMetaObject BindTargetProvider(QualifiedName name, object criteria, IServiceProvider serviceProvider) {
            var domain = AppDomain.CurrentDomain;

            MemberInfo member = null;
            if (name != null)
                member = domain.GetProviderMember(this.ComponentType, name);
            else
                member = domain.GetProviderMember(this.ComponentType, criteria);

            if (name != null && name.Namespace.IsDefault) {
                member = member ?? domain.GetProviderMember(this.ComponentType, name.LocalName);
            }

            return SetProviderMember(member);
        }

        public override PropertyTreeMetaObject BindStreamingSource(StreamContext input, IServiceProvider serviceProvider) {
            var ss = StreamingSource.Create(this.ComponentType);
            var comp = ss.Load(input, this.ComponentType);
            return PropertyTreeMetaObject.Create(comp);
        }

        internal PropertyTreeMetaObject SetProviderMember(MemberInfo member) {
            if (member == null)
                return this;

            if (member.MemberType == MemberTypes.TypeInfo) {
                this.componentType = (Type) member;
                return this;

            } else if (member.MemberType == MemberTypes.Method) {
                this.factoryDefinition = ReflectedProviderFactoryDefinitionBase.Create(this.componentType, member);
                this.componentType = ((MethodInfo) member).ReturnType;
                return this;

            } else {
                var component = ((FieldInfo) member).GetValue(null);
                return this.Parent.CreateChild(component);
            }
        }

        public override void BindTargetType(TypeReference type, IServiceProvider serviceProvider) {
            // TODO Should only allow changing this component type via a target type bind once

            var newType = type.TryResolve();
            if (newType == null) {
                ProbeRuntimeComponents();

                newType = type.Resolve();
            }

            if (this.componentType.IsAssignableFrom(newType)) {
                this.componentType = newType;

            } else {
                throw Failure.NotAssignableFrom(newType, this.componentType);
            }

        }
    }
}
