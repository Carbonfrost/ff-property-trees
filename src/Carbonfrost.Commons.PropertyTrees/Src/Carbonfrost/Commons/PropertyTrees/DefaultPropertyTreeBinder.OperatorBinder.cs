//
// - DefaultPropertyTreeBinder.OperatorBinder.cs -
//
// Copyright 2010 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Resources;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using RuntimeAdaptable = Carbonfrost.Commons.Shared.Runtime.Adaptable;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class DefaultPropertyTreeBinder {

        class OperatorBinder : DefaultPropertyTreeBinder {

            public override object Bind(PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {
                BindOperator(context, navigator);
                return context.Component;
            }

            private void BindOperator(
                PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {

                OperatorDefinition addon = context.FactoryOperator;

                if (addon.DefaultParameter != null) {
                    Type itemType = ((PropertyTreeFactoryDefinition) addon).OutputType;

                    // TODO Use service activation (we have the output type)
                    // TODO Create the child binder context (which should bind in ordinary way... )
                    // object model = Activator.CreateInstance(itemType);

                    var childContext = context.CreateChildContext();
                    childContext.ComponentType = itemType;

                    object model = childContext.GetPropertyTreeBinder().Bind(childContext, navigator);
                    // XXX object model = Bind(itemType, navigator);

                    // TODO Should other params be handled?
                    // Add(Customer,value:,other:)
                    // Create and bind Customer  object, precollect value and other
                    addon.Apply(context.Component, null, new Dictionary<string, object> { { addon.DefaultParameter.Name, model } } );

                } else {

                    // TODO What to do if errors or missing component here?
                    // It's also possible that the method could return void, in which case
                    // no further binding
                    var childContext = context.CreateChildContext();
                    if (navigator.MoveToFirstChild()) {
                        BindParametersConstruct(context.FactoryOperator, navigator, childContext);
                        navigator.MoveToParent();

                        if (childContext.Component != null) {
                            // TODO Allow further binding Bind(model, navigator); ???
                            base.Bind(childContext, navigator);
                        }

                    } else {
                        BindParametersConstruct(context.FactoryOperator, navigator, childContext, false);
                    }
                }
            }
        }
    }

}
