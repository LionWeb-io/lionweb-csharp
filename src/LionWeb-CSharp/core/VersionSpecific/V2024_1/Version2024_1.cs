﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

// ReSharper disable InconsistentNaming
namespace LionWeb.Core.VersionSpecific.V2024_1;

internal sealed class Version2024_1 : VersionBase<IBuiltInsLanguage_2024_1, ILionCoreLanguage_2024_1>,
    LionWebVersions.IVersion2024_1
{
    internal static readonly Version2024_1 Instance = new Lazy<Version2024_1>(() => new()).Value;
    private Version2024_1() { }

    public override string VersionString => "2024.1";

    public override IBuiltInsLanguage_2024_1 BuiltIns =>
        new Lazy<IBuiltInsLanguage_2024_1>(() => BuiltInsLanguage_2024_1.Instance).Value;

    public override ILionCoreLanguage_2024_1 LionCore =>
        new Lazy<ILionCoreLanguage_2024_1>(() => LionCoreLanguage_2024_1.Instance).Value;
}