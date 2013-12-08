//
// - PropertyTreeFlattener.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Specialized;
using System.Text;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    class PropertyTreeFlattener : PropertyTreeVisitor {

        private NameValueCollection result;
        // TODO PropertyTreeFlattener

        public NameValueCollection DoVisit(PropertyNode node) {
            this.result = new PropertyCollection();
            Visit(node);
            return result;
        }

        protected internal override void VisitProperty(Property property) {
        }

        protected internal override void VisitPropertyTree(PropertyTree propertyTree) {
            foreach (var child in propertyTree.Children) {
                Visit(child);
            }
        }

        sealed class PropertyCollection : NameValueCollection {

            public override string ToString() {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Count; i++) {
                    if (sb.Length > 0)
                        sb.Append(';');

                    sb.Append(this.GetKey(i));
                    sb.Append('=');

                    // TODO Escaping in values
                    sb.Append(Get(i));
                }
                return sb.ToString();
            }

        }
    }
}
