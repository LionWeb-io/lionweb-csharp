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

namespace LionWeb.Core;

using M1;
using M1.Event;
using M1.Event.Partition;
using M1.Event.Partition.Emitter;
using M2;
using M3;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// An interface that LionWeb AST nodes implement to provide <em>read</em> access.
public interface IReadableNode
{
    /// The <em>unique within the repository</em> ID of the node.
    /// A node id has no "meaning". 
    /// An id must be a valid <i>identifier</i>, i.e. a non-empty string of arbitrary length
    /// comprised of uppercase or lowercase A to Z, 1 to 9, - (dash) and _ (underscore).
    public NodeId GetId();

    /// <summary>
    /// The parent of the node, or <c>null</c> if the node is a root node.
    /// The parent of <c>this</c> node should have a containment of <c>this</c> node among its <see cref="Containment"/> features.
    /// </summary>
    /// <seealso cref="IWritableNode.SetParent"/>
    public IReadableNode? GetParent();

    /// <summary>
    /// Contains all annotations.
    /// </summary>
    /// <seealso cref="IWritableNode.AddAnnotations"/>
    /// <seealso cref="IWritableNode.InsertAnnotations"/>
    /// <seealso cref="IWritableNode.RemoveAnnotations"/>
    public IReadOnlyList<IReadableNode> GetAnnotations();
    
    /// The <see cref="Classifier"/> that <c>this</c> node is an instance of.
    public Classifier GetClassifier();

    /// Returns all features for which a value has been set on <c>this</c> node.
    public IEnumerable<Feature> CollectAllSetFeatures();

    /// <summary>
    /// Gets the value of the given <paramref name="feature"/> on <c>this</c> node.
    /// </summary>
    /// <exception cref="UnsetFeatureException">If <paramref name="feature"/> has not been set or is empty.</exception>
    /// <seealso cref="IWritableNode.Set"/>
    /// <see cref="CollectAllSetFeatures"/>
    public object? Get(Feature feature);
    
    /// <summary>
    /// Tries to get the value of the given <paramref name="feature"/> on <c>this</c> node.
    /// </summary>
    /// <returns>
    /// If <paramref name="feature"/> is _single_: <c>true</c> if <paramref name="feature"/> is set on <c>this</c>; <c>false</c> otherwise.
    /// If <paramref name="feature"/> is _multiple_: <c>true</c> if <paramref name="feature"/> is not empty on <c>this</c>; <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="Get"/>
    /// <see cref="CollectAllSetFeatures"/>
    public bool TryGet(Feature feature, [NotNullWhen(true)] out object? value);
}

/// The type-parametrized twin of the non-generic <see cref="IReadableNode"/> interface.
public interface IReadableNode<out T> : IReadableNode where T : IReadableNode
{
    /// <inheritdoc/>
    IReadableNode? IReadableNode.GetParent() => GetParent();

    /// <inheritdoc cref="IReadableNode.GetParent()"/>
    public new T? GetParent();

    /// <inheritdoc/>
    IReadOnlyList<IReadableNode> IReadableNode.GetAnnotations() => (IReadOnlyList<IReadableNode>)GetAnnotations();

    /// <inheritdoc cref="IReadableNode.GetAnnotations()"/>
    public new IReadOnlyList<T> GetAnnotations();
}

/// Instance of an <see cref="Annotation"/>.
/// <inheritdoc />
public interface IAnnotationInstance : IReadableNode
{
    Classifier IReadableNode.GetClassifier() => GetAnnotation();

    /// <inheritdoc cref="IReadableNode.GetClassifier()"/>
    public Annotation GetAnnotation();
}

/// <inheritdoc cref="IAnnotationInstance" />
public interface IAnnotationInstance<out T> : IReadableNode<T>, IAnnotationInstance where T : IReadableNode
{
}

/// Instance of an <see cref="Concept"/>.
/// <inheritdoc />
public interface IConceptInstance : IReadableNode
{
    Classifier IReadableNode.GetClassifier() => GetClassifier();

    /// <inheritdoc cref="IReadableNode.GetClassifier()"/>
    public Concept GetConcept();
}

/// <inheritdoc cref="IConceptInstance" />
public interface IConceptInstance<out T> : IReadableNode<T>, IConceptInstance where T : IReadableNode
{
}


