//
// - DefaultPopulateCallback.cs -
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
using System.Xml;

using Carbonfrost.Commons.ComponentModel;

namespace Carbonfrost.Commons.PropertyTrees {

    sealed class DefaultPopulateCallback : IPopulateComponentCallback {

        // IPopulateComponentCallback
        public void OnPropertyAnnotation(string property, InterfaceUsageInfo usageInfo) {
            _OnPropertyAnnotation(property, usageInfo);
        }

        public void OnEventAnnotation(string @event, InterfaceUsageInfo usageInfo) {
            _OnPropertyAnnotation(@event, usageInfo);
        }

        void _OnPropertyAnnotation(string property, InterfaceUsageInfo usageInfo) {
            if (usageInfo == null)
                throw new ArgumentNullException("usageInfo"); // $NON-NLS-1

            switch (usageInfo.Usage) {
                case InterfaceUsage.Missing:
                    throw PropertyTreesFailure.BinderMissingProperty(usageInfo);

                case InterfaceUsage.Obsolete:
                case InterfaceUsage.Preliminary:
                    if (usageInfo.IsError)
                        throw PropertyTreesFailure.BinderObsoleteProperty(usageInfo);

                    break;
            }
        }

        public void OnConversionException(string property, object value, Exception exception) {
            // Wrap the exception so that original exception site is maintained
            throw PropertyTreesFailure.ConversionGenericMessage(exception);
        }

    }
}
