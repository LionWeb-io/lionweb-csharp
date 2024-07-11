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

// ReSharper disable NotAccessedPositionalProperty.Global

namespace LionWeb.Core.Utilities;

using M2;
using M3;
using System.Diagnostics;
using System.Text;

/// A difference identified during <see cref="Comparer">node comparison</see>.
public interface IDifference
{
    /// Parent difference, roughly following the containment of the diffed node tree. 
    public IContainerDifference? Parent { get; set; }

    /// <summary>
    /// Concisely describes the difference in a human-readable form.
    /// Assumes the context is established by preceding output
    /// (e.g. a property difference assumes its node has been described above).
    /// </summary>
    /// <param name="outputConfig">Detailed configuration how to format the description.</param>
    /// <returns>Human-readable text description of itself.</returns>
    public string Describe(ComparerOutputConfig outputConfig);
}

/// A difference that might serve as parent.
public interface IContainerDifference : IDifference;

/// Provides helper methods for formatting the output according to <see cref="ComparerOutputConfig"/>. 
public abstract record DifferenceBase : IDifference
{
    private ComparerOutputConfig? _outputConfig;

    /// <inheritdoc />
    public IContainerDifference? Parent { get; set; }

    /// <inheritdoc/>
    // We want config to be easily accessible, but cannot set it permanently
    // (otherwise record default Equals() implementation compares it, and testing becomes hard).
    public string Describe(ComparerOutputConfig outputConfig)
    {
        try
        {
            _outputConfig = outputConfig;
            return Describe();
        } finally
        {
            _outputConfig = null;
        }
    }

    /// <inheritdoc cref="Describe(ComparerOutputConfig)"/>
    protected abstract string Describe();

    private ComparerOutputConfig OutputConfig
    {
        get
        {
            Debug.Assert(_outputConfig != null, nameof(_outputConfig) + " != null");
            return _outputConfig;
        }
    }

    /// Text that describes the left side, e.g. "left" or "old"
    protected string LeftDescription() => OutputConfig.LeftDescription;

    /// Text that describes the right side, e.g. "right" or "new"
    protected string RightDescription() => OutputConfig.RightDescription;

    /// Pretty-printing of a node including classifier according to Config.
    protected string NC(IReadableNode? node) => node != null ? $"{N(node)}{C(node.GetClassifier())}" : "null";

    /// Pretty-printing of a node according to Config.
    protected string N(IReadableNode? node)
    {
        if (node != null)
        {
            if (OutputConfig.NodeName && node.CollectAllSetFeatures().Contains(BuiltInsLanguage.Instance.INamed_name))
            {
                var name = node.Get(BuiltInsLanguage.Instance.INamed_name);
                if (name is string s)
                {
                    return $"{s}({node.GetId()})";
                }
            }

            return $"({node.GetId()})";
        }

        return "(null)";
    }

    /// Pretty-printing of a feature according to Config.
    protected string F(Feature? feature)
    {
        if (feature != null)
        {
            if (OutputConfig.FullFeature)
            {
                return $"<{feature.Name}[{feature.Key},{C(feature.GetFeatureClassifier())}]>";
            }

            return $"<{feature.Name}>";
        }

        return "null";
    }

    /// Pretty-printing of classifier according to Config.
    protected string C(LanguageEntity? entity)
    {
        if (entity != null)
        {
            if (OutputConfig.FullClassifier)
            {
                return $"<{entity.Name}[{entity.Key},{L(entity.GetLanguage())}]>";
            }

            return $"<{entity.Name}>";
        }

        return "null";
    }

    /// Pretty-printing of language according to Config.
    protected string L(Language? language)
    {
        if (language != null)
        {
            if (OutputConfig.LanguageId)
            {
                return $"{language.Key}@{language.Version}({language.GetId()})";
            }

            return $"{language.Key}@{language.Version}";
        }

        return "null";
    }
}

