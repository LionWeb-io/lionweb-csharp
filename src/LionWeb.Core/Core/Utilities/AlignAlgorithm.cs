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

public abstract class AlignAlgorithm
{
    protected static readonly int infinity = 10000000;
    protected int[,] mat;
    protected string s1;
    protected string s2;
    protected int _editCost;
    protected int cellsComputed;

    protected AlignAlgorithm(String str1, String str2, int[,] m) {
        mat = m;
        s1 = str1;
        s2 = str2;
        _editCost = -1;
        cellsComputed = 0;
	
        // if (s1.Length != m.dimX-1 || 
        //     s2.Length != m.dimY-1)
        // {
        //     throw new Exception("Bad matrix size");
        // }
    }

    public abstract string algName();

    public void run() {
        Console.WriteLine("Hirschberg" + " running.");
        go();

        Console.WriteLine("Hirschberg" + " done. Cost="+_editCost+"\n");
        Console.WriteLine(traceBack());
        Console.WriteLine("\n"+"Cells computed = "+cellsComputed);
    }

    int min2(int a, int b) { return (a<b ? a : b); }
    int max2(int a, int b) { return (a>b ? a : b); }

    protected int min3(int a, int b, int c) { return (a<b ? (a<c ? a : c) : (b<c ? b : c)); }
    int max3(int a, int b, int c) { return (a>b ? (a>c ? a : c) : (b>c ? b : c)); }

    public int editCost() {
        if (_editCost<0)
            go();
        return _editCost;
    }

    public virtual string traceBack() {
        return "traceBack not implemented yet for this algorithm";
    }

    abstract public void go();

}