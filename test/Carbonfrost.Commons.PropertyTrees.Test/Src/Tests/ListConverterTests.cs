//
// - ListConverterTests.cs -
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
using System.Collections.ObjectModel;
using System.Globalization;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using NUnit.Framework;
using Prototypes;
using Tests;
namespace Tests {

    [TestFixture]
    public class ListConverterTests {

        [Test]
        public void ConvertFromString_should_parse_closed_generic_type_collection_interface() {
            var conv = TypeHelper.GetConverter(null, typeof(IList<Glob>));
            Assert.That(conv, Is.InstanceOf<ListConverter>());

            var items = conv.ConvertFromString("**/*.* abc/**/*.txt \t\t\r\n */.cs");
            // TODO Should the return type be Collection<Glob> rather than List?  (Currently,
            // we only use converters for aggregation - not directly)
            Assert.That(items, Is.InstanceOf<List<Glob>>());
            Assert.That(items, Has.Count.EqualTo(3));
            Assert.That(items, Contains.Item(Glob.Anything));
        }

        [Test]
        public void ConvertFromString_should_parse_closed_generic_type_derived_collection() {
            var conv = TypeHelper.GetConverter(null, typeof(Collection<Glob>));
            Assert.That(conv, Is.InstanceOf<ListConverter>());

            var items = conv.ConvertFromString("**/*.* abc/**/*.txt \t\t\r\n */.cs");
            Assert.That(items, Has.Count.EqualTo(3));
            Assert.That(items, Contains.Item(Glob.Anything));
        }

    }
}


