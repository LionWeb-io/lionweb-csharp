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

// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591

namespace LionWeb_CSharp_Test.tests;

using Examples.Shapes.Dynamic;
using Examples.Shapes.M2;
using LionWeb.Core;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;

/// <summary>
/// Note: the assertions in this test class use <see cref="CollectionAssert"/>,
/// and rely on value equality of C# record types such as <see cref="ReferenceValue"/>.
/// </summary>
[TestClass]
public class ReferenceUtilsTests
{
    [TestMethod]
    public void finds_a_reference_from_a_feature_of_a_concept()
    {
        var language = ShapesLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateReferenceGeometry();

        var geometry = (ExampleModels.ExampleModel(language) as Geometry)!;
        referenceGeometry.AddShapes(geometry.Shapes);

        List<INode> scope = [geometry, referenceGeometry];
        var expectedRefs = new List<ReferenceValue>
        {
            new (referenceGeometry, language.ReferenceGeometry_shapes, 0, geometry.Shapes[0])
        };

        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.FindIncomingReferences(geometry.Shapes[0], scope).ToList());
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.ReferenceValues(scope).ToList());
    }

    [TestMethod]
    public void finds_a_reference_from_an_annotation()
    {
        var language = ShapesLanguage.Instance;
        var factory = language.GetFactory();

        var circle = factory.CreateCircle();

        var line = (ExampleModels.ExampleLine(language) as Line)!;
        var bom = factory.CreateBillOfMaterials();
        bom.AddMaterials([circle]);
        line.AddAnnotations([bom]);

        var geometry = factory.CreateGeometry();
        geometry.AddShapes([circle, line]);

        CollectionAssert.AreEqual(
            new List<ReferenceValue>
            {
                new (bom, language.BillOfMaterials_materials, 0, circle)
            },
            ReferenceUtils.FindIncomingReferences(circle, [bom]).ToList()
        );
    }

    [TestMethod]
    public void finds_a_reference_to_itself()
    {
        var language = new DynamicLanguage("lang");
        var concept = new DynamicConcept("concept", language);
        language.AddEntities([concept]);

        var selfRef = new DynamicReference("selfRef", concept);
        concept.AddFeatures([selfRef]);
        selfRef.Type = concept;

        var node = new DynamicNode("node", concept);
        node.Set(selfRef, node);

        CollectionAssert.AreEqual(
            new List<ReferenceValue>
            {
                new (node, selfRef, null, node)
            },
            ReferenceUtils.FindIncomingReferences(node, [node]).ToList()
        );
    }

    [TestMethod]
    public void finds_references_in_different_features_of_the_source()
    {
        var language = new DynamicLanguage("lang");
        var concept = new DynamicConcept("concept", language);
        language.AddEntities([concept]);

        var ref1 = new DynamicReference("ref1", concept);
        concept.AddFeatures([ref1]);
        ref1.Type = concept;

        var ref2 = new DynamicReference("ref2", concept);
        concept.AddFeatures([ref2]);
        ref2.Type = concept;

        var targetNode = new DynamicNode("targetNode", concept);
        var sourceNode = new DynamicNode("sourceNode", concept);
        sourceNode.Set(ref1, targetNode);
        sourceNode.Set(ref2, targetNode);

        CollectionAssert.AreEquivalent(
            new List<ReferenceValue>
            {
                new (sourceNode, ref1, null, targetNode),
                new (sourceNode, ref2, null, targetNode),
            },
            ReferenceUtils.FindIncomingReferences(targetNode, [sourceNode]).ToList()
        );
    }

    [TestMethod]
    public void finds_multiple_references_to_target_in_a_multivalued_feature_of_the_source()
    {
        var language = new DynamicLanguage("lang");
        var concept = new DynamicConcept("concept", language);
        language.AddEntities([concept]);

        var myRef = new DynamicReference("myRef", concept);
        concept.AddFeatures([myRef]);
        myRef.Type = concept;
        myRef.Multiple = true;

        var targetNode = new DynamicNode("targetNode", concept);
        var sourceNode = new DynamicNode("sourceNode", concept);
        sourceNode.Set(myRef, new List<INode> { targetNode, targetNode });

        CollectionAssert.AreEquivalent(
            new List<ReferenceValue>
            {
                new (sourceNode, myRef, 0, targetNode),
                new (sourceNode, myRef, 1, targetNode),
            },
            ReferenceUtils.FindIncomingReferences(targetNode, [sourceNode]).ToList()
        );
    }

    [TestMethod]
    public void finds_references_among_multiple_sources_and_targets()
    {
        var language = new DynamicLanguage("lang");
        var concept = new DynamicConcept("concept", language);
        language.AddEntities([concept]);

        var myRef = new DynamicReference("myRef", concept);
        concept.AddFeatures([myRef]);
        myRef.Type = concept;

        var sourceNode1 = new DynamicNode("sourceNode1", concept);
        var sourceNode2 = new DynamicNode("sourceNode2", concept);
        var targetNode1 = new DynamicNode("targetNode1", concept);
        var targetNode2 = new DynamicNode("targetNode2", concept);
        sourceNode1.Set(myRef, targetNode1);
        sourceNode2.Set(myRef, targetNode2);

        CollectionAssert.AreEquivalent(
            new List<ReferenceValue>
            {
                new (sourceNode1, myRef, null, targetNode1),
                new (sourceNode2, myRef, null, targetNode2),
            },
            ReferenceUtils.FindIncomingReferences([targetNode1, targetNode2], [sourceNode1, sourceNode2]).ToList()
        );
    }

    [TestMethod]
    public void has_defined_behavior_for_duplicate_target_nodes()
    {
        var language = ShapesLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateReferenceGeometry();

        var geometry = (ExampleModels.ExampleModel(language) as Geometry)!;
        referenceGeometry.AddShapes(geometry.Shapes);

        List<INode> scope = [geometry, referenceGeometry];
        var expectedRefs = new List<ReferenceValue>
        {
            new (referenceGeometry, language.ReferenceGeometry_shapes, 0, geometry.Shapes[0])
        };

        var targetNode = geometry.Shapes[0];
        IEnumerable<IReadableNode> duplicateTargetNodes = [targetNode, targetNode];
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.FindIncomingReferences(duplicateTargetNodes, scope).ToList());
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.ReferenceValues(scope).ToList());
    }

    [TestMethod]
    public void has_defined_behavior_when_duplicate_nodes_in_scope()
    {
        var language = ShapesLanguage.Instance;
        var factory = language.GetFactory();
        var referenceGeometry = factory.CreateReferenceGeometry();

        var geometry = (ExampleModels.ExampleModel(language) as Geometry)!;
        referenceGeometry.AddShapes(geometry.Shapes);

        List<INode> scope = [geometry, referenceGeometry];
        var expectedRefs = new List<ReferenceValue>
        {
            new (referenceGeometry, language.ReferenceGeometry_shapes, 0, geometry.Shapes[0])
        };

        var duplicateScope = scope.Concat(scope);
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.FindIncomingReferences(geometry.Shapes[0], duplicateScope).ToList());
        CollectionAssert.AreEqual(expectedRefs, ReferenceUtils.ReferenceValues(scope).ToList());
    }
}