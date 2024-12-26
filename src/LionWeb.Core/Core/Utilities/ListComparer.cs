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

public class ListComparer<T>
{
    private readonly List<T> _left;
    private readonly List<T> _right;
    private readonly IComparer<T> _comparer;

    public ListComparer(List<T> left, List<T> right, IComparer<T>? comparer = null)
    {
        _left = left;
        _right = right;
        _comparer = comparer ?? Comparer<T>.Default;
    }

    public List<ListChange> Compare()
    {
        var leftEnumerator = _left.GetEnumerator();
        var rightEnumerator = _right.GetEnumerator();


        return [];
    }

//     private int max(int a, int b, int c) =>
//         Math.Max(Math.Max(a, b), c);
//
//     int[] NWScore(string X, string Y) {
//     int[][] Score = new int[X.Length][];
//     Score[0][0] = 0; // 2 * (length(Y) + 1) array
//     for (int j = 1; j <= Y.Length; j++)
//     {
//         Score[0] = new int[Y.Length];
//         Score[0][j] = Score[0][j - 1] + Ins(Y[j]);
//     }
//
//     for (int i = 1; i <= X.Length; i++) // Init array
//     {
//         Score[1][0] = Score[0][0] + Del(X[i]);
//         for (int j = 1; j <= Y.Length; j++)
//         {
//             var scoreSub = Score[0][j - 1] + Sub(X[i], Y[j]);
//             var scoreDel = Score[0][j] + Del(X[i]);
//             var scoreIns = Score[1][j - 1] + Ins(Y[j]);
//             Score[1][j] = max(scoreSub, scoreDel, scoreIns);
//         }
//
//         // Copy Score[1] to Score[0]
//         Score[0] = Score[1];
//     }
//
//     // for (int j = 0; j <= Y.Length; j++)
//     //     LastLine(j) = Score(1, j)
//     return Score[1];
//     }
//
//     private int Sub(char a, char b) => 2;
//     private int Ins(char c) => 1;
//     private int Del(char c) => 0;
//
//
//     (string, string) Hirschberg(string X, string Y)
//     {
//         string Z = "";
//         string W = "";
//         if (X.Length == 0)
//         {
//             for (int i = 1; i <= Y.Length; i++)
//             {
//                 Z = Z + '-';
//                 W = W + Y[i];
//             }
//         }
//         else if (Y.Length == 0)
//         {
//         for (int i = 1; i <= X.Length; i++)
//         {
//             Z = Z + X[i];
//             W = W + '-';
//         }
//         }
//         else if (X.Length == 1 || Y.Length == 1)
//         {
//             (Z, W) = NeedlemanWunsch(X, Y);
//         }
//         else
//         {
//         int xlen = X.Length;
//         int xmid = X.Length / 2;
//         int ylen = Y.Length;
//
//         var ScoreL = NWScore(X[1..xmid], Y);
//         var ScoreR = NWScore(X[(xmid + 1)..xlen].Reverse().ToString(), Y.Reverse().ToString());
//         var ymid = arg max ScoreL + ScoreR.Reverse().ToString();
//
//             (Z, W) = Hirschberg(X[1..xmid], Y[1..ymid]) + Hirschberg(X[(xmid + 1)..xlen], Y[(ymid + 1)..ylen]);
//         }
//
//         return (Z, W);
//     }
//
//     private (string, string, int)[] nw(string A, string B, int[,] simMatrix, int gapPenalty, Dictionary<char, int> alphEnum)
//     {
// // The Needleman-Wunsch algorithm
//
// // Stage 1: Create a zero matrix and fills it via algorithm
//         int n = A.Length;
//         int m = B.Length;
//         int[,] mat = new int[n, m];
//
//         {
//             for (int i = 0; i <= n + 1; i++)
//                 mat[i,0] = mat[0,0] * (m + 1);
//             for (int j = 0; j <= m + 1; j++)
//                 mat[0, j] = gapPenalty * j;
//             for (int i = 0; i <= n + 1; i++)
//                 mat[i, 0] = gapPenalty * i;
//             for (int i = 1; i <= n + 1; i++)
//             {
//                 for (int j = 1; j <= m + 1; j++)
//                     mat[i, j] = max(mat[i - 1, j - 1] + simMatrix[alphEnum[A[i - 1]], alphEnum[B[j - 1]]],
//                         mat[i, j - 1] + gapPenalty, mat[i - 1, j] + gapPenalty);
//             }
//         }
//
// // Stage 2: Computes the final alignment, by backtracking through matrix
//         string alignmentA = "";
//         string alignmentB = "";
//         {
//             int i = n;
//             int j = m;
//             while (i != 0 && j != 0)
//             {
//                 int score = mat[i, j];
//                 int scoreDiag = mat[i - 1, j - 1];
//                 int scoreUp = mat[i - 1, j];
//                 int scoreLeft = mat[i, j - 1];
//                 if (score == scoreDiag + simMatrix[alphEnum[A[i - 1]], alphEnum[B[j - 1]]])
//                 {
//                     alignmentA = A[i - 1] + alignmentA;
//                     alignmentB = B[j - 1] + alignmentB;
//                     i -= 1;
//                     j -= 1;
//                 } else if (score == scoreUp + gapPenalty)
//                 {
//                     alignmentA = A[i - 1] + alignmentA;
//                     alignmentB = '-' + alignmentB;
//                     i -= 1;
//                 } else if (score == scoreLeft + gapPenalty)
//                 {
//                     alignmentA = '-' + alignmentA;
//                     alignmentB = B[j - 1] + alignmentB;
//                     j -= 1;
//                 }
//             }
//
//             while (i != 0)
//             {
//                 alignmentA = A[i - 1] + alignmentA;
//                 alignmentB = '-' + alignmentB;
//                 i -= 1;
//             }
//
//             while (j != 0)
//             {
//                 alignmentA = '-' + alignmentA;
//                 alignmentB = B[j - 1] + alignmentB;
//                 j -= 1;
//             }
//         }
//         
// // Now return result in format: [1st alignment, 2nd alignment, similarity]
//         return (alignmentA, alignmentB, mat[n, m]);
//     }
//
//     private int[] forwards(string x, string y, int[,] simMatrix, int gapPenalty, Dictionary<char, int> alphEnum)
//     {
// // This is the forwards subroutine.
//         int n = x.Length;
//         int m = y.Length;
//
//         int[,] mat = new int[n, m];
//         for (int i = 0; i <= n + 1; i++)
//             mat[i, 0] = mat[0,0] * (m + 1);
//         for (int j = 0; j <= (m + 1); j++)
//             mat[0, j] = gapPenalty * j;
//         for (int i = 1; i <= n + 1; i++)
//         {
//             mat[i, 0] = mat[i - 1, 0] + gapPenalty;
//             for (int j = 1; j <= m + 1; j++)
//                 mat[i, j] = max(
//                     mat[i - 1, j - 1] + simMatrix[alphEnum[x[i - 1]], alphEnum[y[j - 1]]],
//                     mat[i - 1, j] + gapPenalty,
//                     mat[i, j - 1] + gapPenalty
//                 );
// // Now clear row from memory.
//             // mat[i - 1] = null;
//         }
//
//         return mat[n];
//     }
//
//     private int[] backwards(string x, string y, int[,] simMatrix, int gapPenalty, Dictionary<char, int> alphEnum)
//     {
// // This is the backwards subroutine.
//         int n = x.Length;
//         int m = y.Length;
//         int[,] mat = new int[n, m];
//         for (int i = 0; i <= n + 1; i++)
//             mat[i, 0] = mat[0,0] * (m + 1);
//         for (int j = 0; j <= m + 1; j++)
//             mat[0, j] = gapPenalty * j;
//         for (int i = 1; i <= n + 1; i++)
//         {
//             mat[i, 0] = mat[i - 1, 0] + gapPenalty;
//             for (int j = 1; j <= m + 1; j++)
//                 mat[i, j] = max(mat[i - 1, j - 1] + simMatrix[alphEnum[x[n - i]], alphEnum[y[m - j]]],
//                     mat[i - 1, j] + gapPenalty,
//                     mat[i, j - 1] + gapPenalty
//                 );
// // Now clear row from memory.
//             // mat[i - 1] = []
//         }
//
//         return mat[n];
//     }
//
//     public (string, string, int)[] hirschberg(string x, string y, int[,] simMatrix, int gapPenalty, Dictionary<char, int> alphEnum)
//     {
//         // This is the main Hirschberg routine.
//         int n = x.Length;
//         int m = y.Length;
//
//         if (n < 2 || m < 2)
// // In this case we just use the N-W algorithm.
//             return nw(x, y, simMatrix, gapPenalty, alphEnum);
//
// // Make partitions, call subroutines.
//         var F = forwards(x[..(n / 2)], y, simMatrix, gapPenalty, alphEnum);
//         var B = backwards(x[(n / 2)..], y, simMatrix, gapPenalty, alphEnum);
//         var partition = Enumerable.Range(0,m+1).Select(j => F[j] + B[m - j]).ToList();
//         var cut = partition.IndexOf(partition.Max());
// // Clear all memory now, so that we don't store data during recursive calls.
//         F = null;
//         B = null;
//         partition = null;
// // Now make recursive calls.
//         var callLeft = hirschberg(x[..(n / 2)], y[..cut], simMatrix, gapPenalty, alphEnum);
//         var callRight = hirschberg(x[(n / 2)..], y[cut..], simMatrix, gapPenalty, alphEnum);
// // Now return result in format: [1st alignment, 2nd alignment, similarity]
//         return Enumerable.Range(0, 3).Select(r => callLeft[r] + callRight[r]).ToArray();
//     }

    public interface ListChange;

    public record struct ListAdded(T Element, int RightIndex) : ListChange;

    public record struct ListRemoved(T Element, int LeftIndex) : ListChange;

    public record struct ListMoved(T LeftElement, int LeftIndex, T RightElement, int RightIndex) : ListChange;
}




