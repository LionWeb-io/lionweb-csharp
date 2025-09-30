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
public class ParamsTests
{
    private const string _resourceDir = "../../../resources";
    private const string _testLanguage2023 = $"{_resourceDir}/testLanguage.2023_1.json";
    private const string _testLanguage2024 = $"{_resourceDir}/testLanguage.2024_1.json";
    private const string _testLanguageNamespace = $"{_resourceDir}/testLanguage.namespace.2024_1.json";
    private const string _partialConfig = $"{_resourceDir}/partial.config.json";
    private const string _completeConfig = $"{_resourceDir}/complete.config.json";
    private const string _relativeOutputDir = $"{_resourceDir}/out";

    [TestMethod]
    public void None()
    {
        var generator = new LionWebGenerator();
        var result = generator.Exec([]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);
    }

    [TestMethod]
    public void OnlyLanguageFile()
    {
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            _testLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);
    }

    [TestMethod]
    public void LanguageFile_NonExistent()
    {
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "asf"
        ]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);
    }

    [TestMethod]
    public void Namespace_Empty()
    {
        var generatedFile = new FileInfo("out/TestLanguage.g.cs");
        Delete(generatedFile);
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--namespace", "",
            "--output", "out",
            _testLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguage2024),
                OutputDir = new DirectoryInfo("out"),
                Namespace = "",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains("namespace;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void Namespace_Valid()
    {
        var generatedFile = new FileInfo("out/TestLanguage.g.cs");
        Delete(generatedFile);
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--namespace", "a.b.c",
            "--output", "out",
            _testLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguage2024),
                OutputDir = new DirectoryInfo("out"),
                Namespace = "a.b.c",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains("namespace a.b.c;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void Namespace_Invalid()
    {
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--namespace", "!a",
            "--output", "out",
            _testLanguage2024
        ]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);
    }

    [TestMethod]
    public void NamespacePattern_DotSeparated()
    {
        Delete(new DirectoryInfo("out"));
        
        var generatedFile = new FileInfo("out/a.B.cee.Dee.@if.else/u1234/U1234aBcd.TestLanguage.g.cs");
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--namespacePattern", nameof(NamespacePattern.DotSeparated),
            "--output", "out",
            _testLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparated,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains(@"namespace a.B.cee.Dee.@if.@else.\u1234.\U1234aBcd.TestLanguage;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void NamespacePattern_DotSeparatedFirstUppercase()
    {
        Delete(new DirectoryInfo("out"));
        
        var generatedFile = new FileInfo("out/A.B.Cee.Dee.@if.Else/u1234/U1234aBcd.TestLanguage.g.cs");
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--namespacePattern", nameof(NamespacePattern.DotSeparatedFirstUppercase),
            "--output", "out",
            _testLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains(@"namespace A.B.Cee.Dee.@if.Else.\u1234.\U1234aBcd.TestLanguage;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PathPattern_NamespaceAsPath_DotSeparated()
    {
        Delete(new DirectoryInfo("out"));
        
        var generatedFile = new FileInfo("out/a/B/cee/Dee/@if/else/_u1234/_U1234aBcd/TestLanguage.g.cs");
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--pathPattern", nameof(PathPattern.NamespaceAsPath),
            "--namespacePattern", nameof(NamespacePattern.DotSeparated),
            "--output", "out",
            _testLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparated,
                PathPattern = PathPattern.NamespaceAsPath,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains(@"namespace a.B.cee.Dee.@if.@else.\u1234.\U1234aBcd.TestLanguage;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PathPattern_NamespaceAsPath_DotSeparatedFirstUppercase()
    {
        Delete(new DirectoryInfo("out"));
        
        var generatedFile = new FileInfo("out/A/B/Cee/Dee/@if/else/_u1234/_U1234aBcd/TestLanguage.g.cs");
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--pathPattern", nameof(PathPattern.NamespaceAsPath),
            "--namespacePattern", nameof(NamespacePattern.DotSeparatedFirstUppercase),
            "--output", "out",
            _testLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase,
                PathPattern = PathPattern.NamespaceAsPath,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains(@"namespace A.B.Cee.Dee.@if.Else.\u1234.\U1234aBcd.TestLanguage;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PathPattern_NamespaceAsFilename()
    {
        Delete(new DirectoryInfo("out"));
        
        var generatedFile = new FileInfo("out/a.B.Cee.Dee.@if.else._u1234._U1234aBcd.TestLanguage.g.cs");
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--pathPattern", nameof(PathPattern.NamespaceAsFilename),
            "--namespacePattern", nameof(NamespacePattern.DotSeparatedFirstUppercase),
            "--output", "out",
            _testLanguageNamespace
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguageNamespace),
                OutputDir = new DirectoryInfo("out"),
                NamespacePattern = NamespacePattern.DotSeparatedFirstUppercase,
                PathPattern = PathPattern.NamespaceAsFilename,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig()
            }, generator.ValidConfigurations[0]);

        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains(@"namespace A.B.Cee.Dee.@if.Else.\u1234.\U1234aBcd.TestLanguage;", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void OutputDir_NonExistent()
    {
        var outputDir = new DirectoryInfo("asdf");
        Delete(outputDir);
        
        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--output", outputDir.ToString(),
            "--namespace", "",
            _testLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);
        AssertExists(new FileInfo($"{outputDir}/TestLanguage.g.cs"));
    }

    [TestMethod]
    public void OutputDir_Exists()
    {
        var outputDir = new DirectoryInfo("asdf");
        if (!outputDir.Exists)
            outputDir.Create();

        var generatedFile = new FileInfo($"{outputDir}/TestLanguage.g.cs");
        Delete(generatedFile);

        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--output", outputDir.ToString(),
            "--namespace", "",
            _testLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
    }

    [TestMethod]
    public void OutputFile_Exists()
    {
        Delete(new DirectoryInfo("out"));
        var generatedFile = new FileInfo($"out/x/y/MyFile.abc.xx");
        Delete(generatedFile);
        generatedFile.Directory!.Create();
        using (var writeOut = generatedFile.CreateText())
        {
            writeOut.WriteLine("hello");
        }
        AssertExists(generatedFile);
        Assert.Contains("hello", File.ReadAllText(generatedFile.FullName));

        var generator = new LionWebGenerator();
        var result = generator.Exec([
            "--outputFile", generatedFile.ToString(),
            "--namespace", "",
            _testLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);
        AssertExists(generatedFile);
        Assert.Contains("namespace", File.ReadAllText(generatedFile.FullName));
    }

    [TestMethod]
    public void PartialConfigFile_NoParams()
    {
        var generator = new LionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", _partialConfig]);

        Assert.HasCount(2, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);
    }

    [TestMethod]
    public void PartialConfigFile_AllRequiredParams()
    {
        var generator = new LionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");

        var relativeOutputDir = DeleteOutDir(out var outputDir);

        var result = generator.Exec([
            "--config", _partialConfig,
            "--output", relativeOutputDir
        ]);

        Assert.HasCount(2, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(-1, result);
        Assert.IsTrue(Directory.Exists(outputDir));
        AssertExists(new FileInfo($"{outputDir}/TestLanguage.g.cs"));
    }

    [TestMethod]
    public void CompleteConfigFile_Override()
    {
        var generator = new LionWebGenerator();
        var currDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"CurrentDir: {currDir}");

        var relativeOutputDir = DeleteOutDir(out var outputDir);

        var result = generator.Exec([
            "--config", _completeConfig,
            "--output", relativeOutputDir,
            "--namespace", "OtherNameSpace",
            "--lionWebVersion", "2024.1",
            "--writableInterfaces", "false",
            _testLanguage2024
        ]);

        Assert.HasCount(2, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(-1, result);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(new FileInfo(Path.Combine(currDir, _testLanguage2023)).FullName),
                OutputDir = new DirectoryInfo(relativeOutputDir),
                Namespace = "OtherNameSpace",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = false }
            }, generator.Configurations[0]);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(_testLanguage2024),
                OutputDir = new DirectoryInfo(relativeOutputDir),
                Namespace = "OtherNameSpace",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = false }
            }, generator.Configurations[1]);

        Assert.AreEqual(generator.Configurations[1], generator.ValidConfigurations[0]);

        Assert.IsTrue(Directory.Exists(outputDir));
        AssertExists(new FileInfo($"{outputDir}/TestLanguage.g.cs"));
    }

    private static string DeleteOutDir(out string outputDir)
    {
        outputDir = Path.Combine(Directory.GetCurrentDirectory(), _relativeOutputDir);
        if (!Directory.Exists(outputDir))
            return _relativeOutputDir;

        Directory.Delete(outputDir, true);
        Assert.IsFalse(Directory.Exists(outputDir), outputDir);
        return _relativeOutputDir;
    }

    private static void Delete(FileInfo fileInfo)
    {
        if (fileInfo.Exists)
            fileInfo.Delete();
        Assert.IsFalse(fileInfo.Exists, fileInfo.ToString());
    }

    private static void Delete(DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists)
            directoryInfo.Delete(true);
        Assert.IsFalse(directoryInfo.Exists, directoryInfo.ToString());
    }

    private static void AssertExists(FileInfo fileInfo)
    {
        fileInfo.Refresh();
        Assert.IsTrue(fileInfo.Exists, fileInfo.ToString());
    }
}