//
// - PropertyTreeBindingContext.cs -
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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.Shared.Runtime.Components;

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeBindingContext : IServiceProvider, ITypeDescriptorContext {

        private readonly BitArray markedProperties = new BitArray(0);
        private DefaultPopulateCallback _default = new DefaultPopulateCallback(null);
        private IXmlLineInfo lineInfo;
        private IServiceProvider serviceProvider;
        private IPopulateComponentCallback callback;
        private readonly INameScope names;

        public IXmlLineInfo LineInfo {
            get { return lineInfo; }
            set {
                lineInfo = value;
                _default = new DefaultPopulateCallback(value);
            }
        }

        // TODO Shouldn't need ComponentType -- set this property instead of
        // deriving it (requires that non-structured types like Int32 have a
        // representation)

        public PropertyTreeDefinition TreeDefinition {
            get {
                if (ComponentType == null)
                    return null;

                return PropertyTreeDefinition.FromType(ComponentType);
            }
        }

        public IPopulateComponentCallback Callback {
            get { return callback ?? _default; }
            set {
                if (value != this) {
                    callback = value;
                }
            }
        }

        public PropertyTreeBindingContext Parent { get; private set; }
        public object Component { get; set; }
        public Type ComponentType { get; set; }
        public PropertyDefinition Property { get; set; }
        public OperatorDefinition FactoryOperator { get; set; }

        public INameScope NameScope {
            get {
                INameScope ns = this.Component as INameScope;
                if (ns == null)
                    return this.Parent.NameScope ?? Utility.NullNameScope;
                else
                    return names ?? ns;
            }
        }

        public PropertyTreeBindingContext() : this(null) {}

        internal PropertyTreeBindingContext(PropertyTreeBindingContext parentContext,
                                            INameScope scope = null) {
            this.serviceProvider = parentContext ?? ServiceProvider.Null;
            this.Parent = parentContext;
            this.names = scope;
        }

        public PropertyTreeBindingContext CreateChildContext() {
            return new PropertyTreeBindingContext(this);
        }

        internal PropertyTreeBinder GetPropertyTreeBinder() {
            return PropertyTreeBinder.GetPropertyTreeBinder(this.ComponentType, this);
        }

        internal bool Mark(PropertyTreeNavigator nav) {
            BitArray ba = this.markedProperties;
            int position = nav.Position;
            ba.Length = Math.Max(position + 1, ba.Length);

            if (ba[position])
                return false;
            else
                return ba[position] = true;
        }

        // IServiceProvider implementation
        public object GetService(Type serviceType) {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType"); // $NON-NLS-1

            if (typeof(IPopulateComponentCallback).Equals(serviceType))
                return this;

            if (typeof(IXmlLineInfo).Equals(serviceType))
                return this.LineInfo;

            return serviceProvider.GetService(serviceType);
        }

        // ITypeDescriptorContext implementation
        IContainer ITypeDescriptorContext.Container {
            get { return null; } }

        object ITypeDescriptorContext.Instance {
            get { return this.Component;  } }

        bool ITypeDescriptorContext.OnComponentChanging() {
            return false;
        }

        void ITypeDescriptorContext.OnComponentChanged() {}

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor {
            get {
                if (this.Property == null)
                    return null;
                else
                    return this.Property.GetUnderlyingDescriptor();
            }
        }

        internal void SetProviderMember(Type providerType, MemberInfo member) {
            if (member == null)
                return;

            if (member.MemberType == MemberTypes.TypeInfo) {
                this.ComponentType = (Type) member;
                this.FactoryOperator = this.TreeDefinition.Constructor;

            } else if (member.MemberType == MemberTypes.Method) {
                this.ComponentType = ((MethodInfo) member).ReturnType;
                this.FactoryOperator = ReflectedProviderFactoryDefinitionBase.Create(providerType, member);

            } else {
                this.Component = ((FieldInfo) member).GetValue(null);
            }
        }

        internal void SetProvider(Type providerType, object value) {
            // TODO Might be possible that the value is formatted as a prefix qname, in which case must be bound
            var qualified = value.ToString();
            var provider = AppDomain.CurrentDomain.GetProviderMember(this.ComponentType, qualified);
            SetProviderMember(providerType, provider);
        }

        // TODO It would be better if the component or a service could control this logic
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
    }

}
