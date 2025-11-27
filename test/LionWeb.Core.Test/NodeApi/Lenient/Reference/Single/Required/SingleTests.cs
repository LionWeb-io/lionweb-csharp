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

namespace LionWeb.Core.Test.NodeApi.Lenient.Reference.Single.Required;

[TestClass]
public class SingleTests : LenientNodeTestsBase
{
    [TestMethod]
    public void Reflective()
    {
        var parent = newOffsetDuplicate("od");
        var reference = newLine("myId");
        parent.Set(OffsetDuplicate_source, reference);
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Get(OffsetDuplicate_source));
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldReference = newLine("old");
        var parent = newOffsetDuplicate("od");
        parent.Set(OffsetDuplicate_source, oldReference);
        var reference = newLine("myId");
        parent.Set(OffsetDuplicate_source, reference);
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.Get(OffsetDuplicate_source));
    }
}