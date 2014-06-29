//
// - PropertyTreeBinderImpl.cs -
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl : PropertyTreeMetaObjectBinder, IServiceProvider {

        private readonly IPropertyTreeBinderErrors errors;
        private readonly PropertyTreeBinderStep[] Pipeline = {
            new ApplyGenericParametersStep(),
            new ApplyLateBoundTypeStep(),
            new ApplyProviderTypeStep(),
            new ApplyConstructorStep(),
            new ApplyStreamingSourcesStep(),
            new ApplyDefaultConstructorStep(),
            new ProcessMembersStep
                (
                    new ProcessPropertiesStep(false),
                    new ProcessOperatorsStep(),
                    new ProcessPropertiesStep(true)
                ),
            new ErrorUnmatchedMembersStep(),
            new EndObjectStep(),
        };

        private readonly SerializerDirectiveFactory directiveFactory;
        private readonly IPopulateComponentCallback callback;

        public SerializerDirectiveFactory DirectiveFactory {
            get {
                return directiveFactory;
            }
        }

        public IPopulateComponentCallback Callback {
            get {
                return callback;
            }
        }

        private static QualifiedName ImpliedName(PropertyTreeNavigator nav, PropertyTreeMetaObject target) {
            QualifiedName qualifiedName = nav.QualifiedName;
            if (nav.IsExpressNamespace)
                return qualifiedName;

            NamespaceUri impliedNS = Utility.GetXmlnsNamespaceSafe(target.ComponentType);
            if (impliedNS == null)
                return qualifiedName;
            else
                return qualifiedName.ChangeNamespace(impliedNS);
        }

        internal IServiceProvider GetBasicServices(PropertyTreeNavigator nav) {
            return ServiceProvider.Compose(this,
                                           ServiceProvider.FromValue(nav));
        }

        internal static FileLocation FindFileLocation(IServiceProvider serviceProvider) {
            var loc = serviceProvider.TryGetService(Utility.NullLineInfo);
            string uri = Convert.ToString(serviceProvider.TryGetService(Utility.NullUriContext).BaseUri);
            return new FileLocation(loc.LineNumber, loc.LinePosition, uri);
        }

        public PropertyTreeBinderImpl(IPropertyTreeBinderErrors errors,
                                      IPopulateComponentCallback callback) {
            this.errors = errors;
            Array.ForEach(Pipeline, t => t.Parent = this);
            this.directiveFactory = new SerializerDirectiveFactory(this);
            this.callback = callback;
        }

        private static Predicate<PropertyTreeNavigator> ImplicitDirective(PropertyTreeMetaObject target, string name) {
            var defaultNS = NamespaceUri.Default + name;
            var langNS = Xmlns.PropertyTrees2010Uri + name;
            var existing = target.SelectMember(defaultNS);

            if (existing == null) {
                return t => t.Name == name;

            } else {
                return t => t.QualifiedName == langNS;
            }
        }

        private static IEnumerable<PropertyTreeNavigator> SelectChildren(PropertyTreeNavigator navigator) {
            navigator = navigator.Clone();

            if (navigator.MoveToFirstChild()) {
                do {
                    yield return navigator.Clone();
                } while (navigator.MoveToNext());
            }
        }

        internal PropertyTreeMetaObject BindChildNodes(PropertyTreeMetaObject target,
                                                       PropertyTreeNavigator self,
                                                       NodeList children) {
            foreach (var step in Pipeline) {
                target = step.Process(target, self, children);
            }

            return target;
        }

        public override PropertyTreeMetaObject Bind(PropertyTreeMetaObject target,
                                                    PropertyTreeNavigator navigator,
                                                    IServiceProvider serviceProvider) {
            if (target == null)
                throw new ArgumentNullException("target");
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            if (navigator.IsProperty) {
                string value = Convert.ToString(navigator.Value);
                try {
                    var sp = ServiceProvider.Compose(ServiceProvider.FromValue(navigator), serviceProvider, this);
                    return target.BindInitializeValue(value, sp);

                } catch (Exception ex) {
                    // Throw critical exceptions if they originate within PT; otherwise, allow
                    // callback to decide how to handle them.
                    if (ex.IsCriticalException())
                        throw;

                    var descriptor = serviceProvider.GetService<ITypeDescriptorContext>();
                    string property;
                    Type componentType;
                    if (descriptor == null || descriptor.PropertyDescriptor == null) {
                        property = navigator.QualifiedName.ToString();
                        componentType = descriptor.Instance.GetType();
                    } else {
                        property = descriptor.PropertyDescriptor.Name;
                        componentType = descriptor.PropertyDescriptor.ComponentType;
                    }

                    FileLocation loc = navigator.FileLocation;

                    try {
                        Callback.OnConversionException(property, value, ex);

                    } catch (Exception ex2) {
                        if (Require.IsCriticalException(ex2))
                            throw;

                        throw PropertyTreesFailure.BinderConversionError(value, property, componentType, ex2, loc);
                    }
                }
            }

            var children = SelectChildren(navigator);
            return BindChildNodes(target, navigator, NodeList.Create(children));
        }

        public object GetService(Type serviceType) {
            if (serviceType == typeof(IPropertyTreeBinderErrors))
                return this.errors;

            return null;
        }

    }
}
