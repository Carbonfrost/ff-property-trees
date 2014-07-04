//
// - IPropertyTreeBinderErrors.cs -
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

    interface IPropertyTreeBinderErrors {

        void BadAddChild(Type parentType, Exception ex, FileLocation loc);
        void FailedToLoadFromSource(Uri uri, Exception ex, FileLocation fileLocation);
        void CouldNotBindStreamingSource(Type componentType, FileLocation loc);
        void CouldNotBindGenericParameters(Type componentType, Exception ex, FileLocation loc);

        void DuplicatePropertyName(IEnumerable<QualifiedName> duplicates,
                                   FileLocation loc);
        void RequiredPropertiesMissing(IEnumerable<string> requiredMissing,
                                       OperatorDefinition op,
                                       FileLocation loc);
        void BadTargetTypeDirective(Exception ex, FileLocation loc);
        void BadTargetProviderDirective(Exception ex, FileLocation loc);
        void BadSourceDirective(Exception ex, FileLocation loc);
        void NoAddMethodSupported(Type type, FileLocation loc);
        void NoTargetProviderMatches(Type componentType, FileLocation loc);
    }

}
