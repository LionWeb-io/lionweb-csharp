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

namespace LionWeb.Core.Test.Utilities;

using Core.Utilities;

[TestClass]
public class IdUtilsTests
{
    [TestMethod]
    public void IsValid_null() =>
        Assert.IsFalse(IdUtils.IsValid(null!));

    [TestMethod]
    public void IsValid_invalidDot() =>
        Assert.IsFalse(IdUtils.IsValid("my.id"));

    [TestMethod]
    public void IsValid_emptyString() =>
        Assert.IsFalse(IdUtils.IsValid(""));

    [TestMethod]
    public void IsValid_onlySpace() =>
        Assert.IsFalse(IdUtils.IsValid(" "));

    [TestMethod]
    public void IsValid_invalidSpace() =>
        Assert.IsFalse(IdUtils.IsValid("my id"));

    [TestMethod]
    public void IsValid_startWithNumber() =>
        Assert.IsTrue(IdUtils.IsValid("12abc"));

    [TestMethod]
    public void IsValid_onlyNumber() =>
        Assert.IsTrue(IdUtils.IsValid("23"));

    [TestMethod]
    public void IsValid_dash() =>
        Assert.IsTrue(IdUtils.IsValid("-"));

    [TestMethod]
    public void IsValid_underscore() =>
        Assert.IsTrue(IdUtils.IsValid("_"));

    [TestMethod]
    public void EncodeBase64Url_equals() =>
        Assert.AreEqual("YWI", IdUtils.EncodeBase64Url("ab"));

    [TestMethod]
    public void EncodeBase64Url_underscore() =>
        Assert.AreEqual("w5_Dtg", IdUtils.EncodeBase64Url("ßö"));

    [TestMethod]
    public void EncodeBase64Url_dash() =>
        Assert.AreEqual("WcK-", IdUtils.EncodeBase64Url("Y¾"));
}