//
// - PropertyTreeReaderTest.Bind.cs -
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.PropertyTrees.Schema;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.Shared.Runtime.Conditions;
using NUnit.Framework;
using Prototypes;
using Tests;
using NUnitIgnore = NUnit.Framework.IgnoreAttribute;

namespace Tests {

    [TestFixture]
    public class PropertyTreeReaderTest : TestBase {

        [Test]
        public void bind_complex_types() {
            PropertyTreeReader pt = LoadContent("beta.xml");
            Assume.That(pt.Read(), Is.True);

            Beta b = pt.Bind(new Beta());

            Assert.That(b.A.A, Is.True);
            Assert.That(b.A.AA.HasValue, Is.False);
            Assert.That(b.A.B, Is.EqualTo(0));
            Assert.That(b.A.BB.HasValue, Is.True);

            Assert.That(b.C, Is.EqualTo(new Uri("http://carbonfrost.com")));
            Assert.That(b.D, Is.EqualTo("Generic text"));
        }

        [Test]
        public void bind_complex_types_latebound() {
            // Demonstrates that the late bound version of Gamma
            // should be used (GammaExtension); As such, the shadowed
            // property should bind correctly
            PropertyTreeReader pt = LoadContent("beta-2.xml");
            Assume.That(pt.Read(), Is.True);

            Beta b = pt.Bind(new Beta());

            Assert.That(b.E is GammaExtension);

            GammaExtension ge = (GammaExtension) b.E;
            Assert.That(ge.B, Is.EqualTo(DateTime.Parse("6/16/2012 6:48 PM")));
        }

        [Test]
        public void bind_missing_properties_throws_exception() {
            PropertyTreeReader pt = LoadContent("beta-4.xml");
            Assume.That(pt.Read(), Is.True);

            Assert.That(() => pt.Bind<Beta>(), Throws.InstanceOf<PropertyTreeException>()
                        .And.Message.StringMatching("line 2, pos 5"));
        }

        [Test]
        public void bind_streaming_source() {
            PropertyTreeReader pt = LoadContent("beta-3.xml");
            Assume.That(pt.Read(), Is.True);

            Beta b = pt.Bind(new Beta());

            Assert.That(b.Source, Is.EqualTo("not a streaming source"));

            Assert.That(b.F, Is.Not.Null);
            Assert.That(b.F.GetProperty("time"), Is.EqualTo("after"));
            Assert.That(b.F.GetProperty("fall"), Is.EqualTo("i"));
            Assert.That(b.F.GetProperty("source"), Is.EqualTo("not a streaming source"));
        }

        [Test]
        public void bind_latebound_provider() {
            Assume.That(StreamingSource.FromName("xmlFormatter"), Is.Not.Null);

            PropertyTreeReader pt = LoadContent("iota-chi.xml");
            Assume.That(pt.Read(), Is.True);

            IotaChi b = pt.Bind(new IotaChi());

            Assert.That(AppDomain.CurrentDomain.GetProviderName(typeof(StreamingSource), b.A).LocalName,
                        Is.EqualTo("xmlFormatter"));
        }

        [Test]
        public void bind_latebound_provider_criteria() {
            PropertyTreeReader pt = LoadContent("iota-chi-2.xml");
            Assume.That(pt.Read(), Is.True);

            IotaChi b = pt.Bind(new IotaChi());

            Assert.That(AppDomain.CurrentDomain.GetProviderName(typeof(StreamingSource), b.A).LocalName,
                        Is.EqualTo("properties"));
        }

        [Test]
        public void bind_concrete_class_indirections() {
            PropertyTreeReader pt = LoadContent("mu.xml");
            Assume.That(pt.Read(), Is.True);

            var b = (MuAlpha) pt.Bind<Mu>();
            Assert.That(b.B,
                        Is.EqualTo(420));
        }

        [Test]
        public void bind_composable_providers() {
            PropertyTreeReader pt = LoadContent("pi-chi.xml");
            Assume.That(pt.Read(), Is.True);

            var b = pt.Bind<PiChi>().Preconditions;
            Assert.That(b.ToString(),
                        Is.EqualTo("and(environment(\"PROCESSOR_LEVEL\", \"l\"), environment(\"QR\", \"m\"), platform())"));
        }

        [Test]
        public void bind_composable_providers_add_child() {
            PropertyTreeReader pt = LoadContent("pi-chi.xml");
            Assume.That(pt.Read(), Is.True);

            var b = pt.Bind<PiChi>().Controls;
            Assert.True(b.Controls[0] is Paragraph);
            Assert.True(b.Controls[0].Controls[0] is TextBox);
        }

