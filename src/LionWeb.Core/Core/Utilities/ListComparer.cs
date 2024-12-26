// Copyright 2024 TRUMPF Laser SE and other contributors
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

using CostBase = sbyte;
using LeftIndex = int;
using RightIndex = int;

/// <remarks>
/// Implement the Hirschberg O(n) space algorithm for finding an alignment.
/// Inspired by David Powell's Java implementation at https://github.com/drpowell/AlignDemo/blob/master/Hirschberg.java
/// </remarks>
public class ListComparer<T>
{
    private readonly List<T> _left;
    private readonly List<T> _right;
    private readonly IComparer<T> _comparer;

    private readonly Cost[,] _matrix;
    private readonly Cost[,] _forwardMatrix;
    private readonly Cost[,] _reverseMatrix;

    private Cost _editCost;
    private int _cellsComputed;
    private readonly List<T> _alignLeft;
    private readonly List<T> _alignRight;
    private readonly SortedList<RightIndex, (T, RightIndex)> _added = [];
    private readonly SortedList<LeftIndex, (T, LeftIndex)> _removed = [];

    /// <summary>
    /// Compares two lists, and returns the minimum number of <see cref="ListChange">changes</see>
    /// to convert <paramref name="left"/> into <paramref name="right"/>. 
    /// </summary>
    public ListComparer(List<T> left, List<T> right, IComparer<T>? comparer = null)
    {
        _left = left;
        _right = right;
        _comparer = comparer ?? Comparer<T>.Default;

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

    public List<ListChange> Compare()
    {
        CompareInternal();
        return CollectChanges();
    }

    public List<ListChange> Run()
    {
        Console.WriteLine("Hirschberg" + " running.");
        CompareInternal();

        Console.WriteLine("Hirschberg" + " done. Cost=" + _editCost + "\n");
        Console.WriteLine(TraceBack());
        Console.WriteLine("\n" + "Cells computed = " + _cellsComputed);

        Console.WriteLine($"added:\n  {Join(_added)}");
        Console.WriteLine($"removed:\n  {Join(_removed)}");

        var result = CollectChanges();

        Console.WriteLine($"\nchanges:\n  {string.Join("\n  ", result)}");

        return result;

        string Join(SortedList<int, (T, int)> list)
        {
            return string.Join("\n  ", list.Values.Select(v => $"{v.Item1} @ {v.Item2}"));
        }
    }

    private List<ListChange> CollectChanges()
    {
        List<ListChange> result = [];
        foreach ((T, RightIndex) addedEntry in _added.Values.ToList())
        {
            (T, LeftIndex) removedEntry = _removed.Values.FirstOrDefault(p => Equals(p.Item1, addedEntry.Item1));
            if (!removedEntry.Equals(default))
            {
                LeftIndex indexOfValue = _removed.IndexOfValue(removedEntry);
                result.Add(new ListMoved(removedEntry.Item1, removedEntry.Item2, addedEntry.Item1, addedEntry.Item2));
                _added.RemoveAt(_added.IndexOfValue(addedEntry));
                _removed.RemoveAt(indexOfValue);
            }
        }

        result.AddRange(_added.Values.Select(c => new ListAdded(c.Item1, c.Item2)).Cast<ListChange>());
        result.AddRange(_removed.Values.Select(c => new ListRemoved(c.Item1, c.Item2)).Cast<ListChange>());

        return result;
    }

    private void CompareInternal()
    {
        // mat.clearAlignment();
        _matrix[0, 0] = Cost.Align;
        _matrix[_left.Count, _right.Count] = Cost.Align;
        Align(0, _left.Count, 0, _right.Count);
    }

    private T CreateEmpty() => default!;

    private void Align(LeftIndex lowerBoundLeft, LeftIndex upperBoundLeft, RightIndex lowerBoundRight,
        RightIndex upperBoundRight)
    {
        // Align s1[p1..p2) with p2[q1..q2)
        //System.out.println("s1["+p1+".."+(p2-1)+"] : s2["+q1+".."+(q2-1)+"]");

        // First the base cases...

        if (upperBoundLeft <= lowerBoundLeft)
        {
            // s1 is empty
            for (RightIndex rightIndex = lowerBoundRight; rightIndex < upperBoundRight; rightIndex++)
            {
                _alignLeft.Add(CreateEmpty());
                T rightElement = _right[rightIndex];
                _alignRight.Add(rightElement);
                Add(rightElement, rightIndex);
                _matrix[lowerBoundLeft, rightIndex + 1] = Cost.Align;
                // mat.addAlignment(p1,j,p1,j+1);
            }

            return;
        }

        if (upperBoundRight <= lowerBoundRight)
        {
            // s2 is empty
            for (LeftIndex leftIndex = lowerBoundLeft; leftIndex < upperBoundLeft; leftIndex++)
            {
                T leftElement = _left[leftIndex];
                _alignLeft.Add(leftElement);
                _alignRight.Add(CreateEmpty());
                Remove(leftElement, leftIndex);
                _matrix[leftIndex + 1, upperBoundRight] = Cost.Align;
                // mat.addAlignment(i,q2,i+1,q2);
            }

            return;
        }

        if (lowerBoundLeft + 1 == upperBoundLeft)
        {
            // s1 is exactly one character
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
                        Add(rightElement, lowerBoundLeft);
                } else
                {
                    _alignLeft.Add(CreateEmpty());
                    if (!Equals(leftElement, rightElement))
                        Remove(leftElement, lowerBoundLeft);
                }

                _alignRight.Add(rightElement);

                if (rightIndex < memo)
                    _matrix[lowerBoundLeft, rightIndex + 1] = Cost.Align;
                else
                    _matrix[lowerBoundLeft + 1, rightIndex + 1] = Cost.Align;

                // mat.addAlignment((j<=memo ? p1 : p1+1),j,
                // 		 (j<memo? p1 : p1+1),j+1);
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
        //mat.addAlignment(mid,s2mid, mid+1, s2mid+1);

        // Recurse on the two halves...
        Align(lowerBoundLeft, midBoundLeft, lowerBoundRight, midBoundRight);
        Align(midBoundLeft, upperBoundLeft, midBoundRight, upperBoundRight);
    }

    private void Remove(T leftElement, LeftIndex leftIndex) =>
        _removed.Add(leftIndex, (leftElement, leftIndex));

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
        // c.setVal(v,col);
        _matrix[leftIndex, rightIndex] = col;
    }


    private string TraceBack() =>
        string.Join(",", _alignLeft) + "\n" + string.Join(",", _alignRight);

    private bool Equals(T left, T right) =>
        _comparer.Compare(left, right) == 0;

    private Cost OneIfMismatch(LeftIndex leftIndex, RightIndex rightIndex) =>
        Equals(_left[leftIndex], _right[rightIndex]) ? Cost.Zero : Cost.One;

    private Cost Min(Cost a, Cost b, Cost c) =>
        new[] { a, b, c }.Min();

    public interface ListChange;

    public record struct ListAdded(T Element, RightIndex RightIndex) : ListChange;

    public record struct ListRemoved(T Element, LeftIndex LeftIndex) : ListChange;

    public record struct ListMoved(T LeftElement, LeftIndex LeftIndex, T RightElement, RightIndex RightIndex)
        : ListChange;
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