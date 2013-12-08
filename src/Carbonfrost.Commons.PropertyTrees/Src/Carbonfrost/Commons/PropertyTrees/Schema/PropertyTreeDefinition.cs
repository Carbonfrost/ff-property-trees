//
// - PropertyTreeDefinition.cs -
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.ComponentModel.Annotations;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    [ConcreteClass(typeof(DynamicPropertyTreeDefinition))]
    public abstract class PropertyTreeDefinition : PropertyNodeDefinition {

        private readonly static IDictionary<Type, PropertyTreeDefinition> map = new Dictionary<Type, PropertyTreeDefinition>();

        public abstract OperatorDefinitionCollection Operators { get; }
        public abstract PropertyTreeFactoryDefinition Constructor { get; }
        public abstract PropertyDefinitionCollection Properties { get; }
        public abstract PropertyDefinition DefaultProperty { get; }
        public abstract Type SourceClrType { get; }
        public abstract PropertyTreeSchema Schema { get; }

        public static PropertyTreeDefinition FromType(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            // TODO Optimize: Don't create defs for types that wouldn't be usable.
            return map.GetValueOrCache(type, t => new ReflectedPropertyTreeDefinition(t));
        }

        public static PropertyTreeDefinition FromValue(object component) {
            if (component == null)
                throw new ArgumentNullException("component");

            // TODO Handle using TypeDescriptor
            return FromType(component.GetType());
        }

        internal sealed override TResult AcceptVisitor<TArgument, TResult>(PropertyTreeSchemaVisitor<TArgument, TResult> visitor, TArgument argument) {
            return visitor.VisitPropertyTree(this, argument);
        }

        internal sealed override void AcceptVisitor(PropertyTreeSchemaVisitor visitor) {
            visitor.VisitPropertyTree(this);
        }

        public abstract IEnumerable<OperatorDefinition> EnumerateOperators(bool declaredOnly = false);
        public abstract IEnumerable<PropertyDefinition> EnumerateProperties(bool declaredOnly = false);

        public OperatorDefinition GetOperator(string name, string ns, bool declaredOnly = false) {
            return GetOperator(QualifiedName.Create(ns, name), declaredOnly);
        }

        public abstract OperatorDefinition GetOperator(string name, bool declaredOnly = false);
        public abstract OperatorDefinition GetOperator(QualifiedName name, bool declaredOnly = false);

        public PropertyDefinition GetProperty(string name, string ns, bool declaredOnly = false) {
            return GetProperty(QualifiedName.Create(ns, name), declaredOnly);
        }

        public abstract PropertyDefinition GetProperty(QualifiedName name, bool declaredOnly = false);
        public abstract PropertyDefinition GetProperty(string name, bool declaredOnly = false);

    }
}