        [Test]
        public void bind_nondefault_constructors() {
            // Demonstrates that an object with a nondefault constructor
            // can be populated with arbitrary simple values

            PropertyTreeReader pt = LoadContent("eta.xml");
            Assume.That(pt.Read(), Is.True);

            Eta e = pt.Bind<Eta>();

            Assert.That(e.A, Is.EqualTo(256));
            Assert.That(e.B, Is.EqualTo(TimeSpan.Parse("2.5:05:05.200")));
            Assert.That(e.C, Is.EqualTo(2256.231250002));
            Assert.That(e.D, Is.EqualTo(293680235));
        }

        [Test]
        public void bind_nondefault_constructors_complex() {
            // Demonstrates that an object with a nondefault constructor
            // can be populated with a complex object like Eta

            PropertyTreeReader pt = LoadContent("iota.xml");
            Assume.That(pt.Read(), Is.True);

            Iota i = pt.Bind<Iota>();

            Assert.That(i.A, Is.EqualTo(8256));
            Assert.That(i.B, Is.EqualTo(TimeSpan.Parse("82.5:05:05.200")));
            Assert.That(i.C, Is.EqualTo(82256.231250002));
            Assert.That(i.D, Is.EqualTo(8293680235));
            Assert.That(i.F, Is.EqualTo("Typedescriptor"));
            Assert.That(i.G, Is.EqualTo(new Uri("http://carbonfrost.com")));

            Eta e = i.E;

            Assert.That(e.A, Is.EqualTo(256));
            Assert.That(e.B, Is.EqualTo(TimeSpan.Parse("2.5:05:05.200")));
            Assert.That(e.C, Is.EqualTo(2256.231250002));
            Assert.That(e.D, Is.EqualTo(293680235));
        }

        [Test]
        public void bind_abstract_builder_types() {
            // Demonstrates that the builder indirection can be used on abstract types

            PropertyTreeReader pt = LoadContent("epsilon-chi-builder.xml");
            Assume.That(pt.Read(), Is.True);

            EpsilonChi e = pt.Bind<EpsilonChi>();
            Assert.That(e, Is.InstanceOf<EpsilonChiAlpha>());
        }

        [Test]
        public void bind_builder_types() {
            // Demonstrates that the builder indirection can be used

            PropertyTreeReader pt = LoadContent("epsilon-builder.xml");
            Assume.That(pt.Read(), Is.True);

            Assert.That(TypeDescriptor.GetProperties(typeof(Epsilon)).Find("e", true), Is.Not.Null);
            Epsilon e = pt.Bind<Epsilon>();

            Assert.That(e is EpsilonAlpha);
            Assert.That(e.A, Is.EqualTo(256));
            Assert.That(e.B, Is.EqualTo(TimeSpan.Parse("2.5:05:05.200")));
            Assert.That(e.C, Is.EqualTo(2256.231250002));
            Assert.That(e.D, Is.EqualTo(293680235));

            Assert.That(e.E, Is.InstanceOf(typeof(EpsilonExtended)));

            EpsilonExtended f = (EpsilonExtended) e.E;
            Assert.That(f.A, Is.EqualTo(1256));
            Assert.That(f.B, Is.EqualTo(TimeSpan.Parse("12.5:05:05.200")));
            Assert.That(f.C, Is.EqualTo(12256.231250002));
            Assert.That(f.D, Is.EqualTo(1293680235));
            Assert.That(f.F, Is.EqualTo(DateTime.Parse("2011-05-12 8:45AM")));
        }

        [Test]
        public void bind_empty_node() {
            // Trivial binding of primitive types

            PropertyTree pt = LoadTree("alpha-empty.xml");
            Alpha a = pt.Bind<Alpha>();
            Assume.That(a.A, Is.False);
        }

