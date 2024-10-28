// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb_CSharp_Test.tests.serialization.deserialization;

using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M1;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class DeserializationWithCustomHandlerTests
{
    private abstract class NotImplementedDeserializerHandler : IDeserializerHandler
    {
        public virtual Classifier? UnknownClassifier(string id, MetaPointer metaPointer) =>
            throw new NotImplementedException();

        public virtual TFeature? UnknownFeature<TFeature>(Classifier classifier, CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class, Feature =>
            throw new NotImplementedException();

        public virtual INode? UnknownParent(CompressedId parentId, INode node) => throw new NotImplementedException();

        public virtual INode? UnknownChild(CompressedId childId, IWritableNode node) =>
            throw new NotImplementedException();

        public virtual IReadableNode?
            UnknownReferenceTarget(CompressedId targetId, string? resolveInfo, IWritableNode node) =>
            throw new NotImplementedException();

        public virtual INode? UnknownAnnotation(CompressedId annotationId, INode node) =>
            throw new NotImplementedException();

        public virtual INode? InvalidAnnotation(IReadableNode annotation, IWritableNode node) =>
            throw new NotImplementedException();

        public virtual Enum? UnknownEnumerationLiteral(string nodeId, Enumeration enumeration, string key) =>
            throw new NotImplementedException();

        public object? UnknownDatatype(string nodeId, Feature property, string? value) =>
            throw new NotImplementedException();

        public bool SkipDeserializingDependentNode(string id) => throw new NotImplementedException();

        public virtual TFeature? InvalidFeature<TFeature>(Classifier classifier,
            CompressedMetaPointer compressedMetaPointer,
            IReadableNode node) where TFeature : class, Feature =>
            throw new NotImplementedException();

        public virtual void InvalidContainment(IReadableNode node) => throw new NotImplementedException();

        public virtual void InvalidReference(IReadableNode node) => throw new NotImplementedException();

        public virtual IWritableNode? InvalidAnnotationParent(IReadableNode annotation, string parentId) =>
            throw new NotImplementedException();
    }

    #region unknown classifier

    private class UnknownClassifierHandler(Func<Classifier?> incrementer) : NotImplementedDeserializerHandler
    {
        public override Classifier? UnknownClassifier(string id, MetaPointer metaPointer) => incrementer();
    }

    [TestMethod]
    public void unknown_classifier()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = [new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "foo",
                    Classifier = new MetaPointer("key-Shapes", "1", "key-unknown"),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                }
            ]
        };

        var count = 0;

        IDeserializer deserializer = new DeserializerBuilder()
            .WithHandler(new UnknownClassifierHandler(() =>
            {
                Interlocked.Increment(ref count);
                return null;
            }))
            .WithLanguage(ShapesLanguage.Instance)
            .Build();

        try
        {
            deserializer.Deserialize(serializationChunk);
        } catch (InvalidOperationException _) { }

        Assert.AreEqual(1, count);
    }

    #endregion
}