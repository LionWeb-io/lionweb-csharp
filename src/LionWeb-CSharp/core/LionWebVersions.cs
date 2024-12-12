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

namespace LionWeb.Core;

using M2;
using M3;
using VersionSpecific.V2023_1;
using VersionSpecific.V2024_1;

public interface LionWebVersions
{
    public string VersionString { get; }
    public IBuiltInsLanguage BuiltIns { get; }
    public ILionCoreLanguage LionCore { get; }

    public static LionWebVersions Current => v2023_1;

    public static readonly IVersion2023_1 v2023_1 = Version2023_1.Instance;
    public interface IVersion2023_1 : LionWebVersions;
    
    public interface IVersion2024_1 : LionWebVersions;
    public static readonly IVersion2024_1 v2024_1 = Version2024_1.Instance;
}

internal abstract class VersionBase<TBuiltIns, TLionCore> : LionWebVersions where TBuiltIns : IBuiltInsLanguage where TLionCore : ILionCoreLanguage
{
    public abstract string VersionString { get; }
    
    IBuiltInsLanguage LionWebVersions.BuiltIns { get => BuiltIns; }
    public abstract TBuiltIns BuiltIns { get; }
    
    ILionCoreLanguage LionWebVersions.LionCore { get => LionCore; }
    public abstract TLionCore LionCore { get; }
    
}