        [Test]
        public void bind_primitive_types() {
            // Trivial binding of primitive types

            PropertyTreeReader pt = LoadContent("alpha.xml");
            Assume.That(pt.Read(), Is.True);

            Alpha a = pt.Bind(new Alpha());

            Assert.That(a.A, Is.True);
            Assert.That(a.AA.HasValue, Is.False);
            Assert.That(a.B, Is.EqualTo(0));
            Assert.That(a.BB.HasValue, Is.True);
            Assert.That(a.BB, Is.EqualTo(0));

            Assert.That(a.D, Is.EqualTo('g'));
            Assert.That(a.E, Is.EqualTo(DateTime.Parse("3/30/2011 1:50 AM")));

            Assert.That(a.F, Is.EqualTo(10.5000m));
            Assert.That(a.G, Is.EqualTo(10.5));
            Assert.That(a.H, Is.EqualTo(256));
            Assert.That(a.I, Is.EqualTo(1024));
            Assert.That(a.J, Is.EqualTo(102410241024));
            Assert.That(a.K, Is.EqualTo(-120));
            Assert.That(a.L, Is.EqualTo(float.NaN));
            Assert.That(a.M, Is.EqualTo("Carbonfrost F5 Project"));
            Assert.That(a.N, Is.EqualTo(65535));
            Assert.That(a.O, Is.EqualTo(6553620));
            Assert.That(a.P, Is.EqualTo(6553620655362));
            Assert.That(a.Q, Is.EqualTo(new Uri("http://carbonfrost.com")));
            Assert.That(a.R, Is.EqualTo(TimeSpan.Parse("4.12:0:30.5")));
            Assert.That(a.S, Is.EqualTo(new Guid("{BF972F0A-CB10-441B-9D25-3D6DEB9065D1}")));
            Assert.That(a.T, Is.EqualTo(DateTimeOffset.Parse("4/1/2011 12:11:01 AM -04:00")));
            Assert.That(a.U, Is.EqualTo(typeof(Glob)));
        }

        [Test]
        public void bind_ordered_list() {
            // Binding of an ordered list IList<T> implementation

            PropertyTreeReader pt = LoadContent("delta.xml");
            Assume.That(pt.Read(), Is.True);

            Delta originalD = new Delta();
            Delta d = pt.Bind(originalD);
            Assert.That(object.ReferenceEquals(d, originalD));
            Assert.That(d.A.Count, Is.EqualTo(3));

            Assert.That(d.A[0].A, Is.True);
            Assert.That(d.A[0].B, Is.EqualTo(0));
            Assert.That(d.A[0].D, Is.EqualTo('g'));
            Assert.That(d.A[0].E, Is.EqualTo(DateTime.Parse("3/30/2011 1:50 AM")));

            Assert.That(d.A[1].F, Is.EqualTo(10.5000m));
            Assert.That(d.A[1].G, Is.EqualTo(10.5));
            Assert.That(d.A[1].H, Is.EqualTo(256));
            Assert.That(d.A[1].I, Is.EqualTo(1024));

            Assert.That(d.A[2].M, Is.EqualTo("Carbonfrost F5 Project"));
            Assert.That(d.A[2].Q, Is.EqualTo(new Uri("http://carbonfrost.com")));
            Assert.That(d.A[2].R, Is.EqualTo(TimeSpan.Parse("4.12:0:30.5")));
            Assert.That(d.A[2].S, Is.EqualTo(new Guid("{ED826F6C-47B5-4C40-B5B1-E847CB193E03}")));

            Assert.That(d.B[0].A.F, Is.EqualTo(10.5000m));
            Assert.That(d.B[0].A.G, Is.EqualTo(10.5));
        }

        [Test]
        public void bind_with_type_conversion() {
            PropertyTreeReader pt = LoadContent("gamma.xml");
            Assume.That(pt.Read(), Is.True);

            Gamma g = new Gamma();
            pt.Bind(g);

            Assert.That(g.B, Is.EqualTo(145 * 1000 * 1000));
            Assert.That(g.C.ToString(), Is.EqualTo(@"http(s)?://carbonfrost\.(com|net|org)"));
        }

        [Test]
        public void bind_add_method_factories() {
            // Binding of add methods as factories

            PropertyTreeReader pt = LoadContent("omicron.xml");
            Assume.That(pt.Read(), Is.True);

            // Add methods defined on the type and defined via an extension method
            Omicron o = pt.Bind<Omicron>();
            Assert.That(o.A.A, Is.True);
            Assert.That(o.A.AA.HasValue, Is.False);
            Assert.That(o.A.B, Is.EqualTo(0));
            Assert.That(o.A.BB.HasValue, Is.True);
            Assert.That(o.A.D, Is.EqualTo('g'));
            Assert.That(o.A.E, Is.EqualTo(DateTime.Parse("3/30/2011 1:50 AM")));

            Assert.That(o.B.C, Is.EqualTo(new Uri("http://carbonfrost.com")));
            Assert.That(o.B.D, Is.EqualTo("Generic text"));
            Assert.That(o.B.A.A, Is.EqualTo(true));

            Assert.That(o.G.A, Is.EqualTo(CultureInfo.GetCultureInfo("en-US")));
        }

