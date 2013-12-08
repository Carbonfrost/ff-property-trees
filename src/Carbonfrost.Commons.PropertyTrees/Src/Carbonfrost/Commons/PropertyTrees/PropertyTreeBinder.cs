//
// - PropertyTreeBinder.cs -
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
using System.Xml;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    public abstract partial class PropertyTreeBinder {

        public T Bind<T>(PropertyTree tree) {
            if (tree == null)
                throw new ArgumentNullException("tree");

            return (T) Bind(typeof(T), tree.CreateNavigator());
        }

        public T Bind<T>(PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return Bind<T>(reader.CreateNavigator());
        }

        public object Bind(Type componentType, PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return Bind(componentType, reader.CreateNavigator());
        }

        public object Bind(object component, PropertyTreeReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return Bind(component, reader.CreateNavigator());
        }

        public T Bind<T>(PropertyTreeNavigator navigator) {
            return (T) Bind(typeof(T), navigator);
        }

        public object Bind(Type componentType, PropertyTreeNavigator navigator) {
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            var context = new PropertyTreeBindingContext {
                ComponentType = componentType
            };
            return Bind(context, navigator);
        }

        public object Bind(object component, PropertyTreeNavigator navigator) {
            if (component == null)
                throw new ArgumentNullException("component"); // $NON-NLS-1
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            var context = new PropertyTreeBindingContext {
                Component = component,
                ComponentType = component.GetType(),
            };

            return Bind(context, navigator);
        }

        public abstract object Bind(PropertyTreeBindingContext context, PropertyTreeNavigator navigator);

        protected virtual void SetProperty(PropertyTreeBindingContext context,
                                           PropertyDefinition property,
                                           object value) {
            if (context == null)
                throw new ArgumentNullException("context");

            // TODO Decide how to handle errors here or null component

            if (context.Component != null) {
                try {
                    property.SetValue(context.Component, value);

                } catch (NullReferenceException nre) {
                    context.Callback.OnConversionException(property.Name, value, nre);

                } catch (ArgumentException a) {
                    context.Callback.OnConversionException(property.Name, value, a);

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;
                }
            }
        }
    }

}
