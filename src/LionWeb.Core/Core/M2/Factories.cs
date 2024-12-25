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
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using Utilities;

/// An <see cref="INodeFactory"/> instance creates nodes, given an ID and a Concept.
/// For concepts that the instance is not familiar with, a "placeholder" node (such as an instance of <see cref="DynamicNode"/>) should be returned.
public interface INodeFactory
{
    /// <returns>A node created given the <paramref name="id">ID</paramref> and <paramref name="classifier">Classifier</paramref>.</returns>
    /// <exception cref="UnsupportedClassifierException">If <paramref name="classifier"/> cannot be instantiated.</exception>
    public INode CreateNode(string id, Classifier classifier);

    /// <returns>The C# (runtime) enumeration literal corresponding to the given <paramref name="literal">LionCore M3 enumeration literal</paramref>.</returns>
    /// <exception cref="UnsupportedEnumerationLiteralException">If <paramref name="literal"/> cannot be found.</exception>
    public Enum GetEnumerationLiteral(EnumerationLiteral literal);

    /// <returns>An instance of <paramref name="structuredDataType"/>, initialized with <paramref name="fieldValues"/>.</returns>
    /// <exception cref="UnsupportedStructuredDataTypeException">If <paramref name="structuredDataType"/> cannot be created.</exception>
    public IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues);
}

/// Intermediate data structure to initialize an <see cref="IStructuredDataTypeInstance"/>.
public interface IFieldValues : IEnumerable<(Field field, object? value)>
{
    /// <summary>
    /// Gets the value associated with <paramref name="field"/>.
    /// </summary>
    /// <param name="field">Field to get the value associated with.</param>
    /// <returns>The value associated with <paramref name="field"/>, or <c>null</c> if <paramref name="field"/> is not set.</returns>
    object? Get(Field field);
}

/// <inheritdoc />
public class FieldValues : IFieldValues
{
    private readonly Dictionary<Field, object?> _fieldValues;

    /// <inheritdoc cref="FieldValues"/>
    public FieldValues()
    {
        _fieldValues = new Dictionary<Field, object?>(new FieldIdentityComparer());
    }

    /// <inheritdoc cref="FieldValues"/>
    /// <param name="fieldValues">Field/value pairs to initialize with.</param>
    public FieldValues(IEnumerable<(Field, object?)> fieldValues)
    {
        _fieldValues = fieldValues.ToDictionary(new FieldIdentityComparer());
    }

    /// <summary>
    /// Adds a new Field/value pair.
    /// </summary>
    /// <param name="field">Field to add.</param>
    /// <param name="value">Value to add.</param>
    public void Add(Field field, object? value) =>
        _fieldValues.Add(field, value);

    /// <inheritdoc />
    public object? Get(Field field) =>
        _fieldValues.GetValueOrDefault(field);

    /// <inheritdoc />
    public IEnumerator<(Field, object?)> GetEnumerator() =>
        _fieldValues.Select(p => (p.Key, p.Value)).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}

/// An abstract base implementation of <see cref="INodeFactory"/>
/// that holds a reference to the (single) <see cref="Language"/> it's a node factory for.
public abstract class AbstractBaseNodeFactory(Language language) : INodeFactory
{
    /// The <see cref="Language"/> this <see cref="AbstractBaseNodeFactory"/> is a node factory for.
    public readonly Language Language = language;

    /// <inheritdoc />
    public abstract INode CreateNode(string id, Classifier classifier);

    /// Fallback implementation of <see cref="CreateNode"/> that <see cref="LogWarning">logs</see> a warning,
    /// and instantiates an instance of the generic <see cref="DynamicNode"/> that's not backed by a specific class.
    protected INode FallbackCreateNode(string id, Classifier classifier)
    {
        LogWarning(
            $"No class was generated for classifier with meta-pointer {classifier.ToMetaPointer()} - returning direct instance of Node.");
        return CreateDynamicNode(id, classifier);
    }

    /// Default implementation of <see cref="CreateNode"/> that instantiates
    /// an instance of the generic <see cref="DynamicNode"/> that's not backed by a specific class.
    protected DynamicNode CreateDynamicNode(string id, Classifier classifier) => classifier switch
    {
        Annotation a => new DynamicAnnotationInstance(id, a),
        Concept { Partition: true } => new DynamicPartitionInstance(id, classifier),
        _ => new DynamicNode(id, classifier)
    };

    /// Logs <paramref name="message"/> as warning.
    protected virtual void LogWarning(string message) =>
        Console.WriteLine($"[WARNING]{message}");

    /// Creates a new, unique <see cref="IReadableNode.GetId">node id</see>.
    protected virtual string GetNewId() => IdUtils.NewId();

    /// <inheritdoc />
    public abstract Enum GetEnumerationLiteral(EnumerationLiteral literal);

    /// A typed implementation of <see cref="GetEnumerationLiteral"/> that's used by generated code.
    protected T EnumValueFor<T>(EnumerationLiteral literal) where T : Enum
        => Enum.GetValues(typeof(T))
               .OfType<T>()
               .FirstOrDefault(enumValue => enumValue.LionCoreKey() == literal.Key)
           ?? throw new UnsupportedEnumerationLiteralException(literal);

    /// <summary>
    /// Implementation of <see cref="CreateStructuredDataTypeInstance"/> that dynamically instantiates <paramref name="sdtType"/>.
    /// </summary>
    /// <param name="sdtType">C# type to instantiate. MUST implement <see cref="IStructuredDataTypeInstance"/> with the same pattern as a <c>struct record</c>.</param>
    /// <param name="fieldValues">Values to initialize with.</param>
    /// <returns>New instance of <paramref name="sdtType"/>, initialized with <paramref name="fieldValues"/>.</returns>
    protected IStructuredDataTypeInstance StructuredDataTypeInstanceFor(Type sdtType, IFieldValues fieldValues)
    {
        if (!sdtType.IsSubclassOf(typeof(IStructuredDataTypeInstance)) || sdtType.TypeInitializer is null)
            throw new UnsupportedStructuredDataTypeException(sdtType);

        var result = sdtType.TypeInitializer.Invoke([]);

        foreach ((Field field, var value) in fieldValues)
        {
            var propertyInfo = sdtType.GetProperties().FirstOrDefault(p => p.LionCoreKey() == field.Key)
                               ?? throw new UnsupportedStructuredDataTypeException(sdtType,
                                   $"Cannot find property for key '{field.Key}'");
            propertyInfo.SetValue(result, value);
        }

        return (IStructuredDataTypeInstance)result;
    }

    /// <inheritdoc />
    public abstract IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues);
}

/// An <see cref="INodeFactory"/> that's a node factory for multiple languages.
[Obsolete("Not needed anymore, all usage sites can handle multiple languages and factories.")]
public class MultiLanguageNodeFactory : INodeFactory
{
    private readonly IDictionary<Language, AbstractBaseNodeFactory> _map;

    /// Can be passed as the params argument to the Deserializer.Deserialize method.
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

    /// <inheritdoc />
    public IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues)
        => _map[structuredDataType.GetLanguage()].CreateStructuredDataTypeInstance(structuredDataType, fieldValues);
}

/// Creates all requested node instances and enumerations dynamically.
public class ReflectiveBaseNodeFactory(Language language) : AbstractBaseNodeFactory(language)
{
    /// <inheritdoc />
    public override INode CreateNode(string id, Classifier classifier) =>
        CreateDynamicNode(id, classifier);

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

    /// <inheritdoc />
    public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues) =>
        new DynamicStructuredDataTypeInstance(structuredDataType, fieldValues);
}