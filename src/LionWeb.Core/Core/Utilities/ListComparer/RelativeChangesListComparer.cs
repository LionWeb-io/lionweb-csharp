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

public class RelativeChangesListComparer<T> : IListComparer<T> where T : notnull
{
    private readonly IListComparer<T> _listComparer;
    private List<IListChange<T>> _allChanges;
    
    public RelativeChangesListComparer(List<T> left, List<T> right, IEqualityComparer<T>? comparer = null)
        : this(new ListComparer<T>(left, right, comparer))
    {
    }

    public RelativeChangesListComparer(IListComparer<T> listComparer)
    {
        _listComparer = listComparer;
    }

    /// <inheritdoc />
    public List<IListChange<T>> Compare()
    {
        _allChanges = _listComparer.Compare();
        Console.WriteLine("allChanges: \n" + string.Join("\n", _allChanges));

        var sorted = SortChangesToLeft(_allChanges);
        Console.WriteLine("Changes sorted to left: \n" + string.Join("\n", _allChanges));
        
        List<IListChange<T>> result = ShiftDeleted(sorted).ToList();

        Console.WriteLine("\nbeforeMoveAdjustment: \n" + string.Join("\n", result));
        return result;
    }

    private IEnumerable<IListChange<T>> SortChangesToLeft(List<IListChange<T>> inputChanges) =>
            inputChanges
                .OrderBy(c => c is ListDeleted<T> ? 0 : 1)
                .ThenBy(c => c.Index);

    private IEnumerable<IListChange<T>> ShiftDeleted(IEnumerable<IListChange<T>> sorted)
    {
        int shift = 0;

        return sorted.Select(change =>
        {
            Console.WriteLine("\ncurrent: " + change);
            
            if (change is ListDeleted<T>)
            {
                change.Index += shift;
                shift--;
            }
            
            return change;
        });
    }
}