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

using M3;
using LeftIndex = int;
using RightIndex = int;

/// <inheritdoc />
/// <remarks>
/// <see cref="ListComparer{T}"/> reports <i>all</i> changes based on the original and result indices.
/// This class converts them into stepwise changes, i.e. the indices in each step are based on all previous changes. 
/// </remarks>
public class StepwiseListComparer<T> : IListComparer<T>
{
    private int _delta;
    private readonly IListComparer<T> _listComparer;

    private readonly Dictionary<LeftIndex, RightIndex> _leftIndices;
    private readonly Dictionary<RightIndex, LeftIndex> _rightIndices;

    private readonly Map<LeftIndex, RightIndex> _indices;
    private List<IListComparer<T>.IChange> _allChanges;

    /// <inheritdoc cref="ListComparer{T}(List{T}, List{T}, IEqualityComparer{T}?)"/>
    public StepwiseListComparer(List<T> left, List<T> right, IEqualityComparer<T>? comparer = null)
     : this(Math.Max(left.Count, right.Count), right.Count- left.Count, new ListComparer<T>(left, right, comparer))
    {
    }

    public StepwiseListComparer(int maxSize, int delta, IListComparer<T> listComparer)
    {
        _delta = delta;
        _listComparer = listComparer;

        _leftIndices = new Dictionary<LeftIndex, RightIndex>(maxSize);
        _rightIndices = new Dictionary<RightIndex, LeftIndex>(maxSize);
        _indices = new Map<LeftIndex, RightIndex>();

        for (var i = 0; i < maxSize; i++)
        {
            _leftIndices.Add(i, i);
            _rightIndices.Add(i, i);
            // _indices.Add(i, i);
        }
    }

    /// <inheritdoc />
    public List<IListComparer<T>.IChange> Compare()
    {
        _allChanges = _listComparer.Compare();
        
        Console.WriteLine("allChanges: \n"+ string.Join("\n", _allChanges));
        
        var changes = _allChanges
            .Select(c => (IListComparer<T>.IChange) c.Clone())
            .ToList();

        for (int i = 0; i < changes.Count; i++)
        {
            var change = changes[i];

            Console.WriteLine("\ncurrent: "+ change);

            switch (change)
            {
                case IListComparer<T>.Added added:
                    for (int j = 0; j < i; j++)
                    {
                        var change1 = changes[j];
                        if (change1 is IListComparer<T>.IRightChange right && added.RightIndex < right.RightIndex && right.RightIndex > 0)
                            right.RightIndex -= 1;
                    }
            
                    for (int j = i + 1; j < changes.Count; j++)
                    {
                        var change2 = changes[j];
                        if (change2 is IListComparer<T>.ILeftChange left && added.RightIndex - _delta < left.LeftIndex)
                            left.LeftIndex += 1;
                    }

                    _delta++;
                    break;
                
                case IListComparer<T>.Deleted deleted:
                    _delta--;
                    for (int j = 0; j < i; j++)
                    {
                        var change1 = changes[j];
                        if (change1 is IListComparer<T>.IRightChange right && deleted.LeftIndex + _delta < right.RightIndex)
                            right.RightIndex += 1;
                    }
            
                    for (int j = i + 1; j < changes.Count; j++)
                    {
                        var change2 = changes[j];
                        if (change2 is IListComparer<T>.ILeftChange left && deleted.LeftIndex < left.LeftIndex && left.LeftIndex > 0)
                            left.LeftIndex -= 1;
                    }

                    break;
                
                case IListComparer<T>.Moved moved:
                    // changes after me
                    for (int j = i + 1; j < changes.Count; j++)
                    {
                        var change2 = changes[j];
                        if (change2 is IListComparer<T>.ILeftChange moveAfterMeLeft)
                        {
                            if (moved.RightIndex > moveAfterMeLeft.LeftIndex && moved.LeftIndex < moveAfterMeLeft.LeftIndex) 
                                moveAfterMeLeft.LeftIndex -= 1;
                        }
                        if (change2 is IListComparer<T>.Moved moveAfterMeRight)
                        {
                            if (moved.RightIndex <= moveAfterMeRight.LeftIndex && moved.LeftIndex > moveAfterMeRight.RightIndex)
                                moveAfterMeRight.LeftIndex += 1;
                        }
                        // if (change2 is IListComparer<T>.Moved moveAfterMe)
                        // {
                        //     if (moved.RightIndex <= moveAfterMe.LeftIndex && moved.LeftIndex > moveAfterMe.RightIndex)
                        //         moveAfterMe.LeftIndex += 1;
                        //     if (moved.RightIndex > moveAfterMe.LeftIndex && moved.LeftIndex < moveAfterMe.LeftIndex) 
                        //         moveAfterMe.LeftIndex -= 1;
                        // }
                    }

                    // changes before me
                    for (int j = 0; j < i; j++)
                    {
                        var change2 = changes[j];
                        if (change2 is IListComparer<T>.Moved moveBeforeMe)
                        {
                            if (moved.RightIndex <= moveBeforeMe.RightIndex && moved.LeftIndex > moveBeforeMe.RightIndex)
                                moveBeforeMe.RightIndex -= 1;
                            if (moved.RightIndex > moveBeforeMe.RightIndex && moved.LeftIndex < moveBeforeMe.LeftIndex) 
                                moveBeforeMe.LeftIndex += 1;
                        }
                    }

                    break;
            }
            
            Console.WriteLine(string.Join("\n", changes));
        }

        List<IListComparer<T>.IChange> result = [];
        
        for (var i = 0; i < changes.Count; i++)
        {
            var change = changes[i];
            if (i < changes.Count - 1 && false)
            {
                var nextChange = changes[i + 1];
                if (change is IListComparer<T>.Added added && nextChange is IListComparer<T>.Deleted deleted)
                {
                    if (added.Element.Equals(deleted.Element))
                    {
                        LeftIndex leftIndex = deleted.LeftIndex;
                        RightIndex rightIndex = added.RightIndex;
                        
                        if (leftIndex > rightIndex)
                            leftIndex -= 1;
                        else
                            rightIndex -= 1;
                        // aBcdf
                        // 01234
                        
                        result.Add(new IListComparer<T>.Moved(deleted.Element, leftIndex, added.Element, rightIndex));
                        i++;
                        continue;
                    }
                }
            }
            
            result.Add(change);
        }
        
        return result;

        void AdjustRight(int i, RightIndex ownIndex)
        {
            for (int j = 0; j < i; j++)
            {
                var change = changes[j];
                if (change is IListComparer<T>.IRightChange right && ownIndex < right.RightIndex && right.RightIndex > 0)
                    right.RightIndex -= 1;
            }
            
            for (int j = i + 1; j < changes.Count; j++)
            {
                var change = changes[j];
                if (change is IListComparer<T>.ILeftChange left && ownIndex - _delta <= left.LeftIndex)
                    left.LeftIndex += 1;
            }
        }
        
        void AdjustLeft(int i, LeftIndex ownIndex)
        {
            for (int j = 0; j < i; j++)
            {
                var change = changes[j];
                if (change is IListComparer<T>.IRightChange right && ownIndex + _delta <= right.RightIndex)
                    right.RightIndex += 1;
            }
            
            for (int j = i + 1; j < changes.Count; j++)
            {
                var change = changes[j];
                if (change is IListComparer<T>.ILeftChange left && ownIndex < left.LeftIndex && left.LeftIndex > 0)
                    left.LeftIndex -= 1;
            }
        }
    }

