//
// - PropertyTreeBinderStep.cs -
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.PropertyTrees.Schema;

namespace Carbonfrost.Commons.PropertyTrees.Serialization {

    partial class PropertyTreeBinderImpl {

        public Dictionary<string, PropertyTreeMetaObject> ExtractParameterDictionary(
            OperatorDefinition op,
            PropertyTreeMetaObject target,
            IServiceProvider serviceProvider,
            NodeList children) {

            // Named constructor arguments
            var duplicates = new HashSet<QualifiedName>();
            var mapped = new Dictionary<QualifiedName, PropertyTreeNavigator>();
            foreach (var child in children) {
                // Implicitly map default NS to real
                var impliedName = ImpliedName(child, target);

                if (duplicates.Contains(impliedName)) {
                    // Duplicates can't bind to parameters (only to param arrays)

                } else if (mapped.ContainsKey(impliedName)) {
                    // Detected a duplicate
                    duplicates.Add(impliedName);
                    mapped.Remove(impliedName);

                } else {
                    mapped.Add(impliedName, child);
                }
            }

            var args = new Dictionary<string, PropertyTreeMetaObject>(op.Parameters.Count);

            PropertyDefinition myParam = null;
            List<string> requiredMissing = new List<string>();

            foreach (PropertyDefinition p in op.Parameters) {
                // Fallback to empty ns
                PropertyTreeNavigator nav;
                QualifiedName impliedName = p.QualifiedName;
                if (p.QualifiedName.Namespace.IsDefault) {
                    impliedName = impliedName.ChangeNamespace(op.Namespace);
                }

                if (mapped.TryGetValue(impliedName, out nav)) {
                    // Binds a parameter required for activating an instance
                    // TODO Should we supply/use attributes from the parameter
                    // and/or corresponding property descriptor?

                    var childContext = target.CreateChild(p.PropertyType);
                    args[p.Name] = nav.TopLevelBind(childContext, serviceProvider);
                    children.Remove(nav);
                }
                else if (p.IsOptional) {
                    PropertyTreeMetaObject defaultValue;
                    if (p.DefaultValue == null)
                        defaultValue = PropertyTreeMetaObject.Create(p.PropertyType);
                    else
                        defaultValue = PropertyTreeMetaObject.Create(p.DefaultValue);
                    args[p.Name] = defaultValue;

                } else if (p.IsParamArray)
                    myParam = p;

                else if (TypeHelper.IsParameterRequired(p.PropertyType)) {
                    requiredMissing.Add(Utility.DisplayName(p.QualifiedName));
                }
            }

            if (requiredMissing.Count > 0)
                errors.RequiredPropertiesMissing(requiredMissing, op, FindFileLocation(serviceProvider));

            if (myParam == null && target.GetDefinition().DefaultProperty == null && duplicates.Any(t => target.SelectProperty(t) != null))
                errors.DuplicatePropertyName(duplicates, FindFileLocation(serviceProvider));

            // Try param array
            if (myParam != null) {
                var all = new List<object>();
                var elementType = myParam.PropertyType.GetElementType();
                foreach (var kvp in children) {

                    // Bind child nodes so tha latebound applies
                    var childrenList = NodeList.Create(PropertyTreeBinderImpl.SelectChildren(kvp));
                    var inline = BindChildNodes(PropertyTreeMetaObject.Create(elementType), kvp, childrenList);
                    var inlineVal = inline.Component;
                    all.Add(inlineVal);
                }

                children.Clear();
                var array = Array.CreateInstance(elementType, all.Count);
                ((System.Collections.ICollection) all).CopyTo(array, 0);
                args[myParam.Name] = PropertyTreeMetaObject.Create(array);
            }

            return args;
        }

        abstract class PropertyTreeBinderStep {

            public PropertyTreeBinderImpl Parent { get; set; }

            public SerializerDirectiveFactory DirectiveFactory {
                get {
                    return Parent.DirectiveFactory;
                }
            }

            public PropertyTreeMetaObject Process(
                PropertyTreeMetaObject target,
                PropertyTreeNavigator self,
                NodeList children)
            {

                target = StartStep(target, self, children);
                return EndStep(target);
            }

            public virtual PropertyTreeMetaObject StartStep(PropertyTreeMetaObject target,
                                                            PropertyTreeNavigator self,
                                                            NodeList children) {
                return target;
            }

            public virtual PropertyTreeMetaObject EndStep(PropertyTreeMetaObject target)
            {
                return target;
            }

            internal Dictionary<string, PropertyTreeMetaObject> ExtractParameterDictionary(
                OperatorDefinition op,
                PropertyTreeMetaObject target,
                IServiceProvider serviceProvider,
                NodeList children)
            {
                return Parent.ExtractParameterDictionary(op, target, serviceProvider, children);
            }
        }
    }

}
