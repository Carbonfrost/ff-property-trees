//
// - TypeHelperTests.cs -
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
using System.Linq;

using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;

namespace Tests {

    [TestFixture]
    public class TypeHelperTests {

        [Test]
        public void should_bind_generic_collection_ns() {
            var ns = TypeHelper.GetNamespaceName(typeof(ICollection<PropertyTree>));
            Assert.That(ns, Is.EqualTo(Xmlns.PropertyTrees2010));
        }

        [Test]
        public void should_bind_generic_dictionary_ns() {
            var ns = TypeHelper.GetNamespaceName(typeof(IDictionary<string, PropertyTree>));
            Assert.That(ns, Is.EqualTo(Xmlns.PropertyTrees2010));
        }

        [Test]
        public void should_bind_generic_dictionary_ns_recursive() {
            var ns = TypeHelper.GetNamespaceName(typeof(ICollection<KeyValuePair<string, PropertyTree>>));
            Assert.That(ns, Is.EqualTo(Xmlns.PropertyTrees2010));
        }

        [Test]
        public void IsParameterRequired_should_be_true_on_value_types() {
            Assert.True(TypeHelper.IsParameterRequired(typeof(bool)));
            Assert.False(TypeHelper.IsParameterRequired(typeof(Uri)));
        }

        [Test]
        public void IsParameterType_should_be_false_on_nullables() {
            Assert.False(TypeHelper.IsParameterRequired(typeof(bool?)));
        }
    }
}
