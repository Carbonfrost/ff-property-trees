//
// - TypeHelper.cs -
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees {

    static class TypeHelper {

        public static MethodBase FindActivationConstructor(Type declaringType) {
            MethodBase ctor = declaringType.GetActivationConstructor();
            if (ctor == null)
                ctor = declaringType.GetConstructors().FirstOrDefault();

            return ctor;
        }

        public static Type GetReturnType(MethodBase method) {
            if (method.IsConstructor)
                return ((ConstructorInfo) method).DeclaringType;
            else
                return ((MethodInfo) method).ReturnType;
        }

        public static TypeConverter GetConverter(PropertyDefinition property, Type neededType) {
            TypeConverter conv = null;
            if (property != null && !(property.Converter is ReferenceConverter))
                conv = property.Converter;

            if (conv == null)
                conv = TypeDescriptor.GetConverter(neededType);

            // On .NET, ReferenceConverter is returned
            if (conv == null || conv is ReferenceConverter) {
                if (neededType.IsGenericType
                    && !neededType.IsGenericTypeDefinition
                    && (neededType.GetGenericTypeDefinition() == typeof(IList<>)
                        || neededType.GetGenericTypeDefinition() == typeof(List<>)))
                    return ListConverter.Instance(neededType.GetGenericArguments()[0]);
            }

            if (conv is EnumConverter)
                return EnumConverterExtension.Instance(neededType);

            if (conv is BooleanConverter)
                return BooleanConverterExtension.Instance;

            return conv;
        }

        public static Uri ConvertToUri(object value) {
            // TODO Need a uri context?
            if (object.ReferenceEquals(value, null))
                return null;
            else
                return new Uri(value.ToString());
        }

        public static TimeSpan ConvertToTimeSpan(object value) {
            if (object.ReferenceEquals(value, null))
                return TimeSpan.Zero;
            else
                return TimeSpan.Parse(value.ToString());
        }

        public static Type TypeOf(object value, Type fallback = null) {
            if (value == null)
                return fallback ?? typeof(object);
            else
                return value.GetType();
        }

        public static Type ConvertToType(object value, IServiceProvider context) {
            if (value == null)
                return null;

            string s = value as string;
            if (s != null)
                return TypeReference.Parse(s, context).Resolve();

            return value as Type;
        }

        public static string GetNamespaceName(Type type) {
            if (type.IsGenericType && !type.IsGenericTypeDefinition) {
                var def = type.GetGenericTypeDefinition();

                if (def == typeof(IAddChild<>) || def == typeof(IEnumerable<>) || def == typeof(ICollection<>) || def == typeof(IList<>))
                    return GetNamespaceName(type.GetGenericArguments()[0]);

                else if (def == typeof(IDictionary<,>) || def == typeof(KeyValuePair<,>))
                    return GetNamespaceName(type.GetGenericArguments()[1]);

                else
                    return string.Empty;
            }

            if (type.IsArray || type.IsByRef || type.IsPointer)
                return GetNamespaceName(type.GetElementType());

            return type.GetQualifiedName().NamespaceName;
        }
    }
}

