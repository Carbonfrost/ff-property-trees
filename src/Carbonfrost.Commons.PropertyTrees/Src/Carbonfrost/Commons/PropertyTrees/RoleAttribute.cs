//
// - RoleAttribute.cs -
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
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    #pragma warning disable 3015

    // There are no accessible constructors for an attribute, but
    // it's abstract

    public abstract class RoleAttribute : Attribute {

        public string Name { get; set; }

        internal RoleAttribute() {}

        internal virtual void ProcessExtensionMethod(MethodInfo mi) {
        }

        internal virtual string ComputeName(MethodBase method) {
            return Name;
        }

        internal abstract OperatorDefinition BuildInstance(MethodInfo method);

    }
}
