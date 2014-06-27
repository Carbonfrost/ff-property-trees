//
// - ProcessOperatorsStep.cs -
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
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ProcessOperatorsStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject StartStep(PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                children.FindAndRemove(t => Apply(target, t)).All();
                return target;
            }

            private bool Apply(PropertyTreeMetaObject target, PropertyTreeNavigator node) {
                var member = target.SelectOperator(ImpliedName(node, target));

                if (member != null) {
                    DoOperatorBind(target, node, member);
                    return true;
                }

                return false;
            }

            private PropertyTreeMetaObject DoOperatorBind(PropertyTreeMetaObject target, PropertyTreeNavigator navigator, OperatorDefinition op) {
                OperatorDefinition addon = op;

                if (addon.DefaultParameter != null) {
                    Type itemType = ((PropertyTreeFactoryDefinition) addon).OutputType;
                    // TODO Use service activation (we have the output type)

                    var item = target.CreateChild(itemType);
                    var model = navigator.TopLevelBind(item, null);
                    var args = new Dictionary<string, PropertyTreeMetaObject>
                    {
                        { addon.DefaultParameter.Name, model }
                    };

                    try {
                        target.BindAddChild(op, args);

                    } catch (Exception ex) {
                        if (ex.IsCriticalException())
                            throw;

                        Parent.errors.BadAddChild(target.ComponentType, ex, navigator.FileLocation);
                    }

                } else {
                    // TODO The number and kinds of arguments are constrained.  This should probably
                    // be enforced within schema operator reflection (spec)

                    Action<IReadOnlyDictionary<string, PropertyTreeMetaObject>> func;
                    var children = NodeList.Create(SelectChildren(navigator));

                    switch (addon.OperatorType) {
                        case OperatorType.Add:
                            func = args => {
                                var child = target.BindAddChild(addon, args);

                                if (child.ShouldBindChildren) {
                                    Parent.BindChildNodes(child, navigator, children);
                                }
                            };
                            break;

                        case OperatorType.Remove:
                            func = args => target.BindRemoveChild(addon, args);
                            break;

                        case OperatorType.Clear:
                        default:
                            func = args => target.BindClearChildren(addon, args);
                            break;
                    }

                    var services = Parent.GetBasicServices(navigator);
                    var args2 = ExtractParameterDictionary(op, target, services, children);
                    func(args2);
                }

                return target;
            }
        }
    }

}
