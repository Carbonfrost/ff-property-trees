//
// - PropertyTreeNodeWriterTest.cs -
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
    public class PropertyTreeNodeWriterTest : TestBase {

        // TODO Do some invald documents
        // TODO Error messages on invalid documents, including on invalid XML reading should not be obscure, and should include line pos

        [Test]
        public void trivial_document() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteEndDocument();

            Assert.That(writer.Root, Is.Null);
        }

        [Test]
        public void document() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteStartTree("hello");
            writer.WriteStartTree("george");
            writer.WriteEndTree();
            writer.WriteEndTree();
            writer.WriteEndDocument();

            Assert.That(writer.Root.Name, Is.EqualTo("hello"));
            Assert.That(writer.Root.Children.Count, Is.EqualTo(1));
            Assert.That(writer.Root.Children[0].Name, Is.EqualTo("george"));
            Assert.That(writer.Root.Children["george"], Is.Not.Null);
            Assert.That(writer.Root["george"], Is.Not.Null);
        }

        [Test]
        public void document2() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteStartTree("hello");
            writer.WriteStartProperty("george");
            writer.WritePropertyValue("burdell");
            writer.WriteEndProperty();
            writer.WriteEndTree();
            writer.WriteEndDocument();

            Assert.That(writer.Root.Children.Count, Is.EqualTo(1));
            Assert.That(writer.Root.Children["george"], Is.Not.Null);
            Assert.That(writer.Root["george"], Is.Not.Null);
            Assert.That(writer.Root["george"].Value, Is.EqualTo("burdell"));
        }


        [Test]
        public void document3() {
            PropertyTreeNodeWriter writer = new PropertyTreeNodeWriter();
            writer.WriteStartDocument();
            writer.WriteStartTree("hello");

            writer.WriteStartProperty("george");
            writer.WritePropertyValue("burdell");
            writer.WriteEndProperty();

            writer.WriteStartProperty("buzz");
            writer.WritePropertyValue("234");
            writer.WriteEndProperty();

            writer.WriteProperty("hey", "arnold");

            writer.WriteEndTree();
            writer.WriteEndDocument();

            Assert.That(writer.Root.Children.Count, Is.EqualTo(3));
            Assert.That(writer.Root.Children.Select(t => t.Name).ToArray(), Is.EquivalentTo(new [] { "george", "buzz", "hey" }));
            Assert.That(writer.Root.Children["buzz"], Is.Not.Null);
            Assert.That(writer.Root["buzz"], Is.Not.Null);
        }

    }
}
