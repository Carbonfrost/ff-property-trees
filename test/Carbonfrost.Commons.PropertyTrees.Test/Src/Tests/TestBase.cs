//
// - TestBase.cs -
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
using System.Xml;
using Carbonfrost.Commons.PropertyTrees;

namespace Tests {

    public abstract class TestBase {

        protected static PropertyTree LoadTree(string fileName) {
            return PropertyTree.FromFile(GetContentPath(fileName));
        }

        protected static PropertyTreeReader LoadContent(string fileName) {
			if (fileName.EndsWith(".xml", StringComparison.Ordinal))
				return PropertyTreeReader.CreateXml(GetContentPath(fileName));
			else
				throw new NotImplementedException();
        }

        protected static string GetContentPath(string fileName) {
            return Path.Combine("./Content/", fileName);
        }

        protected static XmlReader GetXmlReader(string fileName) {
            return XmlReader.Create(Path.Combine("./Content/", fileName));
        }

        protected static string GetContent(string fileName) {
            return File.ReadAllText(GetContentPath(fileName));
        }

    }
}
