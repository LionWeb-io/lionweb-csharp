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

using LeftIndex = int;
using RightIndex = int;

/// <inheritdoc />
/// <remarks>
/// <see cref="ListComparer{T}"/> reports <i>all</i> changes based on the original and result indices.
/// This class converts them into stepwise changes, i.e. the indices in each step are based on all previous changes. 
/// </remarks>
public class StepwiseListComparer<T> : IListComparer<T>
{
    private readonly IListComparer<T> _listComparer;

    private List<IListComparer<T>.IChange> _allChanges;

    /// <inheritdoc cref="ListComparer{T}(List{T}, List{T}, IEqualityComparer{T}?)"/>
    public StepwiseListComparer(List<T> left, List<T> right, IEqualityComparer<T>? comparer = null)
        : this(Math.Max(left.Count, right.Count), right.Count - left.Count, new ListComparer<T>(left, right, comparer))
    {
    }

    public StepwiseListComparer(int maxSize, int delta, IListComparer<T> listComparer)
    {
        _listComparer = listComparer;
    }

    interface Pseudo;

    record PseudoAdded(IListComparer<T>.Moved Moved)
        : IListComparer<T>.Added(Moved.Element, Moved.RightIndex), Pseudo
    {
        public override RightIndex RightIndex { get => Moved.RightIndex; set => Moved.RightIndex = value; }
    }

    record PseudoDeleted(IListComparer<T>.Moved Moved)
        : IListComparer<T>.Deleted(Moved.Element, Moved.LeftIndex), Pseudo
    {
        public override LeftIndex LeftIndex { get => Moved.LeftIndex; set => Moved.LeftIndex = value; }
    }

    /// We need to make additions and deletions comparable.
    /// For this, we make the longest list possible, with all the additions executed,
    /// but none of the deletions yet.
    private List<IListComparer<T>.IChange> AllInOne(List<IListComparer<T>.IChange> inputChanges)
    {
        List<IListComparer<T>.IChange> result = [];

        var remainingChanges = inputChanges.ToList();

        var leftMostChange = LeftMostChange();

        List<IListComparer<T>.Moved> moveAdjustments = [];

        while (leftMostChange != null)
        {
            remainingChanges.Remove(leftMostChange);

            var delta = 0;
            var compareIndex = leftMostChange.Index;// - (leftMostChange is IListComparer<T>.ILeftChange ? 1 : 0);
            foreach (var move in moveAdjustments
                         .Where(m => m.RightEdge < compareIndex)
                         .ToList()
                    )
            {
                delta += move.MoveLeftToRight ? -1 : +1;
                moveAdjustments.Remove(move);
            }

            leftMostChange.Index += delta;

            delta++;

            if (leftMostChange is IListComparer<T>.Added)
            {
                foreach (var d in remainingChanges
                             .OfType<IListComparer<T>.ILeftChange>())
                {
                    d.LeftIndex += delta;
                }

                for (var i = 0; i < moveAdjustments.Count; i++)
                {
                    var move = moveAdjustments[i];
                    if (move.MoveRightToLeft)
                        move.LeftIndex += delta;
                }
            }

            if (leftMostChange is IListComparer<T>.Deleted)
            {
                foreach (var a in remainingChanges
                             .OfType<IListComparer<T>.IRightChange>()
                        )
                {
                    a.RightIndex += delta;
                }

                for (var i = 0; i < moveAdjustments.Count; i++)
                {
                    var move = moveAdjustments[i];
                    if (move.MoveLeftToRight)
                        move.RightIndex += delta;
                }
            }

            if (leftMostChange is IListComparer<T>.Moved moved)
            {
                moveAdjustments.Add(moved);
                // if (moved.Index == moved.LeftIndex)
                // {
                //     moveAdjustments.Add((moved.RightIndex, moved));
                // } else
                // {
                //     moveAdjustments.Add((moved.LeftIndex, moved));
                // }
            }

            if (leftMostChange is not Pseudo)
                result.Add(leftMostChange);

            leftMostChange = LeftMostChange();
        }

        return result;

        IListComparer<T>.IChange? LeftMostChange()
        {
            return remainingChanges
                .OrderBy(c => c.Index)
                .ThenBy(c => c is IListComparer<T>.ILeftChange ? 0 : 1)
                .FirstOrDefault();
        }
    }

    /// <inheritdoc />
    public List<IListComparer<T>.IChange> Compare()
    {
        _allChanges = _listComparer.Compare();

        Console.WriteLine("allChanges: \n" + string.Join("\n", _allChanges));

        var allInOne = AllInOne(_allChanges);

        Console.WriteLine("\nallInOne: \n" + string.Join("\n", allInOne));

        List<IListComparer<T>.IChange> result = [];

        int delta = 0;

        List<(int, int)> deltaAdjustments = [];

        foreach (var change in allInOne)
        {
            Console.WriteLine("\ncurrent: " + change);

            foreach (var adjustment in deltaAdjustments.Where(da => da.Item1 < change.Index).ToList())
            {
                delta += adjustment.Item2;
                deltaAdjustments.Remove(adjustment);
            }

            if (change is IListComparer<T>.Deleted deleted)
            {
                deleted.LeftIndex -= delta;
            }

            if (change is IListComparer<T>.Added added)
            {
                added.RightIndex -= delta;
            }

            if (change is IListComparer<T>.Deleted)
                delta++;

            if (change is IListComparer<T>.Moved moved)
            {
                if (moved.Index == moved.LeftIndex)
                {
                    // deltaAdjustments.Add((moved.RightIndex, +1));
                    delta++;
                } else
                {
                    delta++;
                    deltaAdjustments.Add((moved.RightIndex, -1));
                }
            }

            result.Add(change);

            Console.WriteLine(string.Join("\n", result));
        }
        
        Console.WriteLine("\nbeforeMoveAdjustment: \n" + string.Join("\n", result));

        
        for (var i = 0; i < result.Count; i++)
        {
            var r = result[i];
            if (r is IListComparer<T>.Added a)
            {
                var partnerDelete = result[i..].OfType<IListComparer<T>.Deleted>()
                    .FirstOrDefault(o => o.Element.Equals(r.Element));
                if (partnerDelete != null)
                {
                    
                }
            } else if (r is IListComparer<T>.Deleted d)
            {
                for (var j = i + 1; j < result.Count; j++)
                {
                    var partner = result[j];
                    if (partner is IListComparer<T>.Added p && d.Element.Equals(r.Element))
                    {
                        for (var k = j - 1; k > i; k--)
                        {
                            var intermediate = result[k];
                            if (intermediate is IListComparer<T>.Added interAdd)
                            {
                                interAdd.RightIndex += 1;
                                p.RightIndex -= 1;
                                result[k] = p;
                                result[k + 1] = intermediate;
                            }

                            if (intermediate is IListComparer<T>.Deleted interDel)
                            {
                                // interDel.LeftIndex -= 1;
                                p.RightIndex += 1;
                                result[k] = p;
                                result[k + 1] = intermediate;
                            }
                        }
                        
                    }
                }
            }

        }

        return result;
    }
}