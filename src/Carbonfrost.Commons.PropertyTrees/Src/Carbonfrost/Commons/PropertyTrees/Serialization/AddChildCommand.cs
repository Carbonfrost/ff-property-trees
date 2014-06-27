//
// - AddChildCommand.cs -
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
using System.Diagnostics;
using System.Linq;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class TemplateMetaObject {

        [DebuggerDisplay("{debuggerDisplay}")]
        sealed class AddChildCommand : ITemplateCommand {

            readonly OperatorDefinition definition;
            readonly IReadOnlyDictionary<string, object> arguments;

            private string debuggerDisplay {
                get {
                    return string.Format("{0} (Count = {1})", definition, arguments.Count);
                }
            }

            public AddChildCommand(OperatorDefinition definition,
                                   IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
                this.definition = definition;
                this.arguments = arguments.ToDictionary(t => t.Key, t => t.Value.Component);
            }

            void ITemplateCommand.Apply(Stack<object> values) {
                object component = values.Peek();
                values.Push(definition.Apply(null, component, arguments));
            }
        }
    }
}
