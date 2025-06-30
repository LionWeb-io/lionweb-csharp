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

namespace LionWeb.Core.Utilities;

using M1;
using M3;
using System.Collections;
using System.Collections.Immutable;

/// <summary>
/// <para>
/// Deeply compares two lists of nodes with detailed report on differences.
/// Considers all input nodes, including all descendants and annotations. 
/// </para>
///
/// <list type="bullet">
///   <listheader>Elements are considered equal if:</listheader>
///   <item>Language: same key, same version</item>
///   <item>Classifier: same key, same Language</item>
///   <item>Feature: same key, same Classifier</item>
///   <item>Node: same classifier, same features, same annotations</item>
///   <item>list of nodes: same length, and same node at each position</item>
///   <item>annotations: same list of nodes</item>
///   <item>features: each feature is set and equal</item>
///   <item>property: same type and same value</item>
///   <item>string property: string comparison</item>
///   <item>integer property: int comparison</item>
///   <item>boolean property: bool comparison</item>
///   <item>enum property: same literal name and C# enum type</item>
///   <item>literal name: string comparison</item>
///   <item>C# enum type: ==</item>
///   <item>single containment: same node</item>
///   <item>multiple containment: same list of nodes</item>
///   <item>single reference with internal target: target at same relative position</item>
///   <item>single reference with external target: target with same id</item>
///   <item>multiple reference: same reference at each position</item>
///   <item>key: string comparison</item>
///   <item>version: string comparison</item>
///   <item>id: string comparison</item>
/// </list>
///
/// <para>
/// We consider a reference target _internal_ if the target is part of the input nodes.
/// Otherwise, we consider the target _external_.
/// </para>
/// </summary>
/// <param name="_left">List of initial nodes on left side.</param>
/// <param name="_right">List of initial nodes on right side.</param>
public class Comparer(IList<IReadableNode?> _left, IList<IReadableNode?> _right)
{
    private readonly Dictionary<IReadableNode, IReadableNode?> _nodeMapping = [];

    /// <param name="left">List of initial nodes on left side.</param>
    /// <param name="right">List of initial nodes on right side.</param>
    public Comparer(IList<INode?> left, IList<INode?> right) : this(
        left.Cast<IReadableNode?>().ToList(),
        right.Cast<IReadableNode?>().ToList()
    )
    {
    }

    /// List of initial nodes on left side. 
    public ImmutableList<IReadableNode?> Left => [.. _left];

    /// List of initial nodes on right side. 
    public ImmutableList<IReadableNode?> Right => [.. _right];

    /// <inheritdoc cref="ComparerBehaviorConfig"/>
    public ComparerBehaviorConfig BehaviorConfig { get; init; } = new();

    /// Differences found by this comparer, if any.
    /// Only populated after <see cref="Compare()"/> has been called.
    public List<IDifference> Differences { get; } = [];

    /// <summary>
    /// Compares <see cref="Left"/> and <see cref="Right"/>.
    /// </summary>
    /// <returns>List of differences. Empty list if <see cref="Left"/> and <see cref="Right"/> are equal.</returns>
    public IEnumerable<IDifference> Compare()
    {
        List<IDifference> result = CompareNodes(null, _left, null, null, _right);

        PostprocessReferenceTargets(result);

        RemoveEmptyContainers(result);

        Differences.AddRange(result);

        return Differences;
    }

    /// <summary>
    /// Convenience method to directly check whether <see cref="Left"/> and <see cref="Right"/> are equal.
    /// </summary>
    /// <returns>true if <see cref="Left"/> and <see cref="Right"/> are equal, false otherwise.</returns>
    public bool AreEqual()
    {
        IEnumerable<IDifference> compareNodes = Compare();
        return !compareNodes.Any();
    }

    /// <summary>
    /// Formats all <see cref="Differences"/> into a tree. 
    /// </summary>
    public string ToMessage(ComparerOutputConfig outputConfig) =>
        Differences.DescribeAll(outputConfig);

