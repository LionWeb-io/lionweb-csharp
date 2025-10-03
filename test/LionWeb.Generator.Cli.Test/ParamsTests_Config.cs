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

using Core;

[TestClass]
public class ParamsTests_Config : ParamsTestsBase
{
    [TestMethod]
    public void PartialConfigFile_NoParams()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", PartialConfig]);

        Assert.HasCount(2, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("Neither OutputDir nor OutputFile set", generator.Errors);
        Assert.Contains("Skipping invalid configuration", generator.Errors);
        Assert.Contains(s => s.Contains("LanguageFile doesn't exist:") && s.Contains("missingLanguage.json"),
            generator.Errors);
        Assert.Contains(s => s.Contains("OutputDir doesn't exist:") && s.Contains("out/missing"),
            generator.Errors);
    }

    [TestMethod]
    public void PartialConfigFile_AllRequiredParams()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");

        var outputDir = DeleteOutDir();

        var result = generator.Exec([
            "--config", PartialConfig,
            "--output", RelativeOutputDir
        ]);

        Assert.HasCount(2, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(-1, result);

        Assert.Contains(s => s.Contains("LanguageFile doesn't exist:") && s.Contains("missingLanguage.json"),
            generator.Errors);
        Assert.Contains("OutputDir doesn't exist: ../../../resources/out", generator.Errors);
        Assert.Contains("Skipping invalid configuration", generator.Errors);

        Assert.IsTrue(Directory.Exists(outputDir));
        AssertExists(new FileInfo($"{outputDir}/TestLanguage.g.cs"));
    }

    [TestMethod]
    public void CompleteConfigFile()
    {
        var generator = new TestLionWebGenerator();
        var currDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"CurrentDir: {currDir}");

        DeleteOutDir();

        var outputDir = Path.Combine(currDir, ResourceDir, "out/2023");
        
        var result = generator.Exec([
            "--config", CompleteConfig
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(new FileInfo(Path.Combine(currDir, TestLanguage2023)).FullName),
                OutputDir = new DirectoryInfo(outputDir),
                Namespace = "My.Name.Space",
                PathPattern = PathPattern.NamespaceInFilename,
                DotGSuffix = false,
                LionWebVersion = "2023.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = true }
            }, generator.Configurations[0]);

        Assert.AreEqual(generator.Configurations[0], generator.ValidConfigurations[0]);

