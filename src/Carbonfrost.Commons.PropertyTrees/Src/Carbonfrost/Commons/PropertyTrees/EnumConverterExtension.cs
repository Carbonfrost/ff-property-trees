//
// - EnumConverterExtension.cs -
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
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class EnumConverterExtension : EnumConverter {

        private static readonly char[] WS =  {
            ' ',
            ',', // sic
            '\t',
            '\r',
            '\n'
        };

        // TODO Use a weak cache here - no need to hold onto these
        private static readonly IDictionary<Type, TypeConverter> map = new Dictionary<Type, TypeConverter>();

        public EnumConverterExtension(Type t) : base(t) {
        }

        public static TypeConverter Instance(Type t) {
            return map.GetValueOrCache(t, _ => new EnumConverterExtension(t));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            string text = value as string;

            if (text != null && text.IndexOfAny(WS) >= 0) {
                try {

                    long num = 0;
                    foreach (var a in text.Split(WS, StringSplitOptions.RemoveEmptyEntries)) {
                        num |= Convert.ToInt64((Enum) Enum.Parse(this.EnumType, a, /* ignoreCase */ true), culture);
                    }

                    return Enum.ToObject(this.EnumType, num);

                } catch (Exception) {
                    // Let framework handle this by retrying
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

}
