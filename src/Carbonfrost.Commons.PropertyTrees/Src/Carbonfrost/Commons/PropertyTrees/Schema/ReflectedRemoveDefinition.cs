//
// - ReflectedRemoveDefinition.cs -
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
using System.Reflection;

using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedRemoveDefinition : RemoveOperatorDefinition {

        private readonly CommonReflectionInfo info;

        public ReflectedRemoveDefinition(RemoveAttribute attr, MethodBase method) {
            this.info = new CommonReflectionInfo(this, attr ?? RemoveAttribute.Default, method);
        }

        public override PropertyDefinitionCollection Parameters {
            get { return info.Parameters; } }

        public override MethodBase UnderlyingMethod {
            get { return info.Method; } }

        public override string Namespace {
            get { return info.Namespace; }
        }

        public override string Name {
            get { return info.Name; }
        }

        public override object Apply(object component, object parent, IReadOnlyDictionary<string, object> parameters) {
            var parms = MapParameters(UnderlyingMethod, parent, parameters);
            return info.Method.Invoke(component, parms);
        }

        public override PropertyDefinition DefaultParameter {
            get { return null; } }
    }
}

