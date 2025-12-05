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
    /// <remarks>
    /// <i>Should</i> return <c>List{<see cref="IAnnotationInstance"/>}</c>,
    /// but for broken models we want to allow invalid annotation instances.
    /// </remarks>
    protected internal IReadOnlyList<IReadableNode> GetAnnotationsRaw();

    /// <remarks>
    /// For broken models, we might allow invalid values for a feature (e.g. a string for a containment).
    /// </remarks>
    protected internal virtual bool TryGetRaw(Feature feature, out object? value)
    {
        switch (feature)
        {
            case Property f:
                return TryGetPropertyRaw(f, out value);
            case Containment { Multiple: false } f:
                if (TryGetContainmentRaw(f, out var c))
                {
                    value = c;
                    return true;
                }
                value = null;
                return false;

            case Reference { Multiple: false } f:
                if (TryGetReferenceRaw(f, out var r))
                {
                    value = r;
                    return true;
                }
                value = null;
                return false;
            case Containment { Multiple: true } f:
                if (TryGetContainmentsRaw(f, out var cs))
                {
                    value = cs;
                    return true;
                }
                value = null;
                return false;
            case Reference { Multiple: true } f:
                if (TryGetReferencesRaw(f, out var rs))
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

    protected internal bool TryGetPropertyRaw(Property property, out object? value);
    protected internal bool TryGetContainmentRaw(Containment containment, out IReadableNode? node);
    protected internal bool TryGetContainmentsRaw(Containment containment, out IReadOnlyList<IReadableNode> nodes);
    protected internal bool TryGetReferenceRaw(Reference reference, out IReferenceTarget? target);
    protected internal bool TryGetReferencesRaw(Reference reference, out IReadOnlyList<IReferenceTarget> targets);
}

public interface IReadableNodeRaw<out T> : IReadableNode<T>, IReadableNodeRaw where T : IReadableNode;