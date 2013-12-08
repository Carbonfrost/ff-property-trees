//
// - AddAttribute.cs -
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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class AddAttribute : RoleAttribute {

        static readonly Regex PATTERN = new Regex(@"(Builder)?Collection(`\d+)?$");

        internal static readonly AddAttribute Default = new AddAttribute();
        internal static readonly AddAttribute Natural = new AddAttribute();

        public AddAttribute() {}

        internal override void ProcessExtensionMethod(MethodInfo mi) {
            var target = (ReflectedPropertyTreeDefinition) PropertyTreeDefinition.FromType(
                mi.GetParameters()[0].ParameterType);

            target.AddFactoryDefinition(ReflectedPropertyTreeFactoryDefinition.FromFactory(this, mi));
        }

        internal override string ComputeName(MethodBase method) {
            if (string.IsNullOrEmpty(Name)) {
                if (object.ReferenceEquals(this, Natural))
                    return GetNaturalName(method.DeclaringType);
                else
                    return TrimImplicitAdd(method.Name, method.DeclaringType);

            } else
                return Name;
        }

        internal override OperatorDefinition BuildInstance(MethodInfo method) {
            return ReflectedPropertyTreeFactoryDefinition.FromFactory(this, method);
        }

        internal static string TrimImplicitAdd(string name, Type declaringType) {
            if (name.Length > 3 && name.StartsWith("Add", StringComparison.Ordinal) && char.IsUpper(name[3])) {
                string result = name.Substring(3);

                if (result == "New") {
                    result = GetNaturalName(declaringType);
                }

                return Utility.CamelCase(result);

            } else
                return name;
        }

        internal static string GetNaturalName(Type declaringType) {
            if (declaringType.IsGenericType) {

                foreach (var m in declaringType.GetInterfaces()) {

                    if (m.IsGenericType) {
                        Type def = m.GetGenericTypeDefinition();

                        if (def.Equals(typeof(ICollection<>))) {
                            Type type = m.GetGenericArguments()[0];

                            if (type.IsGenericType)
                                return null;
                            else
                                return type.Name;
                        }
                    }
                }

                return null;
            } else
                return PATTERN.Replace(declaringType.Name, string.Empty);
        }
    }

}
