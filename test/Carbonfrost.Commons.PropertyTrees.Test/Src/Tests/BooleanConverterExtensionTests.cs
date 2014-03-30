//
// - BooleanConverterExtensionTests.cs -
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
using System.Linq;
using Carbonfrost.Commons.PropertyTrees;
using NUnit.Framework;

namespace Tests {

    [TestFixture]
    public class BooleanConverterExtensionTests {

        [Test]
        public void test_convert_builtins() {
            Assert.That(BooleanConverterExtension.Instance.ConvertFromInvariantString("true"),
                        Is.EqualTo(true));
            Assert.That(BooleanConverterExtension.Instance.ConvertFromInvariantString("false"),
                        Is.EqualTo(false));
        }

        [Test]
        public void test_convert_aliases() {
            Assert.That(BooleanConverterExtension.Instance.ConvertFromInvariantString("yes"),
                        Is.EqualTo(true));
            Assert.That(BooleanConverterExtension.Instance.ConvertFromInvariantString("no"),
                        Is.EqualTo(false));

            Assert.That(BooleanConverterExtension.Instance.ConvertFromInvariantString("YES"),
                        Is.EqualTo(true));
            Assert.That(BooleanConverterExtension.Instance.ConvertFromInvariantString("NO"),
                        Is.EqualTo(false));
        }
    }


}


