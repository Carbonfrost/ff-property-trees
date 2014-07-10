//
// - SerializerDirectiveFactory.cs -
//
// Copyright 2014 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Linq;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    class SerializerDirectiveFactory {

        private readonly PropertyTreeBinderImpl parent;

        private IPropertyTreeBinderErrors Errors {
            get {
                return parent.TryGetService(PropertyTreeBinderErrors.Default);
            }
        }

        public SerializerDirectiveFactory(PropertyTreeBinderImpl parent)
        {
            this.parent = parent;
        }

        public TargetTypeDirective CreateTargetType(PropertyTreeNavigator nav) {
            if (nav.IsProperty) {
                Exception error;

                try {
                    var tr = TypeReference.Parse(Convert.ToString(nav.Value), parent.GetBasicServices(nav));
                    return new TargetTypeDirective(tr, null);

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    error = ex;
                    Errors.BadTargetTypeDirective(error, nav.FileLocation);
                    return null;
                }
            }
            else {

                try {
                    return nav.Bind<TargetTypeDirective>();

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    Errors.BadTargetTypeDirective(ex, nav.FileLocation);
                    return null;
                }
            }
        }

        public TargetProviderDirective CreateTargetProvider(Type providerType, PropertyTreeNavigator nav) {
            var serviceProvider = parent.GetBasicServices(nav);

            if (nav.IsProperty) {
                string text = nav.Value.ToString();
                Exception error;

                try {
                    var qn = QualifiedName.Parse(text, serviceProvider);
                    return new TargetProviderDirective { Name = qn };

                } catch (ArgumentException e) {
                    error = e;
                } catch (FormatException e) {
                    error = e;
                }

                Errors.BadTargetProviderDirective(error, nav.FileLocation);
                return null;
            }
            else {
                try {
                    return nav.Bind<TargetProviderDirective>();

                } catch (Exception ex) {
                    if (Require.IsCriticalException(ex))
                        throw;

                    Errors.BadTargetProviderDirective(ex, nav.FileLocation);
                    return null;
                }
            }
        }

        public TargetSourceDirective CreateTargetSource(PropertyTreeNavigator nav, IUriContext uriContext) {
            if (nav.IsProperty) {

                Exception error;
                try {
                    Uri uri = new Uri(nav.Value.ToString(), UriKind.RelativeOrAbsolute);
                    uri = Utility.CombineUri(uriContext, uri);

                    return new TargetSourceDirective(uri);

                } catch (ArgumentException e) {
                    error = e;
                } catch (UriFormatException e) {
                    error = e;
                }

                Errors.BadSourceDirective(error, nav.FileLocation);
                return null;
            }

            else {
                return nav.Bind<TargetSourceDirective>();
            }
        }

    }

}
