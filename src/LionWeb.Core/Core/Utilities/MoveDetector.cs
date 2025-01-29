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

namespace LionWeb.Core.Utilities;

public class MoveDetector<T> : IListComparer<T>
{
    private readonly List<IListComparer<T>.IChange> _changes;

    public MoveDetector(List<IListComparer<T>.IChange> changes)
    {
        _changes = changes;
    }

    public List<IListComparer<T>.IChange> Compare()
    {
        List<IListComparer<T>.Moved> movingChanges = [];
        
        // Extract moves from the list of changes
        for (var i = 0; i < _changes.Count; i++)
        {
            var currentResult = _changes[i];
            bool isAMove = false;
            
            if (currentResult is IListComparer<T>.Deleted d)
            {
                for (var j = i + 1; j < _changes.Count; j++)
                {
                    var partner = _changes[j];
                    if (partner is IListComparer<T>.Added partnerAdd && d.Element.Equals(partnerAdd.Element))
                    {
                        Console.WriteLine("\nCouple detected as a move: " + d + " " + partnerAdd);
                        
                        for (var k = j - 1; k > i; k--)
                        {
                            var intermediate = _changes[k];
                            if (intermediate is IListComparer<T>.Added interAdd)
                                if (interAdd.RightIndex > d.LeftIndex && interAdd.RightIndex < partnerAdd.RightIndex)
                                {
                                    interAdd.RightIndex += 1;
                                    _changes[k] = interAdd;
                                } 
                                else if (interAdd.RightIndex <= d.LeftIndex)
                                {
                                    d.LeftIndex += 1;
                                }
                            if (intermediate is IListComparer<T>.Deleted interDel)
                            {
                                interDel.LeftIndex += 1;
                                _changes[k] = interDel;
                            }   
                        }
                        
                        movingChanges.Add(new IListComparer<T>.Moved(d.Element, d.LeftIndex, 
                                            partnerAdd.Element, partnerAdd.RightIndex));
                        _changes.RemoveAt(j);
                        isAMove = true;
                        break;
                    }
                }
            }

            if (isAMove) 
            {
                _changes.RemoveAt(i);
                i--;
                Console.WriteLine("   after extracting this couple: \n" + string.Join("\n", _changes));
            }
        }
        
        // Add moves to the tail of the changes list
        for  (var i = 0; i < movingChanges.Count; i++)
        {
            var currentChange = movingChanges[i];
            
            for (var j = i + 1; j < movingChanges.Count; j++)
            {
                if (currentChange.MoveLeftToRight)
                {
                    if (movingChanges[j].LeftIndex > currentChange.LeftIndex &&
                        movingChanges[j].LeftIndex <= currentChange.RightIndex)
                    {
                        movingChanges[j].LeftIndex -= 1;
                        if (movingChanges[j].MoveLeftToRight &&
                            movingChanges[j].RightIndex > currentChange.RightIndex)
                        {
                            currentChange.RightIndex += 1;
                        } 
                        else if (movingChanges[j].MoveLeftToRight &&
                                 movingChanges[j].RightIndex <= currentChange.RightIndex)
                        {
                            movingChanges[j].RightIndex -= 1;
                        } 
                        else if (movingChanges[j].MoveRightToLeft &&
                                 movingChanges[j].RightIndex <= currentChange.LeftIndex)
                        {
                            currentChange.LeftIndex -= 1;
                        }
                        else if (movingChanges[j].MoveRightToLeft &&
                                 movingChanges[j].RightIndex >= currentChange.LeftIndex)
                        {
                            movingChanges[j].RightIndex -= 1;
                        }
                    }
                } 
                else if (currentChange.MoveRightToLeft && movingChanges[j].MoveRightToLeft &&
                         movingChanges[j].RightIndex <= currentChange.RightIndex)
                {
                    currentChange.RightIndex -= 1;
                    currentChange.LeftIndex -= 1;
                }
            }
            if (currentChange.LeftIndex != currentChange.RightIndex)
                _changes.Add(currentChange);
        }
        
        Console.WriteLine("\nafter MoveDetector: \n" + string.Join("\n", _changes));
        return _changes;
    }
    /*
    public List<IListComparer<T>.IChange> Compare()
    {
        for (var i = 0; i < _changes.Count; i++)
        {
            var currentResult = _changes[i];
            if (currentResult is IListComparer<T>.Added a)
            {
                for (var j = i + 1; j < _changes.Count; j++)
                {
                    var partner = _changes[j];
                    if (partner is IListComparer<T>.Deleted partnerDelete && a.Element.Equals(partnerDelete.Element))
                    {
                        Console.WriteLine("\nCouple detected as a move (first variant): " + a + "   " + partnerDelete);
                        for (var k = j - 1; k > i; k--)
                        {
                            var intermediate = _changes[k];
                            if (intermediate is IListComparer<T>.Added interAdd)
                            {
                                // interAdd.RightIndex += 1;
                                partnerDelete.LeftIndex -= 1;
                                _changes[k] = partnerDelete;
                                _changes[k + 1] = intermediate;
                            }

                            if (intermediate is IListComparer<T>.Deleted interDel)
                            {
                                // interDel.LeftIndex -= 1;
                                partnerDelete.LeftIndex += 1;
                                _changes[k] = partnerDelete;
                                _changes[k + 1] = intermediate;
                            }
                        }

                        _changes[i] = new IListComparer<T>.Moved(partnerDelete.Element, partnerDelete.LeftIndex - 1,
                            a.Element, a.RightIndex);
                        _changes.RemoveAt(i + 1);
                        i--;
                    }
                }
            } else if (currentResult is IListComparer<T>.Deleted d)
            {
                for (var j = i + 1; j < _changes.Count; j++)
                {
                    var partner = _changes[j];
                    if (partner is IListComparer<T>.Added partnerAdd && d.Element.Equals(partnerAdd.Element))
                    {
                        Console.WriteLine("\nCouple detected as a move: " + d + " " + partnerAdd);
                        var shift = 1;
                        if (d.LeftIndex < partnerAdd.RightIndex) // move from left to right?
                            shift = -1;
                        
                        for (var k = j - 1; k > i; k--)
                        {
                            var intermediate = _changes[k];
                            if (intermediate is IListComparer<T>.Added interAdd)
                            {
                                // interAdd.RightIndex += 1;
                                //if (interAdd.RightIndex < partnerAdd.RightIndex)
                                    partnerAdd.RightIndex -= 1;
                                _changes[k] = partnerAdd;
                                //interAdd.RightIndex += 1; // hmmm
                                _changes[k + 1] = interAdd;
                            }

                            if (intermediate is IListComparer<T>.Deleted interDel)
                            {
                                // interDel.LeftIndex -= 1;
                                //if (interDel.LeftIndex < partnerAdd.RightIndex)
                                    partnerAdd.RightIndex += 1;
                                _changes[k] = partnerAdd;
                                //interDel.LeftIndex += shift;
                                _changes[k + 1] = interDel;
                            }
                        }

                        _changes[i] = new IListComparer<T>.Moved(d.Element, d.LeftIndex, partnerAdd.Element,
                            partnerAdd.RightIndex);
                        _changes.RemoveAt(i + 1);
                        
                        Console.WriteLine("   after this move detected: \n" + string.Join("\n", _changes));
                        i--;
                    }
                }
            }
        }

        Console.WriteLine("\nafter MoveDetector: \n" + string.Join("\n", _changes));

        return _changes;
    }*/
}