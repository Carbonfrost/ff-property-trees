//
// - PropertyTreeTests.cs -
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
using System.Linq;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;
using Prototypes;

namespace Tests {

    [TestFixture]
    public class PropertyTreeTests {

        [Test]
        public void copy_property_nominal() {
            Property a = new Property { Name = "a", Value = 420 };
            Property b = new Property { Name = "b", Value = 24 };
            b.CopyTo(a);

            Assert.That(a.Name, Is.EqualTo("b"));
            Assert.That(a.Value, Is.EqualTo(24));
        }

        [Test]
        public void copy_property_tree_nominal() {
            PropertyTree a = new PropertyTree { Name = "a" };
            a.AppendProperty("x", 300);
            a.AppendProperty("y", 300);

            PropertyTree b = new PropertyTree();
            a.CopyTo(b);

            Assert.That(b.Name, Is.EqualTo("a"));
            Assert.That(b.Children.Count, Is.EqualTo(2));
            Assert.That(b.Children["x"].Value, Is.EqualTo(300));
            Assert.That(b.Children["y"].Value, Is.EqualTo(300));
        }

        [Test]
        public void binding_from_object_into_tree_value() {
            PropertyTree tree = new PropertyTree();
            Alpha a = new Alpha();
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));
            tree.Value = a;

            Assert.That(tree.QualifiedName, Is.Null);
            Assert.That(tree.Children.Count, Is.EqualTo(typeof(Alpha).GetProperties().Length));
            Assert.That(tree.Children.Select(t => t.Name), Contains.Item("TT"));

            Assert.That(tree["TT"].Value, Is.EqualTo(a.TT));
            Assert.That(tree["A"].Value, Is.True);
        }

        [Test]
        public void binding_from_object_to_tree() {
            Alpha a = new Alpha();
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));

            PropertyTree pt = PropertyTree.FromValue(a);
            Assert.That(pt["A"].Value, Is.True);
            Assert.That(pt["TT"].Value, Is.EqualTo(a.TT));
        }

        [Test]
        public void copy_from_object_to_tree() {
            Alpha a = new Alpha();
            a.A = true;
            a.TT = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));

            PropertyTree p0 = new PropertyTree();
            PropertyTree pt = PropertyTree.FromValue(a);
            pt.CopyTo(p0);

            Assert.That(p0["A"].Value, Is.True);
            Assert.That(p0["TT"].Value, Is.EqualTo(a.TT));
        }

    }
}

