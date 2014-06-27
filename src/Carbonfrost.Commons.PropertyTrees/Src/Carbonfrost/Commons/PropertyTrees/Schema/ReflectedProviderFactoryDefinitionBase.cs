//
// - ReflectedProviderFactoryDefinitionBase.cs -
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
using System.Reflection;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    abstract class ReflectedProviderFactoryDefinitionBase : PropertyTreeFactoryDefinition {

        private readonly Type outputType;
        private readonly QualifiedName qname;
        private readonly PropertyDefinitionCollection parameters;
        private readonly MethodBase method;

        protected ReflectedProviderFactoryDefinitionBase(MethodBase method,
                                                         QualifiedName qname,
                                                         Type outputType) {
            this.qname = qname;
            this.outputType = outputType;
            this.method = method;

            this.parameters = new PropertyDefinitionCollection();
            parameters.AddRange(this, qname.NamespaceName, method.GetParameters(), method.IsExtension());
        }

        public sealed override Type OutputType {
            get { return this.outputType; } }

        public sealed override PropertyDefinitionCollection Parameters {
            get { return this.parameters; } }

        public sealed override MethodBase UnderlyingMethod {
            get { return this.method; } }

        public override string Namespace {
            get { return qname.NamespaceName; }
        }

        public override string Name {
            get { return qname.LocalName; }
        }

        public override PropertyDefinition DefaultParameter {
            get {
                return null;
            }
        }

        public static ReflectedProviderFactoryDefinitionBase Create(Type providerType, MemberInfo concrete) {
            if (concrete.MemberType == MemberTypes.TypeInfo)
                return new ProviderTypeFactoryDefinition(providerType, (Type) concrete);

            else if (concrete.MemberType == MemberTypes.Method)
                return new ProviderMethodFactoryDefinition(providerType, (MethodInfo) concrete);

            else
                throw new NotImplementedException();
        }

        private object DoAddChild(object parent, object child) {
            var addChild = parent as IAddChild;
            if (addChild != null)
                addChild.AddChild(child);

            return child;
        }

        sealed class ProviderTypeFactoryDefinition : ReflectedProviderFactoryDefinitionBase {

            private readonly Type concreteProviderType;
            private readonly Type providerType;

            public ProviderTypeFactoryDefinition(Type providerType, Type concreteProviderType)
                : base(concreteProviderType.GetActivationConstructor(),
                       AppDomain.CurrentDomain.GetProviderName(providerType, concreteProviderType),
                       concreteProviderType)
            {
                this.providerType = providerType;
                this.concreteProviderType = concreteProviderType;
            }

            public override object Apply(object component,
                                         object parent,
                                         IReadOnlyDictionary<string, object> parameters) {

                var parms = MapParameters(UnderlyingMethod, parent, parameters);
                return DoAddChild(parent, ((ConstructorInfo) UnderlyingMethod).Invoke(parms));
            }
        }

        sealed class ProviderMethodFactoryDefinition : ReflectedProviderFactoryDefinitionBase {

            private readonly Type providerType;

            public ProviderMethodFactoryDefinition(Type providerType, MethodInfo providerFactoryMethod)
                : base(providerFactoryMethod,
                       AppDomain.CurrentDomain.GetProviderName(providerType, providerFactoryMethod),
                       providerFactoryMethod.ReturnType)
            {
                this.providerType = providerType;
            }

            public override object Apply(object component,
                                         object parent,
                                         IReadOnlyDictionary<string, object> parameters) {

                var parms = MapParameters(UnderlyingMethod, parent, parameters);
                return DoAddChild(parent, UnderlyingMethod.Invoke(null, parms));
            }

        }
    }

}
