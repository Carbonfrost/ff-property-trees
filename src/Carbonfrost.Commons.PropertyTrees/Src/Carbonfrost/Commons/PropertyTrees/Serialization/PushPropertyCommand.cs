//
// - PushPropertyCommand.cs -
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
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class TemplateMetaObject {

        class PushPropertyCommand : ITemplateCommand {

            readonly PropertyDefinition property;
            readonly QualifiedName name;

            public PushPropertyCommand(PropertyDefinition property, QualifiedName name) {
                this.property = property;
                this.name = name;
            }

            void ITemplateCommand.Apply(Stack<object> values) {
                var component = values.Peek();
                var existing = property.GetValue(component, name);
                if (existing == null) {
                    throw new NotImplementedException();
                }

                values.Push(existing);
            }
        }

    }
}


