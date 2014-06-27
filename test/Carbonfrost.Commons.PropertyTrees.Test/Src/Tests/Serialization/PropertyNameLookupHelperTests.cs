//
// - PropertyNameLookupHelperTests.cs -
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
using System.Linq;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.PropertyTrees.Serialization;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime.Components;
using NUnit.Framework;

namespace Tests.Serialization {

    [TestFixture]
    public class PropertyNameLookupHelperTests {

        [Test]
        public void FindOperator_should_implicitly_handle_inherited_operator_names() {
            var unit = new PropertyNameLookupHelper();
            var comp = PropertyTreeDefinition.FromType(typeof(ComponentCollection));

            // Though add is defined inside default NS, it is accessible via the NS of the type
            var qn = QualifiedName.Create(comp.Namespace, "add");
            var result = unit.FindOperator(comp, typeof(ComponentCollection), qn);
            Assert.That(result, Is.Not.Null);
        }
    }
}