    /// <summary>
    /// We only know whether two internal reference targets are equal after full comparison.
    /// Thus, we add a <see cref="InternalTargetDifference"/> for each reference target during regular traversal.
    /// At the end, we revisit them. If the targets are equal, we remove the <see cref="InternalTargetDifference"/>. 
    /// </summary>
    private void PostprocessReferenceTargets(List<IDifference> result)
    {
        foreach (var targetDifference in result.OfType<InternalTargetDifference>().ToList())
        {
            if (_nodeMapping.TryGetValue(targetDifference.LeftTarget, out IReadableNode? rightTarget) &&
                AreSameSideNodesEqual(rightTarget, targetDifference.RightTarget))
            {
                result.Remove(targetDifference);
            }
        }
    }

    /// <summary>
    /// From <see cref="PostprocessReferenceTargets"/>, we might have some empty
    /// <see cref="IContainerDifference">containers</see> left over.
    /// Here we remove them.
    ///
    /// <para>
    /// Assumes <paramref name="result"/> is ordered with all children of a container behind the container.
    /// </para>
    /// </summary>
    private void RemoveEmptyContainers(List<IDifference> result)
    {
        var reversed = result.OfType<IContainerDifference>().ToList();
        reversed.Reverse();

        foreach (var r in reversed)
        {
            if (result.All(d => d.Parent != r))
            {
                result.Remove(r);
            }
        }
    }

    /// Compares <paramref name="left"/> and <paramref name="right"/>.
    protected virtual List<IDifference> CompareNodes(IReadableNode? leftOwner, IList<IReadableNode?> left, Link? link,
        IReadableNode? rightOwner, IList<IReadableNode?> right)
    {
        List<IDifference> result = [];

        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();
        while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
        {
            var leftCurrent = leftEnumerator.Current;
            var rightCurrent = rightEnumerator.Current;

            result.AddRange(CompareNode(leftOwner, leftCurrent, link, rightOwner, rightCurrent));
        }

        var nodeCountDifference = new NodeCountDifference(leftOwner, left.Count, link, rightOwner, right.Count);
        if (left.Count > right.Count)
        {
            result.Add(nodeCountDifference);
            foreach (var leftSurplus in left.Skip(right.Count))
            {
                if (leftSurplus != null)
                {
                    _nodeMapping[leftSurplus] = null;
                    result.Add(new LeftSurplusNodeDifference(leftOwner, link, leftSurplus));
                }
            }
        } else if (left.Count < right.Count)
        {
            result.Add(nodeCountDifference);
            result.AddRange(right.Skip(left.Count).Where(r => r != null)
                .Select(n => new RightSurplusNodeDifference(rightOwner, link, n!)));
        }

        return result;
    }

    /// Compares <paramref name="left"/> and <paramref name="right"/>.
    protected virtual List<IDifference> CompareNode(IReadableNode? leftOwner, IReadableNode? left, Link? containment,
        IReadableNode? rightOwner, IReadableNode? right)
    {
        if (left != null)
            _nodeMapping[left] = right;

        List<IDifference> result = [];

        switch (left, right)
        {
            case (not null, not null):
                result.AddRange(CompareClassifier(left, right));
                result.AddRange(CompareFeatures(left, right));
                result.AddRange(CompareAnnotations(left, right));
                SetParent(result, new NodeDifference(left, right));
                break;
            case (null, not null):
                result.Add(new LeftNullNodeDifference(rightOwner, containment, right));
                break;

            case (not null, null):
                result.Add(new RightNullNodeDifference(leftOwner, containment, left));
                break;
        }

        return result;
    }

    /// Compares <paramref name="left"/> and <paramref name="right"/>.
    protected virtual List<IDifference> CompareAnnotations(IReadableNode left, IReadableNode right)
    {
        List<IDifference> result = [];

        result.AddRange(
            CompareNodes(left, left.GetAnnotations().ToList(), null, right, right.GetAnnotations().ToList()));

        return SetParent(result, new AnnotationDifference(left, right));
    }

