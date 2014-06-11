//
// - Utility.cs -
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
using System.IO;
using System.Linq;
using System.Xml;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.ComponentModel;

namespace Carbonfrost.Commons.PropertyTrees {

    static class Utility {

        public static readonly IXmlLineInfo NullLineInfo = new NullLineInfoImpl();
        public static readonly INameScope NullNameScope = new NullNameScopeImpl();
        public static readonly IXmlNamespaceResolver NullResolver = new NullXmlNamespaceResolverImpl();
        public static readonly IUriContext NullUriContext = new NullUriContextImpl();

        public static readonly IEqualityComparer<QualifiedName> OrdinalIgnoreCaseQualifiedName = new OrdinalIgnoreCaseQualifiedNameImpl();

        static readonly HashSet<Type> PROPERTY_TYPES = new HashSet<Type> {
            typeof(Uri), typeof(TimeSpan), typeof(DateTimeOffset),
        };

        private const int MAX_LENGTH = 40;

        public static IEnumerable<Type> EnumerateInheritedTypes(Type sourceClrType) {
            return sourceClrType.GetInterfaces()
                .Concat(EnumerateInheritedBaseTypes(sourceClrType));
        }

        private static IEnumerable<Type> EnumerateInheritedBaseTypes(Type sourceClrType) {
            var type = sourceClrType.BaseType;
            while (type != null) {
                yield return type;
                type = type.BaseType;
            }
        }

        public static PropertyTreeFlavor InferFlavor(string file) {
            string ext = Path.GetExtension(file);
            if (string.IsNullOrEmpty(ext))
                return PropertyTreeFlavor.Unknown;

            switch (ext.ToLowerInvariant()) {
                case ".pt":
                case ".ptx":
                case ".xml":
                    return PropertyTreeFlavor.Xml;
                default:
                    return PropertyTreeFlavor.Unknown;
            }
        }

        public static bool IsProperty(object value) {
            switch (Type.GetTypeCode(value.GetType())) {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;

                case TypeCode.Object:
                    return PROPERTY_TYPES.Contains(value.GetType());

                case TypeCode.DBNull:
                default:
                    return false;
            }
        }

        sealed class NullNameScopeImpl : INameScope {
            object INameScope.FindName(string name) { return null; }
            void INameScope.RegisterName(string name, object scopedElement) {}
            void INameScope.UnregisterName(string name) {}
        }

        sealed class NullUriContextImpl : IUriContext {
            public Uri BaseUri {
                get { return null; }
                set {}
            }
        }

        sealed class NullLineInfoImpl : IXmlLineInfo {
            public int LineNumber { get { return -1; } }
            public int LinePosition { get { return -1; } }
            public bool HasLineInfo() { return false; }
        }

        public static string CamelCase(string text) {
            if (string.IsNullOrEmpty(text))
                return text;
            else if (text.Length == 1)
                return text.ToLowerInvariant();
            else
                return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }

        struct SimpleLineInfo : IXmlLineInfo {
            public int LineNumber { get; set; }
            public int LinePosition { get; set; }
            public bool HasLineInfo() { return true; }
        }

        sealed class OrdinalIgnoreCaseQualifiedNameImpl : IEqualityComparer<QualifiedName> {

            public bool Equals(QualifiedName x, QualifiedName y) {
                return (x == y) || (x.Namespace == y.Namespace && string.Equals(x.LocalName, y.LocalName, StringComparison.OrdinalIgnoreCase));
            }

            public int GetHashCode(QualifiedName obj) {
                int hashCode = 0;

                unchecked {
                    hashCode += 1000000009 * obj.LocalName.ToLowerInvariant().GetHashCode();
                    hashCode += 1000000021 * obj.Namespace.GetHashCode();
                }

                return hashCode;
            }
        }

        public static IXmlLineInfo CreateLineInfo(int lineNumber, int linePosition) {
            return new SimpleLineInfo { LineNumber = lineNumber, LinePosition = linePosition };
        }

        public static DateTime FromUnix(long time) {
            return new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(time));
        }

        public static string DisplayName(QualifiedName name) {
            return DisplayName(name.LocalName, name.NamespaceName);
        }

        public static string DisplayName(string name, string ns) {
            string displayName = name;
            if (!string.IsNullOrEmpty(ns)) {
                displayName = string.Format("{1} ({0})", ns, name);
            }
            return displayName;
        }
    }
}
