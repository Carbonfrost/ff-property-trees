//
// - DefaultPropertyTreeBinder.cs -
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
using System.Diagnostics;
using System.Linq;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class DefaultPropertyTreeBinder : DefaultPropertyTreeBinderBase {

        static readonly PropertyTreeBinder Property = new PropertyBinder();
        static readonly PropertyTreeBinder Tree = new BuilderIndirectionPropertyTreeBinder();
        static readonly PropertyTreeBinder Operator = new OperatorBinder();

        // `PropertyTreeBinder' overrides

        // TODO Implement property ordering based on [DependsOn]
        // Maybe clone the origin nav and just move back to it?

        public override object Bind(PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {
            if (context == null)
                throw new ArgumentNullException("context"); // $NON-NLS-1
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            if (navigator.IsProperty)
                return Property.Bind(context, navigator);
            else
                return Tree.Bind(context, navigator);
        }

        private void SetProperty(object outputValue, PropertyTreeBindingContext context, PropertyDefinition property) {
            if (context.Component != null) {
                try {
                    property.SetValue(context.Component, outputValue);

                } catch (NullReferenceException nre) {
                    // Normally a "critical" exception, consider it a conversion error
                    context.Callback.OnConversionException(property.Name, outputValue, nre);

                } catch (ArgumentException a) {
                    context.Callback.OnConversionException(property.Name, outputValue, a);

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    context.Callback.OnConversionException(property.Name, outputValue, ex);
                }
            }
        }

        private object BindInlineParameter(PropertyTreeBindingContext context,
                                           PropertyTreeNavigator nav,
                                           PropertyDefinition property,
                                           Type parameterType) {
            // Binds a parameter required for activating an instance

            // TODO Should we supply/use attributes from the parameter
            // and/or corresponding property descriptor?
            PropertyTreeBindingContext childContext = context.CreateChildContext();
            childContext.ComponentType = parameterType;
            childContext.Property = property;

            PropertyTreeBinder binder = GetPropertyTreeBinder(parameterType, context);
            return binder.Bind(childContext, nav);
        }

        private void BindParametersConstruct(OperatorDefinition op,
                                             PropertyTreeNavigator nav,
                                             PropertyTreeBindingContext context,
                                             bool hasEligibleSiblingNodes = true) {

            IDictionary<string, object> args = new Dictionary<string, object>(op.Parameters.Count);

            // TODO throw use parent for the right line nummber
            PropertyDefinition myParam = null;

            List<string> requiredMissing = new List<string>();
            foreach (PropertyDefinition p in op.Parameters) {

                // Fallback to empty ns
                if (hasEligibleSiblingNodes && nav.MoveToSibling(p.Namespace, p.Name) && context.Mark(nav))
                    args[p.Name] = this.BindInlineParameter(context, nav, p, p.PropertyType);
                else if (p.IsOptional)
                    args[p.Name] = p.DefaultValue;
                else if (p.IsParamArray)
                    myParam = p;
                else
                    requiredMissing.Add(Utility.DisplayName(p.QualifiedName));
            }

            if (requiredMissing.Count > 0)
                throw PropertyTreesFailure.RequiredPropertiesMissing(requiredMissing.ToArray(),
                                                                     op.ToString(),
                                                                     nav.LineNumber, nav.LinePosition);

            // Try param
            if (hasEligibleSiblingNodes && myParam != null && nav.MoveToFirst()) {
                var all = new List<object>();
                var elementType = myParam.PropertyType.GetElementType();
                while (context.Mark(nav)) {
                    var inline = BindInlineParameter(context, nav, myParam, elementType);
                    all.Add(inline);

                    if (!nav.MoveToNext())
                        break;
                }

                var array = Array.CreateInstance(elementType, all.Count);
                ((System.Collections.ICollection) all).CopyTo(array, 0);
                args[myParam.Name] = array;
            }

            object parent = null;
            if (context.Parent != null) {
                parent = context.Parent.Component;
            }

            // TODO Would be better to use the operator definition to obtain this return type
            context.ComponentType = TypeHelper.GetReturnType(op.UnderlyingMethod);

            // For remove and clear, we are already in child scope
            object component = context.Component;
            if (op.Operator == OperatorType.Clear
                || op.Operator == OperatorType.Remove) {
                component = parent;
                parent = null;
            }

            SetLineInfo(context, nav);
            context.Component = op.Apply(component, parent, args);
        }

    }
}
