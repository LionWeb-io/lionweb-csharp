// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

// ReSharper disable SuggestVarOrType_Elsewhere

namespace LionWeb_CSharp_Test.tests;

using LionWeb.Core;

[TestClass]
public class NullableLazyHashCodeTests
{
    #region NonNull

    [TestMethod]
    public void Same_NonNull_Set()
    {
        var value = new object();
        var lazy = new NullableLazy<object>(value);
        Assert.AreEqual(value.GetHashCode(), lazy.GetHashCode());
    }

    [TestMethod]
    public void Same_NonNull_Unset()
    {
        var lazy = new NullableLazy<object>(null);
        Assert.AreEqual(0, lazy.GetHashCode());
    }

    #endregion

    #region Nullable

    [TestMethod]
    public void Same_Nullable_Set()
    {
        var value = new object();
        var lazy = new NullableLazy<object?>(value);
        Assert.AreEqual(value.GetHashCode(), lazy.GetHashCode());
    }

    [TestMethod]
    public void Same_Nullable_Unset()
    {
        var lazy = new NullableLazy<object?>(null);
        Assert.AreEqual(0, lazy.GetHashCode());
    }

    #endregion
}