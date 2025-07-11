﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

using CostBase = short;
using LeftIndex = Index;
using RightIndex = Index;

/// <inheritdoc />
/// <remarks>
/// Implements the Hirschberg O(n) space algorithm for finding an alignment.
/// Inspired by David Powell's Java implementation at https://github.com/drpowell/AlignDemo/blob/master/Hirschberg.java
/// </remarks>
public class ListComparer<T> : IListComparer<T>
{
    private readonly List<T> _left;
    private readonly List<T> _right;
    private readonly IEqualityComparer<T> _comparer;

    private readonly Cost[,] _matrix;
    private readonly Cost[,] _forwardMatrix;
    private readonly Cost[,] _reverseMatrix;

    private Cost _editCost;
    private int _cellsComputed;
    private readonly List<T> _alignLeft;
    private readonly List<T> _alignRight;
    private readonly SortedList<RightIndex, (T, RightIndex)> _added = [];
    private readonly SortedList<LeftIndex, (T, LeftIndex)> _deleted = [];

    /// <summary>
    /// Compares two lists, and returns the minimum number of <see cref="IListComparer{T}.IChange">changes</see>
    /// to convert <paramref name="left"/> into <paramref name="right"/>. 
    /// </summary>
    public ListComparer(List<T> left, List<T> right, IEqualityComparer<T>? comparer = null)
    {
        _left = left;
        _right = right;
        _comparer = comparer ?? EqualityComparer<T>.Default;

        LeftIndex leftSize = left.Count + 1;
        RightIndex rightSize = right.Count + 1;
        _matrix = new Cost[leftSize, rightSize];
        _forwardMatrix = new Cost[rightSize, rightSize];
        _reverseMatrix = new Cost[rightSize, rightSize];

        _editCost = Cost.Default;
        _cellsComputed = 0;
        _alignLeft = [];
        _alignRight = [];
    }

    /// <inheritdoc />
    public List<IListComparer<T>.IChange> Compare()
    {
        // Console.WriteLine("Hirschberg" + " running.");
        CompareInternal();
        // Console.WriteLine("Hirschberg" + " done. Cost=" + _editCost + "\n");
        // Console.WriteLine(TraceBack());
        // Console.WriteLine("\n" + "Cells computed = " + _cellsComputed);
        //
        // Console.WriteLine($"added:\n  {Join(_added)}");
        // Console.WriteLine($"deleted:\n  {Join(_deleted)}");

        var result = CollectChanges();

        // Console.WriteLine($"\nchanges:\n  {string.Join("\n  ", result)}");

        return result;

        string Join(SortedList<int, (T, int)> list)
        {
            return string.Join("\n  ", list.Values.Select(v => $"{v.Item1} @ {v.Item2}"));
        }
    }

    /// The original algorithm only detects additions and deletions.
    /// We replace matching add/delete pairs by one move change, and keep the remaining additions and deletions.
    private List<IListComparer<T>.IChange> CollectChanges()
    {
        List<IListComparer<T>.IChange> result = [];
        foreach ((T, RightIndex) addedEntry in _added.Values.ToList())
        {
            (T, LeftIndex) deletedEntry = _deleted.Values.FirstOrDefault(p => Equals(p.Item1, addedEntry.Item1));
            if (!deletedEntry.Equals(default))
            {
                LeftIndex indexOfValue = _deleted.IndexOfValue(deletedEntry);
                T leftElement = deletedEntry.Item1;
                LeftIndex leftIndex = deletedEntry.Item2;
                T rightElement = addedEntry.Item1;
                RightIndex rightIndex = addedEntry.Item2;

                if (leftIndex != rightIndex || !Equals(leftElement, rightElement))
                    result.Add(new IListComparer<T>.Moved(leftElement, leftIndex, rightElement, rightIndex));
                _added.RemoveAt(_added.IndexOfValue(addedEntry));
                _deleted.RemoveAt(indexOfValue);
            }
        }

        result.AddRange(_added.Values.Select(c => new IListComparer<T>.Added(c.Item1, c.Item2)).Cast<IListComparer<T>.IChange>());
        result.AddRange(_deleted.Values.Select(c => new IListComparer<T>.Deleted(c.Item1, c.Item2)).Cast<IListComparer<T>.IChange>());

        return result;
    }

    private void CompareInternal()
    {
        if (_left.SequenceEqual(_right, _comparer))
            return;

        _matrix[0, 0] = Cost.Align;
        _matrix[_left.Count, _right.Count] = Cost.Align;
        Align(0, _left.Count, 0, _right.Count);
    }

    private T CreateEmpty() => default!;

