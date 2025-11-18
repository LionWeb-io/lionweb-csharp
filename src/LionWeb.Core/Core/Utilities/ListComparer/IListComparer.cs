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
    /// Creates a default implementation that compares two lists,
    /// and returns the minimum number of <see cref="IListChange{T}">changes</see>
    /// to convert <paramref name="left"/> into <paramref name="right"/>. 
    static IListComparer<T> Create<T>(List<T> left, List<T> right) where T : notnull =>
        new MoveDetector<T>(new ListComparer<T>(left, right));

    /// <inheritdoc cref="Create{T}"/>
    static IListComparer<T> CreateForNodes<T>(IList<T> left, List<T> right) where T : IReadableNode =>
        new MoveDetector<T>(new ListComparer<T>(left, right, new NodeIdComparer<T>()));

    /// <inheritdoc cref="Create{T}"/>
    static IListComparer<ReferenceTarget> CreateForReferenceDescriptor(IList<ReferenceTarget> left, List<ReferenceTarget> right) =>
        new MoveDetector<ReferenceTarget>(new ListComparer<ReferenceTarget>(left, right, new ReferenceTargetIdComparer()));
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
public interface IListChange<T> where T : notnull
{
    /// Index of the changed element at this point in the list of changes.
    Index Index { get; }
};

/// <paramref name="Element"/> added at <paramref name="RightIndex"/>.
public record ListAdded<T>(T Element, RightIndex RightIndex) : IListChange<T> where T : notnull
{
    /// <inheritdoc />
    public Index Index => RightIndex;

    /// <inheritdoc cref="Index"/>
    public RightIndex RightIndex { get; set; } = RightIndex;
}

/// <paramref name="Element"/> deleted from <paramref name="LeftIndex"/>.
public record ListDeleted<T>(T Element, LeftIndex LeftIndex) : IListChange<T> where T : notnull
{
    /// <inheritdoc />
    public Index Index => LeftIndex;

    /// <inheritdoc cref="Index"/>
    public LeftIndex LeftIndex { get; set; } = LeftIndex;
}

/// <paramref name="LeftElement"/> moved from <paramref name="LeftIndex"/> to <paramref name="RightIndex"/>.
/// We report <paramref name="RightElement"/> separately because it might not be identical to <paramref name="LeftElement"/>
/// (if we use a custom <see cref="IEqualityComparer{T}"/>).
public record ListMoved<T>(T LeftElement, LeftIndex LeftIndex, T RightElement, RightIndex RightIndex)
    : IListChange<T> where T : notnull
{
    /// Index of <see cref="RightElement"/> at this point in the list of changes.
    public RightIndex RightIndex { get; set; } = RightIndex;

    /// Index of <see cref="LeftElement"/> at this point in the list of changes.
    public LeftIndex LeftIndex { get; set; } = LeftIndex;

    /// Changed <see cref="LeftElement"/>.
    public T Element => LeftElement;

    /// Index of the left-most of (<see cref="LeftIndex"/>, <see cref="RightIndex"/>).
    public Index Index => Math.Min(LeftIndex, RightIndex);
}

/// <paramref name="LeftElement"/> at <paramref name="LeftIndex"/> replaced by <paramref name="RightElement"/>.
/// <remarks>Not used yet.</remarks>
public record struct ListReplaced<T>(T LeftElement, LeftIndex LeftIndex, T RightElement)
    : IListChange<T> where T : notnull
{
    /// Changed <see cref="LeftElement"/>.
    public T Element => LeftElement;

    /// <inheritdoc />
    public Index Index => LeftIndex;
}