/// <paramref name="Left"/> and <paramref name="Right"/>, including all their descendants and their annotations,
/// differ in at least one way.
public record NodeDifference(IReadableNode Left, IReadableNode Right) : DifferenceBase, IContainerDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Node: {LeftDescription()}: {NC(Left)} vs. {RightDescription()}: {NC(Right)}";
}

/// <paramref name="Left"/> and <paramref name="Right"/> have a different <see cref="IReadableNode.GetClassifier"/>.
public record ClassifierDifference(IReadableNode Left, IReadableNode Right) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Classifiers: {LeftDescription()}: {C(Left.GetClassifier())} vs. {RightDescription()}: {C(Right.GetClassifier())}";
}

/// One of <paramref name="Left"/> and <paramref name="Right"/> is an <see cref="Annotation"/>, the other one a <see cref="Concept"/>. 
public record IncompatibleClassifierDifference(IReadableNode Left, IReadableNode Right) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Classifiers incompatible: {LeftDescription()}: {C(Left.GetClassifier())} vs. {RightDescription()}: {C(Right.GetClassifier())}";
}

/// The value of <paramref name="Property"/> in <paramref name="Left"/> and <paramref name="Right"/> differ.
public record PropertyValueDifference(
    IReadableNode Left,
    object LeftValue,
    Property Property,
    IReadableNode Right,
    object RightValue
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Value of {F(Property)}: {LeftDescription()}: '{LeftValue}' vs. {RightDescription()}: '{RightValue}'";
}

/// The LionWeb type of <paramref name="Property"/> in <paramref name="Left"/> and <paramref name="Right"/> differ.
public record PropertyValueTypeDifference(
    IReadableNode Left,
    object? LeftValue,
    Property Property,
    IReadableNode Right,
    object? RightValue
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Value type of {F(Property)}: {LeftDescription()}: <{LeftValue?.GetType()}>'{LeftValue}' vs. {RightDescription()}: <{RightValue?.GetType()}>'{RightValue}'";
}

/// The C# type of <paramref name="Property"/> in <paramref name="Left"/> and <paramref name="Right"/> differ.
public record PropertyEnumTypeDifference(
    IReadableNode Left,
    Enum LeftEnum,
    Property Property,
    IReadableNode Right,
    Enum RightEnum
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Enum type of {F(Property)}: {LeftDescription()}: <{LeftEnum.GetType()}> vs. {RightDescription()}: <{RightEnum.GetType()}>";
}

/// The <see cref="Feature"/> is not set on one side.
public interface IUnsetFeatureDifference : IDifference
{
    /// Left side of difference
    IReadableNode Left { get; init; }
    
    /// Feature of difference
    Feature Feature { get; init; }
    
    /// Right side of difference
    IReadableNode Right { get; init; }
}

/// The <paramref name="Feature"/> is not set in <paramref name="Left"/>, i.e. it exists only in <paramref name="Right"/>.
public record UnsetFeatureLeftDifference(
    IReadableNode Left,
    Feature Feature,
    IReadableNode Right
) : DifferenceBase, IUnsetFeatureDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Feature {F(Feature)} missing on {LeftDescription()}";
}

/// The <paramref name="Feature"/> is not set in <paramref name="Right"/>, i.e. it exists only in <paramref name="Left"/>.
public record UnsetFeatureRightDifference(
    IReadableNode Left,
    Feature Feature,
    IReadableNode Right
) : DifferenceBase, IUnsetFeatureDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Feature {F(Feature)} missing on {RightDescription()}";
}

/// Contents of <see cref="Link"/> in <see cref="Left"/> and <see cref="Right"/> differ.
public interface ILinkDifference : IContainerDifference
{
    /// Left side of difference
    IReadableNode Left { get; init; }
    
    /// Link of difference
    Link Link { get; }
    
    /// Right side of difference
    IReadableNode Right { get; init; }
}