    private void Align(LeftIndex lowerBoundLeft, LeftIndex upperBoundLeft, RightIndex lowerBoundRight,
        RightIndex upperBoundRight)
    {
        // First the base cases...

        if (upperBoundLeft <= lowerBoundLeft)
        {
            // left is empty
            for (RightIndex rightIndex = lowerBoundRight; rightIndex < upperBoundRight; rightIndex++)
            {
                _alignLeft.Add(CreateEmpty());
                T rightElement = _right[rightIndex];
                _alignRight.Add(rightElement);
                Add(rightElement, rightIndex);
                _matrix[lowerBoundLeft, rightIndex + 1] = Cost.Align;
            }

            return;
        }

        if (upperBoundRight <= lowerBoundRight)
        {
            // right is empty
            for (LeftIndex leftIndex = lowerBoundLeft; leftIndex < upperBoundLeft; leftIndex++)
            {
                T leftElement = _left[leftIndex];
                _alignLeft.Add(leftElement);
                _alignRight.Add(CreateEmpty());
                Delete(leftElement, leftIndex);
                _matrix[leftIndex + 1, upperBoundRight] = Cost.Align;
            }

            return;
        }

        if (lowerBoundLeft + 1 == upperBoundLeft)
        {
            // left is exactly one entry
            T leftElement = _left[lowerBoundLeft];
            RightIndex memo = lowerBoundRight;
            for (RightIndex rightIndex = lowerBoundRight + 1; rightIndex < upperBoundRight; rightIndex++)
                if (Equals(_right[rightIndex], leftElement))
                    memo = rightIndex;
            // memo = an optimal cross point 
            for (RightIndex rightIndex = lowerBoundRight; rightIndex < upperBoundRight; rightIndex++)
            {
                T rightElement = _right[rightIndex];
                if (rightIndex == memo)
                {
                    _alignLeft.Add(leftElement);
                    if (!Equals(leftElement, rightElement))
                    {
                        Delete(leftElement, lowerBoundLeft);
                    }
                } else
                {
                    _alignLeft.Add(CreateEmpty());
                }

                if (!Equals(leftElement, rightElement))
                {
                    Add(rightElement, rightIndex);
                }

                _alignRight.Add(rightElement);

                if (rightIndex < memo)
                    _matrix[lowerBoundLeft, rightIndex + 1] = Cost.Align;
                else
                    _matrix[lowerBoundLeft + 1, rightIndex + 1] = Cost.Align;
            }

            return;
        }

        // Done with the base cases.  
        // Now the general case. Divide and conquer!
        LeftIndex midBoundLeft = (LeftIndex)Math.Floor((double)((lowerBoundLeft + upperBoundLeft) / 2));
        FwdDpa(lowerBoundLeft, midBoundLeft, lowerBoundRight, upperBoundRight);
        RevDpa(midBoundLeft, upperBoundLeft, lowerBoundRight, upperBoundRight);

        RightIndex midBoundRight = lowerBoundRight;
        Cost bestCost = Cost.Infinity;
        // Find the cheapest split
        for (RightIndex rightIndex = lowerBoundRight; rightIndex <= upperBoundRight; rightIndex++)
        {
            Cost sumCost = _forwardMatrix[midBoundLeft % 2, rightIndex]
                .Add(_reverseMatrix[midBoundLeft % 2, rightIndex]);
            if (sumCost < bestCost)
            {
                bestCost = sumCost;
                midBoundRight = rightIndex;
            }
        }

        if (_editCost == Cost.Default)
            _editCost = bestCost;

        // Mark the matrices unused...
        for (RightIndex rightIndex = lowerBoundRight; rightIndex <= upperBoundRight; rightIndex++)
        {
            UnusedCell(midBoundLeft, rightIndex);
            UnusedCell(midBoundLeft - 1, rightIndex);
            UnusedCell(midBoundLeft + 1, rightIndex);
        }

        _matrix[midBoundLeft, midBoundRight] = Cost.Align;

        // Recurse on the two halves...
        Align(lowerBoundLeft, midBoundLeft, lowerBoundRight, midBoundRight);
        Align(midBoundLeft, upperBoundLeft, midBoundRight, upperBoundRight);
    }

    private void Delete(T leftElement, LeftIndex leftIndex) =>
        _deleted.Add(leftIndex, (leftElement, leftIndex));

    private void Add(T rightElement, RightIndex rightIndex) =>
        _added.Add(rightIndex, (rightElement, rightIndex));

