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

public class ParamsTestsBase
{
    private const string _resourceDir = "../../../resources";
    private const string _relativeOutputDir = $"{_resourceDir}/out";

    protected const string TestLanguage2023 = $"{_resourceDir}/testLanguage.2023_1.json";
    protected const string TestLanguage2024 = $"{_resourceDir}/testLanguage.2024_1.json";
    protected const string TestLanguageNamespace = $"{_resourceDir}/testLanguage.namespace.2024_1.json";
    protected const string PartialConfig = $"{_resourceDir}/partial.config.json";
    protected const string CompleteConfig = $"{_resourceDir}/complete.config.json";

    protected static string DeleteOutDir(out string outputDir)
    {
        outputDir = Path.Combine(Directory.GetCurrentDirectory(), _relativeOutputDir);
        if (!Directory.Exists(outputDir))
            return _relativeOutputDir;

        Directory.Delete(outputDir, true);
        Assert.IsFalse(Directory.Exists(outputDir), outputDir);
        return _relativeOutputDir;
    }

    protected static void Delete(FileInfo fileInfo)
    {
        if (fileInfo.Exists)
            fileInfo.Delete();
        Assert.IsFalse(fileInfo.Exists, fileInfo.ToString());
    }

    protected static void Delete(DirectoryInfo directoryInfo)
    {
        if (directoryInfo.Exists)
            directoryInfo.Delete(true);
        Assert.IsFalse(directoryInfo.Exists, directoryInfo.ToString());
    }

    protected static void AssertExists(FileInfo fileInfo)
    {
        fileInfo.Refresh();
        Assert.IsTrue(fileInfo.Exists, fileInfo.ToString());
    }
}