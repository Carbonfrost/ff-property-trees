//
// - ListConverter.cs -
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class ListConverter : TypeConverter {

        private readonly Type itemType;

        private static readonly char[] WS =  {
            ' ',
            '\t',
            '\r',
            '\n'
        };

        // TODO Use a weak cache here - no need to hold onto these
        private static readonly IDictionary<Type, TypeConverter> map = new Dictionary<Type, TypeConverter>();

        public ListConverter(Type t) {
            this.itemType = t;
        }

        public static TypeConverter Instance(Type t) {
            return map.GetValueOrCache(t, _ => new ListConverter(t));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            Type listType = typeof(List<>).MakeGenericType(itemType);
            string s = (string) value;

            // TODO Support correct tokenization (probably similar to Properties.ParseKeyValuePairs())
            string[] items = s.Split(WS, StringSplitOptions.RemoveEmptyEntries);
            Array array = Array.CreateInstance(itemType, items.Length);

            int index = 0;
            foreach (string m in items) {
                var item = TypeDescriptor.GetConverter(itemType).ConvertFromString(m);
                array.SetValue(item, index++);
            }

            return Activator.CreateInstance(listType, array);
        }
    }
}
