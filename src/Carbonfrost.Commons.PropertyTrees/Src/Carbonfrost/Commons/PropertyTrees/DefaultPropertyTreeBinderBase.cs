//
// - DefaultPropertyTreeBinderBase.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Resources;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using RuntimeAdaptable = Carbonfrost.Commons.Shared.Runtime.Adaptable;

namespace Carbonfrost.Commons.PropertyTrees {

    abstract class DefaultPropertyTreeBinderBase : PropertyTreeBinder {

        internal OperatorDefinition SelectOperator(PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {
            OperatorDefinition factory = null;

            if (context.TreeDefinition != null) {
                factory = context.TreeDefinition.GetOperator(navigator.QualifiedName);

                if (factory == null) {
                    context.ProbeRuntimeComponents();
                    factory = context.TreeDefinition.GetOperator(navigator.QualifiedName);
                }
            }
            return factory;
        }

        internal PropertyDefinition SelectPropertyWithChecks(PropertyTreeBindingContext context,
                                                             string ns,
                                                             string name) {
            // TODO Could have namespaces, in error message
            // TODO Ensure that the lne number is added to the callback error message
            PropertyDefinition property = SelectProperty(context, ns, name);

            if (property == null) {
                string displayName = Utility.DisplayName(name, ns);
                var ui = new InterfaceUsageInfo(InterfaceUsage.Missing,
                                                SR.BinderMissingProperty(displayName, context.ComponentType),
                                                null, true);
                context.Callback.OnPropertyAnnotation(name, ui);
            }

            return property;
        }

        // TODO Use more vigorous inflection-only rules
        internal PropertyDefinition SelectProperty(PropertyTreeBindingContext context,
                                                   string ns,
                                                   string name) {

            object component = context.Component;
            PropertyTreeDefinition definition;

            if (component != null)
                definition = PropertyTreeDefinition.FromValue(component);

            else if (context.ComponentType != null)
                definition = PropertyTreeDefinition.FromType(context.ComponentType);

            else
                return null;

            var result = definition.GetProperty(name, ns);
            if (result == null) {
                context.ProbeRuntimeComponents();
                result = definition.GetProperty(name, ns);
            }

            return result ?? definition.DefaultProperty;
        }

        internal void SetLineInfo(PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {
            context.LineInfo = Utility.CreateLineInfo(navigator.LineNumber, navigator.LinePosition);
        }
    }
}
