//
// - CommonReflectionInfo.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class CommonReflectionInfo {

        private readonly MethodBase method;
        private readonly string name;
        private readonly string ns = string.Empty;
        private readonly PropertyDefinitionCollection parameters;
        private readonly OperatorDefinition def;

        public MethodBase Method { get { return method; } }
        public string Namespace { get { return ns; } }
        public string Name { get { return name; } }

        public PropertyDefinitionCollection Parameters {
            get {
                return parameters;
            }
        }

        public CommonReflectionInfo(OperatorDefinition def, RoleAttribute attr, MethodBase method) {
            this.method = method;
            this.def = def;
            this.name = attr.ComputeName(method);

            var type = method.DeclaringType;
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
                type = type.GetGenericTypeDefinition();

            this.ns = TypeHelper.GetNamespaceName(type);
            parameters = new PropertyDefinitionCollection();
            parameters.AddRange(def, ns, method.GetParameters(), method.IsExtension());
        }
    }
}

