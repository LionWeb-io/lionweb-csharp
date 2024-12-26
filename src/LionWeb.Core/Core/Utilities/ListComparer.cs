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

// Hirschberg - implement the Hirschberg O(n) space algorithm
// for finding an alignment.  Code translated from Lloyd's javascript
// version: http://www.csse.monash.edu.au/~lloyd/tildeAlgDS/Dynamic/Hirsch.html
public class ListComparer<T>
{
    private const int _unusedCol = 0;
    private const int _storeCol = 2;
    private const int _alignCol = 4;
    private const int _infinity = int.MaxValue;

    private readonly int[][] _fwdMat;
    private readonly int[][] _revMat;
    private readonly List<T> _align1;
    private readonly List<T> _align2;

    private readonly SortedList<int, (T, int)> _added = [];
    private readonly SortedList<int, (T, int)> _removed = [];
    private readonly int[,] _mat;
    private readonly List<T> _s1;
    private readonly List<T> _s2;
    private readonly IComparer<T>? _comparer;
    private int _editCost;
    private int _cellsComputed;

    public ListComparer(List<T> str1, List<T> str2, IComparer<T>? comparer = null)
    {
        _mat = new int[str1.Count + 1, str2.Count + 1];
        _s1 = str1;
        _s2 = str2;
        _comparer = comparer ?? Comparer<T>.Default;
        _editCost = -1;
        _cellsComputed = 0;
        _fwdMat = new int[][] { new int[_s2.Count + 1], new int[_s2.Count + 1] };
        _revMat = new int[][] { new int[_s2.Count + 1], new int[_s2.Count + 1] };
        _align1 = [];
        _align2 = [];
    }

    public List<ListChange> Compare()
    {
        CompareInternal();
        return CollectChanges();
    }

    private List<ListChange> CollectChanges()
    {
        List<ListChange> result = [];
        foreach ((T, int) addedEntry in _added.Values.ToList())
        {
            (T, int) removedEntry = _removed.Values.FirstOrDefault(p => Compare(p.Item1, addedEntry.Item1));
            if (!removedEntry.Equals(default))
            {
                var indexOfValue = _removed.IndexOfValue(removedEntry);
                result.Add(new ListMoved(removedEntry.Item1, removedEntry.Item2, addedEntry.Item1, addedEntry.Item2));
                _added.RemoveAt(_added.IndexOfValue(addedEntry));
                _removed.RemoveAt(indexOfValue);
            }
        }

        foreach (var c in _added.Values)
        {
            result.Add(new ListAdded(c.Item1, c.Item2));
        }

        foreach (var c in _removed.Values)
        {
            result.Add(new ListRemoved(c.Item1, c.Item2));
        }

        return result;
    }

    private void CompareInternal()
    {
        // mat.clearAlignment();
        _mat[0, 0] = _alignCol;
        _mat[_s1.Count, _s2.Count] = _alignCol;
        Align(0, _s1.Count, 0, _s2.Count);
    }

    private T CreateEmpty() => default;

    private void Align(int p1, int p2, int q1, int q2)
    {
        // Align s1[p1..p2) with p2[q1..q2)
        //System.out.println("s1["+p1+".."+(p2-1)+"] : s2["+q1+".."+(q2-1)+"]");

        // First the base cases...

        if (p2 <= p1)
        {
            // s1 is empty
            for (int j = q1; j < q2; j++)
            {
                _align1.Add(CreateEmpty());
                var s2Char = _s2[j];
                _align2.Add(s2Char);
                Add(s2Char, j);
                _mat[p1, j + 1] = _alignCol;
                // mat.addAlignment(p1,j,p1,j+1);
            }

            return;
        }

        if (q2 <= q1)
        {
            // s2 is empty
            for (int i = p1; i < p2; i++)
            {
                var s1Char = _s1[i];
                _align1.Add(s1Char);
                _align2.Add(CreateEmpty());
                Remove(s1Char, i);
                _mat[i + 1, q2] = _alignCol;
                // mat.addAlignment(i,q2,i+1,q2);
            }

            return;
        }

        if (p1 + 1 == p2)
        {
            // s1 is exactly one character
            T ch = _s1[p1];
            int memo = q1;
            for (int j = q1 + 1; j < q2; j++)
                if (Compare(_s2[j], ch))
                    memo = j;
            // memo = an optimal cross point 
            for (int j = q1; j < q2; j++)
            {
                var s2Char = _s2[j];
                if (j == memo)
                {
                    _align1.Add(ch);
                    if (!Compare(ch, s2Char))
                        Add(s2Char, p1);
                } else
                {
                    _align1.Add(CreateEmpty());
                    if (!Compare(ch, s2Char))
                        Remove(ch, p1);
                }


                _align2.Add(s2Char);

                if (j < memo)
                    _mat[p1, j + 1] = _alignCol;
                else
                    _mat[p1 + 1, j + 1] = _alignCol;

                // mat.addAlignment((j<=memo ? p1 : p1+1),j,
                // 		 (j<memo? p1 : p1+1),j+1);
            }

            return;
        }

        // Done with the base cases.  
        // Now the general case. Divide and conquer!
        int mid = (int)Math.Floor((double)((p1 + p2) / 2));
        FwdDpa(p1, mid, q1, q2);
        RevDpa(mid, p2, q1, q2);

        var s2Mid = q1;
        var best = _infinity;
        // Find the cheapest split
        for (int j = q1; j <= q2; j++)
        {
            int sum = _fwdMat[mid % 2][j] + _revMat[mid % 2][j];
            if (sum < best)
            {
                best = sum;
                s2Mid = j;
            }
        }

        if (_editCost == -1) _editCost = best;

        // Mark the matrices unused...
        for (int j = q1; j <= q2; j++)
        {
            UnusedCell(mid, j);
            UnusedCell(mid - 1, j);
            UnusedCell(mid + 1, j);
        }

        _mat[mid, s2Mid] = _alignCol;
        //mat.addAlignment(mid,s2mid, mid+1, s2mid+1);

        // Recurse on the two halves...
        Align(p1, mid, q1, s2Mid);
        Align(mid, p2, s2Mid, q2);
    }

