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

namespace LionWeb_CSharp_Test.tests;

using Examples.SDTLang;
using LionWeb.Core;

[TestClass]
public class StructuredDataTypeTests
{
    [TestMethod]
    public void Empty()
    {
        var fqn = new FullyQualifiedName();

        Assert.ThrowsException<UnsetFieldException>(() => fqn.Name);
    }

    [TestMethod]
    public void Empty_ToString()
    {
        var fqn = new FullyQualifiedName();

        Assert.ThrowsException<UnsetFieldException>(() => fqn.ToString());
    }

    [TestMethod]
    public void Empty_GetHashCode()
    {
        var fqn = new FullyQualifiedName();

        Assert.AreEqual(0, fqn.GetHashCode());
    }

    [TestMethod]
    public void Empty_Equals()
    {
        var fqn = new FullyQualifiedName();

        Assert.IsTrue(fqn.Equals(new FullyQualifiedName()));
    }

    [TestMethod]
    public void Nested()
    {
        var fqn = new FullyQualifiedName
        {
            Name = "a",
            Nested = new FullyQualifiedName { Name = "b", Nested = new FullyQualifiedName { Name = "c" } }
        };

        Assert.AreEqual(
            "FullyQualifiedName { Name = a, Nested = FullyQualifiedName { Name = b, Nested = FullyQualifiedName { Name = c, Nested =  } } }",
            fqn.ToString());
    }

    [TestMethod]
    public void Nested_Null()
    {
        var fqn = new FullyQualifiedName
        {
            Name = "a",
            Nested = new FullyQualifiedName
            {
                Name = "b", Nested = new FullyQualifiedName { Name = "c", Nested = null }
            }
        };

        Assert.AreEqual(
            "FullyQualifiedName { Name = a, Nested = FullyQualifiedName { Name = b, Nested = FullyQualifiedName { Name = c, Nested =  } } }",
            fqn.ToString());
    }
}