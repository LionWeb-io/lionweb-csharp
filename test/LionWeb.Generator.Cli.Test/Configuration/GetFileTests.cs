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

namespace LionWeb.Generator.Cli.Test.Configuration;

using Configuration = Cli.Configuration;

[TestClass]
public class GetFileTests : ConfigurationTestsBase
{
    [TestMethod]
    public void OnlyOutputFile()
    {
        var configuration = new Configuration { OutputFile = new FileInfo(@"abc/cde\a.b.c") };

        AssertEquals(@"abc/cde\a.b.c", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void OutputFile_OutputDir()
    {
        var configuration = new Configuration
        {
            OutputFile = new FileInfo(@"abc/cde\a.b.c"), OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"abc/cde\a.b.c", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_VerbatimName_WithG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.VerbatimName, DotGSuffix = true
        };

        AssertEquals(@"myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_VerbatimName_OutputDir()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c",
            PathPattern = PathPattern.VerbatimName,
            DotGSuffix = true,
            OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"out/myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_VerbatimName_NoG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.VerbatimName, DotGSuffix = false
        };

        AssertEquals(@"myLang.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_VerbatimKey_WithG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.VerbatimKey, DotGSuffix = true
        };

        AssertEquals(@"My-lang_key.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_VerbatimKey_OutputDir()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c",
            PathPattern = PathPattern.VerbatimKey,
            DotGSuffix = true,
            OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"out/My-lang_key.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_VerbatimKey_NoG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.VerbatimKey, DotGSuffix = false
        };

        AssertEquals(@"My-lang_key.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceInFilename_WithG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceInFilename, DotGSuffix = true
        };

        AssertEquals(@"a.b.c.myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceInFilename_OutputDir()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c",
            PathPattern = PathPattern.NamespaceInFilename,
            DotGSuffix = true,
            OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"out/a.b.c.myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceInFilename_NoG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceInFilename, DotGSuffix = false
        };

        AssertEquals(@"a.b.c.myLang.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceAsFilename_WithG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceAsFilename, DotGSuffix = true
        };

        AssertEquals(@"a.b.c.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceAsFilename_OutputDir()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c",
            PathPattern = PathPattern.NamespaceAsFilename,
            DotGSuffix = true,
            OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"out/a.b.c.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceAsFilename_NoG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceAsFilename, DotGSuffix = false
        };

        AssertEquals(@"a.b.c.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceInPath_WithG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceInPath, DotGSuffix = true
        };

        AssertEquals(@"a/b/c/myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceInPath_OutputDir()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c",
            PathPattern = PathPattern.NamespaceInPath,
            DotGSuffix = true,
            OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"out/a/b/c/myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceInPath_NoG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceInPath, DotGSuffix = false
        };

        AssertEquals(@"a/b/c/myLang.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceAsPath_WithG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceAsPath, DotGSuffix = true
        };

        AssertEquals(@"a/b/c.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceAsPath_OutputDir()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c",
            PathPattern = PathPattern.NamespaceAsPath,
            DotGSuffix = true,
            OutputDir = new DirectoryInfo("out")
        };

        AssertEquals(@"out/a/b/c.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void Pattern_NamespaceAsPath_NoG()
    {
        var configuration = new Configuration
        {
            Namespace = "a.b.c", PathPattern = PathPattern.NamespaceAsPath, DotGSuffix = false
        };

        AssertEquals(@"a/b/c.cs", configuration.GetFile(LanguageNoDots()));
    }

    [TestMethod]
    public void UnknownPathPattern_Null()
    {
        var ex = Assert.Throws<NullReferenceException>(() =>
        {
            object pathPattern = null;
            var configuration = new Configuration { Namespace = "ns", PathPattern = (PathPattern)pathPattern };
            return configuration.GetFile(LanguageNoDots());
        });
    }

    [TestMethod]
    public void UnknownPathPattern_InvalidInt()
    {
        var configuration = new Configuration { Namespace = "ns", PathPattern = (PathPattern)int.MaxValue };
        var ex = Assert.Throws<UnknownEnumValueException<PathPattern>>(() => configuration.GetFile(LanguageNoDots()));

        Assert.AreEqual($"Unknown PathPattern: {int.MaxValue}", ex.Message);
    }

    [TestMethod]
    public void PathPattern_ValidInt()
    {
        var configuration = new Configuration { Namespace = "ns", PathPattern = (PathPattern)1 };
        AssertEquals("myLang.g.cs", configuration.GetFile(LanguageNoDots()));
    }

    private void AssertEquals(string expected, FileInfo actual) =>
        Assert.AreEqual(new FileInfo(expected).FullName, actual.FullName);
}