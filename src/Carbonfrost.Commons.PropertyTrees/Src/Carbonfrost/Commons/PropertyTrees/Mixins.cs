//
// - Mixins.cs -
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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    internal static class Mixins {

        public static TValue GetValueOrCache<TKey, TValue>(
            this IDictionary<TKey, TValue> source, TKey key,
            Func<TKey, TValue> factory = null)
        {
            TValue result;
            if (!source.TryGetValue(key, out result)) {
                if (factory == null)
                    result = default(TValue);
                else
                    result = factory(key);

                source[key] = result;
            }

            return result;
        }

        public static bool IsExtension(this ICustomAttributeProvider any) {
            return any.IsDefined(typeof(ExtensionAttribute), false);
        }

        public static IEnumerable<T> ByLocalName<T>(this IEnumerable<T> items, string name)
            where T : PropertyNodeDefinition
        {
            return items.Where(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<Type> GetTypesHelper(this Assembly a) {
            try {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException e) {
                return e.Types.Where(t => t != null);
            }
        }

    }
}
