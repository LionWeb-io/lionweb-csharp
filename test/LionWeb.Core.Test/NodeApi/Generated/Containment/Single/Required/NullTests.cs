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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var parent = new OffsetDuplicate("od");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Offset = null);
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new OffsetDuplicate("od");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.SetOffset(null));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new OffsetDuplicate("od");
        Assert.ThrowsExactly<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, null));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Offset);
    }

    [TestMethod]
    public void Constructor()
    {
        Assert.ThrowsExactly<InvalidValueException>(
            () => new OffsetDuplicate("od") { Offset = null });
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new OffsetDuplicate("od");
        Assert.IsFalse(parent.TryGetOffset(out var o));
        Assert.IsNull(o);
    }
}