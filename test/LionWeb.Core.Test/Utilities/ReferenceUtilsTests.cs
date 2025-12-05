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

#pragma warning disable 1591

namespace LionWeb.Core.Test.Utilities;

using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using Languages.Generated.V2024_1.TinyRefLang;

/// <summary>
/// Note: the assertions in this test class use <see cref="CollectionAssert"/>,
/// and rely on value equality of C# record types such as <see cref="ReferenceValue"/>.
/// </summary>
[TestClass]
public class ReferenceUtilsTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    
    [TestMethod]
    public void finds_a_reference_from_a_feature_of_a_concept()
    {
        var language = TestLanguageLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateLinkTestConcept();

        var geometry = (new ExampleModels(_lionWebVersion).ExampleModel(language) as TestPartition)!;
        referenceGeometry.AddReference_0_n(geometry.Links);

        List<INode> scope = [geometry, referenceGeometry];
        var expectedRefs = new List<ReferenceValue>
        {
            new (referenceGeometry, language.LinkTestConcept_reference_0_n, 0, geometry.Links[0])
        };

        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.FindIncomingReferences(geometry.Links[0], scope).ToList());
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.ReferenceValues(scope).ToList());
    }

    [TestMethod]
    [Ignore("Needs TestLanguage to support reference from annotation")]
    public void finds_a_reference_from_an_annotation()
    {
        var language = TestLanguageLanguage.Instance;
        var factory = language.GetFactory();

        var circle = factory.CreateLinkTestConcept();

        var line = (new ExampleModels(_lionWebVersion).ExampleLine(language) as LinkTestConcept)!;
        var bom = factory.CreateTestAnnotation();
        // TODO: Add reference from annotation to circle
        //bom.AddMaterials([circle]);
        line.AddAnnotations([bom]);

        var geometry = factory.CreateTestPartition();
        geometry.AddLinks([circle, line]);

        CollectionAssert.AreEqual(
            new List<ReferenceValue>
            {
                // TODO: Add reference from annotation to circle
                // new (bom, language.BillOfMaterials_materials, 0, circle)
            },
            ReferenceUtils.FindIncomingReferences(circle, [bom]).ToList()
        );
    }

    [TestMethod]
    public void finds_a_reference_to_itself()
    {
        var language = TinyRefLangLanguage.Instance;
        var factory = language.GetFactory();

        var node = factory.CreateMyConcept();
        node.SingularRef = node;

        CollectionAssert.AreEqual(
            new List<ReferenceValue>
            {
                new (node, language.MyConcept_singularRef, null, node)
            },
            ReferenceUtils.FindIncomingReferences(node, [node]).ToList()
        );
    }

    [TestMethod]
    public void finds_references_in_different_features_of_the_source()
    {
        var language = TinyRefLangLanguage.Instance;
        var factory = language.GetFactory();

        var targetNode = factory.CreateMyConcept();
        var sourceNode = factory.CreateMyConcept();
        sourceNode.SingularRef = targetNode;
        sourceNode.AddMultivaluedRef([targetNode]);

        CollectionAssert.AreEquivalent(
            new List<ReferenceValue>
            {
                new (sourceNode, language.MyConcept_singularRef, null, targetNode),
                new (sourceNode, language.MyConcept_multivaluedRef, 0, targetNode),
            },
            ReferenceUtils.FindIncomingReferences(targetNode, [sourceNode]).ToList()
        );
    }

    [TestMethod]
    public void finds_multiple_references_to_target_in_a_multivalued_feature_of_the_source()
    {
        var language = TinyRefLangLanguage.Instance;
        var factory = language.GetFactory();

        var targetNode = factory.CreateMyConcept();
        var sourceNode = factory.CreateMyConcept();
        sourceNode.AddMultivaluedRef([targetNode, targetNode]);

        CollectionAssert.AreEquivalent(
            new List<ReferenceValue>
            {
                new (sourceNode, language.MyConcept_multivaluedRef, 0, targetNode),
                new (sourceNode, language.MyConcept_multivaluedRef, 1, targetNode),
            },
            ReferenceUtils.FindIncomingReferences(targetNode, [sourceNode]).ToList()
        );
    }

    [TestMethod]
    public void finds_references_among_multiple_sources_and_targets()
    {
        var language = TinyRefLangLanguage.Instance;
        var factory = language.GetFactory();

        var sourceNode1 = factory.CreateMyConcept();
        var sourceNode2 = factory.CreateMyConcept();
        var targetNode1 = factory.CreateMyConcept();
        var targetNode2 = factory.CreateMyConcept();
        sourceNode1.SingularRef = targetNode1;
        sourceNode2.SingularRef = targetNode2;

        CollectionAssert.AreEquivalent(
            new List<ReferenceValue>
            {
                new (sourceNode1, language.MyConcept_singularRef, null, targetNode1),
                new (sourceNode2, language.MyConcept_singularRef, null, targetNode2),
            },
            ReferenceUtils.FindIncomingReferences([targetNode1, targetNode2], [sourceNode1, sourceNode2]).ToList()
        );
    }

    [TestMethod]
    public void has_defined_behavior_for_duplicate_target_nodes()
    {
        var language = TestLanguageLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateLinkTestConcept();

        var geometry = (new ExampleModels(_lionWebVersion).ExampleModel(language) as TestPartition)!;
        referenceGeometry.AddReference_0_n(geometry.Links);

        List<INode> scope = [geometry, referenceGeometry];
        var expectedRefs = new List<ReferenceValue>
        {
            new (referenceGeometry, language.LinkTestConcept_reference_0_n, 0, geometry.Links[0])
        };

        var targetNode = geometry.Links[0];
        IEnumerable<IReadableNode> duplicateTargetNodes = [targetNode, targetNode];
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.FindIncomingReferences(duplicateTargetNodes, scope).ToList());
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.ReferenceValues(scope).ToList());
    }

    [TestMethod]
    public void has_defined_behavior_when_duplicate_nodes_in_scope()
    {
        var language = TestLanguageLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateLinkTestConcept();

        var geometry = (new ExampleModels(_lionWebVersion).ExampleModel(language) as TestPartition)!;
        referenceGeometry.AddReference_0_n(geometry.Links);

        List<INode> scope = [geometry, referenceGeometry];
        var expectedRefs = new List<ReferenceValue>
        {
            new (referenceGeometry, language.LinkTestConcept_reference_0_n, 0, geometry.Links[0])
        };

        var duplicateScope = scope.Concat(scope);
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.FindIncomingReferences(geometry.Links[0], duplicateScope).ToList());
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.ReferenceValues(scope).ToList());
    }

    [TestMethod]
    public void finds_unreachable_nodes()
    {
        var language = TestLanguageLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateLinkTestConcept();

        var geometry = (new ExampleModels(_lionWebVersion).ExampleModel(language) as TestPartition)!;
        referenceGeometry.AddReference_0_n(geometry.Links);

        Assert.AreEqual(1, referenceGeometry.Reference_0_n.Count);
        var refValues = ReferenceUtils.ReferencesToOutOfScopeNodes([referenceGeometry]);
        var expectedRefValues = new List<ReferenceValue>
        {
            new (referenceGeometry, language.LinkTestConcept_reference_0_n, 0, geometry.Links[0])
        };
        CollectionAssert.AreEqual(expectedRefValues, refValues.ToList());
    }
}