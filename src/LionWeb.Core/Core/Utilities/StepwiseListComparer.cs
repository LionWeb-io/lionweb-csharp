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

namespace LionWeb.Core.Utilities;

using LeftIndex = Index;
using RightIndex = Index;

/// <inheritdoc />
/// <remarks>
/// <see cref="ListComparer{T}"/> reports <i>all</i> changes based on the original and result indices.
/// This class converts them into stepwise changes, i.e. the indices in each step are based on all previous changes. 
/// </remarks>
public class StepwiseListComparer<T> : IListComparer<T>
{
    private readonly IListComparer<T> _listComparer;

    private readonly Dictionary<LeftIndex, RightIndex> _leftIndices;
    private readonly Dictionary<RightIndex, LeftIndex> _rightIndices;

    /// <inheritdoc cref="ListComparer{T}(List{T}, List{T}, IEqualityComparer{T}?)"/>
    public StepwiseListComparer(List<T> left, List<T> right, IEqualityComparer<T>? comparer = null)
    {
        var maxSize = Math.Max(left.Count, right.Count);
        _listComparer = new ListComparer<T>(left, right, comparer);

        _leftIndices = new Dictionary<LeftIndex, RightIndex>(maxSize);
        _rightIndices = new Dictionary<RightIndex, LeftIndex>(maxSize);

        for (var i = 0; i < maxSize; i++)
        {
            _leftIndices.Add(i, i);
            _rightIndices.Add(i, i);
        }
    }

    /// <inheritdoc />
    public List<IListComparer<T>.IChange> Compare()
    {
        var allChanges = _listComparer.Compare();

        return allChanges
            .OrderBy(it => it.Index)
            .Select(change => change switch
            {
                IListComparer<T>.Added added => Added(added),
                IListComparer<T>.Deleted deleted => Deleted(deleted),
                IListComparer<T>.Replaced replaced => Replaced(replaced),
                IListComparer<T>.Moved moved => Moved(moved),
            })
            .ToList();
    }

    private IListComparer<T>.IChange Added(IListComparer<T>.Added added)
    {
        Add(added.RightIndex);
        return added with { RightIndex = _rightIndices[added.RightIndex] };
    }

    private IListComparer<T>.IChange Replaced(IListComparer<T>.Replaced replaced) =>
        replaced with { LeftIndex = _leftIndices[replaced.LeftIndex] };

    private IListComparer<T>.IChange Moved(IListComparer<T>.Moved moved)
    {
        Add(Math.Min(moved.LeftIndex, moved.RightIndex) + 1, moved.RightIndex);
        Delete(moved.LeftIndex, Math.Max(moved.LeftIndex, moved.RightIndex) - 1);
        return moved with
        {
            LeftIndex = _leftIndices[moved.LeftIndex], RightIndex = _rightIndices[moved.RightIndex]
        };
    }

    private IListComparer<T>.IChange Deleted(IListComparer<T>.Deleted deleted)
    {
        Delete(deleted.LeftIndex);
        return deleted with { LeftIndex = _leftIndices[deleted.LeftIndex] };
    }

    private void Add(RightIndex lowerBoundInclusive, RightIndex? upperBoundExclusive = null)
    {
        LeftIndex leftLowerBoundInclusive = _rightIndices[lowerBoundInclusive];
        LeftIndex? leftUpperBoundExclusive = upperBoundExclusive != null ? _rightIndices[(RightIndex)upperBoundExclusive] : null;
        _rightIndices[lowerBoundInclusive] = lowerBoundInclusive;
        Increment(leftLowerBoundInclusive, _leftIndices, leftUpperBoundExclusive);
        Increment(lowerBoundInclusive + 1, _rightIndices, upperBoundExclusive);
    }

    private void Delete(LeftIndex lowerBoundInclusive, LeftIndex? upperBoundExclusive = null)
    {
        RightIndex rightLowerBoundInclusive = _leftIndices[lowerBoundInclusive];
        RightIndex? rightUpperBoundExclusive = upperBoundExclusive != null ? _leftIndices[(LeftIndex)upperBoundExclusive] : null;
        Decrement(lowerBoundInclusive + 1, _leftIndices, upperBoundExclusive);
        Decrement(rightLowerBoundInclusive, _rightIndices, rightUpperBoundExclusive);
    }

    private void Increment(int lowerBoundInclusive, Dictionary<int, int> dictionary, int? upperBoundExclusive = null)
    {
        int upperBound = upperBoundExclusive ?? dictionary.Count;
        for (int i = lowerBoundInclusive; i < upperBound; i++)
            dictionary[i] += 1;
    }

    private void Decrement(int lowerBoundInclusive, Dictionary<int, int> dictionary, int? upperBoundExclusive = null)
    {
        int upperBound = upperBoundExclusive ?? dictionary.Count;
        for (int i = lowerBoundInclusive; i < upperBound; i++)
            dictionary[i] -= 1;
    }
}