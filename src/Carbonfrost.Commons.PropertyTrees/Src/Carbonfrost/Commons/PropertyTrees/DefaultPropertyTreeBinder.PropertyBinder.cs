//
// - DefaultPropertyTreeBinder.PropertyBinder.cs -
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
using System.Linq;

using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class DefaultPropertyTreeBinder {

        class PropertyBinder : DefaultPropertyTreeBinder {

            public override object Bind(PropertyTreeBindingContext context, PropertyTreeNavigator navigator) {
                Type neededType = context.ComponentType;
                PropertyDefinition property = context.Property;
                object value = navigator.Value;

                if (value == null || neededType == null)
                    return null;

                try {
                    TypeConverter conv = TypeHelper.GetConverter(property, neededType);
                    return conv.ConvertFrom(value);

                } catch (Exception ex) {
                    SetLineInfo(context, navigator);
                    context.Callback.OnConversionException(property.Name, value, ex);
                    return null;
                }
            }

        }
    }

}
