//
// - ReflectedExtenderPropertyDefinition.cs -
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
using System.Reflection;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedExtenderPropertyDefinition : PropertyDefinition {

        private MethodInfo _setter;
        private MethodInfo _getter;
        private readonly AttachedPropertyID _name;

        public ReflectedExtenderPropertyDefinition(AttachedPropertyID name) {
            this._name = name;
        }

        internal void AddMethod(MethodInfo method) {
            // TODO Probably need better testing here for whether getter or setter

            bool isSetter = method.ReturnType == typeof(void);
            if (isSetter && _setter == null) {
                _setter = method;

            } else if (_getter == null) {
                _getter = method;

            } else {
                throw new NotImplementedException();
            }
        }

        internal override PropertyDescriptor GetUnderlyingDescriptor() {
            return null;
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return PropertyTreeDefinition.FromType((_getter ?? _setter).DeclaringType);
            }
        }

        public override bool IsExtender {
            get {
                return true;
            }
        }

        public override object DefaultValue {
            get {
                if (_getter == null)
                    return null;

                DefaultValueAttribute dva = (DefaultValueAttribute)
                    (_getter.GetCustomAttribute(typeof(DefaultValueAttribute))
                     ?? _getter.ReturnParameter.GetCustomAttribute(typeof(DefaultValueAttribute)));
                if (dva == null)
                    return null;
                else
                    return dva.Value;
            }
        }

        public override bool IsOptional {
            get {
                return true;
            }
        }

        public override string Namespace {
            get {
                return TypeHelper.GetNamespaceName(_name.DeclaringType);
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override string Name {
            get {
                return Utility.GetExtenderName(_name);
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override bool CanExtend(Type extendeeType) {
            if (extendeeType == null)
                throw new ArgumentNullException("extendeeType");

            return (_getter ?? _setter).GetParameters()[0].ParameterType.IsAssignableFrom(extendeeType);
        }

        public override Type PropertyType {
            get {
                if (_getter == null)
                    return _setter.GetParameters().Last().ParameterType;
                else
                    return _getter.ReturnType;
            }
        }

        public override TypeConverter Converter {
            get {
                return null;
            }
        }

        public override bool IsReadOnly {
            get {
                return _setter == null;
            }
        }

        public override object GetValue(object component, object ancestor, QualifiedName name) {
            // ancestor.M(component)
            if (_getter == null)
                return null;
            else
                return _getter.Invoke(ancestor, new object[] { component });
        }

        public override void SetValue(object component, object ancestor, QualifiedName name, object value) {
            // ancestor.M(component, value)
            _setter.Invoke(ancestor, new object[] { component, value });
        }

    }
}
