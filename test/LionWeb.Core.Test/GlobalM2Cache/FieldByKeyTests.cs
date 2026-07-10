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

namespace LionWeb.Core.Test.GlobalM2Cache;

using Languages.Generated.V2024_1.CustomPrimitiveTypeLang;
using Languages.Generated.V2024_1.SDTLang;
using M2;

[TestClass]
public class FieldByKeyTests
{
    [TestMethod]
    public void KnownField()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FieldByKey(language.A, language.A_a2b.Key);
        Assert.AreSame(language.A_a2b, actual);
    }

    [TestMethod]
    public void WrongSdt()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FieldByKey(language.A, language.B_b2d.Key);
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void UnknownKey()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FieldByKey(language.A, "asdf");
        Assert.IsNull(actual);
    }

    [TestMethod]
    public void UnknownClassifier()
    {
        var cache = new M2Cache();
        var language = SDTLangLanguage.Instance;
        cache.Register([language]);
        var actual = cache.FieldByKey(CustomPrimitiveTypeLangLanguage.Instance.Sdt, "asdf");
        Assert.IsNull(actual);
    }
}