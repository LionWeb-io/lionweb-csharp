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
public class ParamsTests : ParamsTestsBase
{
    [TestMethod]
    public void None()
    {
        var generator = new TestLionWebGenerator();
        var result = generator.Exec([]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("Nothing to generate", generator.Errors);
    }

    [TestMethod]
    public void OnlyLanguageFile()
    {
        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            TestLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("Neither OutputDir nor OutputFile set", generator.Errors);
        Assert.Contains("Neither Namespace nor NamespacePattern set", generator.Errors);
        Assert.Contains("Skipping invalid configuration", generator.Errors);
    }

    [TestMethod]
    public void LanguageFile_NonExistent()
    {
        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "asf"
        ]);

        Assert.IsEmpty(generator.Configurations);
        Assert.IsEmpty(generator.ValidConfigurations);
        Assert.AreEqual(-2, result);

        Assert.Contains("File does not exist: 'asf'.", generator.Errors);
    }

    [TestMethod]
    public void OutputDir_NonExistent()
    {
        var outputDir = new DirectoryInfo("asdf");
        Delete(outputDir);

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--output", outputDir.ToString(),
            "--namespace", "",
            TestLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

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

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--output", outputDir.ToString(),
            "--namespace", "",
            TestLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

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

        var generator = new TestLionWebGenerator();
        var result = generator.Exec([
            "--outputFile", generatedFile.ToString(),
            "--namespace", "",
            TestLanguage2024
        ]);

        Assert.HasCount(1, generator.Configurations);
        Assert.HasCount(1, generator.ValidConfigurations);
        Assert.AreEqual(0, result);

        Assert.IsEmpty(generator.Errors);

        AssertExists(generatedFile);
        Assert.Contains("namespace", File.ReadAllText(generatedFile.FullName));
    }
}