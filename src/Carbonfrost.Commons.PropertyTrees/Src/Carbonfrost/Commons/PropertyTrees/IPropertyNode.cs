//
// - IPropertyNode.cs -
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

namespace Carbonfrost.Commons.PropertyTrees {

    interface IPropertyNode : IPropertyTreeNavigator, IPropertyTreeReader {

        // This interface mainly ensures that
        // PropertyNode and PropertyTreeNavigator have similar public APIs

        PropertyTreeWriter AppendChild();
        void AppendChild(PropertyNode node);
        void AppendChild(PropertyTreeReader newChild);
        void AppendChild(PropertyTreeNavigator newChild);
        void AppendPropertyTree(string localName, string ns);
        void AppendPropertyTree(string localName);
        void AppendProperty(string localName, string ns, object value);
        void AppendProperty(string localName, object value);
        void AppendProperty(string localName);

        PropertyTreeNavigator CreateNavigator();
        void RemoveChildren(PropertyTreeNavigator lastSiblingToDelete);
        void RemoveSelf();
        PropertyTreeWriter InsertAfter();
        PropertyTreeWriter InsertBefore();
        void InsertBefore(PropertyTreeNavigator newSibling);

    }
}