    /// Compares <paramref name="left"/> and <paramref name="right"/>.
    protected virtual List<IDifference> CompareFeatures(IReadableNode left, IReadableNode right)
    {
        List<IDifference> result = [];

        var leftFeatures = left.CollectAllSetFeatures().ToList();
        var rightFeatures = right.CollectAllSetFeatures().ToList();

        var rightFeatureDict = rightFeatures.ToDictionary(f => f, f => f, new FeatureIdentityComparer());

        foreach (Feature leftFeature in leftFeatures)
        {
            if (rightFeatureDict.TryGetValue(leftFeature, out Feature? rightFeature))
            {
                result.AddRange(CompareFeature(left, leftFeature, right, rightFeature));
                rightFeatureDict.Remove(leftFeature);
            } else
            {
                result.AddRange(
                    RegisterSurplusFeature(left, right, leftFeature, true,
                        (leftP, feature, rightP, parent) =>
                            new UnsetFeatureRightDifference(leftP, feature, rightP) { Parent = parent },
                        (owner, link, surplus, parent) =>
                            new LeftSurplusNodeDifference(owner, link, surplus) { Parent = parent }));
            }
        }

        foreach (var remainingRightFeature in rightFeatureDict.Keys)
        {
            result.AddRange(
                RegisterSurplusFeature(left, right, remainingRightFeature, false,
                    (leftP, feature, rightP, parent) =>
                        new UnsetFeatureLeftDifference(leftP, feature, rightP) { Parent = parent },
                    (owner, link, surplus, parent) =>
                        new RightSurplusNodeDifference(owner, link, surplus) { Parent = parent }));
        }

        return result;
    }

    /// <summary>
    /// We want to be sure the logic for left and right surplus features is the same.
    /// As the logic is extensive, we parameterize the side we're working on.
    /// </summary>
    private static List<IDifference> RegisterSurplusFeature(
        IReadableNode left,
        IReadableNode right,
        Feature feature,
        bool useLeft,
        Func<IReadableNode, Feature, IReadableNode, IContainerDifference?, IUnsetFeatureDifference> unsetFactory,
        Func<IReadableNode, Link, IReadableNode, IContainerDifference?, ISurplusNodeDifference> surplusFactory
    )
    {
        List<IDifference> result = [];

        IContainerDifference? parent = null;
        switch (feature)
        {
            case Containment cont:
                parent = new ContainmentDifference(left, cont, right);
                result.Add(parent);
                break;
            case Reference refer:
                parent = new ReferenceDifference(left, refer, right);
                result.Add(parent);
                break;
        }

        result.Add(unsetFactory(left, feature, right, parent));
        if (feature is Link { Multiple: true } link)
        {
            var owner = useLeft ? left : right;
            var value = owner.Get(feature);
            if (value is ICollection coll)
            {
                (int leftCount, int rightCount) = useLeft ? (coll.Count, 0) : (0, coll.Count);
                result.Add(new NodeCountDifference(left, leftCount, link, right, rightCount) { Parent = parent });
                foreach (var entry in coll.OfType<IReadableNode>())
                {
                    result.Add(surplusFactory(owner, link, entry, parent));
                }
            }
        }

        return result;
    }


    /// Compares <paramref name="leftFeature"/> and <paramref name="rightFeature"/>.
    protected virtual List<IDifference> CompareFeature(IReadableNode left, Feature leftFeature, IReadableNode right,
        Feature rightFeature)
    {
        List<IDifference> result = [];

        switch (leftFeature, rightFeature)
        {
            case (Property leftProp, Property rightProp):
                result.AddRange(CompareProperty(left, leftProp, right, rightProp));
                break;
            case (Containment leftCont, Containment rightCont):
                result.AddRange(CompareContainment(left, leftCont, right, rightCont));
                break;
            case (Reference leftRef, Reference rightRef):
                result.AddRange(CompareReference(left, leftRef, right, rightRef));
                break;
        }

        return result;
    }

