//
// -PropertyTreeBindErrorTests.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using Tests;

namespace Tests {

    [TestFixture]
    public class PropertyTreeBindErrorTests : TestBase {

        [Test]
        public void bind_type_conversion_error() {
            PropertyTreeReader pt = LoadContent("alpha-invalid-1.xml");
            Assume.That(pt.Read(), Is.True);

            var ex = ExpectPropertyTreeException(() => pt.Bind<Alpha>());

            Assert.That(ex.FileLocation.LineNumber, Is.EqualTo(3));
            Assert.That(ex.FileLocation.LinePosition, Is.EqualTo(3));
            Assert.That(ex.Message, Is.StringMatching(@"Cannot parse .+ property `A' \(Prototypes.Alpha\)."));
            Assert.That(ex.InnerException.InnerException.Message, Is.StringMatching("not a valid value for"));
        }

        [Test]
        public void bind_type_reference_does_not_resolve() {
            PropertyTreeReader pt = LoadContent("alpha-invalid-2.xml");
            Assume.That(pt.Read(), Is.True);

            var ex = ExpectPropertyTreeException(() => pt.Bind<Alpha>());

            Assert.That(ex.FileLocation.LineNumber, Is.EqualTo(4));
            Assert.That(ex.FileLocation.LinePosition, Is.EqualTo(3));
            Assert.That(ex.Message, Is.StringMatching(@"Cannot parse .+ property `U' \(Prototypes.Alpha\)."));
            Assert.That(ex.InnerException.InnerException.Message, Is.StringMatching("type was not found"));
        }

        [Test]
        public void bind_missing_required_parameter() {
            PropertyTreeReader pt = LoadContent("eta-invalid-1.xml");
            Assume.That(pt.Read(), Is.True);

            var ex = ExpectPropertyTreeException(() => pt.Bind<Eta>());

            Assert.That(ex.FileLocation.LineNumber, Is.EqualTo(3));
            Assert.That(ex.FileLocation.LinePosition, Is.EqualTo(2));
            Assert.That(ex.Message, Is.StringMatching(@"required properties .+c \(Prototypes.Eta\)"));
        }

        [Test]
        public void bind_missing_required_parameter_different_namespace() {
            PropertyTreeReader pt = LoadContent("eta-invalid-2.xml");
            Assume.That(pt.Read(), Is.True);

            var ex = ExpectPropertyTreeException(() => pt.Bind<Eta>());

            Assert.That(ex.FileLocation.LineNumber, Is.EqualTo(3));
            Assert.That(ex.FileLocation.LinePosition, Is.EqualTo(2));
            Assert.That(ex.Message, Is.StringMatching(@"required properties .+d \(Prototypes.Eta\)"));
        }


        private PropertyTreeException ExpectPropertyTreeException(Action action) {
            string text = "<none>";
            Exception error = null;
            try {
                action();
            } catch (PropertyTreeException ex) {
                return ex;
            } catch (Exception ex) {
                text = ex.GetType().FullName;
                error = ex;
            }

            Assert.Fail("Expected PropertyTreeException, but {0} thrown. \n{1}", text, error);
            return null;
        }
    }
}


