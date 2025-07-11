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

/// Compares two lists and reports
/// <see cref="ListAdded{T}"/>,
/// <see cref="ListDeleted{T}"/>,
/// <see cref="ListReplaced{T}"/>,
/// and <see cref="ListMoved{T}"/> elements.
/// The lists should be passed to the constructor of implementing classes.
public interface IListComparer
{
    static IListComparer<T> Create<T>(List<T> left, List<T> right) where T : notnull =>
        new MoveDetector<T>(new RelativeChangesListComparer<T>(left, right).Compare());
}

/// <inheritdoc />
/// <typeparam name="T">Type of elements of the compared lists.</typeparam>
public interface IListComparer<T> : IListComparer where T : notnull
{
    /// Compares the two lists passed to the constructor.
    /// <returns>All changes that convert the <i>left</i> list to the <i>right</i> list.</returns>
    List<IListChange<T>> Compare();
}

/// A change to a list.
public interface IListChange;

/// <inheritdoc cref="IListChange" />
public interface IListChange<out T> : IListChange, ICloneable where T : notnull
{
    /// Changed element.
    T Element { get; }

    /// Index of the changed element at this point in the change list.
    Index Index { get; set; }
};

/// <paramref name="Element"/> added at <paramref name="RightIndex"/>.
public record ListAdded<T>(T Element, RightIndex RightIndex) : IListChange<T> where T : notnull
{
    /// <inheritdoc />
    public Index Index
    {
        get => RightIndex;
        set => RightIndex = value;
    }

    public RightIndex RightIndex { get; set; } = RightIndex;
    object ICloneable.Clone() => this with { };
}

/// <paramref name="Element"/> deleted from <paramref name="LeftIndex"/>.
public record ListDeleted<T>(T Element, LeftIndex LeftIndex) : IListChange<T> where T : notnull
{
    /// <inheritdoc />
    public Index Index
    {
        get => LeftIndex;
        set => LeftIndex = value;
    }

    public LeftIndex LeftIndex { get; set; } = LeftIndex;
    object ICloneable.Clone() => this with { };
}

/// <paramref name="LeftElement"/> moved from <paramref name="LeftIndex"/> to <paramref name="RightIndex"/>.
/// We report <paramref name="RightElement"/> separately because it might not be identical to <paramref name="LeftElement"/>
/// (if we use a custom <see cref="IEqualityComparer{T}"/>).
public record ListMoved<T>(T LeftElement, LeftIndex LeftIndex, T RightElement, RightIndex RightIndex) : IListChange<T> where T : notnull
{
    public RightIndex RightIndex { get; set; } = RightIndex;
    public LeftIndex LeftIndex { get; set; } = LeftIndex;

    /// Changed <see cref="LeftElement"/>.
    public T Element => LeftElement;

    /// <inheritdoc />
    public Index Index
    {
        get => LeftIndex < RightIndex ? LeftIndex : RightIndex;
        set
        {
            if (LeftIndex < RightIndex)
                LeftIndex = value;
            else
                RightIndex = value;
        }
    }

    public bool MoveLeftToRight => LeftIndex < RightIndex;
    public bool MoveRightToLeft => RightIndex < LeftIndex;

    object ICloneable.Clone() => this with { };
}

/// <paramref name="LeftElement"/> at <paramref name="LeftIndex"/> replaced by <paramref name="RightElement"/>.
public record struct ListReplaced<T>(T LeftElement, LeftIndex LeftIndex, T RightElement) : IListChange<T> where T : notnull
{
    /// Changed <see cref="LeftElement"/>.
    public T Element => LeftElement;

    /// <inheritdoc />
    public Index Index
    {
        get => LeftIndex;
        set => LeftIndex = value;
    }

    object ICloneable.Clone() => this with { };
}