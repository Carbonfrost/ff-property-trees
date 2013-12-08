//
// - PropertyNodeTests.cs -
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
using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;
using Prototypes;

namespace Tests {

    [TestFixture]
    public class PropertyNodeTests : TestBase {

        [Test]
        public void test_position_nested() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.That(pt.Root.Position, Is.EqualTo(0));
            Assert.That(pt.Root.Name, Is.EqualTo("items"));
            Assert.That(pt.Children.Count, Is.EqualTo(5));

            Assert.That(pt.FirstChild.Position, Is.EqualTo(0));
            Assert.That(pt.Children[1].Position, Is.EqualTo(1));
            Assert.That(pt.Children[1].FirstChild.Position, Is.EqualTo(0));
        }

        [Test]
        public void test_position_siblings() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.That(pt.Root.Position, Is.EqualTo(0));
            Assert.That(pt.FirstChild.Position, Is.EqualTo(0));
            Assert.That(pt.Children[1].NextSibling.Position, Is.EqualTo(2));
            Assert.That(pt.Children[1].NextSibling.NextSibling.Position, Is.EqualTo(3));
        }

        [Test]
        public void test_children_count() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Assert.True(pt.Children[1].HasChildren);
            Assert.That(pt.Children[1].Children.Count, Is.EqualTo(22));
            Assert.That(pt.Children[1].NextSibling.Children.Count, Is.EqualTo(0));
            Assert.That(pt.Children[1].NextSibling.NextSibling.Children.Count, Is.EqualTo(1));
        }

        [Test]
        public void test_is_root_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.True(pt.Root.IsRoot);
            Assert.False(pt.FirstChild.IsRoot);
        }

        [Test]
        public void test_parent_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            var p1 = pt.Root.Children[1];
            var p2 = pt.Root.Children[1].FirstChild;

            Assert.That(p1.Parent, Is.SameAs(pt.Root));
            Assert.That(p1.Root, Is.SameAs(pt.Root));
            Assert.That(p2.Parent, Is.SameAs(p1));
            Assert.That(p2.Root, Is.SameAs(pt.Root));
        }
    }
}
