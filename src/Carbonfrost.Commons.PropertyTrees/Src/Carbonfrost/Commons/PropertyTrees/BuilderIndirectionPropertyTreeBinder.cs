//
// - BuilderIndirectionPropertyTreeBinder.cs -
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
using System.Diagnostics;
using System.Linq;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.Shared.Runtime.Components;
using RuntimeAdaptable = Carbonfrost.Commons.Shared.Runtime.Adaptable;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class DefaultPropertyTreeBinder {

        class BuilderIndirectionPropertyTreeBinder : DefaultPropertyTreeBinder {

            // TODO Better implementation that uses service provider activation
            // (which would allow us to create instances that don't have default constructors)
            // (also need to allow deferal for a more specific component type)

            public override object Bind(PropertyTreeBindingContext context,
                                        PropertyTreeNavigator nav) {
                // TODO This ought to be a factory schema def rather than direct reflection

                bool isBuilder = false;
                QualifiedName qname = nav.QualifiedName;
                bool hasChildren = nav.MoveToChild(0);

                if (context.Component == null) {
                    LiftToLateBoundType(nav, context, hasChildren, qname);

                    if (context.ComponentType == null)
                        throw PropertyTreesFailure.LateBoundTypeMissing(nav.LineNumber, nav.LinePosition);

                    isBuilder = PickBuilderTypeIfAvailable(context);

                    if (context.FactoryOperator != null)
                        BindParametersConstruct(context.FactoryOperator, nav, context, hasChildren);
                }

                if (hasChildren && nav.MoveToFirst()) {
                    // TODO Use correct ns logic on this source

                    // Try reading from a source
                    var sourceProperty = SelectProperty(context, nav.Namespace, "source");
                    if (sourceProperty == null && nav.MoveToSibling("source")) {
                        var ss = StreamingSource.Create(context.ComponentType);

                        // TODO Could need to hydrate instead
                        var comp = ss.Load(StreamContext.FromSource(new Uri(nav.Value.ToString())), context.ComponentType);

                        var copyFrom = comp; // Load(inputSource, instance.GetType());
                        // var props = Properties.FromValue(copyFrom);
                        // Activation.Initialize(context.Component, props);
                        Template.Create(copyFrom).Initialize(context.Component);

                        // context.Component = comp;
                        context.Mark(nav);
                        nav.MoveToSibling(0);
                    }

                    do {
                        if (!context.Mark(nav))
                            continue;

                        SetLineInfo(context, nav);

                        if (nav.IsProperty) {
                            var property = SelectPropertyWithChecks(context, nav.Namespace, nav.Name);
                            this.BindProperty(context, property, nav);

                        } else {
                            this.BindPropertyTree(context, nav);
                        }

                    } while (nav.MoveToNext());

                    nav.MoveToParent();
                }

                if (isBuilder)
                    context.Component = RuntimeAdaptable.InvokeBuilder(context.Component, context);

                ApplyActivationProviders(context, nav, context.Component);
                return context.Component;
            }

            void ApplyActivationProviders(PropertyTreeBindingContext ctxt,
                                          PropertyTreeNavigator nav,
                                          object component) {

                IFileLocationConsumer loc = component as IFileLocationConsumer;
                if (loc != null)
                    loc.SetFileLocation(nav.LineNumber, nav.LinePosition);

                // TODO Should apply activation providers using all registered?
            }

            private void BindProperty(PropertyTreeBindingContext context,
                                      PropertyDefinition property,
                                      PropertyTreeNavigator navigator) {

                object value = navigator.Value;
                object outputValue = null;
                try {
                    var sp = ServiceProvider.Compose(ServiceProvider.FromValue(navigator), context);
                    var binder = GetPropertyTreeBinder(property.PropertyType, sp);

                    var childContext = new PropertyTreeBindingContext(context);
                    childContext.ComponentType = property.PropertyType;
                    childContext.Property = property;
                    outputValue = binder.Bind(childContext, navigator);

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    SetLineInfo(context, navigator);
                    context.Callback.OnConversionException(property.Name, value, ex);
                }

                SetProperty(outputValue, context, property);
            }

            private void BindPropertyTree(PropertyTreeBindingContext context,
                                          PropertyTreeNavigator navigator)
            {
                PropertyTreesRequire.ExpectNode(navigator,
                                                PropertyNodeType.PropertyTree);

                OperatorDefinition factory = SelectOperator(context, navigator);

                SetLineInfo(context, navigator);
                if (factory == null) {
                    var property = SelectPropertyWithChecks(context, navigator.Namespace, navigator.Name);
                    BindPropertyTreeIndirect(context, property, navigator);

                } else {
                    context.FactoryOperator = factory;
                    Operator.Bind(context, navigator);
                }
            }

            private void BindPropertyTreeIndirect(PropertyTreeBindingContext context,
                                                  PropertyDefinition property,
                                                  PropertyTreeNavigator navigator)
            {
                if (property == null)
                    return;

                Debug.Assert(context.Component != null);

                // TODO Decide what to do on error here:
                object model = null;
                try {
                    model = property.GetValue(context.Component, navigator.QualifiedName);

                } catch (Exception ex) {

                    if (Require.IsCriticalException(ex))
                        throw;
                    else
                        throw PropertyTreesFailure.ProblemAccessingProperty(ex,  context.Component, navigator.QualifiedName, navigator.LineNumber,  navigator.LinePosition);
                }

                var childContext = context.CreateChildContext();
                childContext.Component = model;
                childContext.ComponentType = TypeHelper.TypeOf(model, property.PropertyType);

                var binder = childContext.GetPropertyTreeBinder();
                model = binder.Bind(childContext, navigator);

                // If was changed, set value
                if (property.IsReadOnly) {

                    if (!object.ReferenceEquals(childContext.Component, model)) {
                        // TODO Component shouldn't change - is this an error?
                        property.SetValue(context.Component, property.QualifiedName, model);
                    }
                } else {
                    property.SetValue(context.Component, property.QualifiedName, model);
                }
            }

            private bool PickBuilderTypeIfAvailable(PropertyTreeBindingContext context) {
                Type builderType = RuntimeAdaptable.GetBuilderType(context.ComponentType);
                if (builderType != null) {
                    context.ComponentType = builderType;
                    context.FactoryOperator = context.TreeDefinition.Constructor;
                }

                if (context.FactoryOperator == null) {
                    context.FactoryOperator = context.TreeDefinition.Constructor;
                }
                return builderType != null;
            }

            private void LiftToLateBoundType(PropertyTreeNavigator nav,
                                             PropertyTreeBindingContext context,
                                             bool hasChildren,
                                             QualifiedName qname) {
                try {
                    LiftToLateBoundTypeUnsafe(nav, context, hasChildren, qname);

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    throw PropertyTreesFailure.LateBoundTypeNotFoundError(ex, nav.LineNumber, nav.LinePosition);
                }
            }

            private void LiftToLateBoundTypeUnsafe(PropertyTreeNavigator nav,
                                                   PropertyTreeBindingContext context,
                                                   bool hasChildren,
                                                   QualifiedName qname) {

                Type type = context.ComponentType ?? typeof(object);
                bool lateBound = typeof(IObjectWithType).IsAssignableFrom(type);

                // TODO If abstract but not composable, then use concrete class provider
                // TODO Should enforce the late bound type
                if (hasChildren && lateBound) {

                    if (nav.MoveToSibling("type") && context.Mark(nav)) {
                        var sp = ServiceProvider.Compose(ServiceProvider.FromValue(nav), context);
                        var binder = GetPropertyTreeBinder(typeof(Type), sp);

                        var childContext = new PropertyTreeBindingContext(context);
                        childContext.ComponentType = typeof(Type);
                        Type outputValue = (Type) binder.Bind(childContext, nav);

                        context.ComponentType = outputValue;
                        context.FactoryOperator = context.TreeDefinition.Constructor;

                    } else if (nav.MoveToSibling("provider") && context.Mark(nav)) {
                        context.SetProvider(type, nav.Value);

                    } else if (nav.MoveToSibling("implementation") && context.Mark(nav))
                        throw new NotImplementedException();

                } else if (hasChildren && type.IsProviderType() && nav.MoveToSibling("provider") && context.Mark(nav)) {
                    context.SetProvider(type, nav.Value);

                } else if (type.GetConcreteClass() != null) {
                    context.ComponentType = type.GetConcreteClass();
                    context.FactoryOperator = context.TreeDefinition.Constructor;

                } else if (type.IsComposable()) {
                    var member = AppDomain.CurrentDomain.GetProviderMember(context.ComponentType, qname);
                    context.SetProviderMember(type, member);
                }
            }

        }
    }

}