    /// Compares <paramref name="leftProp"/> and <paramref name="rightProp"/>.
    protected virtual List<IDifference> CompareProperty(IReadableNode left, Property leftProp, IReadableNode right,
        Property rightProp)
    {
        List<IDifference> result = [];

        var leftValue = left.Get(leftProp);
        var rightValue = right.Get(rightProp);

        switch (leftValue, rightValue)
        {
            case (null, null):
                return result;

            case (string leftStr, string rightStr):
                if (leftStr != rightStr)
                {
                    result.Add(new PropertyValueDifference(left, leftStr, leftProp, right, rightStr));
                }

                return result;

            case (int leftInt, int rightInt):
                if (leftInt != rightInt)
                {
                    result.Add(new PropertyValueDifference(left, leftInt, leftProp, right, rightInt));
                }

                return result;

            case (bool leftBool, bool rightBool):
                if (leftBool != rightBool)
                {
                    result.Add(new PropertyValueDifference(left, leftBool, leftProp, right, rightBool));
                }

                return result;

            case (Enum leftEnum, Enum rightEnum):
                if (!leftEnum.Equals(rightEnum) &&
                    leftEnum.GetType().GetEnumName(leftEnum) != rightEnum.GetType().GetEnumName(rightEnum))
                {
                    result.Add(new PropertyValueDifference(left, leftEnum, leftProp, right, rightEnum));
                }

                if (leftEnum.GetType() != rightEnum.GetType())
                {
                    result.Add(new PropertyEnumTypeDifference(left, leftEnum, leftProp, right, rightEnum));
                }

                return result;

            case (IStructuredDataTypeInstance leftSdt, IStructuredDataTypeInstance rightSdt):
                if (!leftSdt.Equals(rightSdt))
                {
                    result.Add(new PropertyValueDifference(left, leftSdt, leftProp, right, rightSdt));
                }

                return result;


            default:
                result.Add(new PropertyValueTypeDifference(left, leftValue, leftProp, right, rightValue));
                return result;
        }
    }

    /// Compares <paramref name="leftCont"/> and <paramref name="rightCont"/>.
    protected virtual List<IDifference> CompareContainment(IReadableNode left, Containment leftCont,
        IReadableNode right, Containment rightCont)
    {
        List<IDifference> result = [];

        var leftValue = left.Get(leftCont);
        var rightValue = right.Get(rightCont);

        switch (leftValue, rightValue)
        {
            case (null, null):
                return result;
            case (IReadableNode leftNode, IReadableNode rightNode):
                result.AddRange(CompareNode(left, leftNode, leftCont, right, rightNode));
                break;
            case (ICollection leftColl, ICollection rightColl):
                result.AddRange(CompareNodes(left, leftColl.OfType<IReadableNode?>().ToList(), leftCont, right,
                    rightColl.OfType<IReadableNode?>().ToList()));
                break;
            case (null, _):
                result.Add(new UnsetFeatureLeftDifference(left, leftCont, right));
                break;
            case (_, null):
                result.Add(new UnsetFeatureRightDifference(left, rightCont, right));
                break;
        }

        return SetParent(result, new ContainmentDifference(left, leftCont, right));
    }

    private List<IDifference> SetParent(List<IDifference> result, IContainerDifference parent)
    {
        if (result.Count != 0)
        {
            foreach (IDifference r in result.Where(r => r.Parent == null))
            {
                r.Parent = parent;
            }

            result.Insert(0, parent);
        }

        return result;
    }