/// Contents of <paramref name="Containment"/> in <paramref name="Left"/> and <paramref name="Right"/>,
/// including all their descendants and their annotations, differ in at least one way.
public record ContainmentDifference(
    IReadableNode Left,
    Containment Containment,
    IReadableNode Right
) : DifferenceBase, ILinkDifference
{
    /// <inheritdoc />
    public Link Link { get => Containment; }

    /// <inheritdoc />
    protected override string Describe() =>
        $"Containment {F(Containment)}";
}

/// The target(s) of <paramref name="Reference"/> in <paramref name="Left"/> and <paramref name="Right"/> differ.
public record ReferenceDifference(
    IReadableNode Left,
    Reference Reference,
    IReadableNode Right
) : DifferenceBase, ILinkDifference
{
    /// <inheritdoc />
    public Link Link { get => Reference; }

    /// <inheritdoc />
    protected override string Describe() =>
        $"Reference {F(Reference)}";
}

/// <paramref name="LeftTarget"/> is _external_, but <paramref name="RightTarget"/> is _internal_. 
/// <seealso cref="Comparer"/> 
public record ExternalTargetLeftDifference(
    IReadableNode LeftOwner,
    IReadableNode LeftTarget,
    Reference Reference,
    IReadableNode RightOwner,
    IReadableNode RightTarget
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"External target on {LeftDescription()}: {LeftDescription()}: {NC(LeftTarget)} vs. {RightDescription()}: {NC(RightTarget)}";
}

/// <paramref name="LeftTarget"/> is _internal_, but <paramref name="RightTarget"/> is _external_. 
/// <seealso cref="Comparer"/> 
public record ExternalTargetRightDifference(
    IReadableNode LeftOwner,
    IReadableNode LeftTarget,
    Reference Reference,
    IReadableNode RightOwner,
    IReadableNode RightTarget
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"External target on {RightDescription()}: {LeftDescription()}: {NC(LeftTarget)} vs. {RightDescription()}: {NC(RightTarget)}";
}

/// A <paramref name="Reference"/> points to different _external_ nodes. 
/// <seealso cref="Comparer"/> 
public record ExternalTargetDifference(
    IReadableNode LeftOwner,
    IReadableNode LeftTarget,
    Reference Reference,
    IReadableNode RightOwner,
    IReadableNode RightTarget
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"External target: {LeftDescription()}: {NC(LeftTarget)} vs. {RightDescription()}: {NC(RightTarget)}";
}

/// A <paramref name="Reference"/> points to different _internal_ nodes.
/// <seealso cref="Comparer"/> 
public record InternalTargetDifference(
    IReadableNode LeftOwner,
    IReadableNode LeftTarget,
    Reference Reference,
    IReadableNode RightOwner,
    IReadableNode RightTarget
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Internal target: {LeftDescription()}: {NC(LeftTarget)} vs. {RightDescription()}: {NC(RightTarget)}";
}

/// Annotations of <paramref name="Left"/> and <paramref name="Right"/> differ.
public record AnnotationDifference(
    IReadableNode Left,
    IReadableNode Right
) : DifferenceBase, IContainerDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        "Annotations";
}

/// <summary>
/// Lists of nodes differs in their length. 
/// </summary>
/// <param name="Left">Owner of the left list. Might be <c>null</c> if top-level list.</param>
/// <param name="LeftCount">Length of left list.</param>
/// <param name="Link">Feature hosting the list. Might be <c>null</c> if top-level list or annotations.</param>
/// <param name="Right">Owner of the right list. Might be <c>null</c> if top-level list.</param>
/// <param name="RightCount">Length of right list.</param>
public record NodeCountDifference(
    IReadableNode? Left,
    int LeftCount,
    Link? Link,
    IReadableNode? Right,
    int RightCount
) : DifferenceBase
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Number of nodes: {LeftDescription()}: {LeftCount} vs. {RightDescription()}: {RightCount}";
}

/// <see cref="Node"/> is member of list <see cref="Link"/> of <see cref="Owner"/>, but missing on the other side.
public interface ISurplusNodeDifference : IDifference
{
    /// Owner of surplus node
    IReadableNode? Owner { get; init; }
    
