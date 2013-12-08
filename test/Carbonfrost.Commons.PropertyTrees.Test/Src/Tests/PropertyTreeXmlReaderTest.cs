//
// - PropertyTreeXmlReaderTest.cs -
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
    public class PropertyTreeXmlReaderTest : TestBase {

        [Test]
        public void node_types() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));

            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial));
            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.PropertyTree));
            Assert.That(reader.Name, Is.EqualTo("beta"));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("c"));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("d"));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.PropertyTree));
            Assert.That(reader.Name, Is.EqualTo("a"));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("a"));

            // Additional reads
            TestUtils.Times(() => reader.Read(), 5);

            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.EndPropertyTree));
            Assert.That(reader.Name, Is.EqualTo("a"));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.PropertyTree));
            Assert.That(reader.Name, Is.EqualTo("b"));

            Assert.That(reader.Read());
            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.EndPropertyTree));
            Assert.That(reader.Name, Is.EqualTo("b"));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.EndPropertyTree));
            Assert.That(reader.Name, Is.EqualTo("beta"));
        }

        [Test]
        public void depth_and_position() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));

            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial));
            Assert.That(reader.Read());
            Assert.That(reader.Depth, Is.EqualTo(0));
            Assert.That(reader.Position, Is.EqualTo(0));

            Assert.That(reader.Read());
            Assert.That(reader.Depth, Is.EqualTo(1));
            Assert.That(reader.Position, Is.EqualTo(0));

            Assert.That(reader.Read());
            Assert.That(reader.Depth, Is.EqualTo(1)); // d
            Assert.That(reader.Position, Is.EqualTo(1));

            Assert.That(reader.Read());
            Assert.That(reader.Depth, Is.EqualTo(1)); // a
            Assert.That(reader.Position, Is.EqualTo(2));

            Assert.That(reader.Read());
            Assert.That(reader.Depth, Is.EqualTo(2));
            Assert.That(reader.Position, Is.EqualTo(0)); // a.a

            // 9 additional reads
            TestUtils.Times(() => reader.Read(), 9);

            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.ReadState, Is.EqualTo(ReadState.EndOfFile));
        }

        [Test]
        public void read_property_tree_implicitly_moves_initial() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));
            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial));
            PropertyTree tree = reader.ReadPropertyTree();
            Assert.That(tree.Name, Is.EqualTo("beta"));
        }

        [Test]
        public void read_property_tree_implicitly_moves_initial_xml() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetXmlReader("beta.xml"));
            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial));
            PropertyTree tree = reader.ReadPropertyTree();
            Assert.That(tree.Name, Is.EqualTo("beta"));
        }

        [Test]
        public void read_property_tree_from_root() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));
            Assert.That(reader.Read());
            PropertyTree tree = reader.ReadPropertyTree();
            AssertBetaFile(tree);
        }

        [Test]
        public void read_property_tree_from_root_xml() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetXmlReader("beta.xml"));
            Assert.That(reader.Read());
            PropertyTree tree = reader.ReadPropertyTree();
            AssertBetaFile(tree);
        }

        private void AssertBetaFile(PropertyTree tree) {
            Assert.That(tree.Name, Is.EqualTo("beta"));
            Assert.That(tree.FirstChild.Name, Is.EqualTo("c"));
            Assert.That(tree.Children[1].Name, Is.EqualTo("d"));
            Assert.That(tree.Children[2].Name, Is.EqualTo("a"));
            Assert.That(tree[2][1].Name, Is.EqualTo("aa"));
            Assert.That(tree[2][2].Name, Is.EqualTo("b"));
            Assert.That(tree[3][0].Name, Is.EqualTo("a"));
        }

        [Test]
        public void read_full_document() {
            PropertyTreeReader reader = PropertyTreeReader.CreateXml(GetContentPath("beta.xml"));

            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial));
            Assert.That(reader.Read());

            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.PropertyTree));
            Assert.That(reader.Name, Is.EqualTo("beta"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("c"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("d"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.PropertyTree));
            Assert.That(reader.Name, Is.EqualTo("a"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("a"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("aa"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("b"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("bb"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("e"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.EndPropertyTree));
            Assert.That(reader.Name, Is.EqualTo("a"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            // b --
            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.PropertyTree));
            Assert.That(reader.Name, Is.EqualTo("b"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.Property));
            Assert.That(reader.Name, Is.EqualTo("a"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.EndPropertyTree));
            Assert.That(reader.Name, Is.EqualTo("b"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read());
            Assert.That(reader.NodeType, Is.EqualTo(PropertyNodeType.EndPropertyTree));
            Assert.That(reader.Name, Is.EqualTo("beta"));
            Assert.That(reader.Namespace, Is.EqualTo(string.Empty));

            Assert.That(reader.Read(), Is.False);
            Assert.That(reader.ReadState, Is.EqualTo(ReadState.EndOfFile));

        }

    }
}

