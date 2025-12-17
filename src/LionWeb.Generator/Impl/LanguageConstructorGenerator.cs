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

namespace LionWeb.Generator.Impl;

using Core;
using Core.M2;
using Core.M3;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static AstExtensions;
using Property = Core.M3.Property;

/// <summary>
/// Generates constructor of Language class.
/// </summary>
internal class LanguageConstructorGenerator(GeneratorInputParameters generatorInputParameters)
    : LanguageGeneratorBase(generatorInputParameters)
{
    /// <inheritdoc cref="LanguageConstructorGenerator"/>
    public ConstructorDeclarationSyntax GenConstructor() =>
        Constructor(LanguageName, Param("id", AsType(typeof(string))))
            .WithInitializer(Initializer(
                "id",
                $"{AsType(typeof(LionWebVersions))}.v{_lionWebVersion.VersionString.Replace('.', '_')}"
            ))
            .WithBody(AsStatements(
                Language.Entities.Ordered().SelectMany(EntityConstructorInitialization)
                    .Append(GenFactoryInitialization())
            ));

    private StatementSyntax GenFactoryInitialization() =>
        Assignment("_factory", NewCall([This()], _names.FactoryType));

    private IEnumerable<StatementSyntax> EntityConstructorInitialization(LanguageEntity entity)
    {
        (var properties, var metaType) = entity switch
        {
            Annotation a => EntityConstructorInitialization(a),
            Concept c => EntityConstructorInitialization(c),
            Interface i => EntityConstructorInitialization(i),
            Enumeration e => EntityConstructorInitialization(e),
            PrimitiveType p => EntityConstructorInitialization(p),
            StructuredDataType s => EntityConstructorInitialization(s),
            _ => throw new ArgumentException($"unsupported entity: {entity}", nameof(entity))
        };

        if (properties.Count == 0)
            return [];

        var result = new List<StatementSyntax>
        {
            Assignment(LanguageFieldName(entity), NewLazy(
                NewCall([entity.GetId().AsLiteral(), ThisExpression()],
                    metaType, properties.ToArray())
            ))
        };

        switch (entity)
        {
            case Classifier classifier:
                result.AddRange(classifier.Features.Ordered().Select(FeatureConstructorInitialization));
                break;
            case Enumeration enumeration:
                result.AddRange(enumeration.Literals.Ordered().Select(LiteralConstructorInitialization));
                break;
            case StructuredDataType structuredDataType:
                result.AddRange(structuredDataType.Fields.Ordered().Select(FieldConstructorInitialization));
                break;
            case PrimitiveType:
                // fall-through
                break;
            default:
                throw new ArgumentException($"unsupported entity type: {entity}", nameof(entity));
        }

        return result;
    }

    #region LanguageEntity

    private (List<(string, ExpressionSyntax)>, TypeSyntax) EntityConstructorInitialization(Annotation annotation)
    {
        var result = KeyName(annotation);
        result.Add(("AnnotatesLazy", NewLazy(AsProperty(annotation.Annotates ?? _builtIns.Node))));
        result.AddRange(Extends(annotation.Extends));
        result.AddRange(Implements("ImplementsLazy", annotation.Implements));
        result.AddRange(Features(annotation));

        return (result, AsType(typeof(AnnotationBase<>), generics: LanguageType));
    }

    private (List<(string, ExpressionSyntax)>, TypeSyntax) EntityConstructorInitialization(Concept concept)
    {
        var result = KeyName(concept);
        result.Add(("Abstract", concept.Abstract.AsLiteral()));
        result.Add(("Partition", concept.Partition.AsLiteral()));
        result.AddRange(Extends(concept.Extends));
        result.AddRange(Implements("ImplementsLazy", concept.Implements));
        result.AddRange(Features(concept));

        return (result, AsType(typeof(ConceptBase<>), generics: LanguageType));
    }

    private (List<(string, ExpressionSyntax)>, TypeSyntax) EntityConstructorInitialization(Interface iface)
    {
        var result = KeyName(iface);
        result.AddRange(Implements("ExtendsLazy", iface.Extends));
        result.AddRange(Features(iface));

        return (result, AsType(typeof(InterfaceBase<>), generics: LanguageType));
    }

    private (List<(string, ExpressionSyntax)>, TypeSyntax) EntityConstructorInitialization(Enumeration enumeration)
    {
        var result = KeyName(enumeration);
        result.Add(("LiteralsLazy", NewLazy(Collection(enumeration.Literals.Ordered().Select(AsProperty)))));

        return (result, AsType(typeof(EnumerationBase<>), generics: LanguageType));
    }

    private (List<(string, ExpressionSyntax)>, TypeSyntax) EntityConstructorInitialization(PrimitiveType primitive) =>
        (KeyName(primitive), AsType(typeof(PrimitiveTypeBase<>), generics: LanguageType));

    private (List<(string, ExpressionSyntax)>, TypeSyntax) EntityConstructorInitialization(
        StructuredDataType structuredDataType)
    {
        var result = KeyName(structuredDataType);
        result.Add(("FieldsLazy", NewLazy(Collection(structuredDataType.Fields.Ordered().Select(AsProperty)))));

        return (result, AsType(typeof(StructuredDataTypeBase<>), generics: LanguageType));
    }

    private IEnumerable<(string, ExpressionSyntax)> Extends(Classifier? extends)
    {
        if (extends == null)
            return [];

        return [("ExtendsLazy", NewLazy(AsProperty(extends)))];
    }

    private IEnumerable<(string, ExpressionSyntax)> Implements(string key, IEnumerable<Interface>? ifaces)
    {
        var enumerable = ifaces?.Ordered().ToList();
        if (enumerable == null || enumerable.Count == 0)
            return [];

        return [(key, NewLazy(Collection(enumerable.Select(AsProperty))))];
    }

    private IEnumerable<(string, ExpressionSyntax)> Features(Classifier classifier)
    {
        if (!classifier.Features.Any())
            return [];

        return [("FeaturesLazy", NewLazy(Collection(classifier.Features.Ordered().Select(feature => _names.AsProperty(feature)))))];
    }

    #endregion

    private StatementSyntax FeatureConstructorInitialization(Feature feature)
    {
        var properties = KeyName(feature);
        properties.Add(("Optional", feature.Optional.AsLiteral()));

        TypeSyntax? metaType;
        ExpressionSyntax type;

        switch (feature)
        {
            case Property p:
                metaType = AsType(typeof(PropertyBase<>), generics: LanguageType);
                type = AsProperty(p.Type);
                break;
            case Containment c:
                properties.Add(("Multiple", c.Multiple.AsLiteral()));
                metaType = AsType(typeof(ContainmentBase<>), generics: LanguageType);
                type = AsProperty(c.Type);
                break;
            case Reference c:
                properties.Add(("Multiple", c.Multiple.AsLiteral()));
                metaType = AsType(typeof(ReferenceBase<>), generics: LanguageType);
                type = AsProperty(c.Type);
                break;
            default:
                throw new ArgumentException($"unsupported feature: {feature}", nameof(feature));
        }

        properties.Add(("Type", type));

        return Assignment(LanguageFieldName(feature), NewLazy(
            NewCall(
                [feature.GetId().AsLiteral(), AsProperty(feature.GetFeatureClassifier()), ThisExpression()],
                metaType,
                properties.ToArray()
            )
        ));
    }

    private StatementSyntax LiteralConstructorInitialization(EnumerationLiteral literal)
    {
        var properties = KeyName(literal);

        return Assignment(LanguageFieldName(literal), NewLazy(
            NewCall(
                [literal.GetId().AsLiteral(), AsProperty(literal.GetEnumeration()), ThisExpression()],
                AsType(typeof(EnumerationLiteralBase<>), generics: LanguageType),
                properties.ToArray()
            )
        ));
    }

    private StatementSyntax FieldConstructorInitialization(Field field)
    {
        var properties = KeyName(field);

        properties.Add(("Type", AsProperty(field.Type)));

        return Assignment(LanguageFieldName(field), NewLazy(
            NewCall(
                [field.GetId().AsLiteral(), AsProperty(field.GetStructuredDataType()), ThisExpression()],
                AsType(typeof(FieldBase<>), generics: LanguageType),
                properties.ToArray()
            )
        ));
    }

    private List<(string, ExpressionSyntax)> KeyName(IKeyed keyed) =>
    [
        ("Key", keyed.Key.AsLiteral()),
        ("Name", keyed.Name.AsLiteral())
    ];
}