//
// - PropertyTreePathTests.cs -
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
using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;
using IgnoreAttribute = NUnit.Framework.IgnoreAttribute;

namespace Tests {

    [TestFixture, Ignore]
    public class PropertyTreePathTests {

        [Test]
        public void parse_simple_expression() {
            PropertyTreePath.Parse("alpha");
        }

        [Test]
        public void parse_simple_child_expression() {
            PropertyTreePath.Parse("/lm");
        }

        [Test]
        public void parse_child_expression_multi() {
            PropertyTreePath.Parse("/lm/alpha");
        }

        [Test]
        public void parse_indexer_expression() {
            PropertyTreePath.Parse("/lm/alpha[2]");
        }

        [Test]
        public void parse_value_attribute_expression() {
            PropertyTreePath.Parse("/lm/alpha[2][@value]");
        }
    }
}
