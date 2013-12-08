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
using System.Globalization;
using Carbonfrost.Commons.ComponentModel;
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

        public static PropertyTreeException BinderMissingProperty(int lineNumber, int linePosition, InterfaceUsageInfo usageInfo) {
            return Failure.Prepare(new PropertyTreeException(usageInfo.Message, lineNumber, linePosition) {
                                       HelpLink = Convert.ToString(usageInfo.HelpUri),
                                   });
        }

        public static PropertyTreeException BinderObsoleteProperty(int lineNumber, int linePosition, InterfaceUsageInfo usageInfo) {
            return Failure.Prepare(new PropertyTreeException(usageInfo.Message, lineNumber, linePosition) {
                                       HelpLink = Convert.ToString(usageInfo.HelpUri),
                                   });
        }

        public static PropertyTreeException BinderConversionError(string text, string propertyName, int lineNumber, int linePosition, Exception exception) {
            return Failure.Prepare(new PropertyTreeException(SR.BinderConversionError(propertyName), exception, lineNumber, linePosition));
        }

        public static PropertyTreeException LateBoundTypeMissing(int lineNumber, int linePosition) {
            return Failure.Prepare(new PropertyTreeException(SR.LateBoundTypeMissing(), lineNumber, linePosition));
        }

        public static PropertyTreeException RequiredPropertiesMissing(string[] names, string hintSignature, int lineNumber, int linePosition) {
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
    }

}
