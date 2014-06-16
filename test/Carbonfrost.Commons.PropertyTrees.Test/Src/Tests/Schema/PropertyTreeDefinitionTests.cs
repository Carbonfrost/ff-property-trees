//
// - PropertyTreeDefinitionTests.cs -
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
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using NUnit.Framework;
using Prototypes;

namespace Tests.Schema {

    [TestFixture]
    public class PropertyTreeDefinitionTests : TestBase {

        [Test]
        public void get_property_nominal() {
            var a = PropertyTreeDefinition.FromType(typeof(Alpha));
            Assert.That(a.Properties.Select(t => t.Name), Contains.Item("A"));
            Assert.That(a.GetProperty("A"), Is.Not.Null);
        }

        [Test]
        public void get_property_case_insensitive() {
            var a = PropertyTreeDefinition.FromType(typeof(Alpha));
            Assert.That(a.GetProperty("a"), Is.Not.Null);
        }

        [Test]
        public void get_generic_list_operators() {
            var def = PropertyTreeDefinition.FromType(typeof(Delta));
            Assert.That(def.Operators.Count, Is.EqualTo(0));

            def = PropertyTreeDefinition.FromType(typeof(IList<Alpha>));
            Assert.That(def.GetOperator("add"), Is.Not.Null);
            Assert.That(def.GetOperator("remove"), Is.Not.Null);
            Assert.That(def.GetOperator("clear"), Is.Not.Null);
        }

        [Test]
        public void generic_list_operators_should_override() {
            var def = PropertyTreeDefinition.FromType(typeof(GenericList));

            Assert.That(def.GetOperator("add"), Is.Null);
            Assert.That(def.GetOperator("s"), Is.Not.Null);
        }

        [Test]
        public void add_child_operators_nominal() {
            var def = PropertyTreeDefinition.FromType(typeof(IAddChild<Control>));
            var fac = def.GetOperator("p");
            var fac2 = def.Operators.GetByLocalName("p");

            Assert.That(fac, Is.Not.Null);
            Assert.That(fac2, Is.Not.Null);
        }

        [Test]
        public void properties_should_include_text_type_indexer() {
            var def = PropertyTreeDefinition.FromType(typeof(Dictionary<QualifiedName, int>));
            Assert.That(def.Properties.FirstOrDefault(t => t.IsIndexer), Is.Not.Null);

            def = PropertyTreeDefinition.FromType(typeof(Dictionary<string, int>));
            Assert.That(def.Properties.FirstOrDefault(t => t.IsIndexer), Is.Not.Null);
        }

        [Test]
        public void properties_should_not_include_non_text_types() {
            var def = PropertyTreeDefinition.FromType(typeof(List<string>));
            Assert.That(def.Properties.FirstOrDefault(t => t.IsIndexer), Is.Null);
        }

        [Test]
        public void add_child_operators_inherited() {
            var def = PropertyTreeDefinition.FromType(typeof(ContainerControl));
            var fac = def.GetOperator("p");

            Assert.That(fac, Is.Not.Null);
        }

        [Test]
        public void get_extender_property_nominal() {
            var def = PropertyTreeDefinition.FromType(typeof(Canvas));
            var fac = def.GetProperty("Canvas.top");

            Assert.That(fac, Is.Not.Null);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Control)));
        }

        [Test]
        public void get_extender_property_extension_method() {
            var def = PropertyTreeDefinition.FromType(typeof(Control));
            var fac = def.GetProperty("ControlExtensions.left");

            Assert.That(fac, Is.Not.Null);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Paragraph)));

            // Look for type inheritance
            def = PropertyTreeDefinition.FromType(typeof(Paragraph));
            var other = def.GetProperty("ControlExtensions.left");
            Assert.That(other, Is.SameAs(fac));
        }

        [Test]
        public void get_extender_property_extension_method_inherit() {
            var def = PropertyTreeDefinition.FromType(typeof(Paragraph));
            var fac = def.GetProperty("left", GetPropertyOptions.IncludeExtenders);

            Assert.That(fac, Is.Not.Null);
            Assert.True(fac.IsExtender);
            Assert.True(fac.CanExtend(typeof(Paragraph)));
        }

        // TODO Test generics, including open generics

        class GenericList {

            [Add(Name = "s")]
            public void Add(object value) {}
        }

    }
}

