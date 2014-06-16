//
// - ReflectionMetaObject.cs -
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class ReflectionMetaObject : PropertyTreeMetaObject {

        private readonly object component;
        private readonly Type componentType;

        public ReflectionMetaObject(object component, Type componentType) {
            this.componentType = componentType;
            this.component = component;
        }

        public override Type ComponentType {
            get { return componentType; }
        }

        public override object Component {
            get {
                return component;
            }
        }

        public override PropertyTreeMetaObject BindAddChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            var result = definition.Apply(null, component, arguments);
            if (result == null)
                return Null;
            return CreateChild(result);
        }

        public override PropertyTreeMetaObject BindStreamingSource(StreamContext input, IServiceProvider serviceProvider) {
            var ss = StreamingSource.Create(this.ComponentType);

            if (ss == null) {
                var errors = serviceProvider.TryGetService(PropertyTreeBinderErrors.Default);
                errors.CouldNotBindStreamingSource(this.ComponentType, PropertyTreeBinderImpl.FindFileLocation(serviceProvider));
                return this;
            }

            // Hydrate the existing instance
            ss.Load(input, this.Component);
            return this;
        }

        public override void BindClearChildren(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            definition.Apply(null, component, arguments);
        }

        public override void BindRemoveChild(OperatorDefinition definition, IReadOnlyDictionary<string, PropertyTreeMetaObject> arguments) {
            definition.Apply(null, component, arguments);
        }

        public override void BindSetMember(PropertyDefinition property, QualifiedName name, PropertyTreeMetaObject value, PropertyTreeMetaObject ancestor, IServiceProvider serviceProvider) {
            if (property.IsReadOnly) {
                TryAggregation(value, name, property, serviceProvider);
                return;
            }

            object outputValue = value.Component;
            object anc = ancestor == null ? null : ancestor.Component;
            SetValueCore(property, name, anc, value, outputValue, serviceProvider);
        }

        private void SetValueCore(PropertyDefinition property,
                                  QualifiedName name,
                                  object ancestor,
                                  PropertyTreeMetaObject value,
                                  object outputValue,
                                  IServiceProvider serviceProvider) {

            var callback = serviceProvider.TryGetService(PopulateComponentCallback.Null);
            try {
                property.SetValue(component, ancestor, name, value.Component);

            } catch (NullReferenceException nre) {
                // Normally a "critical" exception, consider it a conversion error
                callback.OnConversionException(property.Name, outputValue, nre);

            } catch (ArgumentException a) {
                callback.OnConversionException(property.Name, outputValue, a);

            } catch (Exception ex) {
                if (Require.IsCriticalException(ex))
                    throw;
                callback.OnConversionException(property.Name, outputValue, ex);
            }
        }

        static IEnumerable<Type> GetBestItemTypes(IEnumerable enumerable) {
            var enumerableTypes = enumerable.Cast<object>().Where(t => !ReferenceEquals(t, null)).Select(t => t.GetType()).Distinct();

            if (enumerableTypes.Count() == 1)
                yield return enumerableTypes.First();

            var available = Utility.EnumerateInheritedTypes(enumerableTypes.First());
            foreach (var type in available) {
                if (enumerableTypes.All(type.IsAssignableFrom))
                    yield return type;
            }
        }

        private static MethodInfo FindAddonMethod(Type type, IEnumerable enumerable) {
            var bestItemTypes = GetBestItemTypes(enumerable);

            foreach (var itemType in bestItemTypes) {
                var result = type.GetMethod("Add", new [] { itemType });
                if (result != null)
                    return result;
            }

            return null;
        }

        private void TryAggregation(PropertyTreeMetaObject value,
                                    QualifiedName name,
                                    PropertyDefinition property,
                                    IServiceProvider serviceProvider)
        {

            var current = property.GetValue(component, name);

            var enumerable = value.Component as IEnumerable;
            if (enumerable != null) {

                var items = enumerable;
                if (!ReferenceEquals(current, items) && enumerable.GetEnumerator().MoveNext()) {
                    MethodInfo mi = FindAddonMethod(current.GetType(), enumerable);

                    if (mi == null) {
                        var errors = serviceProvider.TryGetService(PropertyTreeBinderErrors.Default);
                        errors.NoAddMethodSupported(component.GetType(), PropertyTreeBinderImpl.FindFileLocation(serviceProvider));
                        return;
                    }

                    foreach (var item in items) {
                        mi.Invoke(current, new object[] { item });
                    }
                }
            }
            return;
        }

        public override PropertyTreeMetaObject BindInitializeValue(string text, IServiceProvider serviceProvider) {
            object value;
            TryConvertFromText(text, serviceProvider, out value);
            return PropertyTreeMetaObject.Create(value);
        }
    }

}
