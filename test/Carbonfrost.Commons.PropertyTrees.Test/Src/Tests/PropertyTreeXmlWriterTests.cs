//
// - PropertyTreeXmlWriterTests.cs -
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
using System.IO;
using System.Xml;
using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;

namespace Tests {

    [TestFixture]
    public class PropertyTreeXmlWriterTests : TestBase {

        [Test]
        public void write_simple_tree() {
            string result = WritePropertyTreeXmlDocument(
                (PropertyTreeWriter writer) => {
                    writer.WriteStartDocument();

                    writer.WriteStartTree("mercury");
                    writer.WriteProperty("a", "420");
                    writer.WriteProperty("b", "true");
                    writer.WriteProperty("c", "false");
                    writer.WriteProperty("d", "D751E4E1-F6E7-4B2F-93F5-0F7A0E95F804");
                    writer.WriteEndTree();

                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                });

            Assert.That(result, Is.EqualTo(GetContent("Xml/mercury.xml")));
        }

        static string WritePropertyTreeXmlDocument(Action<PropertyTreeWriter> callback) {
            StringWriter sw = new StringWriter();
            XmlWriter outputWriter = XmlWriter.Create(sw);

            PropertyTreeXmlWriter writer = PropertyTreeXmlWriter.Create(outputWriter);
            callback(writer);
            return sw.ToString();
        }

        // TODO Expect an exception about no root node

    }
}
