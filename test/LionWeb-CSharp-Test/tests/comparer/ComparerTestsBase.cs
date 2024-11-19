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

// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes

namespace LionWeb.Utils.Tests.Comparer;

using Core;
using Core.M2;
using Core.M3;
using Core.Utilities;
using Examples.Shapes.M2;

/// <summary>
/// We want to get all left-side nodes from a different language and factory than all the right-side nodes.
/// This way we assure all the M1 and M2 C# objects have different C# identity,
/// and all our comparison only relies on LionWeb reflection and identity (i.e. keys). 
/// </summary>
public abstract class ComparerTestsBase
{
    private static readonly Language leftLanguage = ShapesLanguage.Instance;

    protected static readonly IBuiltInsLanguage _builtIns = LionWebVersionsExtensions.GetCurrent().GetBuiltIns();

    protected static readonly ShapesFactory lF = leftLanguage.GetFactory() as ShapesFactory;

    protected readonly Property leftCoordX =
        leftLanguage.ClassifierByKey("key-Coord").FeatureByKey("key-x") as Property;

    protected readonly Property leftMaterialGroupMatterState =
        leftLanguage.ClassifierByKey("key-MaterialGroup").FeatureByKey("key-matter-state") as Property;

    protected readonly Property leftDocumentationTechnical =
        leftLanguage.ClassifierByKey("key-Documentation").FeatureByKey("key-technical") as Property;

    protected readonly Property leftDocumentationText =
        leftLanguage.ClassifierByKey("key-Documentation").FeatureByKey("key-text") as Property;

    protected readonly Containment leftLineStart =
        leftLanguage.ClassifierByKey("key-Line").FeatureByKey("key-start") as Containment;

    private readonly Containment leftLineEnd =
        leftLanguage.ClassifierByKey("key-Line").FeatureByKey("key-end") as Containment;

    protected readonly Containment leftCircleCoord =
        leftLanguage.ClassifierByKey("key-Circle").FeatureByKey("key-center") as Containment;

    protected readonly Containment leftGeometryShapes =
        leftLanguage.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes") as Containment;

    protected readonly Containment leftGeometryDocumentation =
        leftLanguage.ClassifierByKey("key-Geometry").FeatureByKey("key-documentation") as Containment;

    protected readonly Reference leftOffsetDuplicateSource =
        leftLanguage.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source") as Reference;

    protected readonly Reference leftOffsetDuplicateAltSource =
        leftLanguage.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-alt-source") as Reference;

    protected readonly Reference leftBillOfMaterialsMaterials =
        leftLanguage.ClassifierByKey("key-BillOfMaterials").FeatureByKey("key-materials") as Reference;

    protected static readonly Language rightLanguage =
        LanguagesUtils
            .LoadLanguages("LionWeb-CSharp-Test", "LionWeb_CSharp_Test.languages.defChunks.shapes.json")
            .First();

    protected static readonly AbstractBaseNodeFactory rF = rightLanguage.GetFactory() as AbstractBaseNodeFactory;

    protected readonly Property rightCoordX =
        rightLanguage.ClassifierByKey("key-Coord").FeatureByKey("key-x") as Property;

    protected readonly Property rightCoordY =
        rightLanguage.ClassifierByKey("key-Coord").FeatureByKey("key-y") as Property;

    protected readonly Containment rightLineEnd =
        rightLanguage.ClassifierByKey("key-Line").FeatureByKey("key-end") as Containment;

    protected readonly Property rightDocumentationText =
        rightLanguage.ClassifierByKey("key-Documentation").FeatureByKey("key-text") as Property;

    protected readonly Property rightDocumentationTechnical =
        rightLanguage.ClassifierByKey("key-Documentation").FeatureByKey("key-technical") as Property;

    protected readonly Property rightMaterialGroupMatterState =
        rightLanguage.ClassifierByKey("key-MaterialGroup").FeatureByKey("key-matter-state") as Property;

    protected readonly Containment rightCircleCoord =
        rightLanguage.ClassifierByKey("key-Circle").FeatureByKey("key-center") as Containment;

    protected readonly Containment rightGeometryShapes =
        rightLanguage.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes") as Containment;

    protected readonly Containment rightGeometryDocumentation =
        rightLanguage.ClassifierByKey("key-Geometry").FeatureByKey("key-documentation") as Containment;

    protected readonly Reference rightOffsetDuplicateSource =
        rightLanguage.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source") as Reference;

    protected readonly Reference rightOffsetDuplicateAltSource =
        rightLanguage.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-alt-source") as Reference;

