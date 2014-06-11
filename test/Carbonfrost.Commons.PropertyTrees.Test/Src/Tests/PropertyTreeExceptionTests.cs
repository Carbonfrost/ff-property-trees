//
// - PropertyTreeExceptionTests.cs -
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
    public class PropertyTreeExceptionTests {

        [Test]
        public void Message_should_consolidate_competing_inner_line_infos() {
            // No need to display two line infos in ToString() or Message
            var inner = new PropertyTreeException("error", 10, 20);
            var e = new PropertyTreeException("outer", inner, 2, 4);

            Assert.That(e.ToString(),
                        Is.StringMatching("line 10, pos 20"));
            Assert.That(e.Message,
                        Is.Not.StringMatching("line 2, pos4"));
        }

    }
}

