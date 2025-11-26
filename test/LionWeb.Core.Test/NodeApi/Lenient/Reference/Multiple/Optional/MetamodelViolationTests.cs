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

namespace LionWeb.Core.Test.NodeApi.Lenient.Reference.Multiple.Optional;

[TestClass]
public class MetamodelViolationTests : LenientNodeTestsBase
{
    [TestMethod]
    public void String_Reflective()
    {
        var parent = newReferenceGeometry("od");
        var value = "a";
        parent.Set(ReferenceGeometry_shapes, value);
        Assert.AreEqual("a", parent.Get(ReferenceGeometry_shapes));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newReferenceGeometry("od");
        var value = -10;
        parent.Set(ReferenceGeometry_shapes, value);
        Assert.AreEqual(-10, parent.Get(ReferenceGeometry_shapes));
    }
}