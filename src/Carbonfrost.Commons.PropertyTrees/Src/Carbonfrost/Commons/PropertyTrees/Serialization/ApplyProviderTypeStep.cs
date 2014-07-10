//
// - ApplyProviderTypeStep.cs -
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
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ApplyProviderTypeStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject StartStep(PropertyTreeMetaObject target,
                                                             PropertyTreeNavigator self,
                                                             NodeList children) {
                Type type = target.ComponentType ?? typeof(object);

                // Select providers
                if (type.IsProviderType()) {
                    var node = children.FindAndRemove(ImplicitDirective(target, "provider")).FirstOrDefault();

                    if (node != null) {
                        var serviceProvider = Parent.GetBasicServices(node);
                        var pro = DirectiveFactory.CreateTargetProvider(type, node);
                        if (pro != null) {
                            target = target.BindTargetProvider(pro, serviceProvider);
                            return target;
                        }
                    }
                }

                return target;
            }
        }
    }

}