/// Instance of an <see cref="Concept.Partition"/>.
/// <inheritdoc />
public interface IPartitionInstance : IConceptInstance
{
    /// Optional hook to listen to partition events.
    /// Not supported by every implementation. 
    IPartitionPublisher? GetPublisher() => null;

    /// Optional hook to raise partition events.
    /// Not supported by every implementation. 
    IPartitionCommander? GetCommander() => null;
}

/// <inheritdoc cref="IPartitionInstance" />
public interface IPartitionInstance<out T> : IConceptInstance<T>, IPartitionInstance where T : IReadableNode
{
}

/// An interface that LionWeb AST nodes implement to provide <em>write</em> access.
public interface IWritableNode : IReadableNode
{
    /// <inheritdoc cref="IReadableNode.GetParent"/>
    protected internal void SetParent(IWritableNode? value);

    /// <summary>
    /// Internal helper to remove <paramref name="child"/> from <c>this</c> node's containments. 
    /// </summary>
    /// <param name="child">The child node to detach.</param>
    /// <returns><c>true</c> if <paramref name="child"/> was contained in any of <c>this</c> node's containment, <c>false</c> otherwise.</returns>
    protected internal bool DetachChild(IWritableNode child);

    /// Removes <c>this</c> node from its <see cref="IReadableNode.GetParent">parents'</see> containments.
    /// After completion, <c>this</c> node does not have a parent, and the former parent does not contain <c>this</c> node anymore.
    public void DetachFromParent();

    /// <summary>
    /// Returns the <see cref="Containment"/> of <c>this</c> node that contains <paramref name="child"/>, if any.
    /// </summary>
    /// <param name="child">Node to get the containment of.</param>
    /// <returns><see cref="Containment"/> of <c>this</c> node that contains <paramref name="child"/>, if any; <c>null</c> otherwise.</returns>
    public Containment? GetContainmentOf(IWritableNode child);

    /// <summary>
    /// Adds <see cref="Annotation">annotations</see> to <c>this</c> node.
    /// </summary>
    /// <param name="annotations">Instances of <see cref="Annotation"/> to add.</param>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> contains any node that's not an instance of <see cref="Annotation"/>,
    /// or the annotation cannot <see cref="Annotation.Annotates">annotate</see> <c>this</c> node.
    /// </exception>
    /// <seealso cref="IReadableNode.GetAnnotations"/>
    /// <seealso cref="InsertAnnotations"/>
    /// <seealso cref="RemoveAnnotations"/>
    public void AddAnnotations(IEnumerable<IWritableNode> annotations);

    /// <summary>
    /// Inserts <see cref="Annotation">annotations</see> into <c>this</c> node's annotations at <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Position to insert <paramref name="annotations"/> at.</param>
    /// <param name="annotations">Instances of <see cref="Annotation"/> to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <c>this</c> node's annotations contains less than <paramref name="index"/> annotations.</exception>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> contains any node that's not an instance of <see cref="Annotation"/>,
    /// or the annotation cannot <see cref="Annotation.Annotates">annotate</see> <c>this</c> node.
    /// </exception>
    /// <seealso cref="IReadableNode.GetAnnotations"/>
    /// <seealso cref="AddAnnotations"/>
    /// <seealso cref="RemoveAnnotations"/>
    public void InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations);

    /// <summary>
    /// Removes <see cref="Annotation">annotations</see> to <c>this</c> node.
    /// </summary>
    /// <param name="annotations">Instances of <see cref="Annotation"/> to add.</param>
    /// <returns><c>true</c> if any of <paramref name="annotations"/> used to be annotations of <c>this</c> node and have been removed; <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> contains any node that's not an instance of <see cref="Annotation"/>,
    /// or the annotation cannot <see cref="Annotation.Annotates">annotate</see> <c>this</c> node.
    /// </exception>
    /// <seealso cref="IReadableNode.GetAnnotations"/>
    /// <seealso cref="AddAnnotations"/>
    /// <seealso cref="InsertAnnotations"/>
    public bool RemoveAnnotations(IEnumerable<IWritableNode> annotations);
    
    /// <summary>
    /// Sets the given <paramref name="feature"/> on <c>this</c> node to the given <paramref name="value"/>.
    /// </summary>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> does not adhere to <paramref name="feature"/>'s type or constraints</exception>
    /// <seealso cref="IReadableNode.Get"/>
    public void Set(Feature feature, object? value);
}

