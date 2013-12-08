//
// - PropertyNodeManipulationTests.cs -
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
using Prototypes;

namespace Tests {

    [TestFixture]
    public class PropertyNodeManipulationTests : TestBase {

        [Test]
        public void test_append_to_nominal() {
            PropertyTree pt = new PropertyTree();
            Property p = new Property();
            p.AppendTo(pt);

            Assert.That(pt.FirstChild, Is.SameAs(p));
            Assert.That(p.Position, Is.EqualTo(0));
        }

        [Test]
        public void test_remove_child_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Assert.That(pt.Children[0].Name, Is.EqualTo("alpha1"));
            pt.RemoveChildAt(0);
            Assert.That(pt.Children[0].Name, Is.EqualTo("alpha2"));
        }

        [Test]
        public void test_insert_child_at_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Property property = new Property();

            Assert.That(pt.Children[1].Name, Is.EqualTo("alpha2"));
            pt.InsertChildAt(1, property);
            Assert.That(pt.Children[2].Name, Is.EqualTo("alpha2"));
            Assert.That(pt.Children[1], Is.SameAs(property));
            Assert.That(property.Position, Is.EqualTo(1));
        }

        [Test]
        public void test_insert_child_at_first_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Property property = new Property();

            Assert.That(pt.Children[0].Name, Is.EqualTo("alpha1"));
            pt.InsertChildAt(0, property);
            Assert.That(pt.Children[0], Is.SameAs(property));
            Assert.That(property.Position, Is.EqualTo(0));
        }

        [Test]
        public void test_insert_child_at_last_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Property property = new Property();
            int count = pt.Children.Count;

            Assert.That(pt.Children[count - 1].Name, Is.EqualTo("alpha5"));
            pt.InsertChildAt(count, property);

            Assert.That(pt.Children[count], Is.SameAs(property));
            Assert.That(property.Position, Is.EqualTo(count));
        }

        [Test]
        public void test_replace_with_nominal() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Property property = new Property();
            pt.Children[1].ReplaceWith(property);

            Assert.That(pt.Children[1], Is.SameAs(property));
            Assert.That(pt.FirstChild.NextSibling, Is.SameAs(property));
            Assert.That(pt.FirstChild.Position, Is.EqualTo(0));
            Assert.That(pt.FirstChild.NextSibling.Position, Is.EqualTo(1));
        }

        [Test]
        public void test_replace_first_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Property property = new Property();
            pt.FirstChild.ReplaceWith(property);

            Assert.That(pt.Children[0], Is.SameAs(property));
            Assert.That(pt.FirstChild.Position, Is.EqualTo(0));
            Assert.That(pt.FirstChild.NextSibling.Position, Is.EqualTo(1));
        }

        [Test]
        public void test_replace_last_child() {
            PropertyTree pt = LoadTree("alpha-list.xml");

            Property property = new Property();
            pt.LastChild.ReplaceWith(property);

            Assert.That(pt.Children[4], Is.SameAs(property));
            Assert.That(pt.LastChild, Is.SameAs(property));
            Assert.That(pt.LastChild.Position, Is.EqualTo(4));
            Assert.That(pt.LastChild.PreviousSibling.Position, Is.EqualTo(3));
        }
    }
}
