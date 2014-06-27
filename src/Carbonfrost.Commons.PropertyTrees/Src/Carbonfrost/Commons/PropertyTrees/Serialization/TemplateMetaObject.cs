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

    partial class TemplateMetaObject : PropertyTreeMetaObject {

        private readonly List<ITemplateCommand> _items = new List<ITemplateCommand>();

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

        internal override bool ShouldConstruct {
            get {
                return false;
            }
        }

        internal override bool ShouldBindChildren {
            get {
                return true;
            }
        }

        private TemplateMetaObject(TemplateMetaObject parent, Type componentType) : base(parent) {
            if (componentType.IsGenericType && componentType.GetGenericTypeDefinition() == typeof(ITemplate<>))
                throw new ArgumentException();

            this.componentType = componentType;
        }

        private void AppendCommand(ITemplateCommand cmd) {
            if (this.Parent == null)
                _items.Add(cmd);
            else
                ((TemplateMetaObject) this.Parent).AppendCommand(cmd);
        }

        public static TemplateMetaObject FromTemplateType(Type componentType) {
            // Assume ITemplate<T> is the input
            var instType = componentType.GetGenericArguments()[0];
            if (instType.GetActivationConstructor() == null) {
                throw PropertyTreesFailure.TemplateTypeConstructorMustBeNiladic("componentType", componentType);
            }
            return new TemplateMetaObject(null, instType);
        }

        public static TemplateMetaObject FromInstanceType(TemplateMetaObject parent, Type componentType) {

            return new TemplateMetaObject(parent, componentType);
        }

        private TemplateMetaObject FromInstanceType(Type componentType) {
            return new TemplateMetaObject(this, componentType);
        }

        public override PropertyTreeMetaObject BindConstructor(OperatorDefinition definition,
                                                               IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            if (definition.Parameters.Count != 0) {
                // TODO This is an error because we can't do construction inside template
                throw new NotImplementedException();
            }
            return this;
        }

        public override PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value;
            TryConvertFromText(text, serviceProvider, out value);
            return PropertyTreeMetaObject.Create(value);
        }

        public override void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, PropertyTreeMetaObject ancestor, IServiceProvider serviceProvider) {
            if (property.IsReadOnly && value is TemplateMetaObject) {
                return;
            }

            AppendCommand(new SetPropertyCommand(property, name, value.Component));
        }

        public override PropertyTreeMetaObject BindAddChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            AppendCommand(new AddChildCommand(definition, arguments));
            return FromInstanceType(definition.ReturnType);
        }

        public override PropertyTreeMetaObject BindEndObject(IServiceProvider serviceProvider) {
            if (this.Parent == null) {
                var instType = this.ComponentType;
                var templateType = typeof(PropertyTreeTemplate<>).MakeGenericType(instType);

                this.component = Activator.CreateInstance(templateType, new object[] { _items.ToArray() });
            } else {
                AppendCommand(PopCommand.Instance);
            }

            return this;
        }

        public override PropertyTreeMetaObject CreateChild(object component, Type componentType) {
            throw new NotImplementedException();
        }

        public override PropertyTreeMetaObject CreateChild(PropertyDefinition property,
                                                           QualifiedName name,
                                                           PropertyTreeMetaObject ancestor) {

            if (property.IsReadOnly) {
                AppendCommand(new PushPropertyCommand(property, name));
                return FromInstanceType(property.PropertyType);
            }

            return CreateChild(property.PropertyType);
        }
    }
}
