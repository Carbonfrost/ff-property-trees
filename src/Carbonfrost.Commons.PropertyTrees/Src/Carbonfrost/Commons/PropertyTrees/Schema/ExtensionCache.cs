//
// - ExtensionCache.cs -
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    static class ExtensionCache {

        // This logic must be present here because the static initializer can cause assembly descriptions
        // to load in the middle of writing a definition were this logic on any other class in the Schema ns.

        internal static readonly IEnumerable<Assembly> extensionHelper;

        public static void Init(Assembly assembly) {
            if (!extensionHelper.Contains(assembly)) {
                Console.WriteLine("Unexpected: assembly not found {0}", assembly);
            }
        }

        static ExtensionCache() {
            extensionHelper = AppDomain.CurrentDomain.DescribeAssemblies(
                assembly => {
                    if (!assembly.IsExtension() || assembly.GetPropertyTreeOptions().SkipExtensionScanning)
                        return (IEnumerable<Assembly>) null;

                    foreach (Type type in assembly.GetTypesHelper()) {
                        if (!type.IsExtension())
                            continue;

                        foreach (MethodInfo mi in type.GetMethods()) {
                            if (mi.IsExtension()) {

                                foreach (RoleAttribute ra in Attribute.GetCustomAttributes(mi, typeof(RoleAttribute))) {
                                    ra.ProcessExtensionMethod(mi);
                                }
                            }
                        }
                    }

                    return new [] { assembly };
                });
        }

    }
}
