//
// - PropertyTreeFactoryDefinitionTest.cs -
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
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared.Runtime.Components;
using NUnit.Framework;
using Prototypes;

namespace Tests.Schema {

    [TestFixture]
    public class PropertyTreeFactoryDefinitionTest : TestBase {

        class S {

            [Add(Name = "something-else")]
            public S AddSomethingElse() {
                return null;
            }

            [Add]
            public S AddSomething() {
                return null;
            }
        }

        [Test]
        public void add_method_explicit_name() {
            PropertyTreeDefinition def = PropertyTreeDefinition.FromType(typeof(S));
            Assert.That(def.Operators.Contains("something-else"));

        }

        [Test]
        public void add_method_default_implicit() {
            PropertyTreeDefinition def = PropertyTreeDefinition.FromType(typeof(S));
            Assert.That(def.Operators.Count, Is.EqualTo(2));
            Assert.That(def.Operators.Contains("something"));
        }


        [Test]
        public void addon_method_nominal() {
            var def = PropertyTreeDefinition.FromType(typeof(Omicron));
            var alphaAddon = def.GetOperator("alpha");

            Assert.That(alphaAddon, Is.Not.Null);
            Assert.That(alphaAddon.Parameters.Count, Is.EqualTo(0));
        }

        [Test]
        public void addon_method_extension() {
            var def = PropertyTreeDefinition.FromType(typeof(Omicron));
            var gammaAddon = def.GetOperator("gamma-optional");
            Assert.That(gammaAddon, Is.Not.Null);
            Assert.That(gammaAddon.Parameters.Count, Is.EqualTo(2));
        }

        [Test]
        public void detect_addon_method_in_external() {
            // Clients can define their own AddAttribute when they don't/can't reference the PropertyTree assembly
            var def = PropertyTreeDefinition.FromType(typeof(ComponentCollection));
            Assert.That(def.GetOperator("assembly"), Is.Not.Null);
            Assert.That(def.GetOperator("component"), Is.Not.Null);
            Assert.That(def.GetOperator("add"), Is.Not.Null);
        }
    }

}

