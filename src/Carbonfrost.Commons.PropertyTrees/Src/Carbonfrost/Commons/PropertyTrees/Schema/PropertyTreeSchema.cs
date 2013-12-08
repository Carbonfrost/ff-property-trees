//
// - PropertyTreeSchema.cs -
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.Shared.Runtime.Components;

namespace Carbonfrost.Commons.PropertyTrees.Schema {
    
    [Serializable, StreamingSource(typeof(PropertyTreeSchemaSource))]
    public abstract class PropertyTreeSchema {

        static readonly IDictionary<Assembly, PropertyTreeSchema> schemas = new Dictionary<Assembly, PropertyTreeSchema>();

        public abstract NamespaceUri DefaultNamespace { get; set; }
        public abstract ComponentName SchemaName { get; set; }
        public abstract Assembly SourceAssembly { get; }
        public abstract PropertyTreeDefinitionCollection Types { get; }

        public static PropertyTreeSchema FromAssembly(Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException("assembly"); // $NON-NLS-1

            return schemas.GetValueOrCache(assembly,
                                           a => new ReflectedPropertyTreeSchema(a));
        }

        public PropertyTreeDefinition DefineType(string name) {
            Require.NotNullOrEmptyString("name", name);

            throw new NotImplementedException();
        }

        public PropertyTreeDefinition DefineType(QualifiedName name) {
            if (name == null)
                throw new ArgumentNullException("name");
            throw new NotImplementedException();
        }

        public PropertyTreeDefinition GetType(Type type, bool declaredOnly = false) {
            if (type == null)
                throw new ArgumentNullException("type");
            throw new NotImplementedException();
        }

        public PropertyTreeDefinition ImportType(PropertyTreeDefinition definition) {
            if (definition == null)
                throw new ArgumentNullException("definition");
            throw new NotImplementedException();
        }

        public static PropertyTreeSchema FromFile(string file) {
            if (file == null)
                throw new ArgumentNullException("file");
            if (string.IsNullOrEmpty(file))
                throw Failure.EmptyString("file");
            
            return FromStreamContext(StreamContext.FromFile(file));
        }

        public static PropertyTreeSchema FromSource(Uri source) {
            if (source == null)
                throw new ArgumentNullException("source");
            
            return FromStreamContext(StreamContext.FromSource(source));
        }
        
        public static PropertyTreeSchema FromStreamContext(StreamContext source) {
            if (source == null)
                throw new ArgumentNullException("source");

            throw new NotImplementedException();
        }
    }
}
