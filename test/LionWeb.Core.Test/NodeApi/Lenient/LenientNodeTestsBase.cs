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

namespace LionWeb.Core.Test.NodeApi.Lenient;

using Languages;
using M2;
using M3;

public abstract class LenientNodeTestsBase
{
    protected DynamicLanguage _lang;

    [TestInitialize]
    public void LoadLanguage()
    {
        _lang = ShapesDynamic.Language;
    }

    protected LenientNode newReferenceGeometry(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-ReferenceGeometry"));

    protected LenientNode newLine(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-Line"));

    protected LenientNode newCoord(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-Coord"));

    protected LenientNode newBillOfMaterials(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-BillOfMaterials"));

    protected LenientNode newDocumentation(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-Documentation"));

    protected LenientNode newGeometry(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-Geometry"));

    protected LenientNode newCircle(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-Circle"));

    protected LenientNode newCompositeShape(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-CompositeShape"));

    protected LenientNode newOffsetDuplicate(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-OffsetDuplicate"));

    protected LenientNode newMaterialGroup(string id) =>
        new LenientNode(id, _lang.ClassifierByKey("key-MaterialGroup"));

    protected Feature ReferenceGeometry_shapes
    {
        get => _lang.ClassifierByKey("key-ReferenceGeometry").FeatureByKey("key-shapes-references");
    }

    protected Feature Geometry_shapes
    {
        get => _lang.ClassifierByKey("key-Geometry").FeatureByKey("key-shapes");
    }

    protected Feature CompositeShape_parts
    {
        get => _lang.ClassifierByKey("key-CompositeShape").FeatureByKey("key-parts");
    }

    protected Feature CompositeShape_disabledParts
    {
        get => _lang.ClassifierByKey("key-CompositeShape").FeatureByKey("key-disabled-parts");
    }

    protected Feature CompositeShape_evilPart
    {
        get => _lang.ClassifierByKey("key-CompositeShape").FeatureByKey("key-evil-part");
    }

    protected Feature Geometry_documentation
    {
        get => _lang.ClassifierByKey("key-Geometry").FeatureByKey("key-documentation");
    }

    protected Feature OffsetDuplicate_offset
    {
        get => _lang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-offset");
    }

    protected Feature Documentation_technical
    {
        get => _lang.ClassifierByKey("key-Documentation").FeatureByKey("key-technical");
    }

    protected Feature MaterialGroup_matterState
    {
        get => _lang.ClassifierByKey("key-MaterialGroup").FeatureByKey("key-matter-state");
    }

    protected Feature MaterialGroup_defaultShape
    {
        get => _lang.ClassifierByKey("key-MaterialGroup").FeatureByKey("key-default-shape");
    }

    protected Feature Circle_r
    {
        get => _lang.ClassifierByKey("key-Circle").FeatureByKey("key-r");
    }

    protected Feature Circle_center
    {
        get => _lang.ClassifierByKey("key-Circle").FeatureByKey("key-center");
    }

    protected Feature Line_start
    {
        get => _lang.ClassifierByKey("key-Line").FeatureByKey("key-start");
    }

    protected Feature Line_end
    {
        get => _lang.ClassifierByKey("key-Line").FeatureByKey("key-end");
    }

    protected Feature Documentation_text
    {
        get => _lang.ClassifierByKey("key-Documentation").FeatureByKey("key-text");
    }

    protected Feature MaterialGroup_materials
    {
        get => _lang.ClassifierByKey("key-MaterialGroup").FeatureByKey("key-group-materials");
    }

    protected Feature OffsetDuplicate_altSource
    {
        get => _lang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-alt-source");
    }

    protected Feature OffsetDuplicate_source
    {
        get => _lang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source");
    }

    protected Feature OffsetDuplicate_docs
    {
        get => _lang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-docs");
    }

    protected Feature OffsetDuplicate_secretDocs
    {
        get => _lang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-secret-docs");
    }

    protected Feature BillOfMaterials_groups
    {
        get => _lang.ClassifierByKey("key-BillOfMaterials").FeatureByKey("key-groups");
    }

    protected Feature BillOfMaterials_altGroups
    {
        get => _lang.ClassifierByKey("key-BillOfMaterials").FeatureByKey("key-alt-groups");
    }

    protected Feature BillOfMaterials_defaultGroup
    {
        get => _lang.ClassifierByKey("key-BillOfMaterials").FeatureByKey("key-default-group");
    }

    protected Feature Shape_shapeDocs
    {
        get => _lang.ClassifierByKey("key-Shape").FeatureByKey("key-shape-docs");
    }

    protected Enum MatterState_Gas
    {
        get => _lang.GetFactory().GetEnumerationLiteral(
            MatterState
                .Literals
                .First(l => l.Key == "key-gas")
        );
    }

    protected Enum MatterState_Liquid
    {
        get => _lang.GetFactory().GetEnumerationLiteral(
            MatterState
                .Literals
                .First(l => l.Key == "key-liquid")
        );
    }

    private Enumeration MatterState =>
        _lang
            .Enumerations()
            .First(e => e.Key == "key-MatterState");
}