//
// - PropertyNameLookupHelper.cs -
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
using System.Linq;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class PropertyNameLookupHelper : IPropertyNameLookupHelper {

        public OperatorDefinition FindOperator(PropertyTreeDefinition definition, Type type, QualifiedName qn) {
            return definition
                .EnumerateOperators()
                .FirstOrDefault(t => Compare(t, qn, definition));
        }

        public PropertyDefinition FindProperty(PropertyTreeDefinition definition,
                                               Type componentType,
                                               QualifiedName qn,
                                               IEnumerable<PropertyTreeDefinition> ancestors) {

            // Allow any namespace contained in the definition base classes
            var result = definition
                .EnumerateProperties()
                .FirstOrDefault(t => Compare(t, qn, definition));

            if (result != null)
                return result;

            int dot = qn.LocalName.IndexOf('.');
            if (dot > 0) {
                // TODO Index whether the PTD has extenders so we can skip some ancestors (perf)
                string prefix = qn.LocalName.Substring(0, dot);

                foreach (var currentDef in ancestors) {
                    if (currentDef.Name == prefix) {
                        // TODO Local name could be different
                        var prop = currentDef.GetProperty(qn);
                        if (prop != null) {
                            return prop;
                        }
                    }
                }

            } else {

                foreach (var curDefinition in ancestors) {
                    var prop = curDefinition.GetProperty(qn);
                    if (IsValidExtender(prop, componentType))
                        return prop;

                    var qn2 = qn.ChangeLocalName(curDefinition.Name + "." + qn.LocalName);
                    prop = curDefinition.GetProperty(qn2);
                    if (IsValidExtender(prop, componentType))
                        return prop;

                }
            }

            return null;
        }

        private bool IsValidExtender(PropertyDefinition prop, Type componentType) {
            return prop != null && prop.IsExtender && prop.CanExtend(componentType);
        }

        private static bool Compare(PropertyNodeDefinition t,
                                    QualifiedName qn,
                                    PropertyTreeDefinition definition) {
            var x = t.QualifiedName;
            var y = qn;
            if (x == y)
                return true;

            if (string.Equals(x.LocalName, y.LocalName, StringComparison.OrdinalIgnoreCase))
                return x.Namespace == y.Namespace
                    || x.Namespace == string.Empty
                    || (definition.GetSerializationCandidateNamespaces().Any(m => m == y.Namespace));
            else
                return false;
        }

    }
}
