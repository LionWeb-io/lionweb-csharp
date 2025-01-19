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

        // Execution order is important
        return Added(allChanges)
            .Concat(Moved(allChanges))
            .Concat(Deleted(allChanges))
            .ToList();
    }

    private IEnumerable<IListComparer<T>.IChange> Added(List<IListComparer<T>.IChange> allChanges) =>
        allChanges
            .OfType<IListComparer<T>.Added>()
            .OrderBy(a => a.RightIndex)
            .Select(added =>
            {
                Add(added.RightIndex);
                return (IListComparer<T>.IChange)(added with { RightIndex = _rightIndices[added.RightIndex] });
            });

    private IEnumerable<IListComparer<T>.IChange> Moved(List<IListComparer<T>.IChange> allChanges) =>
        allChanges
            .OfType<IListComparer<T>.Moved>()
            .OrderBy(a => a.RightIndex)
            .Select(moved =>
            {
                Add(moved.RightIndex);
                Delete(moved.RightIndex);
                return (IListComparer<T>.IChange)(moved with
                {
                    LeftIndex = _leftIndices[moved.LeftIndex], RightIndex = _rightIndices[moved.RightIndex]
                });
            });

    private IEnumerable<IListComparer<T>.IChange> Deleted(List<IListComparer<T>.IChange> allChanges) =>
        allChanges
            .OfType<IListComparer<T>.Deleted>()
            .OrderBy(a => a.LeftIndex)
            .Select(deleted =>
            {
                Delete(deleted.LeftIndex);
                return (IListComparer<T>.IChange)(deleted with { LeftIndex = _leftIndices[deleted.LeftIndex] });
            });

    private void Add(RightIndex rightIndex)
    {
        var leftIndex = _rightIndices[rightIndex];
        _rightIndices[rightIndex] = rightIndex;
        Increment(leftIndex, _leftIndices);
        Increment(rightIndex + 1, _rightIndices);
    }

    private void Delete(LeftIndex leftIndex)
    {
        var rightIndex = _leftIndices[leftIndex];
        Decrement(leftIndex + 1, _leftIndices);
        Decrement(rightIndex + 1, _rightIndices);
    }

    private void Increment(int lowerBound, Dictionary<int, int> dictionary)
    {
        for (int i = lowerBound; i < dictionary.Count; i++)
            dictionary[i] += 1;
    }

    private void Decrement(int lowerBound, Dictionary<int, int> dictionary)
    {
        for (int i = lowerBound; i < dictionary.Count; i++)
            dictionary[i] -= 1;
    }
}