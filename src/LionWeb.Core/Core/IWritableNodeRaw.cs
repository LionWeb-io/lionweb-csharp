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

namespace LionWeb.Core;

using M3;

public interface IWritableNodeRaw : IWritableNode, IReadableNodeRaw
{
    protected internal void AddAnnotationsRaw(List<IAnnotationInstance> annotations);
    
    protected internal void InsertAnnotationsRaw(Index index, List<IAnnotationInstance> annotations);

    protected internal bool RemoveAnnotationsRaw(ISet<IAnnotationInstance> annotations);
    
    protected internal bool SetRaw(Feature feature, object? value);

    protected internal bool AddRaw(Link link, List<IReadableNode> nodes);

    protected internal bool InsertRaw(Link link, Index index, List<IReadableNode> nodes);

    protected internal bool RemoveRaw(Link link, List<IReadableNode> nodes);
}

public interface IWritableNodeRaw<T> : IWritableNode<T>, IWritableNodeRaw where T : class, IWritableNode
{
    void IWritableNodeRaw.AddAnnotationsRaw(List<IAnnotationInstance> annotations) =>
        AddAnnotationsRaw(annotations);
    
    protected internal bool AddAnnotationsRaw(List<T> annotations);
    
    void IWritableNodeRaw.InsertAnnotationsRaw(Index index, List<IAnnotationInstance> annotations) =>
        InsertAnnotationsRaw(index, annotations);

    protected internal bool InsertAnnotationsRaw(Index index, List<T> annotations);

    bool IWritableNodeRaw.RemoveAnnotationsRaw(ISet<IAnnotationInstance> annotations) =>
        RemoveAnnotationsRaw(annotations);
    
    protected internal bool RemoveAnnotationsRaw(HashSet<INode> annotations);
    
    bool IWritableNodeRaw.AddRaw(Link link, List<IReadableNode> nodes) =>
        AddRaw(link, nodes);
    
    protected internal bool AddRaw(Link link, List<T> nodes);

    bool IWritableNodeRaw.InsertRaw(Link link, Index index, List<IReadableNode> nodes) =>
        InsertRaw(link, index, nodes);
    
    protected internal bool InsertRaw(Link link, Index index, List<T> nodes);

    bool IWritableNodeRaw.RemoveRaw(Link link, List<IReadableNode> nodes) =>
        RemoveRaw(link, nodes);
    
    protected internal bool RemoveRaw(Link link, List<T> nodes);
}