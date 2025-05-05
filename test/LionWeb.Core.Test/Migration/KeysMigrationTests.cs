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
using Core.Serialization;
using Languages.Generated.V2024_1.SDTLang;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using System.Text;

[TestClass]
public class KeysMigrationTests
{
    private TestContext testContextInstance;
    LionWebVersions lionWebVersion = LionWebVersions.v2024_1;

    [TestMethod]
    public async Task Empty()
    {
        var input = new Circle("c");

        var result = await Migrate([input], []);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task NotApplicable()
    {
        var input = new Circle("c");

        var result = await Migrate([input], new() { { "x", "y" } });

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task LanguageNotMentioned()
    {
        var input = new Circle("c");

        var result = await Migrate([input], new() { { ShapesLanguage.Instance.Circle.Key, "asdf" } });

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task LanguageIdentical()
    {
        var input = new Circle("c");

        var output = new MemoryStream();
        var result = await Migrate([input], new() { { ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Key } }, output);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Language()
    {
        var input = new Circle("c");

        var output = new MemoryStream();
        var result = await Migrate([input], new() { { ShapesLanguage.Instance.Key, "asdf" } }, output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains("""language":"asdf"""), str);
    }

    [TestMethod]
    public async Task Classifier()
    {
        var input = new Circle("c");

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Key },
                { ShapesLanguage.Instance.Circle.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains("""key":"asdf"""), str);
    }

    [TestMethod]
    public async Task Feature()
    {
        var input = new Circle("c") { R = 15 };

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Key },
                { ShapesLanguage.Instance.Circle_r.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains("""key":"asdf"""), str);
    }

    [TestMethod]
    public async Task Enumeration()
    {
        var input = new MaterialGroup("c") { MatterState = MatterState.liquid };

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Key },
                { ShapesLanguage.Instance.MatterState.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task EnumerationLiteral()
    {
        var input = new MaterialGroup("c") { MatterState = MatterState.liquid };

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Key },
                { ShapesLanguage.Instance.MatterState_liquid.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains("""value":"asdf"""), str);
    }

    [TestMethod]
    public async Task Sdt()
    {
        var input = new SDTConcept("c") { Decimal = new Decimal(3, 5) };

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { SDTLangLanguage.Instance.Key, SDTLangLanguage.Instance.Key },
                { SDTLangLanguage.Instance.Decimal.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Field()
    {
        var input = new SDTConcept("c") { Decimal = new Decimal(3, 5) };

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { SDTLangLanguage.Instance.Key, SDTLangLanguage.Instance.Key },
                { SDTLangLanguage.Instance.Decimal_int.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains(@"\u0022asdf\u0022:"), str);
    }

    [TestMethod]
    public async Task FieldWithEnum()
    {
        var input = new SDTConcept("c") { Amount = new Amount(Currency.GBP, true, new Decimal(3, 5)) };

        var output = new MemoryStream();
        var result = await Migrate([input],
            new()
            {
                { SDTLangLanguage.Instance.Key, SDTLangLanguage.Instance.Key },
                { SDTLangLanguage.Instance.Currency_GBP.Key, "asdf" }
            }, output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains(@":\u0022asdf\u0022"), str);
    }

    private async Task<bool> Migrate(IEnumerable<IReadableNode> inputNodes, Dictionary<string, string> keyMapping,
        Stream? outputStream = null)
    {
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build(), inputNodes);
        inputStream.Seek(0, SeekOrigin.Begin);

        var migrator = new ModelMigrator(lionWebVersion, [ShapesLanguage.Instance, SDTLangLanguage.Instance]);
        migrator.RegisterMigration(new KeysMigration(keyMapping) { Priority = 0 });

        var result = await migrator.MigrateAsync(inputStream, outputStream ?? Stream.Null);
        if (outputStream != null)
            outputStream.Seek(0, SeekOrigin.Begin);

        return result;
    }

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}