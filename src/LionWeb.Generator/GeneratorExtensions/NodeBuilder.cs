// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Generator.GeneratorExtensions;

using Core;
using Core.M3;
using Core.Utilities;
using Impl;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Impl.AstExtensions;

/// <summary>
/// Emits Roslyn AST that creates a LionWeb node instance.
/// Assumes the classifier of the node has been <see cref="GeneratorFacade">generated</see>.  
/// </summary>
public class NodeBuilder
{
    private string? _nodeId;
    private string? _nodeType;
    private readonly Dictionary<string, ExpressionSyntax> _initializers = [];

    /// <summary>
    /// Creates a builder without initial values;
    /// </summary>
    public NodeBuilder()
    {
    }

    /// <summary>
    /// Initializes a builder with <paramref name="instance"/>'s
    /// <see cref="IReadableNode.GetId">node id</see>,
    /// <see cref="IReadableNode.GetClassifier">classifier</see>,
    /// and all <see cref="IReadableNode.CollectAllSetFeatures">set</see> <see cref="Property">properties</see>. 
    /// </summary>
    /// <exception cref="InvalidIdException"></exception>
    public NodeBuilder(IReadableNode instance)
    {
        _nodeId = instance.GetId();
        _nodeType = instance.GetClassifier().Name;
        foreach (var property in instance.CollectAllSetFeatures().OfType<Property>())
        {
            WithInitializer(instance, property);
        }
    }

    /// <summary>
    /// Created node's <see cref="IReadableNode.GetId">node id</see>. 
    /// </summary>
    public string? NodeId
    {
        get => _nodeId;
        private set
        {
            CheckNodeId(value);
            _nodeId = value;
        }
    }

    private static void CheckNodeId([NotNull] string? value)
    {
        if (value is null || !IdUtils.IsValid(value))
            throw new InvalidIdException(value);
    }

    /// <inheritdoc cref="NodeId"/>
    /// <exception cref="InvalidIdException"></exception>
    public NodeBuilder WithNodeId(string nodeId)
    {
        NodeId = nodeId;
        return this;
    }

    /// <summary>
    /// Created node's <see cref="IReadableNode.GetClassifier">classifier</see>.
    /// </summary>
    public string? NodeType
    {
        get => _nodeType;
        private set
        {
            CheckNodeType(value);
            _nodeType = value;
        }
    }

    private static void CheckNodeType([NotNull] string? value)
    {
        if (!SyntaxFacts.IsValidIdentifier(value))
            throw new ArgumentException($"Not a valid identifier: {value}");
    }

    /// <inheritdoc cref="NodeType"/>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is not a valid C# identifier.</exception>
    public NodeBuilder WithNodeType(string nodeTypeName)
    {
        NodeType = nodeTypeName;
        return this;
    }

    /// <inheritdoc cref="WithNodeType(string)"/>
    public NodeBuilder WithNodeType(Type nodeType)
    {
        NodeType = nodeType.Name;
        return this;
    }

    /// <summary>
    /// Created node's <i>C# property initializers</i>.
    /// </summary>
    /// <param name="value">property name as <i>key</i>, property value as <i>value</i>.</param>
    /// <exception cref="ArgumentException">If any <i>key</i> is not a valid C# identifier.</exception>
    public Dictionary<string, ExpressionSyntax> Initializers
    {
        get => _initializers;
    }

    private static void CheckPropertyName(string propertyName)
    {
        if (!SyntaxFacts.IsValidIdentifier(propertyName))
            throw new ArgumentException($"Not a valid property name: {propertyName}");
    }

    /// <summary>
    /// Adds an initializer <tt><paramref name="propertyName"/> = <paramref name="initialValue"/></tt>.
    /// Replaces already existing initializer for <paramref name="propertyName"/>.
    /// </summary>
    /// <exception cref="ArgumentException">If <paramref name="propertyName"/> is not a valid C# identifier.</exception>
    public NodeBuilder WithInitializer(string propertyName, ExpressionSyntax initialValue)
    {
        CheckPropertyName(propertyName);
        _initializers[propertyName] = initialValue;
        return this;
    }

    /// <inheritdoc cref="WithInitializer(string, ExpressionSyntax)"/>
    public NodeBuilder WithInitializer(string propertyName, string initialValue) =>
        WithInitializer(propertyName, initialValue.AsLiteral());

    /// <inheritdoc cref="WithInitializer(string, ExpressionSyntax)"/>
    public NodeBuilder WithInitializer(string propertyName, bool initialValue) =>
        WithInitializer(propertyName, initialValue.AsLiteral());

    /// <inheritdoc cref="WithInitializer(string, ExpressionSyntax)"/>
    public NodeBuilder WithInitializer(string propertyName, int initialValue) =>
        WithInitializer(propertyName, initialValue.AsLiteral());

    /// <inheritdoc cref="WithInitializer(string, ExpressionSyntax)"/>
    public NodeBuilder WithInitializer(IReadableNode instance, Property property) =>
        WithInitializer(property.Name.ToFirstUpper(), instance.Get(property) switch
        {
            string s => s.AsLiteral(),
            bool b => b.AsLiteral(),
            int i => i.AsLiteral(),
            var v => throw new InvalidValueException(property, v)
        });

    /// <summary>
    /// Emits Roslyn AST.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidIdException">If <see cref="NodeId"/> is invalid.</exception>
    /// <exception cref="ArgumentException">If <see cref="NodeType"/> is not a valid C# identifier.</exception>
    public ObjectCreationExpressionSyntax Build()
    {
        CheckNodeId(_nodeId);
        CheckNodeType(_nodeType);
        foreach (var key in _initializers.Keys)
        {
            CheckPropertyName(key);
        }

        var result = ObjectCreationExpression(IdentifierName(_nodeType))
            .WithArgumentList(AsArguments([_nodeId.AsLiteral()]));

        if (_initializers.Count == 0)
            return result;

        return result
            .WithInitializer(InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    _initializers
                        .OrderBy(p => p.Key)
                        .Select(p =>
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName(p.Key),
                            p.Value)
                    )
                )
            ));
    }

    /// <summary>
    /// Joins all <paramref name="expressions"/> in square brackets.
    /// </summary>
    public static CollectionExpressionSyntax Join(params ExpressionSyntax[] expressions) =>
        CollectionExpression(SeparatedList<CollectionElementSyntax>(expressions.Select(ExpressionElement)));
}