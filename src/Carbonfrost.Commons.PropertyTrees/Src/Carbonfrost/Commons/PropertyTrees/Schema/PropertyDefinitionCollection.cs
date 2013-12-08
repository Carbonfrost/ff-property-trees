//
// - PropertyDefinitionCollection.cs -
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
using System.Linq;
using System.Reflection;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public class PropertyDefinitionCollection : PropertyNodeDefinitionCollection<PropertyDefinition> {

        // UNDONE Store or support retrieving in bind order.
        // IEnumerator<PropertyDescriptor> props = Adaptable.GetBindOrder(componentType);
        // IRuntimeComponent.Dependencies is always first

        internal PropertyDefinitionCollection(IEnumerable<PropertyDefinition> items) : base() {
            foreach (var o in items)
                Add(o);
        }

        public PropertyDefinitionCollection() : base() {}

        internal PropertyDefinition FirstParameter {
            get; private set;
        }

        internal void AddRange(OperatorDefinition owner, string ns, ParameterInfo[] parameters, bool extension) {
            foreach (var parm in parameters) {
                if (extension && parm.Position == 0)
                    continue;

                this.AddInternal(new ReflectedParameterDefinition(ns, parm));
            }
            this.FirstParameter = this.FirstOrDefault();
        }
    }
}
