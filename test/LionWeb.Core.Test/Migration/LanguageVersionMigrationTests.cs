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
using SDTLangLanguage = Languages.Generated.V2025_1.SDTLang.SDTLangLanguage;

[TestClass]
public class LanguageVersionMigrationTests
{
    private TestContext testContextInstance;
    LionWebVersions lionWebVersion = LionWebVersions.v2024_1;

    [TestMethod]
    public async Task LanguageIdenticalSameVersion()
    {
        var input = new Circle("c");

        await Assert.ThrowsExceptionAsync<IllegalMigrationStateException>(() => Migrate([input],
            [new LanguageVersionMigration.VersionMapping(ShapesLanguage.Instance, ShapesLanguage.Instance.Version)]));
    }

    [TestMethod]
    public async Task LanguageIdenticalOtherVersion()
    {
        var input = new Circle("c");

        var output = new MemoryStream();
        var result = await Migrate([input],
        [
            new LanguageVersionMigration.VersionMapping(ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Version,
                "asdf")
        ], output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains("""version":"asdf"""), str);
    }

    [TestMethod]
    public async Task MultipleLanguages()
    {
        var inputA = new Circle("c");
        var inputB = new SDTConcept("s");
        
        var output = new MemoryStream();
        var result = await Migrate([inputA, inputB],
        [
            new LanguageVersionMigration.VersionMapping(ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Version,
                "asdf"),
            new LanguageVersionMigration.VersionMapping(SDTLangLanguage.Instance.Key, SDTLangLanguage.Instance.Version,
                "qwer"),
        ], output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains("""version":"asdf"""), str);
        Assert.IsTrue(str.Contains("""version":"qwer"""), str);
    }

    [TestMethod]
    public async Task NotAllLanguages()
    {
        var inputA = new Circle("c");
        var inputB = new SDTConcept("s");
        
        var output = new MemoryStream();
        var result = await Migrate([inputA, inputB],
        [
            new LanguageVersionMigration.VersionMapping(SDTLangLanguage.Instance.Key, SDTLangLanguage.Instance.Version,
                "qwer"),
        ], output);

        Assert.IsTrue(result);
        var str = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(str.Contains($"""version":"{ShapesLanguage.Instance.Version}"""), str);
        Assert.IsTrue(str.Contains("""version":"qwer"""), str);
    }

    private async Task<bool> Migrate(IEnumerable<IReadableNode> inputNodes,
        HashSet<LanguageVersionMigration.VersionMapping> versionMappings,
        Stream? outputStream = null)
    {
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build(), inputNodes);
        inputStream.Seek(0, SeekOrigin.Begin);

        var migrator = new ModelMigrator(lionWebVersion, [ShapesLanguage.Instance]);
        migrator.RegisterMigration(new LanguageVersionMigration(versionMappings) { Priority = 0 });

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