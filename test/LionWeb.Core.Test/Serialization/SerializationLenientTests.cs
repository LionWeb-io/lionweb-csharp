// Copyright 2024 TRUMPF Laser SE and other contributors
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
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.Circular.B;
using Languages.Generated.V2024_1.Library.M2;
using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2024_1.WithEnum.M2;
using M1;
using M2;
using M3;
using System.Collections;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class SerializationLenientTests
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private static readonly IBuiltInsLanguage _builtIns = _lionWebVersion.BuiltIns;

    [TestMethod]
    public void InvalidValues()
    {
        var rootNode = new LenientNode("a", ShapesLanguage.Instance.Line);

        var childA = new Book("bookA");
        var childB = new BConcept("bConceptB");
        rootNode.Set(_builtIns.INamed_name, new List<INode> { childA, childB });
        rootNode.Set(ShapesLanguage.Instance.IShape_uuid, new List<IReadableNode> { childA, childB });
        rootNode.Set(ShapesLanguage.Instance.Shape_shapeDocs, "hello");
        rootNode.Set(ShapesLanguage.Instance.Line_start, new List<INode> { childA, childB });
        rootNode.Set(ShapesLanguage.Instance.Line_end, MyEnum.literal1);

        IEnumerable<IReadableNode> nodes = new List<INode> { rootNode, childA, childB };
        var serializationChunk =
            new SerializerBuilder().WithLionWebVersion(_lionWebVersion).Build().SerializeToChunk(nodes);
        Console.WriteLine(JsonUtils.WriteJsonToString(serializationChunk));

        var readableNodes = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguage(ShapesLanguage.Instance)
            .WithCustomFactory(ShapesLanguage.Instance, new LenientFactory(ShapesLanguage.Instance))
            .WithLanguage(LibraryLanguage.Instance)
            .WithLanguage(BLangLanguage.Instance)
            .WithLanguage(WithEnumLanguage.Instance)
            .WithHandler(new LenientHandler())
            .WithCompressedIds(new(true, true))
            .Build()
            .Deserialize(serializationChunk);

        Assert.AreEqual(1, readableNodes.Count);

        var comparer = new LenientComparer(new List<IReadableNode> { rootNode }, readableNodes);
        var differences = comparer.Compare().ToList();
        TestContext.WriteLine(differences.DescribeAll(new ComparerOutputConfig()));

        Assert.AreEqual(2, differences.Count);
        Assert.IsInstanceOfType<NodeDifference>(differences.First());
        Assert.IsInstanceOfType<PropertyValueTypeDifference>(differences.Last());
        var diff = (differences.Last() as PropertyValueTypeDifference);
        Assert.AreEqual(MyEnum.literal1, diff.LeftValue);
        Assert.AreEqual("lit1", diff.RightValue);
    }

    public class LenientFactory(Language language) : AbstractBaseNodeFactory(language)
    {
        public override INode CreateNode(string id, Classifier classifier) =>
            new LenientNode(id, classifier);

        public override Enum GetEnumerationLiteral(EnumerationLiteral literal) =>
            EnumValueFor<Enum>(literal);

        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(
            StructuredDataType structuredDataType,
            IFieldValues fieldValues) =>
            StructuredDataTypeInstanceFor(structuredDataType.GetType(), fieldValues);
    }

    class LenientHandler : DeserializerExceptionHandler
    {
        public override Feature? InvalidFeature<TFeature>(CompressedMetaPointer feature, Classifier classifier,
            IReadableNode node) where TFeature : class
        {
            var replacementFeature = new List<Language>
                {
                    _builtIns,
                    ShapesLanguage.Instance,
                    LibraryLanguage.Instance,
                    BLangLanguage.Instance,
                    WithEnumLanguage.Instance
                }
                .SelectMany(l => l.Entities)
                .OfType<Classifier>()
                .SelectMany(c => c.Features)
                .FirstOrDefault(f =>
                    CompressedMetaPointer.Create(f.ToMetaPointer(), new CompressedIdConfig(true, false))
                        .Equals(feature));

            return replacementFeature;
        }

        public override object? UnknownDatatype(string? value, LanguageEntity datatype, Feature property,
            IReadableNode nodeId) => value;
    }

    class LenientComparer : Comparer
    {
        public LenientComparer(IList<INode?> left, IList<INode?> right) : base(left, right)
        {
        }

        public LenientComparer(IList<IReadableNode?> _left, IList<IReadableNode?> _right) : base(_left, _right)
        {
        }

        class LenientProperty : PropertyBase<Language>
        {
            public LenientProperty(Feature feature) : base(feature.GetId(), feature.GetFeatureClassifier(),
                feature.GetLanguage())
            {
            }

            public static LenientProperty Create(Feature feature, object? value) =>
                new(feature)
                {
                    Name = feature.Name,
                    Key = feature.Key,
                    Optional = feature.Optional,
                    Type = value is Enum e
                        ? new LenientEnumeration("a", feature.GetLanguage())
                        {
                            Key = "a", Name = "a", LiteralsLazy = new([])
                        }
                        : feature.GetLanguage().LionWebVersion.BuiltIns.String
                };
        }

        class LenientEnumeration(string id, Language parent) : EnumerationBase<Language>(id, parent);

        class LenientContainment : ContainmentBase<Language>
        {
            public LenientContainment(Feature feature) : base(feature.GetId(), feature.GetFeatureClassifier(),
                feature.GetLanguage())
            {
            }

            public static LenientContainment Create(Feature feature) =>
                new(feature)
                {
                    Name = feature.Name,
                    Key = feature.Key,
                    Optional = feature.Optional,
                    Multiple = false,
                    Type = feature.GetLanguage().LionWebVersion.BuiltIns.Node
                };
        }

        class LenientReference : ReferenceBase<Language>
        {
            public LenientReference(Feature feature) : base(feature.GetId(), feature.GetFeatureClassifier(),
                feature.GetLanguage())
            {
            }

            public static LenientReference Create(Feature feature) =>
                new(feature)
                {
                    Name = feature.Name,
                    Key = feature.Key,
                    Optional = feature.Optional,
                    Multiple = false,
                    Type = feature.GetLanguage().LionWebVersion.BuiltIns.Node
                };
        }

        protected override List<IDifference> CompareFeature(IReadableNode left, Feature leftFeature,
            IReadableNode right, Feature rightFeature)
        {
            var leftValue = left.Get(leftFeature);
            var rightValue = right.Get(rightFeature);

            Containment leftCont = leftFeature as Containment ?? LenientContainment.Create(leftFeature);
            Reference leftRef = leftFeature as Reference ?? LenientReference.Create(leftFeature);

            Property leftProp = leftFeature as Property ?? LenientProperty.Create(leftFeature, leftValue);
            Property rightProp = rightFeature as Property ?? LenientProperty.Create(rightFeature, rightValue);

            switch ((leftValue, rightValue, leftFeature, rightFeature))
            {
                case (null, null, _, _):
                    return [];

                case (null, _, _, _):
                    return [new UnsetFeatureLeftDifference(left, leftFeature, right)];

                case (_, null, _, _):
                    return [new UnsetFeatureRightDifference(left, rightFeature, right)];

                case (string or int or bool or Enum, string or int or bool or Enum, _, _):
                    return CompareProperty(left, leftProp, right, rightProp);

                case (var lv, var rv, _, _) when lv.GetType().IsValueType && rv.GetType().IsValueType:
                    return CompareProperty(left, leftProp, right, rightProp);

                case (IReadableNode ln, IReadableNode rn, _, _) when ReferenceEquals(left, ln.GetParent()) &&
                                                                     ReferenceEquals(right, ln.GetParent()):
                    return CompareNode(left, ln,
                        leftCont, right, rn);

                case (IReadableNode ln, IReadableNode rn, _, _):
                    return CompareTarget(left, ln,
                        leftRef, right, rn);

                case (IEnumerable le, IEnumerable lr, _, _):
                    {
                        var leftNodes = le.Cast<IReadableNode>().ToList();
                        var rightNodes = lr.Cast<IReadableNode>().ToList();

                        if (leftNodes.All(n => ReferenceEquals(left, n.GetParent())) &&
                            rightNodes.All(n => ReferenceEquals(right, n.GetParent())))
                            return CompareNodes(left, leftNodes, leftCont, right, rightNodes);

                        return CompareTargets(left, leftNodes, leftRef, right, rightNodes);
                    }

                default:
                    return [];
            }
        }
    }

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}