/// The type-parametrized twin of the non-generic <see cref="IWritableNode"/> interface.
public interface IWritableNode<T> : IReadableNode<T>, IWritableNode where T : class, IWritableNode
{
    /// <inheritdoc/>
    void IWritableNode.SetParent(IWritableNode? parent)
    {
        switch (parent)
        {
            case null:
                SetParent((T?)null);
                return;
            case T t:
                SetParent(t);
                return;
            default:
                throw new UnsupportedNodeTypeException(parent, nameof(parent));
        }
    }

    /// <inheritdoc cref="IWritableNode.SetParent"/>
    protected internal void SetParent(T? parent);

    /// <inheritdoc/>
    bool IWritableNode.DetachChild(IWritableNode child)
    {
        if (child is T t)
        {
            return DetachChild(t);
        }

        throw new UnsupportedNodeTypeException(child, nameof(child));
    }

    /// <inheritdoc cref="IWritableNode.DetachChild"/>
    protected internal bool DetachChild(T child);

    /// <inheritdoc/>
    Containment? IWritableNode.GetContainmentOf(IWritableNode child)
    {
        if (child is T t)
        {
            return GetContainmentOf(t);
        }

        throw new UnsupportedNodeTypeException(child, nameof(child));
    }

    /// <inheritdoc cref="IWritableNode.GetContainmentOf"/>
    public Containment? GetContainmentOf(T child);

    /// <inheritdoc/>
    void IWritableNode.AddAnnotations(IEnumerable<IWritableNode> annotations) =>
        AddAnnotations(M2Extensions.AsNodes<T>(annotations));

    /// <inheritdoc cref="IWritableNode.AddAnnotations"/>
    public void AddAnnotations(IEnumerable<T> annotations);

    /// <inheritdoc/>
    void IWritableNode.InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations) =>
        InsertAnnotations(index, M2Extensions.AsNodes<T>(annotations));

    /// <inheritdoc cref="IWritableNode.InsertAnnotations"/>
    public void InsertAnnotations(Index index, IEnumerable<T> annotations);

    /// <inheritdoc/>
    bool IWritableNode.RemoveAnnotations(IEnumerable<IWritableNode> annotations) =>
        RemoveAnnotations(M2Extensions.AsNodes<T>(annotations));

    /// <inheritdoc cref="IWritableNode.RemoveAnnotations"/>
    public bool RemoveAnnotations(IEnumerable<T> annotations);
}

/// An interface that instances of LionWeb <see cref="StructuredDataType">StructuredDataTypes</see> implement.
public interface IStructuredDataTypeInstance
{
    /// The <see cref="StructuredDataType"/> that <c>this</c> is an instance of.
    public StructuredDataType GetStructuredDataType();

    /// Returns all fields for which a value has been set on <c>this</c>.
    public IEnumerable<Field> CollectAllSetFields();

    /// <summary>
    /// Gets the value of the given <paramref name="field"/> on <c>this</c>.
    /// </summary>
    /// <exception cref="UnsetFieldException">If <paramref name="field"/> has not been set.</exception>
    /// <see cref="CollectAllSetFields"/>
    public object? Get(Field field);
}

/// Every model node is an instance of <see cref="INode"/>.
public interface INode : INotifiableNode<INode>;

/// Base implementation of <see cref="IReadableNode{T}"/>.
public abstract class ReadableNodeBase<T> : IReadableNode<T> where T : IReadableNode
{
    /// The <see cref="IBuiltInsLanguage"/> variant used for this node.
    protected virtual IBuiltInsLanguage _builtIns => new Lazy<IBuiltInsLanguage>(() => GetClassifier().GetLanguage().LionWebVersion.BuiltIns).Value;

    /// The <see cref="ILionCoreLanguage"/> variant used for this node.
    protected virtual ILionCoreLanguage _m3 => new Lazy<ILionCoreLanguage>(() => GetClassifier().GetLanguage().LionWebVersion.LionCore).Value;


    /// <summary>
    /// Initializes <c>this</c> node's <see cref="IReadableNode.GetId">id</see> and optionally <see cref="IReadableNode.GetParent">parent</see>.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <param name="parent"><c>this</c> node's <see cref="IReadableNode.GetParent">parent</see></param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    protected ReadableNodeBase(NodeId id, T? parent)
    {
        if (!IdUtils.IsValid(id))
            throw new InvalidIdException(id);

        _id = id;
        _parent = parent;
    }

