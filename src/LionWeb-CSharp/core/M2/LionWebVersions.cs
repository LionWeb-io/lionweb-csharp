// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M2;

using M3;

public enum LionWebVersions
{
    v2023_1,
    v2024_1
}

public static class LionWebVersionsExtensions
{
    public static string GetVersionString(this LionWebVersions version) => version switch
    {
        LionWebVersions.v2023_1 => "2023.1",
        LionWebVersions.v2024_1 => "2024.1",
        _ => throw new UnsupportedVersionException(version)
    };

    public static LionWebVersions GetCurrent() =>
        LionWebVersions.v2023_1;

    public static IBuiltInsLanguage GetBuiltIns(this LionWebVersions version) =>
        IBuiltInsLanguage.GetInstance(version);

    public static ILionCoreLanguage GetLionCore(this LionWebVersions version) =>
        ILionCoreLanguage.GetInstance(version);
}