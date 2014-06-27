//
// - ApplyGenericParametersStep.cs -
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

        class ApplyGenericParametersStep : PropertyTreeBinderStep {

            public override PropertyTreeMetaObject StartStep(
                PropertyTreeMetaObject target,
                PropertyTreeNavigator self,
                NodeList children) {

                if (!(target is UntypedToTypedMetaObject))
                    return target;

                if (!children.Any())
                    return target;

                try {
                    // TODO Only supports one child (lame spec)
                    var rootType = target.Root.ComponentType;
                    var types = children.Select(t => ConvertToType(t, rootType)).ToArray();

                    target = target.BindGenericParameters(types);

                } catch (Exception ex) {
                    if (ex.IsCriticalException())
                        throw;

                    Parent.errors.CouldNotBindGenericParameters(target.ComponentType, ex, self.FileLocation);
                }

                Parent.Bind(target, children.First(), null);
                children.Clear();
                return target;
            }

            private Type ConvertToType(PropertyTreeNavigator nav, Type rootType) {
                QualifiedName fixedName = nav.QualifiedName;
                string name = fixedName.LocalName;

                if (char.IsLower(name[0])) {
                    name = char.ToUpperInvariant(name[0]) + name.Substring(1);
                    fixedName = fixedName.ChangeLocalName(name);
                }

                if (string.IsNullOrEmpty(nav.Namespace)) {
                    // Try looking for the name in the same assembly
                    var choices = rootType.Assembly.GetTypesHelper()
                        .Where(t => t.IsPublic && t.Name == name);

                    if (choices.Any()) {
                        return choices.SingleOrThrow(() => PropertyTreesFailure.UnableToMatchTypeNameAmbiguous(nav.Name, choices));
                    } else {
                        throw PropertyTreesFailure.UnableToMatchTypeNameZero(name);
                    }

                } else {
                    return AppDomain.CurrentDomain.GetTypeByQualifiedName(fixedName, true);
                }
            }
        }
    }

}
