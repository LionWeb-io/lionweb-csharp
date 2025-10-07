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

namespace LionWeb.Generator.Cli.Test;

[TestClass]
public class NamespaceUtilTests
{
    [TestMethod]
    public void BeginNumber() => AssertInvalid("0a");

    [TestMethod]
    public void EndNumber() => AssertValid("a0");

    [TestMethod]
    public void UnderscoreNumber() => AssertValid("_0");

    [TestMethod]
    public void SingleChar() => AssertValid("a");

    [TestMethod]
    public void SingleUnderscore() => AssertValid("_");

    [TestMethod]
    public void MultiUnderscore() => AssertValid("__");

    [TestMethod]
    public void InnerUnderscore() => AssertValid("a_0");

    [TestMethod]
    public void Combined() => AssertValid(@"A.ü._.x0.\u1234.Ⅲ12Ⅲ34Ⅲ.__.\u12345678.Name.if.@if.@hello");

    private void AssertInvalid(string candidate) =>
        Assert.IsFalse(NamespaceUtil.NamespaceRegex().IsMatch(candidate));

    private void AssertValid(string candidate) =>
        Assert.IsTrue(NamespaceUtil.NamespaceRegex().IsMatch(candidate));
}