    private IListComparer<T>.IChange Added(IListComparer<T>.Added added)
    {
        Add(added.RightIndex);
        return added with { RightIndex = _rightIndices[added.RightIndex] - 1 };
    }

    private IListComparer<T>.IChange Replaced(IListComparer<T>.Replaced replaced) =>
        replaced with { LeftIndex = _leftIndices[replaced.LeftIndex] };

    private IListComparer<T>.IChange Moved(IListComparer<T>.Moved moved)
    {
        if (moved.LeftIndex <= moved.RightIndex)
        {
            Delete(moved.LeftIndex, moved.RightIndex);
        } else
        {
            Add(moved.LeftIndex, moved.RightIndex);
        }
        
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
        _rightIndices[lowerBoundInclusive] = leftLowerBoundInclusive + 1;
        Increment(leftLowerBoundInclusive, _leftIndices, leftUpperBoundExclusive);
        // Decrement(lowerBoundInclusive + 1, _rightIndices, upperBoundExclusive);
    }

    private void Delete(LeftIndex lowerBoundInclusive, LeftIndex? upperBoundExclusive = null)
    {
        RightIndex rightLowerBoundInclusive = _leftIndices[lowerBoundInclusive];
        RightIndex? rightUpperBoundExclusive = upperBoundExclusive != null ? _leftIndices[(LeftIndex)upperBoundExclusive] : null;
        Decrement(lowerBoundInclusive + 1, _leftIndices, upperBoundExclusive);
        // Increment(rightLowerBoundInclusive, _rightIndices, rightUpperBoundExclusive);
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
    
    private void Increment(int lowerBoundInclusive, Map<int, int> map, int? upperBoundExclusive = null)
    {
        int upperBound = upperBoundExclusive ?? map.Count;
        for (int i = lowerBoundInclusive; i < upperBound; i++)
            map.Put(i, map.Forward[i] + 1);
    }

    private void Decrement(int lowerBoundInclusive, Map<int, int> map, int? upperBoundExclusive = null)
    {
        int upperBound = upperBoundExclusive ?? map.Count;
        for (int i = lowerBoundInclusive; i < upperBound; i++)
            map.Put(i, map.Forward[i] - 1);
    }
    
    public class Map<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map()
        {
            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }
        
        public void Put(T1 t1, T2 t2)
        {
            _forward[t1] = t2;
            _reverse[t2] = t1;
        }
        
        public int Count => _forward.Count;

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }
    }
    
}