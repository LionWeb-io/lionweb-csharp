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

using LeftIndex = Index;
using RightIndex = Index;

/// Compares two lists and reports <see cref="ListAdded{T}"/>, <see cref="ListDeleted{T}"/>, <see cref="ListReplaced{T}"/>, and <see cref="ListMoved{T}"/> elements.
/// The lists should be passed to the constructor of implementing classes.
/// <typeparam name="T">Type of elements of the compared lists.</typeparam>
public interface IListComparer<T>
{
    /// Compares the two lists passed to the constructor.
    /// <returns>All changes that convert the <i>left</i> list to the <i>right</i> list.</returns>
    List<IListChange<T>> Compare();
}

/// <paramref name="Element"/> added at <paramref name="RightIndex"/>.
record ListAdded<T>(T Element, RightIndex RightIndex) : IRightListChange<T>
{
    public int Index
    {
        get => RightIndex;
        set => RightIndex = value;
    }

    public virtual RightIndex RightIndex { get; set; } = RightIndex;
    object ICloneable.Clone() => this with {};
}

/// <paramref name="Element"/> deleted from <paramref name="LeftIndex"/>.
record ListDeleted<T>(T Element, LeftIndex LeftIndex) : ILeftListChange<T>
{
    public int Index
    {
        get => LeftIndex;
        set => LeftIndex = value;
    }

    public virtual LeftIndex LeftIndex { get; set; } = LeftIndex;
    object ICloneable.Clone() => this with {};
}

/// <paramref name="LeftElement"/> moved from <paramref name="LeftIndex"/> to <paramref name="RightIndex"/>.
/// We report <paramref name="RightElement"/> separately because it might not be identical to <paramref name="LeftElement"/>
/// (if we use a custom <see cref="IEqualityComparer{T}"/>).
record ListMoved<T>(T LeftElement, LeftIndex LeftIndex, T RightElement, RightIndex RightIndex)
    : ILeftListChange<T>, IRightListChange<T>
{
    public RightIndex RightIndex { get; set; } = RightIndex;
    public LeftIndex LeftIndex { get; set; } = LeftIndex;
    /// Changed <see cref="LeftElement"/>.
    public T Element => LeftElement;
    public int Index
    {
        get => LeftIndex < RightIndex ? LeftIndex : RightIndex;
        set => SetIndex(value);
    }

    private LeftIndex SetIndex(int value) => LeftIndex < RightIndex ? LeftIndex = value : RightIndex = value;

    public int RightEdge => LeftIndex > RightIndex ? LeftIndex : RightIndex;
        
    public bool MoveLeftToRight => LeftIndex < RightIndex;
    public bool MoveRightToLeft => RightIndex < LeftIndex;
        
    object ICloneable.Clone() => this with {};
}

/// <paramref name="LeftElement"/> at <paramref name="LeftIndex"/> replaced by <paramref name="RightElement"/>.
record struct ListReplaced<T>(T LeftElement, LeftIndex LeftIndex, T RightElement) : ILeftListChange<T>
{
    /// Changed <see cref="LeftElement"/>.
    public T Element => LeftElement;
    public int Index
    {
        get => LeftIndex;
        set => LeftIndex = value;
    }

    object ICloneable.Clone() => this with {};
}

interface IRightListChange<T> : IListChange<T>
{
    RightIndex RightIndex { get; set; }
}

interface ILeftListChange<T> : IListChange<T>
{
    LeftIndex LeftIndex { get; set; }
}

/// A change to a list.
public interface IListChange<T> : ICloneable
{
    /// Changed element.
    T Element { get; }
    int Index { get; set; }
};