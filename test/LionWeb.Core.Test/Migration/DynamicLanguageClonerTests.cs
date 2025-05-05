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
using Core.Utilities;
using Languages.Generated.V2023_1.Shapes.M2;
using M3;

[TestClass]
public class DynamicLanguageClonerTests
{
    [TestMethod]
    public void DifferentLionWebVersion()
    {
        var shapesLanguage = ShapesLanguage.Instance;
        Assert.AreSame(LionWebVersions.v2023_1, shapesLanguage.LionWebVersion);

        var cloner = new DynamicLanguageCloner(LionWebVersions.v2024_1);
        var dynamicLanguages = cloner.Clone([shapesLanguage]);

        Assert.AreSame(LionWebVersions.v2024_1, dynamicLanguages.Values.First().LionWebVersion);
    }

    [TestMethod]
    public void Language_Empty()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual(inputLang, dynamicLanguages.Values.First());
        AssertEqual(inputLang, cloner.DynamicMap[inputLang]);
    }

    [TestMethod]
    public void MigrationFactory()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        Assert.IsInstanceOfType<MigrationFactory>(dynamicLanguages.Values.First().GetFactory());
    }

    [TestMethod]
    public void Language_InternalDependsOn()
    {
        var inputLangA = new DynamicLanguage("langA", LionWebVersions.Current)
        {
            Key = "key-langA", Name = "langA", Version = "version"
        };
        var inputLangB = new DynamicLanguage("langB", LionWebVersions.Current)
        {
            Key = "key-langB", Name = "langB", Version = "version"
        };
        inputLangA.AddDependsOn([inputLangB]);

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLangA, inputLangB]);

        AssertEqual([inputLangA, inputLangB], dynamicLanguages);
    }

    [TestMethod]
    public void Language_ExternalDependsOn()
    {
        var inputLangA = new DynamicLanguage("langA", LionWebVersions.Current)
        {
            Key = "key-langA", Name = "langA", Version = "version"
        };
        var inputLangB = new DynamicLanguage("langB", LionWebVersions.Current)
        {
            Key = "key-langB", Name = "langB", Version = "version"
        };
        inputLangA.AddDependsOn([inputLangB]);

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);

        Assert.ThrowsException<UnknownLookupException>(() => cloner.Clone([inputLangA]));
    }

    [TestMethod]
    public void Language_ExternalDependsOn_Listed()
    {
        var inputLangA = new DynamicLanguage("langA", LionWebVersions.Current)
        {
            Key = "key-langA", Name = "langA", Version = "version"
        };
        var inputLangB = new DynamicLanguage("langB", LionWebVersions.Current)
        {
            Key = "key-langB", Name = "langB", Version = "version"
        };
        inputLangA.AddDependsOn([inputLangB]);

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current, [inputLangB]);

        var dynamicLanguages = cloner.Clone([inputLangA]);

        AssertEqual([inputLangA], dynamicLanguages);
        Assert.AreEqual(1, dynamicLanguages.Values.Count);
        Assert.AreSame(inputLangB, dynamicLanguages.Values.First().DependsOn[0]);
    }

    [TestMethod]
    public void Annotation()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var iface = inputLang.Interface("iface", "key-iface", "name-iface");

        var superAnn = inputLang.Annotation("superAnn", "key-superAnn", "name-superAnn");

        var ann = inputLang
            .Annotation("ann", "key-ann", "name-ann")
            .Implementing(iface, LionWebVersions.Current.BuiltIns.INamed)
            .Extending(superAnn);

        ann.Annotating(ann);

        var prop = ann.Property("prop", "key-prop", "name-prop").OfType(LionWebVersions.Current.BuiltIns.String);
        var cont = ann.Containment("cont", "key-cont", "name-cont").OfType(LionWebVersions.Current.BuiltIns.Node);
        var refe = ann.Reference("ref", "key-ref", "name-ref").OfType(LionWebVersions.Current.BuiltIns.INamed);

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual([inputLang], dynamicLanguages);
        AssertEqual(ann, cloner.DynamicMap[ann]);
        AssertEqual(prop, cloner.DynamicMap[prop]);
        AssertEqual(cont, cloner.DynamicMap[cont]);
        AssertEqual(refe, cloner.DynamicMap[refe]);
    }

    [TestMethod]
    public void Concept()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var iface = inputLang.Interface("iface", "key-iface", "name-iface");

        var superCon = inputLang.Concept("superCon", "key-superCon", "name-superCon");

        var con = inputLang
            .Concept("con", "key-con", "name-con")
            .Implementing(iface, LionWebVersions.Current.BuiltIns.INamed)
            .Extending(superCon);

        var prop = con.Property("prop", "key-prop", "name-prop").OfType(LionWebVersions.Current.BuiltIns.String);
        var cont = con.Containment("cont", "key-cont", "name-cont").OfType(LionWebVersions.Current.BuiltIns.Node);
        var refe = con.Reference("ref", "key-ref", "name-ref").OfType(LionWebVersions.Current.BuiltIns.INamed);

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual([inputLang], dynamicLanguages);
        AssertEqual(con, cloner.DynamicMap[con]);
        AssertEqual(prop, cloner.DynamicMap[prop]);
        AssertEqual(cont, cloner.DynamicMap[cont]);
        AssertEqual(refe, cloner.DynamicMap[refe]);
    }

    [TestMethod]
    public void Iface()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var superIface = inputLang.Interface("superIface", "key-superIface", "name-superIface");

        var iface = inputLang
            .Interface("iface", "key-iface", "name-iface")
            .Extending(superIface, LionWebVersions.Current.BuiltIns.INamed);

        var prop = iface.Property("prop", "key-prop", "name-prop").OfType(LionWebVersions.Current.BuiltIns.String);
        var cont = iface.Containment("cont", "key-cont", "name-cont").OfType(LionWebVersions.Current.BuiltIns.Node);
        var refe = iface.Reference("ref", "key-ref", "name-ref").OfType(LionWebVersions.Current.BuiltIns.INamed);

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual([inputLang], dynamicLanguages);
        AssertEqual(iface, cloner.DynamicMap[iface]);
        AssertEqual(prop, cloner.DynamicMap[prop]);
        AssertEqual(cont, cloner.DynamicMap[cont]);
        AssertEqual(refe, cloner.DynamicMap[refe]);
    }

    [TestMethod]
    public void Enm()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var enm = inputLang
            .Enumeration("enm", "key-enm", "name-enm");

        var litA = enm.EnumerationLiteral("litA", "key-litA", "name-litA");
        var litB = enm.EnumerationLiteral("litB", "key-litB", "name-litB");

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual([inputLang], dynamicLanguages);
        AssertEqual(enm, cloner.DynamicMap[enm]);
        AssertEqual(litA, cloner.DynamicMap[litA]);
        AssertEqual(litB, cloner.DynamicMap[litB]);
    }

    [TestMethod]
    public void Primitive()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var primitive = inputLang
            .PrimitiveType("primitive", "key-primitive", "name-primitive");

        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual([inputLang], dynamicLanguages);
        AssertEqual(primitive, cloner.DynamicMap[primitive]);
    }

    [TestMethod]
    public void Sdt()
    {
        var inputLang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Key = "key-lang", Name = "lang", Version = "version"
        };

        var sdt = inputLang
            .StructuredDataType("sdt", "key-sdt", "name-sdt");

        var enm = inputLang
            .Enumeration("enm", "key-enm", "name-enm");
        
        var fieldA = sdt.Field("fieldA", "key-fieldA", "name-fieldA").OfType(LionWebVersions.Current.BuiltIns.Boolean);
        var fieldB = sdt.Field("fieldB", "key-fieldB", "name-fieldB").OfType(enm);
        
        var cloner = new DynamicLanguageCloner(LionWebVersions.Current);
        var dynamicLanguages = cloner.Clone([inputLang]);

        AssertEqual([inputLang], dynamicLanguages);
        AssertEqual(sdt, cloner.DynamicMap[sdt]);
        AssertEqual(fieldA, cloner.DynamicMap[fieldA]);
        AssertEqual(fieldB, cloner.DynamicMap[fieldB]);
    }

    private void AssertEqual(IReadableNode expected, IReadableNode actual) =>
        AssertEqual([expected], [actual]);

    private void AssertEqual(List<Language> expected, IDictionary<LanguageIdentity, DynamicLanguage> mapping) =>
        AssertEqual(expected, expected.Select(e => mapping[LanguageIdentity.FromLanguage(e)]).ToList());

    private void AssertEqual(IEnumerable<IReadableNode> expected, IEnumerable<IReadableNode> actual)
    {
        var comparer = new Comparer(expected.ToList(), actual.ToList());
        var differences = comparer.Compare().ToList();
        Assert.AreEqual(0, differences.Count,
            differences.DescribeAll(new() { LeftDescription = "expected", RightDescription = "actual" }));
    }
}