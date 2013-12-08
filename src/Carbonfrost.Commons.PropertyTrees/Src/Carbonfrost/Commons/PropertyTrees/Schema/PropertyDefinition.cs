//
// - PropertyDefinition.cs -
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
using System.ComponentModel;
using System.Reflection;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class PropertyDefinition : PropertyNodeDefinition {

        public abstract Type PropertyType { get; set; }
        public abstract TypeConverter Converter { get; }
        public abstract bool IsReadOnly { get; set; }
        public abstract bool IsOptional { get; set; }
        public abstract object DefaultValue { get; set; }
        public abstract PropertyTreeDefinition DeclaringTreeDefinition { get; }

        public override string ToString() {
            return string.Format("{2}.{0}:{1}", Name, PropertyType, DeclaringTreeDefinition);
        }

        public virtual bool IsIndexer {
            get {
                return false;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public virtual bool IsParamArray {
            get {
                return false;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        internal sealed override TResult AcceptVisitor<TArgument, TResult>(PropertyTreeSchemaVisitor<TArgument, TResult> visitor, TArgument argument) {
            return visitor.VisitProperty(this, argument);
        }

        internal sealed override void AcceptVisitor(PropertyTreeSchemaVisitor visitor) {
            visitor.VisitProperty(this);
        }

        public abstract object GetValue(object component);
        public abstract void SetValue(object component, object value);
        public abstract object GetValue(object component, QualifiedName name);
        public abstract void SetValue(object component, QualifiedName name, object value);

        internal virtual PropertyDescriptor GetUnderlyingDescriptor() {
            return null;
        }
    }
}