    /// Compares <paramref name="leftRef"/> and <paramref name="rightRef"/>.
    protected virtual List<IDifference> CompareReference(IReadableNode left, Reference leftRef, IReadableNode right,
        Reference rightRef)
    {
        List<IDifference> result = [];

        var leftValue = left.Get(leftRef);
        var rightValue = right.Get(rightRef);

        switch (leftValue, rightValue)
        {
            case (null, null):
                return result;
            case (IReadableNode leftTarget, IReadableNode rightTarget):
                result.AddRange(CompareTarget(left, leftTarget, leftRef, right, rightTarget));
                break;
            case (ICollection leftColl, ICollection rightColl):
                result.AddRange(CompareTargets(left, leftColl.OfType<IReadableNode>().ToList(), leftRef, right,
                    rightColl.OfType<IReadableNode>().ToList()));
                break;
            case (null, _):
                result.Add(new UnsetFeatureLeftDifference(left, leftRef, right));
                break;
            case (_, null):
                result.Add(new UnsetFeatureRightDifference(left, rightRef, right));
                break;
        }

        return SetParent(result, new ReferenceDifference(left, leftRef, right));
    }

    protected List<IDifference> CompareTargets(IReadableNode leftOwner, List<IReadableNode> leftTargets,
        Reference reference,
        IReadableNode rightOwner,
        List<IReadableNode> rightTargets)
    {
        List<IDifference> result = [];

        using var leftEnumerator = leftTargets.GetEnumerator();
        using var rightEnumerator = rightTargets.GetEnumerator();
        while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
        {
            result.AddRange(CompareTarget(leftOwner, leftEnumerator.Current, reference, rightOwner,
                rightEnumerator.Current));
        }

        var nodeCountDifference =
            new NodeCountDifference(leftOwner, leftTargets.Count, reference, rightOwner, rightTargets.Count);
        if (leftTargets.Count > rightTargets.Count)
        {
            result.Add(nodeCountDifference);
            result.AddRange(leftTargets.Skip(rightTargets.Count)
                .Select(n => new LeftSurplusNodeDifference(leftOwner, reference, n)));
        } else if (leftTargets.Count < rightTargets.Count)
        {
            result.Add(nodeCountDifference);
            result.AddRange(
                rightTargets.Skip(leftTargets.Count)
                    .Select(n => new RightSurplusNodeDifference(rightOwner, reference, n)));
        }

        return result;
    }

    /// Compares <paramref name="leftTarget"/> and <paramref name="rightTarget"/>.
    protected virtual List<IDifference> CompareTarget(IReadableNode leftOwner, IReadableNode leftTarget,
        Reference reference, IReadableNode rightOwner,
        IReadableNode rightTarget)
    {
        List<IDifference> result = [];

        if (ContainsDeep(_left, leftTarget))
        {
            if (ContainsDeep(_right, rightTarget))
            {
                result.Add(new InternalTargetDifference(leftOwner, leftTarget, reference, rightOwner, rightTarget));
            } else
            {
                result.Add(new ExternalTargetRightDifference(leftOwner, leftTarget, reference, rightOwner,
                    rightTarget));
            }
        } else if (ContainsDeep(_right, rightTarget))
        {
            result.Add(new ExternalTargetLeftDifference(leftOwner, leftTarget, reference, rightOwner, rightTarget));
        } else
        {
            result.AddRange(CompareExternalTarget(leftOwner, leftTarget, reference, rightOwner, rightTarget));
        }

        return result;
    }

    /// Compares <paramref name="leftTarget"/> and <paramref name="rightTarget"/>.
    protected virtual List<IDifference> CompareExternalTarget(IReadableNode leftOwner, IReadableNode leftTarget,
        Reference reference,
        IReadableNode rightOwner, IReadableNode rightTarget)
    {
        List<IDifference> result = [];

        if (leftTarget.GetId() != rightTarget.GetId())
        {
            result.Add(new ExternalTargetDifference(leftOwner, leftTarget, reference, rightOwner, rightTarget));
        }

        return result;
    }

