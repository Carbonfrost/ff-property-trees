//
// - PropertyTreeReader.Static.cs -
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
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    partial class PropertyTreeReader {

        static readonly Func<Stream, Encoding, PropertyTreeReaderSettings, PropertyTreeReader>[] CREATE = {
            (s, e, settings) => (PropertyTreeReader.CreateXml(s, e, new PropertyTreeXmlReaderSettings(settings))), // Unknown
            (s, e, settings) => (PropertyTreeReader.CreateXml(s, e, new PropertyTreeXmlReaderSettings(settings))), // Xml
            (s, e, settings) => { throw PropertyTreesFailure.BinaryNotSupported(); }, // Binary
            (s, e, settings) => (PropertyTreeReader.CreateXml(new GZipStream(s, CompressionMode.Decompress), e, new PropertyTreeXmlReaderSettings(settings))), // XmlGzip
        };

        public static PropertyTreeReader FromObject(object value,
                                                    PropertyTreeReaderSettings settings = null) {
            return new PropertyTreeObjectReader(value);
        }

        public static PropertyTreeReader Create(string fileName,
                                                Encoding encoding = null,
                                                PropertyTreeReaderSettings settings = null) {
            Require.NotNullOrEmptyString("fileName", fileName);
            return GetFactoryFunc(fileName)(File.OpenRead(fileName), encoding, settings);
        }

        public static PropertyTreeReader Create(StreamContext streamContext,
                                                Encoding encoding = null,
                                                PropertyTreeReaderSettings settings = null) {
            if (streamContext == null)
                throw new ArgumentNullException("streamContext"); // $NON-NLS-1

            string lp = string.Empty;
            if (streamContext.Uri.IsAbsoluteUri) {
                lp = streamContext.Uri.LocalPath;
            }

            return GetFactoryFunc(lp)(streamContext.OpenRead(),
                                      encoding,
                                      settings);
        }

        public static PropertyTreeReader Create(Stream stream,
                                                Encoding encoding = null,
                                                PropertyTreeReaderSettings settings = null) {
            if (stream == null)
                throw new ArgumentNullException("stream"); // $NON-NLS-1

            return CreateXml(stream, encoding, new PropertyTreeXmlReaderSettings(settings));
        }

        public static PropertyTreeXmlReader CreateXml(string fileName,
                                                      Encoding encoding = null,
                                                      PropertyTreeXmlReaderSettings settings = null) {
            // TODO Use the encoding
            XmlReaderSettings xsettings = new XmlReaderSettings {};
            XmlReader xr = XmlReader.Create(fileName, xsettings);
            return new PropertyTreeXmlReader(xr);
        }

        public static PropertyTreeXmlReader CreateXml(StreamContext streamContext,
                                                      Encoding encoding = null,
                                                      PropertyTreeXmlReaderSettings settings = null) {
            if (streamContext == null)
                throw new ArgumentNullException("streamContext"); // $NON-NLS-1

            XmlReaderSettings xsettings = new XmlReaderSettings {};
            // TODO Use the encoding
            XmlReader xr = XmlReader.Create(streamContext.OpenRead(), xsettings);
            return new PropertyTreeXmlReader(xr);
        }

        public static PropertyTreeXmlReader CreateXml(Stream stream,
                                                      Encoding encoding = null,
                                                      PropertyTreeXmlReaderSettings settings = null) {
            if (stream == null)
                throw new ArgumentNullException("stream"); // $NON-NLS-1

            // TODO Use the encoding
            XmlReader xr = XmlReader.Create(stream);
            return new PropertyTreeXmlReader(xr);
        }

        // TODO Better involvement of settings

        public static PropertyTreeXmlReader CreateXml(XmlReader reader,
                                                      PropertyTreeXmlReaderSettings settings = null) {
            if (reader == null)
                throw new ArgumentNullException("reader"); // $NON-NLS-1

            return new PropertyTreeXmlReader(reader);
        }

        public static PropertyTreeXmlReader CreateXml(TextReader reader,
                                                      PropertyTreeXmlReaderSettings settings = null) {
            if (reader == null)
                throw new ArgumentNullException("reader"); // $NON-NLS-1

            XmlReader xr = XmlReader.Create(reader);
            return new PropertyTreeXmlReader(xr);
        }

        static Func<Stream, Encoding, PropertyTreeReaderSettings, PropertyTreeReader> GetFactoryFunc(string lp) {
            return CREATE[(int) Utility.InferFlavor(lp)];
        }
    }

}
