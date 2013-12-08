//
// - StreamContextExtensions.cs -
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
using System.Text;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

	public static class StreamContextExtensions {

	    public static T ReadPropertyTree<T>(this StreamContext streamContext, Encoding encoding = null) {
	        return ReadPropertyTree<T>(streamContext, typeof(T), encoding, null);
	    }

	    public static T ReadPropertyTree<T>(this StreamContext streamContext, Type type, Encoding encoding = null) {
	        return ReadPropertyTree<T>(streamContext, type, encoding, null);
	    }

	    public static T ReadPropertyTree<T>(this StreamContext streamContext, Type type, Encoding encoding = null, PropertyTreeReaderSettings settings = null) {
	        using (var reader = PropertyTreeReader.Create(streamContext, encoding, settings)) {
	            reader.Read();
	            return reader.Bind<T>();
	        }
	    }

	}
}
