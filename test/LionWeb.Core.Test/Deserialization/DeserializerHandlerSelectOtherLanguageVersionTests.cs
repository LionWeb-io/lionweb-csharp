// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Deserialization;

using M1;
using M3;

[TestClass]
public class DeserializerHandlerSelectOtherLanguageVersionTests
{
    private readonly LionWebVersions _lionWebVersion= LionWebVersions.Current;

    [TestMethod]
    public void Empty()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "" };

        Assert.AreEqual(0, DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b));
    }

    #region Same

    [TestMethod]
    public void SameInt()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "1" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "1" };

        Assert.AreEqual(0, DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b));
    }

    [TestMethod]
    public void SameFourSegments()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "1.2.3.4" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "1.2.3.4" };

        Assert.AreEqual(0, DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b));
    }

    [TestMethod]
    public void SameSevenSegments()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "1.2.3.4.5.6.7" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "1.2.3.4.5.6.7" };

        Assert.AreEqual(0, DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b));
    }

    [TestMethod]
    public void SameChar()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "a" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "a" };

        Assert.AreEqual(0, DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b));
    }

    [TestMethod]
    public void SameText()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "hello" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "hello" };

        Assert.AreEqual(0, DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b));
    }

    #endregion

    #region Different

    [TestMethod]
    public void DifferentInt()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "1" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "2" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) > 0);
    }

    [TestMethod]
    public void DifferentFourSegments()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "1.2.33.4" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "1.2.3.4" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) < 0);
    }

    [TestMethod]
    public void DifferentIntPrefix()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "01" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "1" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) > 0);
    }

    [TestMethod]
    public void DifferentIntSecondPrefix()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "1.0" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "1.01" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) > 0);
    }

    [TestMethod]
    public void DifferentSevenSegments()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "13.2.30.4.0.06.7" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "13.2.30.4.0.06.77" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) > 0);
    }

    [TestMethod]
    public void DifferentChar()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "a" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "A" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) < 0);
    }

    [TestMethod]
    public void DifferentText()
    {
        var a = new DynamicLanguage("a", _lionWebVersion) { Version = "hello" };
        var b = new DynamicLanguage("b", _lionWebVersion) { Version = "helo" };

        Assert.IsTrue(DeserializerHandlerSelectOtherLanguageVersion.DefaultLanguageComparer(a, b) > 0);
    }

    #endregion
}