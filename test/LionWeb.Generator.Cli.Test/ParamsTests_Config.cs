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

        var relativeOutputDir = DeleteOutDir(out var outputDir);

        var result = generator.Exec([
            "--config", PartialConfig,
            "--output", relativeOutputDir
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
    public void CompleteConfigFile_Override()
    {
        var generator = new TestLionWebGenerator();
        var currDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"CurrentDir: {currDir}");

        var relativeOutputDir = DeleteOutDir(out var outputDir);

        var result = generator.Exec([
            "--config", CompleteConfig,
            "--output", relativeOutputDir,
            "--namespace", "OtherNameSpace",
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
            new Configuration
            {
                LanguageFile = new FileInfo(new FileInfo(Path.Combine(currDir, TestLanguage2023)).FullName),
                OutputDir = new DirectoryInfo(relativeOutputDir),
                Namespace = "OtherNameSpace",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = false }
            }, generator.Configurations[0]);

        Assert.AreEqual(
            new Configuration
            {
                LanguageFile = new FileInfo(TestLanguage2024),
                OutputDir = new DirectoryInfo(relativeOutputDir),
                Namespace = "OtherNameSpace",
                LionWebVersion = "2024.1",
                GeneratorConfig = new GeneratorConfig { WritableInterfaces = false }
            }, generator.Configurations[1]);

        Assert.AreEqual(generator.Configurations[1], generator.ValidConfigurations[0]);

        Assert.IsTrue(Directory.Exists(outputDir));
        AssertExists(new FileInfo($"{outputDir}/TestLanguage.g.cs"));
    }
}