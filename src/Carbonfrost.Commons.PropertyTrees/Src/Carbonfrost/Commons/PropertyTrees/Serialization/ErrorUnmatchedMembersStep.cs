//
// - ErrorUnmatchedMembersStep.cs -
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
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.PropertyTrees.Resources;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ErrorUnmatchedMembersStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject StartStep(PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                foreach (var child in children.Rest()) {

                    string msg;
                    if (target.ComponentType.IsHiddenUX()) {
                        msg = SR.BinderMissingPropertyNoType(child.QualifiedName);
                    } else {
                        msg = SR.BinderMissingProperty(child.QualifiedName, target.ComponentType);
                    }

                    try {
                        var info = new InterfaceUsageInfo(InterfaceUsage.Missing, msg, null, true);
                        Parent.Callback.OnPropertyAnnotation(child.QualifiedName.ToString(), info);

                    } catch (Exception ex) {
                        if (ex.IsCriticalException())
                            throw;

                        throw PropertyTreesFailure.UnmatchedMembersGenericError(ex, child.FileLocation);
                    }
                }

                return target;
            }

        }
    }

}
