//
// - ReflectedPropertyTreeDefinition.cs -
//
// Copyright 2012 Carbonfrost Systems, Inc. (http://carbonfrost.com)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared;
using Carbonfrost.Commons.Shared.Runtime;

namespace Carbonfrost.Commons.PropertyTrees.Schema {

    sealed class ReflectedPropertyTreeDefinition : PropertyTreeDefinition {

        private readonly Type type;
        private readonly OperatorDefinitionCollection operators;
        private PropertyDefinition defaultProperty;
        private readonly PropertyTreeFactoryDefinition activationConstructor;
        private PropertyDefinitionCollection properties;
        private bool scanAdd;

        public override PropertyTreeSchema Schema {
            get {
                return PropertyTreeSchema.FromAssembly(type.Assembly);
            }
        }

        public override Type SourceClrType {
            get { return this.type; }
        }

        public override PropertyDefinitionCollection Properties {
            get {
                EnsureProperties();
                return properties;
            }
        }

        public override PropertyDefinition DefaultProperty {
            get {
                EnsureProperties();
                return defaultProperty;
            }
        }

        public override PropertyTreeFactoryDefinition Constructor {
            get {
                return activationConstructor;
            }
        }

        public ReflectedPropertyTreeDefinition(Type type) {
            this.type = type;
            this.operators = new OperatorDefinitionCollection();

            if (type.IsAbstract) {

                // Composable providers can be abstract
                if (type.IsProviderType() && type.IsDefined(typeof(ComposableAttribute), false)) {

                    var composeMember = AppDomain.CurrentDomain.GetProviderMember(type, "compose");
                    if (composeMember == null)
                        throw new NotImplementedException();

                    this.activationConstructor = ReflectedProviderFactoryDefinitionBase.Create(type, composeMember);
                }

            } else {
                MethodBase ctor = TypeHelper.FindActivationConstructor(type);
                if (ctor != null) {
                    this.activationConstructor = ReflectedPropertyTreeFactoryDefinition.Create(null, ctor);
                }
            }
        }

        public override OperatorDefinitionCollection Operators {
            get {
                EnsureFactoryDefinitions();
                return this.operators;
            }
        }

        public override string Namespace {
            get {
                var nn = type.GetQualifiedName();
                return nn.NamespaceName;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override string Name {
            get {
                return type.Name;
            }
            set {
                throw Failure.ReadOnlyProperty();
            }
        }

        public override PropertyDefinition GetProperty(QualifiedName name, GetPropertyOptions options) {
            PropertyDefinition result = this.Properties[name];
            if (result != null)
                return result;

            bool declaredOnly = options.HasFlag(GetPropertyOptions.DeclaredOnly);
            if (declaredOnly)
                return null;

            var comparer = options.HasFlag(GetPropertyOptions.IncludeExtenders)
                ? Utility.OrdinalIgnoreCaseQualifiedName
                : Utility.AttachedOrdinalIgnoreCaseQualifiedName;
            return EnumerateProperties().FirstOrDefault(t => comparer.Equals(t.QualifiedName, name));
        }

        public override OperatorDefinition GetOperator(QualifiedName name, bool declaredOnly = false) {
            if (name == null)
                throw new ArgumentNullException("name");

            OperatorDefinition result = this.Operators[name];
            if (result != null)
                return result;

            if (declaredOnly)
                return null;

            return EnumerateOperators().FirstOrDefault(t => Utility.OrdinalIgnoreCaseQualifiedName.Equals(t.QualifiedName, name));
        }

        private IEnumerable<Type> EnumerateInheritedTypes() {
            return Utility.EnumerateInheritedTypes(this.SourceClrType);
        }

        private void FindAllOperators(Type type) {
            HashSet<MethodInfo> explicitOperators = new HashSet<MethodInfo>();

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)) {
                bool match = false;
                // TODO Check signature of these methods because they could be illegal
                foreach (RoleAttribute attribute in FindRoleAttributes(method)) {
                    var result = attribute.BuildInstance(method);
                    if (result != null) {
                        this.operators.Add(result);
                        match = true;
                    }
                }

                if (match)
                    explicitOperators.Add(method);
            }

            foreach (var mi in type.GetMethods().Except(explicitOperators)) {
                if (!mi.IsPublic || mi.IsStatic)
                    continue;
                ParameterInfo[] parameters = mi.GetParameters();

                switch (mi.Name) {
                    case "Add":
                        if (IsValidAddon(mi, parameters)) {
                            // TODO We're consuming duplicated names (here, below); they should be errors
                            this.operators.TryAdd(ReflectedPropertyTreeFactoryDefinition.FromListAddMethod(mi));
                            var natural = ReflectedPropertyTreeFactoryDefinition.FromListAddMethod(mi, true);
                            if (natural != null && !this.operators.ContainsKey(natural.QualifiedName))
                                this.operators.Add(natural);
                        }
                        break;

                    case "Clear":
                        this.operators.Add(new ReflectedClearDefinition(null, mi));
                        break;

                    case "RemoveAt":
                        this.operators.TryAdd(new ReflectedRemoveDefinition(null, mi));
                        break;

                    case "Remove":
                        break;
                }
            }
        }

