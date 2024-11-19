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
// ReSharper disable InconsistentNaming

namespace Examples.Shapes.Dynamic;

using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;

/// <summary>
/// Definition of the Shapes language by using the LionCore/M3 implementation in the LionWeb-CSharp NuGet package.
/// </summary>
public static class ShapesDefinition
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;

    /// <summary>
    /// Definition of the Shapes language.
    /// </summary>
    public static readonly DynamicLanguage Language = new("id-Shapes", _lionWebVersion)
    {
        Key = "key-Shapes", Name = "Shapes", Version = "1"
    };

    static ShapesDefinition()
    {
        var builtIns = _lionWebVersion.BuiltIns;

        var Circle = Language.Concept("id-Circle", "key-Circle", "Circle");
        var Coord = Language.Concept("id-Coord", "key-Coord", "Coord");
        var Geometry = Language.Concept("id-Geometry", "key-Geometry", "Geometry");
        var IShape = Language.Interface("id-IShape", "key-IShape", "IShape");
        var Line = Language.Concept("id-Line", "key-Line", "Line");
        var OffsetDuplicate = Language.Concept("id-OffsetDuplicate", "key-OffsetDuplicate", "OffsetDuplicate");
        var Shape = Language.Concept("id-Shape", "key-Shape", "Shape");
        var CompositeShape = Language.Concept("id-CompositeShape", "key-CompositeShape", "CompositeShape");
        var ReferenceGeometry = Language.Concept("id-ReferenceGeometry", "key-ReferenceGeometry", "ReferenceGeometry");
        var Documentation = Language.Annotation("id-Documentation", "key-Documentation", "Documentation");
        var BillOfMaterials = Language.Annotation("id-BillOfMaterials", "key-BillOfMaterials", "BillOfMaterials");
        var MaterialGroup = Language.Concept("id-MaterialGroup", "key-MaterialGroup", "MaterialGroup");
        var MatterState = Language.Enumeration("id-MatterState", "key-MatterState", "MatterState");
        var Time = Language.PrimitiveType("id-Time", "key-Time", "Time");

        Circle.Extending(Shape);
        Circle.Property("id-r", "key-r", "r").OfType(builtIns.Integer);
        Circle.Containment("id-center", "key-center", "center").OfType(Coord);

        Coord.Property("id-x", "key-x", "x").OfType(builtIns.Integer);
        Coord.Property("id-y", "key-y", "y").OfType(builtIns.Integer);
        Coord.Property("id-z", "key-z", "z").OfType(builtIns.Integer);

        Geometry.Containment("id-shapes", "key-shapes", "shapes").OfType(IShape).IsMultiple().IsOptional();
        Geometry.Containment("id-documentation", "key-documentation", "documentation").OfType(Documentation)
            .IsOptional();

        Line.Extending(Shape);
        Line.Implementing(builtIns.INamed);
        Line.Containment("id-start", "key-start", "start").OfType(Coord);
        Line.Containment("id-end", "key-end", "end").OfType(Coord);

        OffsetDuplicate.Extending(Shape);
        OffsetDuplicate.Containment("id-offset", "key-offset", "offset").OfType(Coord);
        OffsetDuplicate.Reference("id-source", "key-source", "source").OfType(Shape);
        OffsetDuplicate.Reference("id-alt-source", "key-alt-source", "altSource").IsOptional().OfType(Shape);
        OffsetDuplicate.Containment("id-docs", "key-docs", "docs").IsOptional().OfType(Documentation);
        OffsetDuplicate.Containment("id-secret-docs", "key-secret-docs", "secretDocs").IsOptional()
            .OfType(Documentation);

        Shape.IsAbstract().Implementing(builtIns.INamed, IShape);
        // TODO: change name to "docs", so we have a name conflict in OffsetDuplicate that the generator needs to resolve
        Shape.Containment("id-shape-docs", "key-shape-docs", "shapeDocs").IsOptional().OfType(Documentation);
        Documentation.Annotating(Shape);

        IShape.Property("id-uuid", "key-uuid", "uuid").OfType(builtIns.String);
        IShape.Containment("id-fixpoints", "key-fixpoints", "fixpoints").OfType(Coord).IsMultiple().IsOptional();

        CompositeShape.Extending(Shape);
        CompositeShape.Containment("id-parts", "key-parts", "parts").OfType(IShape).IsMultiple();
        CompositeShape.Containment("id-disabled-parts", "key-disabled-parts", "disabledParts").OfType(IShape)
            .IsMultiple();
        CompositeShape.Containment("id-evil-part", "key-evil-part", "evilPart").OfType(IShape);

        Documentation.Property("id-text", "key-text", "text").OfType(builtIns.String).IsOptional();
        Documentation.Property("id-technical", "key-technical", "technical").OfType(builtIns.Boolean)
            .IsOptional();

        ReferenceGeometry.Reference("id-shape-references", "key-shapes-references", "shapes").OfType(IShape)
            .IsMultiple().IsOptional();

        BillOfMaterials.Reference("id-materials", "key-materials", "materials").OfType(IShape).IsMultiple()
            .IsOptional();
        BillOfMaterials.Containment("id-groups", "key-groups", "groups").OfType(MaterialGroup).IsMultiple()
            .IsOptional();
        BillOfMaterials.Containment("id-alt-groups", "key-alt-groups", "altGroups").OfType(MaterialGroup).IsMultiple()
            .IsOptional();
        BillOfMaterials.Containment("id-default-group", "key-default-group", "defaultGroup").OfType(MaterialGroup)
            .IsOptional();
        BillOfMaterials.Annotating(builtIns.Node);

        MaterialGroup.Property("id-matter-state", "key-matter-state", "matterState").OfType(MatterState).IsOptional();
        MaterialGroup.Reference("id-group-materials", "key-group-materials", "materials").OfType(IShape).IsMultiple();
        MaterialGroup.Containment("id-default-shape", "key-default-shape", "defaultShape").OfType(IShape).IsOptional();

        MatterState.EnumerationLiteral("id-solid", "key-solid", "solid");
        MatterState.EnumerationLiteral("id-liquid", "key-liquid", "liquid");
        MatterState.EnumerationLiteral("id-gas", "key-gas", "gas");
    }
}