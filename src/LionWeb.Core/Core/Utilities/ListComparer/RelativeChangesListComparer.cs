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

namespace LionWeb.Core.Utilities.ListComparer;

public class RelativeChangesListComparer<T> : IListComparer<T>
{
    private readonly IListComparer<T> _listComparer;
    private List<IListComparer<T>.IChange> _allChanges;
    
    public RelativeChangesListComparer(List<T> left, List<T> right, IEqualityComparer<T>? comparer = null)
        : this(new ListComparer<T>(left, right, comparer))
    {
    }

    public RelativeChangesListComparer(IListComparer<T> listComparer)
    {
        _listComparer = listComparer;
    }
    
    public List<IListComparer<T>.IChange> Compare()
    {
        _allChanges = _listComparer.Compare();
        Console.WriteLine("allChanges: \n" + string.Join("\n", _allChanges));

        var sorted = SortChangesToLeft(_allChanges);
        Console.WriteLine("Changes sorted to left: \n" + string.Join("\n", _allChanges));
        
        List<IListComparer<T>.IChange> result = ShiftDeleted(sorted).ToList();

        Console.WriteLine("\nbeforeMoveAdjustment: \n" + string.Join("\n", result));
        return result;
    }

    private IEnumerable<IListComparer<T>.IChange> SortChangesToLeft(List<IListComparer<T>.IChange> inputChanges) =>
            inputChanges
                .OrderBy(c => c is IListComparer<T>.Deleted ? 0 : 1)
                .ThenBy(c => c.Index);

    private IEnumerable<IListComparer<T>.IChange> ShiftDeleted(IEnumerable<IListComparer<T>.IChange> sorted)
    {
        int shift = 0;

        return sorted.Select(change =>
        {
            Console.WriteLine("\ncurrent: " + change);
            
            if (change is IListComparer<T>.Deleted)
            {
                change.Index += shift;
                shift--;
            }
            
            return change;
        });
    }
}