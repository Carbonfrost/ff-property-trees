//
// - PropertyTreeException.cs -
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
using System.Runtime.Serialization;
using System.Xml;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees {

    [Serializable]
    public class PropertyTreeException : Exception, IXmlLineInfo {

        public string SourceUri {
            get { return this.FileLocation.FileName; }
        }

        public PropertyTreeException() : this(null, -1, -1) {}

        public PropertyTreeException(string message) : this(message, -1, -1) {}

        public PropertyTreeException(string message, int lineNumber, int linePosition)
            : this(message, null, lineNumber, linePosition) {}

        protected PropertyTreeException(SerializationInfo info, StreamingContext context) : base(info, context) {
            this.FileLocation = (FileLocation) info.GetValue("fileLocation", typeof(FileLocation));
        }

        public PropertyTreeException(string message, Exception innerException) : base(message, innerException) {}

        public PropertyTreeException(string message, Exception innerException, int lineNumber, int linePosition)
            : this(message, innerException, new FileLocation(lineNumber, linePosition, null)) {
        }

        public PropertyTreeException(string message, Exception innerException, FileLocation fileLocation)
            : base(BuildMessage(message, fileLocation), innerException) {
        }

        static string BuildMessage(string message, FileLocation location) {
            if (location.IsEmpty)
                return message;
            else
                return message + Environment.NewLine + Environment.NewLine + location.ToString("h");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            info.AddValue("fileLocation", FileLocation);
        }

        // `IXmlLineInfo' implementation
        public int LineNumber {
            get { return this.FileLocation.LineNumber; }
        }

        public int LinePosition {
            get { return this.FileLocation.LinePosition; }
        }

        public FileLocation FileLocation { get; private set; }

        bool IXmlLineInfo.HasLineInfo() {
            return this.LineNumber > 0 && this.LinePosition > 0;
        }
    }

}
