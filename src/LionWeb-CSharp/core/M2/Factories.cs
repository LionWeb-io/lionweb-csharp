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

namespace LionWeb.Core.M2;

using M3;
using Serialization;
using System.Reflection;
using System.Reflection.Emit;
using Utilities;

/// <summary>
/// An <see cref="INodeFactory"/> instance creates nodes, given an ID and a Concept.
/// For concepts that the instance is not familiar with, a "placeholder" node (such as an instance of <see cref="DynamicNode"/>) should be returned.
/// </summary>
public interface INodeFactory
{
    /// <returns>A node created given the <paramref name="id">ID</paramref> and <paramref name="classifier">Classifier</paramref>.</returns>
    public INode CreateNode(string id, Classifier classifier);

    /// <returns>The C# (runtime) enumeration literal corresponding to the given <paramref name="literal">LionCore M3 enumeration literal</paramref>.</returns>
    public Enum GetEnumerationLiteral(EnumerationLiteral literal);
}

/// <summary>
/// An abstract base implementation of <see cref="INodeFactory"/>
/// that holds a reference to the (single) <see cref="Language"/> it's a node factory for.
/// </summary>
public abstract class AbstractBaseNodeFactory(Language language) : INodeFactory
{
    /// <summary>
    /// The <see cref="Language"/> this <see cref="AbstractBaseNodeFactory"/> is a node factory for.
    /// </summary>
    public readonly Language Language = language;

    /// <inheritdoc />
    public abstract INode CreateNode(string id, Classifier classifier);

    /// <summary>
    /// Fallback implementation of <see cref="CreateNode"/> that prints a warning to the console,
    /// and instantiates an instance of the generic <see cref="DynamicNode"/> that's not backed by a specific class.
    /// </summary>
    protected INode FallbackCreateNode(string id, Classifier classifier)
    {
        LogWarning(
            $"No class was generated for classifier with meta-pointer {classifier.ToMetaPointer()} - returning direct instance of Node.");
        return new DynamicNode(id, classifier);
    }

    /// Logs <paramref name="message"/> as warning.
    protected virtual void LogWarning(string message) =>
        Console.WriteLine($"[WARNING]{message}");

    /// Creates a new, unique <see cref="IReadableNode.GetId">node id</see>.
    protected virtual string GetNewId() => IdUtils.NewId();

    /// <inheritdoc />
    public abstract Enum GetEnumerationLiteral(EnumerationLiteral literal);

    /// <summary>
    /// A typed implementation of <see cref="GetEnumerationLiteral"/> that's used by generated code.
    /// </summary>
    protected T EnumValueFor<T>(EnumerationLiteral literal) where T : Enum
        => Enum.GetValues(typeof(T))
               .OfType<T>()
               .FirstOrDefault(enumValue => enumValue.LionCoreKey() == literal.Key)
           ?? throw new UnsupportedEnumerationLiteralException(literal);
}

/// <summary>
/// An <see cref="INodeFactory"/> that's a node factory for multiple languages.
/// </summary>
public class MultiLanguageNodeFactory : INodeFactory
{
    private readonly IDictionary<Language, AbstractBaseNodeFactory> _map;

    /// <summary>
    /// Can be passed as the params argument to the Deserializer.Deserialize method.
    /// </summary>
    public readonly (Language language, INodeFactory nodeFactory)[] Tuples;

    /// Creates a combined factory for all <paramref name="factories"/>.
    public MultiLanguageNodeFactory(params AbstractBaseNodeFactory[] factories)
    {
        _map = factories.ToDictionary(
            nodeFactory => nodeFactory.Language,
            nodeFactory => nodeFactory
        );
        Tuples = factories.Select(nodeFactory => (nodeFactory.Language, nodeFactory as INodeFactory)).ToArray();
    }

    /// <inheritdoc />
    public INode CreateNode(string id, Classifier classifier)
        => _map[classifier.GetLanguage()].CreateNode(id, classifier);

    /// <inheritdoc />
    public Enum GetEnumerationLiteral(EnumerationLiteral literal)
        => _map[literal.GetEnumeration().GetLanguage()].GetEnumerationLiteral(literal);
}

/// Creates all requested node instances and enumerations dynamically.
public class ReflectiveBaseNodeFactory(Language language) : AbstractBaseNodeFactory(language)
{
    /// <inheritdoc />
    public override INode CreateNode(string id, Classifier classifier) =>
        FallbackCreateNode(id, classifier);

    private readonly Dictionary<Enumeration, Type> _enums = [];

    /// <inheritdoc />
    public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
    {
        if (_enums.TryGetValue(literal.GetEnumeration(), out Type enm))
        {
            var result = Enum.Parse(enm, literal.Name);
            return result as Enum;
        }

        return CreateEnum(literal);
    }

    private Enum CreateEnum(EnumerationLiteral literal)
    {
        var name = new AssemblyName(literal.GetEnumeration().Name);
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(literal.GetEnumeration().Name);
        var enumBuilder = moduleBuilder.DefineEnum("EnumeratedTypes." + literal.GetEnumeration().Name,
            TypeAttributes.Public,
            typeof(int));

        int val = 1;
        foreach (var lit in literal.GetEnumeration().Literals)
        {
            enumBuilder.DefineLiteral(lit.Name, val++);
        }

        Type type = enumBuilder.CreateType();
        _enums[literal.GetEnumeration()] = type;
        return Enum.Parse(type, literal.Name) as Enum;
    }
}