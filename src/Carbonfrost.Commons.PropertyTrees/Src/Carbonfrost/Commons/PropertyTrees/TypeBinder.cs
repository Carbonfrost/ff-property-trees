//
// - TypeBinder.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    class TypeBinder : DefaultPropertyTreeBinder {

        internal static readonly PropertyTreeBinder Instance = new TypeBinder();

        public override object Bind(PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {
            if (context == null)
                throw new ArgumentNullException("context");
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            if (navigator.IsProperty) {
                object value = navigator.Value;
                object outputValue = null;

                try {
                    // Type parsing handled in the context of this
                    return outputValue = ConvertToType(navigator.Value, ServiceProvider.Compose(ServiceProvider.FromValue(navigator), context));

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    SetLineInfo(context, navigator);
                    context.Callback.OnConversionException(navigator.Name, value, ex);
                    return null;
                }

            } else {

                // TODO Support TypeBinder on trees
                throw FutureFeatures.TypeBinderOnTrees();
            }
        }

        private static Type ConvertToType(object value, IServiceProvider context) {
            return TypeHelper.ConvertToType(value, context);
        }
    }
}
