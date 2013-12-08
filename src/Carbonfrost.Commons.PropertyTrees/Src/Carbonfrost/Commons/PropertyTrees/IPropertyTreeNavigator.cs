//
// - IPropertyTreeNavigator.cs -
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
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    interface IPropertyTreeNavigator {

        // This interface mainly ensures that PropertyTreeReader,
        // PropertyNode, and PropertyTreeNavigator have similar public APIs

        int Position { get; }
        string Name { get; }
        string Namespace { get; }
        QualifiedName QualifiedName { get; }

        object Value { get; }
        PropertyNodeType NodeType { get; }

        bool HasChildren { get; }
        string Path { get; }
        bool IsProperty { get; }
        bool IsPropertyTree { get; }
        int Depth { get; }

        Type ValueType { get; }
        PropertyNodeDefinition Definition { get; }
        int LineNumber { get; }
        int LinePosition { get; }

        // TODO Bind? Add here?

    }
}