        static bool IsValidAddon(MethodInfo mi, ParameterInfo[] parameters)
        {
            if (parameters.Length == 1) {
                var param = parameters[0];

                return mi.ReturnType == typeof(void)
                    && !mi.IsDefined(typeof(AddAttribute), false)
                    && !param.IsOut
                    && !param.ParameterType.IsByRef;
            }

            return false;
        }

        static bool IsValidIndexer(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Property)
                return false;

            PropertyInfo pi = (PropertyInfo) member;
            MethodInfo mi = pi.GetGetMethod();
            if (mi == null)
                return false;

            ParameterInfo[] parameters = mi.GetParameters();
            if (parameters.Length == 1) {
                var param = parameters[0];

                return (param.ParameterType == typeof(string) || param.ParameterType == typeof(QualifiedName));
            }

            return false;
        }

        IEnumerable<RoleAttribute> FindRoleAttributes(MethodInfo method) {
            var result = (IEnumerable<RoleAttribute>) Attribute.GetCustomAttributes(method, typeof(RoleAttribute));

            foreach (var data in method.GetCustomAttributesData()) {
                if (data.Constructor.DeclaringType.Assembly.Equals(typeof(ReflectedPropertyTreeDefinition).Assembly)) {
                    continue;
                }

                if (data.Constructor.DeclaringType.FullName == typeof(AddAttribute).FullName) {
                    return new [] { AddAttribute.Default };
                }
            }
            return result;
        }

        internal void AddFactoryDefinition(ReflectedPropertyTreeFactoryDefinition definition) {
            this.operators.AddInternal(definition);
        }

        internal void AddPropertyDefinition(ReflectedExtenderPropertyDefinition definition) {
            this.properties.AddInternal(definition);
        }

        void EnsureProperties() {
            if (this.properties == null) {
                this.properties = new PropertyDefinitionCollection(
                    TypeDescriptor.GetProperties(this.SourceClrType).Cast<PropertyDescriptor>().Select(t => new ReflectedPropertyDefinition(t)));

                var defaultMember = (PropertyInfo) this.type.GetDefaultMembers().FirstOrDefault(IsValidIndexer);
                if (defaultMember != null) {
                    this.defaultProperty = new ReflectedIndexerPropertyDefinition(defaultMember);
                    this.Properties.AddInternal(this.defaultProperty);
                }

                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                FindExtensionProperties(methods);

                // Could define extender properties
                ExtensionCache.Init(type.Assembly);
                this.properties.MakeReadOnly();
            }
        }

        void FindExtensionProperties(IEnumerable<MethodInfo> methods) {
            var results = new Dictionary<AttachedPropertyID, ReflectedExtenderPropertyDefinition>();

            foreach (var method in methods) {
                foreach (var epa in method.GetCustomAttributes(typeof(ExtenderAttribute)).Cast<ExtenderAttribute>()) {
                    var pid = epa.GetAttachedPropertyID(method);
                    var current = results.GetValueOrCache(
                        pid, _ => new ReflectedExtenderPropertyDefinition(pid));

                    current.AddMethod(method);
                }
            }

            foreach (var item in results.Values) {
                this.properties.AddInternal(item);
            }
        }

        private void EnsureFactoryDefinitions() {
            if (!this.scanAdd) {
                this.scanAdd = true;
                FindAllOperators(type);
                FindProviderAddChildOperators(type);
            }

            ExtensionCache.Init(type.Assembly);
        }

        private void FindProviderAddChildOperators(Type type) {
            Type arg;

            if (type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == typeof(IAddChild<>)
                && (arg = type.GetGenericArguments()[0]).IsProviderType()) {

                foreach (var m in AppDomain.CurrentDomain.GetProviderMembers(arg)) {
                    var item = ReflectedProviderFactoryDefinitionBase.Create(arg, m);
                    if (this.operators.ContainsKey(item.QualifiedName)) {
                        // TODO If two operators are defined with the same name, trace an error (StatusAppender.ForType(PropertyTreeSchema))
                        continue;
                    }
                    this.operators.Add(item);
                }
            }
        }

        public override IEnumerable<PropertyDefinition> EnumerateProperties(bool declaredOnly = false) {
            IEnumerable<PropertyDefinition> result = this.Properties;
            foreach (var t in EnumerateInheritedTypes()) {
                var ops = PropertyTreeDefinition.FromType(t).Properties;
                result = result.Concat(ops);
            }

            return result;
        }

        public override IEnumerable<OperatorDefinition> EnumerateOperators(bool declaredOnly = false) {
            IEnumerable<OperatorDefinition> result = this.Operators;
            foreach (var t in EnumerateInheritedTypes()) {
                var ops = PropertyTreeDefinition.FromType(t).Operators;
                result = result.Concat(ops);
            }

            return result;
        }

        public override PropertyDefinition GetProperty(string name, GetPropertyOptions options) {
            // TODO Probably treat as ambiguous
            bool declaredOnly = options.HasFlag(GetPropertyOptions.DeclaredOnly);
            bool attached = options.HasFlag(GetPropertyOptions.IncludeExtenders);
            var props = declaredOnly ? this.Properties : EnumerateProperties();
            return props.ByLocalName(name, attached).FirstOrDefault();
        }

        public override OperatorDefinition GetOperator(string name, bool declaredOnly) {
            var props = declaredOnly ? this.Operators : EnumerateOperators();
            return props.ByLocalName(name).FirstOrDefault();
        }
    }
}
