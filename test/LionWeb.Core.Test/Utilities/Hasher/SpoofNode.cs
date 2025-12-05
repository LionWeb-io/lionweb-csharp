// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Utilities.Hasher;

using Core.Notification;
using Languages.Generated.V2024_1.Shapes.M2;
using M3;
using System.Diagnostics.CodeAnalysis;

class SpoofNode(string id) : IShape
{
    public string GetId() => id;

    public INode? GetParent() => null;

    public IReadOnlyList<INode> GetAnnotations() => [];

    public Classifier GetClassifier() => ShapesLanguage.Instance.IShape;

    public IEnumerable<Feature> CollectAllSetFeatures() => [];

    public object? Get(Feature feature) => throw new UnknownFeatureException(GetClassifier(), feature);
    public bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        value = null;
        return false;
    }

    public void DetachFromParent() { }

    public void Set(Feature feature, object? value, INotificationId? notificationId = null) { }
    public void Add(Link? link, IEnumerable<IReadableNode> nodes) {}

    public void Insert(Link? link, int index, IEnumerable<IReadableNode> nodes) {}

    public void Remove(Link? link, IEnumerable<IReadableNode> nodes) {}

    public void SetParent(INode? parent) { }

    public bool DetachChild(INode child) => false;

    public Containment? GetContainmentOf(INode child) => null;

    public void AddAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) { }

    public void InsertAnnotations(Int32 index, IEnumerable<INode> annotations, INotificationId? notificationId = null) { }

    public bool RemoveAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) => false;

    public IReadOnlyList<Coord> Fixpoints { get => []; init { } }
    public IShape AddFixpoints(IEnumerable<Coord> nodes, INotificationId? notificationId = null) => this;

    public IShape InsertFixpoints(int index, IEnumerable<Coord> nodes, INotificationId? notificationId = null) => this;

    public IShape RemoveFixpoints(IEnumerable<Coord> nodes, INotificationId? notificationId = null) => this;
        
    public string Uuid { get => null; set { } }
    public IShape SetUuid(string value, INotificationId? notificationId = null) => this;

    IReadOnlyList<IReadableNode> IReadableNodeRaw.GetAnnotationsRaw() => [];

    bool IReadableNodeRaw.TryGetPropertyRaw(Property property, out object? value)
    {
        value = null;
        return false;
    }

    bool IReadableNodeRaw.TryGetContainmentRaw(Containment containment, out IReadableNode? node)
    {
        node = null;
        return false;
    }

    bool IReadableNodeRaw.TryGetContainmentsRaw(Containment containment, out IReadOnlyList<IReadableNode> nodes)
    {
        nodes = [];
        return false;
    }

    bool IReadableNodeRaw.TryGetReferenceRaw(Reference reference, out IReferenceTarget? target)
    {
        target = null;
        return false;
    }

    bool IReadableNodeRaw.TryGetReferencesRaw(Reference reference, out IReadOnlyList<IReferenceTarget> targets)
    {
        targets = [];
        return false;
    }

    bool IWritableNodeRaw.AddAnnotationsRaw(IWritableNode annotationInstances) => false;

    bool IWritableNodeRaw.InsertAnnotationsRaw(Index index, IWritableNode annotationInstances) => false;

    bool IWritableNodeRaw.RemoveAnnotationsRaw(IWritableNode annotationInstances) => false;

    public bool SetRaw(Feature feature, object? value) => false;

    bool IWritableNodeRaw.SetPropertyRaw(Property property, object? value) => false;

    bool IWritableNodeRaw.SetContainmentRaw(Containment containment, IWritableNode? node) => false;

    bool IWritableNodeRaw.AddContainmentsRaw(Containment containment, IWritableNode node) => false;

    bool IWritableNodeRaw.InsertContainmentsRaw(Containment containment, Index index, IWritableNode node) => false;

    bool IWritableNodeRaw.RemoveContainmentsRaw(Containment containment, IWritableNode node) => false;

    bool IWritableNodeRaw.SetReferenceRaw(Reference reference, ReferenceTarget? targets) => false;

    bool IWritableNodeRaw.AddReferencesRaw(Reference reference, ReferenceTarget target) => false;

    bool IWritableNodeRaw.InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target) => false;

    bool IWritableNodeRaw.RemoveReferencesRaw(Reference reference, ReferenceTarget target) => false;
}