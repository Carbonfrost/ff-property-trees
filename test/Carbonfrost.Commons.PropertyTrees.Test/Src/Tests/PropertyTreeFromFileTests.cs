//
// - PropertyTreeFromFileTests.cs -
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
using System.Linq;
using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;

namespace Tests {

    [TestFixture]
    public class PropertyTreeFromFileTests : TestBase {

        // TODO Whitespace handling tests

        [Test]
        public void get_simple_tree_from_file() {
            PropertyTree pt = PropertyTree.FromFile(GetContentPath("beta.xml"));
            Assert.That(pt.Name, Is.EqualTo("beta"));

            Assert.That(pt.Children.Select(c => c.Name).ToArray(), Is.EquivalentTo(new [] { "c", "d", "a", "b" }));
            Assert.That(pt.Children["c"], Is.InstanceOf(typeof(Property)));
            Assert.That(pt.Children["a"], Is.InstanceOf(typeof(PropertyTree)));
            Assert.That(pt.Children.Count, Is.EqualTo(4));
            Assert.That(pt.Children["a"]["e"].Value, Is.EqualTo("3/30/2011 1:50 AM"));

        }

    }
}

