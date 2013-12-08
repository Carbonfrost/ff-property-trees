//
// - TestUtils.cs -
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
using System.Xml;

namespace Tests {

    static class TestUtils {

        internal static void Times(Action func, int num1) {
            for (int i = 0; i < num1; i++)
                func();
        }

        public static bool AreEquivalent(XmlReader a, XmlReader b) {
            bool v;
            while ((v = a.Read()) == b.Read()) {
                if (v == false)
                    break;

                if (a.NodeType == b.NodeType
                    && a.Value == b.Value
                    && a.Name == b.Name
                    && a.NamespaceURI == b.NamespaceURI)
                    continue;
                else
                    return false;
            }
            return true;
        }
    }
}
