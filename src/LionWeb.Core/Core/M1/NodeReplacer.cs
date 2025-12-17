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

namespace LionWeb.Core.M1;

using M3;

/// <summary>
/// Replaces <paramref name="self"/> in its parent with <paramref name="replacement"/>.
///
/// Does <i>not</i> change references to <paramref name="self"/>.
/// </summary>
/// <param name="self">Base node, must have a parent.</param>
/// <param name="replacement">Node that will replace <paramref name="self"/> in <paramref name="self"/>'s parent.</param>
/// <typeparam name="T">Type of <paramref name="replacement"/>.</typeparam>
/// <returns><paramref name="replacement"/></returns>
/// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent.</exception>
internal class NodeReplacer<T>(INode self, T replacement) where T : INode
{
    protected INode _parent = null!;
    protected Containment _containment = null!;
    protected int _index;

    public virtual T Replace()
    {
        if (!CheckParameters())
            return replacement;

        InitFields();

        if (_containment == null)
            ReplaceAnnotation();
        else
            ReplaceContainment();

        Success = true;
        return replacement;
    }

    public bool Success { get; private set; } = false;
    
    protected virtual void InitFields()
    {
        _containment = _parent.GetContainmentOf(self)!;
    }

    private bool CheckParameters()
    {
        if (ReferenceEquals(self, replacement))
            return false;

        _parent = self.GetParent()!;
        if (_parent == null)
            throw new TreeShapeException(self, "Cannot replace a node with no parent");

        if (replacement is null)
            throw new UnsupportedNodeTypeException(replacement, nameof(replacement));

        return true;
    }

    #region Annotation

    private void ReplaceAnnotation()
    {
        var index = CheckAnnotation();

        ReplaceAnnotation(index);
    }

    private Index CheckAnnotation()
    {
        if (self is not IAnnotationInstance ann || replacement is not IAnnotationInstance replAnn)
            throw new InvalidValueException(null, replacement);

        _index = _parent.GetAnnotationsRaw().IndexOf(ann);
        if (_index < 0)
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        return _index;
    }

    private void ReplaceAnnotation(Index index)
    {
        if (!_parent.InsertAnnotationsRaw(index, replacement)
            || !_parent.RemoveAnnotationsRaw(self))
        {
            throw new InvalidValueException(null, replacement);
        }
    }

    #endregion

    #region Containment

    private void ReplaceContainment()
    {
        if (_containment.Multiple)
            ReplaceMultipleContainment();
        else
            ReplaceSingleContainment();
    }

    private void ReplaceMultipleContainment()
    {
        if (!_parent.TryGetContainmentsRaw(_containment, out var nodes))
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        _index = nodes.IndexOf(self);
        if (_index < 0)
            // should not happen
            throw new TreeShapeException(self, "Node not contained in its parent");

        if (!_parent.InsertContainmentsRaw(_containment, _index, replacement)
            || !_parent.RemoveContainmentsRaw(_containment, self))
        {
            throw new InvalidValueException(_containment, replacement);
        }
    }

    private void ReplaceSingleContainment()
    {
        if (!_parent.SetContainmentRaw(_containment, replacement))
            throw new InvalidValueException(_containment, replacement);
    }

    #endregion
}