//
// - ExtenderAttribute.cs -
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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.ComponentModel;

namespace Carbonfrost.Commons.PropertyTrees {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ExtenderAttribute : RoleAttribute {

        static readonly Regex PATTERN = new Regex(@"^(Get|Set)");

        internal override OperatorDefinition BuildInstance(MethodInfo method) {
            return null;
        }

        internal override void ProcessExtensionMethod(MethodInfo mi) {
            var target = (ReflectedPropertyTreeDefinition) PropertyTreeDefinition.FromType(
                mi.GetParameters()[0].ParameterType);

            var attachedID = GetAttachedPropertyID(mi);
            var existing = (ReflectedExtenderPropertyDefinition) target.GetProperty(attachedID);

            if (existing == null) {
                var extender = new ReflectedExtenderPropertyDefinition(attachedID);
                target.AddPropertyDefinition(extender);
                existing = extender;
            }

            existing.AddMethod(mi);
        }

        internal AttachedPropertyID GetAttachedPropertyID(MethodInfo method) {
            string name = string.IsNullOrEmpty(Name) ? PATTERN.Replace(method.Name, string.Empty) : Name;
            return new AttachedPropertyID(method.DeclaringType, name);
        }
    }
}
