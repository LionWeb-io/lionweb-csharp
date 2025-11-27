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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.GenericApi;

using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var parent = new Geometry("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(ShapesLanguage.Instance.Geometry_shapes, null));
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new Geometry("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, null));
    }

    [TestMethod]
    public void Insert_Empty_OutOfBounds()
    {
        var parent = new Geometry("g");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 1, [null]));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new Geometry("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(ShapesLanguage.Instance.Geometry_shapes, null));
    }
}