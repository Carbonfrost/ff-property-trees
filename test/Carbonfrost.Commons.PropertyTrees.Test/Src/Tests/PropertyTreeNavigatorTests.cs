//
// - PropertyTreeNavigatorTests.cs -
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

namespace Tests {

    [TestFixture]
    public class PropertyTreeNavigatorTests : TestBase {

        [Test]
        public void move_to_child_empty() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            var nav = pt.Children[1].CreateNavigator();

            Assert.That(nav.Name, Is.EqualTo("alpha2"));
            Assert.True(nav.MoveToFirstChild());

            Assert.That(nav.Name, Is.EqualTo("a"));
            Assert.False(nav.MoveToChild(0));
        }
    }

}
