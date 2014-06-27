//
// - SetPropertyCommand.cs -
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class TemplateMetaObject {

        [DebuggerDisplay("{property} = {value}")]
        sealed class SetPropertyCommand : ITemplateCommand {

            readonly PropertyDefinition property;
            readonly QualifiedName name;
            readonly object value;

            public SetPropertyCommand(PropertyDefinition property, QualifiedName name, object value) {
                this.property = property;
                this.name = name;
                this.value = value;
            }

            void ITemplateCommand.Apply(Stack<object> values) {
                object component = values.Peek();
                if (property.IsReadOnly) {
                    TryAggregation(component);
                    return;
                }

                property.SetValue(component, name, value);
            }

            void TryAggregation(object current) {
                var enumerable = value as IEnumerable;
                if (enumerable == null)
                    return;

                var items = enumerable;
                if (!ReferenceEquals(current, items) && enumerable.GetEnumerator().MoveNext()) {
                    MethodInfo mi = ReflectionMetaObject.FindAddonMethod(current.GetType(), enumerable);

                    if (mi == null) {
                        throw new NotImplementedException();
                    }

                    foreach (var item in items) {
                        mi.Invoke(current, new object[] {
                                      item
                                  });
                    }
                }
            }

        }
    }
}
