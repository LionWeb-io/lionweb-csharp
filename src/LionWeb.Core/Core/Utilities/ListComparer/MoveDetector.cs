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

/// Turns a fitting pair of <see cref="ListDeleted{T}"/> and <see cref="ListAdded{T}"/>
/// into one <see cref="ListMoved{T}"/>. 
public class MoveDetector<T>(IListComparer<T> comparer) : IListComparer<T> where T : notnull
{
    /// <inheritdoc />
    public List<IListChange<T>> Compare()
    {
        var changes = comparer.Compare();
        var sorted = SortChangesToLeft(changes);

        var result = ShiftDeleted(sorted).ToList();

        List<ListMoved<T>> movingChanges = ExtractMovingChanges(ref result);
        result.AddRange(AddMovesToTail(movingChanges));

        return result;
    }

    private IEnumerable<IListChange<T>> SortChangesToLeft(List<IListChange<T>> changes) =>
        changes
            .OrderBy(c => c is ListDeleted<T> ? 0 : 1)
            .ThenBy(c => c.Index);

    private IEnumerable<IListChange<T>> ShiftDeleted(IEnumerable<IListChange<T>> sorted)
    {
        Index shift = 0;

        return sorted.Select(change =>
        {
            if (change is ListDeleted<T> deleted)
            {
                deleted.LeftIndex += shift;
                shift--;
            }

            return change;
        });
    }

    private List<ListMoved<T>> ExtractMovingChanges(ref List<IListChange<T>> result)
    {
        List<ListMoved<T>> movingChanges = [];

        for (var deletedIndex = 0; deletedIndex < result.Count; deletedIndex++)
        {
            IListChange<T> currentChange = result[deletedIndex];

            if (currentChange is not ListDeleted<T> deleted)
                continue;

            for (var addedIndex = deletedIndex + 1; addedIndex < result.Count; addedIndex++)
            {
                var partner = result[addedIndex];
                if (partner is not ListAdded<T> added || !deleted.Element.Equals(added.Element))
                    continue;

                MoveIntermediates(addedIndex, deletedIndex, deleted, added, ref result);

                movingChanges.Add(new ListMoved<T>(deleted.Element, deleted.LeftIndex, added.Element,
                    added.RightIndex));
                result.RemoveAt(addedIndex);
                result.RemoveAt(deletedIndex);
                deletedIndex--;
                break;
            }
        }

        return movingChanges;
    }

    private void MoveIntermediates(Index addedIndex, Index deletedIndex, ListDeleted<T> deleted, ListAdded<T> added,
        ref List<IListChange<T>> result)
    {
        for (var intermediateIndex = addedIndex - 1; intermediateIndex > deletedIndex; intermediateIndex--)
        {
            var intermediate = result[intermediateIndex];
            if (intermediate is ListAdded<T> interAdd)
            {
                if (interAdd.RightIndex > deleted.LeftIndex &&
                    interAdd.RightIndex < added.RightIndex)
                {
                    interAdd.RightIndex += 1;
                    result[intermediateIndex] = interAdd;
                    continue;
                }

                if (interAdd.RightIndex <= deleted.LeftIndex)
                {
                    deleted.LeftIndex += 1;
                }

                continue;
            }

            if (intermediate is ListDeleted<T> interDel)
            {
                interDel.LeftIndex += 1;
                result[intermediateIndex] = interDel;
            }
        }
    }

    private List<ListMoved<T>> AddMovesToTail(List<ListMoved<T>> movingChanges)
    {
        List<ListMoved<T>> result = [];

        for (var i = 0; i < movingChanges.Count; i++)
        {
            var currentChange = movingChanges[i];

            AdjustOtherMoves(movingChanges, i, currentChange);

            if (currentChange.LeftIndex != currentChange.RightIndex)
                result.Add(currentChange);
        }

        return result;
    }

    private void AdjustOtherMoves(List<ListMoved<T>> movingChanges, Index current, ListMoved<T> currentChange)
    {
        for (var j = current + 1; j < movingChanges.Count; j++)
        {
            if (ShouldMoveRightToLeft(currentChange) &&
                ShouldMoveRightToLeft(movingChanges[j]) &&
                movingChanges[j].RightIndex <= currentChange.RightIndex
               )
            {
                currentChange.RightIndex -= 1;
                currentChange.LeftIndex -= 1;
                continue;
            }

            if (movingChanges[j].LeftIndex <= currentChange.LeftIndex ||
                movingChanges[j].LeftIndex > currentChange.RightIndex)
                continue;

            movingChanges[j].LeftIndex -= 1;

            if (ShouldMoveLeftToRight(movingChanges[j]))
            {
                if (movingChanges[j].RightIndex > currentChange.RightIndex)
                {
                    currentChange.RightIndex += 1;
                    continue;
                }

                if (movingChanges[j].RightIndex <= currentChange.RightIndex)
                {
                    movingChanges[j].RightIndex -= 1;
                }

                continue;
            }

            if (ShouldMoveRightToLeft(movingChanges[j]))
            {
                if (movingChanges[j].RightIndex <= currentChange.LeftIndex)
                {
                    currentChange.LeftIndex -= 1;
                    continue;
                }

                if (movingChanges[j].RightIndex >= currentChange.LeftIndex)
                {
                    movingChanges[j].RightIndex -= 1;
                }
            }
        }
    }

    private static bool ShouldMoveLeftToRight(ListMoved<T> listMoved) => listMoved.LeftIndex < listMoved.RightIndex;
    private static bool ShouldMoveRightToLeft(ListMoved<T> listMoved) => listMoved.RightIndex < listMoved.LeftIndex;
}