    private readonly NodeId _id;

    /// <inheritdoc />
    public NodeId GetId() => _id;

    /// <inheritdoc cref="IReadableNode.GetParent()"/>
    /// <c>protected</c> so it can be changed by <see cref="NodeBase"/>.
    // ReSharper disable once InconsistentNaming
    protected T? _parent;

    /// <inheritdoc />
    public T? GetParent() => _parent;

    /// <inheritdoc cref="IReadableNode.GetAnnotations()"/>
    // ReSharper disable once InconsistentNaming
    protected readonly List<T> _annotations = [];

    /// <inheritdoc />
    public IReadOnlyList<T> GetAnnotations() => _annotations.AsReadOnly();

    /// <inheritdoc />
    public abstract Classifier GetClassifier();

    /// <inheritdoc />
    public abstract IEnumerable<Feature> CollectAllSetFeatures();

    /// <inheritdoc />
    public abstract object? Get(Feature feature);

    /// <inheritdoc />
    public abstract bool TryGet(Feature feature, [NotNullWhen(true)] out object? value);
}

/// Base implementation of <see cref="INode"/>.
public abstract class NodeBase : ReadableNodeBase<INode>, INode
{
    /// <summary>
    /// Initializes <c>this</c> node's <see cref="IReadableNode.GetId">id</see>.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    protected NodeBase(NodeId id) : base(id, null) { }

    /// <inheritdoc />
    void IWritableNode<INode>.SetParent(INode? parent) =>
        _parent = parent;

    /// <inheritdoc />
    bool IWritableNode<INode>.DetachChild(INode child) =>
        DetachChild(child);

    /// <inheritdoc cref="IWritableNode.DetachChild"/>
    protected virtual bool DetachChild(INode child) =>
        _annotations.Remove(child);

    /// <inheritdoc />
    public void DetachFromParent()
    {
        if (_parent != null)
        {
            _parent.DetachChild(this);
            _parent = null;
        }
    }
    
