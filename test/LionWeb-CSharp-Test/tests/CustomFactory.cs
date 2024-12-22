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

using Examples.V2024_1.Shapes.M2;

[TestClass]
public class CustomFactory
{
    [TestMethod]
    public void CustomIdFactory()
    {
        int i = 0;
        var factory = new CustomShapesFactory(ShapesLanguage.Instance, () => i++.ToString());

        var line0 = factory.CreateLine();
        var line1 = factory.CreateLine();
        var line2 = factory.CreateLine();
        
        Assert.AreEqual("0", line0.GetId());
        Assert.AreEqual("1", line1.GetId());
        Assert.AreEqual("2", line2.GetId());
    }
}

class CustomShapesFactory : ShapesFactory
{
    private readonly Func<string> _idProvider;

    public CustomShapesFactory(ShapesLanguage language, Func<string> idProvider) : base(language)
    {
        _idProvider = idProvider;
    }

    protected override string GetNewId() => _idProvider();
}