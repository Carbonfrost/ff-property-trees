//
// - PropertyTreeSource{T}.cs -
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
using System.IO;

namespace Carbonfrost.Commons.PropertyTrees {

    public class PropertyTreeSource<T> : PropertyTreeSource {

        public override object Load(TextReader reader, Type instanceType) {
            if (instanceType == null) {
                instanceType = typeof(T);
            }
            
            return base.Load(reader, instanceType);
        }
        
        public override void Save(System.IO.TextWriter writer, object value) {
			if (value is T)
				base.Save(writer, value);
			else
			    throw new NotImplementedException();
        }
    }
}
