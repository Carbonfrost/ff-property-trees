//
// - ReflectedIndexerPropertyDefinition.cs -
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

    sealed class ReflectedIndexerPropertyDefinition : PropertyDefinition {

        private readonly PropertyInfo member;
        private readonly Func<QualifiedName, object[]> getIndexParams;

        public ReflectedIndexerPropertyDefinition(PropertyInfo member) {
            this.member = member;
            this.getIndexParams = GetIndexParamsHelper(member);
        }

        static Func<QualifiedName, object[]> GetIndexParamsHelper(PropertyInfo member) {
            var pi = member.GetIndexParameters();

            // QualifiedName
            if (pi[0].ParameterType == typeof(QualifiedName))
                return (q) => (new [] { q } );

            // string, string
            else if (pi.Length == 2)
                return (q) => (q == null ? new [] { string.Empty, string.Empty} : new [] { q.LocalName, q.NamespaceName } );

            // string
            else
                return (q) => (q == null ? new[] { string.Empty } : new [] { q.LocalName } );
        }

        public override PropertyTreeDefinition DeclaringTreeDefinition {
            get {
                return null;
            }
        }

        public override object DefaultValue {
            get {
                return null;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override bool IsParamArray {
            get {
                return false;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override Type PropertyType {
            get {
                return this.member.PropertyType;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override bool IsIndexer {
            get {
                return true;
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

        public override TypeConverter Converter {
            get {
                return TypeDescriptor.GetConverter(typeof(object));
            }
        }

        public override string Namespace {
            get {
                return string.Empty;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override string Name {
            get {
                return this.member.Name;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override bool IsReadOnly {
            get {
                return !this.member.CanWrite;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override object GetValue(object component) {
            return GetValue(component, null);
        }

        public override void SetValue(object component, object value) {
            SetValue(component, null, value);
        }

        public override void SetValue(object component, QualifiedName name, object value) {
            member.SetValue(component, value, getIndexParams(name));
        }

        public override object GetValue(object component, QualifiedName name) {
            return member.GetValue(component, getIndexParams(name));
        }

    }
}
