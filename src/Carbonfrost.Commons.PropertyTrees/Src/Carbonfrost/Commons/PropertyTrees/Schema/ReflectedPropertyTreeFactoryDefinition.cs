//
// - ReflectedPropertyTreeFactoryDefinition.cs -
//
// Copyright 2010, 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    abstract class ReflectedPropertyTreeFactoryDefinition : PropertyTreeFactoryDefinition {

        private readonly Type outputType;
        private readonly CommonReflectionInfo info;

        protected ReflectedPropertyTreeFactoryDefinition(AddAttribute attr, MethodBase method, Type outputType) {
            this.info = new CommonReflectionInfo(this, attr ?? AddAttribute.Default, method);
            this.outputType = outputType;

            Type returnType;
            ConstructorInfo ctor = method as ConstructorInfo;
            if (ctor == null)
                returnType = ((MethodInfo) method).ReturnType;
            else
                returnType = ctor.DeclaringType;
        }

        public static ReflectedPropertyTreeFactoryDefinition Create(AddAttribute attr, MethodBase ctor) {
            if (ctor.IsConstructor)
                return new ConstructorFactory(attr, ctor);
            else
                return FromFactory(attr, (MethodInfo) ctor);
        }

        public static ReflectedPropertyTreeFactoryDefinition FromListAddMethod(MethodInfo method, bool naturalName = false) {
            if (naturalName) {

                if (string.IsNullOrEmpty(AddAttribute.GetNaturalName(method.DeclaringType))) {
                    return null;

                } else {
                    return new ListAddMethod(method, AddAttribute.Natural);
                }

            } else
                return new ListAddMethod(method, null);
        }

        public static ReflectedPropertyTreeFactoryDefinition FromFactory(AddAttribute attribute, MethodInfo method) {
            if (method.IsStatic)
                return new FactoryExtensionMethod(attribute, method);
            else
                return new FactoryMethod(attribute, method);
        }

        static Type GetReturnType(MethodBase method) {
            if (method.MemberType == MemberTypes.Constructor)
                return method.DeclaringType;
            else
                return ((MethodInfo) method).ReturnType;
        }

        public override Type OutputType {
            get { return this.outputType; } }

        public override PropertyDefinitionCollection Parameters {
            get { return info.Parameters; } }

        public override MethodBase UnderlyingMethod {
            get { return info.Method; } }

        public override string Namespace {
            get { return info.Namespace; }
        }

        public override string Name {
            get { return info.Name; }
        }

        public override string ToString() {
            return OutputType.ToString();
        }

        sealed class ConstructorFactory : ReflectedPropertyTreeFactoryDefinition {

            public ConstructorFactory(AddAttribute attr, MethodBase method) : base(attr, method, GetReturnType(method)) {
            }

            public override PropertyDefinition DefaultParameter { get { return null; } }

            public override object Apply(object component, object parent, IReadOnlyDictionary<string, object> parameters) {
                var parms = MapParameters(UnderlyingMethod, parent, parameters);
                return ((ConstructorInfo) UnderlyingMethod).Invoke(parms);
            }

        }

        sealed class FactoryMethod : ReflectedPropertyTreeFactoryDefinition {

            public FactoryMethod(AddAttribute attr, MethodBase method) : base(attr, method, GetReturnType(method)) {
            }

            public override PropertyDefinition DefaultParameter {
                get {
                    return null;
                }
            }

            public override object Apply(object component, object parent, IReadOnlyDictionary<string, object> parameters) {
                var parms = MapParameters(UnderlyingMethod, parent, parameters);
                return UnderlyingMethod.Invoke(parent, parms);
            }
        }

        sealed class FactoryExtensionMethod : ReflectedPropertyTreeFactoryDefinition {

            public FactoryExtensionMethod(AddAttribute attr, MethodBase method)
                : base(attr, method, GetReturnType(method)) {
            }

            public override PropertyDefinition DefaultParameter {
                get {
                    return null;
                }
            }

            public override object Apply(object component, object parent, IReadOnlyDictionary<string, object> parameters) {
                object[] items = MapParameters(UnderlyingMethod, parent, parameters);
                return UnderlyingMethod.Invoke(null, items);
            }
        }

        sealed class ListAddMethod : ReflectedPropertyTreeFactoryDefinition {

            public ListAddMethod(MethodBase method, AddAttribute attr)
                : base(attr, method, method.GetParameters()[0].ParameterType) {
            }

            public override PropertyDefinition DefaultParameter {
                get {
                    return Parameters.FirstParameter;
                }
            }

            public override object Apply(object component, object parent, IReadOnlyDictionary<string, object> parameters) {
                object[] mappedParameters = MapParameters(UnderlyingMethod, parent, parameters);
                UnderlyingMethod.Invoke(component, mappedParameters);
                return mappedParameters[0];
            }

        }

    }

}

