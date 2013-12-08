//
// - PropertyType.cs -
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

namespace Carbonfrost.Commons.PropertyTrees {

    // UNDONE switch to schema types

    public enum PropertyType {
        Boolean,
        Byte,
        Char,
        DateTime,
        DateTimeOffset,
        Decimal,
        Double,
        Guid,
        Int16,
        Int32,
        Int64,
        SByte,
        Single,
        String,
        TimeSpan,
        PropertyTree,
        PropertyTreeReference,
        UInt16,
        UInt32,
        UInt64,
        Uri,

        ByteArray,
        Type,

        // TODO Maybe support Names and Paths
    }
}
