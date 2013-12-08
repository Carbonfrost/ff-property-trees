//
// - PropertyTreePath.cs -
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
using System.ComponentModel;

namespace Carbonfrost.Commons.PropertyTrees {

    [TypeConverter(typeof(PropertyTreePathConverter))]
    public partial class PropertyTreePath {

        public static PropertyTreePath Parse(string text) {
            PropertyTreePath result;
            Exception ex = _TryParse(text, out result);
            if (ex == null)
                return result;
            else
                throw ex;
        }

        public static bool TryParse(string text, out PropertyTreePath result) {
            return _TryParse(text, out result) == null;
        }

        static Exception _TryParse(string text, out PropertyTreePath result) {
            // TODO Parse paths
            throw new NotImplementedException();
        }

        // TODO Represent property tree paths
    }
}
