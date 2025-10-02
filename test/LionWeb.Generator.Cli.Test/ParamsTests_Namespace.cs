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
public class ParamsTests_Namespace : ParamsTestsBase
{
    [TestMethod]
    public void Namespace_Empty()
    {
        var generatedFile = new FileInfo("out/TestLanguage.g.cs");
        Delete(generatedFile);

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--namespace", "",
            "--output", "out",
            TestLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguage2024),
                OutputDir = new DirectoryInfo("out"),
                Namespace = "",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains("namespace;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void Namespace_Valid()
    {
        var generatedFile = new FileInfo("out/TestLanguage.g.cs");
        Delete(generatedFile);

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--namespace", "a.b.c",
            "--output", "out",
            TestLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguage2024),
                OutputDir = new DirectoryInfo("out"),
                Namespace = "a.b.c",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains("namespace a.b.c;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void Namespace_Invalid()
    {
        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--namespace", "!a",
            "--output", "out",
            TestLanguage2024
        ]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("Not a valid namespace: !a", generator.Errors);
    }

    [TestMethod]
    public void NamespacePattern_DotSeparated()
    {
        Delete(new DirectoryInfo("out"));

        var generatedFile = new FileInfo(@"out/a.B.cee.Dee.@if.else._0.\u1234.\U1234aBcd.TestLanguage.g.cs");

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--namespacePattern", nameof(NamespacePattern.DotSeparated),
            "--output", "out",
            TestLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparated,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains(@"namespace a.B.cee.Dee.@if.@else._0.\u1234.\U1234aBcd.TestLanguage;",
            File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void NamespacePattern_DotSeparatedFirstUppercase()
    {
        Delete(new DirectoryInfo("out"));

        var generatedFile = new FileInfo(@"out/a.B.cee.Dee.@if.else._0.\u1234.\U1234aBcd.TestLanguage.g.cs");

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--namespacePattern", nameof(NamespacePattern.DotSeparatedFirstUppercase),
            "--output", "out",
            TestLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains(@"namespace A.B.Cee.Dee.@if.Else._0.\u1234.\U1234aBcd.TestLanguage;",
            File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PathPattern_NamespaceAsPath_DotSeparated()
    {
        Delete(new DirectoryInfo("out"));

        var generatedFile = new FileInfo("out/a/B/cee/Dee/@if/else/_0/_u1234/_U1234aBcd/TestLanguage.g.cs");

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--pathPattern", nameof(PathPattern.NamespaceAsPath),
            "--namespacePattern", nameof(NamespacePattern.DotSeparated),
            "--output", "out",
            TestLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparated,
                PathPattern = PathPattern.NamespaceAsPath,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains(@"namespace a.B.cee.Dee.@if.@else._0.\u1234.\U1234aBcd.TestLanguage;",
            File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PathPattern_NamespaceAsPath_DotSeparatedFirstUppercase()
    {
        Delete(new DirectoryInfo("out"));

        var generatedFile = new FileInfo("out/A/B/Cee/Dee/@if/Else/_0/_u1234/_U1234aBcd/TestLanguage.g.cs");

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--pathPattern", nameof(PathPattern.NamespaceAsPath),
            "--namespacePattern", nameof(NamespacePattern.DotSeparatedFirstUppercase),
            "--output", "out",
            TestLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase,
                PathPattern = PathPattern.NamespaceAsPath,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains(@"namespace A.B.Cee.Dee.@if.Else._0.\u1234.\U1234aBcd.TestLanguage;",
            File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PathPattern_NamespaceAsFilename()
    {
        Delete(new DirectoryInfo("out"));

        var generatedFile = new FileInfo("out/A.B.Cee.Dee.@if.Else._0._u1234._U1234aBcd.TestLanguage.g.cs");

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--pathPattern", nameof(PathPattern.NamespaceAsFilename),
            "--namespacePattern", nameof(NamespacePattern.DotSeparatedFirstUppercase),
            "--output", "out",
            TestLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase,
                PathPattern = PathPattern.NamespaceAsFilename,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains(@"namespace A.B.Cee.Dee.@if.Else._0.\u1234.\U1234aBcd.TestLanguage;",
            File.ReadAllText(generatedFile.FullName));
    }
}