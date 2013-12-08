//
// - PropertyTreePathConverter.cs -
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
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Carbonfrost.Commons.PropertyTrees {

    public sealed class PropertyTreePathConverter : TypeConverter {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (typeof(string).Equals(sourceType))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (typeof(string).Equals(destinationType))
                return true;
            else if (typeof(InstanceDescriptor).Equals(destinationType))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            string s = value as string;
            if (s != null)
                return PropertyTreePath.Parse(s);
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            PropertyTreePath path = value as PropertyTreePath;
            if (path != null) {
                if (typeof(string).Equals(destinationType))
                    return path.ToString();

                else if (typeof(InstanceDescriptor).Equals(destinationType))
                    return CreateInstanceDescriptor(path);
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private static InstanceDescriptor CreateInstanceDescriptor(PropertyTreePath path) {
            // TODO PropertyTreePathConverter.CreateInstanceDescriptor
            throw new NotImplementedException();
        }
    }
}