    protected readonly Reference rightBillOfMaterialsMaterials =
        rightLanguage.ClassifierByKey("key-BillOfMaterials").FeatureByKey("key-materials") as Reference;

    protected static void AreEqual(INode? left, INode? right)
    {
        Comparer comparer = Comparer(left, right);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(OutputConfig));
    }

    protected static void AreEqual(List<INode?> left, List<INode?> right)
    {
        Comparer comparer = Comparer(left, right);
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(OutputConfig));
    }

    protected static void AreDifferent(INode? left, INode? right, params IDifference[] differences)
    {
        Comparer comparer = Comparer(left, right);
        var actual = comparer.Compare().Distinct().ToList();
        var expected = differences.ToList();

        CollectionAssert.AreEqual(expected, actual, comparer.ToMessage(OutputConfig));
    }

    protected static void AreDifferent(List<INode?> left, List<INode?> right, params IDifference[] differences)
    {
        Comparer comparer = Comparer(left, right);
        var actual = comparer.Compare().Distinct().ToList();
        var expected = differences.ToList();

        CollectionAssert.AreEqual(expected, actual, actual.DescribeAll(OutputConfig));
    }

    private static Comparer Comparer(INode? left, INode? right) =>
        Comparer([left], [right]);

    private static Comparer Comparer(List<INode?> left, List<INode?> right) =>
        new(left, right);

    protected static ComparerOutputConfig OutputConfig = new()
    {
        FullClassifier = true, FullFeature = true, LanguageId = true
    };
}

public static class ReflectiveShapesFactory
{
    public static INode NewLine(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-Line"));

    public static INode NewCircle(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-Circle"));

    public static INode NewDocumentation(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-Documentation"));

    public static INode NewBillOfMaterials(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-BillOfMaterials"));

    public static INode NewCoord(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-Coord"));

    public static INode NewGeometry(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-Geometry"));

    public static INode NewOffsetDuplicate(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-OffsetDuplicate"));

    public static INode NewMaterialGroup(this AbstractBaseNodeFactory factory, string s) =>
        factory.CreateNode(s, factory.Language.ClassifierByKey("key-MaterialGroup"));

    public static Enum GetMatterState(this AbstractBaseNodeFactory factory, string name) =>
        factory.GetEnumerationLiteral(factory.Language.Enumerations().First().Literals.First(l => l.Name == name));
}

static class ShapeNodeExtension
{
    internal static void SetName(this INode node, string name) =>
        node.Set(LionWebVersionsExtensions.GetCurrent().GetBuiltIns().INamed_name, name);

    internal static void SetX(this INode node, int x) =>
        node.Set(node.GetClassifier().FeatureByKey("key-x"), x);

    internal static void SetY(this INode node, int y) =>
        node.Set(node.GetClassifier().FeatureByKey("key-y"), y);

    internal static void SetTechnical(this INode node, bool technical) =>
        node.Set(node.GetClassifier().FeatureByKey("key-technical"), technical);

    internal static void SetText(this INode node, string text) =>
        node.Set(node.GetClassifier().FeatureByKey("key-text"), text);

    internal static void SetMatterState(this INode node, Enum matterState) =>
        node.Set(node.GetClassifier().FeatureByKey("key-matter-state"), matterState);

    internal static void SetCenter(this INode node, INode center) =>
        node.Set(node.GetClassifier().FeatureByKey("key-center"), center);

    internal static void SetDocumentation(this INode node, INode? doc) =>
        node.Set(node.GetClassifier().FeatureByKey("key-documentation"), doc);

    internal static void SetEnd(this INode node, INode end) =>
        node.Set(node.GetClassifier().FeatureByKey("key-end"), end);

    internal static void AddShapes(this INode node, params INode[] shapes) =>
        node.Set(node.GetClassifier().FeatureByKey("key-shapes"), shapes);

    internal static void SetShapes(this INode node, INode[] shapes) =>
        node.Set(node.GetClassifier().FeatureByKey("key-shapes"), shapes);

    internal static void SetSource(this INode node, INode source) =>
        node.Set(node.GetClassifier().FeatureByKey("key-source"), source);

    internal static void SetAltSource(this INode node, INode? altSource) =>
        node.Set(node.GetClassifier().FeatureByKey("key-alt-source"), altSource);

    internal static void AddMaterials(this INode node, params INode[] materials) =>
        node.Set(node.GetClassifier().FeatureByKey("key-materials"), materials);

    internal static void SetMaterials(this INode node, INode[] materials) =>
        node.Set(node.GetClassifier().FeatureByKey("key-materials"), materials);
}