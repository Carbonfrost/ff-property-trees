//
// - PropertyTreeSchemaVisitor{T0,T1}.cs -
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

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    public abstract class PropertyTreeSchemaVisitor<TArgument, TResult> {

        public TResult Visit(PropertyNodeDefinition node, TArgument argument) {
            if (node == null)
                throw new ArgumentNullException("node");

            return node.AcceptVisitor(this, argument);
        }

        protected virtual TResult DefaultVisit(PropertyNodeDefinition node, TArgument argument) {
            return default(TResult);
        }

        protected internal virtual TResult VisitPropertyTree(PropertyTreeDefinition node,
                                                                       TArgument argument) {
            return DefaultVisit(node, argument);
        }

        protected internal virtual TResult VisitProperty(PropertyDefinition node,
                                                                   TArgument argument) {
            return DefaultVisit(node, argument);
        }

        protected internal virtual TResult VisitPropertyTreeFactory(PropertyTreeFactoryDefinition node, TArgument argument) {
            return DefaultVisit(node, argument);
        }

        protected internal virtual TResult VisitClearOperator(ClearOperatorDefinition node, TArgument argument) {
            return DefaultVisit(node, argument);
        }

        protected internal virtual TResult VisitRemoveOperator(RemoveOperatorDefinition node, TArgument argument) {
            return DefaultVisit(node, argument);
        }
    }
}