        Assert.IsTrue(Directory.Exists(outputDir));
        AssertExists(new FileInfo($"{outputDir}/My.Name.Space.TestLanguage.cs"));
    }
    
    [TestMethod]
    public void CompleteConfigFile_Override()
    {
        var generator = new TestLionWebGenerator();
        var currDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"CurrentDir: {currDir}");

        var outputDir = DeleteOutDir();

        var result = generator.Exec([
            "--config", CompleteConfig,
            "--output", RelativeOutputDir,
            "--namespace", "OtherNameSpace",
            "--pathPattern", nameof(PathPattern.VerbatimName),
            "--dotGSuffix", "true",
            "--lionWebVersion", "2024.1",
            "--writableInterfaces", "false",
            TestLanguage2024
        ]);

        Assert.HasCount(2, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(-1, result);

        Assert.Contains(
            s => s.Contains(
                "LionWeb.Core.VersionMismatchException: Mismatched LionWeb versions: LionWeb.Core.VersionSpecific.V2024_1.Version2024_1 vs. LionWeb.Core.VersionSpecific.V2023_1.Version2023_1"),
            generator.Errors);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(new FileInfo(Path.Combine(currDir, TestLanguage2023)).FullName),
                OutputDir = new DirectoryInfo(RelativeOutputDir),
                Namespace = "OtherNameSpace",
                PathPattern = PathPattern.VerbatimName,
                DotGSuffix = true,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = false }
            }, generator.Configurations[0]);

        Assert.AreEqual(
            new Cli.Configuration
            {
                LanguageFile = new FileInfo(TestLanguage2024),
                OutputDir = new DirectoryInfo(RelativeOutputDir),
                Namespace = "OtherNameSpace",
                PathPattern = PathPattern.VerbatimName,
                DotGSuffix = true,
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = false }
            }, generator.Configurations[1]);

        Assert.AreEqual(generator.Configurations[1], generator.ValidConfigurations[0]);

        Assert.IsTrue(Directory.Exists(outputDir));
        AssertExists(new FileInfo($"{outputDir}/TestLanguage.g.cs"));
    }
    
    [TestMethod]
    public void RelativeLanguageFile()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");

        var outputDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), ResourceDir, "relative/out"));
        Delete(outputDir);

        var languageFile = Path.Combine(Directory.GetCurrentDirectory(), ResourceDir, "relative/relative.config.json");
        var result = generator.Exec([
            "--config", languageFile
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);

        AssertExists(new FileInfo($"{outputDir}/2023.ceeees"));
    }

    [TestMethod]
    public void MultipleLanguages_valid()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");

        var outputDir = DeleteOutDir();
        
        var languageFile = Path.Combine(Directory.GetCurrentDirectory(), ResourceDir, "multipleLanguages-valid.config.json");
        var result = generator.Exec([
            "--config", languageFile
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);

        AssertExists(new FileInfo($@"{outputDir}/2023/My.Name.Space.a.B.cee.Dee.@if.else._0.aBcd.TestLanguage.cs"));
        AssertExists(new FileInfo($@"{outputDir}/2023/My.Name.Space.SecondTestLanguage.cs"));
    }

    [TestMethod]
    public void MultipleLanguages_invalid()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");

        var outputDir = DeleteOutDir();
        
        var languageFile = Path.Combine(Directory.GetCurrentDirectory(), ResourceDir, "multipleLanguages-invalid.config.json");
        var result = generator.Exec([
            "--config", languageFile
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);
        
        Assert.Contains(s => s.Contains("Single output file ") && s.Contains("out/multi.cs set, but language file ") && s.Contains("relative/multipleLanguages.2024_1.json contains more than one language"), generator.Errors);

        Assert.IsFalse(Directory.Exists(outputDir));
    }

    [TestMethod]
    public void Invalid_LanguageFile()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-LanguageFile.config.json"]);

        Assert.HasCount(2, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains(s => s.Contains("LanguageFile doesn't exist:") && s.Contains("missingLanguage.json"),
            generator.Errors);
        Assert.Contains("LanguageFile doesn't exist: null", generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_Output_NoDir()
    {
        DeleteOutDir();
        
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-Output.config.json"]);

        Assert.HasCount(8, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("Neither OutputDir nor OutputFile set", generator.Errors);
        Assert.Contains("PathPattern ignored because OutputFile is set", generator.Errors);
        Assert.Contains("DotGSuffix ignored because OutputFile is set", generator.Errors);
        Assert.Contains(s => s.Contains("Both OutputDir ") && s.Contains(" and OutputFile "),
            generator.Errors);
        Assert.Contains(s => s.Contains("OutputDir doesn't exist: ") && s.Contains("out/"),
            generator.Errors);
        Assert.Contains(s => s.Contains("OutputFile's parent directory doesn't exist: ") && s.Contains("out/file"),
            generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_Output_DirExists()
    {
        if (!Directory.Exists(RelativeOutputDir))
            Directory.CreateDirectory(RelativeOutputDir);
        
        Assert.IsTrue(Directory.Exists(RelativeOutputDir));
        
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-Output.config.json"]);

        Assert.DoesNotContain(s => s.Contains("OutputDir doesn't exist: ") && s.Contains("out/"),
            generator.Errors);
        Assert.DoesNotContain(s => s.Contains("OutputFile's parent directory doesn't exist: ") && s.Contains("out/file"),
            generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_PathPattern_empty()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-PathPattern-empty.config.json"]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains(s => s.Contains("The JSON value could not be converted to System.Nullable`1[LionWeb.Generator.Cli.PathPattern]"),
            generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_PathPattern_unknown()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-PathPattern-unknown.config.json"]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains(s => s.Contains("The JSON value could not be converted to System.Nullable`1[LionWeb.Generator.Cli.PathPattern]"),
            generator.Errors);
    }

    [TestMethod]
    public void Invalid_Namespace()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-Namespace.config.json"]);

        Assert.HasCount(8, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(-1, result);

        Assert.Contains("Not a valid namespace: '!invalid'", generator.Errors);
        Assert.Contains("Neither Namespace nor NamespacePattern set", generator.Errors);
        Assert.Contains("Both Namespace (myNs) and NamespacePattern (DotSeparated) set", generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_NamespacePattern_empty()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-NamespacePattern-empty.config.json"]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains(s => s.Contains("The JSON value could not be converted to System.Nullable`1[LionWeb.Generator.Cli.NamespacePattern]"),
            generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_NamespacePattern_unknown()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-NamespacePattern-unknown.config.json"]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains(s => s.Contains("The JSON value could not be converted to System.Nullable`1[LionWeb.Generator.Cli.NamespacePattern]"),
            generator.Errors);
    }
    
    [TestMethod]
    public void Invalid_LionWebVersion()
    {
        var generator = new TestLionWebGenerator();
        Console.WriteLine($"CurrentDir: {Directory.GetCurrentDirectory()}");
        var result = generator.Exec(["--config", $"{ResourceDir}/invalid-LionWebVersion.config.json"]);

        Assert.HasCount(7, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("Unsupported LionWebVersion: unknown", generator.Errors);
        
        Assert.Contains(c => c.LionWebVersion == LionWebVersions.v2023_1.VersionString, generator.Configurations);
        Assert.Contains(c => c.LionWebVersion == LionWebVersions.v2024_1.VersionString, generator.Configurations);
        Assert.Contains(c => c.LionWebVersion == LionWebVersions.v2025_1.VersionString, generator.Configurations);
    }
}