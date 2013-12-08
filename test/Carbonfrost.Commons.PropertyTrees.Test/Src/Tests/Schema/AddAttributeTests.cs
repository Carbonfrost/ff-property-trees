//
// - AddAttributeTests.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.ObjectModel;
using System.Linq;

using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;

namespace Tests.Schema {

    [TestFixture]
    public class AddAttributeTests {

        sealed class HelperBuilderCollection {}

        [Test]
        public void addon_method_implicit_collection() {
            string name = AddAttribute.TrimImplicitAdd("AddNew", typeof(PropertyNodeCollection));
            Assert.That(name, Is.EqualTo("propertyNode"));
        }

        [Test]
        public void addon_method_implicit_builder_collection() {
            string name = AddAttribute.TrimImplicitAdd("AddNew", typeof(HelperBuilderCollection));
            Assert.That(name, Is.EqualTo("helper"));
        }

        [Test]
        public void natural_name_generic() {
            string name = AddAttribute.GetNaturalName(typeof(List<string>));
            Assert.That(name, Is.EqualTo("String"));

            name = AddAttribute.GetNaturalName(typeof(Collection<string>));
            Assert.That(name, Is.EqualTo("String"));
        }

        // TODO Support Dictionary<,> natural name - entry
    }
}

