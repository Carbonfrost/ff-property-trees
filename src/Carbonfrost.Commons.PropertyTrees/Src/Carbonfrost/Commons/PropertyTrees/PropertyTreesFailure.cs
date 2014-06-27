//
// - PropertyTreesFailure.cs -
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
using System.Collections.Generic;
using System.Globalization;
using Carbonfrost.Commons.ComponentModel;
using Carbonfrost.Commons.Shared.Runtime;
using Carbonfrost.Commons.PropertyTrees.Resources;
using Carbonfrost.Commons.Shared;

namespace Carbonfrost.Commons.PropertyTrees {

    static class PropertyTreesFailure {

        public static InvalidOperationException CannotAppendChild() {
            return Failure.Prepare(new InvalidOperationException(SR.CannotAppendChild()));
        }

        public static ArgumentException NotAcceptiblePropertyType(Type propertyType) {
            return Failure.Prepare(new ArgumentOutOfRangeException("propertyType", propertyType, SR.NotAcceptiblePropertyType()));
        }

        public static InvalidOperationException InvalidFactoryType(string fullName) {
            return Failure.Prepare(new InvalidOperationException(SR.InvalidFactoryType(fullName)));
        }

        public static NotImplementedException BinaryNotSupported() {
            return Failure.Prepare(new NotImplementedException(SR.BinaryNotSupported()));
        }

        public static PropertyTreeException ReaderWrongPosition(PropertyNodeType expect, PropertyNodeType actual) {
            return Failure.Prepare(new PropertyTreeException(SR.ReaderWrongPosition(expect, actual)));
        }