    /// <inheritdoc />
    public virtual void AddAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null)
    {
        var safeAnnotations = annotations?.ToList();
        AssureAnnotations(safeAnnotations);
        AnnotationAddMultipleNotificationEmitter evt = new(this, safeAnnotations, _annotations, startIndex: null, notificationId: notificationId);
        evt.CollectOldData();
        _annotations.AddRange(SetSelfParent(safeAnnotations, null));
        evt.Notify();
    }

    /// <inheritdoc />
    public virtual void InsertAnnotations(Index index, IEnumerable<INode> annotations, INotificationId? notificationId = null)
    {
        AssureInRange(index, _annotations);
        var safeAnnotations = annotations?.ToList();
        AssureAnnotations(safeAnnotations);
        AnnotationAddMultipleNotificationEmitter evt = new(this, safeAnnotations, _annotations, startIndex: index, notificationId: notificationId);
        evt.CollectOldData();
        _annotations.InsertRange(index, SetSelfParent(safeAnnotations, null));
        evt.Notify();
    }

    /// <inheritdoc />
    public virtual bool RemoveAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) =>
        RemoveSelfParent(annotations?.ToList(), _annotations, null, AnnotationRemover, notificationId);

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() => [];

    /// <inheritdoc />
    public virtual Containment? GetContainmentOf(INode child) => null;

    /// <inheritdoc />
    public sealed override object? Get(Feature? feature)
    {
        if (GetInternal(feature, out var result))
            return result;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc cref="IReadableNode.Get"/>
    protected virtual bool GetInternal(Feature? feature, out object? result)
    {
        if (feature == null)
        {
            result = GetAnnotations();
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value) =>
        GetInternal(feature, out value);

    /// <inheritdoc />
    public void Set(Feature feature, object? value, INotificationId? notificationId = null)
    {
        if (SetInternal(feature, value, notificationId))
            return;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc cref="IWritableNode.Set"/>
    protected virtual bool SetInternal(Feature? feature, object? value, INotificationId? notificationId = null)
    {
        if (feature == null)
        {
            if (value is not IEnumerable)
                throw new InvalidValueException(feature, value);
            var safeNodes = M2Extensions.AsNodes<INode>(value).ToList();
            AssureAnnotations(safeNodes);
            AnnotationSetNotificationEmitter evt = new(this, safeNodes, _annotations, notificationId);
            evt.CollectOldData();
            RemoveSelfParent(_annotations.ToList(), _annotations, null);
            _annotations.AddRange(SetSelfParent(safeNodes, null));
            evt.Notify();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to retrieve the <see cref="IPartitionInstance.GetCommander"/> from this node's <see cref="Concept.Partition"/>.
    /// </summary>
    /// <returns>This node's <see cref="IPartitionCommander"/>, if available.</returns>
    protected virtual IPartitionCommander? GetPartitionCommander() =>
        this.GetPartition()?.GetCommander();

    #region Helpers

    /// <summary>
    /// Assures <paramref name="value"/> is not <c>null</c>.
    /// </summary>
    /// <param name="value">Value to guard against <c>null</c>.</param>
    /// <param name="feature">Feature <paramref name="value"/> originates from.</param>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> is <c>null</c>.</exception>
    protected void AssureNotNull([NotNull] object? value, Feature? feature)
    {
        if (value == null)
            throw new InvalidValueException(feature, value);
    }

    /// <summary>
    /// Assures none of <paramref name="safeNodes">list's</paramref> members are <c>null</c>.
    /// </summary>
    /// <param name="safeNodes">Value to guard against <c>null</c>.</param>
    /// <param name="link">Link <paramref name="safeNodes"/> originates from.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/>.</typeparam>
    /// <exception cref="InvalidValueException">If any member of <paramref name="safeNodes"/> is <c>null</c>.</exception>
    protected void AssureNotNullMembers<T>(IList<T> safeNodes, Link? link)
    {
        foreach (var node in safeNodes)
        {
            if (node == null)
            {
                throw new InvalidValueException(link, node);
            }
        }
    }

    /// <summary>
    /// Assures <paramref name="list"/> is not <c>null</c>, none of its members are <c>null</c>,
    /// and neither <paramref name="list"/> nor <paramref name="storage"/> are empty.
    /// </summary>
    /// <param name="list">Nodes that should be added to <paramref name="storage"/>.</param>
    /// <param name="storage">Where nodes of <paramref name="list"/> are planned to be stored.</param>
    /// <param name="link">Link containing <paramref name="storage"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="list"/> and <paramref name="storage"/>.</typeparam>
    /// <exception cref="InvalidValueException">If <paramref name="list"/> is <c>null</c>, contains any <c>null</c> members,
    /// or both <paramref name="list"/> and <paramref name="storage"/> are empty.</exception>
    protected void AssureNonEmpty<T>([NotNull] List<T>? list, List<T> storage, Link link)
    {
        if (list == null || (list.Count == 0 && storage.Count == 0))
            throw new InvalidValueException(link, list);
        AssureNotNullMembers(list, link);
    }

    /// <summary>
    /// Assures <paramref name="safeNodes"/> is not <c>null</c>, not emmpty, and none of its members are <c>null</c>.
    /// </summary>
    /// <param name="safeNodes">Nodes that should be added to <c>this</c>.</param>
    /// <param name="link">Link that should contain <paramref name="safeNodes"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/>.</typeparam>
    /// <exception cref="InvalidValueException">If <paramref name="safeNodes"/> is <c>null</c>, empty or contains any <c>null</c> members.</exception>
    protected void AssureNonEmpty<T>(List<T> safeNodes, Link link)
    {
        if (safeNodes.Count == 0)
            throw new InvalidValueException(link, safeNodes);
        AssureNotNullMembers(safeNodes, link);
    }

    /// <summary>
    /// Assures <paramref name="index"/> is in range of <paramref name="storage"/>.
    /// We need to check this independently of <see cref="List{T}.InsertRange"/>
    /// to assure we're throwing the correct exception in case multiple exceptions might occur.
    /// </summary>
    /// <param name="index">Index to check for.</param>
    /// <param name="storage">List to check for.</param>
    /// <typeparam name="T">Type of members of <paramref name="storage"/>.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is in out of range of <paramref name="storage"/>.</exception>
    protected void AssureInRange<T>(Index index, IList<T> storage)
    {
        if ((uint)index > (uint)storage.Count)
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    /// <summary>
    /// Usually, we <i>can</i> insert nodes at the end of <param name="storage"></param>
    /// by supplying <paramref name="index"/> = <paramref name="storage"/>.Count.
    /// However, if any of <paramref name="safeNodes"/> is already contained in <paramref name="storage"/>,
    /// we just move the node around -- without changing the length of <paramref name="safeNodes"/>.
    /// This method assures against that case.
    /// </summary>
    /// <param name="index">Index to check for.</param>
    /// <param name="safeNodes">Elements to be inserted.</param>
    /// <param name="storage">Currently stored elements.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/> and <paramref name="storage"/>.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If trying to insert nodes at the end of <paramref name="storage"/>,
    /// and any of <paramref name="safeNodes"/> is already contained in <paramref name="storage"/>.
    /// </exception>
    protected void AssureNoSelfMove<T>(Index index, List<T> safeNodes, List<T> storage)
    {
        if (index == storage.Count && safeNodes.Any(storage.Contains))
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    /// <summary>
    /// Assures <paramref name="storage"/> would contain at least one member after removing all of <paramref name="safeNodes"/>.
    /// Does <i>not</i> modify <paramref name="storage"/>.
    /// </summary>
    /// <param name="safeNodes">Candidates to be removed.</param>
    /// <param name="storage">Currently stored elements.</param>
    /// <param name="link">Link of <paramref name="storage"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/> and <paramref name="storage"/>.</typeparam>
    /// <exception cref="InvalidValueException">If <paramref name="storage"/> were empty after removing all of <paramref name="safeNodes"/>.</exception>
    protected void AssureNotClearing<T>(List<T> safeNodes, List<T> storage, Link link) where T : IReadableNode
    {
        var copy = new List<T>(storage);
        RemoveAll(safeNodes, copy, null);
        if (copy.Count == 0)
            throw new InvalidValueException(link, safeNodes);
    }

    /// <summary>
    /// Assures all of <paramref name="annotations"/> are instances of <see cref="Annotation"/>,
    /// and can annotate <c>this</c> node's <see cref="IReadableNode.GetClassifier">classifier</see>.
    /// </summary>
    /// <param name="annotations">Annotations to check.</param>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> is <c>null</c>, contains any <c>null</c>,
    /// contains any non-<see cref="Annotation"/> instance,
    /// or contains any annotation that cannot annotate <c>this</c> node's <see cref="IReadableNode.GetClassifier">classifier</see>.
    /// </exception>
    protected void AssureAnnotations([NotNull] IList<INode>? annotations)
    {
        AssureNotNull(annotations, null);
        AssureNotNullMembers(annotations, null);
        foreach (var a in annotations)
        {
            if (a.GetClassifier() is not Annotation ann || !ann.CanAnnotate(GetClassifier()))
                throw new InvalidValueException(null, a);
        }
    }

    /// <summary>
    /// Unsets <paramref name="child">child's</paramref> parent, if applicable. 
    /// Does <i>not</i> update parent's containments.
    /// </summary>
    /// <param name="child">Node to unset parent of.</param>
    protected void SetParentNull(INode? child)
    {
        if (child != null)
            SetParentInternal(child, null);
    }

    /// <summary>
    /// <see cref="DetachChildInternal">Detaches</see> <paramref name="child"/> from its current parent,
    /// and adds it to <c>this</c> node's containments.
    /// Does <i>not</i> update parent's containments.
    /// </summary>
    /// <param name="child">Node to become a child of <c>this</c> node.</param>
    protected void AttachChild(INode? child)
    {
        if (child != null)
        {
            DetachChildInternal(child);
            SetParentInternal(child, this);
        }
    }

    /// <summary>
    /// Sets <paramref name="child">child's</paramref> parent to <paramref name="parent"/>.
    /// For some visibility reason, we cannot call <c>child.SetParent(parent)</c> directly at all places.
    /// Does <i>not</i> update parent's containments.
    /// </summary>
    /// <param name="child">Child to set new parent of.</param>
    /// <param name="parent">New parent to <paramref name="child"/>.</param>
    /// <seealso cref="IWritableNode.SetParent"/>
    protected void SetParentInternal(INode child, INode? parent) =>
        child.SetParent(parent);

    /// <summary>
    /// Detaches <paramref name="child"/> from its parent, if applicable.
    /// Updates old parent's containments.
    /// </summary>
    /// <param name="child">Child to detach from its parent.</param>
    /// <seealso cref="IWritableNode.DetachChild"/>
    protected void DetachChildInternal(INode child)
    {
        var parent = child.GetParent();
        if (parent != null)
            parent.DetachChild(child);
    }

    /// <summary>
    /// Returns <paramref name="storage"/> as non-empty read-only list.
    /// </summary>
    /// <param name="storage">list to return.</param>
    /// <param name="link">Origin of <paramref name="storage"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="storage"/>.</typeparam>
    /// <returns>Non-empty, read-only view of <paramref name="storage"/>.</returns>
    /// <exception cref="UnsetFeatureException">If <paramref name="storage"/> is empty.</exception>
    protected IReadOnlyList<T> AsNonEmptyReadOnly<T>(List<T> storage, Link link) =>
        storage.Count != 0
            ? storage.AsReadOnly()
            : throw new UnsetFeatureException(link);

    /// <summary>
    /// Returns singleton list of <paramref name="node"/>.
    /// </summary>
    /// <param name="node">Node to return in a singleton list.</param>
    /// <param name="link">Origin of <paramref name="node"/>.</param>
    /// <typeparam name="T">Desired type of returned singleton list.</typeparam>
    /// <returns>Singleton list of <paramref name="node"/>.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="node"/> is not an instance of <typeparamref name="T"/>.</exception>
    protected List<T> AsList<T>(IReadableNode node, Link? link)
    {
        if (node is not T t)
            throw new InvalidValueException(link, node);

        return [t];
    }

    /// <summary>
    /// Removes all members of <paramref name="list"/> from their old parent, and sets <c>this</c> node as their new parent.
    /// Updates old parents' containments.
    /// Does <i>not</i> update new parent's (aka <c>this</c>) containments.
    /// </summary>
    /// <param name="list">Nodes that should have <c>this</c> node as parent.</param>
    /// <param name="link">Origin of <paramref name="list"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="list"/>.</typeparam>
    /// <returns><paramref name="list"/> as new list.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="list"/> is <c>null</c> or contains any <c>null</c> members.</exception>
    protected List<T> SetSelfParent<T>([NotNull] List<T>? list, Link? link) where T : IReadableNode
    {
        AssureNotNull(list, link);
        AssureNotNullMembers(list, link);

        return list.Select(n =>
        {
            if (n is INode iNode)
            {
                DetachChildInternal(iNode);
                SetParentInternal(iNode, this);
            }

            return n;
        }).ToList();
    }

    /// <summary>
    /// Removes <paramref name="node"/> from <paramref name="storage"/>, and sets its parent to <c>null</c>.
    /// Does <i>not</i> update old parent's containment.
    /// </summary>
    /// <param name="node">Node to remove from <paramref name="storage"/>.</param>
    /// <param name="storage">Storage potentially containing <paramref name="node"/>.</param>
    /// <param name="link">Origin of <paramref name="storage"/>.</param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of members of <paramref name="storage"/>.</typeparam>
    /// <returns><c>true</c> if <paramref name="node"/> has been removed from <paramref name="storage"/>; <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="node"/> is <c>null</c> or not an instance of <typeparamref name="T"/>.</exception>
    protected bool RemoveSelfParent<T>(INode node, List<T> storage, Link? link, INotificationId? notificationId = null)
        where T : class, INode =>
        RemoveSelfParent(AsList<T>(node, link), storage, link);

    /// <summary>
    /// Removes all members of <paramref name="list"/> from <paramref name="storage"/>, and sets their parent to <c>null</c>.
    /// Does <i>not</i> update old parents' containments.
    /// </summary>
    /// <param name="list">Nodes to remove from <paramref name="storage"/>.</param>
    /// <param name="storage">Storage potentially containing members of <paramref name="list"/>.</param>
    /// <param name="link">Origin of <paramref name="storage"/>.</param>
    /// <param name="remover">
    /// Optional Action to call for each removed element of <paramref name="list"/>.
    /// Only called if <see cref="GetPartitionCommander"/> is available.
    /// </param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of members of <paramref name="list"/> and <paramref name="storage"/>.</typeparam>
    /// <returns><c>true</c> if at least one member of <paramref name="list"/> has been removed from <paramref name="storage"/>; <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="list"/> is <c>null</c> or contains any <c>null</c> members.</exception>
    protected bool RemoveSelfParent<T>([NotNull] List<T>? list, List<T> storage, Link? link, Action<IPartitionCommander, Index, T, INotificationId?>? remover = null, INotificationId? notificationId = null)
        where T : IReadableNode
    {
        AssureNotNull(list, link);
        AssureNotNullMembers(list, link);

        var partitionCommander = GetPartitionCommander();

        bool result = false;
        foreach (T node in list)
        {
            var index = storage.IndexOf(node);
            if (index < 0)
                continue;

            storage.RemoveAt(index);
            result = true;
            if(node is  INode iNode)
                SetParentInternal(iNode, null);
            if (partitionCommander != null && remover != null)
                remover(partitionCommander, index, node, notificationId);
        }

        return result;
    }

    /// <summary>
    /// Removes all members of <paramref name="safeNodes"/> from <paramref name="storage"/>.
    /// Silently ignores members of <paramref name="safeNodes"/> that aren't part of <paramref name="storage"/>.
    /// </summary>
    /// <param name="safeNodes">Nodes to remove.</param>
    /// <param name="storage">Storage of nodes.</param>
    /// <param name="remover">
    /// Optional Action to call for each removed element of <paramref name="safeNodes"/>.
    /// Only called if <see cref="GetPartitionCommander"/> is available.
    /// </param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/> and <paramref name="storage"/>.</typeparam>
    protected void RemoveAll<T>(List<T> safeNodes, List<T> storage, Action<IPartitionCommander, Index, T, INotificationId?>? remover, INotificationId? notificationId = null)
        where T : IReadableNode
    {
        var partitionCommander = GetPartitionCommander();

        foreach (var node in safeNodes)
        {
            var index = storage.IndexOf(node);
            if (index < 0)
                continue;

            storage.RemoveAt(index);
            if (partitionCommander != null && remover != null)
                remover(partitionCommander, index, node, notificationId);
        }
    }

    /// Raises <see cref="ReferenceDeletedNotification"/> for <paramref name="reference"/>.
    protected Action<IPartitionCommander, Index, T, INotificationId?> ReferenceRemover<T>(Reference reference) where T : IReadableNode =>
        (commander, index, node, notificationId) =>
        {
            IReferenceTarget deletedTarget = new ReferenceTarget(null, node);
            commander.Raise(new ReferenceDeletedNotification(this, reference, index, deletedTarget, notificationId ?? commander.CreateEventId()));
        };

    /// Raises <see cref="ChildDeletedNotification"/> for <paramref name="containment"/>.
    protected Action<IPartitionCommander, Index, T, INotificationId?> ContainmentRemover<T>(Containment containment) where T : INode =>
        (commander, index, node, notificationId) =>
            commander.Raise(new ChildDeletedNotification(node, this, containment, index, notificationId ?? commander.CreateEventId()));

    /// Raises <see cref="AnnotationDeletedNotification"/>.
    private void AnnotationRemover(IPartitionCommander commander, Index index, INode node, INotificationId? notificationId = null) =>
        commander.Raise(new AnnotationDeletedNotification(node, this, index, notificationId ?? commander.CreateEventId()));

    #endregion
}

/// Base implementation of <see cref="IAnnotationInstance{T}"/>.
public abstract class AnnotationInstanceBase : NodeBase, IAnnotationInstance<INode>
{
    /// <inheritdoc />
    protected AnnotationInstanceBase(NodeId id) : base(id) { }

    /// <inheritdoc cref="IAnnotationInstance.GetClassifier()" />
    public override Classifier GetClassifier() => GetAnnotation();

    /// <inheritdoc />
    public abstract Annotation GetAnnotation();
}

/// Base implementation of <see cref="IConceptInstance{T}"/>.
public abstract class ConceptInstanceBase : NodeBase, IConceptInstance<INode>
{
    /// <inheritdoc />
    protected ConceptInstanceBase(NodeId id) : base(id) { }

    /// <inheritdoc cref="IConceptInstance.GetClassifier()" />
    public override Classifier GetClassifier() => GetConcept();

    /// <inheritdoc />
    public abstract Concept GetConcept();
}

/// Base implementation of <see cref="IPartitionInstance{T}"/>.
public abstract class PartitionInstanceBase : ConceptInstanceBase, IPartitionInstance<INode>
{
    /// <inheritdoc />
    protected PartitionInstanceBase(NodeId id) : base(id) { }
}