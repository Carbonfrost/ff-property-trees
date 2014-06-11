//
// - PropertyTreeBinderErrors.cs -
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
using System.Collections.Generic;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class PropertyTreeBinderErrors : IPropertyTreeBinderErrors {

        public static readonly IPropertyTreeBinderErrors Default = new PropertyTreeBinderErrors();
        public static readonly IPropertyTreeBinderErrors Null = new NullPropertyTreeBinderErrors();

        public void BadAddChild(Type parentType, Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadAddChild(parentType, ex, loc);
        }

        public void CouldNotBindStreamingSource(Type componentType, FileLocation loc) {
            throw PropertyTreesFailure.CouldNotBindStreamingSource(componentType, loc);
        }

        public void DuplicatePropertyName(IEnumerable<QualifiedName> duplicates, FileLocation loc) {
            throw PropertyTreesFailure.DuplicatePropertyName(duplicates, loc);
        }

        public void RequiredPropertiesMissing(IEnumerable<string> requiredMissing, OperatorDefinition op, FileLocation loc) {
            throw PropertyTreesFailure.RequiredPropertiesMissing(requiredMissing, op.ToString(), loc.LineNumber, loc.LinePosition);
        }

        public void NoAddMethodSupported(Type type, FileLocation loc) {
            throw PropertyTreesFailure.NoAddMethodSupported(type, loc);
        }

        public void NoTargetProviderMatches(Type componentType, FileLocation loc) {
            throw PropertyTreesFailure.NoTargetProviderMatches(componentType, loc);
        }

        public void BadTargetTypeDirective(Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadTargetTypeDirective(ex, loc);
        }

        public void BadTargetProviderDirective(Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadTargetProviderDirective(ex, loc);
        }

        public void BadSourceDirective(Exception ex, FileLocation loc) {
            throw PropertyTreesFailure.BadSourceDirective(ex, loc);
        }
    }
}