        public static PropertyTreeException UnmatchedMembersGenericError(Exception ex, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.UnmatchedMembersGenericError(), ex, loc));
        }

        public static PropertyTreeException BinderMissingProperty(InterfaceUsageInfo usageInfo) {
            return Failure.Prepare(new PropertyTreeException(usageInfo.Message) {
                                       HelpLink = Convert.ToString(usageInfo.HelpUri),
                                   });
        }

        public static PropertyTreeException BinderObsoleteProperty(InterfaceUsageInfo usageInfo) {
            return Failure.Prepare(new PropertyTreeException(usageInfo.Message) {
                                       HelpLink = Convert.ToString(usageInfo.HelpUri),
                                   });
        }

        public static PropertyTreeException BinderConversionError(string text, string propertyName, Type type, Exception exception, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.BinderConversionError(propertyName, type), exception, loc));
        }

        public static PropertyTreeException LateBoundTypeMissing(int lineNumber, int linePosition) {
            return Failure.Prepare(new PropertyTreeException(SR.LateBoundTypeMissing(), lineNumber, linePosition));
        }

        public static PropertyTreeException RequiredPropertiesMissing(IEnumerable<string> names, string hintSignature, int lineNumber, int linePosition) {
            return Failure.Prepare(new PropertyTreeException(
                SR.RequiredPropertiesMissing(string.Join(", ", names), hintSignature),
                lineNumber,
                linePosition));
        }

        public static PropertyTreeException LateBoundTypeNotFound(object value, int lineNumber, int linePosition) {
            return Failure.Prepare(new PropertyTreeException(SR.LateBoundTypeNotFound(value), lineNumber, linePosition));
        }

        public static PropertyTreeException ConstructorRequired(Type componentType, int lineNumber, int linePosition) {
            return Failure.Prepare(new PropertyTreeException(SR.ConstructorRequired(componentType), lineNumber, linePosition));
        }

        public static PropertyTreeException ReaderNotMoved() {
            return Failure.Prepare(new PropertyTreeException(SR.ReaderNotMoved()));
        }

        public static InvalidOperationException WouldCreateMalformedDocument() {
            return Failure.Prepare(new InvalidOperationException(SR.WouldCreateMalformedDocument()));
        }

        public static InvalidOperationException WriterIncorrectState(object currentState) {
            return Failure.Prepare(new InvalidOperationException(SR.WriterIncorrectState(currentState)));
        }

        public static InvalidOperationException WriterPropertyOrTreeExists(QualifiedName qn) {
            return Failure.Prepare(new InvalidOperationException(SR.WriterPropertyOrTreeExists(qn)));
        }

        public static InvalidOperationException WriterOnlyPropertyTreesNamespace() {
            return Failure.Prepare(new InvalidOperationException(SR.WriterOnlyPropertyTreesNamespace()));
        }

        public static InvalidOperationException WouldCreateMalformedDocumentRootRequired() {
            return Failure.Prepare(new InvalidOperationException(SR.WouldCreateMalformedDocumentRootRequired()));
        }

        public static InvalidOperationException CannotDeleteRoot() {
            return Failure.Prepare(new InvalidOperationException(SR.CannotDeleteRoot()));
        }

        public static ArgumentException NotFromThisClient(string argName) {
            return Failure.Prepare(new ArgumentException(SR.NotFromThisClient(), argName));
        }

        public static ArgumentException MergeNamedMustMatch(string argName) {
            return Failure.Prepare(new ArgumentException(SR.MergeNamedMustMatch(), argName));
        }

        public static ArgumentException ExpectedClosingBrace() {
            return Failure.Prepare(new ArgumentException(SR.ExpectedClosingBrace()));
        }

        public static ArgumentException DuplicateProperty(string argName, QualifiedName qualifiedName) {
            return Failure.Prepare(new ArgumentException(SR.DuplicateProperty(qualifiedName), argName));
        }

        public static PropertyTreeException LateBoundTypeNotFoundError(Exception ex, int lineNumber, int linePosition) {
            return Failure.Prepare(new PropertyTreeException(SR.LateBoundTypeNotFoundError(), ex, lineNumber, linePosition));
        }

        public static PropertyTreeException ProblemAccessingProperty(Exception ex,
                                                                     object component,
                                                                     QualifiedName qualifiedName,
                                                                     int lineNumber,
                                                                     int linePosition) {
            return Failure.Prepare(new PropertyTreeException(SR.ProblemAccessingProperty(qualifiedName, component.GetType()), ex, lineNumber, linePosition));
        }

        public static PropertyTreeException BadSourceDirective(Exception ex, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.BadSourceDirective(), ex, loc));
        }

        public static PropertyTreeException BadTargetTypeDirective(Exception ex, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.BadTargetTypeDirective(), ex, loc));
        }

        public static ArgumentException TemplateTypeConstructorMustBeNiladic(string argumentName, Type componentType) {
            return Failure.Prepare(new ArgumentException(SR.TemplateTypeConstructorMustBeNiladic(componentType), argumentName));
        }

        public static PropertyTreeException BadTargetProviderDirective(Exception ex, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.BadTargetProviderDirective(), ex, loc));
        }

        public static ArgumentException PropertyTreeMetaObjectComponentNull() {
            return Failure.Prepare(new ArgumentException(SR.PropertyTreeMetaObjectComponentNull()));
        }

        public static PropertyTreeException DuplicatePropertyName(IEnumerable<QualifiedName> duplicates, FileLocation loc) {
            string text = string.Join(", ", duplicates);
            return Failure.Prepare(new PropertyTreeException(SR.DuplicatePropertyName(text), null, loc));
        }

        public static PropertyTreeException NoAddMethodSupported(Type type, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.NoAddMethodSupported(type), null, loc));
        }

        public static PropertyTreeException CouldNotBindStreamingSource(Type type, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.CouldNotBindStreamingSource(type), null, loc));
        }

        public static PropertyTreeException CouldNotBindGenericParameters(Type type, Exception ex, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.CouldNotBindGenericParameters(type), ex, loc));
        }

        public static PropertyTreeException NoTargetProviderMatches(Type type, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.NoTargetProviderMatches(type), null, loc));
        }

        public static FormatException ConversionGenericMessage(Exception exception) {
            return Failure.Prepare(new FormatException(SR.ConversionGenericMessage(), exception));
        }

        public static PropertyTreeException BadAddChild(Type parentType, Exception ex, FileLocation loc) {
            return Failure.Prepare(new PropertyTreeException(SR.BadAddChild(parentType), ex, loc));
        }

        public static InvalidOperationException UnableToMatchTypeNameAmbiguous(string name, IEnumerable<object> list) {
            string listText = string.Join(", ", list);
            return Failure.Prepare(new InvalidOperationException(SR.UnableToMatchTypeNameAmbiguous(name, listText)));
        }

        public static InvalidOperationException UnableToMatchTypeNameZero(string name) {
            return Failure.Prepare(new InvalidOperationException(SR.UnableToMatchTypeNameZero(name)));
        }
    }

}
