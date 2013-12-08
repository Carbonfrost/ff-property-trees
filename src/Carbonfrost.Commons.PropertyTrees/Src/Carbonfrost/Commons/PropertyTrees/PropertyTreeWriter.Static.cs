//
// - PropertyTreeWriter.Static.cs -
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
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class PropertyTreeWriter {

         static readonly Func<Stream, Encoding, PropertyTreeWriterSettings, PropertyTreeWriter>[] CREATE = {
            (s, e, settings) => (PropertyTreeWriter.CreateXml(s, e, new PropertyTreeWriterSettings(settings))), // Unknown
            (s, e, settings) => (PropertyTreeWriter.CreateXml(s, e, new PropertyTreeWriterSettings(settings))), // Xml
            (s, e, settings) => { throw PropertyTreesFailure.BinaryNotSupported(); }, // Binary
            (s, e, settings) => (PropertyTreeWriter.CreateXml(new GZipStream(s, CompressionMode.Decompress), e, new PropertyTreeWriterSettings(settings))), // XmlGzip
        };


        public static PropertyTreeXmlWriter CreateXml(XmlWriter xmlWriter, PropertyTreeWriterSettings settings = null) {
            if (xmlWriter == null)
                throw new ArgumentNullException("xmlWriter"); // $NON-NLS-1

            return new PropertyTreeXmlWriter(xmlWriter, settings);
        }

        public static PropertyTreeXmlWriter CreateXml(TextWriter outputWriter, PropertyTreeWriterSettings settings = null) {
            if (outputWriter == null)
                throw new ArgumentNullException("outputWriter"); // $NON-NLS-1

            return CreateXml(XmlWriter.Create(outputWriter), settings);
        }

        public static PropertyTreeXmlWriter CreateXml(Stream output,
                                                      Encoding encoding = null,
                                                      PropertyTreeWriterSettings settings = null) {
            if (output == null)
                throw new ArgumentNullException("output"); // $NON-NLS-1

            var xsettings = new XmlWriterSettings {
                Encoding = encoding ?? Encoding.UTF8
            };
            return CreateXml(XmlWriter.Create(output, xsettings), settings);
        }

        public static PropertyTreeXmlWriter CreateXml(string outputFileName,
                                                      PropertyTreeWriterSettings settings = null) {

            return CreateXml(XmlWriter.Create(outputFileName), settings);
        }

    }
}
