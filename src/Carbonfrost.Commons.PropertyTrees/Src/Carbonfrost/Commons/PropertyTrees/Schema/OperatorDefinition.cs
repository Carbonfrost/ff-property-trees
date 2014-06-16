//
// - OperatorDefinition.cs -
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
using System.Collections.Generic;
using System.Reflection;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class OperatorDefinition : PropertyNodeDefinition {

        public abstract MethodBase UnderlyingMethod { get; }
        public abstract OperatorType OperatorType { get; }
        public abstract PropertyDefinitionCollection Parameters { get; }
        public abstract PropertyDefinition DefaultParameter { get; }

        internal object Apply(object component, object parent, IReadOnlyDictionary<string, PropertyTreeMetaObject> parameters) {
            // TODO Consider checking for the type instead of instantiating adapter (performance)
            return Apply(component, parent, new MetaArgumentAdapter(parameters));
        }

        public abstract object Apply(object component, object parent, IReadOnlyDictionary<string, object> parameters);

        internal static object[] MapParameters(MethodBase ctor, object parent, IReadOnlyDictionary<string, object> parameters) {
            ParameterInfo[] parms = ctor.GetParameters();
            int index = 0;
            object[] args = new object[parms.Length];

            if (ctor.IsExtension()) {
                args[0] = parent;
                index = 1;
            }

            for (; index < parms.Length; index++) {
                ParameterInfo p = parms[index];
                args[index] = parameters[p.Name];
            }

            return args;
        }
    }
}