        [Test]
        public void bind_add_method_factories_latebound() {

            // The latebound type should be used from add method factories
            PropertyTreeReader pt = LoadContent("omicron-4.xml");
            Assume.That(pt.Read(), Is.True);

            Omicron o = pt.Bind<Omicron>();
            Assert.That(o.G is GammaExtension);
            GammaExtension ge = (GammaExtension) o.G;
            Assert.That(ge.B, Is.EqualTo(DateTime.Parse("6/16/2012 6:48 PM")));
        }

        [Test]
        public void bind_add_method_factory_extension_no_parameters() {
            PropertyTreeReader pt = LoadContent("omicron-2.xml");
            Assume.That(pt.Read(), Is.True);

            Omicron o = pt.Bind<Omicron>();
            Assert.That(o.G.A, Is.EqualTo(CultureInfo.GetCultureInfo("fr-FR")));
        }

        [Test]
        public void bind_add_method_factory_extension_generic() {
            // Demonstrates that a generic interface implementation can
            // supply add methods
            PropertyTreeReader pt = LoadContent("phi.xml");
            Assume.That(pt.Read(), Is.True);

            Phi p = pt.Bind<Phi>();
            Assert.That(p.G, Is.Not.Null);
        }

        [Test]
        public void bind_optional_parameters() {
            PropertyTreeReader pt = LoadContent("upsilon.xml");
            Assume.That(pt.Read(), Is.True);

            Upsilon p = pt.Bind<Upsilon>();
            Assert.That(p.A, Is.EqualTo(45));
            Assert.That(p.B, Is.EqualTo("yipp"));
        }

        // TODO Optionals on ext method

        [Test]
        public void bind_clear_method() {
            PropertyTreeReader pt = LoadContent("psi.xml");
            Assume.That(pt.Read(), Is.True);

            Psi p = new Psi();
            Assume.That(p.A.Count, Is.EqualTo(3));

            p = pt.Bind<Psi>(p);
            Assert.That(p.A.Count, Is.EqualTo(0));
        }

        // TODO Test add range where best item type is needed

        [Test]
        public void bind_add_range_property() {
            PropertyTreeReader pt = LoadContent("psi-add-range.xml");
            Assume.That(pt.Read(), Is.True);

            Psi p = pt.Bind<Psi>(new Psi());
            Assert.That(p.B.Count, Is.EqualTo(4));
            Assert.That(p.B[0], Is.EqualTo("a"));
            Assert.That(p.B[2], Is.EqualTo("c"));
            Assert.That(p.B[3], Is.EqualTo("d"));
        }

        [Test]
        public void bind_remove_method() {
            PropertyTreeReader pt = LoadContent("psi-remove.xml");
            Assume.That(pt.Read(), Is.True);

            Psi p = new Psi();
            Assume.That(p.A.Count, Is.EqualTo(3));

            p = pt.Bind<Psi>(p);
            Assert.That(p.A.Count, Is.EqualTo(2));
            Assert.That(p.A[0].B, Is.EqualTo(47));
            Assert.That(p.A[1].B, Is.EqualTo(47));
        }

        [Test]
        public void bind_dictionary_access() {
            PropertyTreeReader pt = LoadContent("omicron-5.xml");
            Assume.That(pt.Read(), Is.True);

            OmicronAlpha p = pt.Bind<OmicronAlpha>();
            Assert.That(p.A_.B, Is.EqualTo(0));
            Assert.That(p.A_.D, Is.EqualTo('g'));

            // These should not bind as dictionary accesses
            Assert.That(p.B, Is.True);
            Assert.That(p.C_, Is.EqualTo(1));
        }

        [Test]
        public void bind_dictionary_access_kvp_syntax() {
            PropertyTreeReader pt = LoadContent("beta-5.xml");
            Assume.That(pt.Read(), Is.True);

            Beta p = pt.Bind<Beta>();
            Assert.That(p.G["gu"].E, Is.EqualTo(new DateTime(2011, 3, 30, 1, 50, 00)));
            Assert.That(p.G["gu"].F, Is.EqualTo(10.5000));
            Assert.That(p.G["gu"].G, Is.EqualTo(10.5));
        }

