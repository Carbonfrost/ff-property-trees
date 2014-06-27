//
// - ProcessPropertiesStep.cs -
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        class ProcessPropertiesStep : PropertyTreeBinderStep {

            readonly bool _allowDefault;

            public ProcessPropertiesStep(bool allowDefault) {
                this._allowDefault = allowDefault;
            }

            public override PropertyTreeMetaObject StartStep(PropertyTreeMetaObject target, PropertyTreeNavigator self, NodeList children) {
                children.FindAndRemove(t => Apply(target, t)).All();
                return target;
            }

            // TODO Implement property ordering based on [DependsOn]
            private bool Apply(PropertyTreeMetaObject target, PropertyTreeNavigator node) {
                PropertyDefinition prop;
                if (_allowDefault) {
                    prop = target.GetDefinition().DefaultProperty;
                    if (prop == null)
                        return false;

                } else {
                    var im = ImpliedName(node, target);
                    prop = target.SelectProperty(im);
                    if (prop == null || prop.IsIndexer)
                        return false;
                }

                DoPropertyBind(target, node, prop);
                return true;
            }

            private void DoPropertyBind(PropertyTreeMetaObject target,
                                        PropertyTreeNavigator navigator,
                                        PropertyDefinition property)
            {
                object ancestor = null;
                PropertyTreeMetaObject ancestorMeta = null;

                if (property.IsExtender) {
                    var ancestorType = property.DeclaringTreeDefinition.SourceClrType;
                    ancestorMeta = target.GetAncestors().Cast<PropertyTreeMetaObject>().FirstOrDefault(
                        t => ancestorType.IsAssignableFrom(t.ComponentType));

                    if (ancestorMeta != null)
                        ancestor = ancestorMeta.Component;
                }

                var component = target.Component;
                PropertyTreeMetaObject propertyTarget = target.CreateChild(property, navigator.QualifiedName, ancestorMeta);

                var services = ServiceProvider.Compose(
                    new PropertyBindContext(component, property)
                    {
                        LineNumber = navigator.LineNumber,
                        LinePosition = navigator.LinePosition,
                    },
                    Parent);

                propertyTarget = navigator.TopLevelBind(propertyTarget, services);
                target.BindSetMember(property, navigator.QualifiedName, propertyTarget, ancestorMeta, services);
            }

            public override PropertyTreeMetaObject EndStep(PropertyTreeMetaObject target) {
                return target;
            }

            class PropertyBindContext : ITypeDescriptorContext, IXmlLineInfo {

                private readonly object component;
                private readonly PropertyDefinition property;

                public PropertyBindContext(object component, PropertyDefinition property)
                {
                    this.property = property;
                    this.component = component;
                }

                public bool HasLineInfo() {
                    return this.LineNumber > 0;
                }

                public int LineNumber { get; set; }
                public int LinePosition { get; set; }

                public bool OnComponentChanging() {
                    return false;
                }

                public void OnComponentChanged() {}

                public IContainer Container {
                    get {
                        return null;
                    }
                }

                public object Instance {
                    get {
                        return this.component;
                    }
                }

                public PropertyDescriptor PropertyDescriptor {
                    get {
                        return property.GetUnderlyingDescriptor();
                    }
                }

                public object GetService(Type serviceType) {
                    if (serviceType == null)
                        throw new ArgumentNullException("serviceType");

                    if (typeof(ITypeDescriptorContext).Equals(serviceType))
                        return this;

                    if (typeof(IXmlLineInfo).Equals(serviceType))
                        return this;

                    return null;
                }
            }

        }
    }

}
