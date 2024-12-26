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

using System.Text;

// Hirschberg - implement the Hirschberg O(n) space algorithm
// for finding an alignment.  Code translated from Lloyd's javascript
// version: http://www.csse.monash.edu.au/~lloyd/tildeAlgDS/Dynamic/Hirsch.html
public class Hirschberg : AlignAlgorithm
{
    static int unusedCol = 0;
    static int storeCol = 2;
    static int alignCol = 4;

    int[][] fwdMat, revMat;
    StringBuilder align1;
    StringBuilder align2;

    public List<string> actions = [];

    public SortedList<int, (char, int)> added = [];
    public SortedList<int, (char, int)> removed = [];

    public Hirschberg(string str1, string str2, int[,] m) : base(str1, str2, m)
    {
        fwdMat = new int[][] { new int[s2.Length + 1], new int[s2.Length + 1] };
        revMat = new int[][] { new int[s2.Length + 1], new int[s2.Length + 1] };
        align1 = new StringBuilder();
        align2 = new StringBuilder();
    }

    public override string algName() { return "Hirschberg"; }

    public override void go()
    {
        // mat.clearAlignment();
        mat[0, 0] = alignCol;
        mat[s1.Length, s2.Length] = alignCol;
        align(0, s1.Length, 0, s2.Length);
        foreach ((char, int) addedEntry in added.Values.ToList())
        {
            (char, int) removedEntry = removed.Values.FirstOrDefault(p => p.Item1 == addedEntry.Item1);
            if (removedEntry != default)
            {
                var indexOfValue = removed.IndexOfValue(removedEntry);
                actions.Add($"moved: {addedEntry.Item1} from {removedEntry.Item2} to {addedEntry.Item2}");
                added.RemoveAt(added.IndexOfValue(addedEntry));
                removed.RemoveAt(indexOfValue);
            }
        }
        foreach (var c in added.Values)
        {
            actions.Add($"added: {c.Item1} at {c.Item2}");
        }
        foreach (var c in removed.Values)
        {
            actions.Add($"removed: {c.Item1} at {c.Item2}");
        }
    }

    void align(int p1, int p2, int q1, int q2)
    {
        // Align s1[p1..p2) with p2[q1..q2)
        //System.out.println("s1["+p1+".."+(p2-1)+"] : s2["+q1+".."+(q2-1)+"]");

        // First the base cases...

        if (p2 <= p1)
        {
            // s1 is empty
            for (int j = q1; j < q2; j++)
            {
                align1.Append("-");
                var s2Char = s2[j];
                align2.Append(s2Char);
                var s1Char = s1[j];
                Remove(s1Char, j);
                mat[p1, j + 1] = alignCol;
                // mat.addAlignment(p1,j,p1,j+1);
            }

            return;
        }

        if (q2 <= q1)
        {
            // s2 is empty
            for (int i = p1; i < p2; i++)
            {
                var s1char = s1[i];
                align1.Append(s1char);
                align2.Append("-");
                Remove(s1char, i);
                mat[i + 1, q2] = alignCol;
                // mat.addAlignment(i,q2,i+1,q2);
            }

            return;
        }

        if (p1 + 1 == p2)
        {
            // s1 is exactly one character
            char ch = s1[p1];
            int memo = q1;
            for (int j = q1 + 1; j < q2; j++)
                if (s2[j] == ch)
                    memo = j;
            // memo = an optimal cross point 
            for (int j = q1; j < q2; j++)
            {

                var s2Char = s2[j];
                if (j == memo)
                {
                    align1.Append(ch);
                    // actions.Add($"moved: {ch} from {j}");
                    if (ch != s2Char)
                        Add(s2Char, p1);
                } else
                {
                    align1.Append("-");
                    Remove(ch, j);
                    // actions.Add($"moved: {s2[j]} from {j}");
                }

                align2.Append(s2Char);

                if (j < memo)
                    mat[p1, j + 1] = alignCol;
                else
                    mat[p1 + 1, j + 1] = alignCol;

                // mat.addAlignment((j<=memo ? p1 : p1+1),j,
                // 		 (j<memo? p1 : p1+1),j+1);
            }

            return;
        }

        // Done with the base cases.  
        // Now the general case. Divide and conquer!
        int mid = (int)Math.Floor((double)((p1 + p2) / 2));
        fwdDPA(p1, mid, q1, q2);
        revDPA(mid, p2, q1, q2);

        int s2mid = q1, best = infinity;
        // Find the cheapest split
        for (int j = q1; j <= q2; j++)
        {
            int sum = fwdMat[mid % 2][j] + revMat[mid % 2][j];
            if (sum < best)
            {
                best = sum;
                s2mid = j;
            }

            ;
        }

        if (_editCost == -1) _editCost = best;

        // Mark the matrices unused...
        for (int j = q1; j <= q2; j++)
        {
            unusedCell(mid, j);
            unusedCell(mid - 1, j);
            unusedCell(mid + 1, j);
        }

        mat[mid, s2mid] = alignCol;
        //mat.addAlignment(mid,s2mid, mid+1, s2mid+1);

        // Recurse on the two halves...
        align(p1, mid, q1, s2mid);
        align(mid, p2, s2mid, q2);
    }

