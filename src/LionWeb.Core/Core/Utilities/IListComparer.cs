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

using LeftIndex = Index;
using RightIndex = Index;

/// Compares two lists and reports <see cref="Added"/>, <see cref="Deleted"/>, <see cref="Replaced"/>, and <see cref="Moved"/> elements.
/// The lists should be passed to the constructor of implementing classes.
/// <typeparam name="T">Type of elements of the compared lists.</typeparam>
public interface IListComparer<T>
{
    /// Compares the two lists passed to the constructor.
    /// <returns>All changes that convert the <i>left</i> list to the <i>right</i> list.</returns>
    List<IChange> Compare();

    /// A change to a list.
    interface IChange : ICloneable
    {
        /// Changed element.
        T Element { get; }
        int Index { get; }
    };

    interface ILeftChange : IChange
    {
        LeftIndex LeftIndex { get; set; }
    }

    interface IRightChange : IChange
    {
        RightIndex RightIndex { get; set; }
    }

    /// <paramref name="Element"/> added at <paramref name="RightIndex"/>.
    record struct Added(T Element, RightIndex RightIndex) : IRightChange
    {
        public int Index => RightIndex;
        object ICloneable.Clone() => this with {};
    }

    /// <paramref name="Element"/> deleted from <paramref name="LeftIndex"/>.
    record struct Deleted(T Element, LeftIndex LeftIndex) : ILeftChange
    {
        public int Index => LeftIndex;
        object ICloneable.Clone() => this with {};
    }

    /// <paramref name="LeftElement"/> moved from <paramref name="LeftIndex"/> to <paramref name="RightIndex"/>.
    /// We report <paramref name="RightElement"/> separately because it might not be identical to <paramref name="LeftElement"/>
    /// (if we use a custom <see cref="IEqualityComparer{T}"/>).
    record struct Moved(T LeftElement, LeftIndex LeftIndex, T RightElement, RightIndex RightIndex)
        : ILeftChange, IRightChange
    {
        /// Changed <see cref="LeftElement"/>.
        public T Element => LeftElement;
        public int Index => LeftIndex;
        object ICloneable.Clone() => this with {};
    }
    
    /// <paramref name="LeftElement"/> at <paramref name="LeftIndex"/> replaced by <paramref name="RightElement"/>.
    record struct Replaced(T LeftElement, LeftIndex LeftIndex, T RightElement) : ILeftChange
    {
        /// Changed <see cref="LeftElement"/>.
        public T Element => LeftElement;
        public int Index => LeftIndex;
        object ICloneable.Clone() => this with {};
    }
}