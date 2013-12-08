//
// - PropertyTree.Static.cs -
//
// Copyright 2010, 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.IO;
using System.Text;

using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    public partial class PropertyTree {

        public static PropertyTree FromValue(object value, PropertyTreeValueOptions options = PropertyTreeValueOptions.None) {
            if (value == null)
                throw new ArgumentNullException("value"); // $NON-NLS-1

            if (options.HasFlag(PropertyTreeValueOptions.Live))
                throw new NotImplementedException();

            // TODO Rework this to use PropertyTreeObjectReader
            PropertyTree result = new PropertyTree();
            var navigator = result.CreateNavigator();
            
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value)) {
                object val = property.GetValue(value);
                if (!property.Converter.CanConvertTo(typeof(string)))
                    throw new NotImplementedException();
                
                navigator.AppendProperty(property.Name, val);
            }

            return result;
        }

        public static PropertyTree FromFile(string fileName,
                                            Encoding encoding = null) {

            using (FileStream fs = File.OpenRead(fileName))
                return FromStream(fs, encoding);
        }

        public static PropertyTree FromStream(Stream stream,
                                              Encoding encoding = null) {
            if (stream == null)
                throw new ArgumentNullException("stream"); // $NON-NLS-1

            using (var reader = PropertyTreeReader.CreateXml(stream, encoding, null)) {
                if (reader.Read())
                    return reader.ReadPropertyTree();
                else
                    throw Failure.Eof();
            }
        }

        public static PropertyTree FromStreamContext(StreamContext streamContext, Encoding encoding) {
            if (streamContext == null)
                throw new ArgumentNullException("streamContext"); // $NON-NLS-1

            using (PropertyTreeReader reader = PropertyTreeReader.CreateXml(streamContext, encoding, null)) {
                return reader.ReadPropertyTree();
            }
        }
    }
}
