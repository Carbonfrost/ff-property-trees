//
// - PropertyTreeBinderFactoryAttribute.cs -
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
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public sealed class PropertyTreeBinderFactoryAttribute : AdapterFactoryAttribute {

	    public Type PropertyTreeBinderFactoryType { get; set; }

	    public PropertyTreeBinderFactoryAttribute(Type adapterFactoryType) 
	        : base(AdapterRole.PropertyTreeBinder, adapterFactoryType) {
	        this.PropertyTreeBinderFactoryType = adapterFactoryType;
	    }

	}
}
