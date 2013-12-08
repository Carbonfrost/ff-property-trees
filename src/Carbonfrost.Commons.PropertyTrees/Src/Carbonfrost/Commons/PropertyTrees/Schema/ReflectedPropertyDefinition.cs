//
// - ReflectedPropertyDefinition.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.ComponentModel;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedPropertyDefinition : PropertyDefinition {

        private readonly PropertyDescriptor property;

        public ReflectedPropertyDefinition(PropertyDescriptor property) {
            this.property = property;
        }

        internal override PropertyDescriptor GetUnderlyingDescriptor() {
            return this.property;
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return null;
            }
        }

        public override object DefaultValue {
            get {
                DefaultValueAttribute dva = (DefaultValueAttribute) property.Attributes[typeof(DefaultValueAttribute)];
                if (dva == null)
                    return null;
                else
                    return dva.Value;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override bool IsOptional {
            get {
                return true;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override string Namespace {
            get {
                return TypeHelper.GetNamespaceName(property.ComponentType);
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override string Name {
            get {
                return property.Name;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override void SetValue(object component, object value) {
            property.SetValue(component, value);
        }

        public override Type PropertyType {
            get {
                return property.PropertyType;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override object GetValue(object component) {
            return property.GetValue(component);
        }

        public override TypeConverter Converter {
            get {
                return property.Converter;
            }
        }

        public override bool IsReadOnly {
            get {
                return property.IsReadOnly;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override void SetValue(object component, QualifiedName name, object value) {
            SetValue(component, value);
        }

        public override object GetValue(object component, QualifiedName name) {
            return GetValue(component);
        }
    }
}
