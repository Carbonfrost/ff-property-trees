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

    }
}