    private void FwdDpa(LeftIndex lowerBoundLeft, LeftIndex upperBoundLeft, RightIndex lowerBoundRight,
        RightIndex upperBoundRight)
    {
        _forwardMatrix[lowerBoundLeft % 2, lowerBoundRight] = Cost.Zero;
        StoreCell(lowerBoundLeft, lowerBoundRight);
        _cellsComputed++;

        // Setup the first row
        for (RightIndex rightIndex = lowerBoundRight + 1; rightIndex <= upperBoundRight; rightIndex++)
        {
            _forwardMatrix[lowerBoundLeft % 2, rightIndex] = _forwardMatrix[lowerBoundLeft % 2, rightIndex - 1].Inc();
            StoreCell(lowerBoundLeft, rightIndex);
            _cellsComputed++;
        }

        for (LeftIndex leftIndex = lowerBoundLeft + 1; leftIndex <= upperBoundLeft; leftIndex++)
        {
            // Mark the row to be calculated as unused.
            if (leftIndex >= lowerBoundLeft + 2)
                for (RightIndex rightIndex = lowerBoundRight; rightIndex <= upperBoundRight; rightIndex++)
                    UnusedCell(leftIndex - 2, rightIndex);

            _forwardMatrix[leftIndex % 2, lowerBoundRight] = _forwardMatrix[(leftIndex - 1) % 2, lowerBoundRight].Inc();
            StoreCell(leftIndex, lowerBoundRight);
            _cellsComputed++;
            for (RightIndex rightIndex = lowerBoundRight + 1; rightIndex <= upperBoundRight; rightIndex++)
            {
                Cost a = _forwardMatrix[(leftIndex - 1) % 2, rightIndex]
                    .Inc();

                Cost b = _forwardMatrix[leftIndex % 2, rightIndex - 1]
                    .Inc();

                Cost c = _forwardMatrix[(leftIndex - 1) % 2, rightIndex - 1]
                    //match/mismatch
                    .Add(OneIfMismatch(leftIndex - 1, rightIndex - 1));

                _forwardMatrix[leftIndex % 2, rightIndex] = Min(a, b, c);
                StoreCell(leftIndex, rightIndex);
                _cellsComputed++;
            }
        }
    }

    private void RevDpa(LeftIndex lowerBoundLeft, LeftIndex upperBoundLeft, RightIndex lowerBoundRight,
        RightIndex upperBoundRight)
    {
        _reverseMatrix[upperBoundLeft % 2, upperBoundRight] = Cost.Zero;
        StoreCell(upperBoundLeft, upperBoundRight);
        _cellsComputed++;

        // Setup the first row
        for (RightIndex rightIndex = upperBoundRight - 1; rightIndex >= lowerBoundRight; rightIndex--)
        {
            _reverseMatrix[upperBoundLeft % 2, rightIndex] = _reverseMatrix[upperBoundLeft % 2, rightIndex + 1].Inc();
            StoreCell(upperBoundLeft, rightIndex);
            _cellsComputed++;
        }

        for (LeftIndex leftIndex = upperBoundLeft - 1; leftIndex >= lowerBoundLeft; leftIndex--)
        {
            // Mark the row to be calculated as unused.
            if (leftIndex <= upperBoundLeft - 2)
                for (RightIndex rightIndex = upperBoundRight; rightIndex >= lowerBoundRight; rightIndex--)
                    UnusedCell(leftIndex + 2, rightIndex);

            _reverseMatrix[leftIndex % 2, upperBoundRight] = _reverseMatrix[(leftIndex + 1) % 2, upperBoundRight].Inc();
            StoreCell(leftIndex, upperBoundRight);
            _cellsComputed++;
            for (RightIndex rightIndex = upperBoundRight - 1; rightIndex >= lowerBoundRight; rightIndex--)
            {
                Cost a = _reverseMatrix[(leftIndex + 1) % 2, rightIndex]
                    .Inc();

                Cost b = _reverseMatrix[leftIndex % 2, rightIndex + 1]
                    .Inc();

                Cost c = _reverseMatrix[(leftIndex + 1) % 2, rightIndex + 1]
                    //match/mismatch
                    .Add(OneIfMismatch(leftIndex, rightIndex));

                _reverseMatrix[leftIndex % 2, rightIndex] = Min(a, b, c);
                StoreCell(leftIndex, rightIndex);
                _cellsComputed++;
            }
        }
    }

    private void UnusedCell(LeftIndex leftIndex, RightIndex rightIndex)
    {
        if (_matrix[leftIndex, rightIndex] != Cost.Align)
            _matrix[leftIndex, rightIndex] = Cost.Infinity;
    }

    private void StoreCell(LeftIndex leftIndex, RightIndex rightIndex)
    {
        Cost c = _matrix[leftIndex, rightIndex];
        Cost col = Cost.Store;
        if (c == Cost.Align)
            col = Cost.Align;
        _matrix[leftIndex, rightIndex] = col;
    }


    private string TraceBack() =>
        string.Join(",", _alignLeft) + "\n" + string.Join(",", _alignRight);

    private bool Equals(T left, T right) =>
        _comparer.Equals(left, right);

    private Cost OneIfMismatch(LeftIndex leftIndex, RightIndex rightIndex) =>
        Equals(_left[leftIndex], _right[rightIndex]) ? Cost.Zero : Cost.One;

    private Cost Min(Cost a, Cost b, Cost c) =>
        new[] { a, b, c }.Min();
}

internal enum Cost : CostBase
{
    Default = -1,
    Zero = 0,
    One = 1,
    Store = 2,
    Align = 4,
    Infinity = CostBase.MaxValue
}

internal static class CostExtensions
{
    public static Cost Add(this Cost self, Cost other) =>
        checked(self + (CostBase)other);

    public static Cost Inc(this Cost self) =>
        self.Add(Cost.One);
}