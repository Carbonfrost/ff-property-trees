//
// - Xmlns.cs -
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
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    static class Xmlns {

        public const string PropertyTrees2010 = "http://ns.carbonfrost.com/commons/propertytrees";
        public static readonly NamespaceUri PropertyTrees2010Uri = NamespaceUri.Create(PropertyTrees2010);
        public const string PropertyTreesSchema2010 = "http://ns.carbonfrost.com/commons/propertytrees/schema";
    }
}
