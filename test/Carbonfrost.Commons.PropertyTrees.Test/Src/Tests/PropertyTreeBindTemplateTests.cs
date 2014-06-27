//
// - PropertyTreeBindTemplateTests.cs -
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
using System.Globalization;
using Carbonfrost.Commons.PropertyTrees;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using NUnit.Framework;
using Prototypes;
using Tests;

namespace Tests {


    [TestFixture]
    public class PropertyTreeBindTemplateTests : TestBase {

        [Test]
        public void bind_template_primitive_types() {
            PropertyTreeReader pt = LoadContent("alpha.xml");
            Assume.That(pt.Read(), Is.True);
            var template = pt.Bind<ITemplate<Alpha>>();
            var a = new Alpha();
            template.Initialize(a);

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
        public void bind_template_complex_types_readonly_accessor() {
            PropertyTreeReader pt = LoadContent("beta-6.xml");
            Assume.That(pt.Read(), Is.True);
            var template = pt.Bind<ITemplate<Beta>>();
            var b = new Beta();
            template.Initialize(b);

            Assert.That(b.H.A, Is.True);
            Assert.That(b.A.AA.HasValue, Is.False);
            Assert.That(b.H.B, Is.EqualTo(10));
            Assert.That(b.H.BB.HasValue, Is.True);
            Assert.That(b.H.D, Is.EqualTo('g'));
            Assert.That(b.H.E, Is.EqualTo(DateTime.Parse("3/30/2011 1:50 AM")));
        }

        [Test]
        public void bind_template_add_method_factory_extension_no_parameters() {
            PropertyTreeReader pt = LoadContent("omicron-2.xml");
            Assume.That(pt.Read(), Is.True);

            var tmpl = pt.Bind<ITemplate<Omicron>>();
            Omicron o = new Omicron();
            tmpl.Initialize(o);

            Assert.That(o.G.A, Is.EqualTo(CultureInfo.GetCultureInfo("fr-FR")));
        }

        [Test]
        public void bind_template_untyped_transition() {
            // The property return type ITemplate gets constructed as a
            // typed version
            PropertyTreeReader pt = LoadContent("iota-chi-3.xml");
            Assume.That(pt.Read(), Is.True);

            var template = pt.Bind<IotaChi>().Template;
            var a = new Alpha();
            template.Initialize(a);

            Assert.That(template, Is.InstanceOf<ITemplate<Alpha>>());
            Assert.That(a.I, Is.EqualTo(1024));
            Assert.That(a.J, Is.EqualTo(102410241024));
            Assert.That(a.K, Is.EqualTo(-120));
            Assert.That(a.L, Is.EqualTo(float.NaN));
        }

        [Test]
        public void bind_template_add_method_factories() {
            PropertyTreeReader pt = LoadContent("omicron.xml");
            Assume.That(pt.Read(), Is.True);

            var template = pt.Bind<ITemplate<Omicron>>();
            Omicron o = new Omicron();
            template.Initialize(o);

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
        public void bind_template_add_range_property() {
            PropertyTreeReader pt = LoadContent("psi-add-range.xml");
            Assume.That(pt.Read(), Is.True);

            Psi p = new Psi();
			var template = pt.Bind<ITemplate<Psi>>();
			template.Initialize(p);

            Assert.That(p.B.Count, Is.EqualTo(4));
            Assert.That(p.B[0], Is.EqualTo("a"));
            Assert.That(p.B[2], Is.EqualTo("c"));
            Assert.That(p.B[3], Is.EqualTo("d"));
        }
    }
}
