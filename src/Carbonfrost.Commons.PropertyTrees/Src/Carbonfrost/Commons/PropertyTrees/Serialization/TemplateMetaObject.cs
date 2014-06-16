//
// - TemplateMetaObject.cs -
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
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class TemplateMetaObject : PropertyTreeMetaObject {

        private readonly List<ITemplateCommand> items
            = new List<ITemplateCommand>();

        private object component;
        private readonly Type componentType;

        // Corresponds to the type of the template object (instead of ITemplate<>)
        public override Type ComponentType {
            get {
                return this.componentType;
            }
        }

        public override object Component {
            get {
                return component;
            }
        }

        public TemplateMetaObject(Type componentType) {
            this.componentType = componentType.GetGenericArguments()[0];
        }

        public override PropertyTreeMetaObject BindConstructor(OperatorDefinition definition,
                                                               IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            if (definition.Parameters.Count != 0) {
                // TODO This is an error because we can't do construction inside template
                throw new NotImplementedException();
            }
            return this;
        }

        public override void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, PropertyTreeMetaObject ancestor, IServiceProvider serviceProvider) {
            items.Add(new SetPropertyCommand(property, name, value.Component));
        }

        public override PropertyTreeMetaObject BindEndObject(IServiceProvider serviceProvider) {
            var instType = this.ComponentType;
            var templateType = typeof(PropertyTreeTemplate<>).MakeGenericType(instType);
            this.component = Activator.CreateInstance(templateType, new object[] { this.items.ToArray() });
            return this;
        }

        interface ITemplateCommand
        {
            void Apply(object component);
        }

        sealed class PropertyTreeTemplate<T> : ITemplate<T> {

            readonly IList<ITemplateCommand> items;

            public PropertyTreeTemplate(ITemplateCommand[] items) {
                this.items = items;
            }

            public void Initialize(T value) {
                foreach (var m in items)
                    m.Apply(value);
            }
        }

        class SetPropertyCommand : ITemplateCommand {

            readonly PropertyDefinition property;
            readonly QualifiedName name;
            readonly object value;

            public SetPropertyCommand(PropertyDefinition property, QualifiedName name, object value) {
                this.property = property;
                this.name = name;
                this.value = value;
            }

            void ITemplateCommand.Apply(object component) {
                property.SetValue(component, name, value);
            }
        }

    }
}
