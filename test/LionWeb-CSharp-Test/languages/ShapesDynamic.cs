// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace Examples.Shapes.Dynamic;

using LionWeb.Core;
using LionWeb.Core.M3;

/// <summary>
/// Entrypoint for working with the Shapes example language.
/// </summary>
public static class ShapesDynamic
{
    public static readonly DynamicLanguage Language = GetLanguage(LionWebVersions.Current);

    public static DynamicLanguage GetLanguage(LionWebVersions lionWebVersion) =>
        LanguagesUtils.LoadLanguages("LionWeb-CSharp-Test", lionWebVersion switch
            {
                IVersion2023_1 => "LionWeb_CSharp_Test.languages.defChunks.shapes_2023_1.json",
                IVersion2024_1 => "LionWeb_CSharp_Test.languages.defChunks.shapes_2024_1.json",
                IVersion2024_1_Compatible => "LionWeb_CSharp_Test.languages.defChunks.shapes_2024_1.json",
                _ => throw new UnsupportedVersionException(lionWebVersion)
            }, lionWebVersion)
            .First();
}