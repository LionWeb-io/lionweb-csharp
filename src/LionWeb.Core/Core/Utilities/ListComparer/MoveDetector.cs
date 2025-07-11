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

/// Turns a fitting pair of <see cref="ListDeleted{T}"/> and <see cref="ListAdded{T}"/> into one <see cref="ListMoved{T}"/>. 
public class MoveDetector<T> : IListComparer<T> where T : notnull
{
    private readonly List<IListChange<T>> _changes;

    public MoveDetector(List<IListChange<T>> changes)
    {
        _changes = changes;
    }

    /// <inheritdoc />
    public List<IListChange<T>> Compare()
    {
        List<ListMoved<T>> movingChanges = ExtractMovingChanges();
        AddMovesToTail(movingChanges);

        return _changes;
    }

    private List<ListMoved<T>> ExtractMovingChanges()
    {
        List<ListMoved<T>> movingChanges = [];

        for (var deletedIndex = 0; deletedIndex < _changes.Count; deletedIndex++)
        {
            IListChange<T> currentChange = _changes[deletedIndex];

            if (currentChange is not ListDeleted<T> deleted)
                continue;

            for (var addedIndex = deletedIndex + 1; addedIndex < _changes.Count; addedIndex++)
            {
                var partner = _changes[addedIndex];
                if (partner is not ListAdded<T> added || !deleted.Element.Equals(added.Element))
                    continue;

                MoveIntermediates(addedIndex, deletedIndex, deleted, added);

                movingChanges.Add(new ListMoved<T>(deleted.Element, deleted.LeftIndex, added.Element,
                    added.RightIndex));
                _changes.RemoveAt(addedIndex);
                _changes.RemoveAt(deletedIndex);
                deletedIndex--;
                break;
            }
        }

        return movingChanges;
    }

    private void MoveIntermediates(Index addedIndex, Index deletedIndex, ListDeleted<T> deleted, ListAdded<T> added)
    {
        for (var intermediateIndex = addedIndex - 1; intermediateIndex > deletedIndex; intermediateIndex--)
        {
            var intermediate = _changes[intermediateIndex];
            if (intermediate is ListAdded<T> interAdd)
            {
                if (interAdd.RightIndex > deleted.LeftIndex &&
                    interAdd.RightIndex < added.RightIndex)
                {
                    interAdd.RightIndex += 1;
                    _changes[intermediateIndex] = interAdd;
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
                _changes[intermediateIndex] = interDel;
            }
        }
    }

    private void AddMovesToTail(List<ListMoved<T>> movingChanges)
    {
        for (var i = 0; i < movingChanges.Count; i++)
        {
            var currentChange = movingChanges[i];

            AdjustOtherMoves(movingChanges, i, currentChange);

            if (currentChange.LeftIndex != currentChange.RightIndex)
                _changes.Add(currentChange);
        }
    }

    private static void AdjustOtherMoves(List<ListMoved<T>> movingChanges, Index current, ListMoved<T> currentChange)
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