    private void Remove(T s1Char, int index) =>
        _removed.Add(index, (s1Char, index));

    private void Add(T s1Char, int index) =>
        _added.Add(index, (s1Char, index));

    private void FwdDpa(int p1, int p2, int q1, int q2)
    {
        _fwdMat[p1 % 2][q1] = 0;
        StoreCell(p1, q1, 0);
        _cellsComputed++;

        // Setup the first row
        for (int j = q1 + 1; j <= q2; j++)
        {
            _fwdMat[p1 % 2][j] = _fwdMat[p1 % 2][j - 1] + 1;
            StoreCell(p1, j, _fwdMat[p1 % 2][j]);
            _cellsComputed++;
        }

        for (int i = p1 + 1; i <= p2; i++)
        {
            // Mark the row to be calculated as unused.
            if (i >= p1 + 2)
                for (int j = q1; j <= q2; j++)
                    UnusedCell(i - 2, j);

            _fwdMat[i % 2][q1] = _fwdMat[(i - 1) % 2][q1] + 1;
            StoreCell(i, q1, _fwdMat[i % 2][q1]);
            _cellsComputed++;
            for (int j = q1 + 1; j <= q2; j++)
            {
                _fwdMat[i % 2][j] = Min3(_fwdMat[(i - 1) % 2][j] + 1, //delete
                    _fwdMat[i % 2][j - 1] + 1, //insert
                    _fwdMat[(i - 1) % 2][j - 1] + //match/mismatch
                    (Compare(_s1[i - 1], _s2[j - 1]) ? 0 : 1));
                StoreCell(i, j, _fwdMat[i % 2][j]);
                _cellsComputed++;
            }
        }
    }

    private void RevDpa(int p1, int p2, int q1, int q2)
    {
        _revMat[p2 % 2][q2] = 0;
        StoreCell(p2, q2, 0);
        _cellsComputed++;

        // Setup the first row
        for (int j = q2 - 1; j >= q1; j--)
        {
            _revMat[p2 % 2][j] = _revMat[p2 % 2][j + 1] + 1;
            StoreCell(p2, j, _revMat[p2 % 2][j]);
            _cellsComputed++;
        }

        for (int i = p2 - 1; i >= p1; i--)
        {
            // Mark the row to be calculated as unused.
            if (i <= p2 - 2)
                for (int j = q2; j >= q1; j--)
                    UnusedCell(i + 2, j);

            _revMat[i % 2][q2] = _revMat[(i + 1) % 2][q2] + 1;
            StoreCell(i, q2, _revMat[i % 2][q2]);
            _cellsComputed++;
            for (int j = q2 - 1; j >= q1; j--)
            {
                _revMat[i % 2][j] = Min3(_revMat[(i + 1) % 2][j] + 1, //delete
                    _revMat[i % 2][j + 1] + 1, //insert
                    _revMat[(i + 1) % 2][j + 1] + //match/mismatch
                    (Compare(_s1[i], _s2[j]) ? 0 : 1));
                StoreCell(i, j, _revMat[i % 2][j]);
                _cellsComputed++;
            }
        }
    }

    private void UnusedCell(int i, int j)
    {
        if (_mat[i, j] != _alignCol)
            _mat[i, j] = int.MaxValue;
    }

    private void StoreCell(int i, int j, int v)
    {
        var c = _mat[i, j];
        int col = _storeCol;
        if (c == _alignCol) col = _alignCol;
        // c.setVal(v,col);
        _mat[i, j] = col;
    }


    private string TraceBack() =>
        string.Join(",", _align1) + "\n" + string.Join(",", _align2);

    public List<ListChange> Run()
    {
        Console.WriteLine("Hirschberg" + " running.");
        CompareInternal();

        Console.WriteLine("Hirschberg" + " done. Cost=" + _editCost + "\n");
        Console.WriteLine(TraceBack());
        Console.WriteLine("\n" + "Cells computed = " + _cellsComputed);

        Console.WriteLine($"added:\n  {string.Join("\n  ", _added.Values.Select(v => $"{v.Item1} @ {v.Item2}"))}");
        Console.WriteLine($"removed:\n  {string.Join("\n  ", _removed.Values.Select(v => $"{v.Item1} @ {v.Item2}"))}");

        var result = CollectChanges();
        
        Console.WriteLine($"\nchanges:\n  {string.Join("\n  ", result)}");

        return result;
    }

    private int Min3(int a, int b, int c) =>
        (a < b ? (a < c ? a : c) : (b < c ? b : c));

    private bool Compare(T left, T right) =>
        _comparer.Compare(left, right) == 0;

    public interface ListChange;

    public record struct ListAdded(T Element, int RightIndex) : ListChange;

    public record struct ListRemoved(T Element, int LeftIndex) : ListChange;

    public record struct ListMoved(T LeftElement, int LeftIndex, T RightElement, int RightIndex) : ListChange;
}