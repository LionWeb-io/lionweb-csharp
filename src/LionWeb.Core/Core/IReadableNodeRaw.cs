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

public interface IReadableNodeRaw : IReadableNode
{
    protected internal IReadOnlyList<IAnnotationInstance> GetAnnotationsRaw();

    protected internal virtual bool TryGetRaw(Feature feature, out object? value)
    {
        switch (feature)
        {
            case Property:
                return TryGetPropertyRaw(feature, out value);
            case Containment { Multiple: false }:
                if (TryGetContainmentRaw(feature, out var c))
                {
                    value = c;
                    return true;
                }
                value = null;
                return false;

            case Reference { Multiple: false }:
                if (TryGetReferenceRaw(feature, out var r))
                {
                    value = r;
                    return true;
                }
                value = null;
                return false;
            case Containment { Multiple: true }:
                if (TryGetContainmentsRaw(feature, out var cs))
                {
                    value = cs;
                    return true;
                }
                value = null;
                return false;
            case Reference { Multiple: true }:
                if (TryGetReferencesRaw(feature, out var rs))
                {
                    value = rs;
                    return true;
                }
                value = null;
                return false;
            default:
                value = null;
                return false;
        }
    }

    protected internal bool TryGetPropertyRaw(Feature property, out object? value);
    protected internal bool TryGetContainmentRaw(Feature containment, out IWritableNode? node);
    protected internal bool TryGetContainmentsRaw(Feature containment, out IReadOnlyList<IWritableNode> nodes);
    protected internal bool TryGetReferenceRaw(Feature reference, out IReferenceTarget? target);
    protected internal bool TryGetReferencesRaw(Feature reference, out IReadOnlyList<IReferenceTarget> targets);
}

public interface IReadableNodeRaw<out T> : IReadableNode<T>, IReadableNodeRaw where T : IReadableNode;