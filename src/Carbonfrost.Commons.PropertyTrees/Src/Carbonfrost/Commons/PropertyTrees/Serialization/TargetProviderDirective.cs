//
// - TargetProviderDirective.cs -
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
using System.Reflection;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class TargetProviderDirective : PropertyProvider {

        readonly IDictionary<string, string> values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public AssemblyName Assembly {
            get;
            set;
        }

        public string this[string property] {
            get {
                if (property == null)
                    throw new ArgumentNullException("property");
                if (string.IsNullOrEmpty(property))
                    throw Failure.EmptyString("property");

                return values.GetValueOrDefault(property);
            }
            set {
                if (property == null)
                    throw new ArgumentNullException("property");
                if (string.IsNullOrEmpty(property))
                    throw Failure.EmptyString("property");

                values[property] = value;
            }
        }

        public QualifiedName Name {
            get;
            set;
        }

        protected override bool TryGetPropertyCore(string property, Type requiredType, out object value) {
            return base.TryGetPropertyCore(property, requiredType, out value)
                || _TryGetProperty(property, requiredType, out value);
        }

        private bool _TryGetProperty(string property, Type propertyType, out object value) {
            string s;
            value = null;

            if (values.TryGetValue(property, out s)) {
                value = s;

                // TODO Apply type conversion
                return true;
            }
            return false;
        }

    }
}