    /// Link containing surplus node
    Link? Link { get; init; }
    
    /// surplus node
    IReadableNode Node { get; init; }
}

/// <summary>
/// <paramref name="Node"/> is member of list <paramref name="Link"/> of left <paramref name="Owner"/>, but missing at right.
/// </summary>
/// <param name="Owner">Owner of the left list. Might be <c>null</c> if top-level list.</param>
/// <param name="Link">Feature hosting the list. Might be <c>null</c> if top-level list or annotations.</param>
/// <param name="Node">Surplus node.</param>
public record LeftSurplusNodeDifference(
    IReadableNode? Owner,
    Link? Link,
    IReadableNode Node
) : DifferenceBase, ISurplusNodeDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Surplus: {LeftDescription()}: {NC(Node)}";
}

/// <summary>
/// <paramref name="Node"/> is member of list <paramref name="Link"/> of right <paramref name="Owner"/>, but missing at left.
/// </summary>
/// <param name="Owner">Owner of the right list. Might be <c>null</c> if top-level list.</param>
/// <param name="Link">Feature hosting the list. Might be <c>null</c> if top-level list or annotations.</param>
/// <param name="Node">Surplus node.</param>
public record RightSurplusNodeDifference(
    IReadableNode? Owner,
    Link? Link,
    IReadableNode Node) : DifferenceBase, ISurplusNodeDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"Surplus: {RightDescription()}: {NC(Node)}";
}

/// <summary>
/// <paramref name="Node"/> is member of list <paramref name="Link"/> of right <paramref name="Owner"/>, but null at left.
/// </summary>
/// <param name="Owner">Owner of the right list. Might be <c>null</c> if top-level list.</param>
/// <param name="Link">Feature hosting the list. Might be <c>null</c> if top-level list or annotations.</param>
/// <param name="Node">Non-null right node.</param>
public record LeftNullNodeDifference(
    IReadableNode? Owner,
    Link? Link,
    IReadableNode Node
) : DifferenceBase, ISurplusNodeDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"{LeftDescription()} null: {NC(Node)}";
}

/// <summary>
/// <paramref name="Node"/> is member of list <paramref name="Link"/> of left <paramref name="Owner"/>, but null at left.
/// </summary>
/// <param name="Owner">Owner of the left list. Might be <c>null</c> if top-level list.</param>
/// <param name="Link">Feature hosting the list. Might be <c>null</c> if top-level list or annotations.</param>
/// <param name="Node">Non-null left node.</param>
public record RightNullNodeDifference(
    IReadableNode? Owner,
    Link? Link,
    IReadableNode Node) : DifferenceBase, ISurplusNodeDifference
{
    /// <inheritdoc />
    protected override string Describe() =>
        $"{RightDescription()} null: {NC(Node)}";
}

/// Utilities to pretty-print <see cref="DifferenceBase">Differences</see>.
public static class DifferenceUtils
{
    /// <summary>
    /// Formats all <paramref name="differences"/> into a tree with formatting defined by <paramref name="outputConfig"/>. 
    /// </summary>
    /// <para>
    /// Assumes <paramref name="differences"/> is ordered with all children of a container behind the container.
    /// </para>
    public static string DescribeAll(this IEnumerable<IDifference> differences, ComparerOutputConfig outputConfig)
    {
        var result = new StringBuilder();

        var parents = new Stack<IDifference>();

        foreach (var diff in differences)
        {
            if (diff.Parent == null)
            {
                parents.Clear();
            } else
            {
                while (parents.Peek() != diff.Parent)
                {
                    parents.Pop();
                }
            }

            for (int i = 0; i < parents.Count; i++)
            {
                result.Append("  ");
            }

            result.AppendLine(diff.Describe(outputConfig));

            if (diff is IContainerDifference)
            {
                parents.Push(diff);
            }
        }

        return result.ToString();
    }
}