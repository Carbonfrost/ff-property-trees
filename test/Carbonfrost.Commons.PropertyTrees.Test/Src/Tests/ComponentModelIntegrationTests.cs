//
// - ComponentModelIntegrationTests.cs -
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
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared.Runtime;
using NUnit.Framework;

namespace Tests {

    // TODO test sourcing, test pt:property,
    // test circular refs (#), test type converters,
    // test whitespace handling (never trim from attributes)

    [TestFixture]
    public class ComponentModelIntegrationTests {

        [Test]
        public void should_provide_content_type() {
            AppDomain.CurrentDomain.GetContentTypes().Contains(ContentTypes.PropertyTrees);
        }

        [Test]
        public void should_provide_streaming_source_using_file_extensions() {
            Assert.That(StreamingSource.Create(typeof(object), (ContentType) null, ".pt"), Is.InstanceOf<PropertyTreeSource>());
            Assert.That(StreamingSource.Create(typeof(object), (ContentType) null, ".ptx"), Is.InstanceOf<PropertyTreeSource>());
        }

        [Test]
        public void should_provide_streaming_source_using_content_type() {
            Assert.That(StreamingSource.Create(typeof(object), ContentType.Parse(ContentTypes.PropertyTrees)),
                        Is.InstanceOf<PropertyTreeSource>());
        }

        [Test]
        public void should_provide_xmlns() {
            Assert.That(typeof(PropertyTree).GetQualifiedName().NamespaceName,
                        Is.EqualTo(Xmlns.PropertyTrees2010));

            Assert.That(typeof(PropertyTreeDefinition).GetQualifiedName().NamespaceName,
                        Is.EqualTo(Xmlns.PropertyTreesSchema2010));
        }
    }
}