        [Test]
        public void bind_should_apply_explicit_factory_names() {
            PropertyTreeReader pt = LoadContent("beta-list.xml");
            Assume.That(pt.Read(), Is.True);

            var p = pt.Bind<BetaList>();

            Assert.That(p[0].D, Is.EqualTo("Generic text"));
            Assert.That(p[0].C, Is.EqualTo(new Uri("http://carbonfrost.com")));
            Assert.That(p[0].A.A, Is.EqualTo(true));
            Assert.That(p[0].A.E, Is.EqualTo(new DateTime(2011, 3, 30, 1, 50, 00)));

            // TODO Behavior of <add> is technically undefined because it could be Add(Beta) or Add(Object)
            Assert.That(p[1].D, Is.EqualTo("Built-in add method"));

        }

        [Test]
        public void bind_should_invoke_ancestor_attached_property_context() {
            PropertyTreeReader pt = LoadContent("control-extension-property.xml");
            Assume.That(pt.Read(), Is.True);

            var p = pt.Bind<Canvas>();

            Assert.That(p.Controls[0]._Top, Is.EqualTo(40));
            Assert.That(p.Controls[1]._Top, Is.EqualTo(80));
        }

        [Test]
        public void bind_should_invoke_generic_ancestor_attached_property_context() {
            PropertyTreeReader pt = LoadContent("control-extension-property-3.xml");
            Assume.That(pt.Read(), Is.True);

            var c = pt.Bind<Canvas>();

            Assert.That(c.Controls.Count, Is.EqualTo(2));
            Assert.That(c.Controls[0].Controls.Count, Is.EqualTo(1));
            Assert.That(c.Controls[0].Controls[0]._Left, Is.EqualTo(132));
            Assert.That(c.Controls[1]._Left, Is.EqualTo(62));
        }

        [Test]
        public void bind_should_invoke_extension_method() {
            PropertyTreeReader pt = LoadContent("control-extension-property-2.xml");
            Assume.That(pt.Read(), Is.True);

            var p = pt.Bind<Canvas>();

            Assert.That(p.Controls[0]._Left, Is.EqualTo(132));
            Assert.That(p.Controls[1]._Left, Is.EqualTo(66));
        }

        [Test]
        public void bind_should_transparently_handle_implied_operator_ns() {
            PropertyTreeReader pt = LoadContent("bravo-1.xml");
            Assume.That(pt.Read(), Is.True);

            var p = pt.Bind<Bravo>();

            Assert.That(p.Components[0].Type, Is.EqualTo("assembly"));
            Assert.That(p.Components[0].Name.Name, Is.EqualTo("Carbonfrost.Commons.SharedRuntime"));
        }

        [Test]
        public void bind_should_transparently_handle_implied_parameter_ns() {
            PropertyTreeReader pt = LoadContent("bravo-2.xml");
            Assume.That(pt.Read(), Is.True);

            var p = pt.Bind<Bravo>();

            Assert.That(p.Components[0].Type, Is.EqualTo("assembly"));
            Assert.That(p.Components[0].Name.Name, Is.EqualTo("Carbonfrost.Commons.SharedRuntime"));
        }

        [Test]
        public void bind_should_bind_nullable_reference_types() {
            // Reference types in ctor don't have to be specified

            PropertyTreeReader pt = LoadContent("iota-2.xml");
            Assume.That(pt.Read(), Is.True);

            Iota i = pt.Bind<Iota>();

            Assert.That(i.A, Is.EqualTo(8256));
            Assert.That(i.B, Is.EqualTo(TimeSpan.Parse("82.5:05:05.200")));
            Assert.That(i.C, Is.EqualTo(82256.231250002));
            Assert.That(i.D, Is.EqualTo(8293680235));
        }

        [Test]
        public void bind_should_bind_nested_class() {
            PropertyTreeReader pt = LoadContent("iota-2.xml");
            Assume.That(pt.Read(), Is.True);

            Iota i = pt.Bind<Iota>();

            Assert.That(i.H.A, Is.EqualTo("ng"));
        }

        [Test]
        public void bind_should_be_invalid_on_value_type() {
            // Values types in ctor must be specified

            PropertyTreeReader pt = LoadContent("iota-invalid-1.xml");
            Assume.That(pt.Read(), Is.True);

            Assert.That(() => pt.Bind<Iota>(), Throws.InstanceOf<PropertyTreeException>()
                        .And.Message.StringMatching("line 3, pos 2"));
        }
    }

}
