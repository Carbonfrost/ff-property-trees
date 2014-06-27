//
// - ReflectedParameterDefinition.cs -
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
using System.Reflection;

using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedParameterDefinition : PropertyDefinition {

        private readonly ParameterInfo parameter;
        private readonly string ns;

        public ReflectedParameterDefinition(string ns, ParameterInfo pi) {
            this.parameter = pi;
            this.ns = ns;
        }

        public override TypeConverter Converter {
            get {
                return TypeDescriptor.GetConverter(parameter.ParameterType);
            }
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return null;
            }
        }

        public override bool IsParamArray {
            get {
                return this.parameter.IsDefined(typeof(ParamArrayAttribute), false);
            }
        }

        public override object DefaultValue {
            get {
                return parameter.DefaultValue;
            }
        }

        public override bool IsOptional {
            get {
                return parameter.IsOptional;
            }
        }

        public override string Namespace {
            get {
                return this.ns;
            }
        }

        public override string Name {
            get {
                return parameter.Name;
            }
        }

        public override Type PropertyType {
            get {
                return parameter.ParameterType;
            }
        }

        // TODO Semantics of read-only and value aren't defined for parameters

        public override bool IsReadOnly {
            get {
                return true;
            }
        }

        public override object GetValue(object component, object ancestor, QualifiedName name) {
            return null;
        }

        public override void SetValue(object component, object ancestor, QualifiedName name, object value) {
        }
    }

}

