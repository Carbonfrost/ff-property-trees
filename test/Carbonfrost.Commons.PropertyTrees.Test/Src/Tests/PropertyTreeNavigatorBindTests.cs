//
// - PropertyTreeNavigatorBindTests.cs -
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
using Prototypes;

namespace Tests {

    [TestFixture]
    public class PropertyTreeNavigatorBindTests : TestBase {

        [Test]
        public void bind_empty_node() {
            PropertyTree pt = LoadTree("alpha-empty.xml");
            Alpha a = new Alpha();

            PropertyTreeBinder.GetPropertyTreeBinder(typeof(Alpha), null)
                .Bind(a, pt.CreateNavigator());
            Assume.That(a.A, Is.False);
            Assume.That(a.C, Is.Null);
        }

        [Test]
        public void bind_empty_node_in_parent_context() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Alpha a = new Alpha();

            PropertyTreeBinder.GetPropertyTreeBinder(typeof(Alpha), null)
                .Bind(a, pt.Children[0].CreateNavigator());

            Assume.That(a.A, Is.False);
            Assume.That(a.C, Is.Null);
            Assume.That(a.U, Is.Null);
        }

        [Test]
        public void bind_empty_self_closing_node_in_parent_context() {
            PropertyTree pt = LoadTree("alpha-list.xml");
            Alpha a = new Alpha();

            PropertyTreeBinder.GetPropertyTreeBinder(typeof(Alpha), null)
                .Bind(a, pt.Children[2].CreateNavigator());

            Assume.That(a.A, Is.False);
            Assume.That(a.C, Is.Null);
            Assume.That(a.U, Is.Null);
        }
    }
}
