//
// - PropertyTreeMetaObject.cs -
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
using System.Reflection;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.Shared.Runtime.Components;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    public abstract class PropertyTreeMetaObject : IHierarchyObject {

        public static readonly PropertyTreeMetaObject Null
            = new NullMetaObject();

        private IEnumerable<PropertyTreeMetaObject> Ancestors {
            get {
                return this.GetAncestors().Cast<PropertyTreeMetaObject>();
            }
        }

        public abstract Type ComponentType {
            get;
        }

        public abstract object Component {
            get;
        }

        public PropertyTreeMetaObject Parent {
            get;
            private set;
        }

        internal bool IsLateBound {
            get {
                return typeof(IObjectWithType).IsAssignableFrom(ComponentType);
            }
        }

        protected PropertyTreeMetaObject() {}

        protected PropertyTreeMetaObject(PropertyTreeMetaObject parent) {
            this.Parent = parent;
        }

        public virtual PropertyTreeMetaObject BindEndObject(IServiceProvider serviceProvider) {
            return this;
        }

        public virtual PropertyTreeMetaObject BindAddChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            return null;
        }

        public virtual void BindRemoveChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
        }

        public virtual void BindClearChildren(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
        }

        public virtual PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            return this;
        }

        public virtual void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, PropertyTreeMetaObject ancestor, IServiceProvider serviceProvider) {
        }

        internal PropertyTreeMetaObject BindTargetProvider(TargetProviderDirective binder, IServiceProvider serviceProvider) {
            return BindTargetProvider(binder.Name, binder, serviceProvider);
        }

        public virtual PropertyTreeMetaObject BindTargetProvider(QualifiedName name, object criteria, IServiceProvider serviceProvider) {
            return this;
        }

        internal PropertyTreeMetaObject BindStreamingSource(TargetSourceDirective targetSourceBinder, IServiceProvider serviceProvider) {
            return BindStreamingSource(targetSourceBinder.GetStreamingContext(), serviceProvider);
        }

        public virtual PropertyTreeMetaObject BindStreamingSource(StreamContext input, IServiceProvider serviceProvider) {
            return this;
        }

        internal void BindTargetType(TargetTypeDirective e, IServiceProvider serviceProvider) {
            BindTargetType(e.Type, serviceProvider);
        }

        public virtual void BindTargetType(TypeReference type, IServiceProvider serviceProvider) {
        }

        public virtual PropertyTreeMetaObject BindConstructor(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            return this;
        }

        public static PropertyTreeMetaObject Create(object component, Type componentType) {
            if (component == null && componentType == null) {
                throw PropertyTreesFailure.PropertyTreeMetaObjectComponentNull();
            }

            componentType = componentType ?? component.GetType();
            if (component != null && !componentType.IsInstanceOfType(component)) {
                throw new NotImplementedException();
            }

            if (component is System.Collections.ICollection)
                return new CollectionMetaObject(component);

            return new ReflectionMetaObject(component, componentType);
        }

        public static PropertyTreeMetaObject Create(object component) {
            if (component == null)
                throw new ArgumentNullException("component");

            return Create(component, component.GetType());
        }

        public static PropertyTreeMetaObject Create(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            if (componentType == typeof(Type))
                return new TypeMetaObject();

            if (componentType.IsGenericType && componentType.GetGenericTypeDefinition() == typeof(ITemplate<>)) {
                var instType = componentType.GetGenericArguments()[0];
                if (instType.GetActivationConstructor() == null) {
                    throw PropertyTreesFailure.TemplateTypeConstructorMustBeNiladic("componentType", componentType);
                }
                return new TemplateMetaObject(componentType);
            }

            return new PreactivationMetaObject(componentType);
        }

        public virtual PropertyTreeMetaObject CreateChild(Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            var result = Create(componentType);
            result.Parent = this;
            return result;
        }

        public virtual PropertyTreeMetaObject CreateChild(object component, Type componentType) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            var result = Create(component, componentType);
            result.Parent = this;
            return result;
        }

        public PropertyTreeMetaObject CreateChild(object component) {
            if (component == null)
                throw new ArgumentNullException("component");

            return CreateChild(component, component.GetType());
        }

        internal bool TryConvertFromText(string text,
                                         IServiceProvider serviceProvider,
                                         out object result)
        {
            // TODO In some cases, this will be treated as if a streaming source hydration
            Type neededType = this.ComponentType;
            PropertyDescriptor property = null;
            object value = text;

            var context = serviceProvider.TryGetService<ITypeDescriptorContext>();
            if (context != null) {
                property = context.PropertyDescriptor;

                // Get type from property providers
                var pp = context.Instance as IPropertyProvider;
                if (pp != null) {
                    neededType = property == null ? typeof(string) : (pp.GetPropertyType(property.Name) ?? typeof(string));
                }
            }

            // Apply concrete classes
            var cp = neededType.GetConcreteClass() ?? neededType;
            var conv = TypeHelper.GetConverter(property, cp);
            result = conv.ConvertFrom(value);
            return true;
        }

        internal PropertyTreeDefinition GetDefinition() {
            object component = this.Component;

            if (component != null)
                return PropertyTreeDefinition.FromValue(component);

            else if (this.ComponentType != null)
                return PropertyTreeDefinition.FromType(this.ComponentType);

            else
                return null;

        }

        internal PropertyNodeDefinition SelectMember(QualifiedName qn) {
            return SelectProperty(qn) ?? (PropertyNodeDefinition) SelectOperator(qn);
        }

        internal PropertyDefinition SelectProperty(QualifiedName qn) {
            var result = SelectPropertyCore(qn);

            if (result == null) {
                this.ProbeRuntimeComponents();
                result = SelectPropertyCore(qn);
            }

            return result ?? GetDefinition().DefaultProperty;
        }

        private PropertyDefinition SelectPropertyCore(QualifiedName qn) {
            var definition = GetDefinition();
            var result = definition.GetProperty(qn);
            if (result != null)
                return result;

            int dot = qn.LocalName.IndexOf('.');
            if (dot > 0) {
                // TODO Index whether the PTD has extenders so we can skip some ancestors (perf)
                string prefix = qn.LocalName.Substring(0, dot);

                foreach (var current in Ancestors) {
                    var currentDef = current.GetDefinition();
                    if (currentDef.Name == prefix) {
                        // TODO Local name could be different
                        var prop = currentDef.GetProperty(qn);
                        if (prop != null) {
                            return prop;
                        }
                    }
                }

            } else {

                foreach (var current in Ancestors) {
                    var curDefinition = current.GetDefinition();
                    var prop = curDefinition.GetProperty(qn);
                    if (IsValidExtender(prop))
                        return prop;

                    var qn2 = qn.ChangeLocalName(current.ComponentType.Name + "." + qn.LocalName);
                    prop = curDefinition.GetProperty(qn2);
                    if (IsValidExtender(prop))
                        return prop;

                }
            }

            return null;
        }

        private bool IsValidExtender(PropertyDefinition prop) {
            return prop != null && prop.IsExtender && prop.CanExtend(this.ComponentType);
        }

        // TODO According to spec, dependencies should be enumerated first, which should mean we don't probe after they have been
        internal void ProbeRuntimeComponents() {
            var rc = this.Component as IRuntimeComponent;

            if (rc == null) {
                if (this.Parent != null)
                    this.Parent.ProbeRuntimeComponents();

            } else {
                foreach (var comp in rc.Dependencies) {
                    if (comp.IsAssembly)
                        Assembly.Load(comp.Name.ToAssemblyName());
                }
            }
        }

        internal virtual OperatorDefinition SelectOperator(QualifiedName qn) {
            OperatorDefinition factory = null;
            var treeDef = PropertyTreeDefinition.FromType(this.ComponentType);
            if (treeDef != null) {
                factory = treeDef.GetOperator(qn);

                if (factory == null) {
                    this.ProbeRuntimeComponents();

                    factory = treeDef.GetOperator(qn);
                }
            }
            return factory;
        }

        IHierarchyObject IHierarchyObject.ParentObject {
            get {
                return this.Parent;
            }
            set {
            }
        }

        public IEnumerable<IHierarchyObject> ChildrenObjects {
            get {
                // TODO Should probably be an error
                return Empty<IHierarchyObject>.Array;
            }
        }

    }

}
