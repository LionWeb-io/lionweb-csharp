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

namespace LionWeb.Core.Test.Migration;

using Core.Migration;
using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class MigrationResultTests
{
    [TestMethod]
    public void Validate_OutputNull_ChangedTrue()
    {
        List<LenientNode> input = [new LenientNode("l", ShapesLanguage.Instance.Line)];
        List<LenientNode> output = null;

        Assert.ThrowsException<ArgumentException>(() => new MigrationResult(true, output).Validate(input));
    }

    [TestMethod]
    public void Validate_OutputNull_ChangedFalse()
    {
        List<LenientNode> input = [new LenientNode("l", ShapesLanguage.Instance.Line)];
        List<LenientNode> output = null;

        Assert.ThrowsException<ArgumentException>(() => new MigrationResult(false, output).Validate(input));
    }

    [TestMethod]
    public void Validate_OutputEmpty_ChangedFalse()
    {
        List<LenientNode> input = [new LenientNode("l", ShapesLanguage.Instance.Line)];
        List<LenientNode> output = [];

        Assert.ThrowsException<ArgumentException>(() => new MigrationResult(false, output).Validate(input));
    }
    
    [TestMethod]
    public void Validate_OutputEmpty_ChangedFalse_InputEmpty()
    {
        List<LenientNode> input = [];
        List<LenientNode> output = [];

        Assert.IsNotNull(new MigrationResult(false, output).Validate(input));
    }
    
    [TestMethod]
    public void Validate_ChangedFalse_OutputInputEqual()
    {
        List<LenientNode> input = [new LenientNode("l", ShapesLanguage.Instance.Line)];
        List<LenientNode> output = [new LenientNode("l", ShapesLanguage.Instance.Line)];

        Assert.IsNotNull(new MigrationResult(false, output).Validate(input));
    }
    
    [TestMethod]
    public void Validate_ChangedFalse_OutputInputNotEqual()
    {
        List<LenientNode> input = [new LenientNode("l", ShapesLanguage.Instance.Line)];
        List<LenientNode> output = [new LenientNode("l", ShapesLanguage.Instance.Circle)];

        Assert.ThrowsException<ArgumentException>(() => new MigrationResult(false, output).Validate(input));
    }
}