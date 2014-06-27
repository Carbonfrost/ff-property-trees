//
// - UntypedToTypedMetaObject.cs -
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
using Carbonfrost.Commons.PropertyTrees.Serialization;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class UntypedToTypedMetaObject : PropertyTreeMetaObject {

        private readonly Type componentType;
        private readonly Func<IEnumerable<Type>, PropertyTreeMetaObject> _transition;

        public override Type ComponentType {
            get {
                return this.componentType;
            }
        }

        public override object Component {
            get {
                return null;
            }
        }

        public UntypedToTypedMetaObject(Type componentType,
                                        Func<IEnumerable<Type>, PropertyTreeMetaObject> _transition)
        {
            this.componentType = componentType;
            this._transition = _transition;

        }

        internal override PropertyTreeMetaObject BindGenericParameters(IEnumerable<Type> types) {
            return _transition(types);
        }

    }

}