    private bool ContainsDeep(IList<IReadableNode?> list, IReadableNode target)
    {
        var allNodes = list.OfType<IReadableNode>()
            .SelectMany(n => M1Extensions.Descendants(n, true, true)).ToList();
        return allNodes.Contains(target);
    }

    /// Compares <paramref name="left"/> and <paramref name="right"/>.
    protected virtual List<IDifference> CompareClassifier(IReadableNode left, IReadableNode right)
    {
        List<IDifference> result = [];

        switch (left.GetClassifier(), right.GetClassifier())
        {
            case (Concept leftConcept, Concept rightConcept):
                result.AddRange(CompareConcept(left, leftConcept, right, rightConcept));
                break;
            case (Annotation leftAnn, Annotation rightAnn):
                result.AddRange(CompareAnnotation(left, leftAnn, right, rightAnn));
                break;
            case (Annotation, Concept) or (Concept, Annotation) when BehaviorConfig.CompareCompatibleClassifier:
                result.Add(new IncompatibleClassifierDifference(left, right));
                break;
            case var (leftClassifier, rightClassifier):
                result.AddRange(CompareClassifier(left, leftClassifier, right, rightClassifier));
                break;
        }

        return result;
    }

    /// Compares <paramref name="leftConcept"/> and <paramref name="rightConcept"/>.
    protected virtual List<IDifference> CompareConcept(IReadableNode left, Concept leftConcept, IReadableNode right,
        Concept rightConcept)
    {
        List<IDifference> result = [];

        if (!leftConcept.EqualsIdentity(rightConcept))
        {
            result.Add(new ClassifierDifference(left, right));
        }

        return result;
    }

    /// Compares <paramref name="leftAnn"/> and <paramref name="rightAnn"/>.
    protected virtual List<IDifference> CompareAnnotation(IReadableNode left, Annotation leftAnn, IReadableNode right,
        Annotation rightAnn)
    {
        List<IDifference> result = [];

        if (!leftAnn.EqualsIdentity(rightAnn))
        {
            result.Add(new ClassifierDifference(left, right));
        }

        return result;
    }

    /// Compares <paramref name="leftClassifier"/> and <paramref name="rightClassifier"/>.
    protected virtual List<IDifference> CompareClassifier(IReadableNode left, Classifier leftClassifier,
        IReadableNode right,
        Classifier rightClassifier)
    {
        List<IDifference> result = [];

        if (!leftClassifier.EqualsIdentity(rightClassifier))
        {
            result.Add(new ClassifierDifference(left, right));
        }

        return result;
    }

    /// Compares <paramref name="a"/> and <paramref name="b"/> from the same side.
    protected virtual bool AreSameSideNodesEqual(IReadableNode? a, IReadableNode? b) =>
        // ReSharper disable once PossibleUnintendedReferenceComparison
        a == b;
}

/// <summary>
/// Configures the behavior of <see cref="Comparer"/>.
/// </summary>
public class ComparerBehaviorConfig
{
    /// Whether we should compare classifier compatibility; defaults to <c>true</c>.
    /// <seealso cref="IncompatibleClassifierDifference"/>
    public bool CompareCompatibleClassifier { get; init; } = true;
}

/// <summary>
/// Configures the output of <see cref="Comparer"/>.
/// </summary>
public class ComparerOutputConfig
{
    /// Text that describes the left side, e.g. "left" or "old"
    public string LeftDescription { get; set; } = "left";

    /// Text that describes the right side, e.g. "right" or "new"
    public string RightDescription { get; set; } = "right";

    /// Whether classifiers print only their name or also key and language.
    public bool FullClassifier { get; set; } = false;

    /// Whether languages print key and version or also their id.
    public bool LanguageId { get; set; } = false;

    /// Whether features print only their name or also key and language.
    public bool FullFeature { get; set; } = false;

    /// Whether implementers of IReadableNode print only their id or also their name.
    public bool NodeName { get; set; } = true;
}