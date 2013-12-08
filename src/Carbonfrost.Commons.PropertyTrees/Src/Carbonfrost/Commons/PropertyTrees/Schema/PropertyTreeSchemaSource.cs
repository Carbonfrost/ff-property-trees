//
// - PropertyTreeSchemaSource.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {
    
    [StreamingSourceUsage(Extensions = ".schema.xml")]
    public class PropertyTreeSchemaSource : TextSource {
        
        public override object Load(TextReader reader, Type instanceType) {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (instanceType == null)
                throw new ArgumentNullException("instanceType");

            throw new NotImplementedException();
        }
        
        public override void Save(TextWriter writer, object value) {
            if (writer == null)
                throw new ArgumentNullException("writer");
            
            throw new NotImplementedException();
        }
    }
}


