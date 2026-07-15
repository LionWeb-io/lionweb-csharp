// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.GlobalM2Cache;

using M2;

[TestClass]
public class GlobalM2CacheTests : IDisposable
{
    public void Dispose() =>
        IGlobalM2Cache.Disable();

    [TestMethod]
    public void NoCacheByDefault() =>
        Assert.IsNull(IGlobalM2Cache.Instance);
    
    [TestMethod]
    public void EnableWithoutPrior()
    {
        var cache = IGlobalM2Cache.Enable();
        Assert.IsNotNull(cache);
        Assert.AreSame(cache, IGlobalM2Cache.Instance);
    }
    
    [TestMethod]
    public void EnableWithPrior()
    {
        var cache = IGlobalM2Cache.Enable();
        Assert.AreSame(cache, IGlobalM2Cache.Enable());
        Assert.AreSame(cache, IGlobalM2Cache.Instance);
    }
    
    [TestMethod]
    public void DisableWithoutPrior()
    {
        IGlobalM2Cache.Disable();
        Assert.IsNull(IGlobalM2Cache.Instance);
    }
    
    [TestMethod]
    public void DisableWithPrior()
    {
        Assert.IsNotNull(IGlobalM2Cache.Enable());
        Assert.IsNotNull(IGlobalM2Cache.Instance);
        IGlobalM2Cache.Disable();
        Assert.IsNull(IGlobalM2Cache.Instance);
    }
}