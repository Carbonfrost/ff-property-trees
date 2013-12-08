//
// - PropertyNode.Helpers.cs -
//
// Copyright 2013 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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

namespace Carbonfrost.Commons.PropertyTrees {

    partial class PropertyNode {
        
        // TODO This is a IPropertyTreeClient (futures)

        public bool GetBoolean(string path) {
            return Convert.ToBoolean(GetValue(path));
        }

        public bool GetBoolean(PropertyTreePath path) {
            return Convert.ToBoolean(GetValue(path));
        }

        public byte GetByte(string path) {
            return Convert.ToByte(GetValue(path));
        }

        public byte GetByte(PropertyTreePath path) {
            return Convert.ToByte(GetValue(path));
        }

        public char GetChar(string path) {
            return Convert.ToChar(GetValue(path));
        }

        public char GetChar(PropertyTreePath path) {
            return Convert.ToChar(GetValue(path));
        }

        public DateTime GetDateTime(string path) {
            return Convert.ToDateTime(GetValue(path));
        }

        public DateTime GetDateTime(PropertyTreePath path) {
            return Convert.ToDateTime(GetValue(path));
        }

        public DateTimeOffset GetDateTimeOffset(string path) {
            throw new NotImplementedException();
        }

        public DateTimeOffset GetDateTimeOffset(PropertyTreePath path) {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(string path) {
            return Convert.ToDecimal(GetValue(path));
        }

        public decimal GetDecimal(PropertyTreePath path) {
            return Convert.ToDecimal(GetValue(path));
        }

        public double GetDouble(string path) {
            return Convert.ToDouble(GetValue(path));
        }

        public double GetDouble(PropertyTreePath path) {
            return Convert.ToDouble(GetValue(path));
        }

        public Guid GetGuid(string path) {
            throw new NotImplementedException();
        }

        public Guid GetGuid(PropertyTreePath path) {
            throw new NotImplementedException();
        }

        public short GetInt16(string path) {
            return Convert.ToInt16(GetValue(path));
        }

        public short GetInt16(PropertyTreePath path) {
            return Convert.ToInt16(GetValue(path));
        }

        public int GetInt32(string path) {
            return Convert.ToInt32(GetValue(path));
        }

        public int GetInt32(PropertyTreePath path) {
            return Convert.ToInt32(GetValue(path));
        }

        public long GetInt64(string path) {
            return Convert.ToInt64(GetValue(path));
        }

        public long GetInt64(PropertyTreePath path) {
            return Convert.ToInt64(GetValue(path));
        }

        [CLSCompliant(false)]
        public sbyte GetSByte(string path) {
            return Convert.ToSByte(GetValue(path));
        }

        [CLSCompliant(false)]
        public sbyte GetSByte(PropertyTreePath path) {
            return Convert.ToSByte(GetValue(path));
        }

        public float GetSingle(string path) {
            return Convert.ToSingle(GetValue(path));
        }

        public float GetSingle(PropertyTreePath path) {
            return Convert.ToSingle(GetValue(path));
        }

        public string GetString(string path) {
            return Convert.ToString(GetValue(path));
        }

        public string GetString(PropertyTreePath path) {
            return Convert.ToString(GetValue(path));
        }

        public TimeSpan GetTimeSpan(string path) {
            throw new NotImplementedException();
        }

        public TimeSpan GetTimeSpan(PropertyTreePath path) {
            throw new NotImplementedException();
        }

        public PropertyTree GetPropertyTree(string path) {
            throw new NotImplementedException();
        }

        public PropertyTree GetPropertyTree(PropertyTreePath path) {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public ushort GetUInt16(string path) {
            return Convert.ToUInt16(GetValue(path));
        }

        [CLSCompliant(false)]
        public ushort GetUInt16(PropertyTreePath path) {
            return Convert.ToUInt16(GetValue(path));
        }

        [CLSCompliant(false)]
        public uint GetUInt32(string path) {
            return Convert.ToUInt32(GetValue(path));
        }

        [CLSCompliant(false)]
        public uint GetUInt32(PropertyTreePath path) {
            return Convert.ToUInt32(GetValue(path));
        }

        [CLSCompliant(false)]
        public ulong GetUInt64(string path) {
            return Convert.ToUInt64(GetValue(path));
        }

        [CLSCompliant(false)]
        public ulong GetUInt64(PropertyTreePath path) {
            return Convert.ToUInt64(GetValue(path));
        }

        public Uri GetUri(string path) {
            return new Uri(Convert.ToString(GetValue(path)));
        }

        public Uri GetUri(PropertyTreePath path) {
            return new Uri(Convert.ToString(GetValue(path)));
        }

        // TODO Correctly handle paths

        public void SetValue(string path, object value) {
            PropertyNode node = this.Children[path];
            bool prop = Utility.IsProperty(value);

            if (value == null && node != null) {
                this.RemoveChild(node);

            } else if (node == null || prop != node.IsProperty) {

                if (prop) {
                    this.AppendProperty(path, value);
                } else {
                    PropertyTree tree = this.AppendTree(path);
                    tree.Value = value;
                }

            } else {
                node.Value = value;
            }
        }

        public void SetValue(PropertyTreePath path, object value) {
            throw new NotImplementedException();
        }

        private object GetValue(PropertyTreePath path) {
            throw new NotImplementedException();
        }

        private object GetValue(string path) {
            PropertyNode node = this.Children[path];
            if (node == null)
                return null;
            else
                return node.Value;
        }

    }
}
