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

namespace LionWeb.Generator.Cli.Test.Configuration;

using Configuration = Cli.Configuration;

[TestClass]
public class GetNamespaceTests : ConfigurationTestsBase
{
    [TestMethod]
    public void OnlyNamespace()
    {
        var configuration = new Configuration { Namespace = "a.b.c" };

        Assert.AreEqual("a.b.c", configuration.GetNamespace(LanguageNoDots()));
    }

    [TestMethod]
    public void OnlyNamespace_Empty()
    {
        var configuration = new Configuration { Namespace = "" };

        Assert.AreEqual("", configuration.GetNamespace(LanguageNoDots()));
    }

    [TestMethod]
    public void Namespace_Pattern()
    {
        var configuration = new Configuration { Namespace = "a.b.c", NamespacePattern = NamespacePattern.DotSeparated };

        Assert.AreEqual("a.b.c", configuration.GetNamespace(LanguageNoDots()));
    }

    [TestMethod]
    public void Namespace_Empty_Pattern()
    {
        var configuration = new Configuration { Namespace = "", NamespacePattern = NamespacePattern.DotSeparated };

        Assert.AreEqual("", configuration.GetNamespace(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NoUpper_NoDots()
    {
        var configuration = new Configuration { NamespacePattern = NamespacePattern.DotSeparated };

        Assert.AreEqual("myLang", configuration.GetNamespace(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_Upper_NoDots()
    {
        var configuration = new Configuration { NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase };

        Assert.AreEqual("MyLang", configuration.GetNamespace(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NoUpper_WithDots()
    {
        var configuration = new Configuration { NamespacePattern = NamespacePattern.DotSeparated };

        Assert.AreEqual("A.Bee.cee.d.@a.myLang", configuration.GetNamespace(LanguageWithDots()));
    }

    [TestMethod]
    public void Pattern_Upper_WithDots()
    {
        var configuration = new Configuration { NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase };

        Assert.AreEqual("A.Bee.Cee.D.@a.MyLang", configuration.GetNamespace(LanguageWithDots()));
    }

    [TestMethod]
    public void UnknownNamespacePattern_Null()
    {
        var configuration = new Configuration { NamespacePattern = null };
        var ex = Assert.Throws<UnknownEnumValueException<NamespacePattern?>>(() =>
            configuration.GetNamespace(LanguageNoDots()));

        Assert.AreEqual("Unknown NamespacePattern: null", ex.Message);
    }

    [TestMethod]
    public void UnknownNamespacePattern_InvalidInt()
    {
        var configuration = new Configuration { NamespacePattern = (NamespacePattern?)int.MaxValue };
        var ex = Assert.Throws<UnknownEnumValueException<NamespacePattern>>(() =>
            configuration.GetNamespace(LanguageNoDots()));

        Assert.AreEqual($"Unknown NamespacePattern: {int.MaxValue}", ex.Message);
    }

    [TestMethod]
    public void NamespacePattern_ValidInt()
    {
        var configuration = new Configuration { NamespacePattern = (NamespacePattern?)1 };
        Assert.AreEqual("myLang", configuration.GetNamespace(LanguageNoDots()));
    }
}