    private void Remove(char s1char, int index) => removed
        .Add(index, (s1char, index));

    // private void DetectMove(char s1Char, int index)
    // {
    //     var lastIndex = removed.FindLastIndex(p => p.Item1 == s1Char);
    //     if (lastIndex != -1)
    //     {
    //         removed.RemoveAt(lastIndex);
    //         actions.Add($"moved: {s1Char} from {lastIndex} to {index}");
    //     } else
    //     {
    //         Add(s1Char, index);
    //     }
    // }

    private void Add(char s1Char, int index) =>
        added.Add(index, (s1Char, index));

    void fwdDPA(int p1, int p2, int q1, int q2)
    {
        fwdMat[p1 % 2][q1] = 0;
        storeCell(p1, q1, 0);
        cellsComputed++;

        // Setup the first row
        for (int j = q1 + 1; j <= q2; j++)
        {
            fwdMat[p1 % 2][j] = fwdMat[p1 % 2][j - 1] + 1;
            storeCell(p1, j, fwdMat[p1 % 2][j]);
            cellsComputed++;
        }

        for (int i = p1 + 1; i <= p2; i++)
        {
            // Mark the row to be calculated as unused.
            if (i >= p1 + 2)
                for (int j = q1; j <= q2; j++)
                    unusedCell(i - 2, j);

            fwdMat[i % 2][q1] = fwdMat[(i - 1) % 2][q1] + 1;
            storeCell(i, q1, fwdMat[i % 2][q1]);
            cellsComputed++;
            for (int j = q1 + 1; j <= q2; j++)
            {
                fwdMat[i % 2][j] = min3(fwdMat[(i - 1) % 2][j] + 1, //delete
                    fwdMat[i % 2][j - 1] + 1, //insert
                    fwdMat[(i - 1) % 2][j - 1] + //match/mismatch
                    (s1[i - 1] == s2[j - 1] ? 0 : 1));
                storeCell(i, j, fwdMat[i % 2][j]);
                cellsComputed++;
            }
        }
    }

    void revDPA(int p1, int p2, int q1, int q2)
    {
        revMat[p2 % 2][q2] = 0;
        storeCell(p2, q2, 0);
        cellsComputed++;

        // Setup the first row
        for (int j = q2 - 1; j >= q1; j--)
        {
            revMat[p2 % 2][j] = revMat[p2 % 2][j + 1] + 1;
            storeCell(p2, j, revMat[p2 % 2][j]);
            cellsComputed++;
        }

        for (int i = p2 - 1; i >= p1; i--)
        {
            // Mark the row to be calculated as unused.
            if (i <= p2 - 2)
                for (int j = q2; j >= q1; j--)
                    unusedCell(i + 2, j);

            revMat[i % 2][q2] = revMat[(i + 1) % 2][q2] + 1;
            storeCell(i, q2, revMat[i % 2][q2]);
            cellsComputed++;
            for (int j = q2 - 1; j >= q1; j--)
            {
                revMat[i % 2][j] = min3(revMat[(i + 1) % 2][j] + 1, //delete
                    revMat[i % 2][j + 1] + 1, //insert
                    revMat[(i + 1) % 2][j + 1] + //match/mismatch
                    (s1[i] == s2[j] ? 0 : 1));
                storeCell(i, j, revMat[i % 2][j]);
                cellsComputed++;
            }
        }
    }

    void unusedCell(int i, int j)
    {
        if (mat[i, j] != alignCol)
            mat[i, j] = int.MaxValue;
    }

    void storeCell(int i, int j, int v)
    {
        var c = mat[i, j];
        int col = storeCol;
        if (c == alignCol) col = alignCol;
        // c.setVal(v,col);
        mat[i, j] = col;
    }


    public override string traceBack()
    {
        return align1.ToString() + "\n" + align2.ToString();
    }
}